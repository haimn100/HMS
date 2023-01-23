using System.Linq;

namespace casa_benjamin.Extensions
{
    public static class StringExtensions
    {

        public static string Capitalize(this string str)
        {
            return str.First().ToString().ToUpper() + str.Substring(1);
        }

        public static string Sex(this string str)
        {
            return str.ToLower() == "true" ? "M" : "F";
        }

            
    }
}