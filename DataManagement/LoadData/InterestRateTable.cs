using System;
using IDataAccessLayer.LoadData;

namespace DataManagement.LoadData
{


    /// <summary>
    /// 
    /// </summary>
    public class InterestRateTable : IInterestRateTable
    {
        public DateTime[] ValuationDates { get; set; }
        public int[] Tenor { get; set; }
        public double[,] InterestRates { get; set; }
        readonly int _rowLen;
        readonly int _colLen;

        public InterestRateTable(DateTime[] valuationDates, int[] tenor)
        {
            ValuationDates = valuationDates;
            Tenor = tenor;


            _colLen = tenor.Length;
            _rowLen = valuationDates.Length;


            InterestRates = new double[_rowLen, _colLen];
        }

        /// <summary>
        /// Gets the index of the col.
        /// </summary>
        /// <param name="tenorInDays">The tenor in days.</param>
        /// <returns></returns>
        private int GetColIndex(int tenorInDays)
        {
            var colIndex = Array.BinarySearch<int>(this.Tenor, tenorInDays);
            if (colIndex >= 0) // element is found
            {
                if (colIndex > 0)
                    return colIndex - 1;
                if (colIndex < _colLen - 1)
                    return colIndex + 1;
            }
            else
            {
                colIndex = ~colIndex;
                if (colIndex < _colLen)
                    return colIndex;
                if (colIndex > 0)
                    return colIndex - 1;
            }

            return 0;
        }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <param name="valuationDate">The valuation date.</param>
        /// <returns></returns>
        private int GetRowIndex(DateTime valuationDate)
        {
            var rowIndex = Array.BinarySearch<DateTime>(this.ValuationDates, valuationDate);
            if (rowIndex >= 0) // element is found
            {
                if (rowIndex > 0)
                    return rowIndex - 1;

                if (rowIndex < _colLen - 1)
                    return rowIndex + 1;
            }
            else
            {
                rowIndex = ~rowIndex;
                if (rowIndex < _colLen)
                    return rowIndex;
                if (rowIndex > 0)
                    return rowIndex - 1;
            }

            return 0;
        }

        /// <summary>
        /// Gets the discount factor.
        /// </summary>
        /// <param name="valuationDate">The valuation date.</param>
        /// <param name="maturityDate">The maturity date.</param>
        /// <returns></returns>
        public double GetDiscountFactor(DateTime valuationDate, DateTime maturityDate)
        {
            var tenor = (int) Math.Round((maturityDate - valuationDate).TotalDays);

            var colIndex = GetColIndex(tenor);
            var rowIndex = GetRowIndex(valuationDate);

            var interestRate = InterestRates[rowIndex, colIndex];

            return Math.Exp(-interestRate * tenor / 365);
        }


    }
}