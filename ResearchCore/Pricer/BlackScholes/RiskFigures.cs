using System;
using IReserachCore.Pricer.BlackScholes;
using ResearchCore.Helper.Functions;

namespace ResearchCore.Pricer.BlackScholes
{
    /// <summary>
    /// 
    /// </summary>
    public class RiskFigures : IRiskFigures
    {
        /// <summary>
        ///     Deltas the call on forward.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <returns></returns>
        public double DeltaCall(double ForwardPrice, double strikeLevel, double impliedVolatility,
            double tenor, double dividendDiscountFactor = 1)
        {
            return dividendDiscountFactor * NormalDistribution.NormCdf(d1(ForwardPrice, strikeLevel, impliedVolatility, tenor));
        }

        /// <summary>
        /// Deltas the put on forward.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <returns></returns>
        public double DeltaPut(double ForwardPrice, double strikeLevel, double impliedVolatility,
            double tenor, double dividendDiscountFactor = 1)
        {
            return DeltaCall(ForwardPrice, strikeLevel, impliedVolatility, tenor, dividendDiscountFactor) - dividendDiscountFactor;
        }


        /// <summary>
        /// Gammas the specified forward price.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <returns></returns>
        public double Gamma(double spotPrice, double strikeLevel, double impliedVolatility,
            double tenor, double ratesDiscountFactor = 1, double dividendDiscountFactor = 1)
        {
            return dividendDiscountFactor * NormalDistribution.NormPdf(d1(spotPrice * dividendDiscountFactor / ratesDiscountFactor, strikeLevel, impliedVolatility, tenor)) /
                   (spotPrice * impliedVolatility * Math.Sqrt(tenor));
        }

        /// <summary>
        /// Rhoes the specified spot price.
        /// </summary>
        /// <param name="spotPrice">The spot price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <param name="ratesDiscountFactor">The rates discount factor.</param>
        /// <param name="dividendDiscountFactor">The dividend discount factor.</param>
        /// <returns></returns>
        public double Rho(double spotPrice, double strikeLevel, double impliedVolatility,
            double tenor, double ratesDiscountFactor = 1, double dividendDiscountFactor = 1)
        {
            return strikeLevel * tenor * ratesDiscountFactor * NormalDistribution.NormCdf(
                       d2(spotPrice / ratesDiscountFactor * dividendDiscountFactor, strikeLevel, impliedVolatility,
                           tenor));
        }

        /// <summary>
        /// Vegas the specified forward price.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <returns></returns>
        public double Vega(double spotPrice, double strikeLevel, double impliedVolatility,
            double tenor, double ratesDiscountFactor = 1, double dividendDiscountFactor = 1)
        {
            return NormalDistribution.NormPdf(d1(spotPrice/ratesDiscountFactor * dividendDiscountFactor, strikeLevel, impliedVolatility, tenor)) * spotPrice * dividendDiscountFactor * Math.Sqrt(tenor);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RiskFigures" /> class.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        public double Theta(double spotPrice, double strikeLevel, double impliedVolatility,
                    double tenor, double ratesDiscountFactor = 1, double dividendDiscountFactor = 1)
        {
            return -dividendDiscountFactor * spotPrice * NormalDistribution.NormPdf(d1(
                    spotPrice / ratesDiscountFactor * dividendDiscountFactor, strikeLevel, impliedVolatility, tenor)) *
                impliedVolatility / (2 * Math.Sqrt(tenor)) +
                Math.Log(ratesDiscountFactor) / tenor * strikeLevel * ratesDiscountFactor * NormalDistribution.NormCdf(
                    d2(spotPrice / ratesDiscountFactor * dividendDiscountFactor, strikeLevel, impliedVolatility,
                        tenor)) -
                Math.Log(dividendDiscountFactor) / tenor * spotPrice * dividendDiscountFactor *
                NormalDistribution.NormCdf(d1(spotPrice / ratesDiscountFactor * dividendDiscountFactor, strikeLevel,
                    impliedVolatility, tenor));
        }


        /// <summary>
        ///     D1s the specified forward price.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <returns></returns>
        private static double d1(double ForwardPrice, double strikeLevel, double impliedVolatility,
            double tenor)
        {
            return (Math.Log(ForwardPrice / strikeLevel) + impliedVolatility * impliedVolatility / 2 * tenor) /
                   (impliedVolatility * Math.Sqrt(tenor));
        }

        /// <summary>
        ///     D2s the specified forward price.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <returns></returns>
        private static double d2(double ForwardPrice, double strikeLevel, double impliedVolatility,
            double tenor)
        {
            return d1(ForwardPrice, strikeLevel, impliedVolatility, tenor) - impliedVolatility * Math.Sqrt(tenor);
        }
    }
}