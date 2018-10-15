using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IDataAccessLayer.StoreData
{
    public interface IStoreDataToHardDisc
    {
        /// <summary>
        ///     Exports the file to zip.
        /// </summary>
        /// <param name="zipFileInfo">The zip file information.</param>
        /// <param name="lines">The lines.</param>
        /// <param name="outputFilename">The output filename.</param>
        void ExportFileToZip(FileInfo zipFileInfo, string[] lines, string outputFilename);

        void ExportStringToCsv(FileInfo csvFileInfo, StringBuilder csv);

        void ExportStringToCsv(FileInfo csvFileInfo, IList<string> csv);
    }
}