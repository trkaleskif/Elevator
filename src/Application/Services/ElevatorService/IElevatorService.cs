using Application.Domain;

namespace Application.Services;
public interface IElevatorService
{
    IReadOnlyList<Elevator> Elevators { get; }
    void AcceptHallCalls(IEnumerable<HallCall> calls, TimeSpan now);
    void Tick(TimeSpan now, TimeSpan tickSize);
    string GetStatusLine(TimeSpan now);
    void PrintElevatorStatus(TimeSpan now);
    void Reset();
}