using LinuxResourceMonitorApi;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

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
            var websocket = await context.WebSockets.AcceptWebSocketAsync();
            await Echo(websocket);
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

            Console.WriteLine($"{r}:\nuser id: {r.userId}, message: {r.message}");
            await webSocket.SendAsync(Encoding.ASCII.GetBytes(r.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);

        }
    }
    while (!result.CloseStatus.HasValue);

    Console.WriteLine("client disconnected");
    await webSocket.CloseAsync(result.CloseStatus.Value, "connection terminated", CancellationToken.None);
}

app.Run();
