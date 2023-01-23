

using casa_benjamin.Modules.Booking.Room.Enums;
using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.Booking.Room.Entities
{
    [Table("bed")]
    public class Bed
    {
        [Key]
        public int id { get; set; }

        public BedType bed_type_id { get; set; }
        public int? double_bed_partner_id { get; set; }      
    }   
}