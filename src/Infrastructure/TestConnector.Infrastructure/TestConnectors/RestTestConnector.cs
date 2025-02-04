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

    public Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
    {
        throw new NotImplementedException();
    }
}