using GFAlarm.Data.Element;
using GFAlarm.Util;
using LocalizationResources;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GFAlarm.Data
{
    public class GameData
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static JObject operation = new JObject();

        /// <summary>
        /// 지도 정보
        /// </summary>
        /*
        public class Map
        {
            /// <summary>
            /// 아군 정보
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static JObject GetAllyData(int id)
            {
                string sql = string.Format("SELECT * FROM gfdb_ally_team WHERE id = '{0}' LIMIT 1", id);
                return GetDb(sql, "GFData");
            }

            /// <summary>
            /// 적 정보
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static JObject GetEnemyData(int id)
            {
                string sql = string.Format("SELECT * FROM gfdb_enemy_team WHERE id = '{0}' LIMIT 1", id);
                return GetDb(sql, "GFData");
            }

            /// <summary>
            /// 건물 정보
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static JObject GetBuildingData(int id)
            {
                string sql = string.Format("SELECT * FROM gfdb_building WHERE id = '{0}' LIMIT 1", id);
                return GetDb(sql, "GFData");
            }

            /// <summary>
            /// 거점 정보
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static JObject GetSpotData(int id)
            {
                string sql = string.Format("SELECT * FROM gfdb_spot WHERE id = '{0}' LIMIT 1", id);
                return GetDb(sql, "GFData");
            }

            /// <summary>
            /// 거점 정보
            /// </summary>
            /// <param name="mission_id"></param>
            /// <returns></returns>
            public static List<JObject> GetSpotDataList(int mission_id)
            {
                string sql = string.Format("SELECT * FROM gfdb_spot WHERE mission_id = '{0}'", mission_id);
                return GetDbs(sql, "GFData");
            }

            /// <summary>
            /// 전역 정보
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static JObject GetMissionData(int id)
            {
                string sql = string.Format("SELECT * FROM gfdb_mission WHERE id = '{0}' LIMIT 1", id);
                return GetDb(sql, "GFData");
            }
        }
        */

        /// <summary>
        /// Logistics 군수지원
        /// </summary>
        public class Logistics
        {
            /// <summary>
            /// 군수지원 정보 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <param name="col"></param>
            /// <returns></returns>
            public static JObject GetData(int id) 
            {
                //string sql = string.Format("SELECT * FROM operation WHERE id = {0} LIMIT 1", id);
                //return GetDb(sql, "GFData");

                JObject json = GetJsonDb("operation");
                string key = id.ToString();
                if (json.ContainsKey(key))
                {
                    return json[key].Value<JObject>();
                }
                return new JObject();
            }
        }

        /// <summary>
        /// Mission 전역
        /// </summary>
        public class Mission
        {
            /// <summary>
            /// 전역 정보 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <param name="col"></param>
            /// <returns></returns>
            public static JObject GetData(int id)
            {
                //string sql = string.Format("SELECT * FROM mission WHERE id = {0} LIMIT 1", id);
                //return GetDb(sql, "GFData");

                JObject json = GetJsonDb("mission");
                string key = id.ToString();
                if (json.ContainsKey(key))
                {
                    return json[key].Value<JObject>();
                }
                return new JObject();
            }

            /// <summary>
            /// 자율작전 소요시간 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static int GetRequireTime(int id)
            {
                //string sql = string.Format("SELECT require_time FROM mission WHERE id = {0} LIMIT 1", id);
                //string value = GetDb(sql, "require_time", "GFData");
                //if (string.IsNullOrEmpty(value))
                //    value = "00:00";
                //return TimeUtil.ParseHHMM(value);

                JObject json = GetData(id);
                string hhmm = "00:00";
                if (json.ContainsKey("require_time"))
                {
                    hhmm = json["require_time"].Value<string>();
                }
                return TimeUtil.ParseHHMM(hhmm);
            }

            /// <summary>
            /// 모의전에서 사용한 모의작전점수
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static int GetUsedAp(int id)
            {
                switch (id)
                {
                    // 초급
                    case 1101: // 경험특훈
                    case 1201: // 강화연습
                    case 1301: // 자료추출
                    case 1501: // 마인드회랑
                        return 1;
                    // 중급
                    case 1102: // 경험특훈
                    case 1202: // 강화연습
                    case 1302: // 자료추출
                    case 1502: // 마인드회랑
                        return 2;
                    // 고급
                    case 1103: // 경험특훈
                    case 1203: // 강화연습
                    case 1303: // 자료추출
                    case 1503: // 마인드회랑
                        return 3;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// 적 정보
        /// </summary>
        public static class Enemy
        {
            /// <summary>
            /// 적 제대 멤버 코드 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static string[] GetEnemyCodes(long id)
            {
                //string[] result = null;
                //string sql = string.Format("SELECT members FROM gfdb_enemy_team WHERE id = {0} LIMIT 1", id);
                //string value = GetDb(sql, "members", "GFData");
                /*
                    id, members
                    73, Scouts,Prowler,Prowler,Prowler,Scouts,Vespid,Vespid,Vespid
                */
                //if (string.IsNullOrEmpty(value))
                //    return null;
                //if (value.Contains(","))
                //    result = value.Split(',');
                //else
                //    result = new string[] { value };
                //return result;

                JObject json = GetJsonDb("gfdb_enemy_team");
                string key = id.ToString();
                if (json.ContainsKey(key))
                {
                    try
                    {
                        string[] result = null;
                        string members = json[key]["members"].Value<string>();
                        /*
                            id, members
                            73, Scouts,Prowler,Prowler,Prowler,Scouts,Vespid,Vespid,Vespid
                        */
                        if (string.IsNullOrEmpty(members))
                            return null;
                        if (members.Contains(","))
                            result = members.Split(',');
                        else
                            result = new string[] { members };
                        return result;
                    }
                    catch { }
                }
                return new string[] { };
            }
        }

        /// <summary>
        /// 칩셋
        /// </summary>
        public class Chip
        {
            /// <summary>
            /// 칩셋 스탯
            /// [모양]-[별]-[종류]-[포인트]
            /// [모양] 1, 2, 3, 4, 5A, 5B, 6
            /// [별] 1, 2, 3, 4, 5
            /// [종류] 1: 살상, 2: 파쇄, 3: 정밀, 4: 장전
            /// </summary>
            private static Dictionary<string, int> _chipStat = new Dictionary<string, int>()
            {
                #region 칩셋 스탯
                { "1-2-1-1", 2 },
                { "1-3-1-1", 2 },
                { "1-4-1-1", 3 },
                { "1-5-1-1", 4 },
                { "1-2-2-1", 4 },
                { "1-3-2-1", 6 },
                { "1-4-2-1", 8 },
                { "1-5-2-1", 10 },
                { "1-2-3-1", 2 },
                { "1-3-3-1", 4 },
                { "1-4-3-1", 4 },
                { "1-5-3-1", 6 },
                { "1-2-4-1", 2 },
                { "1-3-4-1", 3 },
                { "1-4-4-1", 4 },
                { "1-5-4-1", 5 },
                { "2-2-1-1", 2 },
                { "2-2-1-2", 3 },
                { "2-3-1-1", 2 },
                { "2-3-1-2", 4 },
                { "2-4-1-1", 3 },
                { "2-4-1-2", 5 },
                { "2-5-1-1", 4 },
                { "2-5-1-2", 7 },
                { "2-2-2-1", 4 },
                { "2-2-2-2", 8 },
                { "2-3-2-1", 6 },
                { "2-3-2-2", 12 },
                { "2-4-2-1", 8 },
                { "2-4-2-2", 15 },
                { "2-5-2-1", 10 },
                { "2-5-2-2", 19 },
                { "2-2-3-1", 2 },
                { "2-2-3-2", 4 },
                { "2-3-3-1", 4 },
                { "2-3-3-2", 7 },
                { "2-4-3-1", 4 },
                { "2-4-3-2", 8 },
                { "2-5-3-1", 6 },
                { "2-5-3-2", 11 },
                { "2-2-4-1", 2 },
                { "2-2-4-2", 4 },
                { "2-3-4-1", 3 },
                { "2-3-4-2", 6 },
                { "2-4-4-1", 4 },
                { "2-4-4-2", 7 },
                { "2-5-4-1", 5 },
                { "2-5-4-2", 9 },
                { "3-2-1-1", 2 },
                { "3-2-1-2", 3 },
                { "3-2-1-3", 5 },
                { "3-3-1-1", 3 },
                { "3-3-1-2", 5 },
                { "3-3-1-3", 7 },
                { "3-4-1-1", 3 },
                { "3-4-1-2", 6 },
                { "3-4-1-3", 8 },
                { "3-5-1-1", 4 },
                { "3-5-1-2", 7 },
                { "3-5-1-3", 11 },
                { "3-2-2-1", 5 },
                { "3-2-2-2", 9 },
                { "3-2-2-3", 13 },
                { "3-3-2-1", 7 },
                { "3-3-2-2", 13 },
                { "3-3-2-3", 19 },
                { "3-4-2-1", 8 },
                { "3-4-2-2", 16 },
                { "3-4-2-3", 23 },
                { "3-5-2-1", 10 },
                { "3-5-2-2", 20 },
                { "3-5-2-3", 29 },
                { "3-2-3-1", 3 },
                { "3-2-3-2", 5 },
                { "3-2-3-3", 7 },
                { "3-3-3-1", 4 },
                { "3-3-3-2", 7 },
                { "3-3-3-3", 11 },
                { "3-4-3-1", 5 },
                { "3-4-3-2", 9 },
                { "3-4-3-3", 13 },
                { "3-5-3-1", 6 },
                { "3-5-3-2", 11 },
                { "3-5-3-3", 17 },
                { "3-2-4-1", 2 },
                { "3-2-4-2", 4 },
                { "3-2-4-3", 6 },
                { "3-3-4-1", 3 },
                { "3-3-4-2", 6 },
                { "3-3-4-3", 9 },
                { "3-4-4-1", 4 },
                { "3-4-4-2", 7 },
                { "3-4-4-3", 11 },
                { "3-5-4-1", 5 },
                { "3-5-4-2", 9 },
                { "3-5-4-3", 13 },
                { "4-2-1-1", 2 },
                { "4-2-1-2", 3 },
                { "4-2-1-3", 5 },
                { "4-2-1-4", 6 },
                { "4-3-1-1", 3 },
                { "4-3-1-2", 5 },
                { "4-3-1-3", 7 },
                { "4-3-1-4", 9 },
                { "4-4-1-1", 3 },
                { "4-4-1-2", 6 },
                { "4-4-1-3", 9 },
                { "4-4-1-4", 12 },
                { "4-5-1-1", 4 },
                { "4-5-1-2", 8 },
                { "4-5-1-3", 11 },
                { "4-5-1-4", 15 },
                { "4-2-2-1", 5 },
                { "4-2-2-2", 9 },
                { "4-2-2-3", 13 },
                { "4-2-2-4", 17 },
                { "4-3-2-1", 7 },
                { "4-3-2-2", 13 },
                { "4-3-2-3", 19 },
                { "4-3-2-4", 25 },
                { "4-4-2-1", 9 },
                { "4-4-2-2", 17 },
                { "4-4-2-3", 25 },
                { "4-4-2-4", 33 },
                { "4-5-2-1", 11 },
                { "4-5-2-2", 21 },
                { "4-5-2-3", 31 },
                { "4-5-2-4", 41 },
                { "4-2-3-1", 3 },
                { "4-2-3-2", 5 },
                { "4-2-3-3", 7 },
                { "4-2-3-4", 10 },
                { "4-3-3-1", 4 },
                { "4-3-3-2", 7 },
                { "4-3-3-3", 11 },
                { "4-3-3-4", 14 },
                { "4-4-3-1", 5 },
                { "4-4-3-2", 10 },
                { "4-4-3-3", 14 },
                { "4-4-3-4", 19 },
                { "4-5-3-1", 6 },
                { "4-5-3-2", 12 },
                { "4-5-3-3", 18 },
                { "4-5-3-4", 23 },
                { "4-2-4-1", 2 },
                { "4-2-4-2", 4 },
                { "4-2-4-3", 6 },
                { "4-2-4-4", 8 },
                { "4-3-4-1", 3 },
                { "4-3-4-2", 6 },
                { "4-3-4-3", 9 },
                { "4-3-4-4", 11 },
                { "4-4-4-1", 4 },
                { "4-4-4-2", 8 },
                { "4-4-4-3", 11 },
                { "4-4-4-4", 15 },
                { "4-5-4-1", 5 },
                { "4-5-4-2", 10 },
                { "4-5-4-3", 14 },
                { "4-5-4-4", 19 },
                { "5A-2-1-1", 2 },
                { "5A-2-1-2", 4 },
                { "5A-2-1-3", 5 },
                { "5A-2-1-4", 7 },
                { "5A-3-1-1", 3 },
                { "5A-3-1-2", 5 },
                { "5A-3-1-3", 8 },
                { "5A-3-1-4", 10 },
                { "5A-4-1-1", 4 },
                { "5A-4-1-2", 7 },
                { "5A-4-1-3", 10 },
                { "5A-4-1-4", 13 },
                { "5A-5-1-1", 5 },
                { "5A-5-1-2", 9 },
                { "5A-5-1-3", 13 },
                { "5A-5-1-4", 17 },
                { "5A-2-2-1", 5 },
                { "5A-2-2-2", 10 },
                { "5A-2-2-3", 14 },
                { "5A-2-2-4", 19 },
                { "5A-3-2-1", 8 },
                { "5A-3-2-2", 15 },
                { "5A-3-2-3", 22 },
                { "5A-3-2-4", 29 },
                { "5A-4-2-1", 10 },
                { "5A-4-2-2", 19 },
                { "5A-4-2-3", 28 },
                { "5A-4-2-4", 37 },
                { "5A-5-2-1", 12 },
                { "5A-5-2-2", 24 },
                { "5A-5-2-3", 36 },
                { "5A-5-2-4", 47 },
                { "5A-2-3-1", 3 },
                { "5A-2-3-2", 6 },
                { "5A-2-3-3", 8 },
                { "5A-2-3-4", 11 },
                { "5A-3-3-1", 4 },
                { "5A-3-3-2", 8 },
                { "5A-3-3-3", 12 },
                { "5A-3-3-4", 16 },
                { "5A-4-3-1", 6 },
                { "5A-4-3-2", 11 },
                { "5A-4-3-3", 16 },
                { "5A-4-3-4", 21 },
                { "5A-5-3-1", 7 },
                { "5A-5-3-2", 14 },
                { "5A-5-3-3", 20 },
                { "5A-5-3-4", 27 },
                { "5A-2-4-1", 3 },
                { "5A-2-4-2", 5 },
                { "5A-2-4-3", 7 },
                { "5A-2-4-4", 9 },
                { "5A-3-4-1", 4 },
                { "5A-3-4-2", 7 },
                { "5A-3-4-3", 10 },
                { "5A-3-4-4", 13 },
                { "5A-4-4-1", 5 },
                { "5A-4-4-2", 9 },
                { "5A-4-4-3", 13 },
                { "5A-4-4-4", 17 },
                { "5A-5-4-1", 6 },
                { "5A-5-4-2", 11 },
                { "5A-5-4-3", 16 },
                { "5A-5-4-4", 21 },
                { "5B-2-1-1", 2 },
                { "5B-2-1-2", 4 },
                { "5B-2-1-3", 6 },
                { "5B-2-1-4", 8 },
                { "5B-3-1-1", 3 },
                { "5B-3-1-2", 6 },
                { "5B-3-1-3", 8 },
                { "5B-3-1-4", 11 },
                { "5B-4-1-1", 4 },
                { "5B-4-1-2", 8 },
                { "5B-4-1-3", 11 },
                { "5B-4-1-4", 15 },
                { "5B-5-1-1", 5 },
                { "5B-5-1-2", 9 },
                { "5B-5-1-3", 14 },
                { "5B-5-1-4", 18 },
                { "5B-2-2-1", 6 },
                { "5B-2-2-2", 11 },
                { "5B-2-2-3", 16 },
                { "5B-2-2-4", 21 },
                { "5B-3-2-1", 8 },
                { "5B-3-2-2", 16 },
                { "5B-3-2-3", 23 },
                { "5B-3-2-4", 31 },
                { "5B-4-2-1", 11 },
                { "5B-4-2-2", 21 },
                { "5B-4-2-3", 31 },
                { "5B-4-2-4", 41 },
                { "5B-5-2-1", 13 },
                { "5B-5-2-2", 26 },
                { "5B-5-2-3", 39 },
                { "5B-5-2-4", 51 },
                { "5B-2-3-1", 3 },
                { "5B-2-3-2", 6 },
                { "5B-2-3-3", 9 },
                { "5B-2-3-4", 12 },
                { "5B-3-3-1", 5 },
                { "5B-3-3-2", 9 },
                { "5B-3-3-3", 13 },
                { "5B-3-3-4", 18 },
                { "5B-4-3-1", 6 },
                { "5B-4-3-2", 12 },
                { "5B-4-3-3", 18 },
                { "5B-4-3-4", 23 },
                { "5B-5-3-1", 8 },
                { "5B-5-3-2", 15 },
                { "5B-5-3-3", 22 },
                { "5B-5-3-4", 29 },
                { "5B-2-4-1", 3 },
                { "5B-2-4-2", 5 },
                { "5B-2-4-3", 7 },
                { "5B-2-4-4", 10 },
                { "5B-3-4-1", 4 },
                { "5B-3-4-2", 7 },
                { "5B-3-4-3", 11 },
                { "5B-3-4-4", 14 },
                { "5B-4-4-1", 5 },
                { "5B-4-4-2", 10 },
                { "5B-4-4-3", 14 },
                { "5B-4-4-4", 19 },
                { "5B-5-4-1", 6 },
                { "5B-5-4-2", 12 },
                { "5B-5-4-3", 18 },
                { "5B-5-4-4", 23 },
                { "6-2-1-1", 2 },
                { "6-2-1-2", 4 },
                { "6-2-1-3", 6 },
                { "6-2-1-4", 8 },
                { "6-2-1-5", 9 },
                { "6-3-1-1", 3 },
                { "6-3-1-2", 6 },
                { "6-3-1-3", 8 },
                { "6-3-1-4", 11 },
                { "6-3-1-5", 14 },
                { "6-4-1-1", 4 },
                { "6-4-1-2", 8 },
                { "6-4-1-3", 11 },
                { "6-4-1-4", 15 },
                { "6-4-1-5", 18 },
                { "6-5-1-1", 5 },
                { "6-5-1-2", 9 },
                { "6-5-1-3", 14 },
                { "6-5-1-4", 18 },
                { "6-5-1-5", 22 },
                { "6-2-2-1", 6 },
                { "6-2-2-2", 11 },
                { "6-2-2-3", 16 },
                { "6-2-2-4", 21 },
                { "6-2-2-5", 26 },
                { "6-3-2-1", 8 },
                { "6-3-2-2", 16 },
                { "6-3-2-3", 23 },
                { "6-3-2-4", 31 },
                { "6-3-2-5", 39 },
                { "6-4-2-1", 11 },
                { "6-4-2-2", 21 },
                { "6-4-2-3", 31 },
                { "6-4-2-4", 41 },
                { "6-4-2-5", 51 },
                { "6-5-2-1", 13 },
                { "6-5-2-2", 26 },
                { "6-5-2-3", 39 },
                { "6-5-2-4", 51 },
                { "6-5-2-5", 64 },
                { "6-2-3-1", 3 },
                { "6-2-3-2", 6 },
                { "6-2-3-3", 9 },
                { "6-2-3-4", 12 },
                { "6-2-3-5", 15 },
                { "6-3-3-1", 5 },
                { "6-3-3-2", 9 },
                { "6-3-3-3", 13 },
                { "6-3-3-4", 18 },
                { "6-3-3-5", 22 },
                { "6-4-3-1", 6 },
                { "6-4-3-2", 12 },
                { "6-4-3-3", 18 },
                { "6-4-3-4", 23 },
                { "6-4-3-5", 29 },
                { "6-5-3-1", 8 },
                { "6-5-3-2", 15 },
                { "6-5-3-3", 22 },
                { "6-5-3-4", 29 },
                { "6-5-3-5", 36 },
                { "6-2-4-1", 3 },
                { "6-2-4-2", 5 },
                { "6-2-4-3", 7 },
                { "6-2-4-4", 10 },
                { "6-2-4-5", 12 },
                { "6-3-4-1", 4 },
                { "6-3-4-2", 7 },
                { "6-3-4-3", 11 },
                { "6-3-4-4", 14 },
                { "6-3-4-5", 18 },
                { "6-4-4-1", 5 },
                { "6-4-4-2", 10 },
                { "6-4-4-3", 14 },
                { "6-4-4-4", 19 },
                { "6-4-4-5", 23 },
                { "6-5-4-1", 6 },
                { "6-5-4-2", 12 },
                { "6-5-4-3", 18 },
                { "6-5-4-4", 23 },
                { "6-5-4-5", 29 }
                #endregion
            };

            /// <summary>
            /// 칩셋 스탯 가져오기
            /// </summary>
            /// <param name="gridId"></param>
            /// <param name="star"></param>
            /// <param name="type"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static int GetStat(int gridId, int star, int type, int point)
            {
                string key = string.Format("{0}-{1}-{2}-{3}", GetBlockType(gridId), star, type, point);
                if (_chipStat.ContainsKey(key))
                    return _chipStat[key];
                return 0;
            }

            /// <summary>
            /// 칩셋 모양 이름 가져오기
            /// </summary>
            /// <param name="gridId"></param>
            /// <returns></returns>
            public static string GetBlockType(int gridId)
            {
                switch (gridId)
                {
                    case 1:
                        return "1";
                    case 2:
                        return "2";
                    case 3:
                    case 4:
                        return "3";
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                        return "4";
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                        return "5A";
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                        return "5B";
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                        return "6";
                    default:
                        return "0";
                }
            }

            /// <summary>
            /// 핵심데이터 이름 가져오기
            /// </summary>
            /// <param name="pieceId"></param>
            /// <returns></returns>
            public static string GetSquadPieceName(int pieceId)
            {
                //string sql = string.Format("SELECT name FROM squad WHERE piece_id = {0} LIMIT 1", pieceId);
                //return GetDb(sql, "name", "GFData");

                int id = pieceId % 300;
                JObject json = GetJsonDb("squad");
                string key = id.ToString();
                if (json.ContainsKey(key))
                {
                    return json[key]["name"].Value<string>();
                }
                return "";
            }
        }

        // 시설
        public class Facility
        {
            // 정보 센터
            public class IntelCenter
            {
                /// <summary>
                /// 정보분석 소요시간
                /// </summary>
                public static Dictionary<int, string> _dataAnalysisTime = new Dictionary<int, string>()
                {
                    { 0, "08:00" }, 
                    { 1, "07:36" },
                    { 2, "07:12" },
                    { 3, "06:48" },
                    { 4, "06:24" },
                    { 5, "06:00" },
                    { 6, "05:36" },
                    { 7, "05:12" },
                    { 8, "04:48" },
                    { 9, "04:24" },
                    { 10, "02:00" },
                };


                /// <summary>
                /// 샘플 분석시간 가져오기
                /// </summary>
                /// <param name="lv"></param>
                /// <returns></returns>
                public static int GetDataAnalysisTime(int lv)
                {
                    if (_dataAnalysisTime.ContainsKey(lv))
                        return TimeUtil.ParseHHMM(_dataAnalysisTime[lv]);
                    return 0;
                }
            }

            // 자료실
            public class DataRoom
            {
                /// <summary>
                /// 자유경험치 상한
                /// </summary>
                public static Dictionary<int, int> _maxSurplusExp = new Dictionary<int, int>()
                {
                    { 0, 15000 },
                    { 1, 30000 },
                    { 2, 45000 },
                    { 3, 60000 },
                    { 4, 75000 },
                    { 5, 90000 },
                    { 6, 120000 },
                    { 7, 150000 },
                    { 8, 180000 },
                    { 9, 210000 },
                    { 10, 240000 },
                };

                /// <summary>
                /// 작전보고서 작성시간
                /// </summary>
                public static Dictionary<int, string> _combatReportTime = new Dictionary<int, string>()
                {
                    { 0, "11:00" },
                    { 1, "10:00" },
                    { 2, "09:00" },
                    { 3, "08:00" },
                    { 4, "07:00" },
                    { 5, "06:00" },
                    { 6, "05:00" },
                    { 7, "04:00" },
                    { 8, "03:00" },
                    { 9, "02:00" },
                    { 10, "01:00" },
                };

                /// <summary>
                /// 자유경험치 상한 가져오기
                /// </summary>
                /// <param name="dataTableLevel"></param>
                /// <returns></returns>
                public static int GetMaxSurplusExp(int lv)
                {
                    if (_maxSurplusExp.ContainsKey(lv))
                        return _maxSurplusExp[lv];
                    return 0;
                }

                /// <summary>
                /// 작전보고서 작성시간 가져오기
                /// </summary>
                /// <param name="lv"></param>
                /// <returns></returns>
                public static int GetCombatReportTime(int lv)
                {
                    if (_combatReportTime.ContainsKey(lv))
                        return TimeUtil.ParseHHMM(_combatReportTime[lv]);
                        //return Parser.String.ParseHHMM(requireTime[lv]);
                    return 0;
                }
            }
        }

        /// <summary>
        /// 요정
        /// </summary>
        public class Fairy
        {
            /// <summary>
            /// 요정 정보 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static JObject GetData(int id)
            {
                //string sql = string.Format("SELECT * FROM fairy WHERE id = {0} LIMIT 1", id);
                //return GetDb(sql, "GFData");

                JObject json = GetJsonDb("fairy");
                string key = id.ToString();
                if (json.ContainsKey(key))
                {
                    return json[key].Value<JObject>();
                }
                return new JObject();
            }

            /// <summary>
            /// 요정 이름 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static string GetName(int id)
            {
                return LanguageResources.Instance[string.Format("FAIRY_{0}", id)];
            }

            /// <summary>
            /// 요정 특성 가져오기
            /// </summary>
            /// <param name="traitId"></param>
            /// <returns></returns>
            public static JObject GetTraitData(int traitId)
            {
                //string sql = string.Format("SELECT * FROM fairy_trait WHERE id = {0} LIMIT 1", traitId);
                //return GetDb(sql, "GFData");

                JObject json = GetJsonDb("fairy_trait");
                string key = traitId.ToString();
                if (json.ContainsKey(key))
                {
                    return json[key].Value<JObject>();
                }
                return new JObject();
            }

            /// <summary>
            /// 요정 특성 이름 가져오기
            /// </summary>
            /// <param name="traitId"></param>
            /// <returns></returns>
            public static string GetTraitName(int traitId)
            {
                return LanguageResources.Instance[string.Format("TRAIT_{0}", traitId)];
            }

            /// <summary>
            /// 요정 스킬훈련시간 가져오기
            /// </summary>
            /// <param name="toLv"></param>
            /// <returns></returns>
            public static int GetFairySkillTrainTime(int toLv)
            {
                int requireTime = 0;
                if (_skillTrainTime.ContainsKey(toLv))
                {
                    requireTime = _skillTrainTime[toLv];
                    // 지휘관 보너스 사용 시
                    if (Config.Costume.coBonusSkillTrainTime)
                    {
                        double tempRequireTime = (double)requireTime;
                        requireTime = (int)(tempRequireTime - tempRequireTime * (Convert.ToDouble(Config.Costume.coBonusSkillTrainTimePercent) / 100));
                    }
                }
                return requireTime;
            }
            private static Dictionary<int, int> _skillTrainTime = new Dictionary<int, int>()
            {
                { 2, TimeUtil.HOUR * 2 },
                { 3, TimeUtil.HOUR * 6 },
                { 4, TimeUtil.HOUR * 10 },
                { 5, TimeUtil.HOUR * 14 },
                { 6, TimeUtil.HOUR * 20 },
                { 7, TimeUtil.HOUR * 26 },
                { 8, TimeUtil.HOUR * 32 },
                { 9, TimeUtil.HOUR * 40 },
                { 10, TimeUtil.HOUR * 48 },
            };

            // 경험치 관련
            public class Exp
            {
                // 필요 경험치
                public static Dictionary<int, int> requireExp = new Dictionary<int, int>()
                {
                    { 1, 0 },
                    { 2, 300 },
                    { 3, 600 },
                    { 4, 900 },
                    { 5, 1200 },
                    { 6, 1500 },
                    { 7, 1800 },
                    { 8, 2100 },
                    { 9, 2400 },
                    { 10, 2700 },
                    { 11, 3000 },
                    { 12, 3300 },
                    { 13, 3600 },
                    { 14, 3900 },
                    { 15, 4200 },
                    { 16, 4500 },
                    { 17, 4800 },
                    { 18, 5100 },
                    { 19, 5500 },
                    { 20, 6000 },
                    { 21, 6500 },
                    { 22, 7100 },
                    { 23, 8000 },
                    { 24, 9000 },
                    { 25, 10000 },
                    { 26, 11000 },
                    { 27, 12200 },
                    { 28, 13400 },
                    { 29, 14700 },
                    { 30, 16000 },
                    { 31, 17500 },
                    { 32, 18900 },
                    { 33, 20500 },
                    { 34, 22200 },
                    { 35, 23900 },
                    { 36, 25700 },
                    { 37, 27600 },
                    { 38, 29500 },
                    { 39, 31600 },
                    { 40, 33700 },
                    { 41, 35900 },
                    { 42, 38200 },
                    { 43, 40500 },
                    { 44, 43000 },
                    { 45, 45500 },
                    { 46, 48200 },
                    { 47, 50900 },
                    { 48, 53700 },
                    { 49, 56600 },
                    { 50, 59600 },
                    { 51, 62700 },
                    { 52, 65900 },
                    { 53, 69200 },
                    { 54, 72600 },
                    { 55, 76000 },
                    { 56, 79600 },
                    { 57, 83300 },
                    { 58, 87000 },
                    { 59, 90900 },
                    { 60, 94900 },
                    { 61, 99000 },
                    { 62, 103100 },
                    { 63, 107400 },
                    { 64, 111800 },
                    { 65, 116300 },
                    { 66, 120900 },
                    { 67, 125600 },
                    { 68, 130400 },
                    { 69, 135300 },
                    { 70, 140400 },
                    { 71, 145500 },
                    { 72, 150800 },
                    { 73, 156100 },
                    { 74, 161600 },
                    { 75, 167200 },
                    { 76, 172900 },
                    { 77, 178700 },
                    { 78, 184700 },
                    { 79, 190700 },
                    { 80, 196900 },
                    { 81, 203200 },
                    { 82, 209600 },
                    { 83, 216100 },
                    { 84, 222800 },
                    { 85, 229600 },
                    { 86, 236500 },
                    { 87, 243500 },
                    { 88, 250600 },
                    { 89, 257900 },
                    { 90, 265300 },
                    { 91, 272800 },
                    { 92, 280400 },
                    { 93, 288200 },
                    { 94, 296100 },
                    { 95, 304100 },
                    { 96, 312300 },
                    { 97, 320600 },
                    { 98, 329000 },
                    { 99, 337500 },
                    { 100, 357000 },
                };

                // 누적 경험치
                public static Dictionary<int, int> totalExp = new Dictionary<int, int>()
                {
                    { 1, 0 },
                    { 2, 300 },
                    { 3, 900 },
                    { 4, 1800 },
                    { 5, 3000 },
                    { 6, 4500 },
                    { 7, 6300 },
                    { 8, 8400 },
                    { 9, 10800 },
                    { 10, 13500 },
                    { 11, 16500 },
                    { 12, 19800 },
                    { 13, 23400 },
                    { 14, 27300 },
                    { 15, 31500 },
                    { 16, 36000 },
                    { 17, 40800 },
                    { 18, 45900 },
                    { 19, 51400 },
                    { 20, 57400 },
                    { 21, 63900 },
                    { 22, 71000 },
                    { 23, 79000 },
                    { 24, 88000 },
                    { 25, 98000 },
                    { 26, 109000 },
                    { 27, 121200 },
                    { 28, 134600 },
                    { 29, 149300 },
                    { 30, 165300 },
                    { 31, 182800 },
                    { 32, 201700 },
                    { 33, 222200 },
                    { 34, 244400 },
                    { 35, 268300 },
                    { 36, 294000 },
                    { 37, 321600 },
                    { 38, 351100 },
                    { 39, 382700 },
                    { 40, 416400 },
                    { 41, 452300 },
                    { 42, 490500 },
                    { 43, 531000 },
                    { 44, 574000 },
                    { 45, 619500 },
                    { 46, 667700 },
                    { 47, 718600 },
                    { 48, 772300 },
                    { 49, 828900 },
                    { 50, 888500 },
                    { 51, 951200 },
                    { 52, 1017100 },
                    { 53, 1086300 },
                    { 54, 1158900 },
                    { 55, 1234900 },
                    { 56, 1314500 },
                    { 57, 1397800 },
                    { 58, 1484800 },
                    { 59, 1575700 },
                    { 60, 1670600 },
                    { 61, 1769600 },
                    { 62, 1872700 },
                    { 63, 1980100 },
                    { 64, 2091900 },
                    { 65, 2208200 },
                    { 66, 2329100 },
                    { 67, 2454700 },
                    { 68, 2585100 },
                    { 69, 2720400 },
                    { 70, 2860800 },
                    { 71, 3006300 },
                    { 72, 3157100 },
                    { 73, 3313200 },
                    { 74, 3474800 },
                    { 75, 3642000 },
                    { 76, 3814900 },
                    { 77, 3993600 },
                    { 78, 4178300 },
                    { 79, 4369000 },
                    { 80, 4565900 },
                    { 81, 4769100 },
                    { 82, 4978700 },
                    { 83, 5194800 },
                    { 84, 5417600 },
                    { 85, 5647200 },
                    { 86, 5883700 },
                    { 87, 6127200 },
                    { 88, 6377800 },
                    { 89, 6635700 },
                    { 90, 6901000 },
                    { 91, 7173800 },
                    { 92, 7454200 },
                    { 93, 7742400 },
                    { 94, 8038500 },
                    { 95, 8342600 },
                    { 96, 8654900 },
                    { 97, 8975500 },
                    { 98, 9304500 },
                    { 99, 9642000 },
                    { 100, 9999000 },
                };

                // 요정 경험치 전송률
                public static double expRatio = 0.2;

                // 요정 경험치 전송률
                private static Dictionary<int, double> _expRatio = new Dictionary<int, double>()
                {
                    { 0, 0.05 },
                    { 1, 0.065 },
                    { 2, 0.08 },
                    { 3, 0.095 },
                    { 4, 0.11 },
                    { 5, 0.125 },
                    { 6, 0.14 },
                    { 7, 0.155 },
                    { 8, 0.17 },
                    { 9, 0.185 },
                    { 10, 0.2 },
                };
                // TODO: 요정의 방 시설 레벨에 따라서 요정 경험치 전송률 수정할 것

                /// <summary>
                /// 누적 경험치 가져오기
                /// </summary>
                /// <param name="lv"></param>
                /// <returns></returns>
                public static int GetTotalExp(int lv)
                {
                    if (totalExp.ContainsKey(lv))
                        return totalExp[lv];
                    return 0;
                }
            }
        }

        /// <summary>
        /// 국지전
        /// </summary>
        public class Theater
        {
            /// <summary>
            /// 국지전 웨이브 정보 가져오기
            /// </summary>
            /// <param name="area_id"></param>
            /// <param name="enemy_id"></param>
            /// <returns></returns>
            public static JObject GetData(int area_id, int enemy_id)
            {
                /*
                try
                {
                    string sql = string.Format("SELECT * FROM theater_enemy WHERE area_id = {0} AND id = {1}", 
                                                area_id, enemy_id);
                    return GetDb(sql, "GFData");
                }
                catch (Exception ex)
                {
                    log.Error(ex, "국지전 웨이브 정보 가져오기 에러");
                }
                */
                return null;
            }
        }

        /// <summary>
        /// 중장비부대
        /// </summary>
        public class Squad
        {
            /// <summary>
            /// 스킬훈련 소요시간 (스킬 1)
            /// </summary>
            public static Dictionary<int, int> skill1TrainTime = new Dictionary<int, int>()
            {
                { 2, TimeUtil.HOUR * 2 },
                { 3, TimeUtil.HOUR * 4 },
                { 4, TimeUtil.HOUR * 8 },
                { 5, TimeUtil.HOUR * 12 },
                { 6, TimeUtil.HOUR * 18 },
                { 7, TimeUtil.HOUR * 24 },
                { 8, TimeUtil.HOUR * 30 },
                { 9, TimeUtil.HOUR * 36 },
                { 10, TimeUtil.HOUR * 48 },
            };

            /// <summary>
            /// 스킬훈련 소요시간 (스킬 2, 3)
            /// </summary>
            public static Dictionary<int, int> skill23TrainTime = new Dictionary<int, int>()
            {
                { 2, TimeUtil.HOUR * 1 },
                { 3, TimeUtil.HOUR * 2 },
                { 4, TimeUtil.HOUR * 4 },
                { 5, TimeUtil.HOUR * 6 },
                { 6, TimeUtil.HOUR * 9 },
                { 7, TimeUtil.HOUR * 12 },
                { 8, TimeUtil.HOUR * 15 },
                { 9, TimeUtil.HOUR * 18 },
                { 10, TimeUtil.HOUR * 24 },
            };

            /// <summary>
            /// 누적 경험치 (level, total_exp)
            /// </summary>
            public static Dictionary<int, long> totalExp = new Dictionary<int, long>()
            {
                { 1, 0L },
                { 2, 500L },
                { 3, 1400L },
                { 4, 2700L },
                { 5, 4500L },
                { 6, 6700L },
                { 7, 9400L },
                { 8, 12600L },
                { 9, 16200L },
                { 10, 20200L },
                { 11, 24700L },
                { 12, 29700L },
                { 13, 35100L },
                { 14, 40900L },
                { 15, 47200L },
                { 16, 54000L },
                { 17, 61200L },
                { 18, 68800L },
                { 19, 77100L },
                { 20, 86100L },
                { 21, 95900L },
                { 22, 106500L },
                { 23, 118500L },
                { 24, 132000L },
                { 25, 147000L },
                { 26, 163500L },
                { 27, 181800L },
                { 28, 201900L },
                { 29, 223900L },
                { 30, 247900L },
                { 31, 274200L },
                { 32, 302500L },
                { 33, 333300L },
                { 34, 366600L },
                { 35, 402400L },
                { 36, 441000L },
                { 37, 482400L },
                { 38, 526600L },
                { 39, 574000L },
                { 40, 624600L },
                { 41, 678400L },
                { 42, 735700L },
                { 43, 796500L },
                { 44, 861000L },
                { 45, 929200L },
                { 46, 1001500L },
                { 47, 1077900L },
                { 48, 1158400L },
                { 49, 1243300L },
                { 50, 1332700L },
                { 51, 1426800L },
                { 52, 1525600L },
                { 53, 1629400L },
                { 54, 1738300L },
                { 55, 1852300L },
                { 56, 1971800L },
                { 57, 2096700L },
                { 58, 2227200L },
                { 59, 2363500L },
                { 60, 2505900L },
                { 61, 2654400L },
                { 62, 2809000L },
                { 63, 2970100L },
                { 64, 3137800L },
                { 65, 3312300L },
                { 66, 3493800L },
                { 67, 3682300L },
                { 68, 3877800L },
                { 69, 4080800L },
                { 70, 4291400L },
                { 71, 4509600L },
                { 72, 4735800L },
                { 73, 4970000L },
                { 74, 5212500L },
                { 75, 5463300L },
                { 76, 5722800L },
                { 77, 5990800L },
                { 78, 6267800L },
                { 79, 6553800L },
                { 80, 6849300L },
                { 81, 7154000L },
                { 82, 7468500L },
                { 83, 7792500L },
                { 84, 8127000L },
                { 85, 8471000L },
                { 86, 8826000L },
                { 87, 9191000L },
                { 88, 9567000L },
                { 89, 9954000L },
                { 90, 10352000L },
                { 91, 10761000L },
                { 92, 11182000L },
                { 93, 11614000L },
                { 94, 12058000L },
                { 95, 12514000L },
                { 96, 12983000L },
                { 97, 13464000L },
                { 98, 13957000L },
                { 99, 14463000L },
                { 100, 15000000L },
            };

            /// <summary>
            /// 중장비부대 정보 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <param name="col"></param>
            /// <returns></returns>
            public static string GetData(int id, string col)
            {
                //string sql = string.Format("SELECT {0} FROM squad WHERE id = {1} LIMIT 1", col, id);
                //string value = GetDb(sql, col, "GFData");
                //if (string.IsNullOrEmpty(value))
                //    value = id.ToString();
                //return value;

                JObject json = GetJsonDb("squad");
                string key = id.ToString();
                if (json.ContainsKey(key))
                {
                    return json[key]["name"].Value<string>();
                }
                return "";
            }

            /// <summary>
            /// 중장비 스킬훈련 시간 가져오기
            /// </summary>
            /// <param name="skill"></param>
            /// <param name="toLv"></param>
            /// <returns></returns>
            public static int GetSquadSkillTrainTime(int skill, int toLv)
            {
                int requireTime = 0;
                switch (skill)
                {
                    case 1:
                        if (skill1TrainTime.ContainsKey(toLv))
                            requireTime = skill1TrainTime[toLv];
                        break;
                    case 2:
                    case 3:
                        if (skill23TrainTime.ContainsKey(toLv))
                            requireTime = skill23TrainTime[toLv];
                        break;
                }
                // 지휘관 보너스 사용 시
                if (Config.Costume.coBonusSkillTrainTime)
                {
                    double tempRequireTime = (double)requireTime;
                    requireTime = (int)(tempRequireTime - tempRequireTime * (double)(Config.Costume.coBonusSkillTrainTimePercent) / 100);
                }
                return requireTime;
            }
        }

        /// <summary>
        /// 인형
        /// </summary>
        public class Doll
        {
            /// <summary>
            /// 인형정보 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static JObject GetDollData(int id)
            {
                //string sql = string.Format("SELECT * FROM doll WHERE id = '{0}' LIMIT 1", id);
                //return GetDb(sql, "GFData");

                JObject json = GetJsonDb("doll");
                string key = id.ToString();
                if (json.ContainsKey(key))
                {
                    return json[key].Value<JObject>();
                }
                return new JObject();
            }

            /// <summary>
            /// 인형 이름 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static string GetDollName(int id)
            {
                return LanguageResources.Instance[string.Format("DOLL_{0}", id)];
            }

            /// <summary>
            /// 인형 스킨 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static List<int> GetDollSkin(int no, bool isLive2d = false, bool isChild = false)
            {
                //HashSet<int> result = new HashSet<int>();
                //no = no > 20000 ? no % 20000 : no;
                //string sql = string.Format("SELECT * FROM skin WHERE gun_id = '{0}'", no);
                //if (isLive2d)
                //    sql += " AND is_live2d = true ";
                //if (isChild)
                //    sql += " AND is_child = true ";
                //List<JObject> items = GetDbs(sql, "GFData");
                //foreach (JObject item in items)
                //{
                //    result.Add(Parser.Json.ParseInt(item["skin_id"]));
                //}
                //return result;

                HashSet<int> result = new HashSet<int>();
                no = no > 20000 ? no % 20000 : no;
                string id = no.ToString();
                JObject json = GetJsonDb("skin");
                string[] keys = json.Properties().Select(p => p.Name).ToArray();
                foreach (string key in keys)
                {
                    JObject item = json[key].Value<JObject>();
                    if (item["gun_id"].Value<string>() != id)
                        continue;
                    if (item["is_live2d"].Value<string>() != (isLive2d == true ? "1" : "0"))
                        continue;
                    if (item["is_child"].Value<string>() != (isChild == true ? "1" : "0"))
                        continue;
                    result.Add(Parser.String.ParseInt(key));
                }
                return result.ToList();
            }

            /// <summary>
            /// 인형 스킨 가져오기
            /// </summary>
            /// <param name="isLive2d"></param>
            /// <param name="isChild"></param>
            /// <returns></returns>
            public static List<int> GetDollSkin(bool isLive2d, bool isChild)
            {
                //List<int> result = new List<int>();
                //string sql = string.Format("SELECT * FROM skin WHERE 1 = 1");
                //if (isLive2d)
                //    sql += " AND is_live2d = true ";
                //if (isChild)
                //    sql += " AND is_child = true ";
                //List<JObject> items = GetDbs(sql, "GFData");
                //foreach (JObject item in items)
                //{
                //    result.Add(Parser.Json.ParseInt(item["skin_id"]));
                //}
                //return result;

                HashSet<int> result = new HashSet<int>();
                JObject json = GetJsonDb("skin");
                string[] keys = json.Properties().Select(p => p.Name).ToArray();
                foreach (string key in keys)
                {
                    JObject item = json[key].Value<JObject>();
                    if (item["is_live2d"].Value<string>() != (isLive2d == true ? "1" : "0"))
                        continue;
                    if (item["is_child"].Value<string>() != (isChild == true ? "1" : "0"))
                        continue;
                    result.Add(Parser.String.ParseInt(key));
                }
                return result.ToList();
            }

            /// <summary>
            /// 인형 스킨 가져오기 (보유 스킨 중에서)
            /// </summary>
            /// <param name="no"></param>
            /// <param name="ownSkins"></param>
            /// <returns></returns>
            public static List<int> GetDollSkin(int no, List<int> ownSkins, bool isLive2d = false, bool isChild = false)
            {
                List<int> result = new List<int>();
                List<int> dollSkins = GetDollSkin(no, isLive2d, isChild);
                foreach (int ownSkin in ownSkins)
                {
                    if (dollSkins.Contains(ownSkin))
                    {
                        result.Add(ownSkin);
                    }
                }
                return result;
            }

            /// <summary>
            /// 스킨을 보유하고 있는 인형 가져오기
            /// type = 0: 모든 스킨, 1: 랖투디 스킨만, 2: 아동절 스킨만
            /// </summary>
            /// <param name="ownSkins"></param>
            /// <returns></returns>
            public static List<int> GetDollOwnSkin(List<int> ownSkins, bool isLive2d = false, bool isChild = false)
            {
                //try
                //{
                //    HashSet<int> result = new HashSet<int>();
                //    string sql = string.Format("SELECT * FROM skin WHERE skin_id in ({0})", string.Join(",", ownSkins));
                //    if (isLive2d)
                //        sql += " AND is_live2d = true ";
                //    if (isChild)
                //        sql += " AND is_child = true ";
                //    List<JObject> items = GetDbs(sql, "GFData");
                //    foreach (JObject item in items)
                //    {
                //        result.Add(Parser.Json.ParseInt(item["gun_id"]));
                //    }
                //    return result.ToList();
                //}
                //catch { }
                //return new List<int>();

                try
                {
                    HashSet<int> result = new HashSet<int>();
                    JObject json = GetJsonDb("skin");
                    string[] keys = json.Properties().Select(p => p.Name).ToArray();
                    foreach (string key in keys)
                    {
                        JObject item = json[key].Value<JObject>();
                        if (!ownSkins.Contains(item["skin_id"].Value<int>()))
                            continue;
                        if (item["is_live2d"].Value<string>() != (isLive2d == true ? "1" : "0"))
                            continue;
                        if (item["is_child"].Value<string>() != (isChild == true ? "1" : "0"))
                            continue;
                        result.Add(item["gun_id"].Value<int>());
                    }
                    return result.ToList();
                }
                catch { }
                return new List<int>();
            }

            /// <summary>
            /// 랜덤 부관 가져오기
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public static string GetRandomAdjutant(JObject index)
            {
                Random random = new Random();
                string result = "";
                try
                {
                    int confirmDoll = 0;    // 확정 인형
                    int confirmSkin = 0;    // 확정 스킨
                    int confirmShape = 0;   // 확정 형태 (통상, 중상)
                    int confirmMod = 0;     // 확정 개조

                    // 소속 인형
                    HashSet<int> ownDollSet = new HashSet<int>();
                    if (index.ContainsKey("gun_with_user_info") && index["gun_with_user_info"] is JArray)
                    {
                        JArray gunWithUserInfo = index["gun_with_user_info"].Value<JArray>();
                        foreach (JObject item in gunWithUserInfo)
                        {
                            try
                            {
                                int level = Parser.Json.ParseInt(item["gun_level"]);
                                if (level <= 1)
                                    continue;
                                int no = Parser.Json.ParseInt(item["gun_id"]);
                                ownDollSet.Add(no);
                            }
                            catch { }
                        }
                        ownDollSet.Add(-1); // 카리나 추가
                    }
                    // 보유 스킨
                    HashSet<int> ownSkinSet = new HashSet<int>();
                    if (index.ContainsKey("skin_with_user_info") && index["skin_with_user_info"] is JObject)
                    {
                        JObject skinWithUserInfo = index["skin_with_user_info"].Value<JObject>();
                        string[] keys = skinWithUserInfo.Properties().Select(p => p.Name).ToArray();
                        foreach (string key in keys)
                        {
                            try
                            {
                                int skinId = skinWithUserInfo[key]["skin_id"].Value<int>();
                                ownSkinSet.Add(skinId);
                            }
                            catch { }
                        }
                    }

                    // 인형 핕터링 (블랙리스트, 화이트리스트)
                    HashSet<int> filteredDollSet = new HashSet<int>();
                    foreach (int item in ownDollSet)
                    {
                        // 블랙리스트
                        if (Config.Adjutant.dollBlacklist.Contains(item))
                            continue;
                        // 화이트리스트
                        if (Config.Adjutant.dollWhitelist.Length > 0 && !Config.Adjutant.dollWhitelist.Contains(item))
                            continue;
                        filteredDollSet.Add(item);
                    }
                    log.Debug("소속 인형 {0}명 {1}", filteredDollSet.Count(), string.Join(",", filteredDollSet));

                    // 스킨 필터링 (블랙리스트, 화이트리스트)
                    HashSet<int> filteredSkinSet = new HashSet<int>();
                    foreach (int item in ownSkinSet)
                    {
                        // 블랙리스트
                        if (Config.Adjutant.skinBlacklist.Contains(item))
                            continue;
                        // 화이트리스트
                        if (Config.Adjutant.skinWhitelist.Length > 0 && !Config.Adjutant.skinWhitelist.Contains(item))
                            continue;
                        filteredSkinSet.Add(item);
                    }
                    log.Debug("보유 스킨 {0}개 {1}", filteredSkinSet.Count(), string.Join(",", filteredSkinSet));

                    bool isLive2d = Config.Adjutant.adjutantSkinCategory == "live2d_skin" ? true : false;
                    bool isChild = Config.Adjutant.adjutantSkinCategory == "child_skin" ? true : false;

                    // 스킨있는 인형만
                    if (Config.Adjutant.adjutantSkinCategory == "skin" ||           // 스킨만
                        Config.Adjutant.adjutantSkinCategory == "live2d_skin" ||    // 랖투디만
                        Config.Adjutant.adjutantSkinCategory == "child_skin")       // 아동절만
                    {
                        List<int> ownSkinDoll = GetDollOwnSkin(filteredSkinSet.ToList(), isLive2d, isChild);
                        log.Debug("스킨 보유 인형 {0}명 {1}", ownSkinDoll.Count(), string.Join(",", ownSkinDoll));
                        HashSet<int> tempFilteredDollSet = new HashSet<int>();
                        foreach (int item in filteredDollSet) 
                        {
                            if (ownSkinDoll.Contains(item))
                                tempFilteredDollSet.Add(item);
                        }
                        filteredDollSet = tempFilteredDollSet;
                    }

                    if (Config.Adjutant.adjutantSkinCategory == "mod_skin")  // 개조만
                        filteredDollSet = filteredDollSet.Select(n => n).Where(n => n > 20000).ToHashSet();

                    // 인형 확정
                    log.Debug("랜덤 인형 범위 {0}~{1} {2}", 0, filteredDollSet.Count(), string.Join(",", filteredDollSet));
                    if (filteredDollSet.Count() > 0)
                    {
                        int randDollIdx = random.Next(0, filteredDollSet.Count());
                        int[] filteredDollArray = filteredDollSet.ToArray();
                        confirmDoll = filteredDollArray[randDollIdx];
                        log.Debug("확정 부관 idx={0} no={1} name={2}", randDollIdx, confirmDoll, GameData.Doll.GetDollName(confirmDoll));
                        confirmMod = confirmDoll > 20000 ? 3 : 0;
                    }
                    if (Config.Adjutant.adjutantSkinCategory == "normal")
                    {
                        confirmSkin = 0;
                    }
                    else
                    {
                        if (filteredSkinSet.Count() > 0)
                        {
                            // 스킨 확정
                            List<int> filteredSkinArray = GameData.Doll.GetDollSkin(confirmDoll, filteredSkinSet.ToList(), isLive2d, isChild);
                            if (Config.Adjutant.adjutantSkinCategory == "mod_skin")
                                filteredSkinArray.Add(0);
                            log.Debug("랜덤 스킨 범위 {0}~{1} {2}", 0, filteredSkinArray.Count(), string.Join(",", filteredSkinArray));
                            if (filteredSkinArray.Count() > 0)
                            {
                                int randSkinIdx = random.Next(0, filteredSkinArray.Count());
                                confirmSkin = filteredSkinArray[randSkinIdx];
                                if (Config.Adjutant.adjutantSkinCategory == "mod_skin") // 개조만
                                    confirmSkin = 0;
                            }
                            log.Debug("확정 스킨 {0}", confirmSkin);
                        }
                        else
                        {
                            confirmSkin = 0;
                            log.Debug("스킨 없음");
                        }
                    }

                    // 중상 여부
                    switch (Config.Adjutant.adjutantShape)
                    {
                        case 0: // 통상
                            confirmShape = 0;
                            log.Debug("확정 통상");
                            break;
                        case 1: // 중상
                            confirmShape = 1;
                            log.Debug("확정 중상");
                            break;
                        case 2: // 랜덤
                            int randShape = random.Next(0, 100);
                            confirmShape = randShape < 50 ? 0 : 1;
                            log.Debug("확정 중상 {0} ({1})", confirmShape, randShape);
                            break;
                    }
                    if (confirmDoll == -1) // 카리나는 중상 없음
                        confirmShape = 0;

                    if (confirmDoll != 0)
                        result = string.Format("{0},{1},{2},{3}", confirmDoll, confirmSkin, confirmShape, confirmMod);
                    else
                        result = index["user_record"]["adjutant"].Value<string>();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                log.Debug("랜덤 부관 결과 - {0}", result);
                MainWindow.proxyView.setRandomAdjutant = result;
                return result;
            }

            /// <summary>
            /// 인형 스킬훈련시간 가져오기
            /// </summary>
            /// <param name="lv"></param>
            /// <returns></returns>
            public static int GetDollSkillTrainTime(int lv)
            {
                int requireTime = 0;
                if (_skillTrainTime.ContainsKey(lv))
                {
                    requireTime = _skillTrainTime[lv];
                    if (Config.Costume.coBonusSkillTrainTime)
                    {
                        double tempRequireTime = (double)requireTime;
                        requireTime = (int)(tempRequireTime - tempRequireTime * ((double)Config.Costume.coBonusSkillTrainTimePercent / 100));
                    }
                }
                return requireTime;
            }
            private static Dictionary<int, int> _skillTrainTime = new Dictionary<int, int>()
            {
                { 2, TimeUtil.HOUR * 1  },
                { 3, TimeUtil.HOUR * 2  },
                { 4, TimeUtil.HOUR * 3  },
                { 5, TimeUtil.HOUR * 4  },
                { 6, TimeUtil.HOUR * 6  },
                { 7, TimeUtil.HOUR * 9  },
                { 8, TimeUtil.HOUR * 12 },
                { 9, TimeUtil.HOUR * 18 },
                { 10, TimeUtil.HOUR * 24 }
            };

            // 경험치 관련
            public class Exp
            {
                /// <summary>
                /// 누적 경험치
                /// </summary>
                public static Dictionary<int, int> totalExp = new Dictionary<int, int>()
                {
                    #region 누적 경험치
                    { 1, 0 },
                    { 2, 100 },
                    { 3, 300 },
                    { 4, 600 },
                    { 5, 1000 },
                    { 6, 1500 },
                    { 7, 2100 },
                    { 8, 2800 },
                    { 9, 3600 },
                    { 10, 4500 },
                    { 11, 5500 },
                    { 12, 6600 },
                    { 13, 7800 },
                    { 14, 9100 },
                    { 15, 10500 },
                    { 16, 12000 },
                    { 17, 13600 },
                    { 18, 15300 },
                    { 19, 17100 },
                    { 20, 19000 },
                    { 21, 21000 },
                    { 22, 23100 },
                    { 23, 25300 },
                    { 24, 27600 },
                    { 25, 30000 },
                    { 26, 32500 },
                    { 27, 35100 },
                    { 28, 37900 },
                    { 29, 41000 },
                    { 30, 44400 },
                    { 31, 48600 },
                    { 32, 53200 },
                    { 33, 58200 },
                    { 34, 63600 },
                    { 35, 69400 },
                    { 36, 75700 },
                    { 37, 82400 },
                    { 38, 89600 },
                    { 39, 97300 },
                    { 40, 105500 },
                    { 41, 114300 },
                    { 42, 123600 },
                    { 43, 133500 },
                    { 44, 144000 },
                    { 45, 155100 },
                    { 46, 166900 },
                    { 47, 179400 },
                    { 48, 192500 },
                    { 49, 206400 },
                    { 50, 221000 },
                    { 51, 236400 },
                    { 52, 252500 },
                    { 53, 269400 },
                    { 54, 287200 },
                    { 55, 305800 },
                    { 56, 325300 },
                    { 57, 345700 },
                    { 58, 367000 },
                    { 59, 389300 },
                    { 60, 412600 },
                    { 61, 436900 },
                    { 62, 462200 },
                    { 63, 488500 },
                    { 64, 515900 },
                    { 65, 544400 },
                    { 66, 574000 },
                    { 67, 604800 },
                    { 68, 636800 },
                    { 69, 670000 },
                    { 70, 704400 },
                    { 71, 749500 },
                    { 72, 796300 },
                    { 73, 844900 },
                    { 74, 895300 },
                    { 75, 947500 },
                    { 76, 1001500 },
                    { 77, 1057400 },
                    { 78, 1115300 },
                    { 79, 1175100 },
                    { 80, 1236900 },
                    { 81, 1300800 },
                    { 82, 1366800 },
                    { 83, 1434900 },
                    { 84, 1505200 },
                    { 85, 1577800 },
                    { 86, 1652600 },
                    { 87, 1729700 },
                    { 88, 1809200 },
                    { 89, 1891100 },
                    { 90, 1975400 },
                    { 91, 2088000 },
                    { 92, 2204100 },
                    { 93, 2323600 },
                    { 94, 2446700 },
                    { 95, 2573400 },
                    { 96, 2703800 },
                    { 97, 2837900 },
                    { 98, 2975800 },
                    { 99, 3117600 },
                    { 100, 3263300 },
                    { 101, 3363300 },
                    { 102, 3483300 },
                    { 103, 3623300 },
                    { 104, 3783300 },
                    { 105, 3963300 },
                    { 106, 4163300 },
                    { 107, 4383300 },
                    { 108, 4623300 },
                    { 109, 4903300 },
                    { 110, 5263300 },
                    { 111, 5743300 },
                    { 112, 6383300 },
                    { 113, 7283300 },
                    { 114, 8483300 },
                    { 115, 10083300 },
                    { 116, 12283300 },
                    { 117, 15283300 },
                    { 118, 19283300 },
                    { 119, 24283300 },
                    { 120, 30283300 },
                    #endregion
                };

                /// <summary>
                /// 필요 경험치
                /// </summary>
                public static Dictionary<int, int> requireExp = new Dictionary<int, int>()
                {
                    #region 필요 경험치
                    { 1, 0 },
                    { 2, 100 },
                    { 3, 200 },
                    { 4, 300 },
                    { 5, 400 },
                    { 6, 500 },
                    { 7, 600 },
                    { 8, 700 },
                    { 9, 800 },
                    { 10, 900 },
                    { 11, 1000 },
                    { 12, 1100 },
                    { 13, 1200 },
                    { 14, 1300 },
                    { 15, 1400 },
                    { 16, 1500 },
                    { 17, 1600 },
                    { 18, 1700 },
                    { 19, 1800 },
                    { 20, 1900 },
                    { 21, 2000 },
                    { 22, 2100 },
                    { 23, 2200 },
                    { 24, 2300 },
                    { 25, 2400 },
                    { 26, 2500 },
                    { 27, 2600 },
                    { 28, 2800 },
                    { 29, 3100 },
                    { 30, 3400 },
                    { 31, 4200 },
                    { 32, 4600 },
                    { 33, 5000 },
                    { 34, 5400 },
                    { 35, 5800 },
                    { 36, 6300 },
                    { 37, 6700 },
                    { 38, 7200 },
                    { 39, 7700 },
                    { 40, 8200 },
                    { 41, 8800 },
                    { 42, 9300 },
                    { 43, 9900 },
                    { 44, 10500 },
                    { 45, 11100 },
                    { 46, 11800 },
                    { 47, 12500 },
                    { 48, 13100 },
                    { 49, 13900 },
                    { 50, 14600 },
                    { 51, 15400 },
                    { 52, 16100 },
                    { 53, 16900 },
                    { 54, 17800 },
                    { 55, 18600 },
                    { 56, 19500 },
                    { 57, 20400 },
                    { 58, 21300 },
                    { 59, 22300 },
                    { 60, 23300 },
                    { 61, 24300 },
                    { 62, 25300 },
                    { 63, 26300 },
                    { 64, 27400 },
                    { 65, 28500 },
                    { 66, 29600 },
                    { 67, 30800 },
                    { 68, 32000 },
                    { 69, 33200 },
                    { 70, 34400 },
                    { 71, 45100 },
                    { 72, 46800 },
                    { 73, 48600 },
                    { 74, 50400 },
                    { 75, 52200 },
                    { 76, 54000 },
                    { 77, 55900 },
                    { 78, 57900 },
                    { 79, 59800 },
                    { 80, 61800 },
                    { 81, 63900 },
                    { 82, 66000 },
                    { 83, 68100 },
                    { 84, 70300 },
                    { 85, 72600 },
                    { 86, 74800 },
                    { 87, 77100 },
                    { 88, 79500 },
                    { 89, 81900 },
                    { 90, 84300 },
                    { 91, 112600 },
                    { 92, 116100 },
                    { 93, 119500 },
                    { 94, 123100 },
                    { 95, 126700 },
                    { 96, 130400 },
                    { 97, 134100 },
                    { 98, 137900 },
                    { 99, 141800 },
                    { 100, 145700 },
                    { 101, 100000 },
                    { 102, 120000 },
                    { 103, 140000 },
                    { 104, 160000 },
                    { 105, 180000 },
                    { 106, 200000 },
                    { 107, 220000 },
                    { 108, 240000 },
                    { 109, 280000 },
                    { 110, 360000 },
                    { 111, 480000 },
                    { 112, 640000 },
                    { 113, 900000 },
                    { 114, 1200000 },
                    { 115, 1600000 },
                    { 116, 2200000 },
                    { 117, 3000000 },
                    { 118, 4000000 },
                    { 119, 5000000 },
                    { 120, 6000000 },
                    #endregion
                };

                /// <summary>
                /// 누적 경험치 가져오기
                /// </summary>
                /// <param name="lv"></param>
                /// <returns></returns>
                public static int GetTotalExp(int lv)
                {
                    if (totalExp.ContainsKey(lv))
                        return totalExp[lv];
                    return 0;
                }

                /// <summary>
                /// 필요 경험치 가져오기
                /// </summary>
                /// <param name="lv"></param>
                /// <returns></returns>
                public static int GetRequireExp(int lv, int mod = 0)
                {
                    if (mod == 0 && lv == 101)
                        return 0;
                    else if (mod == 1 && lv == 111)
                        return 0;
                    else if (mod == 2 && lv == 116)
                        return 0;
                    else if (mod == 3 && lv == 121)
                        return 0;
                    if (requireExp.ContainsKey(lv))
                        return requireExp[lv];
                    return 0;
                }
            }

            // 스탯 관련
            public class Stat
            {
                #region 스탯 관련 변수
                // 능력치 종류
                private static string[] attrList = new string[] { "hp", "pow", "hit", "dodge", "speed", "rate", "armor" };

                // 인형 종류별 기본 능력치
                private static Dictionary<string, Dictionary<string, double>> dollAttrs = new Dictionary<string, Dictionary<string, double>>()
            {
                { "HG", new Dictionary<string, double>() {
                    { "hp", 0.6 },
                    { "pow", 0.6 },
                    { "rate", 0.8 },
                    { "speed", 1.5 },
                    { "hit", 1.2 },
                    { "dodge", 1.8 }
                } },
                { "SMG", new Dictionary<string, double>() {
                    { "hp", 1.6 },
                    { "pow", 0.6 },
                    { "rate", 1.2 },
                    { "speed", 1.2 },
                    { "hit", 0.3 },
                    { "dodge", 1.6 }
                } },
                { "RF", new Dictionary<string, double>() {
                    { "hp", 0.8 },
                    { "pow", 2.4 },
                    { "rate", 0.5 },
                    { "speed", 0.7 },
                    { "hit", 1.6 },
                    { "dodge", 0.8 }
                } },
                { "AR", new Dictionary<string, double>() {
                    { "hp", 1 },
                    { "pow", 1 },
                    { "rate", 1 },
                    { "speed", 1 },
                    { "hit", 1 },
                    { "dodge", 1 }
                } },
                { "MG", new Dictionary<string, double>() {
                    { "hp", 1.5 },
                    { "pow", 1.8 },
                    { "rate", 1.6 },
                    { "speed", 0.4 },
                    { "hit", 0.6 },
                    { "dodge", 0.6 }
                } },
                { "SG", new Dictionary<string, double>() {
                    { "hp", 2 },
                    { "pow", 0.7 },
                    { "rate", 0.4 },
                    { "speed", 0.6 },
                    { "hit", 0.3 },
                    { "dodge", 0.3 },
                    { "armor", 1 }
                } },
            };

                // 인형 성장 능력치
                private static Dictionary<string, Dictionary<string, double[]>> dollGrows = new Dictionary<string, Dictionary<string, double[]>>()
            {
                { "Basic", new Dictionary<string, double[]>() {
                    { "armor", new double[] { 2, 0.161 } },
                    { "dodge", new double[] { 5 } },
                    { "hit", new double[] { 5 } },
                    { "hp", new double[] { 55, 0.555 } },
                    { "pow", new double[] { 16 } },
                    { "rate", new double[] { 45 } },
                    { "speed", new double[] { 10 } },
                }},
                { "Grow", new Dictionary<string, double[]>()
                {
                    { "dodge", new double[] { 0.303, 0 } },
                    { "hit", new double[] { 0.303, 0 } },
                    { "pow", new double[] { 0.242, 0 } },
                    { "rate", new double[] { 0.181, 0 } },
                }},
            };
                // 개조 이후
                private static Dictionary<string, Dictionary<string, double[]>> dollGrowsAfter100 = new Dictionary<string, Dictionary<string, double[]>>()
            {
                { "Basic", new Dictionary<string, double[]>() {
                    { "armor", new double[] { 13.979, 0.04 } },
                    { "dodge", new double[] { 5 } },
                    { "hit", new double[] { 5 } },
                    { "hp", new double[] { 96.283, 0.138 } },
                    { "pow", new double[] { 16 } },
                    { "rate", new double[] { 45 } },
                    { "speed", new double[] { 10 } },
                }},
                { "Grow", new Dictionary<string, double[]>()
                {
                    { "dodge", new double[] { 0.075, 22.572 } },
                    { "hit", new double[] { 0.075, 22.572 } },
                    { "pow", new double[] { 0.06, 18.018 } },
                    { "rate", new double[] { 0.022, 15.741 } },
                }},
            };
                #endregion

                /// <summary>
                /// 인형 스탯 정보 가져오기
                /// 0: hp, 1: pow, 2: hit, 3: dodge, 4: speed, 5: rate, 6: armor, 7: bullet, 8: crit
                /// </summary>
                /// <param name="no"></param>
                /// <param name="level"></param>
                /// <param name="favor"></param>
                /// <returns>
                /// short[] { 0: hp, 1: pow, 2: hit, 3: dodge, 4: speed, 5: rate, 6: armor, 7: bullet, 8: crit }
                /// </returns>
                public static int[] GetStat(int no, int level, int favor = 50)
                {
                    //log.Debug("no {0} {1} level {2} favor {3}", no, GameData2.Doll.GetDollName(no), level, favor);
                    int[] result = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    try
                    {
                        JObject data = GetDollData(no);
                        // 장탄
                        if (data.ContainsKey("bullet"))
                            result[7] = Parser.Json.ParseShort(data["bullet"]);
                        // 치명률
                        if (data.ContainsKey("crit"))
                            result[8] = Parser.Json.ParseShort(data["crit"]);

                        // 인형 능력치 정보
                        Dictionary<string, double> dollStat = new Dictionary<string, double>();
                        foreach (string attr in attrList)
                        {
                            if (data.ContainsKey(attr))
                            {
                                double stat = Parser.Json.ParseDouble(data[attr]);
                                if (stat > 0)
                                    dollStat.Add(attr, stat);
                            }
                        }
                        // 인형 성장 능력치
                        double grow = 0;
                        if (data.ContainsKey("grow"))
                            grow = Parser.Json.ParseDouble(data["grow"]);

                        // 병종별 능력치 팩터
                        string type = Parser.Json.ParseString(data["type"]);
                        Dictionary<string, double> dollAttr = dollAttrs[type];

                        // 레벨업 능력치 팩터
                        Dictionary<string, double[]> dollBasicStat = level > 100 ? dollGrowsAfter100["Basic"] : dollGrows["Basic"];
                        Dictionary<string, double[]> dollGrowStat = level > 100 ? dollGrowsAfter100["Grow"] : dollGrows["Grow"];
                        for (int i = 0; i < attrList.Length; i++)
                        {
                            string key = attrList[i];

                            double realStat = 0;

                            double stat = 0;
                            if (dollStat.ContainsKey(key))
                                stat = dollStat[key];

                            double[] basicData;
                            if (dollBasicStat.ContainsKey(key))
                                basicData = dollBasicStat[key];
                            else
                                basicData = new double[] { 0 };

                            double attr = 0;
                            if (dollAttr.ContainsKey(key))
                                attr = dollAttr[key];

                            if (basicData.Length > 1)
                                realStat = Math.Ceiling((((basicData[0] + ((level - 1) * basicData[1])) * attr) * stat) / 100);
                            else
                                realStat = Math.Ceiling(((basicData[0] * attr) * stat) / 100);

                            if (dollGrowStat.ContainsKey(key))
                            {
                                double[] growData = dollGrowStat[key];
                                realStat += Math.Ceiling(((((growData[1] + ((level - 1) * growData[0])) * attr * stat) * grow) / 100) / 100);
                            }

                            if ("pow".Equals(key) || "hit".Equals(key) || "dodge".Equals(key))
                                realStat += Math.Sign(GetFavorBonusRatio(favor)) * Math.Ceiling(Math.Abs(realStat * GetFavorBonusRatio(favor)));

                            result[i] = Convert.ToInt32(realStat);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    return result;
                }

                /// <summary>
                /// 호감도 능력치 보너스
                /// </summary>
                /// <param name="favor"></param>
                /// <returns></returns>
                public static double GetFavorBonusRatio(int favor)
                {
                    if (favor < 10)
                    {
                        return -0.05;
                    }
                    else if (favor < 90)
                    {
                        return 0;
                    }
                    else if (favor < 140)
                    {
                        return 0.05;
                    }
                    else if (favor < 190)
                    {
                        return 0.1;
                    }
                    return 0.15;
                }
            }

            // 수복 관련
            public class Restore
            {
                /// <summary>
                /// 병과별 수복 자원/시간 배율
                /// (string: 병과, double[]: 시간, 인력, 부품)
                /// </summary>
                private static Dictionary<string, double[]> restoreRatio = new Dictionary<string, double[]>()
                {
                    { "HG", new double[] { 0.5, 2.0, 0.7 } },
                    { "SMG", new double[] { 1.0, 4.5, 1.2 } },
                    { "AR", new double[] { 1.0, 4.0, 1.4 } },
                    { "RF", new double[] { 1.0, 3.5, 1.6 } },
                    { "MG", new double[] { 2.0, 7.5, 3.0 } },
                    { "SG", new double[] { 2.0, 6.5, 3.2 } },
                };

                /// <summary>
                /// 수복 정보 가져오기
                /// </summary>
                /// <param name="gunWithUserId"></param>
                /// <returns></returns>
                public static int[] GetRestore(long gunWithUserId)
                {
                    DollWithUserInfo doll = UserData.Doll.Get(gunWithUserId);
                    if (doll != null)
                        return GetRestore(doll.type, doll.level, doll.hp, doll.maxHp, doll.maxLink, doll.married);
                    return new int[] { 0, 0, 0 };
                }

                public static int[] GetRestore(string type, double level, double hp, double maxHp, double maxLink, bool married)
                {
                    int[] result = new int[] { 0, 0, 0 };

                    // 인형 정보 없거나 최대 체력인 경우
                    if (string.IsNullOrEmpty(type) || level <= 0 || hp <= 0 || maxHp < 1 || maxLink < 1 || hp == maxHp)
                        return result;

                    // 인형 병과별 자원/시간 배율
                    double timeRatio = 1.0;
                    double mpRatio = 1.0;
                    double partRatio = 1.0;
                    if (restoreRatio.ContainsKey(type))
                    {
                        timeRatio = restoreRatio[type][0];
                        mpRatio = restoreRatio[type][1];
                        partRatio = restoreRatio[type][2];
                    }

                    // 서약 수복시간 배율
                    double marriedRatio = 1.0;
                    if (married)
                    {
                        marriedRatio = 0.7;
                    }

                    // 손실 체력
                    double loseHp = maxHp - hp;

                    // 중상 여부 (체력 30% 미만)
                    bool wounded = hp < maxHp * 0.3 ? true : false;

                    double @base = 0.0;
                    if (wounded)
                    {
                        // 중상 수복
                        // Roundup(서약 배율 * 수리 시간 배율 * (인형 레벨 + 20) * (잃은 체력 / 최대 체력) ^ 5 * (400 - 35000 / (잃은 체력 - 최대 체력 * 0.7 + 150)))
                        result[0] = (int)Math.Ceiling(marriedRatio * timeRatio * (level + 20) * Math.Pow(loseHp / maxHp, 5) * (400 - 35000 / (loseHp - maxHp * 0.7 + 150)));

                        // 수복 자원
                        // Roundup(잃은 체력 / 최대 체력 * 0.5 * (최대 더미 수 + 1) * (10 + (잃은 체력 - 최대 체력 * 0.7) / 최대 체력 * 40) * 수복 자원 배율)
                        @base = loseHp / maxHp * 0.5 * (maxLink + 1) * (10 + (loseHp - maxHp * 0.7) / maxHp * 40);
                        result[1] = (int)Math.Ceiling(@base * mpRatio);
                        result[2] = (int)Math.Ceiling(@base * partRatio);
                    }
                    else
                    {
                        // 일반 수복
                        // Roundup(서약 배율 * 수리 시간 배율 * (인형 레벨 + 20) * 잃은 체력 / 최대 체력 * 40)
                        result[0] = (int)Math.Ceiling(marriedRatio * timeRatio * (level + 20) * loseHp / maxHp * 40);

                        // 수복 자원
                        // Roundup(잃은 체력 / 최대 체력 * 0.5 * (최대 더미 수 + 1) * 10 * 수복 자원 배율)
                        @base = loseHp / maxHp * 0.5 * (maxLink + 1) * 10;
                        result[1] = (int)Math.Ceiling(@base * mpRatio);
                        result[2] = (int)Math.Ceiling(@base * partRatio);
                    }

                    // 지휘관 복장 보너스 (수복 시간 단축)
                    if (Config.Costume.coBonusRestoreTime)
                    {
                        double tempTime = (double)result[0];
                        result[0] = (int)Math.Ceiling(tempTime - tempTime * (Convert.ToDouble(Config.Costume.coBonusRestoreTimePercent) / 100));
                    }

                    return result;
                }
            }



            /// <summary>
            /// 인형 최대 레벨 가져오기
            /// </summary>
            /// <param name="doll"></param>
            /// <returns></returns>
            public static int GetMaxLevel(DollWithUserInfo doll)
            {
                if (doll != null)
                {
                    return GetMaxLevel(doll.mod);
                }
                return 100;
            }
            public static int GetMaxLevel(int mod = 0)
            {
                int maxLevel = 100;
                switch (mod)
                {
                    case 0:
                        maxLevel = 100;
                        break;
                    case 1:
                        maxLevel = 110;
                        break;
                    case 2:
                        maxLevel = 115;
                        break;
                    case 3:
                        maxLevel = 120;
                        break;
                }
                return maxLevel;
            }

            /// <summary>
            /// 인형 소비 탄약/식량 (1회 전투 기준)
            /// (string: 병종, int: 링크, int[]: 탄식)
            /// </summary>
            public static MultiKeyDictionary<string, int, int[]> spendSupply = new MultiKeyDictionary<string, int, int[]>()
            {
                { "HG", 1, new int[] { 2,2 } },
                { "HG", 2, new int[] { 3,3 } },
                { "HG", 3, new int[] { 4,4 } },
                { "HG", 4, new int[] { 5,5 } },
                { "HG", 5, new int[] { 6,6 } },

                { "SMG", 1, new int[] { 5,4 } },
                { "SMG", 2, new int[] { 8,6 } },
                { "SMG", 3, new int[] { 11,8 } },
                { "SMG", 4, new int[] { 14,10 } },
                { "SMG", 5, new int[] { 17,12 } },

                { "AR", 1, new int[] { 4,4 } },
                { "AR", 2, new int[] { 6,6 } },
                { "AR", 3, new int[] { 8,8 } },
                { "AR", 4, new int[] { 10,10 } },
                { "AR", 5, new int[] { 12,12 } },

                { "RF", 1, new int[] { 3,6 } },
                { "RF", 2, new int[] { 4,9 } },
                { "RF", 3, new int[] { 7,12 } },
                { "RF", 4, new int[] { 9,15 } },
                { "RF", 5, new int[] { 11,18 } },

                { "MG", 1, new int[] { 8,6 } },
                { "MG", 2, new int[] { 13,9 } },
                { "MG", 3, new int[] { 18,12 } },
                { "MG", 4, new int[] { 23,15 } },
                { "MG", 5, new int[] { 28,18 } },

                { "SG", 1, new int[] { 6,8 } },
                { "SG", 2, new int[] { 9,13 } },
                { "SG", 3, new int[] { 12,18 } },
                { "SG", 4, new int[] { 15,23 } },
                { "SG", 5, new int[] { 18,28 } },
            };

            /// <summary>
            /// 인형 소비 탄약 / 식량 가져오기 (1회 전투 기준)
            /// </summary>
            /// <param name="doll"></param>
            /// <param name="ammo"></param>
            /// <param name="mre"></param>
            public static int[] GetSpendSupplyAmount(DollWithUserInfo doll)
            {
                if (doll != null)
                {
                    string category = doll.type;
                    int linkMax = doll.maxLink;

                    int spendAmmo = spendSupply[category, linkMax][0];
                    int spendMre = spendSupply[category, linkMax][1];
                    return new int[] { spendAmmo, spendMre };
                }
                return new int[] { 10, 10 };
            }

            /// <summary>
            /// 인형 남은 탄약 / 식량 가져오기 (남은 전투 횟수)
            /// </summary>
            /// <param name="doll"></param>
            /// <param name="ammoCount"></param>
            /// <param name="mreCount"></param>
            public static int[] GetSupplyCount(DollWithUserInfo doll)
            {
                if (doll != null)
                {
                    int link = doll.maxLink;
                    int ammo = doll.ammo;
                    int mre = doll.mre;

                    int[] spend = GetSpendSupplyAmount(doll);

                    if (spend.Length == 2)
                    {
                        int ammoCount = ammo / spend[0];
                        int mreCount = mre / spend[1];
                        return new int[] { ammoCount, mreCount };
                    }
                }
                return new int[] { };
            }

            /// <summary>
            /// 인형 거지런 1회 획득경험치 가져오기
            /// </summary>
            /// <param name="doll"></param>
            /// <param name="baseExp"></param>
            /// <param name="levelPenalty"></param>
            public static int GetRunExp(DollWithUserInfo doll, int baseExp, int levelPenalty = 120, bool ignoreMarriedBonus = false)
            {
                int level = doll.level;

                // 경험치 페널티
                double expPenalty = 1.0;
                if (level >= levelPenalty + 40)
                    return 10;
                else if (level >= levelPenalty + 30)
                    expPenalty = 0.2;
                else if (level >= levelPenalty + 20)
                    expPenalty = 0.4;
                else if (level >= levelPenalty + 10)
                    expPenalty = 0.6;
                else if (level >= levelPenalty)
                    expPenalty = 0.8;

                // 리더 보너스
                double leaderBonus = 1.0;
                if (doll.location == 1)
                    leaderBonus = 1.2;

                // 개조 시 서약 보너스
                double marriedBonus = 1.0;
                if (doll.married == true && 
                    doll.mod > 0 && 
                    ignoreMarriedBonus == false)
                    marriedBonus = 2.0;

                // 링크 보너스
                double linkBonus = 1.0;
                switch (doll.maxLink)
                {
                    case 2:
                        linkBonus = 1.5;
                        break;
                    case 3:
                        linkBonus = 2.0;
                        break;
                    case 4:
                        linkBonus = 2.5;
                        break;
                    case 5:
                        linkBonus = 3.0;
                        break;
                    default:
                        break;
                }

                // 지휘관 보너스
                double commanderBonus = 1.0;
                if (Config.Costume.coBonusDollExp)
                {
                    commanderBonus = Convert.ToDouble(Config.Costume.coBonusDollExpPercent) / 100 + 1.0;
                }

                // 경험치 업 보너스
                double expBonusEvent = 1.0;
                if (Config.Echelon.expUpEvent)
                {
                    expBonusEvent = 1.5;
                }

                return Convert.ToInt32(Math.Truncate(baseExp * linkBonus * leaderBonus * expPenalty * marriedBonus * commanderBonus * expBonusEvent));
            }

            /// <summary>
            /// 인형 코어수 가져오기
            /// </summary>
            /// <param name="star"></param>
            /// <returns></returns>
            public static int GetCore(int star)
            {
                switch(star)
                {
                    case 3:
                        return 1;
                    case 4:
                        return 3;
                    case 5:
                        return 5;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// 장비
        /// </summary>
        public class Equip
        {
            /// <summary>
            /// 장비 정보 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static JObject GetData(int id)
            {
                //string sql = string.Format("SELECT * FROM equip WHERE id = {0} LIMIT 1", id);
                //return GetDb(sql, "GFData");

                JObject json = GetJsonDb("equip");
                string key = id.ToString();
                if (json.ContainsKey(key))
                {
                    return json[key].Value<JObject>();
                }
                return new JObject();
            }

            /// <summary>
            /// 장비 이름 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static string GetName(int id)
            {
                string name = LanguageResources.Instance[string.Format("EQUIP_{0}", id)];
                return name;
            }
        }

        /// <summary>
        /// 임무
        /// </summary>
        public class Quest
        {
            /// <summary>
            /// 임무 정보 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <param name="col"></param>
            /// <returns></returns>
            public static JObject GetData(int id)
            {
                //string sql = string.Format("SELECT * FROM quest WHERE id = {0} LIMIT 1", id);
                //return GetDb(sql, "GFData");

                JObject json = GetJsonDb("quest");
                string key = id.ToString();
                if (json.ContainsKey(key))
                {
                    return json[key].Value<JObject>();
                }
                return new JObject();
            }
        }

        /// <summary>
        /// 탐색
        /// </summary>
        public class Explore
        {
            /// <summary>
            /// 탐색 보상
            /// 메인 보상 | 서브 보상 | 부정확한 정보 여부
            /// </summary>
            private static Dictionary<int, int[]> reward = new Dictionary<int, int[]>()
            {
                #region explore_reward
                { 1, new int[] { 150, 15, 1 } },
                { 2, new int[] { 150, 15, 1 } },
                { 3, new int[] { 150, 15, 1 } },
                { 4, new int[] { 150, 15, 1 } },
                { 5, new int[] { 150, 15, 1 } },
                { 6, new int[] { 150, 16, 0 } },
                { 7, new int[] { 160, 17, 0 } },
                { 8, new int[] { 170, 18, 0 } },
                { 9, new int[] { 180, 20, 0 } },
                { 10, new int[] { 185, 20, 0 } },
                { 11, new int[] { 195, 20, 0 } },
                { 12, new int[] { 198, 20, 1 } },
                { 13, new int[] { 201, 21, 1 } },
                { 14, new int[] { 204, 21, 1 } },
                { 15, new int[] { 207, 21, 1 } },
                { 16, new int[] { 210, 21, 1 } },
                { 17, new int[] { 215, 22, 1 } },
                { 18, new int[] { 220, 22, 0 } },
                { 19, new int[] { 240, 23, 0 } },
                { 20, new int[] { 255, 24, 0 } },
                #endregion
            };

            /// <summary>
            /// 탐색 보상 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static int[] GetReward(int id)
            {
                if (reward.ContainsKey(id))
                    return reward[id];
                return new int[] { 0, 0, 0 };
            }
        }

        /// <summary>
        /// 거점
        /// </summary>
        public class Spot
        {
            /// <summary>
            /// 정상 퇴각 여부 확인
            /// </summary>
            /// <param name="currentTurn"></param>
            /// <param name="cycle"></param>
            /// <returns></returns>
            public static bool IsProperWithdraw(int spotId)
            {
                // "1,1"
                // 1턴 닫힘
                // 2턴 열림
                // 3턴 닫힘
                // 4턴 열림
                // 5턴 닫힘
                // 6턴 열림

                // "2,1"
                // 1턴 닫힘 2
                // 2턴 닫힘 1
                // 3턴 열림 0
                // 4턴 닫힘 2
                // 5턴 닫힘 1
                // 6턴 열림 0

                // "3,1"
                //  1턴 닫힘 3
                //  2턴 닫힘 2
                //  3턴 닫힘 1
                //  4턴 열림
                //  5턴 닫힘 3
                //  6턴 닫힘 2
                //  7턴 닫힘 1
                //  8턴 열림 
                //  9턴 닫힘 3
                // 10턴 닫힘 2
                // 11턴 닫힘 1
                // 12턴 열림

                try
                {
                    JObject spot = GameData.Spot.GetSpotData(spotId);
                    int belong = UserData.CurrentMission.GetBelongSpot(spotId);
                    if (belong != 1) // 그리폰 점렴 상태 아니면 탄식 소실
                        return false;
                    int type = Parser.Json.ParseInt(spot["type"]);
                    switch (type)
                    {
                        case 1: // 지휘부
                            return true;
                        case 3: // 헬리포트
                        case 7: // 대형헬리포트
                            return IsOpenHeliport(spot);
                        case 2: // 일반거점
                        case 5: // 보급품
                        case 6: // 보급로
                        default:
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "헬리포트 열림 여부 가져오는 도중 에러 발생 - 위치 {0}", spotId);
                }
                return true;
            }

            /// <summary>
            /// 헬리포트 열림 여부
            /// </summary>
            /// <param name="spot"></param>
            /// <returns></returns>
            public static bool IsOpenHeliport(JObject spot)
            {
                int currentTurn = UserData.CurrentMission.turnCount;
                string[] activeCycle = Parser.Json.ParseString(spot["active_cycle"]).Split(',');
                int cycle = 0;
                int duration = 0;
                if (activeCycle.Length == 2)
                {
                    cycle = Parser.String.ParseInt(activeCycle[0]);
                    duration = Parser.String.ParseInt(activeCycle[1]);
                }

                if (cycle > 0)
                {
                    currentTurn = currentTurn - 1;
                    currentTurn = currentTurn % (cycle + duration);
                    if (currentTurn >= cycle)
                        return true;
                    return false;
                }
                return true;
            }

            /// <summary>
            /// 거점 정보 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <param name="col"></param>
            /// <returns></returns>
            public static JObject GetSpotData(int id)
            {
                //string sql = string.Format("SELECT * FROM gfdb_spot WHERE id = {0} LIMIT 1", id);
                //return GetDb(sql, "GFData");

                JObject json = GetJsonDb("gfdb_spot");
                string key = id.ToString();
                if (json.ContainsKey(key))
                {
                    return json[key].Value<JObject>();
                }
                return new JObject();
            }
        }

        #region Json Functions

        static Dictionary<string, JObject> jsonDb = new Dictionary<string, JObject>();

        private static JObject GetJsonDb(string db)
        {
            if (jsonDb.ContainsKey(db))
                return jsonDb[db];

            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string file = string.Format("{0}/Resource/db/{1}.json", dir, db);
            if (File.Exists(file))
            {
                try
                {
                    string str = CompressUtil.Decompress(FileUtil.GetFile(file));
                    JObject json = JObject.Parse(str);
                    jsonDb.Add(db, json);
                    return jsonDb[db];
                }
                catch { }
            }
            return new JObject();
        }

        #endregion

        #region Sqlite Functions

        /*
        private static volatile Dictionary<string, SQLiteConnection> conns = new Dictionary<string, SQLiteConnection>();
        private static object syncLock = new Object();

        private ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 연결 가져오기
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static SQLiteConnection GetConnection(string dbName)
        {
            if (!conns.ContainsKey(dbName))
            {
                string relativePath = string.Format("Resource/{0}.db", dbName);
                string currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                string absolutePath = Path.Combine(currentPath, relativePath);
                absolutePath = absolutePath.Remove(0, 6); //this code is written to remove file word from absolute path
                if (File.Exists(absolutePath))
                {
                    string connString = string.Format("Data Source={0};Version=3", absolutePath);
                    conns.Add(dbName, new SQLiteConnection(connString));
                }
            }
            return conns[dbName];
        }

        public static string GetDb(string sql, string col, string dbName)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            SQLiteDataReader rdr = null;

            string value = "";

            lock (syncLock)
            {
                try
                {
                    conn = GetConnection(dbName);
                    conn.Open();
                    cmd = new SQLiteCommand(sql, conn);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        value = rdr[col].ToString();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "데이터베이스 에러 {0}", sql);
                }
                finally
                {
                    if (rdr != null) { rdr.Close(); rdr = null; }
                    if (conn != null) { conn.Close(); conn = null; }
                }
            }
            return value;
        }

        public static List<JObject> GetDbs(string sql, string dbName)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            SQLiteDataReader rdr = null;

            List<JObject> values = new List<JObject>();
            lock (syncLock)
            {
                try
                {
                    conn = GetConnection(dbName);
                    conn.Open();
                    cmd = new SQLiteCommand(sql, conn);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        JObject value = new JObject();
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            string col = rdr.GetName(i);
                            value.Add(col, rdr[col].ToString());
                        }
                        values.Add(value);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                finally
                {
                    if (rdr != null) { rdr.Close(); rdr = null; }
                    if (conn != null) { conn.Close(); conn = null; }
                }
            }
            return values;
        }

        public static JObject GetDb(string sql, string dbName)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            SQLiteDataReader rdr = null;

            JObject values = new JObject();
            lock (syncLock)
            {
                try
                {
                    conn = GetConnection(dbName);
                    conn.Open();
                    cmd = new SQLiteCommand(sql, conn);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            string col = rdr.GetName(i);
                            values.Add(col, rdr[col].ToString());
                        }
                        break;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "데이터베이스 에러 {0}", sql);
                }
                finally
                {
                    if (rdr != null) { rdr.Close(); rdr = null; }
                    if (conn != null) { conn.Close(); conn = null; }
                }
            }
            return values;
        }

        /// <summary>
        /// 연결 닫기
        /// </summary>
        public void CloseConnection()
        {
            foreach (var item in conns)
                conns[item.Key].Close();
        }
        */

        #endregion
    }
}
