using GFAlarm.Data;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;

namespace GFAlarm.Util
{
    public class Common
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 모든 사설 IP 가져오기
        /// </summary>
        /// <returns></returns>
        internal static List<string> GetAllLocalIP()
        {
            List<string> list = new List<string>();
            string hostname = Dns.GetHostName();
            IPHostEntry ipHostEntry = Dns.GetHostEntry(hostname);
            foreach (IPAddress ipAddress in ipHostEntry.AddressList)
            {
                list.Add(ipAddress.ToString());
                log.Debug("사용가능한 IP {0}", ipAddress.ToString());
            }
            return list;
        }

        // 사설 IP 가져오기
        // https://stackoverflow.com/a/28621250
        internal static string GetLocalIPv4()
        {
            string output = "IP 불러오는 중 문제 발생";
            string ethernetIp = "";
            string wirelessIp = "";

            try
            {
                NetworkInterfaceType _type = NetworkInterfaceType.Ethernet;
                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                    {
                        IPInterfaceProperties adapterProperties = item.GetIPProperties();

                        if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                        {
                            foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                            {
                                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    ethernetIp = ip.Address.ToString();
                                    //log.Info("ip {0}", output);
                                }
                            }
                        }
                    }
                }

                _type = NetworkInterfaceType.Wireless80211;
                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                    {
                        IPInterfaceProperties adapterProperties = item.GetIPProperties();

                        if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                        {
                            foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                            {
                                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    wirelessIp = ip.Address.ToString();
                                    //log.Info("ip {0}", output);
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("IP주소 불러오는 중 에러 발생 " + ex.ToString());
            }

            if (Regex.Match(wirelessIp, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Success)
                output = wirelessIp;
            if (Regex.Match(ethernetIp, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Success)
                output = ethernetIp;
            if ("0.0.0.0".Equals(output))
                output = "IP 불러오는 중 문제 발생";

            return output;
        }

        /// <summary>
        /// URI 엘리먼트 가져오기
        /// </summary>
        public static string GetUriQuery(string sUri, string key)
        {
            try
            {
                Uri uri = new Uri(sUri);
                NameValueCollection queries = HttpUtility.ParseQueryString(uri.Query);
                string queryContent = queries.Get(key);
                return queryContent;
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to uri query - uri={0}, target={1}", sUri, key);
            }
            return "";
        }

        /// <summary>
        /// 요청 값 가져오기
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetUriQueries(string url)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                Uri uri = new Uri(url);
                NameValueCollection queries = HttpUtility.ParseQueryString(uri.Query);
                string[] keys = queries.AllKeys;
                foreach (string key in keys)
                    result.Add(key, queries.Get(key));
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to parse uri query - uri={0}", url);
            }
            return result;
        }

        public static DateTime LocalDateTimeConvertToKorea(DateTime dateTime)
        {
            TimeZoneInfo timeZoneSource = TimeZoneInfo.Local;
            //TimeZoneInfo timeZoneDestination = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            TimeZoneInfo timeZoneDestination = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
            return TimeZoneInfo.ConvertTime(dateTime, timeZoneSource, timeZoneDestination);
        }

        public static int ConvertDateTime_Korea_Int(DateTime time, bool ifoffset = false)
        {
            try
            {
                TimeZoneInfo timeZoneSource = TimeZoneInfo.Local;
                //TimeZoneInfo timeZoneDestination = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                TimeZoneInfo timeZoneDestination = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
                time = TimeZoneInfo.ConvertTime(time, timeZoneSource, timeZoneDestination);


                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(LocalDateTimeConvertToKorea(new DateTime(1970, 1, 1, 0, 0, 0, 0)));

                long t = (time.Ticks - startTime.Ticks) / 10000000;
                //if (ifoffset)
                //    return (int)t + ProgrameData.timeoffset;
                //else
                    return (int)t;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        /// <summary>
        /// 현재 절대경로 가져오기
        /// </summary>
        /// <returns></returns>
        public static string GetAbsolutePath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        // DPI 가져오기
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hDc, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        public const int LOGPIXELSX = 88;
        public const int LOGPIXELSY = 90;
        
        public static double GetDpiScale()
        {
            double dpi = 1.0;

            IntPtr hDc = GetDC(IntPtr.Zero);
            if (hDc != IntPtr.Zero)
            {
                int dpiX = GetDeviceCaps(hDc, LOGPIXELSX);
                ReleaseDC(IntPtr.Zero, hDc);

                dpi = (double)dpiX / 96;
            }

            return dpi;
        }

        /// <summary>
        /// 정상적인 제대 번호 여부
        /// </summary>
        /// <param name="team_id"></param>
        /// <returns></returns>
        public static bool IsValidTeamId(int team_id)
        {
            if (1 <= team_id && team_id <= 10)
                return true;
            return false;
        }

        /// <summary>
        /// 사잇값 여부
        /// </summary>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsBetween(int n, int a, int b)
        {
            if (a <= n && n <= b)
                return true;
            return false;
        }
    }
}
