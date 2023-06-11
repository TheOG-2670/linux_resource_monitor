using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinuxResourceMonitorApi
{
    public class Client
    {
        static ClientWebSocket ws = new ClientWebSocket();
        static CancellationTokenSource cts = new CancellationTokenSource();
        public static async Task Main(string[] args)
        {
            Uri uri = new Uri(args[0]);
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            try
            {
                await ws.ConnectAsync(uri, cts.Token);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine(ex.Message);
            }

            await GetResponse();
            Console.WriteLine("connection terminated");
        }

        public static async Task GetResponse()
        {

            byte[] buffer = new byte[256];
            while (ws.State == WebSocketState.Open)
            {
                ParamRequest paramRequest = new ParamRequest()
                {
                    userId = ".net client",
                    message = JsonSerializer.Serialize(new CpuInfo(new Random().Next(0, 1000)))
                };

                string message = JsonSerializer.Serialize(paramRequest);

                try
                {
                    await ws.SendAsync(Encoding.ASCII.GetBytes(message), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, cts.Token);
                    var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                    Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseOutputAsync(ws.CloseStatus.Value, string.Empty, CancellationToken.None);
                        break;
                    }
                }
                catch (Exception ex) when (ex is WebSocketException || ex is TaskCanceledException)
                {
                    break;
                }
            }
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

    }
}
