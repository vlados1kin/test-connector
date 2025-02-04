using System.Globalization;
using System.Text.Json;
using TestConnector.Domain.Entities;
using TestConnector.Domain.Interfaces;

namespace TestConnector.Infrastructure.TestConnectors;

public class RestTestConnector : IRestTestConnector
{
    private readonly HttpClient _httpClient; 
    
    public RestTestConnector(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api-pub.bitfinex.com/v2/");
    }
    
    public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int? maxCount, int? sort, DateTimeOffset? start, DateTimeOffset? end)
    {
        var queryParameters = new Dictionary<string, string?>()
        {
            { "limit", maxCount?.ToString() },
            { "sort", sort?.ToString() },
            { "start", start?.ToUnixTimeMilliseconds().ToString() },
            { "end", end?.ToUnixTimeMilliseconds().ToString() }
        };

        var queryBuilder = queryParameters.Where(kvp => kvp.Value != null).ToList();

        var query = string.Join("&", queryBuilder.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        
        var request = $"trades/{pair}/hist?{query}";
        
        var responseMessage = await _httpClient.GetAsync(request);
        
        responseMessage.EnsureSuccessStatusCode();

        var json = await responseMessage.Content.ReadAsStringAsync();

        var rawTrades = JsonSerializer.Deserialize<List<List<decimal>>>(json);
        if (rawTrades == null)
        {
            return new List<Trade>();
        }

        return rawTrades.Select(rawTrade => new Trade
            {
                Pair = pair,
                Price = rawTrade[3],
                Amount = rawTrade[2],
                Side = rawTrade[2] > 0 ? "buy" : "sell",
                Time = DateTimeOffset.FromUnixTimeMilliseconds((long)rawTrade[1]),
                Id = rawTrade[0].ToString(CultureInfo.InvariantCulture)
            }).ToList();
    }

    public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, int? sort, DateTimeOffset? from, DateTimeOffset? to, long? count)
    {
        var queryParameters = new Dictionary<string, string?>()
        {
            { "sort", sort?.ToString() },
            { "start", from?.ToUnixTimeMilliseconds().ToString() },
            { "end", to?.ToUnixTimeMilliseconds().ToString() },
            { "limit", count?.ToString() }
        };

        var queryBuilder = queryParameters.Where(kvp => kvp.Value != null).ToList();

        var query = string.Join("&", queryBuilder.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var timeFrame = GetTimeFrameFromPeriodInSeconds(periodInSec);
        
        var request = $"candles/trade%3A{timeFrame}%3A{pair}/hist?{query}";

        var responseMessage = await _httpClient.GetAsync(request);

        responseMessage.EnsureSuccessStatusCode();

        var json = await responseMessage.Content.ReadAsStringAsync();

        var rawCandles = JsonSerializer.Deserialize<List<List<decimal>>>(json);
        if (rawCandles == null)
        {
            return new List<Candle>();
        }

        return rawCandles.Select(rawCandle => new Candle
            {
                Pair = pair,
                OpenPrice = rawCandle[1],
                HighPrice = rawCandle[3],
                LowPrice = rawCandle[4],
                ClosePrice = rawCandle[2],
                TotalPrice = (rawCandle[1] + rawCandle[2]) / 2 * rawCandle[5],
                TotalVolume = rawCandle[5],
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds((long)rawCandle[0])
            }).ToList();
    }
    
    private string GetTimeFrameFromPeriodInSeconds(int periodInSec)
    {
        var timeFrame = _timeFrames
            .Where(kvp => kvp.Key <= periodInSec)
            .Select(kvp => kvp.Value)
            .LastOrDefault();

        return timeFrame ?? "1m";
    }

    private readonly Dictionary<int, string> _timeFrames = new()
    {
        { 1 * 60, "1m" },
        { 5 * 60, "5m" },
        { 15 * 60, "15m" },
        { 30 * 60, "30m" },
        { 1 * 60 * 60, "1h" },
        { 3 * 60 * 60, "3h" },
        { 6 * 60 * 60, "6h" },
        { 12 * 60 * 60, "12h" },
        { 1 * 24 * 60 * 60, "1D" },
        { 7 * 24 * 60 * 60, "1W" },
        { 14 * 24 * 60 * 60, "14D" },
        { 30 * 24 * 60 * 60, "1M" }
    };
}