
using casa_benjamin.Modules.Booking.Room.Enums;
using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.Booking.Room.Entities
{
    [Table("room")]
    public class Room
    {
        [Key]
        public int id { get; set; }

        public int room_number { get; set; }
        public int floor { get; set; }
        public RoomType room_type_id { get; set; }
        public bool is_clean_required { get; set; }
        public bool is_cleaning_inspection_required { get; set; }
        public string assigned_house_keeper_name { get; set; }
        public DateTime? assigned_house_keeper_date { get; set; }
        public int? assigned_house_keeper_id { get; set; }
        public int? house_keeping_tracking_id { get; set; }
        public string note { get; set; }
        public DateTime? is_cleaning_inspection_date { get; set; }
        public DateTime? last_cleaned { get; set; }
        public string amenities { get; set; }
    }
}