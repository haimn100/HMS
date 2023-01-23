using casa_benjamin.Modules.Shared.Enums;
using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.User.Entities
{
    [Table("user")]
    public class User
    {
        [Key]
        public int id { get; set; }

        public string passport { get; set; }
        public DateTime cidate { get; set; }
        public DateTime? codate { get; set; }
        public bool is_checked_out { get; set; }
        public string pic { get; set; }
        public int bed_id { get; set; }

        /// <summary>
        /// True is male
        /// </summary>
        public bool sex { get; set; }

        public string name { get; set; }
        public string email { get; set; }
        public string nationality { get; set; }
        public string checked_in_by { get; set; }
        public string phone { get; set; }
        
        /// <summary>
        /// Immigration Fields
        /// </summary>
        public DateTime? birth_date { get; set; }
        public string arrival { get; set; }
        public string destination { get; set; }
        public string profession { get; set; }
        public DocumentType document_type { get; set; }
        public string barcode { get; set; }

        public string last_name { get; set; }
        public bool is_resident { get; set; }
        public DateTime intended_codate { get; set; }
        public int? res_id { get; set; }
    }

    public enum UserType
    {
        Admin = 1,
        Guest = 2,
        Employee = 3,
        Editor,
        HouseKeeper,
        KitchenManager
    }   
}