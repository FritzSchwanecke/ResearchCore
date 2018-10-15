using System;
using System.Collections.Generic;
using IDataAccessLayer.LoadData;
using IDataAccessLayer.ProcessData;
using IReserachCore.Instruments.Options;
using JetBrains.Annotations;
using ResearchCore.Instruments.Options;

namespace DataManagement.ProcessData
{
    /// <summary>
    /// </summary>
    /// <seealso cref="IDataAccessLayer.ProcessData.IOptionDataProvider" />
    public class OptionDataProvider : IOptionDataProvider
    {
        public IOptionDictionary GenerateOptionDictionaries([NotNull] IEnumerable<ISingleAssetOption> optionEnumerable,
            string underlyingName, IInterestRateTable interestRateTable)
        {
            using (var enumerator = optionEnumerable.GetEnumerator())
            {
                var numberOfCalls = 0;
                var numberOfPuts = 0;

                if (!enumerator.MoveNext())
                {
                    //Trace.WriteLine("Enumerator for underlying {0} was empty.", underlyingName);
                }

                var callDictionary =
                    new Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>>();

                var putDictionary =
                    new Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>>();

                var jointPremiumDictionary = new List<ISingleAssetOption>();

                while (enumerator.MoveNext())
                {
                    var option = enumerator.Current;

                    if (option.ValuationDate == new DateTime(1900, 1, 1) ||
                        !option.Underlying.Equals(underlyingName)) continue;

                    option.DiscountFactor = interestRateTable.GetDiscountFactor(option.ValuationDate, option.Maturity);


                    if (option.OptionType == eOptionType.C)
                    {
                        numberOfCalls++;

                        if (!callDictionary.ContainsKey(option.ValuationDate))
                        {
                            callDictionary.Add(option.ValuationDate,
                                new Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>
                                {
                                    {
                                        option.Maturity, new Dictionary<double, ISingleAssetOption>
                                        {
                                            {
                                                option.Strike, option
                                            }
                                        }
                                    }
                                });
                        }
                        else
                        {
                            if (callDictionary[option.ValuationDate].ContainsKey(option.Maturity))
                            {
                                if (callDictionary[option.ValuationDate][option.Maturity]
                                    .ContainsKey(option.Strike))
                                    callDictionary[option.ValuationDate][option.Maturity][option.Strike]
                                        = option;
                                else
                                    callDictionary[option.ValuationDate][option.Maturity]
                                        .Add(option.Strike, option);
                            }
                            else
                            {
                                callDictionary[option.ValuationDate].Add(option.Maturity,
                                    new Dictionary<double, ISingleAssetOption>
                                    {
                                        {option.Strike, option}
                                    });
                            }
                        }

                        AddJointOption(putDictionary, option, ref jointPremiumDictionary);
                    }
                    else
                    {
                        numberOfPuts++;
                        if (!putDictionary.ContainsKey(option.ValuationDate))
                        {
                            putDictionary.Add(option.ValuationDate,
                                new Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>
                                {
                                    {
                                        option.Maturity, new Dictionary<double, ISingleAssetOption>
                                        {
                                            {option.Strike, option}
                                        }
                                    }
                                });
                        }
                        else
                        {
                            if (putDictionary[option.ValuationDate].ContainsKey(option.Maturity))
                            {
                                if (putDictionary[option.ValuationDate][option.Maturity]
                                    .ContainsKey(option.Strike))
                                    putDictionary[option.ValuationDate][option.Maturity][option.Strike]
                                        = option;
                                else
                                    putDictionary[option.ValuationDate][option.Maturity]
                                        .Add(option.Strike, option);
                            }
                            else
                            {
                                putDictionary[option.ValuationDate].Add(option.Maturity,
                                    new Dictionary<double, ISingleAssetOption>
                                    {
                                        {option.Strike, option}
                                    });
                            }
                        }

                        AddJointOption(callDictionary, option, ref jointPremiumDictionary);
                    }
                }

                return new OptionDictionary
                {
                    Calls = callDictionary,
                    NumberOfCalls = numberOfCalls,
                    NumberOfPuts = numberOfPuts,
                    PremiumDifferenceForPutCallParity = jointPremiumDictionary,
                    Puts = putDictionary,
                    UnderlyingName = underlyingName
                };
            }
        }


