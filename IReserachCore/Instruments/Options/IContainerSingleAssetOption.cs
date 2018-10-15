using System;
using System.Collections.Generic;

namespace IReserachCore.Instruments.Options
{
    public interface IContainerSingleAssetOption
    {
        string Underlying { get; set; }


        /// <summary>
        /// Gets or sets the start value date.
        /// </summary>
        /// <value>
        /// Timeperiod for which option values are given
        /// </value>
        DateTime StartValueDate { get; set; }
        DateTime EndValueDate { get; set; }


        /// <summary>
        /// Gets or sets the index of the call strike.
        /// </summary>
        /// <value>
        /// The index of the call strike.
        /// </value>
        int MaxStrikeIndex { get; set; }
        Dictionary<double, int> StrikeToIndexDictionary { get; set; }
        Dictionary<int, double> IndexToStrikeDictionary { get; set; }

        int MaxMaturityIndex { get; set; }
        Dictionary<DateTime, int> MaturityToIndexDictionary { get; set; }
        Dictionary<int, DateTime> IndexToMaturityDictionary { get; set; }

        /// <summary>
        /// Gets or sets the call option bool.
        /// </summary>
        /// <value>
        /// Indicates whether the premium array contains a value: Index1: ValueDate, Index2 Maturity, Index3 Strike
        /// </value>
        bool[,,] CallOptionBool { get; set; }
        double[,,] CallOptionPremium { get; set; }
        double[,,] CallOptionImpliedVol { get; set; }

        bool[,,] PutOptionBool { get; set; }
        double[,,] PutOptionPremium { get; set; }
        double[,,] PutOptionImpliedVol { get; set; }

        double[] StockPrice { get; set; }
        double[,] RateDiscountFactor { get; set; }
        double[,] DividendDiscountFactor { get; set; }

        DateTime GetIndexValuationDate(int valuationDateIndex);
        int GetValuationDateIndex(DateTime valuationDate);
    }
}