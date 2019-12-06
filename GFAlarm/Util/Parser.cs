using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GFAlarm.Util
{
    public class Parser
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public class Json
        {
            /// <summary>
            /// string => JObject
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            public static JObject ParseJObject(string text)
            {
                try
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        //log.Warn("failed to parse text - empty");
                        return new JObject();
                    }

                    text = text.Trim();
                    if (text.StartsWith("{") && text.EndsWith("}"))
                    {
                        return JObject.Parse(text);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to parse text - text={0}", text);
                }
                return new JObject();
            }

            /// <summary>
            /// string => JArray
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            public static JArray ParseJArray(string text)
            {
                try
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        //log.Warn("failed to parse text - empty");
                        return new JArray();
                    }

                    text = text.Trim();
                    if (text.StartsWith("[") && text.EndsWith("]"))
                    {
                        return JArray.Parse(text);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to parse text - text={0}", text);
                }
                return new JArray();
            }

            /// <summary>
            /// string => Dictionary<string, string>
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            public static Dictionary<string, string> ParseItems(string text)
            {
                JObject obj = ParseJObject(text);
                return ParseItems(obj);
            }
            public static Dictionary<string, string> ParseItems(JObject obj)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                if (obj != null)
                {
                    List<string> keys = obj.Properties().Select(p => p.Name).ToList();
                    foreach (string key in keys)
                    {
                        try
                        {
                            result.Add(key, ParseString(obj[key]));
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                }
                else
                {
                    log.Warn("failed to parse dictionary - invalid json format");
                }
                return result;
            }

            /// <summary>
            /// JToken => bool
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public static bool ParseBool(JToken token)
            {
                if (token != null)
                    return token.Value<bool>();
                return false;
            }

            /// <summary>
            /// JToken => int
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public static int ParseInt(JToken token)
            {
                int result = 0;
                if (token != null)
                {
                    string temp = ParseString(token);
                    int.TryParse(temp, out result);
                }
                return result;
            }
            public static int ParseInt(JToken token, string key1, string key2)
            {
                int result = 0;
                if (token != null && token[key1] != null && token[key1][key2] != null)
                {
                    string temp = ParseString(token[key1][key2]);
                    int.TryParse(temp, out result);
                }
                return result;
            }

            /// <summary>
            /// JToken => short
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public static short ParseShort(JToken token)
            {
                short result = 0;
                if (token != null)
                {
                    string temp = ParseString(token);
                    short.TryParse(temp, out result);
                }
                return result;
            }

            /// <summary>
            /// JToken => double
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public static double ParseDouble(JToken token)
            {
                double result = 0;
                if (token != null)
                {
                    string temp = ParseString(token);
                    double.TryParse(temp, out result);
                }
                return result;
            }

            /// <summary>
            /// JToken => long
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public static long ParseLong(JToken token)
            {
                long result = 0;
                if (token != null)
                {
                    string temp = ParseString(token);
                    long.TryParse(temp, out result);
                }
                return result;
            }
            public static long ParseLong(JToken token, string key1)
            {
                long result = 0;
                if (token != null && token[key1] != null)
                {
                    string temp = ParseString(token[key1]);
                    long.TryParse(temp, out result);
                }
                return result;
            }
            public static long ParseLong(JToken token, string key1, string key2)
            {
                long result = 0;
                if (token != null && token[key1] != null && token[key1][key2] != null)
                {
                    string temp = ParseString(token[key1][key2]);
                    long.TryParse(temp, out result);
                }
                return result;
            }

            /// <summary>
            /// JToken => string[]
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public static string[] ParseStringArray(JToken token)
            {
                if (token != null)
                    return token.Select(j => j.ToString()).ToArray();
                return new string[] { };
            }
            
            /// <summary>
            /// JToken => int[]
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public static int[] ParseIntArray(JToken token)
            {
                if (token != null)
                    return token.Select(j => Convert.ToInt32(j)).ToArray();
                return new int[] { };
            }

            /// <summary>
            /// JToken => long
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public static long[] ParseLongArray(JToken token)
            {
                if (token != null)
                    return token.Select(j => (long)j).ToArray();
                return new long[] { };
            }

            /// <summary>
            /// JToken => string
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public static string ParseString(JToken token)
            {
                if (token != null)
                    return token.ToString();
                return "";
            }
            public static string ParseString(JToken token, string key1)
            {
                if (token != null && token[key1] != null)
                    return token[key1].ToString();
                return "";
            }
        }

        public class String
        {
            /// <summary>
            /// string => bool
            /// </summary>
            /// <param name="text"></param>
            /// <param name="defaultValue"></param>
            /// <returns></returns>
            public static bool ParseBool(string text, bool defaultValue = false)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    if ("true".Equals(text.ToLower()) || "1".Equals(text))
                        return true;
                    else if ("false".Equals(text.ToLower()) || "0".Equals(text))
                        return false;
                }
                return defaultValue;
            }

            /// <summary>
            /// string => int[]
            /// </summary>
            /// <param name="value"></param>
            /// <param name="defaultLength"></param>
            /// <returns></returns>
            public static int[] ParseIntArray(string value, int defaultLength = 2)
            {
                List<int> result = new List<int>();
                try
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        string[] items = value.Split(',');
                        foreach (string item in items)
                        {
                            result.Add(ParseInt(item));
                        }
                        return result.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                // 기본값
                result = new List<int>();
                for (int i = 0; i < defaultLength; i++)
                {
                    result.Add(i);
                }
                return result.ToArray();
            }

            /// <summary>
            /// string => bool[]
            /// </summary>
            /// <param name="value"></param>
            /// <param name="defaultValue"></param>
            /// <param name="defaultLength"></param>
            /// <returns></returns>
            public static bool[] ParseBoolArray(string value, bool defaultValue = false, int defaultLength = 2)
            {
                List<bool> result = new List<bool>();
                try
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        string[] items = value.Split(',');
                        foreach (string item in items)
                        {
                            result.Add(ParseBool(item));
                        }
                        return result.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                // 기본값
                result = new List<bool>();
                for (int i = 0; i < defaultLength; i++)
                {
                    result.Add(defaultValue);
                }
                return result.ToArray();
            }

            /// <summary>
            /// string => int
            /// </summary>
            /// <param name="value"></param>
            /// <param name="defaultValue"></param>
            /// <returns></returns>
            public static int ParseInt(string value, int defaultValue = 0)
            {
                int result = defaultValue;
                if (!string.IsNullOrEmpty(value))
                    int.TryParse(value, out result);
                return result;
            }

            /// <summary>
            /// string => short
            /// </summary>
            /// <param name="value"></param>
            /// <param name="defaultValue"></param>
            /// <returns></returns>
            public static short ParseShort(string value, short defaultValue = 0)
            {
                short result = defaultValue;
                if (!string.IsNullOrEmpty(value))
                    short.TryParse(value, out result);
                return result;
            }

            /// <summary>
            /// string => double
            /// </summary>
            /// <param name="value"></param>
            /// <param name="defaultValue"></param>
            /// <returns></returns>
            public static double ParseDouble(string value, double defaultValue = 0)
            {
                double result = defaultValue;
                if (!string.IsNullOrEmpty(value))
                    double.TryParse(value, out result);
                return result;
            }

            /// <summary>
            /// string => long
            /// </summary>
            /// <param name="value"></param>
            /// <param name="defaultValue"></param>
            /// <returns></returns>
            public static long ParseLong(string value, long defaultValue = 0)
            {
                long result = defaultValue;
                if (!string.IsNullOrEmpty(value))
                    long.TryParse(value, out result);
                return result;
            }

            /// <summary>
            /// HH:MM => milliseconds
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ParseHHMM(string value)
            {
                Regex regex = new Regex(@"^[0-9][0-9]:[0-5][0-9]$");
                if (regex.IsMatch(value))
                {
                    long time = 0;
                    time += long.Parse(value.Substring(0, 2)) * 60 * 60 * 1000;
                    time += long.Parse(value.Substring(3, 2)) * 60 * 1000;
                    return time;
                }
                return 0;
            }
        }

        public class Long
        {
            /// <summary>
            /// milliseconds => HH:MM:SS
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static string ParseMS(long value)
            {
                string result = "00:00:00";
                if (value > 0)
                {
                    value /= 1000;
                    int hour = Convert.ToInt32(value / 3600);
                    value = value % 3600;
                    int min = Convert.ToInt32(value / 60);
                    value = value % 60;
                    int sec = Convert.ToInt32(value);
                    result = string.Format("{0}:{1}:{2}", hour.ToString().PadLeft(2, '0'), min.ToString().PadLeft(2, '0'), sec.ToString().PadLeft(2, '0'));
                }
                return result;
            }
        }

        public class Time
        {
            public const long MINUTE = 60 * 1000;
            public const long HOUR = 60 * MINUTE;
            public const long DAY = 24 * HOUR;
            public const long WEEK = 7 * DAY;

            /// <summary>
            /// milliseconds => DateTime
            /// </summary>
            /// <param name="ms"></param>
            /// <param name="toUtc"></param>
            /// <returns></returns>
            public static DateTime GetDateTime(long ms, bool toUtc = false)
            {
                try
                {
                    TimeSpan ts = TimeSpan.FromMilliseconds(ms);
                    DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + ts;
                    if (toUtc)
                        return dt.ToUniversalTime();
                    else
                        return dt.ToLocalTime();
                }
                catch { }
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            /// <summary>
            /// DateTime => milliseconds
            /// </summary>
            /// <param name="dt"></param>
            /// <returns></returns>
            public static long GetMs(DateTime dt)
            {
                if (dt != null)
                {
                    long ms = (long)dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                    return ms;
                }
                log.Warn("null datetime");
                return 0;
            }

            /// <summary>
            /// 현재 시간 (Milliseconds)
            /// </summary>
            /// <returns></returns>
            public static long GetCurrentMs()
            {
                DateTime now = DateTime.Now;
                long ms = (long)now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                return ms;
            }

            /// <summary>
            /// 남은 시간 (Milliseconds)
            /// </summary>
            /// <param name="ms"></param>
            /// <returns></returns>
            public static long GetRemainMs(long endTime)
            {
                try
                {
                    long remainTime = endTime - GetCurrentMs();
                    return remainTime;
                }
                catch { }
                return 0;
            }

            /// <summary>
            /// 현재 시간 (DateTime)
            /// </summary>
            /// <param name="toUtc"></param>
            /// <returns></returns>
            public static DateTime GetCurrentDateTime(bool toUtc = false)
            {
                DateTime now;
                if (toUtc)
                    now = DateTime.UtcNow;
                else
                    now = DateTime.Now;
                return now;
            }

            /// <summary>
            /// 남은 시간 (HH:MM:SS)
            /// </summary>
            /// <param name="end_time"></param>
            /// <returns></returns>
            public static string GetRemainHHMMSS(long end_time, long now_time=0)
            {
                try
                {
                    if (now_time == 0)
                        now_time = GetCurrentMs();
                    long remain_time = end_time - now_time;
                    //log.Debug("end_time {0}", end_time);
                    if (remain_time <= 0)
                        return "00:00:00";
                    int hh = Convert.ToInt32(remain_time / HOUR);
                    int mm = Convert.ToInt32(remain_time % HOUR / MINUTE);
                    int ss = Convert.ToInt32(remain_time % MINUTE / 1000);
                    return System.String.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", hh, mm, ss);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return "00:00:00";
            }

            /// <summary>
            /// 자정까지 남은 시간 (Milliseconds)
            /// </summary>
            /// <param name="tomorrow_time"></param>
            /// <returns></returns>
            public static long GetTodayRemainMs(long tomorrow_time)
            {
                try
                {
                    DateTime now = DateTime.UtcNow;
                    DateTime tomorrow = GetDateTime(tomorrow_time, true);
                    log.Debug("현재(UTC) {0} 비교(UTC) {1}", now.ToString("MM-dd HH:mm"), tomorrow.ToString("MM-dd HH:mm"));
                    while (DateTime.Compare(now, tomorrow) > 0)
                        tomorrow.AddDays(1);
                    long nowMs = GetMs(now);
                    long tomorrowMs = GetMs(tomorrow);
                    long ms = tomorrowMs - nowMs;
                    log.Debug("남은 시간 {0}", Parser.Long.ParseMS(ms));
                    return ms;
                }
                catch { }
                return 0;
            }
        }
    }
}
