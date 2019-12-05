using GFAlarm.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Data.Element
{
    public class SquadWithUserInfo
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Packet Example
        /*
        "squad_with_user_info": {
            "1215": {
                "id": "1215",
                "user_id": "343650",
                "squad_id": "1",
                "squad_exp": "15000000",
                "squad_level": "100",
                "rank": "5",
                "advanced_level": "3",
                "life": "242",
                "cur_def": "0",
                "ammo": "8",
                "mre": "9",
                "assist_damage": "112",
                "assist_reload": "61",
                "assist_hit": "246",
                "assist_def_break": "277",
                "damage": "0",
                "atk_speed": "0",
                "hit": "0",
                "def": "0",
                "skill1": "10",
                "skill2": "10",
                "skill3": "8"
            },
            "7097": {
                "id": "7097",
                "user_id": "343650",
                "squad_id": "2",
                "squad_exp": "15000000",
                "squad_level": "100",
                "rank": "5",
                "advanced_level": "6",
                "life": "231",
                "cur_def": "0",
                "ammo": "8",
                "mre": "9",
                "assist_damage": "63",
                "assist_reload": "289",
                "assist_hit": "156",
                "assist_def_break": "105",
                "damage": "0",
                "atk_speed": "0",
                "hit": "0",
                "def": "0",
                "skill1": "10",
                "skill2": "10",
                "skill3": "10"
            },
         */
        #endregion

        public long id = 0;                     // 중장비 ID
        public int squadId = 0;                 // 중장비 도감번호
        public long exp = 0;                    // 누적 경험치
        public int level = 0;                   // 레벨
        public int rank = 0;                    // 강화 레벨
        public int advancedRank = 0;            // 강화 레벨 (빨별)

        public short life = 0;                  // 체력

        public short ammo = 0;                  // 탄약
        public short mre = 0;                   // 식량

        public short assistDamage = 0;          // 살상
        public short assistReload = 0;          // 장전
        public short assistHit = 0;             // 정밀
        public short assistDefBreak = 0;        // 파쇄

        public int skill1 = 0;                  // 스킬 1
        public int skill2 = 0;                  // 스킬 2
        public int skill3 = 0;                  // 스킬 3

        public string name = "";                // 중장비 이름

        public SquadWithUserInfo(dynamic json)
        {
            try
            {
                this.id = Parser.Json.ParseLong(json["id"]);
                this.squadId = Parser.Json.ParseInt(json["squad_id"]);
                this.exp = Parser.Json.ParseInt(json["squad_exp"]);
                this.level = Parser.Json.ParseShort(json["squad_level"]);
                this.rank = Parser.Json.ParseShort(json["rank"]);
                this.advancedRank = Parser.Json.ParseShort(json["advanced_level"]);

                this.life = Parser.Json.ParseShort(json["life"]);

                this.ammo = Parser.Json.ParseShort(json["ammo"]);
                this.mre = Parser.Json.ParseShort(json["mre"]);

                this.assistDamage = Parser.Json.ParseShort(json["assist_damage"]);
                this.assistReload = Parser.Json.ParseShort(json["assist_reload"]);
                this.assistHit = Parser.Json.ParseShort(json["assist_hit"]);
                this.assistDefBreak = Parser.Json.ParseShort(json["assist_def_break"]);

                this.skill1 = Parser.Json.ParseShort(json["skill1"]);
                this.skill2 = Parser.Json.ParseShort(json["skill2"]);
                this.skill3 = Parser.Json.ParseShort(json["skill3"]);

                this.name = GameData.Squad.GetData(this.squadId, "name");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
