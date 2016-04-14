using System;

namespace Debonair.Utilities.Extensions
{
    public static class DateExts
    {
        public static bool IsBusinessDay(this DateTime date)
        {
            return
                date.DayOfWeek != DayOfWeek.Saturday &&
                date.DayOfWeek != DayOfWeek.Sunday;
        }

        public static int BusinessDaysSince(this DateTime fromDate)
        {
            return BusinessDaysTo(fromDate, DateTime.Now);
        }

        public static int BusinessDaysTo(this DateTime fromDate, DateTime toDate)
        {
            var ret = -1;
            var dt = fromDate;
            while (dt < toDate)
            {
                if (dt.IsBusinessDay())
                {
                    ret++;
                }

                dt = dt.AddDays(1);
            }
            return ret;
        }
    }
}
