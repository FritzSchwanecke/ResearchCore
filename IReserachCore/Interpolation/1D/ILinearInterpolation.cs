namespace IReserachCore.Interpolation._1D
{
    public interface ILinearInterpolation
    {
        eInterpolationMethod Method { get; }

        double Calculate(double[] xValues, double[] yValues, double xValue);
    }
}