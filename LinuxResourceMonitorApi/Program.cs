using LinuxResourceMonitorApi;

var builder = WebApplication.CreateBuilder();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

CpuInfo currentInfo=new CpuInfo();

app.MapGet("/resourcemonitor", () =>
{
    return currentInfo;
})
.WithName("GetResourceMonitor")
.WithOpenApi();

app.MapPost("/resourcemonitor", (CpuInfo info) =>
{
    currentInfo=info;
    return Results.NoContent();
})
.WithName("PostResourceMonitor")
.WithOpenApi();

app.MapHub<ChatHub>("/chatHub");

app.Run();
