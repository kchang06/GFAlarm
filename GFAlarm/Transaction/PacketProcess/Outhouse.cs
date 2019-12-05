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
    /// <summary>
    /// 시설 패킷 처리
    /// </summary>
    public class Outhouse
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 작전보고서 작성 시작
        /// ("Outhouse/establish_build")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartBattleReport(string request_string, string response_string)
        {
            #region Packet Example
            // request Outhouse/establish_build?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "establish_type": 201,
                    "num": 1,
                    "payway": "build_coin"
                }
                */
            // response Outhouse/establish_build
            /*
                {
                    "build_coin": 3,
                    "gift_item_id": 200001,
                    "exp": 3000,
                    "build_num": 1,
                    "build_tmp_data": [1,200001,3000,"build_coin",3]
                }
            */
            #endregion
            try
            {
                log.Debug("작전보고서 작성");
                JObject response = Parser.Json.ParseJObject(response_string);
                if (response != null)
                {
                    if (response.ContainsKey("exp"))
                    {
                        if (response.ContainsKey("build_num"))
                        {
                            int buildNum = Parser.Json.ParseInt(response["build_num"]);
                            UserData.BattleReport.num = buildNum;
                            UserData.BattleReport.startTime = TimeUtil.GetCurrentSec();
                            //UserData.BattleReport.startTime = Parser.Time.GetCurrentMs();
                            log.Debug("작전보고서 {0} 장 작성", buildNum);
                        }

                        // 자유경험치 반영
                        UserData.GlobalExp.exp -= Parser.Json.ParseInt(response["exp"]);
                        //MainWindow.view.SetGlobalExp(UserData.GlobalExp.exp, UserData.GlobalExp.maxExp);

                        // 알림 여부 해제
                        UserData.GlobalExp.notified = false;
                        UserData.BattleReport.notified = false;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "작전보고서 작성 시작 처리 실패");
            }
        }

        /// <summary>
        /// 작전보고서 작성 완료
        /// ("Outhouse/establish_build_finish")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishBattleReport(string request_string, string response_string)
        {
            #region Packet Example
            // request Outhouse/establish_build_finish?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"establish_type":201,"payway":"build_coin"}
                */
            // response Outhouse/establish_build_finish
            /*
                {"gift":"200001-80"}
            */
            #endregion
            try
            {
                log.Debug("작전보고서 작성 완료");
                JObject response = Parser.Json.ParseJObject(response_string);
                if (response != null)
                {
                    string gift = Parser.Json.ParseString(response["gift"]);
                    if (gift.Contains("-"))
                    {
                        string[] gifts = gift.Split('-');
                        int item = Parser.String.ParseInt(gifts[0]);
                        int number = Parser.String.ParseInt(gifts[1]);

                        switch (item)
                        {
                            // 작전보고서
                            case 200001:
                                UserData.BattleReport.Reset();
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "작전보고서 작성 완료 처리 실패");
            }
        }
    }
}
