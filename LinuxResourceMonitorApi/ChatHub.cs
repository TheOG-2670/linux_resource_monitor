using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace LinuxResourceMonitorApi
{
    public class ChatHub : Hub //class that handles client-server communications
    {
        public async Task SendMessage(string user, string message) //called by client to send message to 'All' clients
        {
            var j = JsonSerializer.Deserialize<CpuInfo>(message);
            Console.WriteLine(j);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
