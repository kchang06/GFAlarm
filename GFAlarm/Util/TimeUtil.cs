using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GFAlarm.Util
{
    public class TimeUtil
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public const int MINUTE = 60;
        public const int HOUR = 60 * MINUTE;
        public const int DAY = 24 * HOUR;
        public const int WEEK = 7 * DAY;

        public static int testSec = 0;

        /// <summary>
        /// HH:mm => Seconds
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ParseHHMM(string value)
        {
            Regex regex = new Regex(@"^[0-9][0-9]:[0-5][0-9]$");
            if (regex.IsMatch(value))
            {
                int time = 0;
                time += int.Parse(value.Substring(0, 2)) * HOUR;
                time += int.Parse(value.Substring(3, 2)) * MINUTE;
                return time;
            }
            return 0;
        }

        /// <summary>
        /// Seconds => DateTime
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="utc"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(int sec, bool utc = false)
        {
            try
            {
                TimeSpan ts = TimeSpan.FromSeconds(sec);
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + ts;
                if (utc)
                    return dt.ToUniversalTime();
                else
                    return dt.ToLocalTime();
            }
            catch { }
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Seconds => DateTime (format)
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="format"></param>
        /// <param name="utc"></param>
        /// <returns></returns>
        public static string GetDateTime(int sec, string format, bool utc = false)
        {
            return GetDateTime(sec, utc).ToString(format);
        }

        /// <summary>
        /// DateTime => Seconds
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetSec(DateTime dt)
        {
            return (int)dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        /// <summary>
        /// HH:MM => seconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetSec(string time)
        {
            Regex regex = new Regex(@"^[0-9][0-9]:[0-5][0-9]$");
            if (regex.IsMatch(time))
            {
                int sec = 0;
                sec += int.Parse(time.Substring(0, 2)) * 60 * 60;
                sec += int.Parse(time.Substring(3, 2)) * 60;
                return sec;
            }
            return 0;
        }

        /// <summary>
        /// seconds => HH:mm:ss
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static string GetTime(int sec)
        {
            return System.String.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}",
                sec / 3600,
                sec % 3600 / 60,
                sec % 60);
        }

        /// <summary>
        /// 현재 시간 (Seconds)
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentSec()
        {
            return (int)DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 현재 시간 (DateTime)
        /// </summary>
        /// <param name="utc"></param>
        /// <returns></returns>
        public static DateTime GetCurrentDateTime(bool utc = false)
        {
            if (utc)
                return DateTime.UtcNow;
            else
                return DateTime.Now;
        }

        /// <summary>
        /// 남은 시간 (Seconds)
        /// </summary>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetRemainSec(int endTime)
        {
            try
            {
                return endTime - GetCurrentSec();
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// 남은 시간 (HH:MM:SS)
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        public static string GetRemainHHMMSS(int endTime, int nowTime = 0)
        {
            if (nowTime == 0)
                nowTime = GetCurrentSec();

            try
            {
                int remainTime = endTime - nowTime;
                if (remainTime <= 0)
                    return "00:00:00";

                return System.String.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}",
                    remainTime / HOUR,
                    remainTime % HOUR / MINUTE,
                    remainTime % MINUTE);
            }
            catch { }
            return "00:00:00";
        }

        /// <summary>
        /// 자정까지 남은 시간 (Seconds)
        /// </summary>
        /// <param name="tomorrowTime"></param>
        /// <returns></returns>
        public static int GetTodayRemainSec(int tomorrowTime)
        {
            try
            {
                DateTime now = GetCurrentDateTime(true);
                DateTime tomorrow = GetDateTime(tomorrowTime, true);
                while (DateTime.Compare(now, tomorrow) > 0)
                    tomorrow.AddDays(1);
                int nowSec = GetSec(now);
                int tomorrowSec = GetSec(tomorrow);
                int remainSec = tomorrowSec - nowSec;
                log.Debug("남은 시간 {0}", TimeUtil.GetTime(remainSec));
                return remainSec;
            }
            catch { }
            return -1;
        }
    }
}
