using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.Restaurant.Menu.Entities
{
    [Table("menu_item_ingredient")]
    public class MenuItemIngredient
    {
        [Key]
        public int id { get; set; }

        public int menu_item_id { get; set; }
        public int ingredient_id { get; set; }
        public double ingredient_price { get; set; }
        public string ingredients_group { get; set; }
        public int ingredients_group_number { get; set; }
        public bool ingredients_group_single_select { get; set; }
        public decimal product_weight { get; set; }
        public int? product_id { get; set; }


        public bool IsRelatedToProductAndHaveQuantity()
        {
            return product_id.HasValue && product_weight > 0;
        }
    }
}