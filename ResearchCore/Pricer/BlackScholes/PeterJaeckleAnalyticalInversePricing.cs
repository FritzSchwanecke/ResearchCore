using System;
using IReserachCore.Instruments.Options;
using IReserachCore.Pricer.BlackScholes;
using ResearchCore.Helper.Functions;
using Unity.Attributes;

namespace ResearchCore.Pricer.BlackScholes
{
    public class PeterJaeckleAnalyticalInversePricing : IPeterJaeckleAnalyticalInversePricing
    {
        [InjectionConstructor]
        public PeterJaeckleAnalyticalInversePricing(IPeterJaeckleAnalyticalPricing pricingEngine)
        {
            PeterJaeckelPricingEngine = pricingEngine;
        }

        private static double TwoPi { get; } = 6.283185307179586476925286766559005768394338798750;

        private static double SqrtPiOverTwo { get; } = 1.253314137315500251207882642405522626503493370305;

        private static double SqrtThree { get; } = 1.732050807568877293527446341505872366942805253810;

        private static double SqrtOneOverThree { get; } = 0.577350269189625764509148780501957455647601751270;

        private static double TwoPiOverSqrtTwentySeven { get; } = 1.209199576156145233729385505094770488189377498728;

        private static double PiOverSix { get; } = 0.523598775598298873077107230546583814032861566563;

        /// <summary>
        ///     Gets the denormalisation cutoff.
        /// </summary>
        /// <value>The denormalisation cutoff.</value>
        private static double DenormalisationCutoff { get; } = 0.0;

        /// <summary>
        ///     Gets the maximum number of iterations - two is sufficient to achieve maximum precision.
        /// </summary>
        /// <value>The maximum iterations.</value>
        private static int MaximumIterations { get; } = 2;

        /// <summary>
        ///     Gets the minimum rational cubic control parameter value.
        /// </summary>
        /// <value>The minimum rational cubic control parameter value.</value>
        private static double MinimumRationalCubicControlParameterValue { get; } = -(1 - Math.Sqrt(double.Epsilon));

        /// <summary>
        ///     Gets the maximum rational cubic control parameter value.
        /// </summary>
        /// <value>The maximum rational cubic control parameter value.</value>
        private static double MaximumRationalCubicControlParameterValue { get; } = 2 / Math.Pow(double.Epsilon, 2.0);

        /// <summary>
        ///     Gets the householder method order.
        /// </summary>
        /// <value>The householder method order.</value>
        private static double HouseholderMethodOrder { get; } = 4;

        public IPeterJaeckleAnalyticalPricing PeterJaeckelPricingEngine { get; set; }


        public double Volatility(DateTime valuationDate, DateTime maturityDate, double strike , double price, double underlyingPrice, eOptionType optionType,
            double discountFactor = 1d, double dividendDiscountFactor = 1d)
        {
            var tenor = (maturityDate - valuationDate).TotalDays / 365.0;

            //Get forward price
            var forwardPrice = underlyingPrice * dividendDiscountFactor / discountFactor;
            //Get discounted strike
            var discountedStrike = dividendDiscountFactor * strike;
            var discountedSpot = underlyingPrice * dividendDiscountFactor;

            //Get put call indicator
            int q;
            if (optionType == eOptionType.C)
                q = 1;
            else
                q = -1;

            //Calculate intrinsic value
            var intrinsicValue = Math.Max(q < 0 ? strike - forwardPrice : forwardPrice - strike, 0.0);

            //Check if price is below intrinsic
            if (price <= intrinsicValue) return double.NaN;

            //Check if price is above maximum price
            var maxPrice = q < 0 ? strike : forwardPrice;

            if (price >= maxPrice) return double.NaN;

            //Get moneyness
            var moneyness = Math.Log(forwardPrice / strike);

            if (!(intrinsicValue > 0.0))
                return UncheckedNormalizedImpliedVolatility(price / (Math.Sqrt(forwardPrice) * Math.Sqrt(strike)),
                           moneyness, q, MaximumIterations) / Math.Sqrt(tenor);

            // Map in-the-money to out-of-the-money
            // price = Math.Max(price - intrinsicValue, 0.0); //** Original Code**

            price = q < 0
                ? Math.Max(price + discountedSpot - discountedStrike, 0.0)
                : Math.Max(price - discountedSpot + discountedStrike, 0.0);

            //Check if price is above maximum price
            maxPrice = q > 0 ? strike : forwardPrice;

            if (price >= maxPrice) return double.NaN;

            //Calculate intrinsic value
            intrinsicValue = Math.Max(q > 0 ? strike - forwardPrice : forwardPrice - strike, 0.0);

            //Check if price is below intrinsic
            if (price <= intrinsicValue) return double.NaN;


            return UncheckedNormalizedImpliedVolatility(price / (Math.Sqrt(forwardPrice) * Math.Sqrt(strike)),
                       moneyness, -q, MaximumIterations) / Math.Sqrt(tenor);
        }

