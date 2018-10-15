using System;

namespace IDataAccessLayer.LoadData
{
    public interface IInterestRateTable
    {
        DateTime[] ValuationDates { get; set; }
        int[] Tenor { get; set; }
        double[,] InterestRates { get; set; }

        /// <summary>
        /// Gets the discount factor.
        /// </summary>
        /// <param name="valuationDate">The valuation date.</param>
        /// <param name="maturityDate">The maturity date.</param>
        /// <returns></returns>
        double GetDiscountFactor(DateTime valuationDate, DateTime maturityDate);
    }
}