using Microsoft.AspNetCore.Mvc;
using TestConnector.Domain.Interfaces;

namespace TestConnector.API.Controllers;

[ApiController]
[Route("api/v1/trades")]
public class RestTestController : ControllerBase
{
    private readonly IRestTestConnector _connector;
    
    public RestTestController(IRestTestConnector connector)
    {
        _connector = connector;
    }

    [HttpGet("{pair}")]
    public async Task<IActionResult> GetNewTrades(
        [FromRoute] string pair,
        [FromQuery] int? maxCount,
        [FromQuery] int? sort,
        [FromQuery] DateTimeOffset? start,
        [FromQuery] DateTimeOffset? end)
    {
        var trades = await _connector.GetNewTradesAsync(pair, maxCount, sort, start, end);

        return Ok(trades);
    }
}