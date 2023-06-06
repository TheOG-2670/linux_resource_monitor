using LinuxResourceMonitorApi;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<ChatHub>("/chatHub");

app.Run();
