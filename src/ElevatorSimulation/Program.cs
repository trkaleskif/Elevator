using Application.Config;
using Application.Policies;
using Application.Services;
using Console.App;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables(); // This now works with the correct using directive
        var configuration = builder.Build();

        var services = new ServiceCollection();
        services.Configure<SimulationOptions>(configuration.GetSection("SimulationOptions"));
        services.AddSingleton<IDispatchPolicy, DispatchPolicy>();
        services.AddSingleton<IRequestGenerator, RandomRequestGenerator>();
        services.AddSingleton<IClock, SimulationClock>();
        services.AddLogging(config => config.AddConsole());
        services.AddSingleton<IElevatorService, ElevatorService>();
        services.AddSingleton<Simulation>();

        var provider = services.BuildServiceProvider();
        var sim = provider.GetRequiredService<Simulation>();
        sim.Run();
    }
}