        /// <summary>
        ///     Determine the volatility underlying the option price.
        /// </summary>
        /// <param name="valuationTime">The valuation time.</param>
        /// <param name="optionPrice">Option price.</param>
        /// <param name="underlyingPrice">Underlying price.</param>
        /// <param name="volatilitySurface">The volatility surface.</param>
        /// <returns>System.Double.</returns>
        public double Volatility(DateTime valuationDate, ISingleAssetOption option, double underlyingPrice,
            double discountFactor = 1d, double dividendDiscountFactor = 1d)
        {
            var tenor = (option.Maturity - valuationDate).TotalDays / 365.0;

            //Get strike
            var strike = option.Strike;

            //Get option price
            var price = option.Premium;

            //Get forward price
            var forwardPrice = underlyingPrice * dividendDiscountFactor / discountFactor;
            //Get discounted strike
            var discountedStrike = dividendDiscountFactor * option.Strike;
            var discountedSpot = underlyingPrice * dividendDiscountFactor;

            //Get put call indicator
            int q;
            if (option.OptionType == eOptionType.C)
                q = 1;
            else
                q = -1;


            //Calculate intrinsic value
            var intrinsicValue = Math.Max(q < 0 ? strike - forwardPrice : forwardPrice - strike, 0.0);

            //Check if price is below intrinsic
            if (price <= intrinsicValue) return double.NaN;

            //Check if price is above maximum price
            var maxPrice = q < 0 ? strike : forwardPrice;

            if (price >= maxPrice) return double.NaN;

            //Get moneyness
            var moneyness = Math.Log(forwardPrice / strike);

            if (!(intrinsicValue > 0.0))
                return UncheckedNormalizedImpliedVolatility(price / (Math.Sqrt(forwardPrice) * Math.Sqrt(strike)),
                           moneyness, q, MaximumIterations) / Math.Sqrt(tenor);

            // Map in-the-money to out-of-the-money
            // price = Math.Max(price - intrinsicValue, 0.0); //** Original Code**

            price = q < 0
                ? Math.Max(price + discountedSpot - discountedStrike, 0.0)
                : Math.Max(price - discountedSpot + discountedStrike, 0.0);

            //Check if price is above maximum price
            maxPrice = q > 0 ? strike : forwardPrice;

            if (price >= maxPrice) return double.NaN;

            //Calculate intrinsic value
            intrinsicValue = Math.Max(q > 0 ? strike - forwardPrice : forwardPrice - strike, 0.0);

            //Check if price is below intrinsic
            if (price <= intrinsicValue) return double.NaN;


            return UncheckedNormalizedImpliedVolatility(price / (Math.Sqrt(forwardPrice) * Math.Sqrt(strike)),
                       moneyness, -q, MaximumIterations) / Math.Sqrt(tenor);
        }

        /// <summary>
        ///     Computes the f lower map with derivatives.
        /// </summary>
        /// <param name="moneyness">The moneyness.</param>
        /// <param name="sigma">The sigma.</param>
        /// <returns>System.Double.</returns>
        private double[] ComputeFLowerMapDerivatives(double moneyness, double sigma)
        {
            //Create result array
            var result = new double[3];

            var absMoneyness = Math.Abs(moneyness);

            var z = SqrtOneOverThree * absMoneyness / sigma;
            var y = Math.Pow(z, 2.0);
            var v = Math.Pow(sigma, 2.0);

            var capitalPhi = NormalDistribution.NormCdf(-z);
            var phi = NormalDistribution.NormPdf(z);

            result[2] = PiOverSix * y / (v * sigma) * capitalPhi *
                        (8 * SqrtThree * sigma * absMoneyness +
                         (3 * v * (v - 8) - 8 * moneyness * moneyness) * capitalPhi / phi) * Math.Exp(2 * y + 0.25 * v);

            if (Math.Abs(sigma) < DenormalisationCutoff)
            {
                result[1] = 1.0;
                result[0] = 0.0;
            }
            else
            {
                var capitalPhi2 = Math.Pow(capitalPhi, 2.0);

                result[1] = TwoPi * y * capitalPhi2 * Math.Exp(y + 0.125 * sigma * sigma);

                if (Math.Abs(moneyness) < DenormalisationCutoff) result[0] = 0.0;
                else
                    result[0] = TwoPiOverSqrtTwentySeven * absMoneyness * (capitalPhi2 * capitalPhi);
            }

            return result;
        }

