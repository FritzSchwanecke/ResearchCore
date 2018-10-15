using System;
using IReserachCore.Instruments;
using IReserachCore.Instruments.Options;

namespace ResearchCore.Instruments.Options
{
    /// <summary>
    /// </summary>
    /// <seealso cref="IReserachCore.Instruments.Options.ISingleAssetOption" />
    public class SingleAssetOption : ISingleAssetOption
    {
        public DateTime ValuationDate { get; set; }
        public DateTime Maturity { get; set; }
        public double Strike { get; set; }
        public eCurrency Currency { get; set; }
        public double Nominal { get; set; }
        public double Premium { get; set; }
        public double ImpliedVolatility { get; set; }
        public eOptionType OptionType { get; set; }
        public string Underlying { get; set; }
        public double StockPrice { get; set; }
        public double DiscountFactor { get; set; }
        public double DividendDiscountFactor { get; set; }
    }
}