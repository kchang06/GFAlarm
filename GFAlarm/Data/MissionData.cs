using GFAlarm.Data.Element;
using GFAlarm.Util;
using Newtonsoft.Json.Linq;
using NLog;
using System.Collections.Generic;

namespace GFAlarm.Data
{
    public class MissionData
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static int mission_id = -1;
        public static string location = "Unknown";

        public static Dictionary<int, SpotInfo> spot_infos = new Dictionary<int, SpotInfo>();

        /// <summary>
        /// 전역 생성
        /// </summary>
        /// <param name="mission_id"></param>
        public static void InitMission(int mission_id)
        {

        }

        /// <summary>
        /// 전역 반영
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void ReflectMission(string uri, JObject request, JObject response)
        {
            if (request != null && response != null)
            {
                // 전역시작
                // ("Mission/startMission")
                if (uri.EndsWith("Mission/startMission"))
                {
                    #region Packet Example
                    // request
                    /*
                    {
                        "mission_id": 10076,
                        "spots": [
                            {
                                "spot_id": 6005,
                                "team_id": 3
                            }
                        ],
                        "squad_spots": [],
                        "ally_id": 1569310407
                    }
                     */
                    // response
                    /*
                    {
                       "night_spots": [
                           {
                               "spot_id": "6006",
                               "belong": "1",
                               "seed": 1665
                           },
                           {
                               "spot_id": "6007",
                               "belong": "1",
                               "seed": 4574
                           },
                           {
                               "spot_id": "6008",
                               "belong": "2",
                               "seed": 5473
                           },
                           {
                               "spot_id": "6009",
                               "belong": "2",
                               "seed": 413
                           },
                           {
                               "spot_id": "6010",
                               "belong": "1",
                               "seed": 8523
                           },
                           {
                               "spot_id": "6011",
                               "belong": "1",
                               "seed": 8483
                           },
                           {
                               "spot_id": "6012",
                               "belong": "3",
                               "seed": 9118
                           },
                           {
                               "spot_id": "6013",
                               "belong": "2",
                               "seed": 2376
                           },
                           {
                               "spot_id": "6014",
                               "belong": "2",
                               "seed": 4299
                           },
                           {
                               "spot_id": "6015",
                               "belong": "1",
                               "seed": 7040
                           },
                           {
                               "spot_id": "6016",
                               "belong": "1",
                               "seed": 4653
                           },
                           {
                               "spot_id": "6017",
                               "belong": "3",
                               "seed": 5124
                           },
                           {
                               "spot_id": "6018",
                               "belong": "2",
                               "seed": 4612
                           },
                           {
                               "spot_id": "6019",
                               "belong": "2",
                               "seed": 2665
                           },
                           {
                               "spot_id": "6020",
                               "belong": "1",
                               "seed": 3600
                           },
                           {
                               "spot_id": "6021",
                               "belong": "3",
                               "seed": 6153
                           },
                           {
                               "spot_id": "6022",
                               "belong": "3",
                               "seed": 9378
                           },
                           {
                               "spot_id": "6023",
                               "belong": "3",
                               "seed": 3628
                           },
                           {
                               "spot_id": "6024",
                               "belong": "2",
                               "seed": 4149
                           },
                           {
                               "spot_id": "6025",
                               "belong": "2",
                               "seed": 4538
                           },
                           {
                               "spot_id": "6026",
                               "belong": "2",
                               "seed": 7931
                           },
                           {
                               "spot_id": "6027",
                               "belong": "2",
                               "seed": 640
                           },
                           {
                               "spot_id": "6028",
                               "belong": "2",
                               "seed": 7529
                           },
                           {
                               "spot_id": "6029",
                               "belong": "2",
                               "seed": 1894
                           },
                           {
                               "spot_id": "6030",
                               "belong": "2",
                               "seed": 7777
                           },
                           {
                               "spot_id": "6005",
                               "belong": "1",
                               "seed": 2307
                           }
                       ],
                       "night_enemy": [],
                       "night_ally": [],
                       "ally_instance_info": [],
                       "ally_battle": [],
                       "building_info": [],
                       "squad_info": [],
                       "ap": 9,
                       "fairy_skill_return": [],
                       "fairy_skill_perform": [],
                       "fairy_skill_on_spot": [],
                       "fairy_skill_on_team": [],
                       "fairy_skill_on_enemy": [],
                       "fairy_skill_on_squad": []
                    }
                     */
                    #endregion
                    int mission_id = Parser.Json.ParseInt(request["mission_id"]);
                    
                }
                // 제대이동
                // ("Mission/teamMove")
                else if (uri.EndsWith("Mission/teamMove"))
                {

                }
                // 친구제대배치
                // ("Mission/reinforceFriendTeam")
                else if (uri.EndsWith("Mission/reinforceFriendTeam"))
                {

                }
                // 친구제대이동
                // ("Mission/friendTeamMove")
                else if (uri.EndsWith("Mission/friendTeamMove"))
                {

                }
                // 전투 종료
                // ("Mission/battleFinish")
                else if (uri.EndsWith("Mission/battleFinish"))
                {

                }
                // 턴 종료
                // ("Mission/endTurn")
                else if (uri.EndsWith("Mission/endTurn"))
                {

                }
                // 인질 구출
                // ("Mission/saveHostage")
                else if (uri.EndsWith("Mission/saveHostage"))
                {

                }
            }
        }

