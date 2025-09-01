using Application.Domain.Enums;

namespace Application.Domain;
public class Elevator
{
    public int Id { get; set; }
    public int CurrentFloor { get; set; }
    public DirectionEnum Direction { get; set; }
    public ElevatorStateEnum State { get; set; }
    public int DwellRemaining { get; set; }
    public SortedSet<int> UpStops { get; }
    public SortedSet<int> DownStops { get; }

    private readonly int _minFloor;
    private readonly int _maxFloor;
    private readonly int _dwellTicks;

    public Elevator()
    {
        UpStops = new SortedSet<int>();
        DownStops = new SortedSet<int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
    }

    public Elevator(int id, int initialFloor, int minFloor, int maxFloor, int dwellTicks)
    {
        Id = id;
        CurrentFloor = initialFloor;
        Direction = DirectionEnum.None;
        State = ElevatorStateEnum.Idle;
        DwellRemaining = 0;
        UpStops = new SortedSet<int>();
        DownStops = new SortedSet<int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
        _minFloor = minFloor;
        _maxFloor = maxFloor;
        _dwellTicks = dwellTicks;
    }

    public void AddHallCall(HallCall call)
    {
        // Ignore calls outside valid floor range.
        if (call.Floor < _minFloor || call.Floor > _maxFloor)
            return;

        // If idle at requested floor, start dwelling immediately.
        if (call.Floor == CurrentFloor && State == ElevatorStateEnum.Idle)
        {
            AddStop(call.Floor, call.Direction);
            State = ElevatorStateEnum.Dwelling;
            DwellRemaining = _dwellTicks;
            Direction = call.Direction;
            return;
        }

        AddStop(call.Floor, call.Direction);
    }

    private void AddStop(int floor, DirectionEnum direction)
    {
        if (direction == DirectionEnum.Up)
            UpStops.Add(floor);
        else
            DownStops.Add(floor);
    }

    public void Tick(TimeSpan now, TimeSpan tickSize)
    {
        switch (State)
        {
            case ElevatorStateEnum.Idle:
                if (UpStops.Any())
                {
                    if (CurrentFloor < UpStops.FirstOrDefault())
                        Direction = DirectionEnum.Up;
                    else if (CurrentFloor > UpStops.FirstOrDefault())
                        Direction = DirectionEnum.Down;

                    State = ElevatorStateEnum.Moving;
                }
                else if (DownStops.Any())
                {
                    if (CurrentFloor < DownStops.FirstOrDefault())
                        Direction = DirectionEnum.Up;
                    else if (CurrentFloor > DownStops.FirstOrDefault())
                        Direction = DirectionEnum.Down;
                    State = ElevatorStateEnum.Moving;
                }
                break;
            case ElevatorStateEnum.Moving:
                MoveOneFloor();

                // If arrived at a stop, start dwelling.
                if (ShouldStopHere())
                {
                    State = ElevatorStateEnum.Dwelling;
                    DwellRemaining = _dwellTicks;
                }
                break;
            case ElevatorStateEnum.Dwelling:
                DwellRemaining--;
                if (DwellRemaining <= 0)
                {
                    // Remove the stop just served.
                    RemoveStopHere();
                    // Continue in current direction if stops remain.
                    if (HasStopsInCurrentDirection())
                    {
                        State = ElevatorStateEnum.Moving;
                    }
                    // Flip direction if stops exist in the opposite direction.
                    else if (HasStopsInOppositeDirection())
                    {
                        FlipDirection();
                        State = ElevatorStateEnum.Moving;
                    }
                    else
                    {
                        State = ElevatorStateEnum.Idle;
                        Direction = DirectionEnum.None;
                    }
                }
                break;
        }
    }

    private void MoveOneFloor()
    {
        if (Direction == DirectionEnum.Up && CurrentFloor < _maxFloor)
            CurrentFloor++;
        else if (Direction == DirectionEnum.Down && CurrentFloor > _minFloor)
            CurrentFloor--;
    }

    public bool ShouldStopHere()
    {
        return Direction == DirectionEnum.Up && UpStops.Contains(CurrentFloor) ||
               Direction == DirectionEnum.Down && DownStops.Contains(CurrentFloor);
    }

    private void RemoveStopHere()
    {
        if (Direction == DirectionEnum.Up)
            UpStops.Remove(CurrentFloor);
        else if (Direction == DirectionEnum.Down)
            DownStops.Remove(CurrentFloor);
    }

    private bool HasStopsInCurrentDirection()
    {
        return Direction == DirectionEnum.Up ? UpStops.Any() : DownStops.Any();
    }

    private bool HasStopsInOppositeDirection()
    {
        return Direction == DirectionEnum.Up ? DownStops.Any() : UpStops.Any();
    }

    private void FlipDirection()
    {
        Direction = Direction == DirectionEnum.Up ? DirectionEnum.Down : DirectionEnum.Up;
    }

    public int EstimatedAdditionalFloorsToServe(HallCall hallCall)
    {
        int stops = UpStops.Count + DownStops.Count;
        int dist = Math.Abs(CurrentFloor - hallCall.Floor);
        return stops + dist;
    }
}