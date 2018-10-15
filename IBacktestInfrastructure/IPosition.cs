using System;

namespace IBacktestInfrastructure
{
    public interface IPosition
    {
        double AvgBot { get; set; }
        double AvgSld { get; set; }
        int Buys { get; set; }
        double RealisedPnl { get; set; }
        int Sells { get; set; }
        double TotalBot { get; set; }
        double TotalCommission { get; set; }
        double TotalSld { get; set; }
        double UnrealisedPnl { get; set; }
        double CostBasis { get; set; }
        double Net { get; set; }
        double NetTotal { get; set; }
        double NetTotalInclCommission { get; set; }
        double MarketValue { get; set; }
        DateTime LastPriceUpdate { get; set; }
    }
}