using Application.Config;
using Application.Services;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace Console.App;

public class Simulation
{
    private readonly IElevatorService _elevatorService;
    private readonly IRequestGenerator _requestGenerator;
    private readonly IClock _clock;
    private readonly SimulationOptions _options;

    public Simulation(
        IElevatorService elevatorService,
        IRequestGenerator requestGenerator,
        IClock clock,
        IOptions<SimulationOptions> options)
    {
        _elevatorService = elevatorService;
        _requestGenerator = requestGenerator;
        _clock = clock;
        _options = options.Value;
    }

    public void Run()
    {
        for (int i = 0; i < _options.TotalTicks; i++)
        {
            var now = _clock.Now;

            // Generate new hall calls for this tick.
            var calls = _requestGenerator.Generate(now).ToList();

            // Assign hall calls to elevators and log assignments.
            _elevatorService.AcceptHallCalls(calls, now);

            // Log the status of all elevators for this tick.
            _elevatorService.PrintElevatorStatus(now);
            AnsiConsole.Write(new Rule());

            // Advance all elevators by one tick.
            _elevatorService.Tick(now, _clock.TickSize);

            // Advance the simulation clock to the next tick.
            _clock.Advance();
        }
    }
}