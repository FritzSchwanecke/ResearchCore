using System;
using System.Collections.Generic;
using IReserachCore.Instruments.Options;

namespace IDataAccessLayer.ProcessData
{
    public interface IOptionDictionary
    {
        /// <summary>
        ///     The underlying name
        /// </summary>
        string UnderlyingName { get; set; }

        /// <summary>
        ///     Gets or sets the calls.
        /// </summary>
        /// <value>
        ///     The calls.
        /// </value>
        Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>> Calls { get; set; }

        /// <summary>
        ///     Gets or sets the puts.
        /// </summary>
        /// <value>
        ///     The puts.
        /// </value>
        Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>> Puts { get; set; }

        /// <summary>
        ///     The number of calls
        /// </summary>
        int NumberOfCalls { get; set; }

        /// <summary>
        ///     The number of puts
        /// </summary>
        int NumberOfPuts { get; set; }

        /// <summary>
        /// Gets or sets the premium difference for put call parity.
        /// </summary>
        /// <value>
        /// The premium difference for put call parity.
        /// </value>
        List<ISingleAssetOption> PremiumDifferenceForPutCallParity { get; set; }
    }
}