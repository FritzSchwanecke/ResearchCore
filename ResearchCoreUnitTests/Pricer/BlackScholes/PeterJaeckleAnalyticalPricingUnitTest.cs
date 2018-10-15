using System;
using IReserachCore.Helper.Functions;
using IReserachCore.Instruments;
using IReserachCore.Instruments.Options;
using IReserachCore.Pricer.BlackScholes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResearchCore.Helper.Functions;
using ResearchCore.Instruments.Options;
using ResearchCore.Pricer.BlackScholes;
using Unity;

namespace ResearchCore.UnitTests.Pricer.BlackScholes
{
    public class PeterJaeckleAnalyticalPricingUnitTest
    {
        [TestClass]
        public class PriceOption
        {
            /// <summary>
            /// The io c container
            /// </summary>
            private IUnityContainer IoCContainer;

            private void RegisterToUnity()
            {
                IoCContainer = new UnityContainer();
                IoCContainer.RegisterType<IPeterJaeckleAnalyticalPricing, PeterJaeckleAnalyticalPricing>();
                IoCContainer.RegisterType<IPeterJaeckleAnalyticalInversePricing, PeterJaeckleAnalyticalInversePricing>();
                IoCContainer.RegisterType<ISingleAssetOption, SingleAssetOption>();
            }

            [TestMethod]
            public void PriceOptionUnitTest()
            {
                // Arrange
                RegisterToUnity();
                var pricingEngine = IoCContainer.Resolve<IPeterJaeckleAnalyticalPricing>();
                var option = IoCContainer.Resolve<ISingleAssetOption>();
                option.OptionType = eOptionType.C;
                option.ImpliedVolatility = 0.15;
                option.Maturity = new DateTime(2018,6,30);
                option.ValuationDate = new DateTime(2018,1,1);
                option.Currency = eCurrency.EUR;
                option.Strike = 60.0;
                option.StockPrice = 50.0;

                option.Premium = pricingEngine.NetPresentValue(option.ValuationDate, option, option.StockPrice);

            }

            [TestMethod]
            public void DetermineImpliedVolatility()
            {
                // Arrange
                RegisterToUnity();
                var inversepricingEngine = IoCContainer.Resolve<IPeterJaeckleAnalyticalInversePricing>();
                var option = IoCContainer.Resolve<ISingleAssetOption>();
                option.OptionType = eOptionType.C;
                option.Premium = 0.12;
                option.Maturity = new DateTime(2014, 1, 17);
                option.ValuationDate = new DateTime(2013, 1, 2);
                option.Currency = eCurrency.EUR;
                option.Strike = 2.0;
                option.StockPrice = 0.859;

                option.ImpliedVolatility = inversepricingEngine.Volatility(option.ValuationDate, option, option.StockPrice, 0.713131555, 0.991290003);

            }
        }
    }
}