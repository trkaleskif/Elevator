using Application.Domain;
using Application.Domain.Enums;

namespace Application.Policies;

public class DispatchPolicy : IDispatchPolicy
{
    public Elevator Pick(IEnumerable<Elevator> elevators, HallCall call, TimeSpan now)
    {
        // 1. Pass-by elevator: already moving in the call's direction and will pass the call's floor.
        var passBy = elevators
            .Where(e => e.State == ElevatorStateEnum.Moving && e.Direction == call.Direction)
            .Where(e => call.Direction == DirectionEnum.Up && e.CurrentFloor <= call.Floor ||
                        call.Direction == DirectionEnum.Down && e.CurrentFloor >= call.Floor)
            .OrderBy(e => Math.Abs(e.CurrentFloor - call.Floor)) //order by closest to the call's floor
            .FirstOrDefault();
        if (passBy != null) return passBy;

        // 2. Nearest idle elevator: not moving, closest to the call's floor.
        var idle = elevators
            .Where(e => e.State == ElevatorStateEnum.Idle)
            .OrderBy(e => Math.Abs(e.CurrentFloor - call.Floor))
            .FirstOrDefault();
        if (idle != null) return idle;

        // 3. Least work: elevator with the smallest sum of outstanding stops and distance to the new call.
        return elevators
            .OrderBy(e => e.EstimatedAdditionalFloorsToServe(call))
            .First();
    }
}