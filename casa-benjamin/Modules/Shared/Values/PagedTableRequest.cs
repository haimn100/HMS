using System;

namespace casa_benjamin.Modules.Shared.Values
{
    public class PagedTableRequest
    {
        public string table { get; set; }
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public string sortBy { get; set; }
        public bool sortDesc { get; set; }
        public string field { get; set; }
        public string fieldVal { get; set; }
        
        /// <summary>
        /// Equal to Where clause / Dont add 'where' in the string
        /// </summary>
        public string predicate { get; set; }
        public string groupBy { get; set; }
        public string dateField { get; set; }

        public string search { get; set; }
    }
}
