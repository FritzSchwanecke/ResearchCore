using System;
using System.Collections.Generic;
using IDataAccessLayer.ProcessData;
using IReserachCore.Instruments.Options;

namespace DataManagement.ProcessData
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IDataAccessLayer.ProcessData.IOptionDictionary" />
    public class OptionDictionary : IOptionDictionary
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
        public Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>> Calls { get; set; }

        /// <summary>
        ///     Gets or sets the puts.
        /// </summary>
        /// <value>
        ///     The puts.
        /// </value>
        public Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>> Puts { get; set; }

        /// <summary>
        ///     The number of calls
        /// </summary>
        public int NumberOfCalls { get; set; }

        /// <summary>
        ///     The number of puts
        /// </summary>
        public int NumberOfPuts { get; set; }


        /// <inheritdoc />
        /// <summary>
        ///     Gets or sets the premium difference for put call parity.
        /// </summary>
        /// <value>
        ///     The premium difference for put call parity.
        /// </value>
        public List<ISingleAssetOption> PremiumDifferenceForPutCallParity { get; set; }
    }
}