using casa_benjamin.Helpers;
using System;

namespace casa_benjamin.Models
{
    public class ReportDateQuery
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool Monthly { get; set; }

        public DateTime FromOrDefault()
        {
            if (From.HasValue)
            {
                return From.Value;
            }
            else
            {
                DateTime now = DateTimeHelper.GetCurrentDateTime();
                return new DateTime(now.Year, now.Month, now.Day);
            }
        }

        public DateTime ToOrDefault()
        {
            if (To.HasValue)
            {
                return To.Value;
            }
            else
            {
                DateTime now = DateTimeHelper.GetCurrentDateTime();
                return new DateTime(now.Year, now.Month, now.Day);
            }
        }
    }
}