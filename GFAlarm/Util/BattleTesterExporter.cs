using GFAlarm.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Util
{
    public class BattleTesterExporter
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        // 제대 정보
        public static Dictionary<int, List<JObject>> echelons = new Dictionary<int, List<JObject>>()
        {
            { 1, new List<JObject>() },
            { 2, new List<JObject>() },
            { 3, new List<JObject>() },
            { 4, new List<JObject>() },
            { 5, new List<JObject>() },
            { 6, new List<JObject>() },
            { 7, new List<JObject>() },
            { 8, new List<JObject>() },
            { 9, new List<JObject>() },
            { 10, new List<JObject>() },
        };

        // 장비 정보
        public static Dictionary<int, List<JObject>> equips = new Dictionary<int, List<JObject>>()
        {
            { 1, new List<JObject>() },
            { 2, new List<JObject>() },
            { 3, new List<JObject>() },
            { 4, new List<JObject>() },
            { 5, new List<JObject>() },
            { 6, new List<JObject>() },
            { 7, new List<JObject>() },
            { 8, new List<JObject>() },
            { 9, new List<JObject>() },
            { 10, new List<JObject>() },
        };

        /// <summary>
        /// 제대 정보 추출
        /// </summary>
        /// <param name="json"></param>
        public static void ExportEchelonInfo(JObject json)
        {
            if (json.ContainsKey("gun_with_user_info") && json["gun_with_user_info"] is JArray)
            {
                log.Debug("전투 테스터용 제대 프리셋 저장 시도...");

                /// 제대 정보 초기화
                foreach (KeyValuePair<int, List<JObject>> item in echelons)
                    item.Value.Clear();

                JArray items = json["gun_with_user_info"].Value<JArray>();
                foreach (JObject item in items)
                {
                    int team_id = Parser.Json.ParseInt(item["team_id"]);
                    if (1 <= team_id && team_id <= 10)
                    {
                        string gun_with_user_id = Parser.Json.ParseString(item["id"]);

                        echelons[team_id].Add(item);
                    }
                }

                // 프리셋 저장
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                Directory.CreateDirectory(path);
                foreach (KeyValuePair<int, List<JObject>> item in echelons)
                {
                    JObject preset = new JObject();
                    int teamId = item.Key;
                    foreach (JObject gun in item.Value)
                    {
                        int location = Parser.Json.ParseInt(gun["location"]);
                        gun["id"] = (JToken)location;
                        //gun["favor"] = (JToken)100000;
                        //gun["max_favor"] = (JToken)2000000;
                        //gun["favor_toplimit"] = (JToken)2000000;

                        preset.Add("gun" + location, gun);
                    }
                    preset.Add("enemyGroupID", "1");
                    preset.Add("BossHP", "0");
                    string filename = string.Format(path + "\\{0}_{1}_battle_tester_echelon_preset_{2}.gun", UserData.name, UserData.uid, teamId);
                    File.WriteAllText(filename, preset.ToString(Formatting.None), Encoding.Default);
                    log.Debug("전투 테스터 제대 프리셋 정보 파일로 저장 (battle_tester_echelon_preset_{0}.gun)", teamId);
                }
            }
            else
            {
                log.Warn("전투 테스터용 제대 프리셋 저장 실패 - 제대 정보 없음");
            }
        }

        // 중장비 키 값들
        public static string[] squad_keys = new string[] { "227", "7615", "7138", "24369", "28011" };
        public static string[] origin_squad_keys = new string[] { "", "", "", "", "", "", "", "" };
        

        /// <summary>
        /// 중장비 정보 추출
        /// </summary>
        /// <param name="json"></param>
        public static void ExportSquadInfo(JObject json)
        {
            if (json.ContainsKey("squad_with_user_info") && json["squad_with_user_info"] is JObject)
            {
                log.Info("전투 테스터용 중장비 프리셋 저장 시도...");

                // 중장비 정보 가져오기
                Dictionary<string, string> items = Parser.Json.ParseItems(json["squad_with_user_info"].Value<JObject>());
                JObject preset = new JObject();
                foreach (KeyValuePair<string, string> item in items)
                {
                    JObject squad = Parser.Json.ParseJObject(item.Value);
                    int idx = Parser.Json.ParseInt(squad["squad_id"]);
                    origin_squad_keys[idx - 1] = item.Key;
                    switch (idx)
                    {
                        case 1:
                            squad["id"] = "227";
                            break;
                        case 2:
                            squad["id"] = "7615";
                            break;
                        case 3:
                            squad["id"] = "7138";
                            break;
                        case 4:
                            squad["id"] = "24369";
                            break;
                        case 5:
                            squad["id"] = "28011";
                            break;
                    }
                    squad["ammo"] = "1000";
                    squad["mre"] = "1000";

                    preset.Add("Squad" + idx, squad);
                }
                preset.Add("switch1", 1);
                preset.Add("switch2", 1);
                preset.Add("switch3", 1);
                preset.Add("switch4", 1);
                preset.Add("switch5", 1);

                // 프리셋 저장
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                Directory.CreateDirectory(path);
                string filename = string.Format(path + "\\{0}_{1}_battle_tester_squad_preset.sqd", UserData.name, UserData.uid);
                File.WriteAllText(filename, preset.ToString(Formatting.None), Encoding.Default);
                log.Info("전투 테스터 중장비 프리셋 정보 파일로 저장 (battle_tester_squad_preset.sqd)");
            }
        }

        /// <summary>
        /// 칩셋 정보 추출
        /// </summary>
        /// <param name="json"></param>
        public static void ExportChipInfo(JObject json)
        {
            if (json.ContainsKey("chip_with_user_info") && json["chip_with_user_info"] is JObject)
            {
                log.Info("전투 테스터용 칩셋 정보 저장 시도...");

                Dictionary<string, string> items = Parser.Json.ParseItems(json["chip_with_user_info"].Value<JObject>());
                JObject preset = new JObject();
                foreach (KeyValuePair<string, string> item in items)
                {
                    JObject chip = Parser.Json.ParseJObject(item.Value);
                    string squad_with_user_id = Parser.Json.ParseString(chip["squad_with_user_id"]);
                    if (squad_with_user_id != "0" && origin_squad_keys.Contains(squad_with_user_id))
                    {
                        int idx = Array.IndexOf(origin_squad_keys, squad_with_user_id);
                        chip["squad_with_user_id"] = squad_keys[idx];
                        chip["user_id"] = "20139";
                        preset.Add(item.Key, chip);
                    }
                }

                string filename = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\BattleTester\\userinfo.json";
                if (File.Exists(filename))
                {
                    JObject user_info = JObject.Parse(File.ReadAllText(filename));
                    if (user_info.ContainsKey("chip_with_user_info"))
                    {
                        user_info["chip_with_user_info"] = preset;

                        // 프리셋 저장
                        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                        Directory.CreateDirectory(path);
                        string filename2 = string.Format(path + "\\userinfo.json");
                        File.WriteAllText(filename2, user_info.ToString(Formatting.None), Encoding.UTF8);
                        log.Info("전투 테스터 사용자 정보 파일로 저장 (userinfo.json)");
                    }
                }

                // 프리셋 저장
                //string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                //Directory.CreateDirectory(path);
                //string filename = string.Format(path + "\\{0}_{1}_battle_tester_chip_preset.chip", UserData.name, UserData.userId);
                //File.WriteAllText(filename, preset.ToString(Formatting.None), Encoding.Default);
                //log.Info("전투 테스터 칩셋 프리셋 정보 파일로 저장 (battle_tester_chip_preset.chip)");
            }
        }

        /// <summary>
        /// 장비 정보 추출
        /// </summary>
        /// <param name="json"></param>
        public static void ExportEquipInfo(JObject json)
        {
            if (json.ContainsKey("equip_with_user_info") && json["equip_with_user_info"] is JObject)
            {
                log.Info("전투 테스터용 장비 정보 저장 시도...");

                JObject equip_with_user_info = json["equip_with_user_info"].Value<JObject>();
                foreach (KeyValuePair<int, List<JObject>> item in echelons)
                {
                    JObject preset = new JObject();
                    foreach (JObject doll in item.Value)
                    {
                        int team_id = Parser.Json.ParseInt(doll["team_id"]);
                        int location = Parser.Json.ParseInt(doll["location"]);
                        string equip1 = Parser.Json.ParseString(doll["equip1"]);
                        string equip2 = Parser.Json.ParseString(doll["equip2"]);
                        string equip3 = Parser.Json.ParseString(doll["equip3"]);

                        if (equip_with_user_info.ContainsKey(equip1) && equip_with_user_info[equip1] is JObject) 
                        {
                            JObject equip = equip_with_user_info[equip1].Value<JObject>();
                            string id = string.Format("{0}", location * 10 + 1);
                            equip["id"] = id;
                            equip["gun_with_user_id"] = string.Format("{0}", location);
                            preset.Add(id, equip);
                        }
                        if (equip_with_user_info.ContainsKey(equip2) && equip_with_user_info[equip2] is JObject)
                        {
                            JObject equip = equip_with_user_info[equip2].Value<JObject>();
                            string id = string.Format("{0}", location * 10 + 2);
                            equip["id"] = id;
                            equip["gun_with_user_id"] = string.Format("{0}", location);
                            preset.Add(id, equip);
                        }
                        if (equip_with_user_info.ContainsKey(equip3) && equip_with_user_info[equip3] is JObject)
                        {
                            JObject equip = equip_with_user_info[equip3].Value<JObject>();
                            string id = string.Format("{0}", location * 10 + 3);
                            equip["id"] = id;
                            equip["gun_with_user_id"] = string.Format("{0}", location);
                            preset.Add(id, equip);
                        }
                    }

                    // 프리셋 저장
                    string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                    Directory.CreateDirectory(path);
                    string filename = string.Format(path + "\\{0}_{1}_battle_tester_equip_preset_{2}.equip", UserData.name, UserData.uid, item.Key);
                    File.WriteAllText(filename, preset.ToString(Formatting.None), Encoding.Default);
                    log.Info("전투 테스터 장비 프리셋 정보 파일로 저장 (battle_tester_equip_preset.equip)");
                }
            }
        }
    }
}
