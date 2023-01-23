using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.User.Entities
{
    [Table("user_night_task_log")]
    public class UserNightTaskLog
    {
        [Key]
        public int id { get; set; }

        public int success { get; set; }
        public DateTime task_date { get; set; }
        public string error { get; set; }
    }

   
}