using System;
using System.Collections.Generic;
using IReserachCore.Instruments.Options;
using IReserachCore.Pricer.Dividends;

namespace ResearchCore.Pricer.Dividends
{
    public class ImpliedDividends : IImpliedDividends
    {
        public void Calculate(
            Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>> callDictionary,
            Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>> putDictionary)
        {
            var maxRange = 0.05;
            var strikeCounter = 0;
            double dividendCash = 0;

            IList<ISingleAssetOption> noMatchOptions = new List<ISingleAssetOption>();

            foreach (var callValuationDate in callDictionary.Keys)
            foreach (var callMaturityDate in callDictionary[callValuationDate].Keys)
            foreach (var callStrike in callDictionary[callValuationDate][callMaturityDate].Keys)
                if (putDictionary.ContainsKey(callValuationDate))
                {
                    if (putDictionary[callValuationDate].ContainsKey(callMaturityDate))
                    {
                        if (putDictionary[callValuationDate][callMaturityDate].ContainsKey(callStrike))
                        {
                            var callOptionPrice =
                                callDictionary[callValuationDate][callMaturityDate][callStrike].Premium;
                            var putOptionPrice = putDictionary[callValuationDate][callMaturityDate][callStrike].Premium;
                            var discountRate = callDictionary[callValuationDate][callMaturityDate][callStrike]
                                .DiscountFactor;

                            var strike = callStrike;
                            var stockPrice = callDictionary[callValuationDate][callMaturityDate][callStrike].StockPrice;

                            var dividendDiscounter =
                                (callOptionPrice - putOptionPrice + strike * discountRate) / stockPrice;

                            if (Math.Abs(strike - stockPrice) / Math.Max(strike, stockPrice) < maxRange)
                            {
                                strikeCounter++;
                                dividendCash += dividendDiscounter;
                            }

                            callDictionary[callValuationDate][callMaturityDate][callStrike]
                                .DividendDiscountFactor = dividendDiscounter;

                            putDictionary[callValuationDate][callMaturityDate][callStrike]
                                .DividendDiscountFactor = dividendDiscounter;
                        }
                        else
                        {
                            noMatchOptions.Add(callDictionary[callValuationDate][callMaturityDate][callStrike]);
                        }
                    }
                    else
                    {
                        noMatchOptions.Add(callDictionary[callValuationDate][callMaturityDate][callStrike]);
                    }
                }
                else
                {
                    noMatchOptions.Add(callDictionary[callValuationDate][callMaturityDate][callStrike]);
                }

            foreach (var option in noMatchOptions)
                if (strikeCounter > 0)
                    option.DividendDiscountFactor = dividendCash / strikeCounter;
        }

        public void Calculate(ref IContainerSingleAssetOption options)
        {
            
            var valueDateCount = options.StockPrice.Length;

            for (var valueDateIndex = 0; valueDateIndex < valueDateCount; valueDateIndex++)
            for (var maturityDateIndex = 0; maturityDateIndex < options.MaxMaturityIndex + 1; maturityDateIndex++)
            {
                var strikeCounter = 0;
                var premiumDifference = double.PositiveInfinity;
                var dividendDiscounter = 1.0;

                for (var strikeIndex = 0; strikeIndex < options.MaxStrikeIndex + 1; strikeIndex++)
                {
                    if (options.CallOptionBool[valueDateIndex, maturityDateIndex, strikeIndex] &&
                        options.PutOptionBool[valueDateIndex, maturityDateIndex, strikeIndex])
                    {
                        var callOptionPrice =
                            options.CallOptionPremium[valueDateIndex, maturityDateIndex, strikeIndex];
                        var putOptionPrice =
                            options.PutOptionPremium[valueDateIndex, maturityDateIndex, strikeIndex];

                        var discountRate = options.RateDiscountFactor[valueDateIndex, maturityDateIndex];

                        var strike = options.IndexToStrikeDictionary[strikeIndex];
                        var stockPrice = options.StockPrice[valueDateIndex];

                        if (callOptionPrice > 0d && putOptionPrice > 0d &&
                            Math.Abs(callOptionPrice - putOptionPrice) < premiumDifference)
                        {
                            dividendDiscounter =
                                (callOptionPrice - putOptionPrice + strike * discountRate) / stockPrice;
                            strikeCounter++;
                            premiumDifference = Math.Abs(callOptionPrice - putOptionPrice);
                        }
                    }

                    if (strikeCounter > 0)
                        options.DividendDiscountFactor[valueDateIndex, maturityDateIndex] = dividendDiscounter;
                }
            }
        }
    }
}