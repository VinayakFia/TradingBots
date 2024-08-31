namespace TradingBots;

public interface IIndicator
{
    public void OnNext(Candle candle);
    public double Value { get; }
    public double PrevValue { get; }
}
