using System;

namespace IReserachCore.Instruments.Options
{
    /// <summary>
    /// </summary>
    public interface ISingleAssetOption
    {
        eCurrency Currency { get; set; }
        double ImpliedVolatility { get; set; }
        DateTime Maturity { get; set; }
        double Nominal { get; set; }
        eOptionType OptionType { get; set; }
        double Premium { get; set; }
        double Strike { get; set; }
        DateTime ValuationDate { get; set; }
        string Underlying { get; set; }
        double StockPrice { get; set; }
        double DiscountFactor { get; set; }
        double DividendDiscountFactor { get; set; }
    }
}