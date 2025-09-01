using Application.Config;
using Microsoft.Extensions.Options;

namespace Console.App;
public class SimulationClock : IClock
{
    private TimeSpan _now = TimeSpan.Zero;
    public TimeSpan Now => _now;
    public TimeSpan TickSize { get; }

    public SimulationClock(IOptions<SimulationOptions> options)
    {
        var simulationOptions = options.Value;
        TickSize = TimeSpan.FromSeconds(simulationOptions.TickSeconds);
    }

    public void Advance()
    {
        _now += TickSize;
    }
}