using System.Net.Http.Json;
using System.Text.Json;

namespace TradingBots;

internal class TickerResponse
{
    public bool? Active { get; set; }
    public string? Cik { get; set; }
    public string? CompositeFigi { get; set; }
    public string? CurrencyName { get; set; }
    public string? DelistedUtc { get; set; }
    public string? LastUpdatedUtc { get; set; }
    public string Locale { get; set; }
    public string Market { get; set; }
    public string Name { get; set; }
    public string? PrimaryExchange { get; set; }
    public string? ShareClassFigi { get; set; }
    public string Ticker { get; set; }
    public string? Type { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}

internal class GetTickersResponse
{
    public int Count { get; set; }
    public string NextUrl { get; set; }
    public string RequestId { get; set; }
    public List<TickerResponse> Results { get; set; }
    public string status { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}

internal class Aggregate
{
    public double C { get; set; }
    public double H { get; set; }
    public double L { get; set; }
    public double? N { get; set; }
    public double O { get; set; }
    public bool? Otc { get; set; }
    public long T { get; set; }
    public double V { get; set; }
    public double? Vw { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}

internal class GetAggregateResponse
{
    public string Ticker { get; set; }
    public bool Adjusted { get; set; }
    public int QueryCount { get; set; }
    public string RequestId { get; set; }
    public int ResultsCount { get; set; }
    public string Status { get; set; }
    public string? NextUrl { get; set; }
    public List<Aggregate> Results { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}

internal class TimeRangeEnum
{
    readonly string typeKeyWord;

    TimeRangeEnum(string typeKeyWord) => this.typeKeyWord = typeKeyWord;

    public override string ToString() => typeKeyWord;

    public static TimeRangeEnum Day = new("day");
}

internal class Polygon
{
    static readonly HttpClient client = new() { BaseAddress = new Uri("https://api.polygon.io/") };

    static Task<T?> GetFromJsonAsync<T>(string url) =>
        client.GetFromJsonAsync<T>($"{url}&apiKey=54wxsJdH398vqJbzVRfdSdHEsl28N0vv");

    public static async Task<List<TickerResponse>> GetTickers(bool active = true, int count = 4)
    {
        var result = await GetFromJsonAsync<GetTickersResponse>(
            $"v3/reference/tickers?market=stocks&active={active}&limit={count}"
        );
        return result?.Results ?? [];
    }

    public static async Task<List<Aggregate>> GetAggregates(
        string ticker,
        TimeRangeEnum timeRange,
        DateTime start,
        DateTime end
    )
    {
        var result = await GetFromJsonAsync<GetAggregateResponse>(
            $"v2/aggs/ticker/{ticker}/range/1/{timeRange}/{start.Formatted()}/{end.Formatted()}?adjusted=true&sort=asc"
        );
        return result?.Results ?? [];
    }
}
