namespace TradingBots;

internal static class DateTimeExtensions
{
    static string FormatNumber(int n) => n >= 10 ? n.ToString() : $"0{n}";

    public static string Formatted(this DateTime dateTime) =>
        $"{dateTime.Year}-{FormatNumber(dateTime.Month)}-{FormatNumber(dateTime.Day)}";
}
