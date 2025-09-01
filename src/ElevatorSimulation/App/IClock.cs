namespace Console.App;

public interface IClock
{
    TimeSpan Now { get; }
    TimeSpan TickSize { get; }
    void Advance();
}