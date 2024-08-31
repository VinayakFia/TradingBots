namespace TradingBots;

public class YFinance
{
    public static async Task DownloadCsvs()
    {
        var tickers = File.ReadLines("C:\\Users\\vinay\\source\\repos\\TradingBots\\TradingBots\\tickers.txt");

        using var client = new HttpClient();
        // add curl user agent to get around 429 TOO MANY REQUESTS
        client.DefaultRequestHeaders.Add("User-Agent", "curl/7.68.0");

        await Task.WhenAll(tickers
            .Where((ticker) => !File.Exists($"{Constants.dataDirectory}\\{ticker}.csv"))
            .Select(async (ticker) =>
            {
                try
                {
                    var link = $"https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1=1452297600&period2=1724469367&interval=1d&events=history&includeAdjustedClose=true";
                    var response = await client.GetByteArrayAsync(link);
                    File.WriteAllBytes($"{Constants.dataDirectory}\\{ticker}.csv", response);
                    Console.WriteLine(ticker);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ticker + " FAILED " + ex.Message);
                }
            }));
    }
}
