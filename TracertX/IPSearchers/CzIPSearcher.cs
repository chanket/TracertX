using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TracertX.IPSearchers
{
    class CzIPSearcher : IPSearcher
    {
        protected BinaryReader Reader { get; }
        protected int Start { get; }
        protected int End { get; }
        protected int Count { get; }

        protected string String(int offset, int count = 2)
        {
            string ret = "";
            if (offset != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                    var flag = Reader.ReadByte();
                    if (flag == 1)
                    {
                        var data = Reader.ReadBytes(3);
                        int offsetNew = data[0] + (data[1] << 8) + (data[2] << 16);
                        ret += String(offsetNew, count);
                        break;
                    }
                    if (flag == 2)
                    {
                        var data = Reader.ReadBytes(3);
                        int offsetNew = data[0] + (data[1] << 8) + (data[2] << 16);
                        ret += String(offsetNew, 1);
                        offset += 4;

                        if (i < count - 1) ret += " ";
                    }
                    else
                    {
                        Reader.BaseStream.Seek(-1, SeekOrigin.Current);
                        while (Reader.PeekChar() != 0) ret += Reader.ReadChar();
                        offset = (int)Reader.BaseStream.Position + 1;

                        if (i < count - 1) ret += " ";
                    }
                }
            }

            return ret;
        }

        protected string Record(int offset)
        {
            return String(offset, 2);
        }

        protected Tuple<uint, int> Index(int index)
        {
            Reader.BaseStream.Seek(Start + index * 7, SeekOrigin.Begin);
            byte[] data = Reader.ReadBytes(4);
            uint addr = (uint)data[0] + ((uint)data[1] << 8) + ((uint)data[2] << 16) + ((uint)data[3] << 24);
            data = Reader.ReadBytes(3);
            int offset = data[0] + (data[1] << 8) + (data[2] << 16);

            return new Tuple<uint, int>(addr, offset);
        }

        public CzIPSearcher(string file = null)
        {
            if (file == null) file = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.ToString() + "\\IPSearchers\\Data\\qqwry.dat";
            Reader = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.GetEncoding("GBK"));
            Start = Reader.ReadInt32();
            End = Reader.ReadInt32();
            Count = (End - Start) / 7;
        }

        public override string Search(IPAddress ip)
        {
            byte[] data = ip.GetAddressBytes().Reverse().ToArray();
            uint addr = BitConverter.ToUInt32(data, 0);

            int left = 0;
            int right = Count - 1;
            int mid;
            while (true)
            {
                mid = (left + right) / 2;
                var index = Index(mid);
                if (addr <= index.Item1)
                {
                    if (right == mid) break;
                    right = mid;
                }
                else
                {
                    if (left == mid) break;
                    left = mid;
                }
            }

            string ret = Record(Index(mid).Item2 + 4);
            if (string.IsNullOrEmpty(ret)) return base.Search(ip);
            else return ret;
        }
    }
}
