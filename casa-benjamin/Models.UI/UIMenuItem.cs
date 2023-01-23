using casa_benjamin.Modules.Restaurant.Menu.Entities;
using System.Collections.Generic;

namespace casa_benjamin.Models
{
    public class UIMenuItem
    {
        public MenuItem menuItem { get; set; }
        public MenuCategory menuItemCategory { get; set; }
        public List<MenuItemIngredient> ingredients { get; set; }

        public UIMenuItem(MenuItem menuItem, List<MenuItemIngredient> ingredients,MenuCategory menuItemCategory)
        {
            this.menuItem = menuItem;
            this.ingredients = ingredients;
            this.menuItemCategory = menuItemCategory;
        }
    }
}