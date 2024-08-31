namespace TradingBots;

internal class CriticalTradingStrategy : Strategy
{
    readonly IIndicator threeMonthHighestHigh;
    readonly IIndicator atr;
    readonly IIndicator twoDayHighestClose;

    double priceToBuy = double.MaxValue;
    int priceToBuyValidDaysLeft = 0;

    int day = 0;

    public CriticalTradingStrategy(string ticker, Portfolio portfolio)
        : base(ticker, portfolio)
    {
        threeMonthHighestHigh = new CustomIndicator(90, (c) => c.High, (e) => e.Max());
        twoDayHighestClose = new CustomIndicator(2, (c) => c.Close, (e) => e.Max());
        atr = new AverageTrueRangeIndicator(20, 2);

        Subscribe(threeMonthHighestHigh);
        Subscribe(twoDayHighestClose);
        Subscribe(atr);
        Subscribe(new IndicatorStopLoss(new AverageTrueRangeIndicator(20), 20, 2));
    }

    protected override void OnNext(Candle candle)
    {
        // Make sure indicators are warmed up
        day++;
        if (day < 90)
        {
            return;
        }

        priceToBuyValidDaysLeft--;
        if (threeMonthHighestHigh.Value > threeMonthHighestHigh.PrevValue)
        {
            priceToBuy = candle.Low - atr.Value;
            priceToBuyValidDaysLeft = 21;
        }

        if (!Held() && priceToBuyValidDaysLeft > 0 && candle.Low < priceToBuy)
        {
            BuySignal();
        }
        else if (Held() && candle.Close > twoDayHighestClose.Value)
        {
            SellSignal();
        }
    }
}
