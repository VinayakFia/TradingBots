namespace TradingBots;

public class NDayMinClose(int n) : CustomIndicator(n, (c) => c.Close, (e) => e.Min()) { }
