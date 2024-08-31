using TradingBots;

//await YFinance.DownloadCsvs();

var runner = new Runner((t, p) => new SimpleStrategy(t, p))
{
    StartDate = new DateTime(2010, 1, 1),
};
runner.Run();
