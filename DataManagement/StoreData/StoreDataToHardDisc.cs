using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using IDataAccessLayer.StoreData;

namespace DataManagement.StoreData
{
    /// <summary>
    /// </summary>
    public class StoreDataToHardDisc : IStoreDataToHardDisc
    {
        /// <summary>
        ///     Exports the file to zip.
        /// </summary>
        /// <param name="zipFileInfo">The zip file information.</param>
        /// <param name="lines">The lines.</param>
        /// <param name="outputFilename">The output filename.</param>
        public void ExportFileToZip(FileInfo zipFileInfo, string[] lines, string outputFilename)
        {
            if (!zipFileInfo.Exists)
            {
                zipFileInfo.Create().Close();
            }

            using (var zipToOpen = new FileStream(zipFileInfo.FullName, FileMode.Open))
            {
                using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    var fileEntry = archive.CreateEntry(outputFilename);
                    using (var writer = new StreamWriter(fileEntry.Open()))
                    {
                        foreach (var line in lines)
                            writer.WriteLine(line);
                    }
                }
            }
        }

        public void ExportStringToCsv(FileInfo csvFileInfo, StringBuilder csv)
        {
            File.WriteAllText(csvFileInfo.FullName, csv.ToString());
        }

        public void ExportStringToCsv(FileInfo csvFileInfo, IList<string> csv)
        {
                File.AppendAllLines(csvFileInfo.FullName, csv);
        }

    }
}