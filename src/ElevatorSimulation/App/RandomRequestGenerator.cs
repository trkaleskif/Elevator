using Application.Config;
using Application.Domain;
using Application.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Console.App;

public class RandomRequestGenerator : IRequestGenerator
{
    private readonly SimulationOptions _options;
    private readonly Random _rng;

    public RandomRequestGenerator(IOptions<SimulationOptions> options)
    {
        _options = options.Value;
        _rng = new Random(_options.RandomSeed);
    }

    public IEnumerable<HallCall> Generate(TimeSpan now)
    {
        var nextDouble = _rng.NextDouble();
        // Only generate a call if the random probability threshold is met.
        if (nextDouble > _options.RequestProbability)
            yield break;

        // Pick a random floor within the valid range.
        int floor = _rng.Next(1, _options.Floors + 1);
        DirectionEnum dir;

        // At the lowest floor, only Up calls are allowed.
        if (floor == 1)
            dir = DirectionEnum.Up;
        else if (floor == _options.Floors)
            dir = DirectionEnum.Down;
        else
            dir = _rng.Next(0, 2) == 0 ? DirectionEnum.Up : DirectionEnum.Down;

        // Yield the generated hall call.
        yield return new HallCall(floor, dir, now);
    }
}