        /// <summary>
        ///     Computes the f upper map with derivatives.
        /// </summary>
        /// <param name="moneyness">The moneyness.</param>
        /// <param name="sigma">The sigma.</param>
        /// <returns>System.Double[].</returns>
        private double[] ComputeFUpperMapDerivatives(double moneyness, double sigma)
        {
            //Create result array
            var result = new double[3];

            result[0] = NormalDistribution.NormCdf(-0.5 * sigma);

            if (Math.Abs(moneyness) < double.Epsilon)
            {
                result[1] = -0.5;
                result[2] = 0.0;
            }
            else
            {
                var w = Math.Pow(moneyness / sigma, 2.0);
                result[1] = -0.5 * Math.Exp(0.5 * w);
                result[2] = SqrtPiOverTwo * Math.Exp(w + 0.125 * sigma * sigma) * w / sigma;
            }

            return result;
        }

        /// <summary>
        ///     Inverse of the f lower map.
        /// </summary>
        /// <returns>System.Double.</returns>
        private double InverseFLowerMap(double x, double f)
        {
            return Math.Abs(f) < double.Epsilon
                ? 0.0
                : Math.Abs(x / (SqrtThree * NormalDistribution.InverseNormCdf(
                                    Math.Pow(f / (TwoPiOverSqrtTwentySeven * Math.Abs(x)), 1.0) / 3.0)));
        }

        /// <summary>
        ///     Inverse of the f upper map.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>System.Double.</returns>
        private double InverseFUpperMap(double f)
        {
            return -2.0 * NormalDistribution.InverseNormCdf(f);
        }

        /// <summary>
        ///     Minimum cubic control parameter for rational function fitting.
        /// </summary>
        /// <param name="dL">The d l.</param>
        /// <param name="dR">The d r.</param>
        /// <param name="sigma">The s.</param>
        /// <param name="preferShapePreservationOverSmoothness">if set to <c>true</c> [prefer shape preservation over smoothness].</param>
        /// <returns>System.Double.</returns>
        private static double MinimumRationalCubicControlParameter(double dL, double dR, double sigma,
            bool preferShapePreservationOverSmoothness)
        {
            var monotonic = dL * sigma >= 0 && dR * sigma >= 0;
            var convex = dL <= sigma && sigma <= dR;
            var concave = dL >= sigma && sigma >= dR;

            // If 3==r_non_shape_preserving_target, this means revert to standard cubic.
            if (!monotonic && !convex && !concave)
                return MinimumRationalCubicControlParameterValue;

            var dRmdl = dR - dL;
            var dRms = dR - sigma;
            var sMdl = sigma - dL;

            var r1 = -double.MaxValue;
            var r2 = r1;

            // If monotonicity on this interval is possible, set r1 to satisfy the monotonicity condition (3.8).
            if (monotonic)
                if (Math.Abs(sigma) > double.Epsilon)
                    r1 = (dR + dL) / sigma;
                // If division by zero would occur, and shape preservation is preferred, set value to enforce linear interpolation.
                else if (preferShapePreservationOverSmoothness)
                    r1 = MaximumRationalCubicControlParameterValue;

            if (convex || concave)
            {
                // (3.18), avoiding division by zero.
                if (!(Math.Abs(sMdl) < double.Epsilon || Math.Abs(dRms) < double.Epsilon))
                    r2 = Math.Max(Math.Abs(dRmdl / dRms), Math.Abs(dRmdl / sMdl));
                else if (preferShapePreservationOverSmoothness)
                    r2 = MaximumRationalCubicControlParameterValue; // This value enforces linear interpolation.
            }
            else if (preferShapePreservationOverSmoothness)
            {
                // This enforces linear interpolation along segments that are inconsistent with the slopes on the boundaries, e.g., a perfectly horizontal segment that has negative slopes on either edge.
                r2 = MaximumRationalCubicControlParameterValue;
            }

            return Math.Max(MinimumRationalCubicControlParameterValue, Math.Max(r1, r2));
        }

