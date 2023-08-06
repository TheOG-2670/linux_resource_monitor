using LinuxResourceMonitorApi;
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

Dictionary<WebSocket, DataDTO?> websocketClients = new Dictionary<WebSocket, DataDTO?>();


app.UseWebSockets(webSocketOptions);
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            websocketClients.Add(await context.WebSockets.AcceptWebSocketAsync(), null);
            foreach(KeyValuePair<WebSocket, DataDTO?> websocket in websocketClients)
            {
                await Echo(websocket.Key);
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

async Task Echo(WebSocket webSocket)
{
    var buffer = new byte[500];
    WebSocketReceiveResult result;
    DataDTO? r = new DataDTO();

    Console.WriteLine("client connected");
    do
    {
        result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Text)
        {
            r = JsonSerializer.Deserialize<DataDTO>(Encoding.ASCII.GetString(buffer, 0, result.Count));
            if (webSocket == websocketClients.FirstOrDefault(x => x.Key== webSocket).Key)
            {
                websocketClients.Remove(websocketClients.FirstOrDefault(x => x.Key == webSocket).Key);
                websocketClients.TryAdd(webSocket, r);
                Console.WriteLine($"{r}:\nuser id: {r.userId}, message: {r.message}");
                Console.WriteLine(websocketClients.FirstOrDefault(x => x.Value == r).Value?.ToString());
                await webSocket.SendAsync(Encoding.ASCII.GetBytes(r.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
    while (!result.CloseStatus.HasValue);
    Console.WriteLine("client disconnected");
    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "connection terminated", CancellationToken.None);
}

app.Run();
