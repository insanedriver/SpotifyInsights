using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyInsights.Model
{
    class Artist
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public int MonthlyListeners { get; set; }
        public int Followers { get; set; }
        public int Popularity { get; set; }

    }
}