        /// <summary>
        /// "spot_act_info" 처리 - 제대 위치
        /// </summary>
        /// <param name="spot_act_info"></param>
        public static void ProcessSpotActInfo(JArray spot_act_info)
        {
            #region Packet Example
            /*
            "spot_act_info": [
                {
                    "spot_id": "5457",
                    "belong": "0",
                    "ally_instance_ids": [
                        1
                    ],
                    "if_random": "0",
                    "seed": 5261,
                    "team_id": "0",
                    "enemy_team_id": "0",
                    "boss_hp": "0",
                    "enemy_hp_percent": "1",
                    "enemy_instance_id": "0",
                    "enemy_ai": 0,
                    "enemy_ai_para": "",
                    "squad_instance_ids": [],
                    "hostage_id": "0",
                    "hostage_hp": "0",
                    "hostage_max_hp": "0",
                    "enemy_birth_turn": "999",
                    "reinforce_count": "0",
                    "supply_count": "0"
                },
             */
            #endregion
            if (spot_act_info != null)
            {
                foreach (JObject item in spot_act_info)
                {
                    int enemy_team_id = Parser.Json.ParseInt(item["enemy_team_id"]);        // 적 제대 id
                    int from_spot_id = Parser.Json.ParseInt(item["from_spot_id"]);
                    int to_spot_id = Parser.Json.ParseInt(item["to_spot_id"]);

                    int hostage_id = Parser.Json.ParseInt(item["hostage_id"]);              // 인질 id (인형도감번호)
                    int hostage_hp = Parser.Json.ParseInt(item["hostage_hp"]);              // 인질 hp

                    int boss_hp = Parser.Json.ParseInt(item["boss_hp"]);
                    int enemy_hp_percent = Parser.Json.ParseInt(item["enemy_hp_percent"]);
                    int enemy_instance_id = Parser.Json.ParseInt(item["enemy_instance_id"]);
                    int to_enemy_birth_turnspot_id = Parser.Json.ParseInt(item["enemy_birth_turn"]);
                    int enemy_ai = Parser.Json.ParseInt(item["enemy_ai"]);
                    int enemy_ai_para = Parser.Json.ParseInt(item["enemy_ai_para"]);
                    int squad_instance_id = Parser.Json.ParseInt(item["squad_instance_id"]);
                    int ally_instance_id = Parser.Json.ParseInt(item["ally_instance_id"]);
                }
            }
        }

