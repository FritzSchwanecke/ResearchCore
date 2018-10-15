using System;
using IReserachCore.Helper.Functions;
using IReserachCore.Instruments.Options;
using IReserachCore.Pricer.BlackScholes;
using ResearchCore.Helper.Functions;
using Unity.Attributes;

namespace ResearchCore.Pricer.BlackScholes
{

    public class PeterJaeckleAnalyticalPricing : IPeterJaeckleAnalyticalPricing
    {


        /// <summary>
        ///     The one over SQRT two
        /// </summary>
        private static double OneOverSqrtTwo { get; } = 0.7071067811865475244008443621048490392848359376887;

        /// <summary>
        ///     The one over SQRT two pi
        /// </summary>
        private static double OneOverSqrtTwoPi { get; } = 0.3989422804014326779399460599343818684758586311649;

        /// <summary>
        ///     The SQRT two pi
        /// </summary>
        private static double SqrtTwoPi { get; } = 2.506628274631000502415765284811045253006986740610;

        private static double SqrtEpsilon { get; } = Math.Sqrt(double.Epsilon);

        private static double FourthRootEpsilon { get; } = Math.Sqrt(SqrtEpsilon);

        private static double EighthRootEpsilon { get; } = Math.Sqrt(FourthRootEpsilon);

        private static double SixteenthRootEpsilon { get; } = Math.Sqrt(EighthRootEpsilon);


        private static double AsymptoticExpansionAccuracyThreshold { get; } = -10.0;

        private static double SmalltExpansionOfNormalizedBlackThreshold { get; } = 2.0 * SixteenthRootEpsilon;

        private static double DenormalizationCutoff { get; } = 0.0;

