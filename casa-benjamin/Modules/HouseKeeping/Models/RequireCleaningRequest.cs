using System;

namespace casa_benjamin.Modules.HouseKeeping.Models
{
    public class RequireCleaningRequest
    {
        public int room_number { get; set; }
        public int house_keeper_id { get; set; }
        public string house_keeper_name { get; set; }
        public DateTime assigned_date { get; set; }
        public string comment { get; set; }
    }
}