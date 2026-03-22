using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BasicWeigh.Web.Data;
using BasicWeigh.Web.Hubs;

namespace BasicWeigh.Web.Services;

public class ScaleBroadcastService : BackgroundService
{
    private readonly IHubContext<ScaleHub> _hub;
    private readonly IScaleService _scale;
    private readonly IServiceProvider _services;
    private readonly PrintQueueService _printQueue;

    // Cache DemoMode to avoid DB hits every 250ms
    private bool _demoMode = true;
    private bool _scalePrintsTicket = false;
    private DateTime _lastSettingsCheck = DateTime.MinValue;

    public ScaleBroadcastService(IHubContext<ScaleHub> hub, IScaleService scale,
        IServiceProvider services, PrintQueueService printQueue)
    {
        _hub = hub;
        _scale = scale;
        _services = services;
        _printQueue = printQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Refresh settings from DB once per second
            if ((DateTime.UtcNow - _lastSettingsCheck).TotalSeconds >= 1)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ScaleDbContext>();
                    var setup = await db.AppSetup.FirstAsync(stoppingToken);
                    _demoMode = setup.DemoMode;
                    _scalePrintsTicket = setup.ScalePrintsTicket;
                    _lastSettingsCheck = DateTime.UtcNow;
                }
                catch { /* DB not ready yet — keep last known values */ }
            }

            // 5-second timeout: if not in demo mode and scale hasn't reported, flag error
            if (!_demoMode && _scale is SimulatedScaleService sim)
            {
                if (sim.LastUpdate.HasValue &&
                    (DateTime.UtcNow - sim.LastUpdate.Value).TotalSeconds > 5)
                {
                    sim.SetWeight(0);
                    sim.SetMotion(false);
                    sim.SetError(true);
                }
            }

            var data = new
            {
                weight = _scale.GetCurrentWeight(),
                motion = _scale.IsInMotion(),
                error = _scale.HasError(),
                ok = _scale.IsConnected()
            };

            await _hub.Clients.All.SendAsync("ScaleUpdate", data, stoppingToken);

            // Dispatch pending print jobs via SignalR to PrintClients group
            if (_scalePrintsTicket && _printQueue.HasPending && _scale is SimulatedScaleService s2)
            {
                // Only dispatch if scale is alive (updated within last 2 seconds)
                if (s2.LastUpdate.HasValue &&
                    (DateTime.UtcNow - s2.LastUpdate.Value).TotalSeconds < 2)
                {
                    if (_printQueue.TryDequeue(out var ticketId) && ticketId != null)
                    {
                        await _hub.Clients.Group("PrintClients").SendAsync("PrintTicket",
                            new { ticketId, pdfUrl = $"/api/ticket/{ticketId}/pdf" },
                            stoppingToken);
                    }
                }
            }

            await Task.Delay(250, stoppingToken);
        }
    }
}
