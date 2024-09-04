## TradingBots
### What is it?
- Relatively functional backtesting library to test trading strategies.
- Indicators
- Stoplosses
- Strategies

### What can it do?
- Backtest your strategy against ~700 stocks and give you some basic stats. (for example, how much money you would've lost, how much time you spent invested etc...)

### How do I make a strategy?
1. Extend the strategy class
2. Implement the `OnNext` method
3. Setup indicators by subscribing them
4. Setup stoplosses by subscribing them
5. Read the code below, it's pretty self explanatory
```csharp
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
```
