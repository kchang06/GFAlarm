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
    "chip_with_user_info": {
        "15693": {
            "id": "15693",
            "user_id": "343650",
            "chip_id": "5052",
            "chip_exp": "0",
            "chip_level": "0",
            "color_id": "1",
            "grid_id": "28",
            "squad_with_user_id": "0",
            "position": "0,0",
            "shape_info": "0,0",
            "assist_damage": "0",
            "assist_reload": "0",
            "assist_hit": "2",
            "assist_def_break": "3",
            "damage": "0",
            "atk_speed": "0",
            "hit": "0",
            "def": "0",
            "is_locked": "0"
        },
        "49563": {
            "id": "49563",
            "user_id": "343650",
            "chip_id": "5061",
            "chip_exp": "9680",
            "chip_level": "12",
            "color_id": "2",
            "grid_id": "32",
            "squad_with_user_id": "0",
            "position": "0,0",
            "shape_info": "3,0",
            "assist_damage": "2",
            "assist_reload": "1",
            "assist_hit": "1",
            "assist_def_break": "2",
            "damage": "0",
            "atk_speed": "0",
            "hit": "0",
            "def": "0",
            "is_locked": "0"
        },
     */
    #endregion

    /// <summary>
    /// 보유 칩셋
    /// </summary>
    public class ChipWithUserInfo
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public long id = 0;                                         // 칩셋 ID
        public short chipId = 0;                                    // 칩 ID
        public long squadWithUserId = 0;                            // 장착 중장비 ID

        public int chipExp = 0;                                     // 칩셋 경험치
        public short chipLevel = 0;                                 // 칩셋 강화 레벨 
        public short colorId = 0;                                   // 칩셋 색상
        public short gridId = 0;                                    // 칩 모양
        public short[] shapeInfo = new short[] { 0, 0 };            // 회전상태
        
        public short assistDamage = 0;                              // 살상
        public short assistDefBreak = 0;                            // 파쇄
        public short assistHit = 0;                                 // 정밀
        public short assistReload = 0;                              // 장전
        
        public bool isLocked = false;                               // 잠금 여부

        public ChipWithUserInfo(dynamic json)
        {
            try
            {
                this.id = Parser.Json.ParseLong(json["id"]);
                this.chipId = Parser.Json.ParseShort(json["chip_id"]);
                this.squadWithUserId = Parser.Json.ParseLong(json["squad_with_user_id"]);

                this.chipExp = Parser.Json.ParseInt(json["chip_exp"]);
                this.chipLevel = Parser.Json.ParseShort(json["chip_level"]);
                this.colorId = Parser.Json.ParseShort(json["color_id"]);
                this.gridId = Parser.Json.ParseShort(json["grid_id"]);
                //this.shapeInfo = Parser.Json.ParseString(json["shape_info"]).Split(',').Select(Int16.Parse).ToArray();
                this.assistDamage = Parser.Json.ParseShort(json["assist_damage"]);
                this.assistDefBreak = Parser.Json.ParseShort(json["assist_def_break"]);
                this.assistHit = Parser.Json.ParseShort(json["assist_hit"]);
                this.assistReload = Parser.Json.ParseShort(json["assist_reload"]);
                this.isLocked = Parser.Json.ParseBool(json["is_locked"]);
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
