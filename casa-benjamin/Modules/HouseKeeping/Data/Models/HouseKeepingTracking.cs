using Dapper.Contrib.Extensions;
using System;


namespace casa_benjamin.Modules.HouseKeeping.Data.Models
{
    [Table("house_keeping_tracking")]
    public class HouseKeepingTracking
    {
        [Key]
        public int id { get; set; }

        public int house_keeper_id { get; set; }
        public string house_keeper_name { get; set; }
        public int room_number { get; set; }
        public DateTime assigned_date { get; set; }
        public DateTime? finish_date { get; set; }
        public int num_of_beds_cleaned { get; set; }
        public string comment { get; set; }
    }
}