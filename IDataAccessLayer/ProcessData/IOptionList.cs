using System.Collections.Generic;
using IReserachCore.Instruments.Options;

namespace IDataAccessLayer.ProcessData
{
    public interface IOptionList
    {
        /// <inheritdoc />
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
        List<ISingleAssetOption> Calls { get; set; }

        /// <summary>
        ///     Gets or sets the puts.
        /// </summary>
        /// <value>
        ///     The puts.
        /// </value>
        List<ISingleAssetOption> Puts { get; set; }

        /// <summary>
        ///     The number of calls
        /// </summary>
        int NumberOfCalls { get; set; }

        /// <summary>
        ///     The number of puts
        /// </summary>
        int NumberOfPuts { get; set; }
    }
}