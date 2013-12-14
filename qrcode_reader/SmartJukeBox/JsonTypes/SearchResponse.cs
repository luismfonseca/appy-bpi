using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartJukeBox.JsonTypes
{
    public class SearchResponse
    {
        public SearchResult results = new SearchResult();
    }

    public class SearchResult
    {
        public Artists artistmatches;
    }

    public class Artists
    {
        public Artist[] artist;
    }

    public class Artist
    {
        public string name { get; set; }

        public string mbid { get; set; }

        public ImageResult[] image { get; set; }
    }

    public class ImageResult
    {
        public string text { get; set; }

        public string size { get; set;}
    }
}
