using System;

namespace IReserachCore.Helper.Time.DayCounter
{
    public interface IDcAct360
    {
        eDcConvention DcConvention { get; }
        double YearFraction(DateTime startDate, DateTime endDate);
    }
}