        public IOptionList GenerateOptionList([NotNull] IEnumerable<ISingleAssetOption> optionEnumerable,
            string underlyingName, IInterestRateTable interestRateTable)
        {
            using (var enumerator = optionEnumerable.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    //Trace.WriteLine("Enumerator for underlying {0} was empty.", underlyingName);
                }

                var callList = new List<ISingleAssetOption>();

                var putList = new List<ISingleAssetOption>();

                var jointPremiumDictionary = new List<ISingleAssetOption>();

                while (enumerator.MoveNext())
                {
                    var option = enumerator.Current;

                    if (option.ValuationDate == new DateTime(1900, 1, 1) ||
                        !option.Underlying.Equals(underlyingName)) continue;

                    option.DiscountFactor = interestRateTable.GetDiscountFactor(option.ValuationDate, option.Maturity);


                    if (option.OptionType == eOptionType.C)
                        callList.Add(option);
                    else
                        putList.Add(option);
                }

                return new OptionList
                {
                    UnderlyingName = underlyingName,
                    Calls = callList,
                    Puts = putList
                };
            }
        }

        private void AddJointOption(
            Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>> referenceDictionary,
            ISingleAssetOption referenceOption, ref List<ISingleAssetOption> listOfJointOption)
        {
            if (!referenceDictionary.ContainsKey(referenceOption.ValuationDate) ||
                !referenceDictionary[referenceOption.ValuationDate].ContainsKey(referenceOption.Maturity) ||
                !referenceDictionary[referenceOption.ValuationDate][referenceOption.Maturity]
                    .ContainsKey(referenceOption.Strike)) return;

            SingleAssetOption optionDiffPremium;
            if (referenceOption.OptionType == eOptionType.C)
                optionDiffPremium = new SingleAssetOption
                {
                    StockPrice = referenceOption.StockPrice,
                    Currency = referenceOption.Currency,
                    DiscountFactor = referenceOption.DiscountFactor,
                    DividendDiscountFactor = referenceOption.DividendDiscountFactor,
                    Maturity = referenceOption.Maturity,
                    Premium = referenceOption.Premium -
                              referenceDictionary[referenceOption.ValuationDate][referenceOption.Maturity][
                                  referenceOption.Strike].Premium,
                    Strike = referenceOption.Strike
                };
            else
                optionDiffPremium = new SingleAssetOption
                {
                    StockPrice = referenceOption.StockPrice,
                    Currency = referenceOption.Currency,
                    DiscountFactor = referenceOption.DiscountFactor,
                    DividendDiscountFactor = referenceOption.DividendDiscountFactor,
                    Maturity = referenceOption.Maturity,
                    Premium = referenceDictionary[referenceOption.ValuationDate][referenceOption.Maturity][
                                  referenceOption.Strike].Premium - referenceOption.Premium,
                    Strike = referenceOption.Strike
                };

            listOfJointOption.Add(optionDiffPremium);
        }

        public IContainerSingleAssetOption CreateSingleAssetOptionContainer(DateTime startDate, DateTime endDate,
            int numberOfStrikes, int numberOfMaturities)
        {
            var numberOfValDates = (int) (endDate - startDate).TotalDays + 1;

            var container = new ContainerSingleAssetOption
            {
                StartValueDate = startDate,
                EndValueDate = endDate,

                MaxStrikeIndex = -1,
                MaxMaturityIndex = -1,

                StockPrice = new double[numberOfValDates],
                DividendDiscountFactor = new double[numberOfValDates, numberOfMaturities],
                RateDiscountFactor = new double[numberOfValDates, numberOfMaturities],

                IndexToMaturityDictionary = new Dictionary<int, DateTime>(),
                IndexToStrikeDictionary = new Dictionary<int, double>(),
                MaturityToIndexDictionary = new Dictionary<DateTime, int>(),
                StrikeToIndexDictionary = new Dictionary<double, int>(),

                CallOptionBool = new bool[numberOfValDates, numberOfMaturities, numberOfStrikes],
                CallOptionPremium = new double[numberOfValDates, numberOfMaturities, numberOfStrikes],
                CallOptionImpliedVol = new double[numberOfValDates, numberOfMaturities, numberOfStrikes],

                PutOptionBool = new bool[numberOfValDates, numberOfMaturities, numberOfStrikes],
                PutOptionPremium = new double[numberOfValDates, numberOfMaturities, numberOfStrikes],
                PutOptionImpliedVol = new double[numberOfValDates, numberOfMaturities, numberOfStrikes],
            };


            return container;
        }
    }
}