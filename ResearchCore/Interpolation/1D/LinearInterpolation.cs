using System;
using IReserachCore.Interpolation._1D;

namespace ResearchCore.Interpolation._1D
{
    public class LinearInterpolation : ILinearInterpolation
    {
        public eInterpolationMethod Method => eInterpolationMethod.linear;

        public double Calculate(double[] xValues, double[] yValues, double xValue)
        {
            var xIndex = Array.BinarySearch(xValues, xValue);

            if (xIndex > 0)
            {
                return yValues[xIndex];
            }

            var upperIndex = ~xIndex;
            var lowerIndex = upperIndex - 1;

            return yValues[lowerIndex] + (yValues[upperIndex] - yValues[lowerIndex]) *
                   (xValue - xValues[lowerIndex]) / (xValues[upperIndex] - xValues[lowerIndex]);
        }
    }
}