        /// <summary>
        ///     Cubic rational function fit which will match 2nd derivative at the inflection point from the left hand side.
        /// </summary>
        /// <param name="xL">The x l.</param>
        /// <param name="xR">The x r.</param>
        /// <param name="yL">The y l.</param>
        /// <param name="yR">The y r.</param>
        /// <param name="dL">The d l.</param>
        /// <param name="dR">The d r.</param>
        /// <param name="secondDerivativeL">The second derivative l.</param>
        /// <param name="preferShapePreservationOverSmoothness">if set to <c>true</c> [prefer shape preservation over smoothness].</param>
        /// <returns>System.Double.</returns>
        private static double ConvexRationalCubicControlParameterToFitSecondDerivativeAtLeftSide(double xL, double xR,
            double yL, double yR, double dL, double dR, double secondDerivativeL,
            bool preferShapePreservationOverSmoothness)
        {
            var r = RationalCubicControlParameterToFitSecondDerivativeAtLeftSide(xL, xR, yL, yR, dL, dR,
                secondDerivativeL);

            var rMin = MinimumRationalCubicControlParameter(dL, dR, (yR - yL) / (xR - xL),
                preferShapePreservationOverSmoothness);

            return Math.Max(r, rMin);
        }

        /// <summary>
        ///     Cubic rational function fit which will match 2nd derivative at the inflection point from the right hand side.
        /// </summary>
        /// <param name="xL">The x l.</param>
        /// <param name="xR">The x r.</param>
        /// <param name="yL">The y l.</param>
        /// <param name="yR">The y r.</param>
        /// <param name="dL">The d l.</param>
        /// <param name="dR">The d r.</param>
        /// <param name="secondDerivativeR">The second derivative r.</param>
        /// <param name="preferShapePreservationOverSmoothness">if set to <c>true</c> [prefer shape preservation over smoothness].</param>
        /// <returns>System.Double.</returns>
        private static double ConvexRationalCubicControlParameterToFitSecondDerivativeAtRightSide(double xL, double xR,
            double yL, double yR, double dL, double dR, double secondDerivativeR,
            bool preferShapePreservationOverSmoothness)
        {
            var r = RationalCubicControlParameterToFitSecondDerivativeAtRightSide(xL, xR, yL, yR, dL, dR,
                secondDerivativeR);

            var rMin = MinimumRationalCubicControlParameter(dL, dR, (yR - yL) / (xR - xL),
                preferShapePreservationOverSmoothness);

            return Math.Max(r, rMin);
        }

        /// <summary>
        ///     Cubic rational function fit which will match 2nd derivative at the inflection point from the left hand side.
        /// </summary>
        /// <param name="xL">The x l.</param>
        /// <param name="xR">The x r.</param>
        /// <param name="yL">The y l.</param>
        /// <param name="yR">The y r.</param>
        /// <param name="dL">The d l.</param>
        /// <param name="dR">The d r.</param>
        /// <param name="secondDerivativeL">The second derivative l.</param>
        /// <returns>System.Double.</returns>
        private static double RationalCubicControlParameterToFitSecondDerivativeAtLeftSide(double xL, double xR,
            double yL, double yR, double dL, double dR, double secondDerivativeL)
        {
            var h = xR - xL;
            var numerator = 0.5 * h * secondDerivativeL + (dR - dL);

            if (Math.Abs(numerator) < double.Epsilon) return 0.0;

            var denominator = (yR - yL) / h - dL;

            if (Math.Abs(denominator) < double.Epsilon)
                return numerator > 0
                    ? MaximumRationalCubicControlParameterValue
                    : MinimumRationalCubicControlParameterValue;

            return numerator / denominator;
        }

