using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Util
{
    public static class Os
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static string productName = "";
        public static string releaseId = "";

        public static bool isWin7
        {
            get
            {
                productName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
                if (productName.Contains("Windows 7"))
                    return true;
                return false;
            }
        }

        public static bool isWin10
        {
            get
            {
                productName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
                if (productName.Contains("Windows 10"))
                    return true;
                return false;
            }
        }
    }
}
