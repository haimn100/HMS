using System.Collections.Generic;

namespace casa_benjamin.Models
{

    public class Graph
    {
        public string Title { get; set; }
        public List<GraphSeries> SeriesList { get; set; }
    }

    public class GraphSeries
    {
        public string EndPoint { get; set; }
        public string Name { get; set; }
    }
}