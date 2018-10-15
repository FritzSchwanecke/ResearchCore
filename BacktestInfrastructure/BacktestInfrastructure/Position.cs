using System;
using IBacktestInfrastructure;

namespace BacktestInfrastructure
{
    //This class encapsulates all data associated with an open position in an asset. That is, it tracks the realised and unrealised profit and loss (PnL) by averaging the multiple
    //"legs" of the transaction, inclusive of transaction costs.

    public class Position : IPosition
    {
        public double AvgBot { get; set; } = 0.0;
        public double AvgSld { get; set; } = 0.0;

        public int Buys { get; set; } = 0;
        public double RealisedPnl { get; set; } = 0.0;
        public int Sells { get; set; } = 0;

        public double TotalBot { get; set; } = 0.0;
        public double TotalCommission { get; set; } = 0.0;
        public double TotalSld { get; set; } = 0.0;

        public double UnrealisedPnl { get; set; } = 0.0;
        public double CostBasis { get; set; } = 0.0;
        public double Net { get; set; }
        public double NetTotal { get; set; }
        public double NetTotalInclCommission { get; set; }
        public double MarketValue { get; set; }
        public DateTime LastPriceUpdate { get; set; }
    }
}