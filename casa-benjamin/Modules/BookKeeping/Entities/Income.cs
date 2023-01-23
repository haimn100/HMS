using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.BookKeeping.Entities
{
    [Table("income")]
    public class Income
    {
        [Key]
        public int id { get; set; }
        public DateTime date { get; set; }
        public int category_id { get; set; }
        public decimal val { get; set; }
        public DateTime report_date { get; set; }
        public string comment { get; set; }
    }
}