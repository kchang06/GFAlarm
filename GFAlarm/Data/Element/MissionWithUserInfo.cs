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
    "mission_with_user_info": [
        {
            "id": "1076536",
            "user_id": "343650",
            "mission_id": "1",
            "medal1": "1",
            "medal2": "0",
            "bestrank": "5",
            "medal4": "1",
            "counter": "3",
            "win_counter": "2",
            "shortest_in_coinmission": "0.00",
            "type5_score": "0",
            "is_open": "1",
            "is_drop_draw_event": "1",
            "is_close": "0",
            "cycle_win_count": "0",
            "mapped_win_counter": "0"
        },
        {
            "id": "1076714",
            "user_id": "343650",
            "mission_id": "2",
            "medal1": "1",
            "medal2": "0",
            "bestrank": "5",
            "medal4": "1",
            "counter": "15198",
            "win_counter": "13977",
            "shortest_in_coinmission": "0.00",
            "type5_score": "0",
            "is_open": "1",
            "is_drop_draw_event": "1",
            "is_close": "0",
            "cycle_win_count": "0",
            "mapped_win_counter": "0"
        },
     */
    #endregion

    public class MissionWithUserInfo
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public long id = 0;
        public int missionId = 0;

        public short medal1 = 0;
        public short medal2 = 0;
        public short medal4 = 0;
        public short bestrank = 0;
        public double shortestInCoinmission = 0.0;

        public int counter = 0;
        public int winCounter = 0;

        public MissionWithUserInfo(dynamic json)
        {
            try
            {
                this.id = Parser.Json.ParseLong(json["id"]);
                this.missionId = Parser.Json.ParseInt(json["mission_id"]);
                this.counter = Parser.Json.ParseInt(json["counter"]);
                this.winCounter = Parser.Json.ParseInt(json["win_counter"]);
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
