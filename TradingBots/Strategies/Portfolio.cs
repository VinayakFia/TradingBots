namespace TradingBots;

public enum OrderType
{
    Buy,
    Sell,
}

public class Order
{
    public OrderType OrderType { get; set; }
    public double Value { get; set; }
    public double Volume { get; set; }
    public DateTime DateTime { get; set; }
}

public class Portfolio(double startingCash)
{
    public Dictionary<string, double> Positions { get; set; } = [];

    double cash = startingCash;
    public double Cash
    {
        get => cash;
        set
        {
            //Console.WriteLine("Setting cash to " + value);
            cash = value;
        }
    }
    public List<Order> Orders { get; } = [];

    public void Buy(string ticker, double value, Candle candle)
    {
        if (Positions.ContainsKey(ticker))
        {
            throw new InvalidOperationException("Cannot buy bought ticker");
        }

        // Currently buys as much as possible
        var count = Math.Floor(Cash / value);
        Positions[ticker] = count;
        Cash -= value * count;

        Orders.Add(
            new()
            {
                OrderType = OrderType.Buy,
                Value = value,
                Volume = count,
                DateTime = candle.Date,
            }
        );

        //Console.WriteLine($"BUY {ticker}, ${value}x{count}");
    }

    public void Sell(string ticker, double value, Candle candle)
    {
        if (!Positions.TryGetValue(ticker, out double count))
        {
            throw new InvalidOperationException("Cannot sell ticker that is not held");
        }

        Positions.Remove(ticker);
        Cash += value * count;

        Orders.Add(
            new()
            {
                OrderType = OrderType.Sell,
                Value = value,
                Volume = count,
                DateTime = candle.Date,
            }
        );

        //Console.WriteLine($"SELL {ticker}, ${value}x{count}");
    }

    public bool IsHeld(string Ticker) => Positions.ContainsKey(Ticker);
}
