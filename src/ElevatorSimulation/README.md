# ElevatorSim

A time-driven simulation of a building with elevators.

## How to Run

1. Open `Console.sln` in Visual Studio 2022.
2. Edit `SimulationOptions` in `Program.cs` or `/Config/SimulationOptions.cs` to set:
   - Floors, Cars, SecondsPerFloor, DwellSeconds, TickSeconds, RequestProbability, RandomSeed, Ticks
3. Build and run the console app.
4. Observe console logs for hall calls, assignments, and elevator status per tick.

## Console Output

- Each tick logs new hall calls, assignments, and elevator status:

## Extensibility

- Swap out `IDispatchPolicy` or `IRequestGenerator` via DI for new behaviors.
- Add metrics, alternative logging, or new elevator features by extending domain classes.
- Deterministic tests via seeded RNG and virtual clock.

## Tests

- Run `Console.Tests` for coverage of FSM, stop planning, dispatch, and determinism.