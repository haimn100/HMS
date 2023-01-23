using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.User.Entities
{
    [Table("user_night_log")]
    public class UserNightLog
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }
        public string user_name { get; set; }
        public string staff_name { get; set; }
        public string action { get; set; }
        public DateTime current_date { get; set; }
        public string action_val { get; set; }
    }

   
}