        /// <summary>
        ///     Cubic rational function fit which will match 2nd derivative at the inflection point from the right hand side.
        /// </summary>
        /// <param name="xL">The x l.</param>
        /// <param name="xR">The x r.</param>
        /// <param name="yL">The y l.</param>
        /// <param name="yR">The y r.</param>
        /// <param name="dL">The d l.</param>
        /// <param name="dR">The d r.</param>
        /// <param name="secondDerivativeR">The second derivative r.</param>
        /// <returns>System.Double.</returns>
        private static double RationalCubicControlParameterToFitSecondDerivativeAtRightSide(double xL, double xR,
            double yL, double yR, double dL, double dR, double secondDerivativeR)
        {
            var h = xR - xL;
            var numerator = 0.5 * h * secondDerivativeR + (dR - dL);

            if (Math.Abs(numerator) < double.Epsilon) return 0.0;

            var denominator = dR - (yR - yL) / h;

            if (Math.Abs(denominator) < double.Epsilon)
                return numerator > 0.0
                    ? MaximumRationalCubicControlParameterValue
                    : MinimumRationalCubicControlParameterValue;

            return numerator / denominator;
        }

        /// <summary>
        ///     Rational cubic interpolation.
        /// </summary>
        /// <param name="moneyness">The x.</param>
        /// <param name="xL">The x l.</param>
        /// <param name="xR">The x r.</param>
        /// <param name="yL">The y l.</param>
        /// <param name="yR">The y r.</param>
        /// <param name="dL">The d l.</param>
        /// <param name="dR">The d r.</param>
        /// <param name="r">The r.</param>
        /// <returns>System.Double.</returns>
        private static double RationalCubicInterpolation(double moneyness, double xL, double xR, double yL, double yR,
            double dL, double dR, double r)
        {
            var h = xR - xL;

            if (Math.Abs(h) <= 0) return 0.5 * (yL + yR);

            // r should be greater than -1. We do not use  assert(r > -1)  here in order to allow values such as NaN to be propagated as they should.
            var t = (moneyness - xL) / h;

            if (r >= MaximumRationalCubicControlParameterValue) return yR * t + yL * (1 - t);

            t = (moneyness - xL) / h;
            var omt = 1 - t;
            var t2 = t * t;
            var omt2 = omt * omt;

            // Formula (2.4) divided by formula (2.5)
            return (yR * t2 * t + (r * yR - h * dR) * t2 * omt + (r * yL + h * dL) * t * omt2 + yL * omt2 * omt) /
                   (1 + (r - 3) * t * omt);
            // Linear interpolation without over-or underflow.
        }

        /// <summary>
        ///     Get the Householder factor
        /// </summary>
        /// <param name="newton">The newton.</param>
        /// <param name="halley">The halley.</param>
        /// <param name="hh3">The HH3.</param>
        /// <returns>System.Double.</returns>
        private static double HouseholderFactor(double newton, double halley, double hh3)
        {
            return HouseholderMethodOrder > 3
                ? (1 + 0.5 * halley * newton) / (1 + newton * (halley + hh3 * newton / 6))
                : (HouseholderMethodOrder > 2 ? 1 / (1 + 0.5 * halley * newton) : 1);
        }