        /// <summary>
        /// "night_spots" 처리 - 거점 변화
        /// </summary>
        /// <param name="night_spots"></param>
        public static void ProcessNightSpots(JArray night_spots)
        {
            #region Packet Example
            /*
                "night_spots": [
                    {
                        "spot_id": "6006",
                        "belong": "1",
                        "seed": 1665
                    },
             */
            #endregion
            if (night_spots != null)
            {
                foreach (JObject item in night_spots)
                {
                    int spot_id = Parser.Json.ParseInt(item["spot_id"]);
                    int belong = Parser.Json.ParseInt(item["belong"]);
                }
            }
        }

        /// <summary>
        /// "night_enemy" 처리 - 제대 위치
        /// </summary>
        /// <param name="night_enemy"></param>
        public static void ProcessNightEnemy(JArray night_enemy)
        {
            #region Packet Example
            /*
                "night_enemy": [
                    {
                        "enemy_team_id": "1694",
                        "from_spot_id": "6008",
                        "to_spot_id": "6007",
                        "boss_hp": "0",
                        "enemy_hp_percent": "1",
                        "enemy_instance_id": "3",
                        "enemy_birth_turn": "1",
                        "enemy_ai": 0,
                        "enemy_ai_para": "",
                        "hostage_id": "122",
                        "hostage_hp": "4",
                        "squad_instance_id": "0",
                        "ally_instance_id": 0
                    },
             */
            #endregion
            if (night_enemy != null)
            {
                foreach (JObject item in night_enemy)
                {
                    int enemy_team_id = Parser.Json.ParseInt(item["enemy_team_id"]);        // 적 제대 id
                    int from_spot_id = Parser.Json.ParseInt(item["from_spot_id"]);
                    int to_spot_id = Parser.Json.ParseInt(item["to_spot_id"]);

                    int hostage_id = Parser.Json.ParseInt(item["hostage_id"]);              // 인질 id (인형도감번호)
                    int hostage_hp = Parser.Json.ParseInt(item["hostage_hp"]);              // 인질 hp

                    int boss_hp = Parser.Json.ParseInt(item["boss_hp"]);
                    int enemy_hp_percent = Parser.Json.ParseInt(item["enemy_hp_percent"]);
                    int enemy_instance_id = Parser.Json.ParseInt(item["enemy_instance_id"]);
                    int to_enemy_birth_turnspot_id = Parser.Json.ParseInt(item["enemy_birth_turn"]);
                    int enemy_ai = Parser.Json.ParseInt(item["enemy_ai"]);
                    int enemy_ai_para = Parser.Json.ParseInt(item["enemy_ai_para"]);
                    int squad_instance_id = Parser.Json.ParseInt(item["squad_instance_id"]);
                    int ally_instance_id = Parser.Json.ParseInt(item["ally_instance_id"]);
                }
            }
        }

        /// <summary>
        /// "change_belong" 처리 - 거점 변화
        /// </summary>
        /// <param name="change_belong"></param>
        public static void ProcessChangeBelong(JObject change_belong)
        {
            #region Packet Example
            // "change_belong":{"6006":1,"6010":1,"6012":1,"6015":1,"6017":1,"6005":1}
            #endregion
            if (change_belong != null)
            {

            }
        }

        /// <summary>
        /// "ally_instance_info" 처리 - 3세력 등록
        /// </summary>
        /// <param name="ally_instance_info"></param>
        public static void ProcessAllyInstanceInfo(JArray ally_instance_info)
        {

        }

        public static void MergeSpot()
        {

        }

        public static void ClearSpot()
        {
            foreach (KeyValuePair<int, SpotInfo> item in spot_infos)
            {
                item.Value.enemy_team_id = -1;
            }
        }

        /// <summary>
        /// 전역 초기화
        /// </summary>
        public static void ClearMission()
        {

        }
    }
}
