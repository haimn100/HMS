using System.Collections.Generic;

namespace casa_benjamin.Modules.Shared.Values
{
    public class PagedTableResponse<T> where T : class
    {
        public List<T> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }

    }
}
