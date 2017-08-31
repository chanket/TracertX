using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace TracertX.IPSearchers
{
    class GeoLite2IPSearcher : IPSearcher
    {
        protected MaxMind.Db.Reader City { get; } = null;

        public GeoLite2IPSearcher(string file)
        {
            City = new MaxMind.Db.Reader(file);
        }

        public override string Search(IPAddress ip)
        {
            string addr = "";
            var data1 = City.Find<Dictionary<string, object>>(ip);
            if (data1 != null)
            {
                if (data1.ContainsKey("country"))
                {
                    var country = data1["country"] as Dictionary<string, object>;
                    if (country != null)
                    {
                        if (country.ContainsKey("names"))
                        {
                            var names = country["names"] as Dictionary<string, object>;
                            if (names != null)
                            {
                                if (names.ContainsKey("zh-CN"))
                                {
                                    addr += names["zh-CN"];
                                }
                                else if (names.ContainsKey("en"))
                                {
                                    addr += names["en"];
                                }
                            }
                        }
                    }
                }
                if (data1.ContainsKey("subdivisions"))
                {
                    var subdivisions = data1["subdivisions"] as List<object>;
                    for (int i=0; i<subdivisions.Count(); i++)
                    {
                        var subdivision = subdivisions[i] as Dictionary<string, object>;
                        if (subdivision != null)
                        {
                            if (subdivision.ContainsKey("names"))
                            {
                                var names = subdivision["names"] as Dictionary<string, object>;
                                if (names != null)
                                {
                                    if (names.ContainsKey("zh-CN"))
                                    {
                                        addr += names["zh-CN"];
                                    }
                                    else if (names.ContainsKey("en"))
                                    {
                                        addr += " " + names["en"];
                                    }
                                }
                            }
                        }
                    }
                }
                if (data1.ContainsKey("city"))
                {
                    var city = data1["city"] as Dictionary<string, object>;
                    if (city != null)
                    {
                        if (city.ContainsKey("names"))
                        {
                            var names = city["names"] as Dictionary<string, object>;
                            if (names != null)
                            {
                                if (names.ContainsKey("zh-CN"))
                                {
                                    addr += names["zh-CN"];
                                }
                                else if (names.ContainsKey("en"))
                                {
                                    addr += " " + names["en"];
                                }
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(addr)) return base.Search(ip);
            else return addr;
        }
    }
}
