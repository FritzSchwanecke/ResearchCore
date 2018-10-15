namespace IBacktestInfrastructure
{
    public interface IPositionHandler
    {
        void UpdateMarketValue(IPosition position, double bid, double ask);

        void CalculateInitialValue(IPosition position, eAction action, int quantity, string ticker,
            double initPrice, double initCommission, double bid, double ask);

        void TransactShares(IPosition position, eAction action, int quantity, double price, double commission);
    }
}