using System;
using System.Collections.Generic;

namespace IReserachCore.Helper.Time
{
    public interface IHolidayCalendar
    {
        /// <summary>
        ///     Determines whether the specified date is a holiday.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        ///     <c>true</c> if the specified date is a holiday; otherwise, <c>false</c>.
        /// </returns>
        bool IsHoliday(DateTime date);

        /// <summary>
        ///     Determines whether the specified date is a weekend.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        ///     <c>true</c> if the specified date is a weekend; otherwise, <c>false</c>.
        /// </returns>
        bool IsWeekend(DateTime date);

        /// <summary>
        ///     Gets the holidays.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DateTime> GetHolidays();
    }
}