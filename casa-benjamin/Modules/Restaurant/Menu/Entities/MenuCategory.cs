using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.Restaurant.Menu.Entities
{
    [Table("menu_category")]
    public class MenuCategory
    {
        public int id { get; set; }
        public string name { get; set; }
        public int number { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public int? parent_cat_id { get; set; }
        public MenuCategoryType menu_category_type { get; set; }
    }

    public enum MenuCategoryType
    {
        Kitchen = 1,
        Service
    }
}