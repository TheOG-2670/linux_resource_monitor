using LinuxResourceMonitorApi;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();

var app = builder.Build();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await Echo(webSocket);
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

static async Task Echo(WebSocket webSocket)
{
    var buffer = new byte[500];
    WebSocketReceiveResult result;
    DataDTO? r = new DataDTO();
    do
    {
        result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Text)
        {
            r = JsonSerializer.Deserialize<DataDTO>(Encoding.ASCII.GetString(buffer, 0, result.Count));
            Console.WriteLine(r);
            await webSocket.SendAsync(Encoding.ASCII.GetBytes(r.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
    while (!result.CloseStatus.HasValue);
    Console.WriteLine($"{r.userId} disconnected");
    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "connection terminated", CancellationToken.None);
}

app.Run();
