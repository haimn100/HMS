using casa_benjamin.Modules.Restaurant.Inventory.Entities;
using casa_benjamin.Modules.Restaurant.Inventory.Values;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Values;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace casa_benjamin.Modules.Restaurant.Inventory.Services
{
    public class ProductService
    {
        private GenericRepository repository;
        private string dbConnectionString;

        private const string PRODUCT_TABLE = "restaurant_inventory_product";

        public ProductService(string dbConnectionString)
        {
            this.dbConnectionString = dbConnectionString;
            repository = new GenericRepository();
        }

        public Product FindOne(int productId)
        {
            return repository.Get<Product>($"select * from {PRODUCT_TABLE} where id = {productId}").FirstOrDefault();
        }

        public List<Product> FindMany(List<int> productIds)
        {
            return repository.Get<Product>($"select * from {PRODUCT_TABLE} where id in ({string.Join(",",productIds.ToArray())}").ToList();
        }

        public List<Product> FindAll()
        {
            return repository.Get<Product>($"select * from {PRODUCT_TABLE}").ToList();
        }

        public PagedTableResponse<Product> Table(PagedTableRequest req)
        {
            req.table = PRODUCT_TABLE;
            return repository.GetTable<Product>(req);
        }

        public Product FindByName(string name)
        {
            string cleanName = CleanName(name);
            return repository.Get<Product>($"select * from {PRODUCT_TABLE} where name ='{cleanName}'").FirstOrDefault();
        }

        public List<Product> FindBySupllier(int supplierId)
        {
            return repository.Get<Product>($"select * from {PRODUCT_TABLE} where supplier_id = {supplierId}").ToList();
        }

        public void DeleteAndArchiveProduct(int productId)
        {
            using (var transactionScope = new TransactionScope())
            {              
                Product product = FindOne(productId);
                var archive = new InventoryArchive
                {
                    original_id = product.id.ToString(),
                    table_name = PRODUCT_TABLE,
                    record_data_json = JsonConvert.SerializeObject(product),
                    created_at = DateTime.Now
                };

                ////archive the product
                repository.Insert(archive);

                //delete the item
                repository.Delete(product);
               
                transactionScope.Complete();
            }
        }

        public int AddProduct(Product product) 
        {
            var errors = product.IsValidProductToSave();
            if (errors.Any())
            {
                throw new Exception(string.Join(";",errors));
            }

            return (int)repository.Insert(product);
        }

        public int AddOrRemoveQuantityAndUpdateStock(ProductQuantityChange quantityChange)
        {
            Product product = FindOne(quantityChange.ProductId);
            if (product == null) throw new Exception("Product not found");

            using (var transactionScope = new TransactionScope())
            {
                //create a stock record
                Stock newStockRecord = new Stock
                {
                    product_id = product.id,
                    change_in_quantity = quantityChange.Value,
                    origin = quantityChange.Origin,
                    quantity = product.quantity_in_stock + quantityChange.Value,
                    note = quantityChange.Note
                };

                int stockRecordId = (int)repository.Insert(newStockRecord);

                //Update product quantity
                product.quantity_in_stock = newStockRecord.quantity;
                repository.Update(product);               

                transactionScope.Complete();

                return stockRecordId;
            }
        }   

        public bool IsDuplicateProduct(string name, string code)
        {
            string cleanName = CleanName(name);
            var p = repository.Get<Product>($"select id, LOWER(name) from {PRODUCT_TABLE} where name ='{cleanName}' and code='{code}'").FirstOrDefault();
            return p != null;
        }

        public void UpdateProductWarningQuantity(int productId, decimal warningQuantity)
        {
            repository.ExecuteScalar($"update {PRODUCT_TABLE} set quantity_warning_thershold = {warningQuantity} where id = {productId}");
        }

        private string CleanName(string name)
        {
            return name.ToLower();
        }
    }
}