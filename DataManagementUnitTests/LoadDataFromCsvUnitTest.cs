using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManagement.LoadData;
using DataManagement.ProcessData;
using DataManagement.StoreData;
using IDataAccessLayer.LoadData;
using IDataAccessLayer.ProcessData;
using IDataAccessLayer.StoreData;
using IReserachCore.Instruments.Options;
using IReserachCore.Pricer.BlackScholes;
using IReserachCore.Pricer.Dividends;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResearchCore.Instruments.Options;
using ResearchCore.Pricer.BlackScholes;
using ResearchCore.Pricer.Dividends;
using Unity;

namespace DataManagement.UnitTests
{
    public class LoadDataFromCsvUnitTest
    {
        [TestClass]
        public class LoadData
        {
            private IUnityContainer IoCContainer;

            private void RegisterToUnity()
            {
                IoCContainer = new UnityContainer();
                IoCContainer.RegisterType<ILoadDataFromCsv, LoadDataFromCsv>();
                IoCContainer.RegisterType<IInterestRateTable, InterestRateTable>();
                IoCContainer.RegisterType<IStoreDataToHardDisc, StoreDataToHardDisc>();

                IoCContainer.RegisterType<IOptionDataLoader, OptionDataLoader>();
                IoCContainer.RegisterType<ISingleAssetOption, SingleAssetOption>();
                IoCContainer.RegisterType<IOptionDataProvider, OptionDataProvider>();

                IoCContainer.RegisterType<IPeterJaeckleAnalyticalPricing, PeterJaeckleAnalyticalPricing>();
                IoCContainer
                    .RegisterType<IPeterJaeckleAnalyticalInversePricing, PeterJaeckleAnalyticalInversePricing>();
                IoCContainer.RegisterType<IImpliedDividends, ImpliedDividends>();
                IoCContainer.RegisterType<IRiskFigures, RiskFigures>();
            }

            [TestMethod]
            public void LoadDataFromCsvTest()
            {
                // Arrange
                RegisterToUnity();
                var dataLoader = IoCContainer.Resolve<LoadDataFromCsv>();

                // Act
                dataLoader.Read(@"C: \Users\Fritz\CsProjects\Data", "AAAP.csv");

                // Assert
            }

