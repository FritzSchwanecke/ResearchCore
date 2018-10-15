namespace IReserachCore.Pricer.BlackScholes
{
    public interface IRiskFigures
    {
        /// <summary>
        ///     Deltas the call on forward.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <returns></returns>
        double DeltaCall(double ForwardPrice, double strikeLevel, double impliedVolatility,
            double tenor, double dividendDiscountFactor = 1);

        /// <summary>
        /// Deltas the put on forward.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <returns></returns>
        double DeltaPut(double ForwardPrice, double strikeLevel, double impliedVolatility,
            double tenor, double dividendDiscountFactor = 1);

        /// <summary>
        /// Gammas the specified forward price.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <returns></returns>
        double Gamma(double spotPrice, double strikeLevel, double impliedVolatility,
            double tenor, double ratesDiscountFactor = 1, double dividendDiscountFactor = 1);

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
        double Rho(double spotPrice, double strikeLevel, double impliedVolatility,
            double tenor, double ratesDiscountFactor = 1, double dividendDiscountFactor = 1);

        /// <summary>
        /// Vegas the specified forward price.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        /// <returns></returns>
        double Vega(double spotPrice, double strikeLevel, double impliedVolatility,
            double tenor, double ratesDiscountFactor = 1, double dividendDiscountFactor = 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="RiskFigures" /> class.
        /// </summary>
        /// <param name="ForwardPrice">The forward price.</param>
        /// <param name="strikeLevel">The strike level.</param>
        /// <param name="impliedVolatility">The implied volatility.</param>
        /// <param name="tenor">The tenor.</param>
        double Theta(double spotPrice, double strikeLevel, double impliedVolatility,
            double tenor, double ratesDiscountFactor = 1, double dividendDiscountFactor = 1);
    }
}