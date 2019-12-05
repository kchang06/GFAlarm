using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Util;
using Newtonsoft.Json.Linq;
using NLog;
using System;

namespace GFAlarm.Transaction.PacketProcess
{
    /// <summary>
    /// 상점 패킷 처리
    /// </summary>
    public class Mall
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 상품 구매
        /// ("Mall/gemToGiftbag")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void BuyPackage(string request_string, string response_string)
        {
            #region Packet Example
            // request Mall/gemToGiftbag?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"mall_id":671,"num":1} 
             */
            // response Mall/gemToGiftbag (마스크 구매)
            /*
                {
                    "prize": [
                        {
                            "id": "40085",
                            "name": "prize-10040085",
                            "user_exp": "0",
                            "mp": "0",
                            "ammo": "0",
                            "mre": "0",
                            "part": "0",
                            "core": "0",
                            "gem": "0",
                            "gun_id": "188",
                            "item_ids": "",
                            "furniture": [],
                            "gift": "",
                            "equip_ids": "",
                            "coins": "",
                            "skin": "0",
                            "content": "prize-20040085",
                            "send_limit": "0",
                            "icon": "",
                            "bp_pay": "0",
                            "fairy_ids": "",
                            "chip": [],
                            "commander_uniform": [],
                            "gun_with_user_id": 365710181,
                            "fairys": [],
                            "equips": []
                        }
                    ],
                    "status": 1
                }
             */
            // response Mall/gemToGiftbag (장비 구매)
            /*
                {
                    "prize": [
                        {
                            "id": "519",
                            "name": "prize-10000519",
                            "user_exp": "0",
                            "mp": "0",
                            "ammo": "0",
                            "mre": "0",
                            "part": "0",
                            "core": "0",
                            "gem": "0",
                            "gun_id": "0",
                            "item_ids": "",
                            "furniture": [],
                            "gift": "",
                            "equip_ids": "95",
                            "coins": "",
                            "skin": "0",
                            "content": "prize-20000519",
                            "send_limit": "0",
                            "icon": "",
                            "bp_pay": "0",
                            "fairy_ids": "",
                            "chip": [],
                            "gun_with_user_id": 0,
                            "fairys": [],
                            "equips": [
                                {
                                    "id": "90283119",
                                    "user_id": "621225",
                                    "gun_with_user_id": "0",
                                    "equip_id": "95",
                                    "equip_exp": "0",
                                    "equip_level": "0",
                                    "pow": "1491",
                                    "hit": "0",
                                    "dodge": "0",
                                    "speed": "0",
                                    "rate": "0",
                                    "critical_harm_rate": "0",
                                    "critical_percent": "0",
                                    "armor_piercing": "4897",
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
                            ]
                        }
                    ],
                    "status": 1
                }
             */
            #endregion
            log.Debug("상품 구매");
            try
            {
                JObject response = Parser.Json.ParseJObject(response_string);
                if (response != null)
                {
                    if (response.ContainsKey("prize") && response["prize"] is JArray)
                    {
                        JArray prize = response["prize"].Value<JArray>();
                        foreach (JObject item in prize)
                        {
                            // 인형 구매
                            if (item.ContainsKey("gun_with_user_id"))
                            {
                                long gunWithUserId = Parser.Json.ParseLong(item["gun_with_user_id"]);
                                if (gunWithUserId > 0)
                                {
                                    int gunId = Parser.Json.ParseInt(item["gun_id"]);

                                    UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId));
                                }
                            }
                            // 장비 구매
                            if (item.ContainsKey("equips") && item["equips"] is JArray)
                            {
                                JArray equips = item["equips"].Value<JArray>();
                                foreach (JObject equip in equips)
                                {
                                    long id = Parser.Json.ParseLong(equip["id"]);
                                    if (id > 0)
                                    {
                                        int equipId = Parser.Json.ParseInt(equip["equip_id"]);

                                        UserData.Equip.Add(id, new EquipWithUserInfo(id, equipId));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "상품 구매 처리 실패");
            }
        }

        /// <summary>
        /// 시설 구매
        /// </summary>
        public static void BuyInfra(string request_string, string response_string)
        {
            #region Packet Example
            // request Mall/gemToMax?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"type":1}
            */
            #endregion
            log.Debug("시설 구매");
            try
            {
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int type = Parser.Json.ParseInt(request["type"]);
                    switch (type)
                    {
                        // 병영크기
                        case 1:
                            UserData.maxDollCount += 10;
                            break;
                        // 장비창고
                        case 2:
                            UserData.maxEquipCount += 20;
                            break;
                        // 요정센터
                        case 3:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "시설 구매 처리 실패");
            }
        }
    }
}
