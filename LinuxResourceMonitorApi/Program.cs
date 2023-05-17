var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

Queue<CpuInfo> CurrentFrequency = new Queue<CpuInfo>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/resourcemonitor", () =>
{
    if(CurrentFrequency.Count > 0)
    {
        return Results.Ok(CurrentFrequency);
    }
    else
    {
        return Results.Problem("Queue is empty!", null, 400);
    }
})
.WithName("GetResourceMonitor")
.WithOpenApi();

app.MapPost("/resourcemonitor", async (HttpRequest request) =>
{
    CpuInfo? info= await request.ReadFromJsonAsync<CpuInfo>();
    if(info != null) 
    {
        CurrentFrequency.Enqueue(info);
        return Results.Created("/resourcemonitor", CurrentFrequency);
    }
    return Results.BadRequest();
})
.WithName("PostResourceMonitor")
.WithOpenApi();

app.Run();

internal record CpuInfo
{
    public long Frequency { get; set; }
}