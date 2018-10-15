using System;
using IReserachCore.Instruments.Options;

namespace IReserachCore.Pricer.BlackScholes
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPeterJaeckleAnalyticalPricing
    {
        /// <summary>
        ///     Normaliseds the vega.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        double NormalisedVega(double x, double s);

        /// <summary>
        /// Nets the present value.
        /// </summary>
        /// <param name="valuationDate">The valuation date.</param>
        /// <param name="option">The option.</param>
        /// <param name="underlyingPrice">The underlying price.</param>
        /// <param name="discountFactor">The discount factor.</param>
        /// <param name="dividendDiscountFactor">The dividend discount factor.</param>
        /// <returns></returns>
        double NetPresentValue(DateTime valuationDate, ISingleAssetOption option, double underlyingPrice,
            double discountFactor = 1d, double dividendDiscountFactor = 1d);

        double NormalisedIntrinsic(double x, double q /* q=±1 */);

        double NormalisedBlackCall(double x, double s);
    }
}