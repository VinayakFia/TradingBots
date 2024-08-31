using System.Diagnostics;
using System.Globalization;
using ConsoleTables;
using CsvHelper;
using ScottPlot;

namespace TradingBots;

public class Runner(Func<string, Portfolio, Strategy> strategyProvider)
{
    public Func<string, Portfolio, Strategy> strategyProvider = strategyProvider;
    public DateTime StartDate { get; init; } = DateTime.MinValue;
    public DateTime EndDate { get; init; } = DateTime.MaxValue;

    public void Run(string? plot = null)
    {
        double totalTimeInvested = 0;
        double totalTime = 0;

        List<double> returnPerOrder = [];
        List<double> annualisedReturn = [];

        var files = Directory.GetFiles(Constants.dataDirectory, "*.csv");
        int done = 0;
        int totalFiles = files.Length;
        foreach (var csv in files)
        {
            var ticker = Path.GetFileNameWithoutExtension(csv);
            var strategy = strategyProvider(ticker, new(10000));
            var candles = GetCandles(csv);

            var startingCash = strategy.Portfolio.Cash;

            candles.ForEach((candle) =>
            {
                strategy.OnNextCandle(candle);

                totalTime++;
                if (strategy.Portfolio.IsHeld(ticker))
                {
                    totalTimeInvested++;
                }
            });

            var endingCash = strategy.Portfolio.Cash;

            foreach (var (buy, sell) in GetBuySellPairs(strategy.Portfolio.Orders))
            {
                returnPerOrder.Add((sell.Value - buy.Value) * 100 / buy.Value);
                Debug.Assert(!double.IsNaN(returnPerOrder.Last()));
            }

            annualisedReturn.Add(CalculateAnnusalisedReturn(startingCash, endingCash, candles.First().Date, candles.Last().Date));
            if (annualisedReturn.Last() > 100)
            {
                Console.WriteLine($"What in the hell : {ticker} with {annualisedReturn.Last()}%");
            }
            Debug.Assert(!double.IsNaN(annualisedReturn.Last()));

            done++;
            Console.Clear();
            Console.Write($"{done}/{totalFiles}");

            if (plot == ticker)
            {
                Plot(candles, strategy.Portfolio);
            }
        }

        double exposure = totalTimeInvested / totalTime;

        var table = new ConsoleTable("Metric", "Value");
        table.AddRow("Exposure", $"{exposure * 100}%");
        table.AddRow("Average Return Per Trade", $"{returnPerOrder.Average()}%");
        table.AddRow("Win Rate", $"{returnPerOrder.Count((r) => r > 0) * 100 / returnPerOrder.Count}%");
        table.AddRow("Annusalised Return", $"{annualisedReturn.Average()}%");
        table.AddRow("Annusalised Return / Exposure", $"{annualisedReturn.Average() / exposure}%");
        table.AddRow("Max Annual Return", $"{annualisedReturn.Max()}%");
        table.Write();
    }

    List<Candle> GetCandles(string csv)
    {
        using var reader = new StreamReader(csv);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        return FilterCandles(csvReader.GetRecords<Candle>().ToList());
    }

    List<Candle> FilterCandles(List<Candle> candles) =>
        candles.SkipWhile((c) => c.Date < StartDate).TakeWhile((c) => c.Date <= EndDate).ToList();

    static double CalculateAnnusalisedReturn(double start, double end, DateTime firstDate, DateTime lastDate)
    {
        if (end == start)
            return 0;
        return (end - start) * 100 / (start * ((lastDate - firstDate).TotalDays / 365));
    }

    static List<(Order buy, Order sell)> GetBuySellPairs(List<Order> orders)
    {
        var res = new List<(Order buy, Order sell)>();
        for (int i = 0; i < orders.Count / 2; i++)
        {
            res.Add((orders[i * 2], orders[i * 2 + 1]));
        }
        return res;
    }

    static void Plot(List<Candle> candles, Portfolio portfolio)
    {
        var plot = new Plot();

        plot.Add.Candlestick(candles
            .Select((a) => new OHLC
            {
                Close = a.Close,
                Open = a.Open,
                High = a.High,
                Low = a.Low,
                TimeSpan = TimeSpan.FromDays(1),
                DateTime = a.Date,
            })
            .ToList());
        plot.Axes.DateTimeTicksBottom();

        foreach (var order in portfolio.Orders)
        {
            var isBuy = order.OrderType == OrderType.Buy;
            var marker = plot.Add.Marker(
                order.DateTime.ToOADate(),
                order.Value + (isBuy ? -5 : 5),
                color: isBuy
                    ? Color.FromColor(System.Drawing.Color.Green)
                    : Color.FromColor(System.Drawing.Color.Red),
                shape: isBuy ? MarkerShape.FilledTriangleUp : MarkerShape.FilledTriangleDown
            );

            marker.MarkerSize = 20;
        }

        foreach (var (buyOrder, sellOrder) in GetBuySellPairs(portfolio.Orders))
        {
            var line = plot.Add.Line(
                buyOrder.DateTime.ToOADate(),
                buyOrder.Value,
                sellOrder.DateTime.ToOADate(),
                sellOrder.Value
            );
            line.LineWidth = 5;
            line.Color = Color.FromColor(
                buyOrder.Value < sellOrder.Value
                    ? System.Drawing.Color.Blue
                    : System.Drawing.Color.Red
            );
        }

        plot.SavePng("plot.png", 6000, 3000);
    }
}
