using LinuxResourceMonitorApi;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

Dictionary<WebSocket, DataDTO> clients = new();

app.UseWebSockets(webSocketOptions);
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var websocket = await context.WebSockets.AcceptWebSocketAsync();
            if (websocket!=null && context.Request.Query!=null)
            {
                string clientId = context.Request.Query["ClientId"].ToString();
                Console.WriteLine(clientId);
                if(!clients.ContainsKey(websocket))
                {
                    clients.Add(websocket, new DataDTO(clientId));
                }

                await Echo(websocket);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next(context);
    }

});

async Task Broadcast(byte[] buffer, WebSocketReceiveResult result)
{
    var otherWebsockets=from client in clients where client.Value.UserId is not "sender" select client;
    foreach (var client in otherWebsockets.AsEnumerable())
    {
        DataDTO? r = client.Value;
        if (r != null)
        {
            r.Message = JsonSerializer.Deserialize<CPUInfo>(Encoding.ASCII.GetString(buffer, 0, result.Count))?.ToString();
        }
        string data = JsonSerializer.Serialize(r);
        await client.Key.SendAsync(Encoding.ASCII.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}

async Task Echo(WebSocket webSocket)
{
    var buffer = new byte[500];
    WebSocketReceiveResult result;
    Console.WriteLine("client connected");

    do
    {
        result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Text)
        {
            await Broadcast(buffer, result);
        }
    }
    while (!result.CloseStatus.HasValue);

    clients.Remove(webSocket);
    Console.WriteLine("client disconnected");
    await webSocket.CloseAsync(result.CloseStatus.Value, "connection terminated", CancellationToken.None);
}

app.Run();