        /// <summary>
        ///     // See http://en.wikipedia.org/wiki/Householder%27s_method for a detailed explanation of the third order
        ///     Householder iteration.
        ///     Given the objective function g(s) whose root x such that 0 = g(s) we seek, iterate
        ///     s_n+1  =  s_n  -  (g/g') · [ 1 - (g''/g')·(g/g') ] / [ 1 - (g/g')·( (g''/g') - (g'''/g')·(g/g')/6 ) ]
        ///     Denoting  newton:=-(g/g'), halley:=(g''/g'), and hh3:=(g'''/g'), this reads
        ///     s_n+1  =  s_n  +  newton · [ 1 + halley·newton/2 ] / [ 1 + newton·( halley + hh3·newton/6 ) ]
        ///     NOTE that this function returns 0 when beta intrinsic without any safety checks.
        /// </summary>
        /// <param name="beta">The beta.</param>
        /// <param name="moneyness">The moneyness.</param>
        /// <param name="q">The q.</param>
        /// <param name="n">The n.</param>
        /// <returns>System.Double.</returns>
        private double UncheckedNormalizedImpliedVolatility(double beta, double moneyness, int q, int n)
        {
            // Subtract intrinsic.
            if (q * moneyness > 0)
            {
                beta = Math.Abs(Math.Max(beta - PeterJaeckelPricingEngine.NormalisedIntrinsic(moneyness, q), 0.0));
                q = -q;
            }

            // Map puts to calls
            if (q < 0) moneyness = -moneyness;

            // For negative or zero prices we return 0.
            if (beta <= 0) return 0.0;

            // For positive but denormalised (a.k.a. 'subnormal') prices, we return 0 since it would be impossible to converge to full machine accuracy anyway.
            if (beta < DenormalisationCutoff) return 0.0;

            //Maximum value
            var bMax = Math.Exp(0.5 * moneyness);

            //Return max double vlaue if beta is above the upper bound
            if (beta >= bMax) return double.MaxValue;

            //Start the Householder iterations
            var iterations = 0;
            var directionReversalCount = 0;
            var f = double.MinValue;
            var sigma = double.MinValue;
            var ds = sigma;
            var dsPrevious = 0.0;
            var sLeft = double.MinValue;
            var sRight = double.MaxValue;

            // The temptation is great to use the optimised form b_c = exp(x/2)/2-exp(-x/2)·Phi(sqrt(-2·x)) but that would require implementing all of the above types of round-off and over/underflow handling for this expression, too.
            var sC = Math.Sqrt(Math.Abs(2 * moneyness));
            var bC = PeterJaeckelPricingEngine.NormalisedBlackCall(moneyness, sC);
            var vC = PeterJaeckelPricingEngine.NormalisedVega(moneyness, sC);

            //Decide which of the four branches to use
            if (beta < bC)
            {
                var sL = sC - bC / vC;
                var bL = PeterJaeckelPricingEngine.NormalisedBlackCall(moneyness, sL);

                if (beta < bL)
                {
                    //First (left) branch - the small value limit
                    var fLowerMapDerivatives = ComputeFLowerMapDerivatives(moneyness, sL);

                    var rLl = ConvexRationalCubicControlParameterToFitSecondDerivativeAtRightSide(0.0, bL, 0.0,
                        fLowerMapDerivatives[0], 1.0, fLowerMapDerivatives[1], fLowerMapDerivatives[2], true);

                    f = RationalCubicInterpolation(beta, 0.0, bL, 0.0, fLowerMapDerivatives[0], 1.0,
                        fLowerMapDerivatives[1], rLl);

                    if (f <= 0.0)
                    {
                        // This can happen due to roundoff truncation for extreme values such as |x|>500.
                        // We switch to quadratic interpolation using f(0)≡0, f(b_l), and f'(0)≡1 to specify the quadratic.
                        var t = beta / bL;
                        f = (fLowerMapDerivatives[0] * t + bL * (1 - t)) * t;
                    }

                    sigma = InverseFLowerMap(moneyness, f);
                    sRight = sL;

                    //
                    // In this branch, which comprises the lowest segment, the objective function is
                    //     g(s) = 1/ln(b(x,s)) - 1/ln(beta)
                    //          ≡ 1/ln(b(s)) - 1/ln(beta)
                    // This makes
                    //              g'               =   -b'/(b·ln(b)²)
                    //              newton = -g/g'   =   (ln(beta)-ln(b))·ln(b)/ln(beta)·b/b'
                    //              halley = g''/g'  =   b''/b'  -  b'/b·(1+2/ln(b))
                    //              hh3    = g'''/g' =   b'''/b' +  2(b'/b)²·(1+3/ln(b)·(1+1/ln(b)))  -  3(b''/b)·(1+2/ln(b))
                    //
                    // The Householder(3) iteration is
                    //     s_n+1  =  s_n  +  newton · [ 1 + halley·newton/2 ] / [ 1 + newton·( halley + hh3·newton/6 ) ]
                    //
                    for (; iterations < n && Math.Abs(ds) > double.Epsilon * sigma; ++iterations)
                    {
                        if (ds * dsPrevious < 0) ++directionReversalCount;

                        if (iterations > 0 && (3 == directionReversalCount || !(sigma > sLeft && sigma < sRight)))
                        {
                            // If looping inefficently, or the forecast step takes us outside the bracket, or onto its edges, switch to binary nesting.
                            // NOTE that this can only really happen for very extreme values of |x|, such as |x| = |ln(F/K)| > 500.
                            sigma = 0.5 * (sLeft + sRight);
                            if (sRight - sLeft <= double.Epsilon * sigma) break;

                            directionReversalCount = 0;
                            ds = 0;
                        }

                        dsPrevious = ds;
                        var b = PeterJaeckelPricingEngine.NormalisedBlackCall(moneyness, sigma);
                        var bp = PeterJaeckelPricingEngine.NormalisedVega(moneyness, sigma);

                        if (b > beta && sigma < sRight) sRight = sigma;
                        else if (b < beta && sigma > sLeft)
                            sLeft = sigma; // Tighten the bracket if applicable.

                        // Numerical underflow. Switch to binary nesting for this iteration.
                        if (b <= 0 || bp <= 0)
                        {
                            ds = 0.5 * (sLeft + sRight) - sigma;
                        }
                        else
                        {
                            var lnB = Math.Log(b);
                            var lnBeta = Math.Log(beta);
                            var bpob = bp / b;
                            var h = moneyness / sigma;
                            var bHalley = h * h / sigma - sigma / 4;
                            var newton = (lnBeta - lnB) * lnB / lnBeta / bpob;
                            var halley = bHalley - bpob * (1 + 2 / lnB);
                            var bHh3 = bHalley * bHalley - 3 * Math.Pow(h / sigma, 2.0) - 0.25;
                            var hh3 = bHh3 + 2 * Math.Pow(bpob, 2.0) * (1 + 3 / lnB * (1 + 1 / lnB)) -
                                      3 * bHalley * bpob * (1 + 2 / lnB);
                            ds = newton * HouseholderFactor(newton, halley, hh3);
                        }

                        sigma += ds = Math.Max(-0.5 * sigma, ds);
                    }

                    return sigma;
                }

                var vL = PeterJaeckelPricingEngine.NormalisedVega(moneyness, sL);
                var rLm = ConvexRationalCubicControlParameterToFitSecondDerivativeAtRightSide(bL, bC, sL, sC, 1 / vL,
                    1 / vC, 0.0, false);

                sigma = RationalCubicInterpolation(beta, bL, bC, sL, sC, 1 / vL, 1 / vC, rLm);
                sLeft = sL;
                sRight = sC;
            }
            else
            {
                var sH = vC > double.Epsilon ? sC + (bMax - bC) / vC : sC;
                var bH = PeterJaeckelPricingEngine.NormalisedBlackCall(moneyness, sH);

                if (beta <= bH)
                {
                    var vH = PeterJaeckelPricingEngine.NormalisedVega(moneyness, sH);
                    var rHm = ConvexRationalCubicControlParameterToFitSecondDerivativeAtLeftSide(bC, bH, sC, sH, 1 / vC,
                        1 / vH, 0.0, false);

                    sigma = RationalCubicInterpolation(beta, bC, bH, sC, sH, 1 / vC, 1 / vH, rHm);
                    sLeft = sC;
                    sRight = sH;
                }
                else
                {
                    var fUpperMapDerivatives = ComputeFUpperMapDerivatives(moneyness, sH);

                    if (fUpperMapDerivatives[2] > -Math.Sqrt(double.MaxValue) &&
                        fUpperMapDerivatives[2] < Math.Sqrt(double.MaxValue))
                    {
                        var rHh = ConvexRationalCubicControlParameterToFitSecondDerivativeAtLeftSide(bH, bMax,
                            fUpperMapDerivatives[0], 0.0, fUpperMapDerivatives[1], -0.5, fUpperMapDerivatives[2], true);
                        f = RationalCubicInterpolation(beta, bH, bMax, fUpperMapDerivatives[0], 0.0,
                            fUpperMapDerivatives[1], -0.5, rHh);
                    }

                    if (f <= 0)
                    {
                        var h = bMax - bH;
                        var t = (beta - bH) / h;

                        // We switch to quadratic interpolation using f(b_h), f(b_max)≡0, and f'(b_max)≡-1/2 to specify the quadratic.
                        f = (fUpperMapDerivatives[0] * (1 - t) + 0.5 * h * t) * (1 - t);
                    }

                    sigma = InverseFUpperMap(f);
                    sLeft = sH;

                    if (beta > 0.5 * bMax)
                    {
                        // Else we better drop through and let the objective function be g(s) = b(x,s)-beta. 
                        //
                        // In this branch, which comprises the upper segment, the objective function is
                        //     g(s) = ln(b_max-beta)-ln(b_max-b(x,s))
                        //          ≡ ln((b_max-beta)/(b_max-b(s)))
                        // This makes
                        //              g'               =   b'/(b_max-b)
                        //              newton = -g/g'   =   ln((b_max-b)/(b_max-beta))·(b_max-b)/b'
                        //              halley = g''/g'  =   b''/b'  +  b'/(b_max-b)
                        //              hh3    = g'''/g' =   b'''/b' +  g'·(2g'+3b''/b')
                        // and the iteration is
                        //     s_n+1  =  s_n  +  newton · [ 1 + halley·newton/2 ] / [ 1 + newton·( halley + hh3·newton/6 ) ].
                        //
                        for (; iterations < n && Math.Abs(ds) > double.Epsilon * sigma; ++iterations)
                        {
                            if (ds * dsPrevious < 0) ++directionReversalCount;

                            if (iterations > 0 && (3 == directionReversalCount || !(sigma > sLeft && sigma < sRight)))
                            {
                                // If looping inefficently, or the forecast step takes us outside the bracket, or onto its edges, switch to binary nesting.
                                // NOTE that this can only really happen for very extreme values of |x|, such as |x| = |ln(F/K)| > 500.
                                sigma = 0.5 * (sLeft + sRight);
                                if (sRight - sLeft <= double.Epsilon * sigma) break;
                                directionReversalCount = 0;
                                ds = 0;
                            }

                            dsPrevious = ds;
                            var b = PeterJaeckelPricingEngine.NormalisedBlackCall(moneyness, sigma);
                            var bp = PeterJaeckelPricingEngine.NormalisedVega(moneyness, sigma);

                            if (b > beta && sigma < sRight) sRight = sigma;
                            else if (b < beta && sigma > sLeft)
                                sLeft = sigma; // Tighten the bracket if applicable.

                            // Numerical underflow. Switch to binary nesting for this iteration.
                            if (b >= bMax || bp <= double.Epsilon)
                            {
                                ds = 0.5 * (sLeft + sRight) - sigma;
                            }
                            else
                            {
                                var bMaxMinusB = bMax - b;
                                var g = Math.Log((bMax - beta) / bMaxMinusB);
                                var gp = bp / bMaxMinusB;

                                var bHalley = Math.Pow(moneyness / sigma, 2.0) / sigma - sigma / 4;
                                var bHh3 = bHalley * bHalley - 3 * Math.Pow(moneyness / (sigma * sigma), 2.0) - 0.25;
                                var newton = -g / gp;
                                var halley = bHalley + gp;
                                var hh3 = bHh3 + gp * (2 * gp + 3 * bHalley);
                                ds = newton * HouseholderFactor(newton, halley, hh3);
                            }

                            sigma += ds = Math.Max(-0.5 * sigma, ds);
                        }

                        return sigma;
                    }
                }
            }

            // In this branch, which comprises the two middle segments, the objective function is g(s) = b(x,s)-beta, or g(s) = b(s) - beta, for short.
            // This makes
            //              newton = -g/g'   =  -(b-beta)/b'
            //              halley = g''/g'  =    b''/b'    =  x²/s³-s/4
            //              hh3    = g'''/g' =    b'''/b'   =  halley² - 3·(x/s²)² - 1/4
            // and the iteration is
            //     s_n+1  =  s_n  +  newton · [ 1 + halley·newton/2 ] / [ 1 + newton·( halley + hh3·newton/6 ) ].
            //
            for (; iterations < n && Math.Abs(ds) > double.Epsilon * sigma; ++iterations)
            {
                if (ds * dsPrevious < 0) ++directionReversalCount;

                if (iterations > 0 && (3 == directionReversalCount || !(sigma > sLeft && sigma < sRight)))
                {
                    // If looping inefficently, or the forecast step takes us outside the bracket, or onto its edges, switch to binary nesting.
                    // NOTE that this can only really happen for very extreme values of |x|, such as |x| = |ln(F/K)| > 500.
                    sigma = 0.5 * (sLeft + sRight);

                    if (sRight - sLeft <= double.Epsilon * sigma) break;
                    directionReversalCount = 0;
                    ds = 0;
                }

                dsPrevious = ds;
                var b = PeterJaeckelPricingEngine.NormalisedBlackCall(moneyness, sigma);
                var bp = PeterJaeckelPricingEngine.NormalisedVega(moneyness, sigma);

                if (b > beta && sigma < sRight) sRight = sigma;
                else if (b < beta && sigma > sLeft) sLeft = sigma; // Tighten the bracket if applicable.

                var newton = (beta - b) / bp;
                var halley = Math.Pow(moneyness / sigma, 2.0) / sigma - sigma / 4;
                var hh3 = halley * halley - 3 * Math.Pow(moneyness / (sigma * sigma), 2.0) - 0.25;
                sigma += ds = Math.Max(-0.5 * sigma, newton * HouseholderFactor(newton, halley, hh3));
            }

            return sigma;
        }
    }
}