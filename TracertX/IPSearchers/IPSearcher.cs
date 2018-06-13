using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TracertX.IPSearchers
{
    enum IPSearcherImpl : int
    {
        Cz88 = 0,
        GeoLite2 = 1,
    }

    class IPSearcher
    {
        public static IPSearcher Create(IPSearcherImpl impl)
        {
            switch (impl)
            {
                case IPSearcherImpl.Cz88: return new CzIPSearcher();
                case IPSearcherImpl.GeoLite2: return new GeoLite2IPSearcher();
                default: return null;
            }
        }

        public virtual string Search(IPAddress ip)
        {
            return "Unknown";
        }
    }
}
