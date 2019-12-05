using GFAlarm.Data;
using GFAlarm.Util;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Web;

namespace GFAlarm.Proxy
{
    public class RequestPacket
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 인형 장비 착용/해제
        /// </summary>
        /// <param name="ifOutfit">착용여부 (true:장착, false:해제)</param>
        /// <param name="gunWithUserId">인형 고유 ID</param>
        /// <param name="equipWithUserId">장비 고유 ID</param>
        /// <param name="equipSlot">장비 슬롯 (1,2,3)</param>
        /// <returns></returns>        
        public static bool RequestGunEquip(bool ifOutfit, long gunWithUserId, long equipWithUserId, int equipSlot)
        {
            // http://gf-game.girlfrontline.co.kr/index.php/1001/Equip/gunEquip
            // ?uid=343650
            // &outdatacode=o6RZPEsSTaO1yaetN2%2f%2bOGJwZ7983k0ZOGGXkfnCDColt8B0pAz7vlORDkPABVMcXDlZD6JShAPfIhdR5hcu7XpDErwY8%2bD0dUxJP2uc%2fVoevJv%2bqkZXGisWb%2f8j5sGl4vPJKKD9PJN7EJDI3xeZj0Tl
            // &req_id=156268781500033
            // {"if_outfit":1,"gun_with_user_id":345595000,"equip_with_user_id":15370720,"equip_slot":1}
            // {"if_outfit":0,"gun_with_user_id":345595000,"equip_with_user_id":15370720,"equip_slot":1}

            JObject json = new JObject();
            json.Add("if_outfit", ifOutfit == true ? 1 : 0);
            json.Add("gun_with_user_id", gunWithUserId);
            json.Add("equip_with_user_id", equipWithUserId);
            json.Add("equip_slot", equipSlot);

            string gunName = "";
            string equipName = "";
            //string gunName = GameData.GetDollData(UserData.Doll.dictionary[gunWithUserId].gunId, "name");
            //string equipName = GameData.GetEquipData(UserData.Equip.dictionary[equipWithUserId].equipId, "name");

            log.Info("인형 {0} {1} 슬롯 장비 {2} {3}", gunName, equipSlot, equipName, ifOutfit == true ? "장착" : "해제");
            string jsonString = json.ToString(Newtonsoft.Json.Formatting.None);
            string outdatacode = AuthCode.Encode(jsonString, UserData.sign);
            string requestString = String.Format("uid={0}&outdatacode={1}&req_id={2}", UserData.userId, HttpUtility.UrlEncode(outdatacode), ++UserData.req_id);

            string responseString = DoPost(UserData.gameHost + "Equip/gunEquip", requestString);

            return true;
        }

        public static string DoPost(string url, string data)
        {
            log.Info("url:{0} data:{1}", url, data);

            try
            {
                //WebClient wc = new WebClient();
                //wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                //wc.Encoding = Encoding.UTF8;
                //byte[] postData = wc.Encoding.GetBytes(data);
                //string result = Encoding.UTF8.GetString(wc.UploadData(url, "POST", postData));
                //return result;
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
            return "";
        }
    }
}
