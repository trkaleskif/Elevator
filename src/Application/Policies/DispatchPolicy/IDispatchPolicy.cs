using Application.Domain;

namespace Application.Policies;
public interface IDispatchPolicy
{
    Elevator Pick(IEnumerable<Elevator> elevators, HallCall call, TimeSpan now);
}