        /// <summary>
        ///     Asymptotics the expansion of normalised black call.
        ///     b  =  Φ(h+t)·exp(x/2) - Φ(h-t)·exp(-x/2)
        ///     with
        ///     h  =  x/s   and   t  =  s/2
        ///     which makes
        ///     b  =  Φ(h+t)·exp(h·t) - Φ(h-t)·exp(-h·t)
        ///     exp(-(h²+t²)/2)
        ///     =  ---------------  ·  [ Y(h+t) - Y(h-t) ]
        ///     √(2π)
        ///     with
        ///     Y(z) := Φ(z)/φ(z)
        ///     for large negative (t-|h|) by the aid of Abramowitz &amp; Stegun (26.2.12) where Φ(z) = φ(z)/|z|·[1-1/z^2+...].
        ///     We define
        ///     r
        ///     A(h,t) :=  --- · [ Y(h+t) - Y(h-t) ]
        ///     t
        ///     with r := (h+t)·(h-t) and give an expansion for A(h,t) in q:=(h/r)² expressed in terms of e:=(t/h)² .
        /// </summary>
        /// <param name="h">The h.</param>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        private double AsymptoticExpansionOfNormalisedBlackCall(double h, double t)
        {
            var e = t / h * (t / h);
            var r = (h + t) * (h - t);
            var q = h / r * (h / r);

            // 17th order asymptotic expansion of A(h,t) in q, sufficient for Φ(h) [and thus y(h)] to have relative accuracy of 1.64E-16 for h <= η  with  η:=-10.
            var asymptoticExpansionSum = 2.0 + q * (-6.0E0 - 2.0 * e + 3.0 * q *
                                                    (1.0E1 + e * (2.0E1 + 2.0 * e) + 5.0 * q *
                                                     (-1.4E1 + e * (-7.0E1 + e * (-4.2E1 - 2.0 * e)) + 7.0 * q *
                                                      (1.8E1 + e * (1.68E2 + e * (2.52E2 + e * (7.2E1 + 2.0 * e))) +
                                                       9.0 * q *
                                                       (-2.2E1 +
                                                        e * (-3.3E2 + e *
                                                             (-9.24E2 + e * (-6.6E2 + e * (-1.1E2 - 2.0 * e)))) +
                                                        1.1E1 * q *
                                                        (2.6E1 +
                                                         e * (5.72E2 + e *
                                                              (2.574E3 + e *
                                                               (3.432E3 + e * (1.43E3 + e * (1.56E2 + 2.0 * e))))) +
                                                         1.3E1 * q *
                                                         (-3.0E1 +
                                                          e * (-9.1E2 + e *
                                                               (-6.006E3 + e *
                                                                (-1.287E4 + e *
                                                                 (-1.001E4 + e * (-2.73E3 + e * (-2.1E2 - 2.0 * e)))))
                                                          ) + 1.5E1 * q *
                                                          (3.4E1 +
                                                           e * (1.36E3 + e *
                                                                (1.2376E4 + e *
                                                                 (3.8896E4 + e *
                                                                  (4.862E4 + e *
                                                                   (2.4752E4 + e * (4.76E3 + e * (2.72E2 + 2.0 * e))))))
                                                           ) + 1.7E1 * q *
                                                           (-3.8E1 +
                                                            e * (-1.938E3 + e *
                                                                 (-2.3256E4 + e *
                                                                  (-1.00776E5 +
                                                                   e * (-1.84756E5 +
                                                                        e * (-1.51164E5 +
                                                                             e * (-5.4264E4 + e *
                                                                                  (-7.752E3 + e * (-3.42E2 - 2.0 * e))))
                                                                   )))) + 1.9E1 * q *
                                                            (4.2E1 +
                                                             e * (2.66E3 + e *
                                                                  (4.0698E4 + e *
                                                                   (2.3256E5 + e *
                                                                    (5.8786E5 + e *
                                                                     (7.05432E5 + e *
                                                                      (4.0698E5 + e *
                                                                       (1.08528E5 + e *
                                                                        (1.197E4 + e * (4.2E2 + 2.0 * e))))))))) +
                                                             2.1E1 * q *
                                                             (-4.6E1 +
                                                              e * (-3.542E3 + e *
                                                                   (-6.7298E4 + e *
                                                                    (-4.90314E5 +
                                                                     e * (-1.63438E6 +
                                                                          e * (-2.704156E6 +
                                                                               e * (-2.288132E6 +
                                                                                    e * (-9.80628E5 +
                                                                                         e * (-2.01894E5 +
                                                                                              e * (-1.771E4 + e *
                                                                                                   (-5.06E2 - 2.0 * e)))
                                                                                    ))))))) + 2.3E1 * q *
                                                              (5.0E1 +
                                                               e * (4.6E3 + e * (1.0626E5 + e *
                                                                                 (9.614E5 + e *
                                                                                  (4.08595E6 + e *
                                                                                   (8.9148E6 + e *
                                                                                    (1.04006E7 + e *
                                                                                     (6.53752E6 + e *
                                                                                      (2.16315E6 + e *
                                                                                       (3.542E5 + e *
                                                                                        (2.53E4 + e * (6.0E2 + 2.0 * e))
                                                                                       ))))))))) + 2.5E1 * q *
                                                               (-5.4E1 +
                                                                e * (-5.85E3 + e *
                                                                     (-1.6146E5 + e *
                                                                      (-1.77606E6 + e *
                                                                       (-9.37365E6 +
                                                                        e * (-2.607579E7 +
                                                                             e * (-4.01166E7 +
                                                                                  e * (-3.476772E7 +
                                                                                       e * (-1.687257E7 +
                                                                                            e * (-4.44015E6 +
                                                                                                 e * (-5.9202E5 + e *
                                                                                                      (-3.51E4 + e *
                                                                                                       (-7.02E2 - 2.0 *
                                                                                                        e)))))))))))) +
                                                                2.7E1 * q *
                                                                (5.8E1 +
                                                                 e * (7.308E3 + e *
                                                                      (2.3751E5 + e *
                                                                       (3.12156E6 + e *
                                                                        (2.003001E7 + e *
                                                                         (6.919458E7 +
                                                                          e * (1.3572783E8 +
                                                                               e * (1.5511752E8 +
                                                                                    e * (1.0379187E8 +
                                                                                         e * (4.006002E7 +
                                                                                              e * (8.58429E6 + e *
                                                                                                   (9.5004E5 + e *
                                                                                                    (4.7502E4 + e *
                                                                                                     (8.12E2 + 2.0 * e))
                                                                                                   ))))))))))) +
                                                                 2.9E1 * q *
                                                                 (-6.2E1 +
                                                                  e * (-8.99E3 + e *
                                                                       (-3.39822E5 + e *
                                                                        (-5.25915E6 + e *
                                                                         (-4.032015E7 + e *
                                                                          (-1.6934463E8 + e *
                                                                           (-4.1250615E8 + e *
                                                                            (-6.0108039E8 +
                                                                             e * (-5.3036505E8 +
                                                                                  e * (-2.8224105E8 +
                                                                                       e * (-8.870433E7 +
                                                                                            e * (-1.577745E7 +
                                                                                                 e * (-1.472562E6 +
                                                                                                      e * (-6.293E4 +
                                                                                                           e * (-9.3E2 -
                                                                                                                2.0 * e)
                                                                                                      ))))))))))))) +
                                                                  3.1E1 * q *
                                                                  (6.6E1 +
                                                                   e * (1.0912E4 + e *
                                                                        (4.74672E5 + e *
                                                                         (8.544096E6 + e *
                                                                          (7.71342E7 + e *
                                                                           (3.8707344E8 + e *
                                                                            (1.14633288E9 + e *
                                                                             (2.07431664E9 +
                                                                              e * (2.33360622E9 +
                                                                                   e * (1.6376184E9 +
                                                                                        e * (7.0963464E8 +
                                                                                             e * (1.8512208E8 +
                                                                                                  e * (2.7768312E7 +
                                                                                                       e * (2.215136E6 +
                                                                                                            e *
                                                                                                            (8.184E4 +
                                                                                                             e *
                                                                                                             (1.056E3 +
                                                                                                              2.0 * e)))
                                                                                                  )))))))))))) +
                                                                   3.3E1 * (-7.0E1 + e *
                                                                            (-1.309E4 + e *
                                                                             (-6.49264E5 + e *
                                                                              (-1.344904E7 + e *
                                                                               (-1.4121492E8 + e *
                                                                                (-8.344518E8 +
                                                                                 e * (-2.9526756E9 +
                                                                                      e * (-6.49588632E9 +
                                                                                           e * (-9.0751353E9 +
                                                                                                e * (-8.1198579E9 +
                                                                                                     e * (-4.6399188E9 +
                                                                                                          e *
                                                                                                          (-1.6689036E9 +
                                                                                                           e *
                                                                                                           (-3.67158792E8 +
                                                                                                            e *
                                                                                                            (-4.707164E7 +
                                                                                                             e *
                                                                                                             (-3.24632E6 +
                                                                                                              e *
                                                                                                              (-1.0472E5 +
                                                                                                               e *
                                                                                                               (-1.19E3 -
                                                                                                                2.0 * e)
                                                                                                              ))))))))))
                                                                                )))))) * q))))))))))))))));
            var b = OneOverSqrtTwoPi * Math.Exp(-0.5 * (h * h + t * t)) * (t / r) * asymptoticExpansionSum;
            return Math.Max(b, 0.0);
        }


