using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Util
{
    public class CompressUtil
    {
        public static string Compress(string str)
        {
            byte[] row = Encoding.UTF8.GetBytes(str);
            byte[] compressed = null;
            using (var outStream = new MemoryStream())
            {
                using (var hgs = new GZipStream(outStream, CompressionMode.Compress))
                {
                    hgs.Write(row, 0, row.Length);
                }
                compressed = outStream.ToArray();
            }
            return Convert.ToBase64String(compressed);
        }

        public static string Decompress(string str)
        {
            string output = "";
            byte[] compressed = Convert.FromBase64String(str);
            using (var decomStream = new MemoryStream(compressed))
            {
                using (var hgs = new GZipStream(decomStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(hgs))
                    {
                        output = reader.ReadToEnd();
                    }
                }
            }
            return output;
        }
    }
}
