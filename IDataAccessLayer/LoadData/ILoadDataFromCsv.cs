using System.Collections.Generic;
using System.IO;
using IReserachCore.Instruments.Options;

namespace IDataAccessLayer.LoadData
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILoadDataFromCsv
    {
        /// <summary>
        ///     Reads the specified input directory.
        /// </summary>
        /// <param name="inputDirectory">The input directory.</param>
        /// <param name="inputFile">The input file.</param>
        /// <param name="skipRows">The skip rows.</param>
        void Read(string inputDirectory, string inputFile, int skipRows = 1);

        /// <summary>
        ///     Reads the specified file stream.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="mapColumns"></param>
        /// <param name="skipRows">The skip rows.</param>
        IEnumerable<ISingleAssetOption> Read(Stream fileStream, Dictionary<string, int> mapColumns = null, int skipRows = 1);

        void ReadAllLinesToContainer(Stream streamFile, ref IContainerSingleAssetOption singleAssetOptionContainer,
            IInterestRateTable interestRateTable, int skipRows = 1);

        string[] ReadIndexConstituents(string inputDirectory, string inputFile);

    }
}