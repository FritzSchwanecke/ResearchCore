using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using IReserachCore.Helper.Time;

namespace ResearchCore.Helper.Time
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IReserachCore.Helper.Time.IHolidayCalendar" />
    public class HolidayCalendar : IHolidayCalendar
    {
        private readonly HashSet<DateTime> _holidays;

        public HolidayCalendar(FileInfo holidayFileInfo)
        {
            if (holidayFileInfo is null || !holidayFileInfo.Exists)
                _holidays = new HashSet<DateTime>();
            else
                _holidays = new HashSet<DateTime>(ReadHolidays(holidayFileInfo));
        }

        public bool IsHoliday(DateTime date)
        {
            return _holidays.Contains(date);
        }

        public bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        [Pure]
        public IEnumerable<DateTime> GetHolidays()
        {
            return _holidays;
        }

        private static IEnumerable<DateTime> ReadHolidays(FileInfo holidayFileInfo)
        {
            var holidays = new List<DateTime>();
            using (var sr = new StreamReader(holidayFileInfo.DirectoryName ??
                                             throw new InvalidOperationException("Holiday Directory does not exist.")))
            {
                sr.ReadLine();
                var line = sr.ReadLine();
                while (line != null)
                {
                    holidays.Add(DateTime.Parse(line, CultureInfo.GetCultureInfo("de-DE")));
                    line = sr.ReadLine();
                }

                return holidays;
            }
        }
    }
}