using GFAlarm.Data;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GFAlarm.MainWindow;

namespace GFAlarm.Transaction.PacketProcess
{
    /// <summary>
    /// 탐색 패킷 처리
    /// </summary>
    public class Explore
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 탐색 시작
        /// ("Explore/start")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartExplore(string request_string, string response_string)
        {
            #region Packet Example
            // request Explore/start?uid={0}&signcode={1}&req_id={2} 
            // null
            // response Explore/start
            /*
                {
                    "user_id": 343650,
                    "start_time": 1565341877,
                    "end_time": 1565359877,
                    "cancel_time": 0,
                    "gun_ids": ["27_801","70","26_1205","96"],
                    "pet_ids": ["40201","40102"],
                    "area_id": "3",             // 숲?
                    "target_id": "43",
                    "draw_event_prize": 0,
                    "item_id": 0,
                    "next_time": "1565345213"
                }
                */
            /*
            {
                "user_id": 621225,
                "start_time": 1565348535,
                "end_time": 1565355735,
                "cancel_time": 0,
                "gun_ids": ["2","26","75"],
                "pet_ids": [],
                "area_id": "1", // 초원?
                "target_id": "2",
                "draw_event_prize": 0,
                "item_id": 0,
                "next_time": "1565352715"
            }
            */
            #endregion
            try
            {
                log.Debug("탐색 시작");
                JObject response = Parser.Json.ParseJObject(response_string);

                int areaId = Parser.Json.ParseInt(response["area_id"]);
                if (areaId > 0)
                {
                    // 알림 제거
                    dashboardView.Remove(new ExploreTemplate()
                    {
                        id = 1
                    });

                    // 이벤트 목표
                    int targetId = Parser.Json.ParseInt(response["target_id"]);
                    // 사용 아이템
                    int itemId = Parser.Json.ParseInt(response["item_id"]);

                    int startTime = Parser.Json.ParseInt(response["start_time"]);
                    int endTime = Parser.Json.ParseInt(response["end_time"]);
                    int nextTime = Parser.Json.ParseInt(response["next_time"]);

                    // 탐색제대 인형들
                    string[] gun_ids = Parser.Json.ParseStringArray(response["gun_ids"]);
                    List<int> gunIds = new List<int>();
                    foreach (string gunIdString in gun_ids)
                    {
                        int gunId;
                        if (gunIdString.Contains("_"))
                            gunId = Parser.String.ParseInt(gunIdString.Split('_')[0]);
                        else
                            gunId = Parser.String.ParseInt(gunIdString);
                        gunIds.Add(gunId);
                    }

                    // 알림 추가
                    dashboardView.Add(new ExploreTemplate()
                    {
                        areaId = areaId,
                        targetId = targetId,
                        itemId = itemId,
                        gunIds = gunIds.ToArray(),

                        endTime = endTime,
                        nextTime = nextTime,
                    });
                    dashboardView.RefreshExplore();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "탐색 시작 처리 실패");
            }
        }

