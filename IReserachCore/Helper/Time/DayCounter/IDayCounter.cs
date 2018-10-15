using System;

namespace IReserachCore.Helper.Time.DayCounter
{
    public interface IDayCounter
    {
        eDcConvention DcConvention { get; }
        double YearFraction(DateTime startDate, DateTime endDate);
    }
}