using System;

namespace casa_benjamin.Models
{
    public class UIEndShift
    {
        public DateTime endshiftdate { get; set; }
        public int staff { get; set; }
        public decimal total { get; set; }
        public decimal totalcash { get; set; }
        public decimal totalcredit { get; set; }
        public decimal totalcanceled { get; set; }
        public decimal totalcheckouts { get; set; }
        public decimal totalkitchen { get; set; }
        public decimal totalservices { get; set; }
        public decimal checkoutscash { get; set; }
        public decimal checkoutscredit { get; set; }
        public decimal expenses { get; set; }
        public decimal incomes { get; set; }
    }
}