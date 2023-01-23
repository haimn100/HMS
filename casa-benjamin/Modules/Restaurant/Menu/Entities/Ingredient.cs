using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.Restaurant.Menu.Entities
{
    [Table("ingredient")]
    public class Ingredient
    {
        [Key]
        public int id { get; set; }
  
        public string name { get; set; }
    }
}