using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinuxResourceMonitorApi
{
    public class Client
    {
        private static HubConnection _connection;
        private const string _url = "http://localhost:5227/chatHub";

        public static void SendConsoleKeyInfo()
        {
            ConsoleKeyInfo cki = Console.ReadKey();
            Send("testuser", cki.KeyChar.ToString());
            /*if (cki.KeyChar == 'q') stop = true;*/
        }

        public static void Main(string[] args)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(_url)
                .Build();

            _connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection.StartAsync();
            };

            try
            {
                _connection.StartAsync();
                while (true)
                {
                    CpuInfo info = new CpuInfo();
                    info.Frequency = new Random().Next(0, 10000);
                    var j = JsonSerializer.Serialize(info);
                    Console.WriteLine(j);
                    Send("testuser", j);
                    Thread.Sleep(1000);
                    //SendConsoleKeyInfo();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }

            _connection.On<string>("ReceiveMessage", (message) =>
            {
                CpuInfo? a = JsonSerializer.Deserialize<CpuInfo>(message);
                Console.WriteLine($"message: {a?.Frequency}");
            });
        }

        private static async void Send(string user, string msg)
        {
            try
            {
                await _connection.InvokeAsync("SendMessage", user, msg);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }

    }
}
