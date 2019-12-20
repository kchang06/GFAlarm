using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Notifier;
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
    /// 인형 패킷 처리
    /// </summary>
    public class Gun
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 인형수복 시작
        /// ("Gun/fixGuns")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartRestore(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/fixGuns?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "if_quick": 0,
                    "fix_guns": {
                        "6762038": 3,
                        "12131545": 2
                    }
                }
            */
            #endregion
            try
            {
                log.Debug("인형수복 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int if_quick = Parser.Json.ParseInt(request["if_quick"]);
                    if (request["fix_guns"] != null && request["fix_guns"] is JObject)
                    {
                        Dictionary<string, string> fixGuns = Parser.Json.ParseItems(request["fix_guns"].Value<JObject>());
                        foreach (KeyValuePair<string, string> fixGun in fixGuns)
                        {
                            long gunWithUserId = Parser.String.ParseLong(fixGun.Key);
                            int fixSlot = Parser.String.ParseInt(fixGun.Value);
                            if (fixSlot > 0)
                            {
                                int startTime = TimeUtil.GetCurrentSec();

                                int[] restore = GameData.Doll.Restore.GetRestore(gunWithUserId);
                                //UserData.Doll.GetRestoreRequireTime(gun_with_user_id, ref require_time, ref require_mp, ref require_part);

                                int endTime = startTime + restore[0];

                                log.Debug("수복인형 {0} ({1})", UserData.Doll.GetName(gunWithUserId), gunWithUserId);
                                log.Debug("필요시간 {0}", restore[0]);
                                log.Debug("필요인력 {0}, 부품 {1}", restore[1], restore[2]);

                                UserData.mp -= restore[1];
                                UserData.part -= restore[2];

                                if (if_quick == 1) // 쾌속수복
                                {
                                    UserData.Doll.Fix(gunWithUserId);
                                }
                                else
                                {
                                    // 알림 탭 추가
                                    dashboardView.Add(new RestoreDollTemplate()
                                    {
                                        slot = fixSlot,
                                        gunWithUserId = gunWithUserId,
                                        endTime = endTime,
                                    });
                                }

                                // 임무 갱신
                                UserData.Quest.Daily.fixGun += 1;
                                UserData.Quest.Weekly.fixGun += 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/fixGuns");
            }
        }

        /// <summary>
        /// 인형수복 완료
        /// ("Gun/fixFinish")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishRestore(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/fixFinish?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"fix_slot":3} 
            */
            #endregion
            try
            {
                log.Debug("인형수복 완료");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int fixSlot = Parser.Json.ParseInt(request["fix_slot"]);

                    long gunWithUserId = dashboardView.FindRestoreDoll(fixSlot);
                    UserData.Doll.Fix(gunWithUserId);

                    // 알림 탭 제거
                    dashboardView.Remove(new RestoreDollTemplate()
                    {
                        slot = fixSlot,
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/fixFinish");
            }
        }

        /// <summary>
        /// 인형제조 시작
        /// ("Gun/developGun")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartProduce(string request_string, string response_string)
        {
            #region Packet Example
            // request /index.php/1001/Gun/developGun
            /*
                {
                    "mp": 130,
                    "ammo": 130,
                    "mre": 130,
                    "part": 30,
                    "build_slot": 1,
                    "input_level": 0
                }
            */
            // response /index.php/1001/Gun/developGun
            /*
                {
                    "gun_id":"141"
                }
            */
            #endregion
            try
            {
                log.Debug("인형제조 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    int buildSlot = Parser.Json.ParseInt(request["build_slot"]);
                    if (buildSlot > 0)
                    {
                        int mp = Parser.Json.ParseInt(request["mp"]);
                        int ammo = Parser.Json.ParseInt(request["ammo"]);
                        int mre = Parser.Json.ParseInt(request["mre"]);
                        int part = Parser.Json.ParseInt(request["part"]);
                        int inputLevel = Parser.Json.ParseInt(request["input_level"]);
                        // 0: 일반, 1: 소투입, 2: 중투입, 3: 고투입

                        UserData.mp -= mp;
                        UserData.ammo -= ammo;
                        UserData.mre -= mre;
                        UserData.part -= part;

                        log.Debug("투입자원 {0}/{1}/{2}/{3}", mp, ammo, mre, part);
                        log.Debug("투입레벨 {0} (0: 일반, 1: 소투입, 2: 중투입, 3: 고투입)", inputLevel);

                        int gunId = Parser.Json.ParseInt(response["gun_id"]);
                        int startTime = TimeUtil.GetCurrentSec();
                        //long startTime = Parser.Time.GetCurrentMs();

                        //log.Debug("제조슬롯 {0} | 인형 {1} | 제조시작 {2}", buildSlot, gunId, Parser.Time.GetDateTime(startTime).ToString("MM-dd HH:mm:ss"));

                        // 알림 탭 추가
                        ProduceDollTemplate template = new ProduceDollTemplate()
                        {
                            slot = buildSlot,
                            gunId = gunId,
                            startTime = startTime,
                            inputLevel = inputLevel,
                            spendResource = new int[] { mp, ammo, mre, part },
                        };
                        dashboardView.Add(template);

                        string gun_name = template.gunName;
                        string gun_type = template.category;
                        int gun_star = template.star;

                        log.Debug("병과 {0} | 레어도 {1} 성", gun_type, gun_star);

                        if ((Config.Alarm.notifyProduceDoll5Star && gun_star >= 5) ||  // 5성 인형제조 알림
                            (Config.Alarm.notifyProduceShotgun && gun_type == "SG") || // 샷건 인형제조 알림
                            (Config.Alarm.notifyProduceDoll))                          // 인형제조 알림
                        {
                            Notifier.Manager.notifyQueue.Enqueue(new Message()
                            {
                                send = MessageSend.All,
                                subject = LanguageResources.Instance["MESSAGE_PRODUCE_DOLL_SUBJECT"],
                                content = string.Format(LanguageResources.Instance["MESSAGE_PRODUCE_DOLL_CONTENT"],
                                                        gun_star, gun_name, gun_type),
                            });
                        }

                        // 일반제조
                        if (inputLevel == 0)
                        {
                            // 임무 갱신
                            UserData.Quest.Daily.produceDoll += 1;
                            UserData.Quest.Weekly.produceDoll += 1;
                        }
                        // 중제조
                        else if (inputLevel > 0)
                        {
                            // 임무 갱신
                            UserData.Quest.Weekly.produceHeavyDoll += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/developGun");
            }
        }

        /// <summary>
        /// 인형제조 완료
        /// ("Gun/finishDevelop")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishProduce(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/finishDevelop?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "build_slot": 3
                }
            */
            // response Gun/finishDevelop
            /*
                {
                    "gun_with_user_add": {
                        "gun_with_user_id": "383506736",
                        "gun_id": "2"
                    }
                }
             */
            #endregion
            try
            {
                log.Debug("인형제조 완료");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null)
                {
                    int buildSlot = Parser.Json.ParseInt(request["build_slot"]);

                    // 알림 탭 제거
                    dashboardView.Remove(new ProduceDollTemplate()
                    {
                        slot = buildSlot,
                    });
                }
                if (response != null)
                {
                    // 인형 추가
                    if (response.ContainsKey("gun_with_user_add") && response["gun_with_user_add"] is JObject)
                    {
                        JObject gun_with_user_add = response["gun_with_user_add"].Value<JObject>();
                        long gunWithUserId = Parser.Json.ParseLong(gun_with_user_add["gun_with_user_id"]);
                        int gunId = Parser.Json.ParseInt(gun_with_user_add["gun_id"]);

                        UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/finishDevelop");
            }
        }

        /// <summary>
        /// 인형일괄제조 시작
        /// ("Gun/developMultiGun")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartMultiProduce(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/developMultiGun?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "mp": 130,
                    "ammo": 130,
                    "mre": 130,
                    "part": 30,
                    "input_level": 0,
                    "build_quick": 0,
                    "build_multi": 2,
                    "build_heavy": 0
                }
            */
            // response Gun/developMultiGun (단일 제조)
            /*
                {
                    "gun_ids": [
                        {
                            "id": "93",
                            "slot": 3
                        },
                        {
                            "id": "5",
                            "slot": 5
                        }
                    ]
                }
            */
            // response Gun/developMultiGun (복수 제조)
            /*
                [
                    {
                        "gun_with_user_add": {
                            "gun_with_user_id": "383509177",
                            "gun_id": "9"
                        }
                    },
                    {
                        "gun_with_user_add": {
                            "gun_with_user_id": "383509178",
                            "gun_id": "90"
                        }
                    },
                    {
                        "gun_with_user_add": {
                            "gun_with_user_id": "383509179",
                            "gun_id": "2"
                        }
                    }
                ]
             */
            #endregion
            try
            {
                log.Debug("인형일괄제조 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int mp = Parser.Json.ParseInt(request["mp"]);
                    int ammo = Parser.Json.ParseInt(request["ammo"]);
                    int mre = Parser.Json.ParseInt(request["mre"]);
                    int part = Parser.Json.ParseInt(request["part"]);
                    int inputLevel = Parser.Json.ParseInt(request["input_level"]);
                    int buildQuick = Parser.Json.ParseInt(request["build_quick"]);
                    // 0: 일반, 1: 소투입, 2: 중투입, 3: 고투입

                    log.Debug("투입자원 {0}/{1}/{2}/{3}", mp, ammo, mre, part);
                    log.Debug("투입레벨 {0} (0: 일반, 1: 소투입, 2: 중투입, 3: 고투입)", inputLevel);

                    // 복수 제조
                    if (buildQuick == 1)
                    {
                        JArray response = Parser.Json.ParseJArray(response_string);
                        foreach (var item in response)
                        {
                            JObject gun_with_user_add = item["gun_with_user_add"].Value<JObject>();
                            long gunWithUserId = Parser.Json.ParseLong(gun_with_user_add["gun_with_user_id"]);
                            int gunId = Parser.Json.ParseInt(gun_with_user_add["gun_id"]);

                            UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId));

                            // 일반제조
                            if (inputLevel == 0)
                            {
                                // 임무 갱신
                                UserData.Quest.Daily.produceDoll += 1;
                                UserData.Quest.Weekly.produceDoll += 1;
                            }
                            // 중형제조
                            else if (inputLevel > 0)
                            {
                                // 임무 갱신
                                UserData.Quest.Weekly.produceHeavyDoll += 1;
                            }
                        }
                    }
                    // 단수 제조
                    else
                    {
                        JObject response = Parser.Json.ParseJObject(response_string);
                        if (response.ContainsKey("gun_ids") && response["gun_ids"] is JArray)
                        {
                            JArray items = response["gun_ids"].Value<JArray>();
                            foreach (var item in items)
                            {
                                int buildSlot = Parser.Json.ParseInt(item["slot"]);
                                if (buildSlot > 0)
                                {
                                    UserData.mp -= mp;
                                    UserData.ammo -= ammo;
                                    UserData.mre -= mre;
                                    UserData.part -= part;

                                    int gunId = Parser.Json.ParseInt(item["id"]);
                                    int startTime = TimeUtil.GetCurrentSec();

                                    //log.Debug("제조슬롯 {0} | 인형 {1} | 제조시작 {2}", buildSlot, gunId, Parser.Time.GetDateTime(startTime).ToString("MM-dd HH:mm:ss"));

                                    JObject doll = GameData.Doll.GetDollData(gunId);
                                    string gunName = "";
                                    string gunType = "";
                                    int gunStar = 0;
                                    if (doll != null)
                                    {
                                        gunName = Parser.Json.ParseString(doll["name"]);
                                        gunType = Parser.Json.ParseString(doll["type"]);
                                        gunStar = Parser.Json.ParseInt(doll["star"]);

                                        log.Debug("병과 {0} | 레어도 {1} 성", gunType, gunStar);
                                    }

                                    // 알림 탭 추가
                                    dashboardView.Add(new ProduceDollTemplate()
                                    {
                                        slot = buildSlot,
                                        gunId = gunId,
                                        startTime = startTime,
                                        inputLevel = inputLevel,
                                        spendResource = new int[] { mp, ammo, mre, part },
                                    });

                                    if ((Config.Alarm.notifyProduceDoll5Star && gunStar >= 5) ||  // 5성 인형제조 알림
                                        (Config.Alarm.notifyProduceShotgun && gunType == "SG") || // 샷건 인형제조 알림
                                        (Config.Alarm.notifyProduceDoll))                         // 인형제조 알림
                                    {
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            subject = LanguageResources.Instance["MESSAGE_PRODUCE_DOLL_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_PRODUCE_DOLL_CONTENT"],
                                                                    gunStar, gunName, gunType),
                                        });
                                    }

                                    // 일반제조
                                    if (inputLevel == 0)
                                    {
                                        // 임무 갱신
                                        UserData.Quest.Daily.produceDoll += 1;
                                        UserData.Quest.Weekly.produceDoll += 1;
                                    }
                                    // 중형제조
                                    else if (inputLevel > 0)
                                    {
                                        // 임무 갱신
                                        UserData.Quest.Weekly.produceHeavyDoll += 1;
                                    }
                                }
                            }
                        }
                    }
                }

                //if (request != null && response != null && response.ContainsKey("gun_ids") && response["gun_ids"] is JArray)
                //{
                //    int mp = Parser.Json.ParseInt(request["mp"]);
                //    int ammo = Parser.Json.ParseInt(request["ammo"]);
                //    int mre = Parser.Json.ParseInt(request["mre"]);
                //    int part = Parser.Json.ParseInt(request["part"]);
                //    int inputLevel = Parser.Json.ParseInt(request["input_level"]);
                //    // 0: 일반, 1: 소투입, 2: 중투입, 3: 고투입

                //    log.Debug("투입자원 {0}/{1}/{2}/{3}", mp, ammo, mre, part);
                //    log.Debug("투입레벨 {0} (0: 일반, 1: 소투입, 2: 중투입, 3: 고투입)", inputLevel);


                //}
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/developMultiGun");
            }
        }

        /// <summary>
        /// 인형일괄제조 완료
        /// ("Gun/finishAllDevelop")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishMultiProduce(string request_string, string response_string)
        {
            #region Packet Example
            // response /index.php/1001/Gun/finishAllDevelop
            /*
                {
                    "gun_with_user_add_list": [
                        {
                            "build_slot": 1,
                            "gun_with_user_id": "343068611",
                            "gun_id": "12"
                        },
                        {
                            "build_slot": 3,
                            "gun_with_user_id": "343068612",
                            "gun_id": "9"
                        }
                    ],
                    "cost_item3_num": 0
                }
                */
            #endregion
            try
            {
                log.Debug("인형일괄제조 완료");
                JObject response = Parser.Json.ParseJObject(response_string);
                if (response != null)
                {
                    if (response["gun_with_user_add_list"] != null && response["gun_with_user_add_list"] is JArray)
                    {
                        JArray items = response["gun_with_user_add_list"].Value<JArray>();
                        foreach (var item in items)
                        {
                            int buildSlot = Parser.Json.ParseInt(item["build_slot"]);
                            long gunWithUserId = Parser.Json.ParseLong(item["gun_with_user_id"]);
                            int gunId = Parser.Json.ParseInt(item["gun_id"]);

                            log.Debug("제조슬롯 {0} | 인형 {1} | 인형번호 {2}", buildSlot, GameData.Doll.GetDollName(gunId), gunWithUserId);

                            // 알림 탭 제거
                            dashboardView.Remove(new ProduceDollTemplate()
                            {
                                slot = buildSlot,
                            });

                            // 인형 추가
                            UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/finishAllDevelop");
            }
        }

        /// <summary>
        /// 인형편제확대
        /// ("Gun/combineGun")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void Expand(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/combineGun?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "gun_with_user_id": 354794702,
                    "combine": [354797051]
                }
            */
            #endregion
            try
            {
                log.Debug("편제확대");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    long gunWithUserId = Parser.Json.ParseLong(request["gun_with_user_id"]);
                    long[] combine = Parser.Json.ParseLongArray(request["combine"]);

                    UserData.Doll.ExpandLink(gunWithUserId, combine);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/combineGun");
            }
        }

        /// <summary>
        /// 인형강화
        /// ("Gun/eatGun")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void Enhance(string request_string, string response_string)
        {
            #region Packet Example
            /*
                {
                    "gun_with_user_id": 346146275,
                    "item9_num": 0,
                    "food": [
                        353789255,
                        353797897,
                        353719890,
                        353789117,
                        353716908
                    ]
                }
            */
            #endregion
            try
            {
                log.Debug("인형강화");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    long gunWithUserId = Parser.Json.ParseLong(request["gunWithUserId"]);
                    log.Debug("인형 {0} {1} 강화", gunWithUserId, UserData.Doll.GetName(gunWithUserId));
                    int item9Num = Parser.Json.ParseInt(request["item9_num"]);
                    log.Debug("강화캡슐 {0} 개 사용", item9Num);
                    long[] food = Parser.Json.ParseLongArray(request["food"]);
                    foreach (var id in food)
                    {
                        log.Debug("강화재료 - 인형 {0} {1}", id, UserData.Doll.GetName(id));
                        UserData.Doll.Remove(id);
                    }
                    UserData.Doll.notified = false;

                    // 임무 갱신
                    UserData.Quest.Daily.eatGun += 1;
                    UserData.Quest.Weekly.eatGun += 1;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/eatGun");
            }
        }

        /// <summary>
        /// 인형해체
        /// ("Gun/retireGun")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void Retire(string request_string, string response_string)
        {
            #region Packet Example
            // request
            /*
                [353723284,353770579,353719986,353770586,353720974]
            */
            #endregion
            try
            {
                log.Debug("인형해체");
                if (!string.IsNullOrEmpty(request_string))
                {
                    request_string = request_string.Replace("[", "").Replace("]", "");
                    string[] retireIds = request_string.Split(',');
                    foreach (string retireId in retireIds)
                    {
                        long id = Parser.String.ParseLong(retireId);
                        log.Debug("인형 {0} {1} 해체", id, UserData.Doll.GetName(id));

                        // 회수자원
                        int[] collect = UserData.Doll.GetRetireDollCollect(id);
                        UserData.mp += collect[0];
                        UserData.ammo += collect[1];
                        UserData.mre += collect[2];
                        UserData.part += collect[3];

                        UserData.Doll.Remove(id);
                    }
                    UserData.Doll.notified = false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/retireGun");
            }
        }

        /// <summary>
        /// 인형개조
        /// ("Gun/mindupdate")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void MindUpdate(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/mindupdate?uid={0}&outdatacode={1}&req_id={2}
            // {"gun_with_user_id":22091581} 
            // resposne Gun/mindupdate
            // 1
            #endregion
            try
            {
                log.Debug("인형개조");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    long gunWithUserId = Parser.Json.ParseLong(request["gun_with_user_id"]);

                    UserData.Doll.UpgradeMod(gunWithUserId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/mindupdate");
            }
        }

        /// <summary>
        /// 인형제대변경
        /// ("Gun/teamGun")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void ChangeLocation(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/teamGun?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "team_id": 1,
                    "gun_with_user_id": 240595162,
                    "location": 2
                }
            */
            #endregion
            try
            {
                log.Debug("인형 제대변경");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int teamId = Parser.Json.ParseInt(request["team_id"]);
                    int location = Parser.Json.ParseInt(request["location"]);
                    long gunWithUserId = Parser.Json.ParseLong(request["gun_with_user_id"]);

                    UserData.Doll.SwapTeam(teamId, location, gunWithUserId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/teamGun");
            }
        }

        /// <summary>
        /// 인형제대변경 (복수)
        /// ("Gun/teamGuns")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void ChangeLocations(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/teamGuns?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "team_id": 1,
                    "guns": {
                        "6762038": 2,
                        "10545856": 1
                    }
                }
            */
            #endregion
            try
            {
                log.Debug("인형제대변경 (복수)");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int teamId = Parser.Json.ParseInt(request["team_id"]);
                    Dictionary<string, string> guns = Parser.Json.ParseItems(request["guns"].Value<JObject>());
                    foreach (KeyValuePair<string, string> gun in guns)
                    {
                        long gunWithUserId = Parser.String.ParseLong(gun.Key);
                        int location = Parser.String.ParseInt(gun.Value);

                        UserData.Doll.SwapTeam(teamId, location, gunWithUserId);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/teamGuns");
            }
        }

        /// <summary>
        /// 제대교환
        /// ("Gun/exchangeTeam")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void ExchangeEchelon(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/exchangeTeam?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"team_a":3,"team_b":1}
            */
            #endregion
            try
            {
                log.Debug("제대교환");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int teamA = Parser.Json.ParseInt(request["team_a"]);
                    int teamB = Parser.Json.ParseInt(request["team_b"]);

                    UserData.Doll.ExchangeTeam(teamA, teamB);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/exchangeTeam");
            }
        }

        /// <summary>
        /// 제대프리셋
        /// ("Gun/presetToTeam")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void PresetToEchelon(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/presetToTeam?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "team_id": 1,
                    "preset_no": 2,
                    "guns": {
                        "1": {
                            "gun_with_user_id": 345933694,
                            "position": 9
                        },
                        "2": {
                            "gun_with_user_id": 10545856,
                            "position": 13
                        },
                        "3": {
                            "gun_with_user_id": 334655595,
                            "position": 7
                        },
                        "4": {
                            "gun_with_user_id": 100673798,
                            "position": 19
                        },
                        "5": {
                            "gun_with_user_id": 302088121,
                            "position": 8
                        }
                    }
                }
            */
            #endregion
            try
            {
                log.Debug("제대프리셋");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int teamId = Parser.Json.ParseInt(request["team_id"]);
                    if (request.ContainsKey("guns") && request["guns"] is JObject)
                    {
                        JObject guns = request["guns"].Value<JObject>();
                        List<string> items = guns.Properties().Select(p => p.Name).ToList();
                        foreach (string item in items)
                        {
                            int location = Parser.Json.ParseInt(item);
                            long gunWithUserId = Parser.Json.ParseLong(guns, item, "gun_with_user_id");

                            UserData.Doll.SwapTeam(teamId, location, gunWithUserId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/presetToTeam");
            }
        }

        /// <summary>
        /// 인형 스킬훈련 시작
        /// ("Gun/skillUpgrade")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartSkillTrain(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/skillUpgrade
            /*
                {
                    "upgrade_slot": 1
                    "gun_with_user_id": 309875627,
                    "skill": 2,
                    "if_quick": 0,
                }
            */
            // response Gun/skillUpgrade
            // 1
            #endregion
            try
            {
                log.Debug("인형 스킬훈련 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int upgradeSlot = Parser.Json.ParseInt(request["upgrade_slot"]);
                    if (upgradeSlot > 0)
                    {
                        long gunWithUserId = Parser.Json.ParseLong(request["gun_with_user_id"]);
                        int skill = Parser.Json.ParseInt(request["skill"]);
                        int ifQuick = request["if_quick"].Value<int>();
                        int startTime = TimeUtil.GetCurrentSec();

                        // 쾌속이 아닌 경우
                        if (ifQuick != 1)
                        {
                            // 알림 탭 추가
                            dashboardView.Add(new SkillTrainTemplate()
                            {
                                slot = upgradeSlot,
                                skill = skill,
                                gunWithUserId = gunWithUserId,
                                startTime = startTime,
                            });
                        }

                        // 임무 갱신
                        UserData.Quest.Weekly.skillTrain += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "인형 스킬훈련 시작 에러");
            }
        }

        /// <summary>
        /// 인형 스킬훈련 완료
        /// ("Gun/finishUpgrade")
        /// ("Gun/quickUpgrade")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishSkillTrain(string request_string, string response_string)
        {
            #region Packet Example
            // request 
            /*
                {
                    "upgrade_slot": 1
                }
            */
            #endregion
            try
            {
                log.Debug("인형 스킬훈련 완료");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int upgradeSlot = Parser.Json.ParseInt(request["upgrade_slot"]);
                    SkillTrainTemplate template = dashboardView.GetSlotSkillTrain(upgradeSlot);
                    if (template != null && template.gunWithUserId > 0)
                    {
                        long id = template.gunWithUserId;
                        DollWithUserInfo doll = UserData.Doll.Get(id);
                        if (doll != null)
                        {
                            if (template.skill == 1)
                            {
                                doll.skill1 = template.toSkillLevel;
                            }
                            else if (template.skill == 2)
                            {
                                doll.skill2 = template.toSkillLevel;
                            }
                            UserData.Doll.Set(doll);
                        }
                    }

                    // 알림 탭 제거
                    dashboardView.Remove(template);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Gun/finishUpgrade");
            }
        }

        /// <summary>
        /// 인형 잠금
        /// ("Gun/changeLock")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void ChangeLock(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/changeLock?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"lock":[352067064],"unlock":[]}
            */
            // response Gun/changeLock
            // 1
            #endregion
            try
            {
                log.Debug("인형잠금");
            }
            catch (Exception ex)
            {
                log.Error(ex, "인형잠금 처리 실패");
            }
        }

        /// <summary>
        /// 인형 복구
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void RecoverGun(string request_string, string response_string)
        {
            #region Packet Example
            // request Gun/coreRecoverGun
            // {"gun_id":255}
            // response
            // {"gun_with_user_id":376037349} 
            #endregion
            try
            {
                log.Debug("인형복구");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    long gunWithUserId = Parser.Json.ParseLong(response["gun_with_user_id"]);
                    int gunId = Parser.Json.ParseInt(request["gun_id"]);

                    UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "인형복구 처리 실패");
            }
        }

        /// <summary>
        /// 인형 서약
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void Constract(string request_string, string response_string)
        {
            #region Packet Example
            // request
            // {"gun_with_user_id":7010880} 
            // response
            // {"soul_bond_time":1575659520} 
            #endregion
            try
            {
                log.Debug("인형서약");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    long gunWithUserId = Parser.Json.ParseLong(request["gun_with_user_id"]);
                    int soulBondTime = Parser.Json.ParseInt(response["soul_bond_time"]);

                    UserData.Doll.Contract(gunWithUserId, soulBondTime);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "인형서약 처리 실패");
            }
        }
    }
}
