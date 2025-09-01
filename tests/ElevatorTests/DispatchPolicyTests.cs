using Application.Domain;
using Application.Domain.Enums;
using Application.Policies;

namespace ElevatorTests;
public class DispatchPolicyTests
{
    [Fact]
    public void PassByElevatorPreferred()
    {
        var upElevator = new Elevator(1, 2, 1, 10, 1);
        upElevator.AddHallCall(new HallCall(5, DirectionEnum.Up, TimeSpan.Zero));
        upElevator.Tick(TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Moving up

        var idleElevator = new Elevator(2, 1, 1, 10, 1);

        var policy = new DispatchPolicy();
        var call = new HallCall(3, DirectionEnum.Up, TimeSpan.Zero);
        var chosen = policy.Pick(new List<Elevator> { upElevator, idleElevator }, call, TimeSpan.Zero);
        Assert.Equal(upElevator, chosen);
    }

    [Fact]
    public void NearestIdleElevatorPreferred()
    {
        var idleElevator1 = new Elevator(1, 2, 1, 10, 1);
        var idleElevator2 = new Elevator(2, 5, 1, 10, 1);

        var policy = new DispatchPolicy();
        var call = new HallCall(3, DirectionEnum.Up, TimeSpan.Zero);
        var chosenElevator = policy.Pick(new List<Elevator> { idleElevator1, idleElevator2 }, call, TimeSpan.Zero);
        Assert.Equal(idleElevator1, chosenElevator);
    }

    [Fact]
    public void LeastWorkHeuristicUsed()
    {
        var busyElevator = new Elevator(1, 2, 1, 10, 1);
        busyElevator.AddHallCall(new HallCall(5, DirectionEnum.Up, TimeSpan.Zero));
        var idleElevator = new Elevator(2, 10, 1, 10, 1);

        var policy = new DispatchPolicy();
        var call = new HallCall(9, DirectionEnum.Down, TimeSpan.Zero);
        var chosenElevator = policy.Pick(new List<Elevator> { busyElevator, idleElevator }, call, TimeSpan.Zero);
        Assert.Equal(idleElevator, chosenElevator);
    }
}