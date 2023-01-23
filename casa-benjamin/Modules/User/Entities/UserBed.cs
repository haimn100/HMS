using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.User.Entities
{
    [Table("user_bed")]
    public class UserBed
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }
        public int bed_id { get; set; }
        public double price { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string comment { get; set; }
    }
}