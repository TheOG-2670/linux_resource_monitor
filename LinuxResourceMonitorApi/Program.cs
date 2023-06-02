using LinuxResourceMonitorApi;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSignalR();

var app = builder.Build();


CpuInfo currentInfo=new CpuInfo();

app.MapGet("/resourcemonitor", () =>
{
    return currentInfo;
});

app.MapPost("/resourcemonitor", (CpuInfo info) =>
{
    currentInfo = info;
    return Results.NoContent();
});

app.MapHub<ChatHub>("/chatHub");

app.Run();