            [TestMethod]
            public void LoadDataFromZipArchive()
            {
                // Arrange
                RegisterToUnity();
                var dataLoader = IoCContainer.Resolve<LoadDataFromCsv>();
                var dataStorer = IoCContainer.Resolve<IStoreDataToHardDisc>();

                var impliedVolCalculator = IoCContainer.Resolve<IPeterJaeckleAnalyticalInversePricing>();
                var dividendCalculator = IoCContainer.Resolve<IImpliedDividends>();
                var optionDataProvider = IoCContainer.Resolve<IOptionDataProvider>();

                var riskFigureCalculator = IoCContainer.Resolve<IRiskFigures>();

                var columnMapper = new Dictionary<string, int>
                {
                    {"Underlying", 1},
                    {"ValuationDate", 4},
                    {"ValuationTime", 17},
                    {"MaturityDate", 7},
                    {"Strike", 8},
                    {"OptionType", 9},
                    {"PremiumBid", 12},
                    {"PremiumAsk", 11},
                    {"StockPrice", 16}
                };

                var interestRateTable =
                    dataLoader.ReadInterestRateTable("C:/Users/Fritz/CsProjects/Data", "expiry_int_rates_cleaned.csv");

                // Act
                IEnumerable<ISingleAssetOption> singleAssetOptions;
                //C: \Users\Fritz\CsProjects\Data\2013\close
                //using (var archive = ZipFile.OpenRead(@"C:\Users\Fritz\CsProjects\Data\2013.zip"))
                var files = new DirectoryInfo(@"C: \Users\Fritz\CsProjects\Data\2013\close\").EnumerateFileSystemInfos()
                    .Where(x => x.FullName.Contains(".csv"));
                {
                    Parallel.ForEach(files, s =>
                        //if (!ZipArchiveEntryExtensions.IsFolder(s))
                        //  if (s.Name.Contains(".csv") && !s.Name.Contains("symbol"))

                    {
                        //Trace.WriteLine(s.FullName);

                        singleAssetOptions = dataLoader.ReadCsv(s.FullName);

                        var underlyingName = Path.GetFileNameWithoutExtension(s.Name);

                        var optionDictionary = optionDataProvider.GenerateOptionDictionaries(singleAssetOptions,
                            underlyingName, interestRateTable);

                        var callDictionary = optionDictionary.Calls;
                        var putDictionary = optionDictionary.Puts;

                        dividendCalculator.Calculate(callDictionary, putDictionary);

                        foreach (var valuationDate in putDictionary.Keys)
                        foreach (var maturityDate in putDictionary[valuationDate].Keys)
                        foreach (var strike in putDictionary[valuationDate][maturityDate].Keys)
                        {
                            var option = putDictionary[valuationDate][maturityDate][strike];

                            putDictionary[valuationDate][maturityDate][strike].ImpliedVolatility =
                                impliedVolCalculator.Volatility(option.ValuationDate, option, option.StockPrice,
                                    option.DiscountFactor, option.DividendDiscountFactor);
                        }

                        foreach (var valuationDate in callDictionary.Keys)
                        foreach (var maturityDate in callDictionary[valuationDate].Keys)
                        foreach (var strike in callDictionary[valuationDate][maturityDate].Keys)
                        {
                            var option = callDictionary[valuationDate][maturityDate][strike];

                            callDictionary[valuationDate][maturityDate][strike].ImpliedVolatility =
                                impliedVolCalculator.Volatility(option.ValuationDate, option, option.StockPrice,
                                    option.DiscountFactor, option.DividendDiscountFactor);
                        }

                        //before your loop
                        var csv = new StringBuilder();

                        var header = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", "Underlying",
                            "DiscountFactor", "DividendDiscountFactor", "ImpliedVol", "Maturity", "ValDate", "Premium",
                            "OptionType",
                            "StockPrice", "Strike", "Delta");

                        csv.AppendLine(header);


                        foreach (var valuationDate in callDictionary.Keys)
                        foreach (var maturityDate in callDictionary[valuationDate].Keys)
                        foreach (var strike in callDictionary[valuationDate][maturityDate].Keys)
                        {
                            var outputOption = callDictionary[valuationDate][maturityDate][strike];
                            var line = string.Format(new CultureInfo("en-GB"),
                                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                outputOption.Underlying,
                                outputOption.DiscountFactor,
                                outputOption.DividendDiscountFactor,
                                outputOption.ImpliedVolatility,
                                outputOption.Maturity.Date.ToShortDateString(),
                                outputOption.ValuationDate.ToShortDateString(),
                                outputOption.Premium,
                                outputOption.OptionType,
                                outputOption.StockPrice,
                                outputOption.Strike,
                                riskFigureCalculator.DeltaCall(
                                    outputOption.StockPrice * outputOption.DividendDiscountFactor /
                                    outputOption.DiscountFactor, outputOption.Strike, outputOption.ImpliedVolatility,
                                    (outputOption.Maturity - outputOption.ValuationDate).TotalDays / 365.0,
                                    outputOption.DividendDiscountFactor));

                            csv.AppendLine(line);
                        }

                        foreach (var valuationDate in putDictionary.Keys)
                        foreach (var maturityDate in putDictionary[valuationDate].Keys)
                        foreach (var strike in putDictionary[valuationDate][maturityDate].Keys)
                        {
                            var outputOption = putDictionary[valuationDate][maturityDate][strike];
                            var line = string.Format(new CultureInfo("en-GB"),
                                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                outputOption.Underlying,
                                outputOption.DiscountFactor,
                                outputOption.DividendDiscountFactor,
                                outputOption.ImpliedVolatility,
                                outputOption.Maturity.Date.ToShortDateString(),
                                outputOption.ValuationDate.ToShortDateString(),
                                outputOption.Premium,
                                outputOption.OptionType,
                                outputOption.StockPrice,
                                outputOption.Strike,
                                riskFigureCalculator.DeltaPut(
                                    outputOption.StockPrice * outputOption.DividendDiscountFactor /
                                    outputOption.DiscountFactor, outputOption.Strike, outputOption.ImpliedVolatility,
                                    (outputOption.Maturity - outputOption.ValuationDate).TotalDays / 365.0,
                                    outputOption.DividendDiscountFactor));
                            csv.AppendLine(line);
                        }

                        var exportFile = new FileInfo("C:/Users/Fritz/CsProjects/ImpliedVolData/2013/" + s.Name);

                        dataStorer.ExportStringToCsv(exportFile, csv);
                    });
                }
            }


