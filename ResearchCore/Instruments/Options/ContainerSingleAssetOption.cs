using System;
using System.Collections.Generic;
using IReserachCore.Instruments.Options;

namespace ResearchCore.Instruments.Options
{
    public class ContainerSingleAssetOption : IContainerSingleAssetOption
    {
        public string Underlying { get; set; }

        public DateTime StartValueDate { get; set; }
        public DateTime EndValueDate { get; set; }

        public int MaxStrikeIndex { get; set; }
        public Dictionary<double, int> StrikeToIndexDictionary { get; set; }
        public Dictionary<int, double> IndexToStrikeDictionary { get; set; }

        public int MaxMaturityIndex { get; set; }
        public Dictionary<DateTime, int> MaturityToIndexDictionary { get; set; }
        public Dictionary<int, DateTime> IndexToMaturityDictionary { get; set; }

        public bool[,,] CallOptionBool { get; set; }
        public double[,,] CallOptionPremium { get; set; }
        public double[,,] CallOptionImpliedVol { get; set; }

        public bool[,,] PutOptionBool { get; set; }
        public double[,,] PutOptionPremium { get; set; }
        public double[,,] PutOptionImpliedVol { get; set; }

        public double[] StockPrice { get; set; }
        public double[,] RateDiscountFactor { get; set; }
        public double[,] DividendDiscountFactor { get; set; }

        public int GetValuationDateIndex(DateTime valuationDate)
        {
            return (int) (valuationDate - this.StartValueDate).TotalDays;
        }

        public DateTime GetIndexValuationDate(int valuationDateIndex)
        {
            return this.StartValueDate.AddDays(valuationDateIndex);
        }
    }
}