using Microsoft.AspNetCore.SignalR;

namespace BasicWeigh.Web.Hubs;

public class ScaleHub : Hub
{
    /// <summary>
    /// Called by remote print agents (e.g. Raspberry Pi) to join the PrintClients group.
    /// </summary>
    public async Task JoinPrintGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "PrintClients");
    }
}
