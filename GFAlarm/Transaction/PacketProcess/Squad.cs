using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using static GFAlarm.MainWindow;

namespace GFAlarm.Transaction.PacketProcess
{
    /// <summary>
    /// 중장비 패킷 처리
    /// </summary>
    public class Squad
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 중장비 스킬훈련 시작
        /// ("Squad/upSkill")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartSkillTrain(string request_string, string response_string)
        {
            #region Packet Example
            // request Squad/upSkill?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "squad_with_user_id": 28314,
                    "skill_slot": 1,
                    "skill": 3,
                    "if_quick": false
                }
            */
            #endregion
            try
            {
                log.Debug("중장비 스킬훈련 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int skillSlot = Parser.Json.ParseInt(request["skill_slot"]);
                    if (skillSlot > 0)
                    {
                        long squadWithUserId = Parser.Json.ParseLong(request["squad_with_user_id"]);
                        int skill = Parser.Json.ParseInt(request["skill"]);
                        bool ifQuick = Parser.Json.ParseBool(request["if_quick"]);
                        // 쾌속이 아닌 경우
                        if (ifQuick != true)
                        {
                            // 알림 탭 추가
                            dashboardView.Add(new SkillTrainTemplate()
                            {
                                slotType = 1,
                                slot = skillSlot,
                                skill = skill,
                                squadWithUserId = squadWithUserId,
                                startTime = TimeUtil.GetCurrentSec()
                            }); ;
                        }

                        // 임무 갱신
                        UserData.Quest.Weekly.skillTrain += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "중장비 스킬훈련 시작 처리 실패");
            }
        }

        /// <summary>
        /// 중장비 스킬훈련 완료
        /// ("Squad/finishSkill")
        /// ("Squad/quickSkill")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishSkillTrain(string request_string, string response_string)
        {
            #region Packet Example
            // request Squad/finishSkill?uid={1}&outdatacode={2}&req_id={3}
            /*
                {
                    "skill_slot":1
                }
            */
            #endregion
            try
            {
                log.Debug("중장비 스킬훈련 완료");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int skillSlot = Parser.Json.ParseInt(request["skill_slot"]);
                    SkillTrainTemplate template = dashboardView.GetSlotSkillTrain(skillSlot, 1);
                    if (template != null && template.squadWithUserId > 0)
                    {
                        long id = template.squadWithUserId;
                        SquadWithUserInfo squad = UserData.Squad.Get(id);
                        if (squad != null)
                        {
                            switch (template.skill)
                            {
                                case 1:
                                    squad.skill1 = template.toSkillLevel;
                                    break;
                                case 2:
                                    squad.skill2 = template.toSkillLevel;
                                    break;
                                case 3:
                                    squad.skill3 = template.toSkillLevel;
                                    break;
                            }
                            UserData.Squad.Set(squad);
                        }
                    }

                    // 알림 탭 제거
                    dashboardView.Remove(template);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "중장비 스킬훈련 완료 처리 실패");
            }
        }

        /// <summary>
        /// 중장비 경험훈련 시작
        /// ("Squad/train")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartExpTrain(string request_string, string response_string)
        {
            #region Packet Example
            // request
            /*
                {
                    "squad_with_user_id": 38664,
                    "train_slot": 1,
                    "num": 12
                }
                */
            // response
            /*
                {
                    "user_id": 343650,
                    "squad_with_user_id": 38664,
                    "squad_train_slot": 1,
                    "end_time": 1569904165,
                    "add_exp": 540000,
                    "num": 12,
                    "cost_item_info": "{\"46\":180,\"506\":60}"
                }
            */
            #endregion
            try
            {
                log.Debug("중장비경험훈련 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    int squadTrainSlot = Parser.Json.ParseInt(response["squad_train_slot"]);
                    if (squadTrainSlot > 0)
                    {
                        long squadWithUserId = Parser.Json.ParseLong(response["squad_with_user_id"]);
                        int endTime = Parser.Json.ParseInt(response["end_time"]);
                        long addExp = Parser.Json.ParseLong(response["add_exp"]);
                        int num = Parser.Json.ParseInt(response["num"]);

                        // 알림 탭 추가
                        dashboardView.Add(new SkillTrainTemplate()
                        {
                            slotType = 1,
                            slot = squadTrainSlot,
                            squadWithUserId = squadWithUserId,
                            addExp = addExp,
                            reportNum = num,
                            endTime = endTime,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "중장비 경험훈련 시작 처리 실패");
            }
        }

        /// <summary>
        /// 중장비 경험훈련 취소
        /// ("Squad/trainGiveUp")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void AbortExpTrain(string request_string, string response_string)
        {
            #region Packet Example
            // request
            // {"train_slot":1}
            // response
            // {"item_46":15,"item_506":5}
            #endregion
            try
            {
                log.Debug("중장비경험훈련 취소");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int trainSlot = Parser.Json.ParseInt(request["train_slot"]);
                    SkillTrainTemplate template = dashboardView.GetSlotSkillTrain(trainSlot, 1);
                    if (template != null)
                    {
                        // 알림 탭 제거
                        dashboardView.Remove(template);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "중장비 경험훈련 취소 처리 실패");
            }
        }

        /// <summary>
        /// 중장비 경험훈련 완료
        /// ("Squad/trainFinish")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishExpTrain(string request_string, string response_string)
        {
            #region Packet Example
            // request
            // {"train_slot":1} 
            // response
            // {"life":122} 
            #endregion
            try
            {
                log.Debug("중장비경험훈련 완료");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int trainSlot = Parser.Json.ParseInt(request["train_slot"]);
                    SkillTrainTemplate template = dashboardView.GetSlotSkillTrain(trainSlot, 1);
                    if (template != null)
                    {
                        // 알림 탭 제거
                        dashboardView.Remove(template);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "중장비 경험훈련 완료 처리 실패");
            }
        }
    }
}
