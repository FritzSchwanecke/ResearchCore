namespace IReserachCore.Helper.Time.Compounder
{
    public interface ICompounder
    {
        double Compound(double interestRate, double yearFraction);
        double Discount(double interestRate, double yearFraction);
        double Compound(double[] interestRates, double[] yearFractions);
        double Discount(double[] interestRates, double[] yearFractions);
    }
}