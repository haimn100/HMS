using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Restaurant.Menu.Values;
using casa_benjamin.Modules.Shared.Repositories;
using System;
using System.Collections.Generic;

namespace casa_benjamin.Modules.Restaurant.Menu.Services
{
    public class MenuService
    {

        private GenericRepository repository;
        private string dbConnectionString;

        private const string MENU_ITEM_INGERDIENTS_TABLE = "menu_item_ingredient";
        private const string MENU_ITEM_TABLE = "menu_item";
        private const string INGERDIENTS_TABLE = "ingredient";
        private const string MENU_CATEGORY = "menu_category";

        public MenuService(string dbConnectionString)
        {
            this.dbConnectionString = dbConnectionString;
            repository = new GenericRepository();
        }

        public List<MenuItemIngredient> GetMenuItemIngredients(int menuItemId)
        {
            return repository.Get<MenuItemIngredient>($"select * from {MENU_ITEM_INGERDIENTS_TABLE} where menu_item_id={menuItemId}");
        }

        public void UpdateIngredientsMenuGroup(MenuItemIngredient groupItem)
        {
            string query = $"update {MENU_ITEM_INGERDIENTS_TABLE} " +
                                    $"set ingredients_group = '{groupItem.ingredients_group}', " +
                                    $"ingredients_group_single_select = {groupItem.ingredients_group_single_select}" +
                                    $" where menu_item_id ={groupItem.menu_item_id} and ingredients_group_number = {groupItem.ingredients_group_number}";
            repository.ExecuteScalar(query);    
        }

        public List<MenuItem> GetActiveMenuItems()
        {
            return repository.Get<MenuItem>($"select * from {MENU_ITEM_TABLE} where is_deleted = 0");
        }

        public MenuItem GetMenuItem(int menuItemId)
        {
            return repository.GetOne<MenuItem>($"select * from {MENU_ITEM_TABLE} where id={menuItemId}");
        }

        public MenuItem IsDuplicateActiveMenuItem(MenuItem item)
        {
            return repository.GetOne<MenuItem>($"select * from {MENU_ITEM_TABLE} where LOWER(name) = '{item.name.ToLower()}' and cat_id = {item.cat_id} and is_active= 1 and is_deleted = 0");
        }

        public void UpdateMenuItem(UpdateMenuItemDTO item)
        {
            var dbItem = repository.GetOne<MenuItem>($"select * from {MENU_ITEM_TABLE} where id = {item.id} and is_deleted = 0");
            
            if(dbItem == null)
            {
                throw new System.Exception("Item does not exist");
            }

            dbItem.name = item.name;
            dbItem.number = item.number;
            dbItem.price = item.price;
            dbItem.product_id = item.product_id;
            dbItem.product_weight = item.product_weight;
            dbItem.is_active = item.is_active;
            repository.Update(dbItem);
        }

        public void UpdateMenuCategory(MenuCategory category)
        {
            var dbItem = repository.GetOne<MenuCategory>($"select * from {MENU_CATEGORY} where id = {category.id}");
            dbItem.name = category.name;
            dbItem.number = category.number;
            dbItem.is_active = category.is_active;
            dbItem.menu_category_type = category.menu_category_type;
            repository.Update(dbItem);
        }

        public int AddMenuItem(MenuItem item)
        {
            var dupItem = IsDuplicateActiveMenuItem(item);            
            if(dupItem != null) throw new System.Exception("Duplicate Item In Menu");

            return (int)repository.Insert(item);
        }

        public int AddMenuItemCategory(MenuCategory item)
        {
            var dbItem = repository.GetOne<MenuCategory>($"select * from {MENU_CATEGORY} " +
                $"where name = '{item.name}' " +
                $"and menu_category_type  = {(int)item.menu_category_type} " +
                $"and is_deleted = 0");
            
            if (dbItem != null)
            {
                throw new Exception("Category already exist");
            }

            return (int)repository.Insert(item);
        }

        public MenuCategory GetCategory(int id)
        {
            return repository.GetOne<MenuCategory>($"select * from {MENU_CATEGORY} where id={id}");
        }

        public MenuItemIngredient FindMenuItemIngredientById(int id)
        {
            return repository.GetOne<MenuItemIngredient>($"select * from {MENU_ITEM_INGERDIENTS_TABLE} where id={id}");
        }

        public List<Ingredient> GetIngredients()
        {
            return repository.Get<Ingredient>($"select * from {INGERDIENTS_TABLE}");
        }

    }
}