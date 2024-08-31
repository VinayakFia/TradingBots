namespace TradingBots;

public class CustomIndicator(
    int days,
    Func<Candle, double> extractor,
    Func<IEnumerable<double>, double> op,
    double multiplier = 1
) : IIndicator
{
    readonly int days = days;
    readonly Func<Candle, double> extractor = extractor;
    readonly Func<IEnumerable<double>, double> op = op;
    readonly Queue<double> lastNCandles = [];
    readonly double multiplier = multiplier;

    public double Value => multiplier * (lastNCandles.Count != 0 ? op(lastNCandles) : 0);
    public double PrevValue { get; set; }

    public void OnNext(Candle candle)
    {
        PrevValue = Value;
        if (lastNCandles.Count == days)
        {
            lastNCandles.Dequeue();
        }
        lastNCandles.Enqueue(extractor(candle));
    }
}
