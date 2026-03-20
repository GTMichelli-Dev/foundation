using Microsoft.AspNetCore.SignalR;
using BasicWeigh.Web.Hubs;

namespace BasicWeigh.Web.Services;

public class ScaleBroadcastService : BackgroundService
{
    private readonly IHubContext<ScaleHub> _hub;
    private readonly IScaleService _scale;

    public ScaleBroadcastService(IHubContext<ScaleHub> hub, IScaleService scale)
    {
        _hub = hub;
        _scale = scale;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var data = new
            {
                weight = _scale.GetCurrentWeight(),
                motion = _scale.IsInMotion(),
                error = _scale.HasError(),
                ok = _scale.IsConnected()
            };

            await _hub.Clients.All.SendAsync("ScaleUpdate", data, stoppingToken);
            await Task.Delay(250, stoppingToken);
        }
    }
}
