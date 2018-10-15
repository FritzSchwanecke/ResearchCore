using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using IDataAccessLayer.LoadData;
using IReserachCore.Instruments.Options;
using LumenWorks.Framework.IO.Csv;
using ResearchCore.Instruments.Options;
using Unity.Attributes;

namespace DataManagement.LoadData
{
    public class LoadDataFromCsv : ILoadDataFromCsv
    {
        private const int MaxBuffer = 33554432 * 10; //32MB

        private readonly char[] _charBuffer = new char[1];
        private readonly IOptionDataLoader _optionDataLoader;
        private byte[] _buffer;
        private StringBuilder _currentLine;
        private string _inputDirectory;
        private string _inputFile;

        [InjectionConstructor]
        public LoadDataFromCsv(IOptionDataLoader optionDataLoader)
        {
            _optionDataLoader = optionDataLoader;
        }

        public void Read(string inputDirectory, string inputFile, int skipRows = 1)
        {
            _buffer = new byte[MaxBuffer];
            _currentLine = new StringBuilder();

            _inputDirectory = inputDirectory;
            _inputFile = inputFile;
            var currentLine = 0;

            var input = new FileInfo(_inputDirectory + "/" + _inputFile);

            using (var fs = File.Open(input.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var bs = new BufferedStream(fs))
                {
                    var memoryStream = new MemoryStream(_buffer);
                    var stream = new StreamReader(memoryStream);
                    while (bs.Read(_buffer, 0, MaxBuffer) != 0)
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        while (!stream.EndOfStream)
                        {
                            var line = ReadLineWithAccumulation(stream, _currentLine);

                            if (skipRows > currentLine)
                            {
                                currentLine++;
                                continue;
                            }

                            if (line != null)
                            {
                                var output = _optionDataLoader.LoadOptionDataFromCsvFile(line);
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<ISingleAssetOption> Read(Stream streamFile, Dictionary<string, int> mapColumns = null,
            int skipRows = 1)
        {
            _buffer = new byte[MaxBuffer];
            _currentLine = new StringBuilder();

            var currentLine = 0;

            using (var fs = streamFile)
            {
                using (var bs = new BufferedStream(fs))
                {
                    var memoryStream = new MemoryStream(_buffer);
                    var stream = new StreamReader(memoryStream);
                    while (bs.Read(_buffer, 0, MaxBuffer) != 0)
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        while (!stream.EndOfStream)
                        {
                            var line = ReadLineWithAccumulation(stream, _currentLine);

                            if (skipRows > currentLine)
                            {
                                currentLine++;
                                continue;
                            }

                            if (line != null)
                                yield return _optionDataLoader.LoadOptionDataFromCsvFile(line, mapColumns);
                        }
                    }
                }
            }
        }

        public string[] ReadIndexConstituents(string inputDirectory, string inputFile)
        {
            var constituents = new string[500];

            var input = new FileInfo(inputDirectory + "/" + inputFile);

            var counter = 0;

            using (var sr = new StreamReader(input.FullName))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Replace(",", "");

                    constituents[counter] = line;

                    counter++;
                }
            }


            return constituents;
        }

        public void ReadAllLinesToContainer(Stream streamFile,
            ref IContainerSingleAssetOption singleAssetOptionContainer, IInterestRateTable interestRateTable,
            int skipRows = 1)
        {
            _buffer = new byte[MaxBuffer];
            _currentLine = new StringBuilder();

            var currentLine = 0;

            using (var fs = streamFile)
            {
                using (var bs = new BufferedStream(fs))
                {
                    var maturityCounter = 0;
                    var strikeCounter = 0;

                    var memoryStream = new MemoryStream(_buffer);
                    var stream = new StreamReader(memoryStream);
                    while (bs.Read(_buffer, 0, MaxBuffer) != 0)
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);


                        while (!stream.EndOfStream)
                        {
                            var line = ReadLineWithAccumulation(stream, _currentLine);

                            if (skipRows > currentLine)
                            {
                                currentLine++;
                                continue;
                            }

                            if (maturityCounter >= 19 && strikeCounter >= 89) break;

                            if (line != null)
                                _optionDataLoader.LoadOptionDataFromCsvFileFast(line,
                                    ref singleAssetOptionContainer, interestRateTable);
                        }
                    }
                }
            }
        }

        public IInterestRateTable ReadInterestRateTable(string inputDirectory, string inputFile)
        {
            var valuationDates = new DateTime[4562];
            var rowCounter = 0;
            var tenors = new int[16];
            const int colCounterAdjustment = 1;

            var input = new FileInfo(inputDirectory + "/" + inputFile);

            var interestRateTable = new InterestRateTable(valuationDates, tenors);

            using (var sr = new StreamReader(input.FullName))
            {
                var line = sr.ReadLine()?.Split(';');

                for (var i = 0; i < 16; i++) interestRateTable.Tenor[i] = int.Parse(line[colCounterAdjustment + i]);


                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine()?.Split(';');

                    interestRateTable.ValuationDates[rowCounter] = DateTime.Parse(line[0]);


                    for (var i = 0; i < 16; i++)
                        interestRateTable.InterestRates[rowCounter, i] = double.Parse(line[colCounterAdjustment + i],
                            new CultureInfo("US-us"));

                    rowCounter++;
                }
            }


            return interestRateTable;
        }

        private string ReadLineWithAccumulation(StreamReader stream, StringBuilder currentLine)
        {
            while (stream.Read(_charBuffer, 0, 1) > 0)
                if (_charBuffer[0].Equals('\n'))
                {
                    var result = currentLine.ToString();
                    currentLine.Clear();

                    if (result.Last() == '\r') //remove if newlines are single character
                        result = result.Substring(0, result.Length - 1);

                    return result;
                }
                else
                {
                    currentLine.Append(_charBuffer[0]);
                }

            return null; //line not complete yet
        }

        public IEnumerable<ISingleAssetOption> ReadAllLines(string inputFile, Dictionary<string, int> mapColumns = null,
            int skipRows = 1)
        {
            return File.ReadLines(inputFile).Skip(skipRows)
                .Select(line => _optionDataLoader.LoadOptionDataFromCsvFileFast(line));
        }


        public void ReadAllLinesToContainerStreamReader(string fileName,
            ref IContainerSingleAssetOption singleAssetOptionContainer, IInterestRateTable interestRateTable,
            int skipRows = 1)
        {
            using (var r = new StreamReader(fileName))
            {
                var line = r.ReadLine();

                while (!r.EndOfStream)
                {
                    line = r.ReadLine();


                    this._optionDataLoader.LoadOptionDataFromCsvFileFast(line,
                        ref singleAssetOptionContainer, interestRateTable);
                }

                r.Dispose();
            }

            
        }


        public IEnumerable<ISingleAssetOption> ReadCsv(string filename)
        {
            // open the file "data.csv" which is a CSV file with headers
            using (var inputData =
                new CsvReader(new StreamReader(filename), true))
            {
                var fieldCount = inputData.FieldCount;

                string[] formats = {"MM/dd/yyyy"};

                ISingleAssetOption returnValue;

                var headers = inputData.GetFieldHeaders();
                while (inputData.ReadNextRecord())
                {
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
                        //Trace.WriteLine(inputData[1]);
                        returnValue = new SingleAssetOption
                        {
                            ValuationDate = new DateTime(1900, 1, 1),
                            Maturity = new DateTime(1901, 1, 1),
                            OptionType = eOptionType.C
                        };
                    }

                    yield return returnValue;
                }
            }
        }
    }
}