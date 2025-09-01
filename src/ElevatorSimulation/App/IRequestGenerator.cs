using Application.Domain;

namespace Console.App;

public interface IRequestGenerator
{
    IEnumerable<HallCall> Generate(TimeSpan now);
}