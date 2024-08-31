namespace TradingBots;

public class AverageTrueRangeIndicator(int days, double multiplier = 1.0)
    : CustomIndicator(days, (c) => c.High - c.Low, (d) => d.Average(), multiplier)
{ }
