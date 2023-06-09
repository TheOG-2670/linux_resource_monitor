using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinuxResourceMonitorApi
{
    public class Client
    {
        public static async Task Main(string[] args)
        {
            Uri uri = new Uri("ws://localhost:5227/ws");
            ClientWebSocket ws = new ClientWebSocket();
            await ws.ConnectAsync(uri, CancellationToken.None);
            byte[] buffer = new byte[256];
            while (ws.State == WebSocketState.Open)
            {
                ParamRequest paramRequest= new ParamRequest();
                paramRequest.userId = ".net client";
                paramRequest.message = JsonSerializer.Serialize(new CpuInfo(1000));

                string message = JsonSerializer.Serialize(paramRequest);
                await ws.SendAsync(Encoding.ASCII.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                }
                else
                {
                    Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
                }
            }
        }

    }
}
