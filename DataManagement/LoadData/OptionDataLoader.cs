using System;
using System.Collections.Generic;
using System.Globalization;
using IDataAccessLayer.LoadData;
using IReserachCore.Instruments.Options;
using ResearchCore.Instruments.Options;

namespace DataManagement.LoadData
{
    /// <summary>
    /// </summary>
    public class OptionDataLoader : IOptionDataLoader
    {
        /// <summary>
        ///     Loads the option data.
        /// </summary>
        /// <param name="inputLine">The input line.</param>
        /// <param name="columnMapper">The column mapper.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns></returns>
        public ISingleAssetOption LoadOptionDataFromCsvFile(string inputLine,
            Dictionary<string, int> columnMapper = null,
            char seperator = ',')
        {
            if (columnMapper == null)
                columnMapper = new Dictionary<string, int>
                {
                    {"Underlying", 1},
                    {"ValuationDate", 14},
                    {"MaturityDate", 4},
                    {"Strike", 5},
                    {"OptionType", 6},
                    {"PremiumBid", 9},
                    {"PremiumAsk", 8},
                    {"StockPrice", 2}
                };

            var inputData = inputLine.Split(seperator);

            string[] formats = {"MM/dd/yyyy"};

            var inputSingleAssetOption = new SingleAssetOption();
            inputSingleAssetOption.ValuationDate = new DateTime(1900, 1, 1);
            inputSingleAssetOption.Maturity = new DateTime(1901, 1, 1);
            inputSingleAssetOption.OptionType = eOptionType.C;


            try
            {
                inputSingleAssetOption = new SingleAssetOption
                {
                    Underlying = inputData[columnMapper["Underlying"]],
                    Maturity = DateTime.ParseExact(inputData[columnMapper["MaturityDate"]], formats,
                        new CultureInfo("US-us"), DateTimeStyles.AdjustToUniversal),
                    Strike = double.Parse(inputData[columnMapper["Strike"]], new CultureInfo("US-us")),
                    //Currency =
                    //    (eCurrency) Enum.Parse(typeof(eCurrency), inputLine[columnMapper["Currency"]].ToString()),
                    OptionType = (eOptionType) Enum.Parse(typeof(eOptionType),
                        inputData[columnMapper["OptionType"]]),
                    Nominal = 10.0 * 100.0,
                    StockPrice = double.Parse(inputData[columnMapper["StockPrice"]], new CultureInfo("US-us"))
                };

                if (columnMapper.ContainsKey("ImpliedVolatility"))
                    inputSingleAssetOption.ImpliedVolatility =
                        double.Parse(inputData[columnMapper["ImpliedVolatility"]]);

                if (columnMapper.ContainsKey("PremiumAsk") && columnMapper.ContainsKey("PremiumBid"))
                    inputSingleAssetOption.Premium =
                        (double.Parse(inputData[columnMapper["PremiumAsk"]], new CultureInfo("US-us")) +
                         double.Parse(inputData[columnMapper["PremiumAsk"]], new CultureInfo("US-us"))) / 2.0;

                if (columnMapper.ContainsKey("ValuationTime"))
                    inputSingleAssetOption.ValuationDate =
                        DateTime.Parse(inputData[columnMapper["ValuationDate"]] + " " +
                                       inputData[columnMapper["ValuationTime"]]);
                inputSingleAssetOption.ValuationDate =
                    DateTime.Parse(inputData[columnMapper["ValuationDate"]]);
            }
            catch (Exception)
            {
                //Trace.WriteLine(inputLine);
            }

            return inputSingleAssetOption;
        }


        public ISingleAssetOption LoadOptionDataFromCsvFileFast(string inputLine)
        {
            var inputData = inputLine.Split(',');

            string[] formats = {"MM/dd/yyyy"};

            ISingleAssetOption returnValue;

            try
            {
                returnValue = new SingleAssetOption
                {
                    Underlying = inputData[1],
                    Maturity = DateTime.ParseExact(inputData[7], formats,
                        new CultureInfo("US-us"), DateTimeStyles.AdjustToUniversal),
                    Strike = double.Parse(inputData[8], new CultureInfo("US-us")),
                    OptionType = (eOptionType) Enum.Parse(typeof(eOptionType),
                        inputData[9]),
                    Nominal = 10.0 * 100.0,
                    StockPrice = double.Parse(inputData[16], new CultureInfo("US-us")),
                    Premium = (double.Parse(inputData[11], new CultureInfo("US-us")) +
                               double.Parse(inputData[12], new CultureInfo("US-us"))) / 2.0,
                    ValuationDate = DateTime.ParseExact(inputData[4], "yyyy-MM-dd", new CultureInfo("US-us"),
                        DateTimeStyles.AdjustToUniversal)
                };
            }


            catch (Exception)
            {
                //Trace.WriteLine(inputLine);
                returnValue = new SingleAssetOption
                {
                    ValuationDate = new DateTime(1900, 1, 1),
                    Maturity = new DateTime(1901, 1, 1),
                    OptionType = eOptionType.C
                };
            }

            return returnValue;
        }

