using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.App.Values
{
    [Table("app_settings")]
    public class AppSettings
    {
        [Key]
        public int id { get; set; }

        public bool AddCreditCardChargeToRegister { get; set; }
        public bool RequireImmigrationInfo { get; set; }
        public string CountryIsoAlpha2Code { get; set; }
        public string CheckInDefaultNationality { get; set; }
        public string barcodePrefix { get; set; }
        public string language { get; set; }
        public string city { get; set; }
        public string hotel_code { get; set; }
        public string hotel_print_info { get; set; }
    }
}