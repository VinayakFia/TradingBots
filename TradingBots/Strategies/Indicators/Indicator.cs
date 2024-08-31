namespace TradingBots;

public class Indicator
{
    public static double Compute(List<Candle> candles, IIndicator indicator)
    {
        candles.ForEach(indicator.OnNext);
        return indicator.Value;
    }
}
