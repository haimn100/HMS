using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.HouseKeeping.Data.Models
{
    [Table("house_keeper")]
    public class HouseKeeper
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public DateTime create_date { get; set; }
        public bool is_active { get; set; }
    }
}