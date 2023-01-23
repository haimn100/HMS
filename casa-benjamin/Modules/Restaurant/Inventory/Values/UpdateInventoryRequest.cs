using casa_benjamin.Modules.Restaurant.Inventory.Enums;
using System.Collections.Generic;

namespace casa_benjamin.Modules.Restaurant.Inventory.Values
{
    public class UpdateInventoryRequest
    {
        /// <summary>
        /// List of <(int)ProductID, (decimal)UpdateAmount>
        /// </summary>
        public List<KeyValuePair<int,decimal>> InventoryProductsToUpdate { get; set; }
        public ProductQuantityChangeOrigin ActionType { get; set; }
        public int? RelatedEntity { get; set; }
    }
}