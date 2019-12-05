using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Notifier;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using LocalizationResources;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using static GFAlarm.MainWindow;

namespace GFAlarm.Transaction.PacketProcess
{
    /// <summary>
    /// 장비 패킷 처리
    /// </summary>
    public class Equip
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 장비제조 시작
        /// ("Equip/develop")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartProduce(string request_string, string response_string)
        {
            #region Packet Example
            /*  제조
                request Equip/develop
                {
                    "mp": 120,
                    "ammo": 92,
                    "mre": 60,
                    "part": 152,
                    "build_slot": 1,
                    "input_level": 0
                }
                response Equip/develop
                {
                    "type": 0,
                    "equip_id": 88
                }
            */
            #endregion
            try
            {
                log.Debug("장비제조 시작");
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

                        UserData.mp -= mp;
                        UserData.ammo -= ammo;
                        UserData.mre -= mre;
                        UserData.part -= part;

                        log.Debug("투입자원 {0}/{1}/{2}/{3}", mp, ammo, mre, part);
                        log.Debug("투입레벨 {0} (0: 일반, 1: 소투입, 2: 중투입, 3: 고투입)", inputLevel);

                        int equipId = Parser.Json.ParseInt(response["equip_id"]);
                        int fairyId = Parser.Json.ParseInt(response["fairy_id"]);

                        log.Debug("제조슬롯 {0} | 장비 {1} | 요정 {2}", buildSlot, GameData.Equip.GetName(equipId), GameData.Fairy.GetName(fairyId));

                        // 알림 탭 추가
                        ProduceEquipTemplate template = new ProduceEquipTemplate()
                        {
                            slot = buildSlot,
                            equipId = equipId,
                            fairyId = fairyId,
                            inputLevel = inputLevel,
                            spendResource = new int[] { mp, ammo, mre, part },
                            startTime = TimeUtil.GetCurrentSec(),
                            //startTime = Parser.Time.GetCurrentMs(),
                        };
                        dashboardView.Add(template);

                        string equipName = template.equipName;
                        string equipNameShort = template.equipNameShort;
                        string fairyName = template.fairyName;
                        string category = template.category;
                        string type = template.type;
                        int star = template.star;

                        if ((Config.Alarm.notifyProduceEquip5Star && star >= 5) ||     // 5성 장비제조 알림
                            (Config.Alarm.notifyProduceFairy && type == "요정") ||     // 요정제조 알림
                            (Config.Alarm.notifyProduceEquip))                         // 장비제조 알림
                        {
                            string content = "";
                            if (type == "요정")
                            {
                                content = string.Format(LanguageResources.Instance["MESSAGE_PRODUCE_FAIRY_CONTENT"], fairyName);
                            }
                            else
                            {
                                content = string.Format(LanguageResources.Instance["MESSAGE_PRODUCE_EQUIP_CONTENT"], star, equipNameShort);
                            }
                            Notifier.Manager.notifyQueue.Enqueue(new Message()
                            {
                                send = MessageSend.All,
                                subject = LanguageResources.Instance["MESSAGE_PRODUCE_EQUIP_SUBJECT"],
                                content = content,
                            });
                        }

                        // 일반제조
                        if (inputLevel == 0)
                        {
                            // 임무 갱신
                            UserData.Quest.Daily.produceEquip += 1;
                            UserData.Quest.Weekly.produceEquip += 1;
                        }
                        // 중형제조
                        else if (inputLevel > 0)
                        {
                            // 임무 갱신
                            UserData.Quest.Weekly.produceHeavyEquip += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Equip/develop");
            }
        }

        /// <summary>
        /// 장비제조 완료
        /// ("Equip/finishDevelop")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishProduce(string request_string, string response_string)
        {
            #region Packet Example
            // request Equip/finishDevelop?uid={0}&outdatacode={1}&req_id={2}
            // {"build_slot":1}
            /*
                {
                    "equip_with_user": {
                        "id": "90294642",
                        "user_id": "343650",
                        "gun_with_user_id": "0",
                        "equip_id": "63",
                        "equip_exp": "0",
                        "equip_level": "0",
                        "pow": "0",
                        "hit": "0",
                        "dodge": "1190",
                        "speed": "0",
                        "rate": "0",
                        "critical_harm_rate": "0",
                        "critical_percent": "3363",
                        "armor_piercing": "0",
                        "armor": "0",
                        "shield": "0",
                        "damage_amplify": "0",
                        "damage_reduction": "0",
                        "night_view_percent": "0",
                        "bullet_number_up": "0",
                        "adjust_count": "0",
                        "is_locked": "0",
                        "last_adjust": ""
                    }
                }
            */
            /*
                {"fairy_with_user":1182216}
            */
            #endregion
            try
            {
                log.Debug("장비제조 완료");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    int buildSlot = Parser.Json.ParseInt(request["build_slot"]);
                    if (response.ContainsKey("equip_with_user") && response["equip_with_user"] is JObject)
                    {
                        long equipWithUserId = Parser.Json.ParseLong(response, "equip_with_user", "id");
                        int equipId = Parser.Json.ParseInt(response, "equip_with_user", "equip_id");

                        log.Debug("제조슬롯 {0} | 장비 {1} | 장비번호 {2}", buildSlot, GameData.Equip.GetName(equipId), equipWithUserId);

                        UserData.Equip.Add(equipWithUserId, new EquipWithUserInfo(equipWithUserId, equipId));
                    }
                    else if (response.ContainsKey("fairy_with_user"))
                    {
                        long fairyWithUserId = Parser.Json.ParseLong(response["fairy_with_user"]);

                        log.Debug("제조슬롯 {0} | 요정번호 {1}", buildSlot, fairyWithUserId);
                    }

                    // 알림 탭 제거
                    dashboardView.Remove(new ProduceEquipTemplate()
                    {
                        slot = buildSlot,
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Equip/finishDevelop");
            }
        }

        /// <summary>
        /// 장비일괄제조 시작
        /// ("Equip/developMulti")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartMultiProduce(string request_string, string response_string)
        {
            #region Packet Example
            /*  request /index.php/1001/Equip/developMulti
                {
                    "mp": 120,
                    "ammo": 92,
                    "mre": 60,
                    "part": 152,
                    "input_level": 0,
                    "build_quick": 0,
                    "build_multi": 3,
                    "build_heavy": 0
                }
                response /index.php/1001/Equip/developMulti
                {
                    "equip_ids": [
                        {
                            "info": {
                                "type": 0,
                                "equip_id": 63
                            },
                            "slot": 3
                        },
                        {
                            "info": {
                                "type": 0,
                                "equip_id": 65
                            },
                            "slot": 5
                        },
                        {
                            "info": {
                                "type": 0,
                                "equip_id": 37
                            },
                            "slot": 7
                        }
                    ]
                }
            */
            #endregion
            try
            {
                log.Debug("장비제조 일괄시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    int mp = Parser.Json.ParseInt(request["mp"]);
                    int ammo = Parser.Json.ParseInt(request["ammo"]);
                    int mre = Parser.Json.ParseInt(request["mre"]);
                    int part = Parser.Json.ParseInt(request["part"]);
                    int inputLevel = Parser.Json.ParseInt(request["input_level"]);

                    log.Debug("투입자원 {0}/{1}/{2}/{3}", mp, ammo, mre, part);
                    log.Debug("투입레벨 {0} (0: 일반, 1: 소투입, 2: 중투입, 3: 고투입)", inputLevel);

                    if (response.ContainsKey("equip_ids") && response["equip_ids"] is JArray)
                    {
                        JArray items = response["equip_ids"].Value<JArray>();
                        foreach (var item in items)
                        {
                            try
                            {
                                int buildSlot = Parser.Json.ParseInt(item["slot"]);
                                if (buildSlot > 0)
                                {
                                    UserData.mp -= mp;
                                    UserData.ammo -= ammo;
                                    UserData.mre -= mre;
                                    UserData.part -= part;

                                    //int type = Parser.Json.ParseInt(item, "info", "type");
                                    int equipId = Parser.Json.ParseInt(item, "info", "equip_id");
                                    int fairyId = Parser.Json.ParseInt(item, "info", "fairy_id");

                                    log.Debug("제조슬롯 {0} | 장비 {1} | 요정 {2}", buildSlot, GameData.Equip.GetName(equipId), GameData.Fairy.GetName(fairyId));

                                    // 알림 탭 추가
                                    ProduceEquipTemplate template = new ProduceEquipTemplate()
                                    {
                                        slot = buildSlot,
                                        equipId = equipId,
                                        fairyId = fairyId,
                                        inputLevel = inputLevel,
                                        spendResource = new int[] { mp, ammo, mre, part },
                                        startTime = TimeUtil.GetCurrentSec(),
                                        //startTime = Parser.Time.GetCurrentMs(),
                                    };
                                    dashboardView.Add(template);

                                    string equipName = template.equipName;
                                    string equipNameShort = template.equipNameShort;
                                    string fairyName = template.fairyName;
                                    string type = template.type;
                                    string category = template.category;
                                    int star = template.star;

                                    if ((Config.Alarm.notifyProduceEquip5Star && star >= 5) ||     // 5성 장비제조 알림
                                        (Config.Alarm.notifyProduceFairy && type == "요정") ||     // 요정제조 알림
                                        (Config.Alarm.notifyProduceEquip))                         // 장비제조 알림
                                    {
                                        string content = "";
                                        if (type == "요정")
                                        {
                                            content = string.Format(LanguageResources.Instance["MESSAGE_PRODUCE_FAIRY_CONTENT"], fairyName);
                                        }
                                        else
                                        {
                                            content = string.Format(LanguageResources.Instance["MESSAGE_PRODUCE_EQUIP_CONTENT"], star, equipNameShort);
                                        }
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            subject = LanguageResources.Instance["MESSAGE_PRODUCE_EQUIP_SUBJECT"],
                                            content = content,
                                        });
                                    }

                                    // 일반제조인 경우
                                    if (inputLevel == 0)
                                    {
                                        // 임무 갱신
                                        UserData.Quest.Daily.produceEquip += 1;
                                        UserData.Quest.Weekly.produceEquip += 1;
                                    }
                                    // 중형제조인 경우
                                    else if (inputLevel > 0)
                                    {
                                        // 임무 갱신
                                        UserData.Quest.Weekly.produceHeavyEquip += 1;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex, "failed to get Equip/developMulti");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "장비제조 일괄시작 에러");
            }
        }

        /// <summary>
        /// 장비일괄제조 완료
        /// ("Equip/finishAllDevelop")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishMultiProduce(string request_string, string response_string)
        {
            #region Packet Example
            /*
                {
                    "equip_with_user_add_list": [
                        {
                            "build_slot": 1,
                            "equip_with_user": {
                                "id": "90835140",
                                "user_id": "621225",
                                "gun_with_user_id": "0",
                                "equip_id": "43",
                                "equip_exp": "0",
                                "equip_level": "0",
                                "pow": "0",
                                "hit": "0",
                                "dodge": "4116",
                                "speed": "0",
                                "rate": "0",
                                "critical_harm_rate": "0",
                                "critical_percent": "0",
                                "armor_piercing": "0",
                                "armor": "3815",
                                "shield": "0",
                                "damage_amplify": "0",
                                "damage_reduction": "0",
                                "night_view_percent": "0",
                                "bullet_number_up": "0",
                                "adjust_count": "0",
                                "is_locked": "0",
                                "last_adjust": ""
                            },
                            "equip_id": "43"
                        },
                        {
                            "build_slot": 3,
                            "equip_with_user": {
                                "id": "90835141",
                                "user_id": "621225",
                                "gun_with_user_id": "0",
                                "equip_id": "42",
                                "equip_exp": "0",
                                "equip_level": "0",
                                "pow": "0",
                                "hit": "0",
                                "dodge": "4898",
                                "speed": "0",
                                "rate": "0",
                                "critical_harm_rate": "0",
                                "critical_percent": "0",
                                "armor_piercing": "0",
                                "armor": "3850",
                                "shield": "0",
                                "damage_amplify": "0",
                                "damage_reduction": "0",
                                "night_view_percent": "0",
                                "bullet_number_up": "0",
                                "adjust_count": "0",
                                "is_locked": "0",
                                "last_adjust": ""
                            },
                            "equip_id": "42"
                        }
                    ],
                    "fairy_with_user_add_list": [],
                    "cost_item3_num": 2
                }
                */
            #endregion
            try
            {
                log.Debug("장비일괄제조 완료");
                JObject response = Parser.Json.ParseJObject(response_string);
                if (response["equip_with_user_add_list"] != null && response["equip_with_user_add_list"] is JArray)
                {
                    JArray items = response["equip_with_user_add_list"].Value<JArray>();
                    foreach (JToken item in items)
                    {
                        int buildSlot = Parser.Json.ParseInt(item["build_slot"]);
                        long equipWithUserId = Parser.Json.ParseLong(item, "equip_with_user", "id");
                        int equipId = Parser.Json.ParseInt(item, "equip_with_user", "equip_id");

                        log.Debug("제조슬롯 {0} | 장비 {1} | 장비번호 {2}", buildSlot, GameData.Equip.GetName(equipId), equipWithUserId);

                        // 알림 탭 제거
                        dashboardView.Remove(new ProduceEquipTemplate()
                        {
                            slot = buildSlot,
                        });
                        UserData.Equip.Add(equipWithUserId, new EquipWithUserInfo(equipWithUserId, equipId));
                    }
                }
                if (response["fairy_with_user_add_list"] != null && response["fairy_with_user_add_list"] is JArray)
                {
                    JArray items = response["fairy_with_user_add_list"].Value<JArray>();
                    foreach (JToken item in items)
                    {
                        int buildSlot = Parser.Json.ParseInt(item["build_slot"]);
                        long fairyWithUserId = Parser.Json.ParseLong(item["fairy_with_user_id"]);
                        int fairyId = Parser.Json.ParseInt(item["fairy_id"]);

                        log.Debug("제조슬롯 {0} | 요정 {1} | 요정번호 {2}", buildSlot, GameData.Fairy.GetName(fairyId), fairyWithUserId);

                        // 알림 탭 제거
                        dashboardView.Remove(new ProduceEquipTemplate()
                        {
                            slot = buildSlot,
                        });
                        UserData.Fairy.Add(fairyWithUserId, new FairyWithUserInfo(fairyWithUserId, fairyId));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Equip/finishAllDevelop");
            }
        }

        /// <summary>
        /// 장비강화
        /// ("Equip/eatEquip")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void Enhance(string request_string, string response_string)
        {
            #region Packet Example
            // request
            /*
                {
                    "equip_with_user_id": 88239740,
                    "food": [
                        89901501,
                        89901440
                    ]
                }
            */
            #endregion
            try
            {
                log.Debug("장비강화");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    long equipWithUserId = Parser.Json.ParseLong(request["equip_with_user_id"]);
                    long[] food = Parser.Json.ParseLongArray(request["food"]);
                    foreach (long id in food)
                    {
                        UserData.Equip.Remove(id);
                    }
                    UserData.Equip.notified = false;

                    // 임무 갱신
                    UserData.Quest.Daily.eatEquip += 1;
                    UserData.Quest.Weekly.eatEquip += 1;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Equip/eatEquip");
            }
        }

        /// <summary>
        /// 장비해체
        /// ("Equip/retire")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void Retire(string request_string, string response_string)
        {
            #region Packet Example
            // request
            /*
                {
                    "equips": [
                        89901399,
                        89902177
                    ]
                }
            */
            #endregion
            try
            {
                log.Debug("장비해체");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    long[] equips = Parser.Json.ParseLongArray(request["equips"]);
                    foreach (long id in equips)
                    {
                        UserData.Equip.Remove(id);
                    }
                    UserData.Equip.notified = false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Equip/retire");
            }
        }

        /// <summary>
        /// 장비교정
        /// ("Equip/adjust")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void Calibrate(string request_string, string response_string)
        {
            #region Packet Example
            // request
            // {"equip_with_user_id":93896845}
            // response
            // {"critical_harm_rate":10000,"dodge":5000} 
            #endregion
            try
            {
                log.Debug("장비교정");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    UserData.Quest.Weekly.adjustEquip += 1;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Equip/adjust");
            }
        }
    }
}
