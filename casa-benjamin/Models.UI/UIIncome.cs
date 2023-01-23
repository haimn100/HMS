using System;

namespace casa_benjamin.Models
{
    public class UIIncome
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public int category_id { get; set; }
        public decimal val { get; set; }
        public string name { get; set; }
        public string type{ get; set; }
        public string comment { get; set; }
    }
}