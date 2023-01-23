using casa_benjamin.Modules.Booking.Room.Entities;
using System.Collections.Generic;

namespace casa_benjamin.Models
{
    public class UIRoom
    {
        public Room room { get; set; }
        public List<RoomBed> beds { get; set; }
    }
}