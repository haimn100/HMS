using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;



namespace casa_benjamin.Modules.Restaurant.Inventory.Entities
{
    [Table("restaurant_inventory_product")]
    public class Product:ICloneable
    {
        [Key]
        public int id { get; set; }      
        public int supplier_id { get; set; }
        public string name { get; set; }
        public string brand { get; set; }
        public string code { get; set; }
        public decimal weight { get; set; }
        public decimal price { get; set; }
        public string note { get; set; }

        public decimal quantity_in_stock { get; set; }

        public decimal quantity_warning_thershold { get; set; }

        public DateTime? created_at { get; set; }

        public IReadOnlyList<string> IsValidProductToSave()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("Must contain title");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                errors.Add("Must contain code");
            }
            return errors;
        }
        public decimal CalcPriceByWeight(decimal weight)
        {
            return (price / weight) * weight;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}