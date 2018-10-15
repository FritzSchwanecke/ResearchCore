using System;
using IReserachCore.Helper.Time.DayCounter;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResearchCore.Helper.Time.DayCounter;
using Unity;

namespace ResearchCore.UnitTests.Helper.Time.DayCounter
{
    [TestClass]
    public class DcUnitTest
    {
        IUnityContainer IoCContainer;

        private void RegisterToUnity()
        {
            this.IoCContainer = new UnityContainer();
            IoCContainer.RegisterType<IDcAct360,DcAct360>();

        }

        [TestMethod]
        public void DcAct360UnitTest()
        {
            // Arrange
            RegisterToUnity();
            var dayCounter = IoCContainer.Resolve<IDcAct360>();
            var startDate = new DateTime(2017, 1, 1);
            var endDate = new DateTime(2017,2,1);


            // Act
            var timedifference = dayCounter.YearFraction(startDate, endDate);

            // Assert
            Assert.AreEqual(31d/360d, timedifference);
        }





    }
}