        public void LoadOptionDataFromCsvFileFast(string inputLine,
            ref IContainerSingleAssetOption singleAssetOptionContainer, IInterestRateTable interestRateTable)
        {
            var skipOption = false;

            try
            {
                var inputData = inputLine.Split(',');
                string[] formats = {"MM/dd/yyyy"};

                var maturity = DateTime.ParseExact(inputData[7], formats, new CultureInfo("US-us"),
                    DateTimeStyles.AdjustToUniversal);
                var strike = double.Parse(inputData[8], new CultureInfo("US-us"));
                var optionType = (eOptionType) Enum.Parse(typeof(eOptionType), inputData[9]);
                var stockPrice = double.Parse(inputData[16], new CultureInfo("US-us"));
                var premium = (double.Parse(inputData[11], new CultureInfo("US-us")) +
                               double.Parse(inputData[12], new CultureInfo("US-us"))) / 2.0;
                var valuationDate = DateTime.ParseExact(inputData[4], "yyyy-MM-dd", new CultureInfo("US-us"),
                    DateTimeStyles.AdjustToUniversal);

                var valDateIndex = singleAssetOptionContainer.GetValuationDateIndex(valuationDate);
                var strikeIndex = 0;

                if (singleAssetOptionContainer.StrikeToIndexDictionary.ContainsKey(strike))
                {
                    strikeIndex = singleAssetOptionContainer.StrikeToIndexDictionary[strike];
                }
                else
                {
                    if (singleAssetOptionContainer.MaxStrikeIndex >= 89)
                    {
                        skipOption = true;
                    }
                    else
                    {
                        singleAssetOptionContainer.MaxStrikeIndex++;
                        strikeIndex = singleAssetOptionContainer.MaxStrikeIndex;
                        singleAssetOptionContainer.StrikeToIndexDictionary.Add(strike, strikeIndex);
                        singleAssetOptionContainer.IndexToStrikeDictionary.Add(strikeIndex, strike);
                    }
                }

                var maturityIndex = 0;
                if (!skipOption)
                {
                    if (singleAssetOptionContainer.MaturityToIndexDictionary.ContainsKey(maturity))
                    {
                        maturityIndex = singleAssetOptionContainer.MaturityToIndexDictionary[maturity];
                    }
                    else
                    {
                        if (singleAssetOptionContainer.MaxMaturityIndex >= 14)

                        {
                            skipOption = true;
                        }
                        else
                        {
                            singleAssetOptionContainer.MaxMaturityIndex++;
                            maturityIndex = singleAssetOptionContainer.MaxMaturityIndex;
                            singleAssetOptionContainer.MaturityToIndexDictionary.Add(maturity, maturityIndex);
                            singleAssetOptionContainer.IndexToMaturityDictionary.Add(maturityIndex, maturity);
                        }
                    }
                }
                


                if (!skipOption)
                {
                    singleAssetOptionContainer.StockPrice[valDateIndex] = stockPrice;
                    singleAssetOptionContainer.RateDiscountFactor[valDateIndex, maturityIndex] =
                        interestRateTable.GetDiscountFactor(valuationDate, maturity);

                    switch (optionType)
                    {
                        case eOptionType.C:
                            singleAssetOptionContainer.CallOptionBool[valDateIndex, maturityIndex, strikeIndex] = true;
                            singleAssetOptionContainer.CallOptionPremium[valDateIndex, maturityIndex, strikeIndex] =
                                premium;
                            break;
                        case eOptionType.P:
                            singleAssetOptionContainer.PutOptionBool[valDateIndex, maturityIndex, strikeIndex] = true;
                            singleAssetOptionContainer.PutOptionPremium[valDateIndex, maturityIndex, strikeIndex] =
                                premium;
                            break;
                    }
                }
            }

            catch (Exception)
            {
                
            }
        }
    }
}