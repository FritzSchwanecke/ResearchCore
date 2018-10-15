using System;
using IBacktestInfrastructure;

namespace BacktestInfrastructure
{


    public class PositionHandler : IPositionHandler
    {
        public void UpdateMarketValue(IPosition position, double bid, double ask)
        {
            // The market value is tricky to calculate as we only have
            // access to the top of the order book through a broker,
            // which means that the true redemption price is
            // unknown until executed.
            // However, it can be estimated via the mid-price of the
            // bid-ask spread.Once the market value is calculated it
            // allows calculation of the unrealised and realised profit
            // and loss of any transactions.

            var midpoint = (bid + ask) / 2.0;
            position.MarketValue = position.NetTotal * midpoint;
            position.UnrealisedPnl = position.MarketValue - position.CostBasis;
            position.LastPriceUpdate = DateTime.UtcNow;
        }

        public void CalculateInitialValue(IPosition position, eAction action, int quantity, string ticker,
            double initPrice, double initCommission, double bid, double ask)
        {
            // Depending upon whether the action was a buy or sell("BOT"
            // or "SLD") calculate the average bought cost, the total bought
            // cost, the average price and the cost basis.
            // Finally, calculate the net total with and without commission.

            switch (action)
            {
                case eAction.BOT:
                    position.Buys = quantity;
                    position.AvgBot = initPrice;
                    position.TotalBot = position.AvgBot * position.Buys;
                    position.AvgBot = (initPrice * quantity + initCommission) / quantity;
                    position.CostBasis = quantity * position.AvgBot;
                    break;
                case eAction.SLD:
                    position.Sells = quantity;
                    position.AvgSld = initPrice;
                    position.TotalSld = position.AvgSld * position.Sells;
                    position.AvgSld = (initPrice * quantity - initCommission) / quantity;
                    position.CostBasis = quantity * position.AvgSld;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }

            position.TotalCommission += initCommission;
            position.Net = position.Buys - position.Sells;
            position.NetTotal = position.TotalBot - position.TotalSld;
            position.NetTotalInclCommission = position.NetTotal - position.TotalCommission;
        }

        public void TransactShares(IPosition position, eAction action, int quantity, double price, double commission)
        {
            //Calculates the adjustments to the Position that occur
            //once new shares are bought and sold.
            //
            //Takes care to update the average bought/sold, total
            //bought/sold, the cost basis and PnL calculations,
            //as carried out through Interactive Brokers TWS.

            var prevQuantity = position.Net;
            var prevComission = position.TotalCommission;
            position.TotalCommission += commission;

            switch (action)
            {
                case eAction.BOT:
                    position.AvgBot = (position.AvgBot * position.Buys + quantity * price + commission) /
                                      (position.Buys + quantity);
                    position.Buys = position.Buys + quantity;
                    position.TotalBot = position.Buys * position.AvgBot;
                    break;
                case eAction.SLD:
                    position.AvgSld = (position.AvgSld * position.Sells + quantity * price - commission) /
                                      (position.Sells + quantity);
                    position.Sells = position.Sells + quantity;
                    position.TotalSld = position.Sells * position.AvgSld;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }

            position.Net = position.Buys - position.Buys;
            position.NetTotal = position.TotalBot - position.TotalSld + position.TotalCommission;
            position.NetTotalInclCommission = position.TotalBot - position.TotalSld;

            position.CostBasis = quantity * position.AvgBot;
        }
    }
}