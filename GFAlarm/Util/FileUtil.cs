using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Util
{
    public class FileUtil
    {
        public static string GetFile(string path)
        {
            if (File.Exists(path))
                return File.ReadAllText(path);
            return "";
        }
    }
}
