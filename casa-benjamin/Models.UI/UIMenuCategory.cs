using casa_benjamin.Modules.Restaurant.Menu.Entities;
using System.Collections.Generic;

namespace casa_benjamin.Models
{
    public class UIMenuCategory
    {
        public MenuCategory category { get; set; }
        public List<UIMenuItem> menuItems { get; set; }

        public UIMenuCategory(MenuCategory category, List<UIMenuItem> menuItems)
        {
            this.category = category;
            this.menuItems = menuItems;
        }
    }
}