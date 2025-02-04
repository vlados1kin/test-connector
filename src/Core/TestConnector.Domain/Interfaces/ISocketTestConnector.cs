using TestConnector.Domain.Entities;

namespace TestConnector.Domain.Interfaces;

// I have just split the ITestConnector into two independent interfaces according to the interface separation principle.
public interface ISocketTestConnector
{
    event Action<Trade> NewBuyTrade;
    event Action<Trade> NewSellTrade;
    void SubscribeTrades(string pair, int maxCount = 100);
    void UnsubscribeTrades(string pair);

    event Action<Candle> CandleSeriesProcessing;
    void SubscribeCandles(string pair, int periodInSec, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0);
    void UnsubscribeCandles(string pair);
}