using GFAlarm.Util;
using Newtonsoft.Json.Linq;
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
    "equip_with_user_info": {
        "577001": {
            "id": "577001",
            "user_id": "343650",
            "gun_with_user_id": "334655595",
            "equip_id": "40",
            "equip_exp": "10025",
            "equip_level": "10",
            "pow": "10000",
            "hit": "0",
            "dodge": "10000",
            "speed": "0",
            "rate": "0",
            "critical_harm_rate": "0",
            "critical_percent": "0",
            "armor_piercing": "0",
            "armor": "0",
            "shield": "0",
            "damage_amplify": "0",
            "damage_reduction": "0",
            "night_view_percent": "0",
            "bullet_number_up": "0",
            "adjust_count": "6",
            "is_locked": "1",
            "last_adjust": "{\"dodge\":8000,\"pow\":10000}"
        },
        "989685": {
            "id": "989685",
            "user_id": "343650",
            "gun_with_user_id": "197462247",
            "equip_id": "32",
            "equip_exp": "10050",
            "equip_level": "10",
            "pow": "10000",
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
            "night_view_percent": "0",
            "bullet_number_up": "0",
            "adjust_count": "5",
            "is_locked": "1",
            "last_adjust": "{\"pow\":\"7500\"}"
        },
     */
    #endregion

    /// <summary>
    /// 보유 장비
    /// </summary>
    public class EquipWithUserInfo
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public long id = 0;                 // 장비 ID
        public long gunWithUserId = 0;      // 착용 인형 ID
        public int equipId = 0;             // 장비 도감번호

        public int equipExp = 0;           // 장비 경험치
        public short equipLevel = 0;          // 장비 강화 레벨

        public short pow = 0;                 // 화력
        public short hit = 0;                 // 명중
        public short dodge = 0;               // 회피
        public short speed = 0;               // 이동속도
        public short rate = 0;                // 사속
        public short criticalPercent = 0;     // 치명률
        public short criticalHarmRate = 0;    // 치명상
        public short armorPiercing = 0;       // 관통
        public short armor = 0;               // 장갑
        public short shield = 0;              // 보호막
        public short damageAmplify = 0;       // 피해 증폭
        public short damageReduction = 0;     // 피해 감쇄
        public short nightViewPercent = 0;    // 야시능력
        public short bulletNumberUp = 0;      // 장탄수

        public short adjustCount = 0;         // 교정 횟수
        public bool isLocked = false;            // 잠김 여부

        public EquipWithUserInfo(dynamic json)
        {
            try
            {
                this.id = Parser.Json.ParseLong(json["id"]);
                this.gunWithUserId = Parser.Json.ParseLong(json["gun_with_user_id"]);
                this.equipId = Parser.Json.ParseInt(json["equip_id"]);
                this.equipExp = Parser.Json.ParseInt(json["equip_exp"]);
                this.equipLevel = Parser.Json.ParseShort(json["equip_level"]);

                this.pow = Parser.Json.ParseShort(json["pow"]);
                this.hit = Parser.Json.ParseShort(json["hit"]);
                this.dodge = Parser.Json.ParseShort(json["dodge"]);
                this.speed = Parser.Json.ParseShort(json["speed"]);
                this.rate = Parser.Json.ParseShort(json["rate"]);
                this.criticalPercent = Parser.Json.ParseShort(json["critical_percent"]);
                this.criticalHarmRate = Parser.Json.ParseShort(json["critical_harm_rate"]);
                this.armorPiercing = Parser.Json.ParseShort(json["armor_piercing"]);
                this.armor = Parser.Json.ParseShort(json["armor"]);
                this.shield = Parser.Json.ParseShort(json["shield"]);
                this.damageAmplify = Parser.Json.ParseShort(json["damage_amplify"]);
                this.damageReduction = Parser.Json.ParseShort(json["damage_reduction"]);
                this.nightViewPercent = Parser.Json.ParseShort(json["night_view_percent"]);
                this.bulletNumberUp = Parser.Json.ParseShort(json["bullet_number_up"]);

                this.adjustCount = Parser.Json.ParseShort(json["adjust_count"]);
                //this.isLocked = Parser.Json.ParseBool(json["is_locked"]);
                this.isLocked = Parser.Json.ParseInt(json["is_locked"]) == 1 ? true : false;
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }

        public EquipWithUserInfo(long equipWithUserId, int equipId)
        {
            try
            {
                this.id = equipWithUserId;
                this.equipId = equipId;
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
