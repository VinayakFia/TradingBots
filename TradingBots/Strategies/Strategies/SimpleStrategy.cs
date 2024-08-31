namespace TradingBots;

internal class SimpleStrategy : Strategy
{
    readonly IIndicator sevenDayLow;
    readonly IIndicator sevenDayHigh;
    readonly IIndicator movingAverage200;
    int day = 0;

    const int indicatorDays = 5;

    public SimpleStrategy(string ticker, Portfolio portfolio) : base(ticker, portfolio)
    {
        sevenDayLow = new NDayMinClose(indicatorDays);
        sevenDayHigh = new CustomIndicator(indicatorDays, (c) => c.High, (e) => e.Max());
        movingAverage200 = new CustomIndicator(200, (c) => c.Low, (e) => e.Sum() / e.Count());
        Subscribe(sevenDayLow);
        Subscribe(sevenDayHigh);
        Subscribe(movingAverage200);
        Subscribe(new IndicatorStopLoss(new AverageTrueRangeIndicator(20), 20, 2));
    }

    protected override void OnNext(Candle candle)
    {
        // Make sure moving average is setup
        day++;
        if (day < 200)
        {
            return;
        }

        if (candle.Open < movingAverage200.PrevValue)
        {
            return;
        }

        if (!Held() && candle.Close < sevenDayLow.PrevValue)
        {
            BuySignal();
        }
        else if (Held() && candle.Close > sevenDayHigh.PrevValue)
        {
            SellSignal();
        }
    }
}
