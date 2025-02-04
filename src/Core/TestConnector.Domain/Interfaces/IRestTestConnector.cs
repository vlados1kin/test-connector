using TestConnector.Domain.Entities;

namespace TestConnector.Domain.Interfaces;

// I have just split the ITestConnector into two independent interfaces according to the interface separation principle.
public interface IRestTestConnector
{
    // I have added soft, start and end parameters. The Bitfinex API uses these as the query parameters.
    Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int? maxCount, int? sort, DateTimeOffset? start, DateTimeOffset? end);
    Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0);
}
