using System;

namespace Adfnet.Core.Helpers
{

    public static class DateTimeHelper
    {
        public static DateTime FirstOfCurrentMonth()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, 0);
        }


        public static DateTime ResetTimeToStartOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }

        public static DateTime ResetTimeToEndOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999);
        }
    }
}
