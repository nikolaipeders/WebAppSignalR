using Microsoft.AspNetCore.SignalR;

namespace WebAppSignalR.Server.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendMove(string message)
        {
            await Clients.All.SendAsync("Logic", message);
        }
        
    }
}
