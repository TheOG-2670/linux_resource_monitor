using LinuxResourceMonitorApi;

var builder = WebApplication.CreateBuilder();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

var app = builder.Build();


CpuInfo currentInfo=new CpuInfo();

app.MapGet("/resourcemonitor", () =>
{
    return currentInfo;
})
.WithName("GetResourceMonitor");

app.MapPost("/resourcemonitor", (CpuInfo info) =>
{
    currentInfo = info;
    return Results.NoContent();
})
.WithName("PostResourceMonitor");

app.MapHub<ChatHub>("/chatHub");

app.Run();
