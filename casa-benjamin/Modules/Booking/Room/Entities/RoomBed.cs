
using casa_benjamin.Modules.Booking.Room.Enums;
using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.Booking.Room.Entities
{
    [Table("room_bed")]
    public class RoomBed
    {
        [Key]
        public int id { get; set; }
        public int room_id { get; set; }
        public int room_number { get; set; }
        public int bed_id { get; set; }
        public BedType bed_type_id { get; set; }
        public int? double_bed_partner_id { get; set; }
        public DateTime? last_checkout { get; set; }
    }
}