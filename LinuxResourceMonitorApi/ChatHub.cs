using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace LinuxResourceMonitorApi
{
    public class ChatHub : Hub //class that handles client-server communications
    {
        public async Task SendMessage(string user, string message) //called by client to send message to 'All' clients
        {
            var j = JsonSerializer.Deserialize<object>(message);
            Console.WriteLine(j);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("client disconnected");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
