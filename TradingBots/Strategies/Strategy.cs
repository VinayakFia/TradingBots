namespace TradingBots;

public abstract class Strategy(string ticker, Portfolio portfolio)
{
    readonly List<IIndicator> indicators = [];
    readonly List<IStopLoss> stopLossStrategies = [];
    double stopLoss = 0;
    public string Ticker { get; set; } = ticker;
    public Portfolio Portfolio { get; set; } = portfolio;
    public List<Candle> Candles { get; set; } = [];
    protected Candle CurrentCandle { get; private set; } = null!;


    public void OnNextCandle(Candle candle)
    {
        Candles.Add(candle);
        CurrentCandle = candle;

        indicators.ForEach((i) => i.OnNext(candle));

        if (Held() && candle.Low < stopLoss)
        {
            //Console.WriteLine($"STOP LOSS, ${candle.Low}");
            SellSignal();
        }

        OnNext(candle);
    }

    protected void BuySignal()
    {
        stopLoss = stopLossStrategies.Count == 0 ? 0 : stopLossStrategies.Min((s) => CurrentCandle.Close - s.Compute(Candles));
        Portfolio.Buy(Ticker, CurrentCandle.Close, CurrentCandle);
    }

    protected void SellSignal() =>
        Portfolio.Sell(Ticker, CurrentCandle.Close, CurrentCandle);

    protected bool Held() => Portfolio.IsHeld(Ticker);

    protected abstract void OnNext(Candle candle);

    /// <summary>
    /// Default behaviour of OnStop is to sell all
    /// </summary>
    public virtual void OnStop()
    {
        if (Held())
        {
            Portfolio.Sell(Ticker, CurrentCandle.Close, CurrentCandle);
        }
    }

    protected void Subscribe(IIndicator i) => indicators.Add(i);

    protected void Subscribe(IStopLoss i) => stopLossStrategies.Add(i);
}
