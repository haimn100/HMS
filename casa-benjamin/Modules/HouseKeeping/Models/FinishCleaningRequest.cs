using System;

namespace casa_benjamin.Modules.HouseKeeping.Models
{
    public class FinishCleaningRequest
    {
        public int room_number { get; set; }
        public DateTime finish_date { get; set; }
        public int num_of_beds { get; set; }
        public string comment { get; set; }
    }
}