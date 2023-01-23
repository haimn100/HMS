using Dapper.Contrib.Extensions;

namespace casa_benjamin.Models
{
    [Table("resident")]
    public class Resident
    {
        [Key]
        public int id { get; set; }
        public int user_id { get; set; }
    }
}