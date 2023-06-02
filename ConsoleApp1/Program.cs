using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinuxResourceMonitorApi
{
    public class Client
    {
        private static HubConnection _connection;

        public static void SendConsoleKeyInfo()
        {
            ConsoleKeyInfo cki = Console.ReadKey();
            /*Send("testuser", Console.ReadLine());*/
            Send("testuser", cki.KeyChar.ToString());
            /*if (cki.KeyChar == 'q') stop = true;*/
        }

        public static void Main(string[] args)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5227/chatHub")
                .Build();

            _connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection.StartAsync();
            };

            _connection.On<string>("ReceiveMessage", (message) =>
            {
                Console.WriteLine($"message: {message}");
            });

            try
            {
                _connection.StartAsync();
                while (true)
                {
                    var j = JsonSerializer.Serialize(
                        new CpuInfo().Frequency=new Random().Next(0, 10000)
                    );
                    var a = Console.ReadLine();
                    Send("testuser", a);
                    //SendConsoleKeyInfo();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
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
