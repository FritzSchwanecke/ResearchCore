using System.Collections;
using System.Collections.Generic;
using IReserachCore.Instruments.Options;

namespace IDataAccessLayer.LoadData
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOptionDataLoader
    {
        /// <summary>
        ///     Loads the option data.
        /// </summary>
        /// <param name="inputLine">The input line.</param>
        /// <param name="columnMapper">The column mapper.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns></returns>
        ISingleAssetOption LoadOptionDataFromCsvFile(string inputLine,
            Dictionary<string, int> columnMapper = null,
            char seperator = ',');

        ISingleAssetOption LoadOptionDataFromCsvFileFast(string inputLine);

        void LoadOptionDataFromCsvFileFast(string inputLine, ref IContainerSingleAssetOption singleAssetOptionContainer,
            IInterestRateTable interestRateTable);

    }
}