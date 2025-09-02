using Application.Config;
using Application.Domain;
using Application.Domain.Enums;
using Application.Policies;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace Application.Services;
public class ElevatorService : IElevatorService
{
    private readonly List<Elevator> _elevators;
    private readonly IDispatchPolicy _dispatchPolicy;
    private readonly SimulationOptions _options;
    private readonly HashSet<(int, DirectionEnum)> _outstandingCalls = new();
    public IReadOnlyList<Elevator> Elevators => _elevators;

    public ElevatorService(IOptions<SimulationOptions> options, IDispatchPolicy dispatchPolicy)
    {
        _options = options.Value;
        _dispatchPolicy = dispatchPolicy;

        // Create elevator cars, all starting at floor 1.
        _elevators = Enumerable.Range(1, _options.Cars)
            .Select(i => new Elevator(i, 1, 1, _options.Floors, _options.StopDurationSeconds / _options.TickSeconds))
            .ToList();
    }

    public void AcceptHallCalls(IEnumerable<HallCall> calls, TimeSpan now)
    {
        foreach (var call in calls)
        {
            var key = (call.Floor, call.Direction);
            if (_outstandingCalls.Contains(key))
                continue;

            _outstandingCalls.Add(key);

            var callPanel = new Panel(
                $"[bold yellow]CALL RECEIVED[/]\n" +
                $"[white]Time:[/] [bold]{now.TotalSeconds}s[/]\n" +
                $"[white]Floor:[/] [bold]{call.Floor}[/]\n" +
                $"[white]Direction:[/] [bold]{(call.Direction == DirectionEnum.Up ? "Up" : "Down")}[/]"
            ).BorderColor(Color.Yellow);
            AnsiConsole.Write(callPanel);

            var elevator = _dispatchPolicy.Pick(_elevators, call, now);
            elevator.AddHallCall(call);

            int eta = Math.Abs(elevator.CurrentFloor - call.Floor) * _options.SecondsPerFloor;
            var assignPanel = new Panel(
                $"[bold green]ASSIGNMENT[/]\n" +
                $"[white]Time:[/] [bold]{now.TotalSeconds}s[/]\n" +
                $"[white]Car:[/] [bold]E{elevator.Id}[/]\n" +
                $"[white]Target Floor:[/] [bold]{call.Floor}[/]\n" +
                $"[white]Direction:[/] [bold]{(call.Direction == DirectionEnum.Up ? "Up" : "Down")}[/]\n" +
                $"[white]ETA:[/] [bold]{eta}s[/]"
            ).BorderColor(Color.Green);
            AnsiConsole.Write(assignPanel);
        }
    }

    public void Tick(TimeSpan now, TimeSpan tickSize)
    {
        foreach (var elevator in _elevators)
        {
            elevator.Tick(now, tickSize);
            // Remove served calls from outstanding set when elevator is dwelling at a stop.
            if (elevator.State == ElevatorStateEnum.Dwelling && elevator.ShouldStopHere())
            {
                _outstandingCalls.Remove((elevator.CurrentFloor, DirectionEnum.Up));
                _outstandingCalls.Remove((elevator.CurrentFloor, DirectionEnum.Down));
            }
        }
    }

    public string GetStatusLine(TimeSpan now)
    {
        var status = string.Join(" | ", _elevators.Select(e =>
            $"E{e.Id}: F{e.CurrentFloor} {e.Direction} {e.State}"));
        return $"t={now.TotalSeconds}s | {status}";
    }

    public void PrintElevatorStatus(TimeSpan now)
    {
        var table = new Table();
        table.AddColumn("Car");
        table.AddColumn("Current floor");
        table.AddColumn("Direction");
        table.AddColumn("State");
        table.AddColumn("Stops in same direction");

        foreach (var e in _elevators)
        {
            table.AddRow(
                $"{e.Id}",
                $"{e.CurrentFloor}",
                e.Direction.ToString(),
                e.State.ToString(),
                string.Join(",", e.Direction == DirectionEnum.Up ? e.UpStops : e.DownStops)
            );
        }

        AnsiConsole.MarkupLine($"[bold yellow]t={now.TotalSeconds}s[/]");
        AnsiConsole.Write(table);
    }

    public void Reset()
    {
        _elevators.Clear();
        _outstandingCalls.Clear();
        // Recreate elevator cars, all starting at floor 1.
        var newElevators = Enumerable.Range(1, _options.Cars)
            .Select(i => new Elevator(i, 1, 1, _options.Floors, _options.StopDurationSeconds / _options.TickSeconds))
            .ToList();
        _elevators.AddRange(newElevators);
    }
}