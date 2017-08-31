using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TracertX.IPSearchers
{
    class IPSearcher
    {
        public virtual string Search(IPAddress ip)
        {
            return "未知地址";
        }
    }
}
