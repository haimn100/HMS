using Newtonsoft.Json;

namespace casa_benjamin.Extensions
{
    public static class JSONExtensions
    {
        public static string ToJSON(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}