using casa_benjamin.Models;
using casa_benjamin.Modules.User.Entities;
using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.Staff.Entities
{
    [Table("staff")]
    public class Staff
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public UserType type { get; set; }
        public DateTime start_date { get; set; }
        public bool is_working { get; set; }
        public string phone { get; set; }
        public string pin { get; set; }
    
    }

    [Table("staff_event")]
    public class StaffEvent
    {
        [Key]
        public int id { get; set; }

        public DateTime event_date { get; set; }
        public int staff_id { get; set; }
        public string staff_name { get; set; }
        public int? guest_id { get; set; }
        public EventType event_type_id { get; set; }
        public string event_value { get; set; }
    }
}