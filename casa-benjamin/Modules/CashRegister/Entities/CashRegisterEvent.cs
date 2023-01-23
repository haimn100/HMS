using casa_benjamin.Models;
using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.CashRegister.Entities
{
    [Table("cash_register_event")]
    public class CashRegisterEvent
    {
        [Key]
        public int id { get; set; }

        public EventType event_type_id { get; set; }
        public decimal event_value { get; set; }
        public decimal current_register_amount { get; set; }
        public DateTime event_date { get; set; }
        public int staff_id { get; set; }
        public string staff_name { get; set; }
        public string comment { get; set; }
        public int? event_realted_entity_id { get; set; }
    }


}