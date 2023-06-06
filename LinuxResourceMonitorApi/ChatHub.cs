using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace LinuxResourceMonitorApi
{
    public class ChatHub : Hub //class that handles client-server communications
    {
        public async Task SendMessage(ParamRequest req) //called by client to send message to 'All' clients
        {
            Console.WriteLine($"{req.userId} : {req.message}");
            var j = JsonSerializer.Deserialize<ParamRequest>(req.message);
            await Clients.All.SendAsync("ReceiveMessage", req);
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
