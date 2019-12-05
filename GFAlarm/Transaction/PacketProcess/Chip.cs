using GFAlarm.Data;
using GFAlarm.Util;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Transaction.PacketProcess
{
    public class Chip
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 칩셋강화
        /// ("Chip/strengthen")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void Enhance(string request_string, string response_string)
        {
            #region Packet Example
            // request
            // {"user_id":"343650","chip_with_user_id":7554346,"foods":[17143214,17152522,17122150,17152527,17179148,17122147,17159869,17159874,17122144,17117089,17122145,17159870,17143210,17117091,17117088,17117093,17191322,17156002,17152526,17147176,17159867,17156010,17152528,17156004,17152529,17143216,17147173,17147177,17147179,17159876,17159872,17191327,17191328]} 
            // response
            // {"chip_add_exp":4500,"mp":-900,"ammo":-900,"mre":-900,"part":-316} 
            #endregion
            try
            {
                log.Debug("칩셋강화");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    long chipWithUserId = Parser.Json.ParseLong(request["chip_with_user_id"]);
                    long[] foods = Parser.Json.ParseLongArray(request["foods"]);
                    foreach (long food in foods)
                    {
                        UserData.Quest.Weekly.eatChip += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "칩셋강화 처리 실패");
            }
        }
    }
}
