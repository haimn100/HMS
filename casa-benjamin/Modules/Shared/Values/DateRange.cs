using System;

namespace casa_benjamin.Modules.Shared.Values
{
    public class DateRange
    {
        private DateTime from;
        private DateTime to;

        public DateRange(DateTime from ,DateTime to)
        {
            this.from = new DateTime(from.Year,from.Month,from.Day);
            this.to = new DateTime(to.Year, to.Month, to.Day);
        }

        public int GetNights()
        {
            return (to - from).Days;
        }
    }
}