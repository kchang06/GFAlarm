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
        "item_with_user_info": [
            {
                "item_id": "507",
                "number": "98"		// 자유경험치
            }
     */
    #endregion

    /// <summary>
    /// 보유 아이템
    /// </summary>
    public class ItemWithUserInfo
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public long id = 0;
        public int itemId = 0;
        public int number = 0;

        public ItemWithUserInfo(dynamic json)
        {
            try
            {
                this.id = Parser.Json.ParseLong(json["id"]);
                this.itemId = Parser.Json.ParseInt(json["item_id"]);
                this.number = Parser.Json.ParseInt(json["number"]);
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
