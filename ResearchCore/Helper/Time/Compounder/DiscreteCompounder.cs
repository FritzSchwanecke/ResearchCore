using System;
using System.Linq;
using IReserachCore.Helper.Time.Compounder;

namespace ResearchCore.Helper.Time.Compounder
{


    public class DiscreteCompounder : IDiscreteCompounder
    {
        public double Compound(double interestRate, double yearFraction)
        {
            return Math.Pow(1 + interestRate, yearFraction);
        }

        public double Discount(double interestRate, double yearFraction)
        {
            return Math.Pow(1 + interestRate, -yearFraction);
        }

        public double Compound(double[] interestRates, double[] yearFractions)
        {
            var temp = interestRates.Zip(yearFractions, this.Compound);
            return temp.Aggregate(1.0, (x, y) => x * y);
        }

        public double Discount(double[] interestRates, double[] yearFractions)
        {
            var temp = interestRates.Zip(yearFractions, this.Discount);
            return temp.Aggregate(1.0, (x, y) => x * y);
        }
    }
}
