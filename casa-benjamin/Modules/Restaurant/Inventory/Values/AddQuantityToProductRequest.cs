using casa_benjamin.Modules.Restaurant.Inventory.Enums;

namespace casa_benjamin.Modules.Restaurant.Inventory.Values
{
    public class AddQuantityToProductRequest
    {
        public int productId { get; set; }
        public decimal quantity { get; set; }
        public string note { get; set; }
        public ProductQuantityChangeOrigin origin { get; set; }
    }
}