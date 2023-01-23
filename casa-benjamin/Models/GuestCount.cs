using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Models
{
    [Table("guest_count")]
    public class GuestCount
    {
        [Key]
        public int id { get; set; }

        public DateTime guestcount_date { get; set; }
        public long count { get; set; }
    }
}