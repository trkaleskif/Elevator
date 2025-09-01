namespace Application.Config;

public class SimulationOptions
{
    public int Floors { get; set; }
    public int Cars { get; set; }
    public int SecondsPerFloor { get; set; }
    public int StopDurationSeconds { get; set; }

    /// <summary>
    /// The duration (in seconds) of one simulation tick.
    /// Each tick represents a discrete time step for movement or dwell.
    /// </summary>
    public int TickSeconds { get; set; }

    /// <summary>
    /// The probability (0.0 to 1.0) of generating a new hall call on each tick.
    /// </summary>
    public double RequestProbability { get; set; }

    /// <summary>
    /// The random seed used for deterministic request generation and simulation.
    /// </summary>
    public int RandomSeed { get; set; }
    public int TotalTicks { get; set; }
}