        /// <summary>
        ///     Calculation of
        ///     b  =  Φ(h+t)·exp(h·t) - Φ(h-t)·exp(-h·t)
        ///     exp(-(h²+t²)/2)
        ///     =  --------------- ·  [ Y(h+t) - Y(h-t) ]
        ///     √(2π)
        ///     with
        ///     Y(z) := Φ(z)/φ(z)
        ///     using an expansion of Y(h+t)-Y(h-t) for small t to twelvth order in t.
        ///     Theoretically accurate to (better than) precision  ε = 2.23E-16  when  h&lt;=0  and  t &lt; τ  with  τ :=
        ///     2·ε^(1/16) ≈ 0.21.
        ///     The main bottleneck for precision is the coefficient a:=1+h·Y(h) when |h|&gt;1 .
        ///     Smalltexpansions the of normalised black call.
        ///     Y(h) := Φ(h)/φ(h) = √(π/2)·erfcx(-h/√2)
        ///     a := 1+h·Y(h)  --- Note that due to h&lt;0, and h·Y(h) -&gt; -1 (from above) as h -&gt; -∞, we also have that a&gt;
        ///     0 and a -&gt; 0 as h -&gt; -∞
        ///     w := t² , h2 := h²
        /// </summary>
        /// <param name="h">The h.</param>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        private double SmalltexpansionOfNormalisedBlackCall(double h, double t)
        {
            var a = 1 + h * (0.5 * SqrtTwoPi) * Erfc.ErfcxCody(-OneOverSqrtTwo * h);
            var w = t * t;
            var h2 = h * h;
            var expansion = 2 * t * (a + w * ((-1 + 3 * a + a * h2) / 6 + w *
                                              ((-7 + 15 * a + h2 * (-1 + 10 * a + a * h2)) / 120 + w *
                                               ((-57 + 105 * a +
                                                 h2 * (-18 + 105 * a + h2 * (-1 + 21 * a + a * h2))) / 5040 +
                                                w * ((-561 + 945 * a +
                                                      h2 * (-285 + 1260 * a +
                                                            h2 * (-33 + 378 * a + h2 * (-1 + 36 * a + a * h2)))
                                                     ) / 362880 + w *
                                                     ((-6555 + 10395 * a +
                                                       h2 * (-4680 + 17325 * a +
                                                             h2 * (-840 + 6930 * a +
                                                                   h2 * (-52 + 990 * a +
                                                                         h2 * (-1 + 55 * a + a * h2))))) /
                                                      39916800 + (-89055 + 135135 * a +
                                                                  h2 * (-82845 + 270270 * a +
                                                                        h2 * (-20370 + 135135 * a +
                                                                              h2 * (-1926 + 25740 * a +
                                                                                    h2 * (-75 + 2145 * a +
                                                                                          h2 * (-1 + 78 * a +
                                                                                                a * h2)))))) *
                                                      w / 6227020800.0))))));
            var b = OneOverSqrtTwoPi * Math.Exp(-0.5 * (h * h + t * t)) * expansion;
            return Math.Max(b, 0.0);
        }

