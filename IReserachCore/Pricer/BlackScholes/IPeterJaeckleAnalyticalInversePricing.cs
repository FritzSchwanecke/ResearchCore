using System;
using IReserachCore.Helper.Functions;
using IReserachCore.Instruments.Options;

namespace IReserachCore.Pricer.BlackScholes
{
    public interface IPeterJaeckleAnalyticalInversePricing
    {
        IPeterJaeckleAnalyticalPricing PeterJaeckelPricingEngine { get; set; }

        /// <summary>
        ///     Determine the volatility underlying the option price.
        /// </summary>
        /// <param name="valuationTime">The valuation time.</param>
        /// <param name="optionPrice">Option price.</param>
        /// <param name="underlyingPrice">Underlying price.</param>
        /// <param name="volatilitySurface">The volatility surface.</param>
        /// <returns>System.Double.</returns>
        double Volatility(DateTime valuationDate, ISingleAssetOption option, double underlyingPrice,
            double discountFactor = 1d, double dividendDiscountFactor = 1d);

        double Volatility(DateTime valuationDate, DateTime maturityDate, double strike, double price,
            double underlyingPrice, eOptionType optionType,
            double discountFactor = 1d, double dividendDiscountFactor = 1d);
    }
}