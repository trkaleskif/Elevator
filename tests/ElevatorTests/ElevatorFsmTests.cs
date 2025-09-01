using Application.Domain;
using Application.Domain.Enums;

namespace ElevatorTests;
public class ElevatorFsmTests
{
    [Fact]
    public void IdleToMovingOnCall()
    {
        var elevator = new Elevator(1, 1, 1, 10, 1);
        elevator.AddHallCall(new HallCall(5, DirectionEnum.Up, TimeSpan.Zero));
        elevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10));
        Assert.Equal(ElevatorStateEnum.Moving, elevator.State);
        Assert.Equal(DirectionEnum.Up, elevator.Direction);
    }

    [Fact]
    public void MovingAdvancesOneFloorPerTick()
    {
        var elevator = new Elevator(1, 1, 1, 10, 1);
        elevator.AddHallCall(new HallCall(3, DirectionEnum.Up, TimeSpan.Zero));
        elevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10));
        elevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10));
        elevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10));
        Assert.Equal(3, elevator.CurrentFloor);
    }

    [Fact]
    public void ArrivingTriggersDwell()
    {
        var elevator = new Elevator(1, 1, 1, 10, 1);
        elevator.AddHallCall(new HallCall(2, DirectionEnum.Up, TimeSpan.Zero));
        elevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Move to 2
        elevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Dwell
        Assert.Equal(ElevatorStateEnum.Dwelling, elevator.State);
    }

    [Fact]
    public void DirectionFlipsOnlyWhenStopsExhausted()
    {
        var elevator = new Elevator(1, 1, 1, 10, 1);
        elevator.AddHallCall(new HallCall(3, DirectionEnum.Up, TimeSpan.Zero));
        elevator.AddHallCall(new HallCall(1, DirectionEnum.Down, TimeSpan.Zero));
        elevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Move to 2
        elevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Move to 3
        elevator.AddHallCall(new HallCall(1, DirectionEnum.Down, TimeSpan.Zero)); // Another call to 1 because first was server while idle
        elevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Dwell at 3
        elevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Flip to Down, move to 2
        Assert.Equal(DirectionEnum.Down, elevator.Direction);
    }
}