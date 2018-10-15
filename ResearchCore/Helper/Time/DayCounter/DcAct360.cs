using System;
using IReserachCore.Helper.Time.DayCounter;

namespace ResearchCore.Helper.Time.DayCounter
{
    public class DcAct360 : IDcAct360
    {
        public eDcConvention DcConvention => eDcConvention.DcAct360;

        public double YearFraction(DateTime startDate, DateTime endDate)
        {
            var timeDelta = endDate - startDate;
            return timeDelta.TotalDays / 360d;
        }
    }
}