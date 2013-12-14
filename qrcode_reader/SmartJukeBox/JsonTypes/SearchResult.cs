using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartJukeBox.JsonTypes
{
    public class SearchResult
    {
        public Artist[] artists;
    }

    public class Artist
    {
        public string name { get; set; }

        public string href { get; set; }

        public string popularity { get; set; }
    }
}
