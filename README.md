# TradingBots

A simple backtesting library for testing trading strategies in C#/.NET.

## What it does

- Test trading strategies against historical stock data (~700 stocks)
- Built-in technical indicators (moving averages, RSI, ATR, etc.)
- Basic stop-loss functionality
- Generate performance metrics and optional charts

## Usage

Create a stretegy by extending the `Strategy` class:


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

## Output

Basic performance metrics:

```
794/794
 -------------------------------------------------------
 | Metric                        | Value               |
 -------------------------------------------------------
 | Exposure                      | 29.816084460890167% |
 -------------------------------------------------------
 | Average Return Per Trade      | 0.5561910795131949% |
 -------------------------------------------------------
 | Win Rate                      | 56%                 |
 -------------------------------------------------------
 | Annusalised Return            | 2.661242002920265%  |
 -------------------------------------------------------
 | Annusalised Return / Exposure | 8.925524766375755%  |
 -------------------------------------------------------
 | Max Annual Return             | 498.97544788687657% |
 -------------------------------------------------------

 Count: 6
```

Optional chart showing buy/sell signals:
<img width="6000" height="3000" alt="plot" src="https://github.com/user-attachments/assets/f53ace20-a10d-46b0-a8a1-2f980a0e7bfd" />

## Features

- Common technical indicators
- ATR-based and percentage stop losses
- Performance analytics
- Simple charting

## Tech Stack

- C#/.NET
- Historical stock data processing

---

*Educational project - not for actual trading*
