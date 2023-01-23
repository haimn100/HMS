using System;

namespace casa_benjamin.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        public static string GetMomentJSDateString()
        {
            return "DD/MM/YYYY";
        }

        public static string GetShortUIDateString()
        {
            return "dd/MM/yyyy";
        }

        public static string GetShortUIDateTimeString()
        {
            return "dd/MM/yyyy HH:mm";
        }

        public static string ToShortUIDateString(this DateTime dt)
        {
            return dt.ToString("dd/MM/yyyy");
        }

        public static string ToShortUIDateTimeString(this DateTime dt)
        {
            return dt.ToString("dd/MM/yyyy HH:mm");
        }

        public static string ToShortUIMonthlyDateString(this DateTime dt)
        {
            return dt.ToString("MM/yyyy");
        }

        public static string ToMySqlDateString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static string ToMySqlDateTimeString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}