            [TestMethod]
            public void LoadDataToOptionContainer()
            {
                // Arrange
                RegisterToUnity();
                var dataLoader = IoCContainer.Resolve<LoadDataFromCsv>();
                var dataStorer = IoCContainer.Resolve<IStoreDataToHardDisc>();

                var impliedVolCalculator = IoCContainer.Resolve<IPeterJaeckleAnalyticalInversePricing>();
                var dividendCalculator = IoCContainer.Resolve<IImpliedDividends>();
                var optionDataProvider = IoCContainer.Resolve<IOptionDataProvider>();

                var riskFigureCalculator = IoCContainer.Resolve<IRiskFigures>();

                var interestRateTable =
                    dataLoader.ReadInterestRateTable("C:/Users/Fritz/CsProjects/Data", "expiry_int_rates_cleaned.csv");
                var constituents =
                    dataLoader.ReadIndexConstituents("C:/Users/Fritz/CsProjects/Data", "SP500_constituents.csv");

                var startDate = new DateTime(2017, 1, 1);
                var endDate = new DateTime(2017, 12, 31);

                // Act

                var files = new DirectoryInfo(@"C: \Users\Fritz\CsProjects\Data\2017\close\").EnumerateFileSystemInfos()
                    .Where(x => x.FullName.Contains(".csv"))
                    .Where(x => constituents.Contains(Path.GetFileNameWithoutExtension(x.Name))).Skip(111).ToList();

                Parallel.ForEach(files, s =>
                {
                    var container = optionDataProvider.CreateSingleAssetOptionContainer(startDate,
                        endDate, 90, 15);

                    var maxValueDateCount = container.StockPrice.Length;

                    dataLoader.ReadAllLinesToContainerStreamReader(s.FullName, ref container, interestRateTable);

                    container.Underlying = Path.GetFileNameWithoutExtension(s.Name);

                    dividendCalculator.Calculate(ref container);

                    var csv = new List<string>();

                    var header = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", "Underlying",
                        "DiscountFactor", "DividendDiscountFactor", "ImpliedVol", "Maturity", "ValDate", "Premium",
                        "OptionType",
                        "StockPrice", "Strike", "Delta");

                    csv.Add(header);

                    var exportFile = new FileInfo("C:/Users/Fritz/CsProjects/ImpliedVolData/2017/" + s.Name);

                    for (var valuationDateIndex = 0; valuationDateIndex < maxValueDateCount; valuationDateIndex++)
                    {
                        for (var maturityDateIndex = 0;
                            maturityDateIndex < container.MaxMaturityIndex + 1;
                            maturityDateIndex++)
                        for (var strikeIndex = 0; strikeIndex < container.MaxStrikeIndex + 1; strikeIndex++)
                        {
                            var optionValuationDate = container.GetIndexValuationDate(valuationDateIndex);
                            var maturityDate = container.IndexToMaturityDictionary[maturityDateIndex];
                            var strike = container.IndexToStrikeDictionary[strikeIndex];

                            var underlyingPrice = container.StockPrice[valuationDateIndex];
                            var discountFactor =
                                container.RateDiscountFactor[valuationDateIndex, maturityDateIndex];
                            var dividendDiscountFactor =
                                container.DividendDiscountFactor[valuationDateIndex, maturityDateIndex];


                            if (container.CallOptionBool[valuationDateIndex, maturityDateIndex, strikeIndex])
                            {
                                var optionPrice = container.CallOptionPremium[valuationDateIndex, maturityDateIndex,
                                    strikeIndex];
                                container.CallOptionImpliedVol[valuationDateIndex, maturityDateIndex, strikeIndex] =
                                    impliedVolCalculator.Volatility(optionValuationDate, maturityDate, strike,
                                        optionPrice, underlyingPrice, eOptionType.C, discountFactor,
                                        dividendDiscountFactor);

                                var line = string.Format(new CultureInfo("en-GB"),
                                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                    container.Underlying,
                                    discountFactor,
                                    dividendDiscountFactor,
                                    container.CallOptionImpliedVol[valuationDateIndex, maturityDateIndex, strikeIndex],
                                    maturityDate.Date.ToShortDateString(),
                                    optionValuationDate.ToShortDateString(),
                                    optionPrice,
                                    eOptionType.C,
                                    underlyingPrice,
                                    strike,
                                    riskFigureCalculator.DeltaCall(
                                        underlyingPrice * dividendDiscountFactor /
                                        discountFactor, strike,
                                        container.CallOptionImpliedVol[valuationDateIndex, maturityDateIndex,
                                            strikeIndex],
                                        (maturityDate - optionValuationDate).TotalDays / 365.0,
                                        dividendDiscountFactor));
                                csv.Add(line);
                            }

                            if (container.PutOptionBool[valuationDateIndex, maturityDateIndex, strikeIndex])
                            {
                                var optionPrice = container.PutOptionPremium[valuationDateIndex, maturityDateIndex,
                                    strikeIndex];

                                container.PutOptionImpliedVol[valuationDateIndex, maturityDateIndex, strikeIndex] =
                                    impliedVolCalculator.Volatility(optionValuationDate, maturityDate, strike,
                                        optionPrice, underlyingPrice, eOptionType.P, discountFactor,
                                        dividendDiscountFactor);

                                var line = string.Format(new CultureInfo("en-GB"),
                                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                    container.Underlying,
                                    discountFactor,
                                    dividendDiscountFactor,
                                    container.PutOptionImpliedVol[valuationDateIndex, maturityDateIndex, strikeIndex],
                                    maturityDate.Date.ToShortDateString(),
                                    optionValuationDate.ToShortDateString(),
                                    optionPrice,
                                    eOptionType.P,
                                    underlyingPrice,
                                    strike,
                                    riskFigureCalculator.DeltaPut(
                                        underlyingPrice * dividendDiscountFactor /
                                        discountFactor, strike,
                                        container.PutOptionImpliedVol[valuationDateIndex, maturityDateIndex,
                                            strikeIndex],
                                        (maturityDate - optionValuationDate).TotalDays / 365.0,
                                        dividendDiscountFactor));
                                csv.Add(line);
                            }
                        }


                    }
                    dataStorer.ExportStringToCsv(exportFile, csv);
                    csv.Clear();

                });
            }

