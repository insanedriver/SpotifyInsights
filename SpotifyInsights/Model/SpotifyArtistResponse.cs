using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyInsights.Model
{
    public class SpotifyArtistResponse
    {
        public List<Artist> artists { get; set; }

        public class Artist
        {
            public Followers followers { get; set; }
            public External_Urls external_urls { get; set; }
            public object[] genres { get; set; }
            public string href { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public int popularity { get; set; }
            public string type { get; set; }
            public string uri { get; set; }
        }

        public class Followers
        {
            public object href { get; set; }
            public int total { get; set; }
        }

        public class External_Urls
        {
            public string spotify { get; set; }
        }
    }
}
