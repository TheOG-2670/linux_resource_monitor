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
            var buffer = new byte[256];
            while (true)
            {
                var result=await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                ParamRequest? r=JsonSerializer.Deserialize<ParamRequest>(Encoding.ASCII.GetString(buffer, 0, result.Count));
                Console.WriteLine(r);
                await webSocket.SendAsync(Encoding.ASCII.GetBytes(r.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
                await Task.Delay(1000);
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

await app.RunAsync();
