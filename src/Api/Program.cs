using Application.Config;
using Application.Policies;
using Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SimulationOptions>(builder.Configuration.GetSection("SimulationOptions"));
builder.Services.AddSingleton<IDispatchPolicy, DispatchPolicy>();
builder.Services.AddSingleton<IElevatorService, ElevatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
