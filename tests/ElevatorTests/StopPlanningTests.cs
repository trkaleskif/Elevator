using Application.Domain;
using Application.Domain.Enums;

namespace ElevatorTests;
public class StopPlanningTests
{
    [Fact]
    public void UpStops()
    {
        var elevator = new Elevator(1, 1, 1, 10, 1);
        elevator.AddHallCall(new HallCall(3, DirectionEnum.Up, TimeSpan.Zero));
        elevator.AddHallCall(new HallCall(5, DirectionEnum.Up, TimeSpan.Zero));
        Assert.Equal(new SortedSet<int> { 3, 5 }, elevator.UpStops);
    }

    [Fact]
    public void DownStops()
    {
        var elevator = new Elevator(1, 10, 1, 10, 1);
        elevator.AddHallCall(new HallCall(8, DirectionEnum.Down, TimeSpan.Zero));
        elevator.AddHallCall(new HallCall(6, DirectionEnum.Down, TimeSpan.Zero));
        Assert.Equal(new SortedSet<int> { 8, 6 }, elevator.DownStops);
    }

    [Fact]
    public void DuplicateStopsAreDeduplicated()
    {
        var elevator = new Elevator(1, 1, 1, 10, 1);
        elevator.AddHallCall(new HallCall(3, DirectionEnum.Up, TimeSpan.Zero));
        elevator.AddHallCall(new HallCall(3, DirectionEnum.Up, TimeSpan.Zero));
        Assert.Single(elevator.UpStops);
    }

    [Fact]
    public void ImmediateStopOnCurrentFloor()
    {
        var elevator = new Elevator(1, 5, 1, 10, 1);
        elevator.AddHallCall(new HallCall(5, DirectionEnum.Up, TimeSpan.Zero));
        Assert.Equal(ElevatorStateEnum.Dwelling, elevator.State);
    }
}