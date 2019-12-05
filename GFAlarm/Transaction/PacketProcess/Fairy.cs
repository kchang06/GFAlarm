using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GFAlarm.MainWindow;

namespace GFAlarm.Transaction.PacketProcess
{
    /// <summary>
    /// 요정 패킷 처리
    /// </summary>
    public class Fairy
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 요정강화
        /// ("Fairy/eatFairy")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name=""></param>
        public static void Enhance(string request_string, string response_string)
        {
            #region Packet Example
            // request
            // {"fairy_with_user_id":884088,"food":[1241429]} 
            // response
            // {"add_fairy_exp":10} 
            #endregion
            try
            {
                log.Debug("요정강화");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    long fairyWithUserId = Parser.Json.ParseLong(request["fairy_with_user_id"]);
                    long[] foods = Parser.Json.ParseLongArray(request["food"]);
                    foreach (long food in foods)
                    {
                        UserData.Fairy.Remove(new FairyWithUserInfo(food, 0));
                    }
                }
                UserData.Quest.Weekly.eatFairy += 1;
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Fairy/eatFairy");
            }
        }

        /// <summary>
        /// 요정교정
        /// ("Fairy/adjust")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void Calibrate(string request_string, string response_string)
        {
            #region Packet Example
            // request 
            // {"fairy_with_user_id":358484} 
            // response
            // {"passive_skill":910107} 
            #endregion
            try
            {
                log.Debug("요정교정");

                // 소비자원
                UserData.mp -= 600;
                UserData.ammo -= 600;
                UserData.mre -= 600;
                UserData.part -= 200;

                UserData.Quest.Weekly.adjustFairy += 1;
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Fairy/adjust");
            }
        }

        /// <summary>
        /// 요정교정 확정
        /// ("Fairy/adjustConfirm")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void CalibrateConfirm(string request_string, string response_string)
        {
            #region Packet Example
            // request
            // {"fairy_with_user_id":1275724} 
            // response
            // 1
            #endregion
            try
            {
                log.Debug("요정교정 확정");
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Fairy/adjustConfirm");
            }
        }

        /// <summary>
        /// 요정제대변경
        /// ("Fairy/teamFairy")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void ChangeLocation(string request_string, string response_string)
        {
            #region Packet Example
            // request Fairy/teamFairy?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"team_id":1,"fairy_with_user_id":320898} 
            */
            #endregion
            try
            {
                log.Debug("요정 제대변경");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int teamId = Parser.Json.ParseInt(request["team_id"]);
                    long fairyWithUserId = Parser.Json.ParseLong(request["fairy_with_user_id"]);

                    UserData.Fairy.SwapTeam(teamId, fairyWithUserId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Fairy/teamFairy");
            }
        }

        /// <summary>
        /// 요정 스킬훈련 시작
        /// ("Fairy/skillUpgrade")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartSkillTrain(string request_string, string response_string)
        {
            #region Packet Example
            // request Fairy/skillUpgrade
            /*
                {
                    "upgrade_slot":1,
                    "fairy_with_user_id":17190,
                    "if_quick":true
                }
            */
            #endregion
            try
            {
                log.Debug("요정 스킬훈련 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int upgradeSlot = Parser.Json.ParseInt(request["upgrade_slot"]);
                    if (upgradeSlot > 0)
                    {
                        long fairyWithUserId = Parser.Json.ParseLong(request["fairy_with_user_id"]);
                        int skill = Parser.Json.ParseInt(request["skill"]);
                        int ifQuick = request["if_quick"].Value<int>();
                        int startTime = TimeUtil.GetCurrentSec();

                        // 쾌속이 아닌 경우
                        if (ifQuick != 1)
                        {
                            // 알림 탭 추가
                            dashboardView.Add(new SkillTrainTemplate()
                            {
                                slot = upgradeSlot,
                                skill = skill,
                                fairyWithUserId = fairyWithUserId,
                                startTime = startTime,
                            });
                        }

                        // 임무 갱신
                        UserData.Quest.Weekly.skillTrain += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "요정 스킬훈련 시작 에러");
            }
        }

        /// <summary>
        /// 요정 스킬훈련 완료
        /// ("Fairy/finishUpgrade")
        /// ("Fairy/quickUpgrade")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishSkillTrain(string request_string, string response_string)
        {
            #region Packet Example
            // request 
            /*
                {
                    "upgrade_slot": 1
                }
            */
            #endregion
            try
            {
                log.Debug("요정 스킬훈련 완료");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int upgradeSlot = Parser.Json.ParseInt(request["upgrade_slot"]);
                    SkillTrainTemplate template = dashboardView.GetSlotSkillTrain(upgradeSlot);
                    if (template != null && template.fairyWithUserId > 0)
                    {
                        long id = template.fairyWithUserId;
                        FairyWithUserInfo fairy = UserData.Fairy.Get(id);
                        if (fairy != null)
                        {
                            fairy.skill = template.toSkillLevel;
                            UserData.Fairy.Set(fairy);
                        }
                    }

                    // 알림 탭 제거
                    dashboardView.Remove(template);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Fairy/finishUpgrade");
            }
        }
    }
}
