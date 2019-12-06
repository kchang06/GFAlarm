using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GFDataParser
{
    class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region 스킨 DB 관련

        /// <summary>
        /// 스킨 클래스
        /// </summary>
        class Skin
        {
            public int gun_id = 0;
            public string gun_name = "";
            public int skin_id = 0;
            public int is_child = 0;
            public int is_live2d = 0;
        }

        static void CreateGFSkinScript()
        {
            StringBuilder sb = new StringBuilder();

            /// ========================================
            /// 인형 스킨 정보 (list.txt)
            /// ========================================
            sb.Append("DELETE FROM skin; \n");
            string[] lines = File.ReadAllLines("input/file/list.txt");
            // [랖투디 스킨 번호] 가져오기
            foreach (string line in lines)
            {
                if (line.StartsWith("live2d_gun_") && line.LastIndexOf('_') > 0)
                {
                    string temp = line.Substring(line.LastIndexOf('_') + 1).Replace(".ab", "");
                    int skinId = 0;
                    Int32.TryParse(temp, out skinId);
                    if (skinId > 0)
                        live2dSkin.Add(new Skin() { skin_id = skinId });
                }
            }
            // 모든 [인형 번호 + 스킨 번호] 가져오기
            HashSet<string> keyValues = new HashSet<string>();
            foreach (string item in lines)
            {
                // 인형
                if (item.StartsWith("character_"))
                {
                    string name = item.Substring(item.IndexOf("character_") + "character_".Length);
                    name = name.Replace("_spine.ab", "").Replace("_he.ab", "").Replace("_nom.ab", "").Replace(".ab", "").Replace("_noarmor", "").Replace("_elfeldt", "").Replace("_noel", "").Replace("_motor", "").Replace("_boss", "").Replace("_ogas", "");
                    int gunId = GetGunId(name);
                    if (gunId > 0)
                    {
                        string[] names = name.Split('_');
                        if (names.Length == 2)
                            keyValues.Add(string.Format("{0}_{1}", gunId, names[1]));
                    }
                }
                // 카리나
                if (item.StartsWith("character_npc_kalina_"))
                {
                    string name = item.Replace("character_npc_kalina_", "").Replace("_spine.ab", "").Replace(".ab", "");
                    int skinId = 0;
                    int.TryParse(name, out skinId);
                    if (skinId > 0)
                    {
                        keyValues.Add("-1_" + skinId);
                    }
                }
            }

            List<Skin> skins = new List<Skin>();
            foreach (string item in keyValues)
            {
                string[] temp = item.Split('_');
                int gunId = Int32.Parse(temp[0]);
                int skinId = Int32.Parse(temp[1]);
                int isChild = 0;
                if (IsChildSkin(skinId))
                {
                    isChild = 1;
                }
                int isLive2d = 0;
                if (IsLive2dSkin(skinId))
                {
                    isLive2d = 1;
                }
                skins.Add(new Skin()
                {
                    gun_id = gunId,
                    skin_id = skinId,
                    is_child = isChild,
                    is_live2d = isLive2d,
                });
            }
            foreach (Skin skin in skins)
            {
                sb.Append(string.Format("INSERT OR REPLACE INTO skin (gun_id, skin_id, is_child, is_live2d) VALUES ('{0}', '{1}', '{2}', '{3}'); \n", skin.gun_id, skin.skin_id, skin.is_child, skin.is_live2d));
            }
            log.Debug("스킨 정보 {0}건", skins.Count());
            File.WriteAllText("output/skin.sql", sb.ToString());
        }

        /// <summary>
        /// 랖투디 스킨 여부
        /// </summary>
        /// <param name="skinId"></param>
        /// <returns></returns>
        static bool IsLive2dSkin(int skinId)
        {
            foreach (Skin skin in live2dSkin)
            {
                if (skin.skin_id == skinId)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 랖투디 스킨
        /// </summary>
        static List<Skin> live2dSkin = new List<Skin>();

        /// <summary>
        /// 아동절 스킨 여부
        /// </summary>
        /// <param name="skinId"></param>
        /// <returns></returns>
        static bool IsChildSkin(int skinId)
        {
            foreach (Skin skin in childSkin)
            {
                if (skin.skin_id == skinId)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 아동절 스킨
        /// </summary>
        static List<Skin> childSkin = new List<Skin>()
        {
            new Skin() { gun_id=197, skin_id=2201 },
            new Skin() { gun_id=198, skin_id=2202 },
            new Skin() { gun_id=213, skin_id=2205 },
            new Skin() { gun_id=142, skin_id=2203 },
            new Skin() { gun_id=64, skin_id=2806 },
            new Skin() { gun_id=64, skin_id=905 },
            new Skin() { gun_id=96, skin_id=903 },
            new Skin() { gun_id=65, skin_id=3401 },
            new Skin() { gun_id=236, skin_id=3402 },
            new Skin() { gun_id=252, skin_id=3405 },
            new Skin() { gun_id=50, skin_id=901 },
            new Skin() { gun_id=25, skin_id=902 },
            new Skin() { gun_id=112, skin_id=904 },
            new Skin() { gun_id=145, skin_id=3406 },
            new Skin() { gun_id=136, skin_id=2204 },
            new Skin() { gun_id=202, skin_id=2206 },
            new Skin() { gun_id=103, skin_id=3403 },
            new Skin() { gun_id=101, skin_id=3404 },
        };

        /// <summary>
        /// 인형 도감번호 가져오기
        /// </summary>
        static JObject gun_info = null;
        static int GetGunId(string gunName)
        {
            if (gun_info == null)
                gun_info = JObject.Parse(File.ReadAllText("input/gfdb/gun_info.json"));
            string[] ids = gun_info.Properties().Select(p => p.Name).ToArray();
            foreach (string id in ids)
            {
                string code = gun_info[id]["code"].Value<string>().ToLower();
                if (gunName.Contains("_"))
                    gunName = gunName.Substring(0, gunName.IndexOf("_"));
                if (gunName == code)
                    return gun_info[id]["id"].Value<int>();
            }
            return -1;
        }

        #endregion

        #region 철혈시트 DB 관련

        static void CreateGFDBScript()
        {
            StringBuilder sb = new StringBuilder();

            /// ========================================
            /// 거점 정보 (spot_info.json)
            /// ========================================
            sb.Append("DELETE FROM gfdb_spot; \n");
            List<JObject> spot_info = GetJObjectList(JObject.Parse(File.ReadAllText("input/gfdb/spot_info.json")));
            log.Debug("거점 정보 (spot_info.json) {0}건", spot_info.Count());
            foreach (JObject item in spot_info)
            {
                int id = item["id"].Value<int>();
                int mission_id = item["mission_id"].Value<int>();
                int type = item["type"].Value<int>();
                int special_eft = item["special_eft"].Value<int>();
                string route = item["route"].Value<string>();
                int coord_x = item["coordinator_x"].Value<int>();
                int coord_y = item["coordinator_y"].Value<int>();
                int belong = item["belong"].Value<int>();
                int random_get = item["random_get"].Value<int>();
                int durability = item["durability"].Value<int>();
                string active_cycle = item["active_cycle"].Value<string>();

                int enemy_team_id = item["enemy_team_id"].Value<int>();
                int ally_team_id = item["ally_team_id"].Value<int>();
                int building_id = item["building_id"].Value<int>();
                string hostage_info = item["hostage_info"].Value<string>();

                sb.Append(string.Format("INSERT OR REPLACE INTO gfdb_spot " +
                    "(id, mission_id, type, special_eft, route, coord_x, coord_y, belong, random_get, durability, active_cycle, enemy_team_id, ally_team_id, building_id, hostage_info) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}'); \n",
                    id, mission_id, type, special_eft, route, coord_x, coord_y, belong, random_get, durability, active_cycle, enemy_team_id, ally_team_id, building_id, hostage_info));
            }
            if (File.Exists("output/spot_info.sql"))
                File.Delete("output/spot_info.sql");
            File.WriteAllText("output/spot_info.sql", sb.ToString());
            sb.Clear();

            /// ========================================
            /// 전역 정보 (mission_info.json)
            /// ========================================
            sb.Append("DELETE FROM gfdb_mission; \n");
            List<JObject> mission_info = GetJObjectList(JObject.Parse(File.ReadAllText("input/gfdb/mission_info.json")));
            log.Debug("전역 정보 (mission_info.json) {0}건", mission_info.Count());
            foreach (JObject item in mission_info)
            {
                int id = item["id"].Value<int>();
                string map_res_name = item["map_res_name"].Value<string>();
                int map_all_width = item["map_all_width"].Value<int>();
                int map_all_height = item["map_all_height"].Value<int>();
                int map_eff_width = item["map_eff_width"].Value<int>();
                int map_eff_height = item["map_eff_height"].Value<int>();
                int map_offset_x = item["map_offset_x"].Value<int>();
                int map_offset_y = item["map_offset_y"].Value<int>();

                sb.Append(string.Format("INSERT OR REPLACE INTO gfdb_mission (id, map_res_name, map_all_width, map_all_height, map_eff_width, map_eff_height, map_offset_x, map_offset_y) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}'); \n",
                    id, map_res_name, map_all_width, map_all_height, map_eff_width, map_eff_height, map_offset_x, map_offset_y));
            }

            if (File.Exists("output/mission_info.sql"))
                File.Delete("output/mission_info.sql");
            File.WriteAllText("output/mission_info.sql", sb.ToString());
            sb.Clear();

            /// ========================================
            /// 적 정보 (enemy_team_info.json)
            /// ========================================
            sb.Append("DELETE FROM gfdb_enemy_team; \n");
            Dictionary<int, JObject> enemy_character_type_info = GetJObjectDictionary(JObject.Parse(File.ReadAllText("input/gfdb/enemy_character_type_info.json")));
            Dictionary<int, JObject> enemy_in_team_info = GetJObjectDictionary(JObject.Parse(File.ReadAllText("input/gfdb/enemy_in_team_info.json")));
            List<JObject> enemy_team_info = GetJObjectList(JObject.Parse(File.ReadAllText("input/gfdb/enemy_team_info.json")));
            log.Debug("적 정보 (enemy_character_type_info.json) {0}건", enemy_character_type_info.Count());
            log.Debug("적 정보 (enemy_in_team_info.json) {0}건", enemy_in_team_info.Count());
            log.Debug("적 정보 (enemy_team_info.json) {0}건", enemy_team_info.Count());
            foreach (JObject item in enemy_team_info)
            {
                int id = item["id"].Value<int>();
                int enemy_leader = item["enemy_leader"].Value<int>();
                string enemyLeaderName = "";
                if (enemy_character_type_info.ContainsKey(enemy_leader))
                {
                    enemyLeaderName = enemy_character_type_info[enemy_leader]["code"].Value<string>();
                }
                int[] member_ids = item["member_ids"].Value<JArray>().Select(p => (int)p).ToArray();
                List<string> member_names = new List<string>();
                foreach (int member_id in member_ids)
                {
                    if (enemy_in_team_info.ContainsKey(member_id))
                    {
                        int enemy_character_type_id = enemy_in_team_info[member_id]["enemy_character_type_id"].Value<int>();
                        if (enemy_character_type_info.ContainsKey(enemy_character_type_id))
                        {
                            string code = enemy_character_type_info[enemy_character_type_id]["code"].Value<string>();
                            member_names.Add(code);
                        }
                    }
                }
                string memberNames = string.Join(",", member_names);

                sb.Append(string.Format("INSERT OR REPLACE INTO gfdb_enemy_team (id, leader, members) VALUES ('{0}', '{1}', '{2}'); \n", id, enemyLeaderName, memberNames));
            }

            if (File.Exists("output/enemy_team.sql"))
                File.Delete("output/enemy_team.sql");
            File.WriteAllText("output/enemy_team.sql", sb.ToString());
            sb.Clear();

            /// ========================================
            /// 건물 정보 (building_info.json)
            /// ========================================
            sb.Append("DELETE FROM gfdb_building; \n");
            List<JObject> building_info = GetJObjectList(JObject.Parse(File.ReadAllText("input/gfdb/building_info.json")));
            log.Debug("건물 정보 (building_info.json) {0}건", building_info.Count());
            foreach (JObject item in building_info)
            {
                int id = item["id"].Value<int>();
                string code = item["code"].Value<string>();
                int belong = item["belong"].Value<int>();
                int hold_belong = item["hold_belong"].Value<int>();

                sb.Append(string.Format("INSERT OR REPLACE INTO gfdb_building (id, code, belong, hold_belong) VALUES ('{0}', '{1}', '{2}', '{3}'); \n",
                    id, code, belong, hold_belong));
            }

            if (File.Exists("output/building.sql"))
                File.Delete("output/building.sql");
            File.WriteAllText("output/building.sql", sb.ToString());
            sb.Clear();

            /// ========================================
            /// 아군 정보 (ally_team_info.json)
            /// ========================================
            sb.Append("DELETE FROM gfdb_ally_team; \n");
            List<JObject> ally_team_info = GetJObjectList(JObject.Parse(File.ReadAllText("input/gfdb/ally_team_info.json")));
            log.Debug("아군 정보 (ally_team_info.json) {0}건", ally_team_info.Count());
            foreach (JObject item in ally_team_info)
            {
                int id = item["id"].Value<int>();
                string code = item["code"].Value<string>();
                string guns = item["guns"].Value<string>();
                int leader_id = item["leader_id"].Value<int>();
                string icon = item["icon"].Value<string>();

                sb.Append(string.Format("INSERT OR REPLACE INTO gfdb_ally_team (id, code, guns, leader_id, icon) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}'); \n",
                    id, code, guns, leader_id, icon));
            }

            if (File.Exists("output/ally_team.sql"))
                File.Delete("output/ally_team.sql");
            File.WriteAllText("output/ally_team.sql", sb.ToString());
            sb.Clear();

            // 폴더 열기
            Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/output");
        }

        #endregion

        static void Main(string[] args)
        {
            #region 로그 설정
            var config = new LoggingConfiguration();

            var logconsole = new ColoredConsoleTarget("logconsole");
            //logconsole.Layout = new SimpleLayout() { Text = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} ${level:uppercase=true:padding=-5} ${message} ${exception:format=tostring}" };
            logconsole.Layout = new SimpleLayout() { Text = "${message} ${exception:format=tostring}" };
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
            #endregion

            CreateGFDBScript();
            CreateGFSkinScript();
        }

        #region Function

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        static List<JObject> GetJObjectList(JObject json)
        {
            List<JObject> list = new List<JObject>();
            string[] keys = json.Properties().Select(p => p.Name).ToArray();
            foreach (string key in keys)
            {
                JObject item = json[key].Value<JObject>();
                list.Add(item);
            }
            return list;
        }

        static Dictionary<int, JObject> GetJObjectDictionary(JObject json)
        {
            Dictionary<int, JObject> dictionary = new Dictionary<int, JObject>();
            string[] keys = json.Properties().Select(p => p.Name).ToArray();
            foreach (string key in keys)
            {
                JObject item = json[key].Value<JObject>();
                int id = item["id"].Value<int>();
                dictionary.Add(id, item);
            }
            return dictionary;
        }

        #endregion
    }
}
