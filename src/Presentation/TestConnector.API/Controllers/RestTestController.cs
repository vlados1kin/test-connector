using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TestConnector.Domain.Interfaces;

namespace TestConnector.API.Controllers;

[ApiController]
[Route("api/v1")]
public class RestTestController : ControllerBase
{
    private readonly IRestTestConnector _connector;
    
    public RestTestController(IRestTestConnector connector)
    {
        _connector = connector;
    }

    [HttpGet("trades/{pair}")]
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

    [HttpGet("candles/trade:{periodInSec}:{pair}")]
    public async Task<IActionResult> GetCandleSeries(
        [FromRoute] string pair,
        [FromRoute] int periodInSec,
        [FromQuery] int? sort,
        [FromQuery] DateTimeOffset? start,
        [FromQuery] DateTimeOffset? end,
        [FromQuery] long? count)
    {
        var candles = await _connector.GetCandleSeriesAsync(pair, periodInSec, sort, start, end, count);

        return Ok(candles);
    }
}