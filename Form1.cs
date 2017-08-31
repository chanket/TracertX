using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TracertX.IPSearchers;

namespace TracertX
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        GeoLite2IPSearcher geolite2 = new GeoLite2IPSearcher("GeoLite2-City.mmdb");
        CzIPSearcher cz = new CzIPSearcher("qqwry.dat");

        private void Tracert(string address, int timeout, int ttl = 1)
        {
            Ping p = new Ping();
            PingOptions po = new PingOptions()
            {
                Ttl = ttl,
            };
            p.PingCompleted += PingCompleted;
            p.SendAsync(address, timeout, new byte[32], po, ttl);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Tracert(textBox1.Text, 5);
        }

        private void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            int ttl = (int)e.UserState;
            if (e.Error != null)
            {
                if (e.Error.InnerException != null)
                    Console.WriteLine(e.Error.InnerException.Message);
                else
                    Console.WriteLine(e.Error.Message);
            }
            else
            {
                switch (e.Reply.Status)
                {
                    case IPStatus.TimedOut:
                    case IPStatus.TtlExpired:
                        {
                            Console.WriteLine(e.Reply.Address.ToString() + "\t" + cz.Search(e.Reply.Address));
                            Tracert(textBox1.Text, 5, ttl + 1);
                        }
                        break;
                    case IPStatus.Success:
                        {
                            Console.WriteLine(e.Reply.Address.ToString() + "\t" + cz.Search(e.Reply.Address));
                        }
                        break;
                }
            }
        }
    }
}
