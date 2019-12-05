using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Notifier;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using LocalizationResources;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Linq;
using static GFAlarm.MainWindow;

namespace GFAlarm.Transaction.PacketProcess
{
    /// <summary>
    /// 자율작전 패킷 처리
    /// </summary>
    public class Automission
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 자율작전 시작
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartAutomission(string request_string, string response_string)
        {
            #region Packet Example
            // request Automission/startAutomission?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "team_ids": [1,6],
                    "auto_mission_id": 114,
                    "number": 1
                }
            */
            #endregion
            try
            {
                log.Debug("자율작전 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int autoMissionId = Parser.Json.ParseInt(request["auto_mission_id"]);
                    int[] teamIds = Parser.Json.ParseIntArray(request["team_ids"]).ToArray();
                    int teamId = teamIds[0];
                    int number = Parser.Json.ParseInt(request["number"]);
                    int startTime = TimeUtil.GetCurrentSec();
                    //long startTime = Parser.Time.GetCurrentMs();

                    log.Debug("작전번호 {0}", autoMissionId);
                    log.Debug("작전횟수 {0}", number);

                    // 제대 리더
                    long gunWithUserId = UserData.Doll.GetTeamLeaderGunWithUserId(teamIds[0]);
                    DollWithUserInfo doll = UserData.Doll.Get(gunWithUserId);
                    int gunId = 0, skinId = 0;
                    if (doll != null)
                    {
                        gunId = doll.no;
                        skinId = doll.skin;
                        log.Debug("리더 {0} {1}", gunWithUserId, doll.name);
                    }

                    // 음성 알림
                    Notifier.Manager.notifyQueue.Enqueue(new Message()
                    {
                        send = MessageSend.Voice,
                        type = MessageType.start_auto_mission,
                        gunId = gunId,
                        skinId = skinId,
                    });

                    dashboardView.Add(new DispatchedEchleonTemplate()
                    {
                        autoMissionNumber = number,
                        autoMissionId = autoMissionId,
                        teamIds = teamIds,
                        startTime = startTime,
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "자율작전 시작 에러");
            }
        }

        /// <summary>
        /// 자율작전 완료
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishAutomission(string request_string, string response_string)
        {
            #region Packet Example
            // request Automission/abortAutomission?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "auto_mission_id": 5
                }
                */
            // response Automission/finishAutomission
            /*
                {
                    "add_gun_exp": [
                        {
                            "gun_with_user_id": "11113158",
                            "gun_exp": "30283300",
                            "gun_life": "540"
                        },
                        {
                            "gun_with_user_id": "12131545",
                            "gun_exp": "3263300",
                            "gun_life": "507"
                        },
                        ...
                    ],
                    "add_fairy_exp": [
                        {
                            "fairy_with_user_id": "490949",
                            "fairy_exp": "6296733"
                        }
                    ],
                    "free_exp": 0,
                    "add_user_exp": "45",
                    "add_gun": [
                        {
                            "gun_with_user_id": "355800306",
                            "gun_id": "9"
                        }
                    ],
                    "add_prize": [],
                    "add_equip": [
                        {
                            "id": "90698638",
                            "user_id": "343650",
                            "gun_with_user_id": "0",
                            "equip_id": "13",
                            "equip_exp": "0",
                            "equip_level": "0",
                            "pow": "0",
                            "hit": "0",
                            "dodge": "0",
                            "speed": "0",
                            "rate": "0",
                            "critical_harm_rate": "0",
                            "critical_percent": "0",
                            "armor_piercing": "0",
                            "armor": "0",
                            "shield": "0",
                            "damage_amplify": "0",
                            "damage_reduction": "0",
                            "night_view_percent": "1650",
                            "bullet_number_up": "0",
                            "adjust_count": "0",
                            "is_locked": "0",
                            "last_adjust": ""
                        },
                        ...
                    ],
                    "favor_change": {
                        "11113158": 81,
                        "12131545": 81,
                        "352953715": 27,
                        "352982323": 90,
                        "352991326": 27
                    },
                    "success_number": "1"
                }
            */
            #endregion
            try
            {
                log.Debug("자율작전 완료/취소");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null)
                {
                    int autoMissionId = Parser.Json.ParseInt(request["auto_mission_id"]);
                    log.Debug("작전번호 {0}", autoMissionId);
                    // 알림 탭 제거
                    dashboardView.Remove(new DispatchedEchleonTemplate()
                    {
                        autoMissionId = autoMissionId,
                    });
                }
                if (response != null)
                {
                    // 인형경험치 획득
                    if (response.ContainsKey("add_gun_exp") && response["add_gun_exp"] is JArray)
                    {
                        JArray items = response["add_gun_exp"].Value<JArray>();
                        foreach (JObject item in items)
                        {
                            try
                            {
                                long gunWithUserId = Parser.Json.ParseLong(item["gun_with_user_id"]);
                                int gunExp = Parser.Json.ParseInt(item["gun_exp"]);

                                UserData.Doll.SetExp(gunWithUserId, gunExp);
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex, "failed to set doll exp");
                            }
                        }
                    }
                    // 요정경험치 획득
                    if (response.ContainsKey("add_fairy_exp") && response["add_fairy_exp"] is JArray)
                    {
                        JArray items = response["add_fairy_exp"].Value<JArray>();
                        foreach (JObject item in items)
                        {
                            try
                            {
                                long fairyWithUserId = Parser.Json.ParseLong(item["fairy_with_user_id"]);
                                int fairyExp = Parser.Json.ParseInt(item["fairy_exp"]);

                                UserData.Fairy.SetExp(fairyWithUserId, fairyExp);
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex, "failed to set fairy exp");
                            }
                        }
                    }
                    // 인형 획득
                    if (response.ContainsKey("add_gun") && response["add_gun"] is JArray)
                    {
                        JArray items = response["add_gun"].Value<JArray>();
                        foreach (JObject item in items)
                        {
                            try
                            {
                                long gunWithUserId = Parser.Json.ParseLong(item["gun_with_user_id"]);
                                int gunId = Parser.Json.ParseInt(item["gun_id"]);

                                UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId));
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex, "failed to add doll");
                            }
                        }
                    }
                    // 장비 획득
                    if (response.ContainsKey("add_equip"))
                    {
                        JArray items = Parser.Json.ParseJArray(Parser.Json.ParseString(response["add_equip"]));
                        foreach (JToken item in items)
                        {
                            try
                            {
                                long equipWithUserId = Parser.Json.ParseLong(item["id"]);
                                int equipId = Parser.Json.ParseInt(item["equip_id"]);

                                UserData.Equip.Add(equipWithUserId, new EquipWithUserInfo(equipWithUserId, equipId));
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex, "failed to add equip");
                            }
                        }
                    }
                    // 자유경험치 획득
                    if (response.ContainsKey("free_exp"))
                    {
                        int freeExp = Parser.Json.ParseInt(response["free_exp"]);
                        UserData.GlobalExp.exp += freeExp;
                        if (UserData.GlobalExp.maxExp <= UserData.GlobalExp.exp)
                        {
                            if (Config.Alarm.notifyMaxGlobalExp && !UserData.GlobalExp.notified)
                            {
                                UserData.GlobalExp.notified = true;
                                Notifier.Manager.notifyQueue.Enqueue(new Message()
                                {
                                    send = MessageSend.All,
                                    type = MessageType.reach_max_global_exp,
                                    subject = LanguageResources.Instance["MESSAGE_MAX_SURPLUS_EXP_SUBJECT"],
                                    content = LanguageResources.Instance["MESSAGE_MAX_SURPLUS_EXP_CONTENT"],
                                });
                            }
                        }
                        //MainWindow.view.SetGlobalExp(UserData.GlobalExp.exp, UserData.GlobalExp.maxExp);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Automission/finishAutomission, Automission/abortAutomission");
            }
        }

        /// <summary>
        /// 자율작전 취소
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void AbortAutomission(string request_string, string response_string)
        {
            FinishAutomission(request_string, response_string);
        }
    }
}
