using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyInsights.Model
{
    class Input
    {
            public string Endpoint { get; set; }
            public string Token { get; set; }
            public List<Artist> Artists { get; set; }
    }
}
