using Application.Domain;
using Application.Services;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/elevators")]
public class ElevatorController : ControllerBase
{
    private readonly IElevatorService _elevatorService;

    public ElevatorController(IElevatorService elevatorService)
    {
        _elevatorService = elevatorService;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<Elevator>> GetElevators()
        => Ok(_elevatorService.Elevators);

    [HttpPost("hallcalls")]
    public IActionResult AcceptHallCalls([FromBody] IEnumerable<HallCall> calls, [FromQuery] double nowSeconds)
    {
        _elevatorService.AcceptHallCalls(calls, TimeSpan.FromSeconds(nowSeconds));
        return Ok();
    }

    [HttpPost("tick")]
    public IActionResult Tick([FromQuery] double nowSeconds, [FromQuery] double tickSizeSeconds)
    {
        _elevatorService.Tick(TimeSpan.FromSeconds(nowSeconds), TimeSpan.FromSeconds(tickSizeSeconds));
        return Ok();
    }

    [HttpGet("status")]
    public ActionResult<string> GetStatus([FromQuery] double nowSeconds)
        => Ok(_elevatorService.GetStatusLine(TimeSpan.FromSeconds(nowSeconds)));

    [HttpPost("reset")]
    public IActionResult Reset()
    {
        _elevatorService.Reset();
        return Ok();
    }
}