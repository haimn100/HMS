using casa_benjamin.Modules.Booking.Reservation.Enums;
using Dapper.Contrib.Extensions;
using System;


namespace casa_benjamin.Modules.Booking.Reservation.Entities
{
    [Table("reservations")]
    public class Reservation
    {
        public int id { get; set; }
        public string holder { get; set; }
        public int guest_id { get; set; }
        public ReservationStatus reservation_status { get; set; }
        public int nights { get; set; }
        public int total_guests { get; set; }
        public DateTime? check_in { get; set; }
        public DateTime? check_out { get; set; }
        public ReservationSource source { get; set; }
        public DateTime? created { get; set; }
    }
}