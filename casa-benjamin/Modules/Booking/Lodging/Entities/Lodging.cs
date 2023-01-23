using casa_benjamin.Modules.Booking.Lodging.Enums;
using Dapper.Contrib.Extensions;
using System;


namespace casa_benjamin.Modules.Booking.Lodging.Entities
{
    [Table("reservation_stayin")]
    public class Lodging
    {
        [Key]
        public int id { get; set; }
        public int res_id { get; set; }
        public int nights { get; set; }
        public DateTime check_in { get; set; }
        public DateTime? check_out { get; set; }
        public int guest_id { get; set; }
        public int guest_number { get; set; }
        public int room { get; set; }
        public int bed { get; set; }
        public decimal price { get; set; }
        public LodgingStatus status{ get; set; }
        public DateTime created { get; set; }

    }
}