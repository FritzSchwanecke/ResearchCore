using System;
using IReserachCore.Helper.Time.Compounder;
using IReserachCore.Helper.Time.DayCounter;
using IReserachCore.Helper.Time;

namespace IReserachCore.Instruments.Cash
{
    public interface IDeposit
    {
        eRatesPosition Position { get; set; }
        eCurrency Currency { get; set; }
        double Notional { get; set; }
        double Accrual { get; set; }
        DateTime PaymentDate { get; set; }
        DateTime EffectiveDate { get; set; }
        IHolidayCalendar Holiday { get; set; }
        IDayCounter DayCounter { get; set; }
        ICompounder Compounder { get; set; }
    }
}