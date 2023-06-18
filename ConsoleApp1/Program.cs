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
        /**
         * class variables accessible to Main and GetResponse methods
         */
        static readonly ClientWebSocket ws = new ClientWebSocket();
        //
        public static async Task Main(string[] args)
        {
            /**
             * Asynchronously establish a connection to the websocket API and wait to receive the response
             * in the GetResponse method.
             */
            Uri uri = new Uri(args[0]);
            try
            {
                await ws.ConnectAsync(uri, CancellationToken.None);
                int num = 0;
                while(ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseSent) 
                {
                    string message=Console.ReadLine();
                    await SendMessageAsync(message);
                    string response=await GetResponse();
                    Console.WriteLine(response);
                }
                await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "closed", CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("connection terminated");
        }

        public static async Task SendMessageAsync(string message)
        {
            DataDTO dto = new DataDTO()
            {
                userId = ".net client",
                message = message
            };

            string serializedMessage = JsonSerializer.Serialize(dto);
            await ws.SendAsync(Encoding.ASCII.GetBytes(serializedMessage), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
        }

        public static async Task<string> GetResponse()
        {
            /**
             * While the connection is open, asynchronously send data to the server and print the response when 
             * received. If the response includes a flag to close the connection, terminate the connection and 
             * break out of the loop.
             */

            byte[] buffer = new byte[256];
            string response="";
            try
            {
                var result = await ws.ReceiveAsync(buffer, CancellationToken.None);

                Console.WriteLine("response from server:");
                response = Encoding.ASCII.GetString(buffer, 0, result.Count);
            }
            catch (Exception ex) when (ex is WebSocketException || ex is TaskCanceledException)
            {
                Console.WriteLine($"exception:\n {ex.Message}");
            }
            return response;
        }

    }
}
