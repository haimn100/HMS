using casa_benjamin.Modules.Restaurant.Inventory.Values;
using casa_benjamin.Modules.Shared.Repositories;

namespace casa_benjamin.Modules.Restaurant.Inventory.Services
{
    public class InventoryService
    {
        private GenericRepository repository;
        private ProductService productService;

        public InventoryService(string dbConnectionString)
        {
            repository = new GenericRepository();
            productService = new ProductService(dbConnectionString);
        }
 
        //public void UpdateInventory(List<OrderItems> orderItems, int orderId, bool SubtractFromInventory)
        //{
        //    var menuItemsProducts = repository.Get<ProductAssign>($"select * from product_assign where menu_item_id in ({string.Join(",",orderItems.Select(x=>x.menu_item_id))})");
        //    var productsToUpdate = new List<KeyValuePair<int, decimal>>();

        //    foreach (var item in orderItems)
        //    {
        //        var menuItemProducts = menuItemsProducts.Where(x => x.menu_item_id == item.menu_item_id);

        //        foreach (var miProduct in menuItemProducts)
        //        {
        //            var p = new KeyValuePair<int, decimal>(
        //                  miProduct.product_id,
        //                  SubtractFromInventory ? -miProduct.product_weight : miProduct.product_weight);

        //            if (miProduct.menu_item_ing_id == 0)
        //            {
        //                /* menu_item_ing_id = 0 means that this product is not an
        //                * ingredient of the menu item but is integral part of it and should
        //                * always be updated
        //                */
        //                productsToUpdate.Add(p);
        //            }
        //            else
        //            {
        //                /* if the product is associated with an ingerdient, we need to check
        //                 * if the the ingredient is selected for this order
        //                */
        //                if (item.menu_item_ingredients_ids.Contains(miProduct.menu_item_ing_id))
        //                {
        //                    productsToUpdate.Add(p);
        //                }
        //            }
        //        }
        //    }

        //    var updateRequest = new UpdateInventoryRequest
        //    {
        //        ActionType = ProductQuantityChangeOrigin.MENU_ORDER,
        //        RelatedEntity = orderId,
        //        InventoryProductsToUpdate = productsToUpdate
        //    };

        //    UpdateInventory(updateRequest);          
        //}

        public void UpdateInventory(UpdateInventoryRequest req)
        {
            //lock (syncUpdate)
            //{
            //    var productsStockItems = stockService.FindByProductID(req.InventoryProductsToUpdate.Select(x => x.Key).ToList());

            //    using (var transactionScope = new TransactionScope())
            //    {
            //        using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            //        {
            //            con.Open();
            //            foreach (var productToUpdate in req.InventoryProductsToUpdate)
            //            {
            //                var stockItem = productsStockItems.First(x => x.product_id== productToUpdate.Key);

            //                //updated the stock item (productToUpdate.Value can be also negative)
            //                stockItem.quantity = stockItem.quantity + productToUpdate.Value;
            //                con.Update(stockItem);

            //                var stockItemLog = new ProductStockChanges
            //                {
            //                    change = productToUpdate.Value,
            //                    current_amount = stockItem.quantity,
            //                    related_entity = req.RelatedEntity,
            //                    action_type = req.ActionType,
            //                    stock_item_id = stockItem.id
            //                };
            //                con.Insert(stockItemLog);        
            //            }
            //        }
            //        transactionScope.Complete();
            //    }
            //}        
        } 
    }
}