using System;
using System.Collections.Generic;
using IReserachCore.Instruments.Options;

namespace IReserachCore.Pricer.Dividends
{
    public interface IImpliedDividends
    {
        void Calculate(
            Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>> callDictionary,
            Dictionary<DateTime, Dictionary<DateTime, Dictionary<double, ISingleAssetOption>>> putDictionary);

        void Calculate(ref IContainerSingleAssetOption options);
    }
}