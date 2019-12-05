using GFAlarm.Util;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Data.Element
{
    #region Packet Example
    /*
        {
            "id": "56184523",
            "user_id": "0",
            "type": "6",
            "sub_id": "0",
            "user_exp": "0",
            "mp": "3000",
            "ammo": "3000",
            "mre": "3000",
            "part": "3000",
            "core": "0",
            "gem": "0",
            "gun_id": "0",
            "fairy_ids": "",
            "item_ids": "41-30",
            "equip_ids": "",
            "furniture": "",
            "gift": "",
            "coins": "",
            "skin": "0",
            "commander_uniform": "",
            "bp_pay": "0",
            "chip": "",
            "title": "泡面番追番奖励",
            "content": "亲爱的指挥官：\r\n《少女前线》泡面番已于7月28日12：00在B站正式开播，开播前追番人数达到20万，特别发放：四项资源*3000，采购币*30，还请查收。\r\n\r\n\r\n《少女前线》运营团队",
            "code": "",
            "start_time": "1564372082",
            "end_time": "1564761599",
            "if_read": "0"
        },
        {
            "id": "56565473",
            "user_id": "1350083",
            "type": "10",
            "sub_id": "133",
            "user_exp": "30",
            "mp": "30",
            "ammo": "30",
            "mre": "30",
            "part": "10",
            "core": "0",
            "gem": "0",
            "gun_id": "29",
            "fairy_ids": "",
            "item_ids": "",
            "equip_ids": "",
            "furniture": "",
            "gift": "",
            "coins": "",
            "skin": "0",
            "commander_uniform": "",
            "bp_pay": "0",
            "chip": "",
            "title": "main_quest-10000133",
            "content": "main_quest-20000133",
            "code": "",
            "start_time": "1564682406",
            "end_time": "2100000000",
            "if_read": "0"
        },
     */
    #endregion

    public class MailWithUserInfo
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public long id = 0;
        public long userId = 0;
        public int type = 0;
        public int subId = 0;
        public long userExp = 0;
        public int mp = 0;
        public int ammo = 0;
        public int mre = 0;
        public int part = 0;
        public int core = 0;
        public int gem = 0;
        public int gunId = 0;
        public int[] fairyIds = new int[] { };
        public int[] itemIds = new int[] { };
        public int[] equipIds = new int[] { };
        
        public MailWithUserInfo(dynamic json)
        {
            try
            {
                if (json.ContainsKey("id"))
                    this.id = Parser.Json.ParseLong(json["id"]);
                if (json.ContainsKey("gun_id"))
                {
                    string gunIdString = Parser.Json.ParseString(json["gun_id"]);
                    if (!string.IsNullOrEmpty(gunIdString))
                    {
                        this.gunId = Parser.String.ParseInt(gunIdString);
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
