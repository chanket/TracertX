using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TracertX.IPSearchers;

namespace TracertX
{
    class Program
    {
        static IPAddress IP;
        static int Timeout = 5000;
        static int MaxTtl = 64;
        static IPSearcherImpl SearcherImpl = IPSearcherImpl.Cz88;
        static IPSearcher Searcher = IPSearcher.Create(SearcherImpl);

        static void Exit()
        {
            Process.GetCurrentProcess().Kill();
        }

        static void ExitUsage()
        {
            Console.WriteLine("Usage: ");
            Console.WriteLine("    " + AppDomain.CurrentDomain.FriendlyName + " [-w Timeout=" + Timeout + "] [-i TTL=" + MaxTtl + "] [-d Searcher=" + (int)SearcherImpl + "]");
            Console.WriteLine("    <Target>");
            Console.WriteLine();
            Console.WriteLine("Searchers: ");
            Console.WriteLine("           " + (int)IPSearcherImpl.Cz88 + ": " + IPSearcherImpl.Cz88.ToString());
            Console.WriteLine("           " + (int)IPSearcherImpl.GeoLite2 + ": " + IPSearcherImpl.GeoLite2.ToString());
            Exit();
        }

        static bool Parse(string[] args)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                switch (args[i])
                {
                    case "-w":
                        {
                            i++;
                            if (i >= args.Length - 1) return false;
                            if (!int.TryParse(args[i], out Timeout)) return false;
                            if (Timeout <= 0) return false;
                        }
                        break;

                    case "-i":
                        {
                            i++;
                            if (i >= args.Length - 1) return false;
                            if (!int.TryParse(args[i], out MaxTtl)) return false;
                            if (MaxTtl <= 0) return false;
                        }
                        break;

                    case "-d":
                        {
                            int searcher;
                            IPSearcherImpl searcherImpl;

                            i++;
                            if (i >= args.Length - 1) return false;
                            if (!int.TryParse(args[i], out searcher)) return false;
                            searcherImpl = (IPSearcherImpl)searcher;
                            Searcher = IPSearcher.Create(searcherImpl);
                            if (Searcher == null) return false;
                        }
                        break;

                    default:
                        {
                            return false;
                        }
                }
            }

            if (args.Length == 0) return false;
            if (!IPAddress.TryParse(args.Last(), out IP))
            {
                try
                {
                    IP = Dns.GetHostAddresses(args.Last())[0];
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        static PingReply Ping(IPAddress address, int timeout, int ttl)
        {
            Ping p = new Ping();
            PingOptions po = new PingOptions()
            {
                Ttl = ttl,
            };
            return p.Send(address, timeout, new byte[32], po);
        }

        static void Main(string[] args)
        {
            if (!Parse(args)) ExitUsage();

            Console.Write("Ping " + IP.ToString() + " ");
            PingReply reply = Ping(IP, Timeout, MaxTtl);
            Console.WriteLine(reply.Status.ToString() + ". ");
            if (reply.Status != IPStatus.Success) Exit();

            Console.WriteLine();
            Console.WriteLine("Tracert: ");
            for (int ttl = 1; ttl <= MaxTtl; ttl++)
            {
                reply = Ping(IP, Timeout, ttl);
                if (reply.Address != null)
                {
                    Console.Write(ttl + "\t" + reply.Address.ToString() + "\t");
                    Console.WriteLine(Searcher.Search(reply.Address));
                }

                if (reply.Status == IPStatus.Success) break;
            }
        }
    }
}
