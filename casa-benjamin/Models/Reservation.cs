
using casa_benjamin.Modules.Booking.Room.Enums;
using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Models
{
    [Table("reservation")]
    public class Reservation
    {

        [Key]
        public int id { get; set; }
        public int res_id { get; set; }
        public RoomType room_type { get; set; }
        public DateTime res_date { get; set; }
        public bool sex { get; set; }
        public int nights { get; set; }
        public string res_name { get; set; }
        public string employee_name { get; set; }
        public ReserVationStatus status { get; set; }
        public bool allow_mix_dorm { get; set; }
        public int room_id { get; set; }
        public DateTime res_date_end { get; set; }
        public int number_of_people { get; set; }
        public string comment { get; set; }
        public bool from_channel_manager { get; set; }

        public string origin { get; set; }

        public DateTime deleted_at{ get; set; }

        /// <summary>
        /// Email of the reserver
        /// </summary>
        public string res_email { get; set; }

        public Reservation ShallowCopy()
        {
            return (Reservation)this.MemberwiseClone();
        }

        public static int GenereateId()
        {
            return new Random().Next();
        }

    }

    public enum ReserVationStatus
    {
        Confirmed = 1,
        CheckedIn = 2,
        CheckedOut = 3
    }
}