            [TestMethod]
            public void LoadDataToOptionContainerTEST()
            {
                // Arrange
                RegisterToUnity();
                var dataLoader = IoCContainer.Resolve<LoadDataFromCsv>();
                var dataStorer = IoCContainer.Resolve<IStoreDataToHardDisc>();

                var impliedVolCalculator = IoCContainer.Resolve<IPeterJaeckleAnalyticalInversePricing>();
                var dividendCalculator = IoCContainer.Resolve<IImpliedDividends>();
                var optionDataProvider = IoCContainer.Resolve<IOptionDataProvider>();

                var riskFigureCalculator = IoCContainer.Resolve<IRiskFigures>();

                var interestRateTable =
                    dataLoader.ReadInterestRateTable("C:/Users/Fritz/CsProjects/Data", "expiry_int_rates_cleaned.csv");
                var constituents =
                    dataLoader.ReadIndexConstituents("C:/Users/Fritz/CsProjects/Data", "SP500_constituents.csv");

                var startDate = new DateTime(2018, 1, 1);
                var endDate = new DateTime(2018, 12, 31);

                // Act

                var files = new DirectoryInfo(@"C: \Users\Fritz\CsProjects\Data\2018\close\").EnumerateFileSystemInfos()
                    .Where(x => x.FullName.Contains(".csv"))
                    .Where(x => constituents.Contains(Path.GetFileNameWithoutExtension(x.Name))).ToList();

                foreach (var s in files)
                {
                    helperFunction(s, dataLoader, optionDataProvider, startDate, endDate, interestRateTable, dividendCalculator, impliedVolCalculator, riskFigureCalculator, dataStorer);

                }
            }

            private void helperFunction(FileSystemInfo s, LoadDataFromCsv dataLoader, IOptionDataProvider optionDataProvider, DateTime startDate, DateTime endDate, IInterestRateTable interestRateTable,IImpliedDividends dividendCalculator, IPeterJaeckleAnalyticalInversePricing impliedVolCalculator, IRiskFigures riskFigureCalculator, IStoreDataToHardDisc dataStorer)
            {

                var container = optionDataProvider.CreateSingleAssetOptionContainer(startDate,
                        endDate, 90, 15);

                var maxValueDateCount = container.StockPrice.Length;

                dataLoader.ReadAllLinesToContainerStreamReader(s.FullName, ref container, interestRateTable);

                container.Underlying = Path.GetFileNameWithoutExtension(s.Name);

                dividendCalculator.Calculate(ref container);

                var csv = new List<string>();

                var header = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", "Underlying",
                    "DiscountFactor", "DividendDiscountFactor", "ImpliedVol", "Maturity", "ValDate", "Premium",
                    "OptionType",
                    "StockPrice", "Strike", "Delta");

                csv.Add(header);

                var exportFile = new FileInfo("C:/Users/Fritz/CsProjects/ImpliedVolData/2018/" + s.Name);

                for (var valuationDateIndex = 0; valuationDateIndex < maxValueDateCount; valuationDateIndex++)
                {
                    for (var maturityDateIndex = 0;
                        maturityDateIndex < container.MaxMaturityIndex + 1;
                        maturityDateIndex++)
                        for (var strikeIndex = 0; strikeIndex < container.MaxStrikeIndex + 1; strikeIndex++)
                        {
                            var optionValuationDate = container.GetIndexValuationDate(valuationDateIndex);
                            var maturityDate = container.IndexToMaturityDictionary[maturityDateIndex];
                            var strike = container.IndexToStrikeDictionary[strikeIndex];

                            var underlyingPrice = container.StockPrice[valuationDateIndex];
                            var discountFactor =
                                container.RateDiscountFactor[valuationDateIndex, maturityDateIndex];
                            var dividendDiscountFactor =
                                container.DividendDiscountFactor[valuationDateIndex, maturityDateIndex];


                            if (container.CallOptionBool[valuationDateIndex, maturityDateIndex, strikeIndex])
                            {
                                var optionPrice = container.CallOptionPremium[valuationDateIndex, maturityDateIndex,
                                    strikeIndex];
                                container.CallOptionImpliedVol[valuationDateIndex, maturityDateIndex, strikeIndex] =
                                    impliedVolCalculator.Volatility(optionValuationDate, maturityDate, strike,
                                        optionPrice, underlyingPrice, eOptionType.C, discountFactor,
                                        dividendDiscountFactor);

                                var line = string.Format(new CultureInfo("en-GB"),
                                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                    container.Underlying,
                                    discountFactor,
                                    dividendDiscountFactor,
                                    container.CallOptionImpliedVol[valuationDateIndex, maturityDateIndex, strikeIndex],
                                    maturityDate.Date.ToShortDateString(),
                                    optionValuationDate.ToShortDateString(),
                                    optionPrice,
                                    eOptionType.C,
                                    underlyingPrice,
                                    strike,
                                    riskFigureCalculator.DeltaCall(
                                        underlyingPrice * dividendDiscountFactor /
                                        discountFactor, strike,
                                        container.CallOptionImpliedVol[valuationDateIndex, maturityDateIndex,
                                            strikeIndex],
                                        (maturityDate - optionValuationDate).TotalDays / 365.0,
                                        dividendDiscountFactor));
                                csv.Add(line);
                            }

                            if (container.PutOptionBool[valuationDateIndex, maturityDateIndex, strikeIndex])
                            {
                                var optionPrice = container.PutOptionPremium[valuationDateIndex, maturityDateIndex,
                                    strikeIndex];

                                container.PutOptionImpliedVol[valuationDateIndex, maturityDateIndex, strikeIndex] =
                                    impliedVolCalculator.Volatility(optionValuationDate, maturityDate, strike,
                                        optionPrice, underlyingPrice, eOptionType.P, discountFactor,
                                        dividendDiscountFactor);

                                var line = string.Format(new CultureInfo("en-GB"),
                                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                    container.Underlying,
                                    discountFactor,
                                    dividendDiscountFactor,
                                    container.PutOptionImpliedVol[valuationDateIndex, maturityDateIndex, strikeIndex],
                                    maturityDate.Date.ToShortDateString(),
                                    optionValuationDate.ToShortDateString(),
                                    optionPrice,
                                    eOptionType.P,
                                    underlyingPrice,
                                    strike,
                                    riskFigureCalculator.DeltaPut(
                                        underlyingPrice * dividendDiscountFactor /
                                        discountFactor, strike,
                                        container.PutOptionImpliedVol[valuationDateIndex, maturityDateIndex,
                                            strikeIndex],
                                        (maturityDate - optionValuationDate).TotalDays / 365.0,
                                        dividendDiscountFactor));
                                csv.Add(line);
                            }
                        }


                }
                dataStorer.ExportStringToCsv(exportFile, csv);
                csv.Clear();

            }

            private static class ZipArchiveEntryExtensions
            {
                public static bool IsFolder(ZipArchiveEntry entry)
                {
                    return entry.FullName.EndsWith("/");
                }
            }
        }
    }
}