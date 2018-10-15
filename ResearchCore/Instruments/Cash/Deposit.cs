using System;
using IReserachCore.Helper.Time;
using IReserachCore.Helper.Time.Compounder;
using IReserachCore.Helper.Time.DayCounter;
using IReserachCore.Instruments;
using IReserachCore.Instruments.Cash;

namespace ResearchCore.Instruments.Cash
{
    public sealed class Deposit : IDeposit
    {
        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        /// <value>
        ///     The position.
        /// </value>
        public eRatesPosition Position { get; set; }

        /// <summary>
        ///     Gets or sets the currency.
        /// </summary>
        /// <value>
        ///     The currency.
        /// </value>
        public eCurrency Currency { get; set; }

        /// <summary>
        ///     Gets or sets the notional.
        /// </summary>
        /// <value>
        ///     The notional.
        /// </value>
        public double Notional { get; set; }

        /// <summary>
        ///     Gets or sets the accrual.
        /// </summary>
        /// <value>
        ///     The accrual.
        /// </value>
        public double Accrual { get; set; }

        /// <summary>
        ///     Gets or sets the payment date.
        /// </summary>
        /// <value>
        ///     The payment date.
        /// </value>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        ///     Gets or sets the effective date.
        /// </summary>
        /// <value>
        ///     The effective date.
        /// </value>
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        ///     Gets or sets the holiday.
        /// </summary>
        /// <value>
        ///     The holiday.
        /// </value>
        public IHolidayCalendar Holiday { get; set; }

        /// <summary>
        ///     Gets or sets the day counter.
        /// </summary>
        /// <value>
        ///     The day counter.
        /// </value>
        public IDayCounter DayCounter { get; set; }

        /// <summary>
        ///     Gets or sets the compounder.
        /// </summary>
        /// <value>
        ///     The compounder.
        /// </value>
        public ICompounder Compounder { get; set; }
    }
}