        //
        // Introduced on 2017-02-18
        //
        //     b(x,s)  =  Φ(x/s+s/2)·exp(x/2)  -   Φ(x/s-s/2)·exp(-x/2)
        //             =  Φ(h+t)·exp(x/2)      -   Φ(h-t)·exp(-x/2)
        //             =  ½ · exp(-u²-v²) · [ erfcx(u-v) -  erfcx(u+v) ]
        //             =  ½ · [ exp(x/2)·erfc(u-v)     -  exp(-x/2)·erfc(u+v)    ]
        //             =  ½ · [ exp(x/2)·erfc(u-v)     -  exp(-u²-v²)·erfcx(u+v) ]
        //             =  ½ · [ exp(-u²-v²)·erfcx(u-v) -  exp(-x/2)·erfc(u+v)    ]
        // with
        //              h  =  x/s ,       t  =  s/2 ,
        // and
        //              u  = -h/√2  and   v  =  t/√2 .
        //
        // Cody's erfc() and erfcx() functions each, for some values of their argument, involve the evaluation
        // of the exponential function exp(). The normalised Black function requires additional evaluation(s)
        // of the exponential function irrespective of which of the above formulations is used. However, the total
        // number of exponential function evaluations can be minimised by a judicious choice of one of the above
        // formulations depending on the input values and the branch logic in Cody's erfc() and erfcx().
        //
        private double NormalisedBlackCallWithOptimalUseOfCodysFunctions(double x, double s)
        {
            const double codysThreshold = 0.46875;
            var h = x / s;
            var t = 0.5 * s;
            var q1 = -OneOverSqrtTwo * (h + t);
            var q2 = -OneOverSqrtTwo * (h - t);
            double twoB;
            if (q1 < codysThreshold)
            {
                if (q2 < codysThreshold)
                    twoB = Math.Exp(0.5 * x) * Erfc.ErfcCody(q1) -
                           Math.Exp(-0.5 * x) * Erfc.ErfcCody(q2);
                else
                    twoB = Math.Exp(0.5 * x) * Erfc.ErfcCody(q1) -
                           Math.Exp(-0.5 * (h * h + t * t)) * Erfc.ErfcCody(q2);
            }
            else
            {
                if (q2 < codysThreshold)
                    twoB = Math.Exp(-0.5 * (h * h + t * t)) * Erfc.ErfcxCody(q1) -
                           Math.Exp(-0.5 * x) * Erfc.ErfcCody(q2);
                else
                    twoB = Math.Exp(-0.5 * (h * h + t * t)) *
                           (Erfc.ErfcxCody(q1) - Erfc.ErfcxCody(q2));
            }



            return Math.Max(0.5 * twoB, 0.0);
        }

        public double NormalisedIntrinsic(double x, double q /* q=±1 */)
        {
            if (q * x <= 0)
                return 0;
            var x2 = x * x;

            if (x2 < 98 * FourthRootEpsilon) // The factor 98 is computed from last coefficient: √√92897280 = 98.1749
                return Math.Max(
                    (q < 0 ? -1 : 1) * x *
                    (1 + x2 * (1.0 / 24.0 + x2 * (1.0 / 1920.0 + x2 * (1.0 / 322560.0 + 1.0 / 92897280.0 * x2)))), 0.0);
            var b_max = Math.Exp(0.5 * x);
            var one_over_b_max = 1 / b_max;
            return Math.Max((q < 0 ? -1 : 1) * (b_max - one_over_b_max), 0.0);
        }

        private double NormalisedIntrinsicCall(double x)
        {
            return NormalisedIntrinsic(x, 1);
        }

