using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.User.Entities
{
    [Table("user_night")]
    public class UserNight
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }
        public DateTime night_date { get; set; }
        public int price { get; set; }
        public int bed { get; set; }
        public int room { get; set; }
    }

   
}