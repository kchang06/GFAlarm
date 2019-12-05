using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Notifier;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using static GFAlarm.MainWindow;

namespace GFAlarm.Transaction.PacketProcess
{
    /// <summary>
    /// 군수지원 패킷 처리
    /// </summary>
    public class Operation
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 군수지원 시작
        /// ("Operation/startOperation")
        /// </summary>
        /// <param name="packet"></param>
        public static void StartOperation(string request_string, string response_string)
        {
            #region Packet Example
            // request Operation/startOperation?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "team_id": 8,
                    "operation_id": 29,
                    "max_level": 120
                }
            */
            #endregion
            try
            {
                log.Debug("군수지원 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int teamId = Parser.Json.ParseInt(request["team_id"]);
                    log.Debug("투입 제대 {0}", teamId);
                    if (Util.Common.IsValidTeamId(teamId))
                    {
                        int operationId = Parser.Json.ParseInt(request["operation_id"]);
                        int maxLevel = Parser.Json.ParseInt(request["max_level"]);
                        int startTime = TimeUtil.GetCurrentSec();
                        log.Debug("군수번호 {0}", operationId);
                        log.Debug("최대 레벨 {0}", maxLevel);

                        // 제대 리더
                        long gunWithUserId = UserData.Doll.GetTeamLeaderGunWithUserId(teamId);
                        int gunId = 0;
                        int skinId = 0;
                        if (gunWithUserId > 0)
                        {
                            DollWithUserInfo doll = UserData.Doll.Get(gunWithUserId);
                            if (doll != null)
                            {
                                gunId = doll.no;
                                skinId = doll.skin;
                                log.Debug("리더 {0} {1}", gunWithUserId, doll.name);
                            }
                        }

                        // 음성 알림
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.Voice,
                            type = MessageType.start_operation,
                            gunId = gunId,
                            skinId = skinId,
                        });

                        // 알림 탭 추가
                        
                        dashboardView.Add(new DispatchedEchleonTemplate()
                        {
                            operationId = operationId,
                            teamId = teamId,
                            startTime = startTime,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Operation/startOperation");
            }
        }

        /// <summary>
        /// 군수지원 완료
        /// ("Operation/finishOperation")
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void FinishOperation(string request_string, string response_string)
        {
            #region Packet Example
            // request Operation/finishOperation?uid={0}&outdatacode={1}&req_id={2} 
            /*
                {
                    "operation_id": 2
                }
            */
            // response Operation/finishOperation
            /*
                {
                    "item_id":"",       // 획득 아이템
                    "big_success":1     // 0: 작전성공 1: 작전대성공
                }
            */
            #endregion
            try
            {
                log.Debug("군수지원 완료");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int operationId = Parser.Json.ParseInt(request["operation_id"]);
                    log.Debug("군수번호 {0}", operationId);

                    // 알림 탭 제거
                    dashboardView.Remove(new DispatchedEchleonTemplate()
                    {
                        operationId = operationId,
                    });

                    // 임무 갱신
                    UserData.Quest.Daily.operation += 1;
                    UserData.Quest.Weekly.operation += 1;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Operation/finishOperation");
            }
        }

        /// <summary>
        /// 군수지원 취소
        /// ("Operation/abortOperation")
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void AbortOperation(string request_string, string response_string)
        {
            #region Packet Example
            // request Operation/abortOperation?uid={0}&outdatacode={1}&req_id={2} 
            /*
                {
                    "operation_id": 2
                }
            */
            #endregion
            try
            {
                log.Debug("군수지원 취소");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int operationId = Parser.Json.ParseInt(request["operation_id"]);
                    log.Debug("군수번호 {0}", operationId);

                    // 알림 탭 제거
                    dashboardView.Remove(new DispatchedEchleonTemplate()
                    {
                        operationId = operationId,
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Operation/abortOperation");
            }
        }
    }
}