        /// <summary>
        ///     Normaliseds the black call.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public double NormalisedBlackCall(double x, double s)
        {
            // Denote h := x/s and t := s/2.
            // We evaluate the condition |h|>|η|, i.e., h<η  &&  t < τ+|h|-|η|  avoiding any divisions by s , where η = asymptotic_expansion_accuracy_threshold  and τ = small_t_expansion_of_normalised_black_threshold .
            if (x < s * AsymptoticExpansionAccuracyThreshold && 0.5 * s * s + x <
                s * (SmalltExpansionOfNormalizedBlackThreshold + AsymptoticExpansionAccuracyThreshold))
            {
                return AsymptoticExpansionOfNormalisedBlackCall(x / s, 0.5 * s);
            }
                
            return 0.5 * s < SmalltExpansionOfNormalizedBlackThreshold
                ? SmalltexpansionOfNormalisedBlackCall(x / s, 0.5 * s)
                : NormalisedBlackCallWithOptimalUseOfCodysFunctions(x , s);
        }

        /// <summary>
        ///     Normaliseds the vega.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public double NormalisedVega(double x, double s)
        {
            var ax = Math.Abs(x);
            return ax <= 0
                ? OneOverSqrtTwoPi * Math.Exp(-0.125 * s * s)
                : (s <= 0 || s <= ax * Math.Sqrt(double.MinValue)
                    ? 0
                    : OneOverSqrtTwoPi * Math.Exp(-0.5 * Math.Pow(x / s, 2) + Math.Pow(0.5 * s, 2)));
        }

        public double NetPresentValue(DateTime valuationDate, ISingleAssetOption option, double underlyingPrice,
            double discountFactor = 1d, double dividendDiscountFactor = 1d)
        {
            var forwardPrice = underlyingPrice * dividendDiscountFactor / discountFactor;
            var strike = option.Strike;
            var tenor = (option.Maturity - option.ValuationDate).TotalDays / 365.0;
            var intrinsicValue = option.OptionType == eOptionType.C
                ? forwardPrice - strike
                : strike - forwardPrice;

            if (intrinsicValue <= 0.0 && option.OptionType == eOptionType.C)
                return discountFactor *
                       NetOtmCallValueNormalized(option.ImpliedVolatility, forwardPrice, strike, tenor) *
                       Math.Sqrt(forwardPrice * strike);

            if (intrinsicValue >= 0.0 && option.OptionType == eOptionType.P)
                return discountFactor *
                       NetOtmPutValueNormalized(option.ImpliedVolatility, forwardPrice, strike, tenor) *
                       Math.Sqrt(forwardPrice * strike);

            if (intrinsicValue > 0.0 && option.OptionType == eOptionType.C)
                return discountFactor * intrinsicValue +
                       this.NetOtmPutValueNormalized(option.ImpliedVolatility, forwardPrice, strike, tenor);

            if (intrinsicValue < 0.0 && option.OptionType == eOptionType.P)
                return discountFactor * intrinsicValue +
                       this.NetOtmCallValueNormalized(option.ImpliedVolatility, forwardPrice, strike, tenor);

            return 0.0;
        }



        /// <summary>
        ///     Nets the otm call value normalized.
        /// </summary>
        /// <param name="valuationDate">The valuation date.</param>
        /// <param name="volatility">The volatility.</param>
        /// <param name="forwardPrice">The forward price.</param>
        /// <param name="strike">The strike.</param>
        /// <param name="tenor">The tenor.</param>
        /// <param name="discountFactor">The discount factor.</param>
        /// <param name="dividendDiscountFactor">The dividend discount factor.</param>
        /// <returns></returns>
        private double NetOtmCallValueNormalized(double volatility, double forwardPrice, double strike, double tenor)
        {
            var moneyness = Math.Log(forwardPrice / strike);

            var sigma = volatility * Math.Sqrt(tenor);

            return sigma <= Math.Abs(moneyness) * DenormalizationCutoff
                ? NormalisedIntrinsicCall(moneyness)
                : NormalisedBlackCall(moneyness, sigma);
        }

        /// <summary>
        ///     Nets the otm put value normalized.
        /// </summary>
        /// <param name="valuationDate">The valuation date.</param>
        /// <param name="volatility">The volatility.</param>
        /// <param name="forwardPrice">The forward price.</param>
        /// <param name="strike">The strike.</param>
        /// <param name="tenor">The tenor.</param>
        /// <param name="discountFactor">The discount factor.</param>
        /// <param name="dividendDiscountFactor">The dividend discount factor.</param>
        /// <returns></returns>
        private double NetOtmPutValueNormalized(double volatility, double forwardPrice, double strike, double tenor)
        {
            var moneyness = Math.Log(strike/ forwardPrice);
            var sigma = volatility * Math.Sqrt(tenor);

            return sigma <= Math.Abs(moneyness) * DenormalizationCutoff
                ? NormalisedIntrinsic(moneyness, -1.0)
                : NormalisedBlackCall(moneyness, sigma);
        }
    }
}