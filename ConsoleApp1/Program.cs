using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinuxResourceMonitorApi
{
    public class Client
    {
        private static HubConnection _connection;
        private const string _url = "http://localhost:5227/chatHub";

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

            _connection.On<ParamRequest>("ReceiveMessage", (req) =>
            {
                Console.WriteLine($"message: {req.message}");
            });

            try
            {
                _connection.StartAsync();
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                Thread a = new Thread(() => RunInBg(tokenSource.Token));
                a.Start();
                if (Console.ReadKey().KeyChar == 'q')
                {
                    tokenSource.Cancel();
                    a.Join();
                    tokenSource.Dispose();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }

        public static void RunInBg(CancellationToken tokenSource)
        {
            while (true)
            {
                if (tokenSource.IsCancellationRequested) break;
                /*Console.WriteLine(serialized);*/
                Send(".net client", JsonSerializer.Serialize(new CpuInfo { Frequency=100 }));
                Thread.Sleep(1000);
            }
        }

        private static async void Send(string user, string msg)
        {
            try
            {
                await _connection.InvokeAsync("SendMessage", new ParamRequest { userId=user, message=msg });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }

    }
}
