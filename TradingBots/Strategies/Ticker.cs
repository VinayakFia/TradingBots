using System.Text.Json;

namespace TradingBots;

internal class Ticker(string symbol, List<Candle> candles)
{
    public string Name { get; init; } = symbol;
    public List<Candle> Aggregates { get; init; } = candles;
}

public class Candle
{
    public double Close { get; init; }
    public double High { get; init; }
    public double Low { get; init; }
    public double Open { get; init; }
    public DateTime Date { get; init; }
    public long Volume { get; init; }
    public override string ToString() => JsonSerializer.Serialize(this);
}
