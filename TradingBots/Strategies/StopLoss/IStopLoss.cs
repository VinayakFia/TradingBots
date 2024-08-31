namespace TradingBots;

public interface IStopLoss
{
    public double Compute(List<Candle> candles);
}

public class IndicatorStopLoss(IIndicator indicator, int days = 0, double multiplier = 1) : IStopLoss
{
    readonly int days = days;
    readonly IIndicator indicator = indicator;
    readonly double multiplier = multiplier;

    public double Compute(List<Candle> candles)
    {
        if (days != 0)
        {
            return multiplier * Indicator.Compute(candles, indicator);
        }
        else
        {
            return multiplier * Indicator.Compute(candles.GetRange(candles.Count - days, days), indicator);
        }
    }
}
