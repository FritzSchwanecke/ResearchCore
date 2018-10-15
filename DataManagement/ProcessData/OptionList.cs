using System.Collections.Generic;
using IDataAccessLayer.ProcessData;
using IReserachCore.Instruments.Options;

namespace DataManagement.ProcessData
{
    /// <summary>
    /// 
    /// </summary>
    public class OptionList : IOptionList
    {
        /// <inheritdoc />
        /// <summary>
        ///     The underlying name
        /// </summary>
        public string UnderlyingName { get; set; }

        // Valuation Date / Maturity / Strike
        /// <summary>
        ///     Gets or sets the calls.
        /// </summary>
        /// <value>
        ///     The calls.
        /// </value>
        public List<ISingleAssetOption> Calls { get; set; }

        /// <summary>
        ///     Gets or sets the puts.
        /// </summary>
        /// <value>
        ///     The puts.
        /// </value>
        public List<ISingleAssetOption> Puts { get; set; }

        /// <summary>
        ///     The number of calls
        /// </summary>
        public int NumberOfCalls { get; set; }

        /// <summary>
        ///     The number of puts
        /// </summary>
        public int NumberOfPuts { get; set; }
    }
}