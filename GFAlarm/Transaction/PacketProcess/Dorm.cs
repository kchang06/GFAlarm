using GFAlarm.Data;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using LocalizationResources;
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
    /// 숙소 패킷 처리
    /// </summary>
    public class Dorm
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 정보분석 시작
        /// ("Dorm/data_analysis")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartDataAnalysis(string request_string, string response_string)
        {
            #region Packet Example
            // request Dorm/data_analysis
            /*
                {
                    "build_slot": 0,
                    "input_level": 1,
                    "number": 10,
                    "quick": 0
                }
                */
            // response Dorm/data_analysis
            /*
                {
                    "data": {
                        "1": 1562000440,
                        "2": 1562000440,
                        "3": 1562000440,
                        "4": 1562000440,
                        "5": 1562000440,
                        "6": 1562000440,
                        "7": 1562000440,
                        "8": 1562000440,
                        "9": 1562000440,
                        "10": 1562000440
                    }
                }
            */
            #endregion
            try
            {
                log.Debug("정보분석 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    int quick = Parser.Json.ParseInt(request["quick"]);
                    int endTime = TimeUtil.GetCurrentSec() + UserData.DataAnalysis.requireTime;
                    //long endTime = Parser.Time.GetCurrentMs() + UserData.DataAnalysis.requireTime;
                    List<string> keys = response["data"].Value<JObject>().Properties().Select(p => p.Name).ToList();
                    foreach (string key in keys)
                    {
                        int buildSlot = Parser.Json.ParseInt(key);
                        if (buildSlot > 0)
                        {
                            // 쾌속이 아닌 경우
                            if (quick != 1)
                            {
                                // 알림 탭 추가
                                dashboardView.Add(new DataAnalysisTemplate()
                                {
                                    slot = buildSlot,
                                    endTime = endTime,
                                    TBPiece = LanguageResources.Instance["UNKNOWN"],
                                    TBPieceGuide = LanguageResources.Instance["NEED_RELOGIN"],
                                });
                            }

                            // 임무 갱신
                            UserData.Quest.Daily.dataAnalysis += 1;
                            UserData.Quest.Weekly.dataAnalysis += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "정보분석 시작 처리 실패");
            }
        }

        /// <summary>
        /// 정보분석 완료
        /// ("Dorm/data_analysis_finish")
        /// ("Dorm/data_analysis_finish_all")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishDataAnalysis(string request_string, string response_string)
        {
            #region Packet Example
            // request Dorm/data_analysis_finish_all?uid={0}&signcode={1}&req_id={2}
            // null
            // response Dorm/data_analysis_finish_all 
            /*
                {
                    "data": {
                        "1": {
                            "piece": "0",
                            "id": "3041",
                            "chip": {
                                "user_id": 343650,
                                "chip_id": "3041",
                                "color_id": "2",
                                "grid_id": "7",
                                "shape_info": "2,0",
                                "assist_damage": "2",
                                "assist_reload": "0",
                                "assist_hit": "0",
                                "assist_def_break": "2",
                                "damage": "0",
                                "atk_speed": "0",
                                "hit": "0",
                                "def": "0",
                                "id": 16166686
                            },
                            "change": 0
                        },
                        "5": {
                            "piece": "1",
                            "id": "303",
                            "chip": [],
                            "change": 1
                        },
                    }
                }
            */
            #endregion
            try
            {
                log.Debug("정보분석 완료");
                JObject response = Parser.Json.ParseJObject(response_string);
                if (response != null)
                {
                    List<string> keys = response["data"].Value<JObject>().Properties().Select(p => p.Name).ToList();
                    foreach (string key in keys)
                    {
                        int buildSlot = Parser.String.ParseInt(key);
                        // 알림 탭 제거
                        dashboardView.Remove(new DataAnalysisTemplate()
                        {
                            slot = buildSlot,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "정보분석 완료 처리 실패");
            }
        }

        /// <summary>
        /// 정보분석 쾌속 완료
        /// ("Dorm/data_analysis_complete")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishQuickDataAnalysis(string request_string, string response_string)
        {
            #region Packet Example
            // request
            // {"is_cost_item16":1} 
            // response
            /*
                {
                    "cost_item16_num": 10,
                    "data": {
                        "1": {
                            "piece": "0",
                            "id": "3061",
                            "chip": {
                                "user_id": 343650,
                                "chip_id": "3061",
                                "color_id": "2",
                                "grid_id": "35",
                                "shape_info": "0,0",
                                "assist_damage": "0",
                                "assist_reload": "3",
                                "assist_hit": "0",
                                "assist_def_break": "3",
                                "damage": "0",
                                "atk_speed": "0",
                                "hit": "0",
                                "def": "0",
                                "id": 17372804
                            },
                            "change": 0
                        },
                        "2": {
                            "piece": "1",
                            "id": "304",
                            "chip": [],
                            "change": 1
                        },
                    ...
            */
            #endregion
            try
            {
                log.Debug("정보분석 전부 확인");
                JObject response = Parser.Json.ParseJObject(response_string);
                if (response != null)
                {
                    List<string> keys = response["data"].Value<JObject>().Properties().Select(p => p.Name).ToList();
                    foreach (string key in keys)
                    {
                        int buildSlot = Parser.String.ParseInt(key);
                        // 알림 탭 제거
                        dashboardView.Remove(new DataAnalysisTemplate()
                        {
                            slot = buildSlot,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "정보분석 쾌속 완료 처리 실패");
            }
        }

        /// <summary>
        /// 선물하기
        /// ("Dorm/giftToGun")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void GiveGift(string request_string, string response_string)
        {
            #region Packet Example
            // request Dorm/giftToGun?uid={0}&outdatacode={1}&req_id={2}
            /*  
                {
                    "item_id": 200001,
                    "gun_with_user_id": 354794702,
                    "num": 15,
                    "new": 1
                }
                */

            // response Dorm/giftToGun
            /*
                {
                    "all_favorup_gun": [],
                    "last_favor_recover_time": "1563674234",
                    "skin_add_favor": 0,
                    "gun_with_user_id": "354794702",
                    "gun_exp": 45000
                }
            */
            #endregion
            try
            {
                log.Debug("선물하기");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    long item_id = Parser.Json.ParseLong(request["item_id"]);
                    long gunWithUserId = Parser.Json.ParseLong(request["gun_with_user_id"]);
                    long fairyWithUserId = Parser.Json.ParseLong(request["fairy_with_user_id"]);
                    int num = Parser.Json.ParseInt(request["num"]);
                    int gunExp = Parser.Json.ParseInt(response["gun_exp"]);

                    switch (item_id)
                    {
                        // 작전보고서
                        case 200001:
                            if (gunWithUserId > 0 && gunExp > 0)
                            {
                                UserData.Doll.SetExp(gunWithUserId, gunExp, true);
                            }
                            else if (fairyWithUserId > 0)
                            {
                                int fairyExp = num * 3000;
                                UserData.Fairy.GainExp(fairyWithUserId, fairyExp);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "선물하기 처리 실패");
            }
        }

        /// <summary>
        /// 스킨 변경
        /// ("Dorm/changeSkin")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void ChangeSkin(string request_string, string response_string)
        {
            #region Packet Example
            // request Dorm/changeSkin?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"gun_with_user_id":190312255,"skin_id":0} 
            */
            #endregion
            try
            {
                log.Debug("스킨 변경");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    long gunWithUserId = Parser.Json.ParseLong(request["gun_with_user_id"]);
                    int skinId = Parser.Json.ParseInt(request["skin_id"]);

                    UserData.Doll.ChangeSkin(gunWithUserId, skinId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "스킨 변경 처리 실패");
            }
        }
    }
}
