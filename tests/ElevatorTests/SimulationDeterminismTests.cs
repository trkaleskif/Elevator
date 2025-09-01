using Application.Config;
using Application.Policies;
using Application.Services;
using Console.App;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ElevatorTests;
public class SimulationDeterminismTests
{
    [Fact]
    public void FixedSeedProducesDeterministicResults()
    {
        var options = new SimulationOptions
        {
            Floors = 10,
            Cars = 2,
            SecondsPerFloor = 10,
            StopDurationSeconds = 10,
            TickSeconds = 10,
            RequestProbability = 0.5,
            RandomSeed = 123,
            TotalTicks = 10
        };

        var elevatorServiceLoggerMock = new Mock<ILogger<ElevatorService>>();
        var optionsWrapper = Options.Create(options);
        var elevatorService = new ElevatorService(optionsWrapper, new DispatchPolicy());

        var sim = new Simulation(elevatorService, new RandomRequestGenerator(optionsWrapper), new SimulationClock(optionsWrapper), optionsWrapper);

        sim.Run();

        Assert.All(elevatorService.Elevators, e => Assert.InRange(e.CurrentFloor, 1, 10));
    }
}