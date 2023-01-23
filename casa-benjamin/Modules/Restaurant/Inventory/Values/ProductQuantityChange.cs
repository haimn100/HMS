using casa_benjamin.Modules.Restaurant.Inventory.Enums;
using casa_benjamin.Modules.Inventory.Enums;

namespace casa_benjamin.Modules.Restaurant.Inventory.Values
{
    public class ProductQuantityChange
    {
        public int ProductId { get; set; }
        public decimal Value { get; set; }
        public ProductUnit ProductUnit { get; set; }
        public ProductQuantityChangeOrigin Origin { get; set; }
        public string Note { get; set; }
    }
}