        /// <summary>
        /// 탐색 정산
        /// ("Explore/balance")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void BalanceExplore(string request_string, string response_string)
        {
            #region Packet Example
            // request Explore/balance?uid={0}&signcode={1}&req_id={2}
            // null
            // response Explore/balance
            /*
            {
                "list": [
                    {
                        "user_id": 621225,
                        "start_time": 1565348535,
                        "end_time": 1565355735,
                        "cancel_time": 1565349483,
                        "affairs": [],
                        "gun_ids": ["2","26","75"],
                        "pet_ids": [],
                        "area_id": "1",
                        "target_id": "2",
                        "draw_event_prize": 0,
                        "item_id": 0
                    },
                    {
                        "user_id": 621225,
                        "start_time": 1565349516,
                        "end_time": 1565356716,
                        "cancel_time": 0,
                        "affairs": [
                            {
                                "affair_id": "112",
                                "affair_time": "1565350788"
                            },
                            {
                                "affair_id": "64",
                                "affair_time": "1565352452"
                            },
                            {
                                "affair_id": "110",
                                "affair_time": "1565355861"
                            }
                        ],
                        "gun_ids": ["2","26","75"],
                        "pet_ids": [],
                        "area_id": "4",
                        "target_id": "62",
                        "draw_event_prize": 0,
                        "item_id": 0
                    }
                ],
                "next_time": 0,
                "ex_prize": {
                    "user_exp": 0,
                    "mp": 0,
                    "ammo": 0,
                    "mre": 0,
                    "part": 0,
                    "gem": 0,
                    "core": 0,
                    "gun_id": [],
                    "item_ids": "",
                    "gift": "",
                    "furniture": [],
                    "equip_ids": [],
                    "coins": "",
                    "skin": "",
                    "bp_pay": 0,
                    "fairy_ids": [],
                    "chip": []
                },
                "is_auto": 1,
                "pets": ["0","0","0"],
                "items_taken": [],
                "next_explore_time": 1565367516,
                "end_time": 1565356716,
                "explore_time_type": 1,
                "mp": "250177",
                "ammo": "193361",
                "mre": "168259",
                "part": "171210"
            } 
            */
            #endregion
            log.Debug("탐색 정산");
            try
            {
                // 새로운 탐색 출발인지 확인
                JObject response = Parser.Json.ParseJObject(response_string);
                if (response != null)
                {
                    // 알림 제거
                    dashboardView.Remove(new ExploreTemplate()
                    {
                        id = 1,
                    });
                    int next_time = Parser.Json.ParseInt(response["next_time"]);
                    if (response.ContainsKey("list") && response["list"] is JArray)
                    {
                        JArray items = response["list"].Value<JArray>();
                        foreach (JObject item in items)
                        {
                            int cancelTime = Parser.Json.ParseInt(item["cancel_time"]);
                            int endTime = Parser.Json.ParseInt(item["end_time"]);
                            if (endTime > TimeUtil.GetCurrentSec())
                            {
                                int startTime = Parser.Json.ParseInt(item["start_time"]);

                                int areaId = Parser.Json.ParseInt(item["area_id"]);
                                int targetId = Parser.Json.ParseInt(item["target_id"]);
                                int itemId = Parser.Json.ParseInt(item["item_id"]);

                                string[] gunIdsString = Parser.Json.ParseStringArray(response["gun_ids"]);
                                List<int> gunIds = new List<int>();
                                foreach (string gunIdString in gunIdsString)
                                {
                                    int gunId;
                                    if (gunIdString.Contains("_"))
                                        gunId = Parser.String.ParseInt(gunIdString.Split('_')[0]);
                                    else
                                        gunId = Parser.String.ParseInt(gunIdString);
                                    gunIds.Add(gunId);
                                }

                                // 알림 추가
                                dashboardView.Add(new ExploreTemplate()
                                {
                                    areaId = areaId,
                                    targetId = targetId,
                                    itemId = itemId,
                                    gunIds = gunIds.ToArray(),

                                    endTime = endTime,
                                    nextTime = next_time,
                                });
                                dashboardView.RefreshExplore();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex, "탐색 정산 처리 실패");
            }
        }

        /// <summary>
        /// 탐색 취소
        /// ("Explore/cancel")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void CancelExplore(string request_string, string response_string)
        {
            log.Debug("탐색 취소");
            try
            {
                // 알림 탭 제거
                dashboardView.Remove(new ExploreTemplate()
                {
                    id = 1,
                });
            }
            catch (Exception ex)
            {
                log.Error(ex, "탐색 취소 처리 실패");
            }
        }

        /// <summary>
        /// 탐색 이벤트
        /// ("Explore/getEvent")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void GetEvent(string request_string, string response_string)
        {
            #region Packet Example
            // request Explore/getEvent?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"time":1565345212} 
                */
            // response Explore/getEvent
            /*
                {
                    "event_list": [
                        {
                            "affair_id": "113",
                            "affair_time": "1565345213"
                        }
                    ],
                    "next_time": "1565349483",
                    "is_prize": 0
                }
            */
            #endregion
            log.Debug("탐색 이벤트");
            try
            {
                JObject response = Parser.Json.ParseJObject(response_string);
                int nextTime = Parser.Json.ParseInt(response["next_time"]);
                if (nextTime > 0)
                {
                    dashboardView.SetExploreEventRemainTime(nextTime);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "탐색 이벤트 처리 실패");
            }
        }

        /// <summary>
        /// 탐색 편성
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void SetTeam(string request_string, string response_string)
        {
            #region Packet Example
            // request
            /*
                {
                    "guns": [18451639,32435361,108002647,216952610,301791334],
                    "pets": [10413439,23579277,22879676],
                    "time_type": 3,
                    "is_auto": 1
                }
                */
            // response
            // 1
            #endregion
            try
            {
                log.Debug("탐색 편성");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null && request is JObject)
                {
                    long[] guns = Parser.Json.ParseLongArray(request["guns"]);
                    long[] pets = Parser.Json.ParseLongArray(request["pets"]);
                    int timeType = Parser.Json.ParseInt(request["time_type"]);
                    int isAuto = Parser.Json.ParseInt(request["is_auto"]);

                    UserData.Doll.exploreTeam.Clear();
                    foreach (long gun in guns)
                    {
                        UserData.Doll.exploreTeam.Add(gun);
                    }
                    dashboardView.RefreshExplore();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "탐색 편성 처리 실패");
            }
        }

        /// <summary>
        /// 탐색 우세
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void GetAdaptiveTeam(string request_string, string response_string)
        {
            #region Packet Example
            // request Explore/getAdaptiveTeam?uid={0}&signcode={1}&req_id={2}
            // null
            // response Explore/getAdaptiveTeam
            /*
                {
                    "id": "5",
                    "guns": ["48","96","26","69","75","27","37","70","81","2"],
                    "pets": ["40102","40201","40402","40501"],
                    "start_time": 1564945200,
                    "end_time": 1565549999
                }
            */
            #endregion
            try
            {
                log.Debug("탐색 우세");
                JObject response = Parser.Json.ParseJObject(response_string);
                if (response != null && response is JObject)
                {
                    int id = Parser.Json.ParseInt(response["id"]);
                    int[] guns = Parser.Json.ParseIntArray(response["guns"]);
                    int[] pets = Parser.Json.ParseIntArray(response["pets"]);
                    int startTime = Parser.Json.ParseInt(response["start_time"]);
                    int endTime = Parser.Json.ParseInt(response["end_time"]);

                    UserData.Doll.exploreAdaptiveDolls.Clear();
                    foreach (int gun in guns)
                    {
                        UserData.Doll.exploreAdaptiveDolls.Add(gun);
                    }
                    dashboardView.RefreshExplore();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "탐색 우세 처리 실패");
            }
        }

        /// <summary>
        /// 탐색 보상 획득
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void GetExploreItem(string request_string, string response_string)
        {
            #region Packet Example
            // response Index/getExploreMail
            /*
            {
                "get_list": [
                    "200832392",
                    "200832394"
                ],
                "out": {
                    "user_exp": 0,
                    "mp": 0,
                    "ammo": 0,
                    "mre": 0,
                    "part": 0,
                    "core": 0,
                    "gem": 0,
                    "gun_id": [],
                    "item_ids": "4003-351,4004-35,4002-35,6006-1,506-10",
                    "equip_ids": [],
                    "furniture": [],
                    "gift": "",
                    "skin": [],
                    "bp_pay": 0,
                    "coins": "",
                    "fairy_ids": "",
                    "commander_uniform": []
                }
            }
            */
            #endregion
        }
    }
}
