using System;
using System.Collections.Generic;
using IDataAccessLayer.LoadData;
using IReserachCore.Instruments.Options;
using JetBrains.Annotations;

namespace IDataAccessLayer.ProcessData
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOptionDataProvider
    {
        IOptionDictionary GenerateOptionDictionaries([NotNull] IEnumerable<ISingleAssetOption> optionEnumerable,
            string underlyingName, IInterestRateTable interestRateTable);

        IOptionList GenerateOptionList([NotNull] IEnumerable<ISingleAssetOption> optionEnumerable,
            string underlyingName, IInterestRateTable interestRateTable);

        IContainerSingleAssetOption CreateSingleAssetOptionContainer(DateTime startDate, DateTime endDate,
            int numberOfStrikes, int numberOfMaturities);
    }
}