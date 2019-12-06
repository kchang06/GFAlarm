using GFAlarm.Constants;
using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Notifier;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using LocalizationResources;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using static GFAlarm.MainWindow;

namespace GFAlarm.Transaction.PacketProcess
{
    public class ReceivePacket
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 응답 (Response)
        /// </summary>
        /// <param name="packet"></param>
        public static void Process(GFPacket packet)
        {
            string uri = packet.uri;

            // 사용자 정보 가져오기
            // (전체 진행상황 가져오기)
            if (uri.EndsWith("Index/index"))
            {
                PacketProcess.Index.GetIndex(packet);
                return; // index 패킷은 출력되면 안됨
            }

            /// 패킷 출력
            if (Config.Setting.logPacket)
            {
                log.Info("req_id={0}, uri={1}", packet.req_id, uri);
                if (!string.IsNullOrEmpty(UserData.sign))
                {
                    string outdatacode = "";
                    try
                    {
                        outdatacode = AuthCode.Decode(packet.outdatacode, UserData.sign);
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(outdatacode))
                        log.Info("outdatacode={0}", outdatacode);
                    else if (!string.IsNullOrEmpty(packet.outdatacode))
                        log.Info("outdatacode={0}", packet.outdatacode);
                    string body = "";
                    try
                    {
                        body = AuthCode.Decode(packet.body, UserData.sign);
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(body))
                        log.Info("body={0}", body);
                    else
                        log.Info("body={0}", packet.body);
                }
                else
                {
                    log.Info("outdatacode={0}", packet.outdatacode);
                    log.Info("body={0}", packet.body);
                }
            }

            /// 패킷 복호화
            string request_string = "";
            string response_string = "";
            try
            {
                request_string = AuthCode.Decode(packet.outdatacode, UserData.sign);
                response_string = AuthCode.Decode(packet.body, UserData.sign);
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to decode response body");
                return;
            }

            // 메인화면
            // ("Index/home")
            if (uri.EndsWith("Index/home"))
            {
                #region Packet Example
                // request
                // {"data_version":"22784a27ae22ea1f4824279fba2f2bf9","ab_version":"0","start_id":195979077,"ignore_time":1} 
                // response 
                /*
                    {
                        "recover_mp": 0,
                        "recover_ammo": 0,
                        "recover_mre": 0,
                        "recover_part": 0,
                        "gem": 4706,
                        "all_favorup_gun": [],
                        "last_favor_recover_time": "1569276194",
                        "recover_ssoc": 0,
                        "last_ssoc_change_time": 1569261600,
                        "kick": 0,
                        "friend_messagelist": [],
                        "friend_applylist": [],
                        "index_getmaillist": [],
                        "squad_data_daily": [
                            {
                                "user_id": 343650,
                                "squad_id": "11",
                                "type": "slay:mech",
                                "last_finish_time": "2019-09-24",
                                "count": 0,
                                "receive": 0
                            },
                            {
                                "user_id": 343650,
                                "squad_id": "14",
                                "type": "mission_night_win",
                                "last_finish_time": "2019-09-24",
                                "count": 0,
                                "receive": 0
                            },
                            {
                                "user_id": 343650,
                                "squad_id": "5",
                                "type": "mission:60",
                                "last_finish_time": "2019-09-24",
                                "count": 0,
                                "receive": 0
                            }
                        ],
                        "build_coin_flag": -4,
                        "is_bind": "0"
                    }
                    */
                #endregion
                try
                {
                    log.Debug("메인화면 진입");
                    JObject response = Parser.Json.ParseJObject(response_string);
                    if (response != null)
                    {
                        // 자원
                        int recoverMp = Parser.Json.ParseInt(response["recover_mp"]);
                        int recoverAmmo = Parser.Json.ParseInt(response["recover_ammo"]);
                        int recoverMre = Parser.Json.ParseInt(response["recover_mre"]);
                        int recoverPart = Parser.Json.ParseInt(response["recover_part"]);
                        UserData.mp += recoverMp;
                        UserData.ammo += recoverAmmo;
                        UserData.mre += recoverMre;
                        UserData.part += recoverPart;

                        // 우편
                        if (response.ContainsKey("index_getmaillist"))
                        {
                            log.Debug("우편 목록 가져오는 중...");
                            try
                            {
                                UserData.Mail.Init(response["index_getmaillist"]);
                                log.Debug("우편 {0} 건", UserData.Mail.Count());
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex, "우편 목록 가져오는 중 에러 발생");
                            }
                        }
                        // 정보임무
                        if (response.ContainsKey("squad_data_daily") && response["squad_data_daily"] is JArray)
                        {
                            List<QuestTemplate> quests = new List<QuestTemplate>();
                            UserData.Quest.isNotify = false;

                            questView.ClearResearch();

                            JArray items = response["squad_data_daily"].Value<JArray>();
                            foreach (JObject item in items)
                            {
                                int squadId = Parser.Json.ParseInt(item["squad_id"]);
                                int count = Parser.Json.ParseInt(item["count"]);

                                switch (squadId)
                                {
                                    case 1:
                                        UserData.Quest.Research.mission = count;
                                        break;
                                    case 2:
                                        UserData.Quest.Research.kill = count;
                                        break;
                                    case 3:
                                        UserData.Quest.Research.mission46 = count;
                                        break;
                                    case 4:
                                        UserData.Quest.Research.mission56 = count;
                                        break;
                                    case 5:
                                        UserData.Quest.Research.mission66 = count;
                                        break;
                                    case 6:
                                        UserData.Quest.Research.mission76 = count;
                                        break;
                                    case 7:
                                        UserData.Quest.Research.killBoss = count;
                                        break;
                                    case 8:
                                        UserData.Quest.Research.killArmorDoll = count;
                                        break;
                                    case 9:
                                        UserData.Quest.Research.killArmorMech = count;
                                        break;
                                    case 10:
                                        UserData.Quest.Research.killDoll = count;
                                        break;
                                    case 11:
                                        UserData.Quest.Research.killMech = count;
                                        break;
                                    case 12:
                                        UserData.Quest.Research.missionNormal = count;
                                        break;
                                    case 13:
                                        UserData.Quest.Research.missionEmergency = count;
                                        break;
                                    case 14:
                                        UserData.Quest.Research.missionNight = count;
                                        break;
                                }

                                quests.Add(new QuestTemplate()
                                {
                                    id = 300 + squadId,
                                    count = count,
                                });
                            }

                            foreach (QuestTemplate quest in quests)
                            {
                                if (quest.TBIsCompleted != "1")
                                    questView.Add(quest);
                            }

                            UserData.Quest.isNotify = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get Index/home");
                }
            }
            // 임무화면
            // ("Index/Quest")
            else if (uri.EndsWith("Index/Quest"))
            {
                PacketProcess.Index.GetQuest(request_string, response_string);
            }
            // 부관변경
            // ("Index/changeAdjutant")
            else if (uri.EndsWith("Index/changeAdjutant"))
            {
                #region Packet Example
                // request Index/changeAdjutant?uid={0}&outdatacode={1}&req_id={2}
                /*
                    {"gun_id":48,"fairy_id":0,"skin_id":0,"is_sexy":0,"mod":0} 
                */
                #endregion
                try
                {
                    log.Debug("부관변경");
                    JObject request = Parser.Json.ParseJObject(request_string);
                    if (request != null)
                    {
                        int gunId = Parser.Json.ParseInt(request["gun_id"]);
                        int fairyId = Parser.Json.ParseInt(request["fairy_id"]);
                        int skinId = Parser.Json.ParseInt(request["skin_id"]);
                        int isSexy = Parser.Json.ParseInt(request["is_sexy"]);

                        UserData.adjutantDoll = gunId;
                        UserData.adjutantDollSkin = skinId;
                        UserData.adjutantFairy = fairyId;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get Index/changeAdjutant");
                }
            }
            // 우편받기
            // ("Index/getResourceInMail")
            else if (uri.EndsWith("Index/getResourceInMail"))
            {
                try
                {
                    log.Debug("우편받기");
                    JObject request = Parser.Json.ParseJObject(request_string);
                    JObject response = Parser.Json.ParseJObject(response_string);
                    if (request != null && response != null)
                    {
                        long mailWithUserId = Parser.Json.ParseLong(request["mail_with_user_id"]);
                        // 인형 획득
                        if (response["guns"] != null && response["guns"] is JArray)
                        {
                            #region Packet Example
                            /*
                                {
                                    "guns": [
                                        {
                                            "gun_with_user_id": 90249445,
                                            "gun_id": "94",
                                            "gun_level": "10",
                                            "gun_exp": 4500,
                                            "number": 2,
                                            "life": 192,
                                            "skill1": "1",
                                            "pow": 2,
                                            "hit": 1,
                                            "dodge": 6,
                                            "rate": 3
                                        }
                                    ]
                                }
                                */
                            #endregion
                            JArray items = response["guns"].Value<JArray>();
                            foreach (var item in items)
                            {
                                long gunWithUserId = Parser.Json.ParseLong(item["gun_with_user_id"]);
                                int gunId = Parser.Json.ParseInt(item["gun_id"]);
                                int gunLevel = Parser.Json.ParseInt(item["gun_level"]);
                                int gunExp = Parser.Json.ParseInt(item["gun_exp"]);
                                int number = Parser.Json.ParseInt(item["number"]);

                                DollWithUserInfo doll = new DollWithUserInfo(gunWithUserId, gunId);
                                doll.level = gunLevel;
                                doll.exp = gunExp;
                                doll.maxLink = number;
                                UserData.Doll.Add(doll);
                            }
                        }
                        // 인형 획득
                        if (response.ContainsKey("gun_with_user_id"))
                        {
                            MailWithUserInfo mail = UserData.Mail.Get(mailWithUserId);

                            if (mail.gunId > 0)
                            {
                                long gunWithUserId = Parser.Json.ParseLong(response["gun_with_user_id"]);
                                int gunId = mail.gunId;

                                DollWithUserInfo doll = new DollWithUserInfo(gunWithUserId, gunId);
                                UserData.Doll.Add(doll);
                            }
                        }
                        UserData.Mail.Remove(mailWithUserId);
                        log.Debug("남은 우편 {0} 건", UserData.Mail.Count());
                    }
                    MainWindow.view.SetIconNotify(Menus.QUEST, false);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get Index/getResourceInMail");
                }
            }
            // 모든 우편받기 
            // ("Index/QuickGetQuestsResourceInMails")
            else if (uri.EndsWith("Index/QuickGetQuestsResourceInMails"))
            {
                #region Packet Example
                // request
                // {"furniture_data":false} 
                // response
                /*
                    {
                        "user_exp": 70,
                        "mp": 500,
                        "ammo": 500,
                        "mre": 500,
                        "part": 500,
                        "core": 3,
                        "gem": 0,
                        "gun_id": [],
                        "item_ids": "1-7,3-2,41-1",
                        "equip_ids": [],
                        "furniture": [],
                        "gift": "",
                        "skin": [],
                        "bp_pay": 0,
                        "coins": "",
                        "fairy_ids": "",
                        "commander_uniform": []
                    }
                 */
                #endregion
                try
                {
                    log.Debug("모든 우펀 수령");
                    JObject response = Parser.Json.ParseJObject(response_string);
                    if (response != null)
                    {
                        int user_exp = Parser.Json.ParseInt(response["user_exp"]);
                        int mp = Parser.Json.ParseInt(response["mp"]);
                        int ammo = Parser.Json.ParseInt(response["ammo"]);
                        int mre = Parser.Json.ParseInt(response["mre"]);
                        int part = Parser.Json.ParseInt(response["part"]);
                        int core = Parser.Json.ParseInt(response["core"]);
                        string item_ids = Parser.Json.ParseString(response["item_ids"]);
                        int gem = Parser.Json.ParseInt(response["gem"]);

                        UserData.mp += mp;
                        UserData.ammo += ammo;
                        UserData.mre += mre;
                        UserData.part += part;
                        UserData.gem += gem;
                    }
                    MainWindow.view.SetIconNotify(Menus.QUEST, false);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get Index/QuickGetQuestsResourceInMails");
                }
            }
            // 출석화면 
            // ("Index/attendance")
            else if (uri.EndsWith("Index/attendance"))
            {
                try
                {
                    log.Debug("출석화면 진입");
                    if (UserData.isReachAttendanceTime == true)
                    {
                        // 임무 초기화
                        UserData.Quest.Clear();
                        UserData.Quest.ClearResearch();
                    }
                    if (UserData.attendanceTime < TimeUtil.GetCurrentSec())
                    {
                        log.Debug("출석시간 지남 - 다음 날로 설정");
                        int nowTime = TimeUtil.GetCurrentSec();
                        while (UserData.attendanceTime < nowTime)
                        {
                            UserData.attendanceTime += TimeUtil.DAY;
                            log.Debug("출석시간 조정 {0}", TimeUtil.GetDateTime(UserData.attendanceTime, "MM-dd HH:mm"));
                        }
                        // TODO: 다음 날 처리
                        UserData.CombatSimulation.remainPointToday = UserData.CombatSimulation.GetRemainPoint();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get Index/attendance");
                }
            }
            // 군수지원 시작 
            // ("Operation/startOperation")
            else if (uri.EndsWith("Operation/startOperation"))
            {
                PacketProcess.Operation.StartOperation(request_string, response_string);
            }
            // 군수지원 완료
            // ("Operation/finishOperation")
            else if (uri.EndsWith("Operation/finishOperation"))
            {
                PacketProcess.Operation.FinishOperation(request_string, response_string);
            }
            // 군수지원 취소
            // ("Operation/abortOperation")
            else if (uri.EndsWith("Operation/abortOperation"))
            {
                PacketProcess.Operation.AbortOperation(request_string, response_string);
            }
            // 자율작전 시작
            // ("Automission/startAutomission")
            else if (uri.EndsWith("Automission/startAutomission"))
            {
                PacketProcess.Automission.StartAutomission(request_string, response_string);
            }
            // 자율작전 완료
            // ("Automission/finishAutomission")
            else if (uri.EndsWith("Automission/finishAutomission"))
            {
                PacketProcess.Automission.FinishAutomission(request_string, response_string);
            }
            // 자율작전 취소
            // ("Automission/abortAutomission")
            else if (uri.EndsWith("Automission/abortAutomission"))
            {
                PacketProcess.Automission.AbortAutomission(request_string, response_string);
            }
            // 인형수복 시작
            // ("Gun/fixGuns")
            else if (uri.EndsWith("Gun/fixGuns"))
            {
                PacketProcess.Gun.StartRestore(request_string, response_string);
            }
            // 인형수복 완료
            // ("Gun/fixFinish")
            else if (uri.EndsWith("Gun/fixFinish"))
            {
                PacketProcess.Gun.FinishRestore(request_string, response_string);
            }
            // 인형제조 시작
            // ("Gun/developGun")
            else if (uri.EndsWith("Gun/developGun"))
            {
                PacketProcess.Gun.StartProduce(request_string, response_string);
            }
            // 인형제조 완료
            // ("Gun/finishDevelop")
            else if (uri.EndsWith("Gun/finishDevelop"))
            {
                PacketProcess.Gun.FinishProduce(request_string, response_string);
            }
            // 인형일괄제조 시작
            // ("Gun/developMultiGun")
            else if (uri.EndsWith("Gun/developMultiGun"))
            {
                PacketProcess.Gun.StartMultiProduce(request_string, response_string);
            }
            // 인형일괄제조 완료
            // ("Gun/finishAllDevelop")
            else if (uri.EndsWith("Gun/finishAllDevelop"))
            {
                PacketProcess.Gun.FinishMultiProduce(request_string, response_string);
            }
            // 장비제조 시작
            // ("Equip/develop")
            else if (uri.EndsWith("Equip/develop"))
            {
                PacketProcess.Equip.StartProduce(request_string, response_string);
            }
            // 장비제조 완료
            // ("Equip/finishDevelop")
            else if (uri.EndsWith("Equip/finishDevelop"))
            {
                PacketProcess.Equip.FinishProduce(request_string, response_string);
            }
            // 장비일괄제조 시작
            // ("Equip/developMulti")
            else if (uri.EndsWith("Equip/developMulti"))
            {
                PacketProcess.Equip.StartMultiProduce(request_string, response_string);
            }
            // 장비일괄제조 완료
            // ("Equip/finishAllDevelop")
            else if (uri.EndsWith("Equip/finishAllDevelop"))
            {
                PacketProcess.Equip.FinishMultiProduce(request_string, response_string);
            }
            // 편제확대
            // ("Gun/combineGun")
            else if (uri.EndsWith("Gun/combineGun"))
            {
                PacketProcess.Gun.Expand(request_string, response_string);
            }
            // 인형강화
            // ("Gun/eatGun")
            else if (uri.EndsWith("Gun/eatGun"))
            {
                PacketProcess.Gun.Enhance(request_string, response_string);
            }
            // 인형해체
            // ("Gun/retireGun")
            else if (uri.EndsWith("Gun/retireGun"))
            {
                PacketProcess.Gun.Retire(request_string, response_string);
            }
            // 인형개조
            // ("Gun/mindupdate")
            else if (uri.EndsWith("Gun/mindupdate"))
            {
                PacketProcess.Gun.MindUpdate(request_string, response_string);
            }
            // 인형서약
            // ("Gun/contract")
            else if (uri.EndsWith("Gun/contract"))
            {
                PacketProcess.Gun.Constract(request_string, response_string);
            }
            // 장비강화
            // ("Equip/eatEquip")
            else if (uri.EndsWith("Equip/eatEquip"))
            {
                PacketProcess.Equip.Enhance(request_string, response_string);
            }
            // 장비해체
            // ("Equip/retire")
            else if (uri.EndsWith("Equip/retire"))
            {
                PacketProcess.Equip.Retire(request_string, response_string);
            }
            // 장비교정
            // ("Equip/adjust")
            else if (uri.EndsWith("Equip/adjust"))
            {
                PacketProcess.Equip.Calibrate(request_string, response_string);
            }
            // 요정강화
            // ("Fairy/eatFairy")
            else if (uri.EndsWith("Fairy/eatFairy"))
            {
                PacketProcess.Fairy.Enhance(request_string, response_string);
            }
            // 요정교정
            // ("Fairy/adjust")
            else if (uri.EndsWith("Fairy/adjust"))
            {
                PacketProcess.Fairy.Calibrate(request_string, response_string);
            }
            // 요정교정 확정
            // ("Fairy/adjustConfirm")
            else if (uri.EndsWith("Fairy/adjustConfirm"))
            {
                PacketProcess.Fairy.CalibrateConfirm(request_string, response_string);
            }
            // 칩셋강화
            // ("Chip/strengthen")
            else if (uri.EndsWith("Chip/strengthen"))
            {
                PacketProcess.Chip.Enhance(request_string, response_string);
            }
            // 인형제대변경
            // ("Gun/teamGun")
            else if (uri.EndsWith("Gun/teamGun"))
            {
                PacketProcess.Gun.ChangeLocation(request_string, response_string);
            }
            // 인형제대변경 (복수)
            // ("Gun/teamGuns")
            else if (uri.EndsWith("Gun/teamGuns"))
            {
                PacketProcess.Gun.ChangeLocations(request_string, response_string);
            }
            // 요정제대변경
            // ("Fairy/teamFairy")
            else if (uri.EndsWith("Fairy/teamFairy"))
            {
                PacketProcess.Fairy.ChangeLocation(request_string, response_string);
            }
            // 제대교환
            // ("Gun/exchangeTeam")
            else if (uri.EndsWith("Gun/exchangeTeam"))
            {
                PacketProcess.Gun.ExchangeEchelon(request_string, response_string);
            }
            // 제대프리셋
            // ("Gun/presetToTeam")
            else if (uri.EndsWith("Gun/presetToTeam"))
            {
                PacketProcess.Gun.PresetToEchelon(request_string, response_string);
            }
            // 인형 스킬훈련 시작
            // ("Gun/skillUpgrade")
            else if (uri.EndsWith("Gun/skillUpgrade"))
            {
                PacketProcess.Gun.StartSkillTrain(request_string, response_string);
            }
            // 요정 스킬훈련 시작
            // ("Fairy/skillUpgrade")
            else if (uri.EndsWith("Fairy/skillUpgrade"))
            {
                PacketProcess.Fairy.StartSkillTrain(request_string, response_string);
            }
            // 인형 스킬훈련 완료
            // ("Gun/finishUpgrade")
            // ("Gun/quickUpgrade")
            else if (uri.EndsWith("Gun/finishUpgrade") || uri.EndsWith("Gun/quickUpgrade"))
            {
                PacketProcess.Gun.FinishSkillTrain(request_string, response_string);
            }
            // 요정 스킬훈련 완료
            // ("Fairy/finishUpgrade")
            // ("Fairy/quickUpgrade")
            else if (uri.EndsWith("Fairy/finishUpgrade") || uri.EndsWith("Fairy/quickUpgrade"))
            {
                PacketProcess.Fairy.FinishSkillTrain(request_string, response_string);
            }
            // 중장비 스킬훈련 시작
            // ("Squad/upSkill")
            else if (uri.EndsWith("Squad/upSkill"))
            {
                PacketProcess.Squad.StartSkillTrain(request_string, response_string);
            }
            // 중장비 스킬훈련 완료
            // ("Squad/finishSkill")
            else if (uri.EndsWith("Squad/finishSkill"))
            {
                PacketProcess.Squad.FinishSkillTrain(request_string, response_string);
            }
            // 중장비 스킬훈련 쾌속완료
            // ("Squad/quickSkill")
            else if (uri.EndsWith("Squad/quickSkill"))
            {
                PacketProcess.Squad.FinishSkillTrain(request_string, response_string);
            }
            // 중장비경험훈련 시작
            // ("Squad/train")
            else if (uri.EndsWith("Squad/train"))
            {
                PacketProcess.Squad.StartExpTrain(request_string, response_string);
            }
            // 중장비경험훈련 취소
            // ("Squad/trainGiveUp")
            else if (uri.EndsWith("Squad/trainGiveUp"))
            {
                PacketProcess.Squad.AbortExpTrain(request_string, response_string);
            }
            // 중장비경험훈련 완료
            // ("Squad/trainFinish")
            else if (uri.EndsWith("Squad/trainFinish"))
            {
                PacketProcess.Squad.FinishExpTrain(request_string, response_string);
            }
            // 정보분석 시작
            // 정보분석 일괄 시작
            // ("Dorm/data_analysis")
            else if (uri.EndsWith("Dorm/data_analysis"))
            {
                PacketProcess.Dorm.StartDataAnalysis(request_string, response_string);
            }
            // 정보분석 완료
            // ("Dorm/data_analysis_finish")
            // 정보분석 일괄 완료
            // ("Dorm/data_analysis_finish_all")
            else if (uri.EndsWith("Dorm/data_analysis_finish") || uri.EndsWith("Dorm/data_analysis_finish_all"))
            {
                PacketProcess.Dorm.FinishDataAnalysis(request_string, response_string);
            }
            // 정보분석 전부 확인
            // ("Dorm/data_analysis_complete")
            else if (uri.EndsWith("Dorm/data_analysis_complete"))
            {
                PacketProcess.Dorm.FinishQuickDataAnalysis(request_string, response_string);
            }
            // 인형잠금
            // ("Gun/changeLock")
            else if (uri.EndsWith("Gun/changeLock"))
            {
                PacketProcess.Gun.ChangeLock(request_string, response_string);
            }
            // 시설작업 시작 (작전보고서)
            // ("Outhouse/establish_build")
            else if (uri.EndsWith("Outhouse/establish_build"))
            {
                PacketProcess.Outhouse.StartBattleReport(request_string, response_string);
            }
            // 시설작업 완료 (작전보고서)
            // ("Outhouse/establish_build_finish")
            else if (uri.EndsWith("Outhouse/establish_build_finish"))
            {
                PacketProcess.Outhouse.FinishBattleReport(request_string, response_string);
            }
            // 작전 시작
            // ("Mission/startMission")
            else if (uri.EndsWith("Mission/startMission"))
            {
                PacketProcess.Mission.StartMission(request_string, response_string);
            }
            // 작전 중지
            // ("Mission/abortMission")
            else if (uri.EndsWith("Mission/abortMission"))
            {
                PacketProcess.Mission.AbortMission(request_string, response_string);
            }
            // 전투 종료
            // ("Mission/battleFinish")
            // 턴 시작
            // ("Mission/startTurn")
            // 턴 종료
            // ("Mission/endTurn")
            // 적 턴 종료
            // ("Mission/endEnemyTurn")
            // 인질구출
            // ("Mission/saveHostage")
            else if (uri.EndsWith("Mission/battleFinish")
                    || uri.EndsWith("Mission/startTurn")
                    || uri.EndsWith("Mission/endTurn")
                    || uri.EndsWith("Mission/endEnemyTurn")
                    || uri.EndsWith("Mission/saveHostage"))
            {
                #region Packet Example (전투 종료)
                // request Mission/battleFinish?uid={1}&outdatacode={1}&req_id={2}
                /*
                    {
                        "spot_id": 17,
                        "if_enemy_die": true,
                        "current_time": 1564036501,
                        "boss_hp": 0,
                        "mvp": 10545856,
                        "last_battle_info": "",
                        "use_skill_squads": [],
                        "guns": [{"id": 6762038,"life": 650},{"id": 10545856,"life": 565},{"id": 12131545,"life": 525},{"id": 346146275,"life": 420},{"id": 352953715,"life": 335}],
                        "user_rec": "{\"seed\":89382062,\"record\":[]}",
                        "1000": {"10": 32020,"11": 31894,"12": 31957,"13": 31948,"15": 9544,"16": 0,"17": 428,"33": 20005,"40": 91,"18": 14,"19": 846,"20": 0,"21": 0,"22": 0,"23": 0,"24": 28800,"25": 0,"26": 28800,"27": 15,"34": 1,"35": 9,"41": 316,"42": 0,"43": 0,"44": 0,"92": 0},
                        "1001": {},
                        "1002": {"6762038": {"47": 0},"10545856": {"47": 1},"12131545": {"47": 0},"346146275": {"47": 0},"352953715": {"47": 0}},
                        "1003": {"490949": {"9": 1,"68": 0}},
                        "1005": {},
                        "battle_damage": {}
                    }
                    */
                // response Mission/battleFinish
                /*
                    {
                        "battle_get_gun": {
                            "gun_with_user_id": "358473723",
                            "gun_id": "71"
                        },
                        "user_exp": "49",
                        "gun_exp": [
                            {
                                "gun_with_user_id": "11113158",
                                "exp": "0"
                            },
                            {
                                "gun_with_user_id": "12131545",
                                "exp": "0"
                            },
                            {
                                "gun_with_user_id": "155315092",
                                "exp": "1225"
                            },
                            {
                                "gun_with_user_id": "172784306",
                                "exp": "1176"
                            },
                            {
                                "gun_with_user_id": "305636618",
                                "exp": "1470"
                            }
                        ],
                        "fairy_exp": 1373,
                        "gun_life": [],
                        "squad_exp": [],
                        "battle_rank": "5",
                        "free_exp": 3773,
                        "change_belong": [],
                        "building_defender_change": [],
                        "mission_win_result": [],
                        "seed": 6838,
                        "favor_change": {
                            "11113158": 1440,
                            "12131545": 432,
                            "155315092": 144,
                            "172784306": 480,
                            "305636618": 144
                        },
                        "type5_score": "0",
                        "ally_instance_transform": [],
                        "ally_instance_betray": [],
                        "mission_control": {
                            "1": 1,
                            "2": 2,
                            "3": 3
                        }
                    }
                    */
                #endregion

                try
                {
                    JObject request = Parser.Json.ParseJObject(request_string);
                    JObject response = Parser.Json.ParseJObject(response_string);
                    if (uri.EndsWith("Mission/battleFinish"))
                    {
                        log.Debug("전투 종료");
                        // 제대이동 완료 알림
                        if (Config.Alarm.notifyTeamMove && Config.Alarm.notifyTeamMoveCount == UserData.CurrentMission.moveCount && Config.Alarm.notifyTeamMoveAndBattleFinish)
                        {
                            Notifier.Manager.notifyQueue.Enqueue(new Message()
                            {
                                send = MessageSend.All,
                                type = MessageType.mission_move_finish,
                                delay = 3500,
                                subject = LanguageResources.Instance["MESSAGE_MISSION_MOVE_TEAM_AND_BATTLE_COMPLETE_SUBJECT"],
                                content = string.Format(LanguageResources.Instance["MESSAGE_MISSION_MOVE_TEAM_AND_BATTLE_COMPLETE_CONTENT"], Config.Alarm.notifyTeamMoveCount)
                            });
                        }
                    }

                    if (uri.EndsWith("Mission/startTurn"))
                    {
                        log.Debug("턴 시작");
                        UserData.CurrentMission.turnCount++;
                        foreach (KeyValuePair<int, int> item in UserData.CurrentMission.teamSpots)
                        {
                            UserData.Doll.SpendAmmoMre(item.Key, true);
                        }
                    }
                    PacketProcess.Mission.MoveTeam(request, response);
                    PacketProcess.Mission.SpotBelongChange(request, response);
                    PacketProcess.Mission.BattleFinish(request, response);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get {0}", uri);
                }
            }
            // 제대 소환
            // ("Mission/reinforceTeam")
            else if (uri.EndsWith("Mission/reinforceTeam"))
            {
                PacketProcess.Mission.ReinforceTeam(request_string, response_string);
            }
            // 제대 보급
            // ("Mission/supplyTeam")
            else if (uri.EndsWith("Mission/supplyTeam"))
            {
                PacketProcess.Mission.SupplyTeam(request_string, response_string);
            }
            // 제대 이동
            // ("Mission/teamMove")
            else if (uri.EndsWith("Mission/teamMove"))
            {
                #region Packet Example
                // request Mission/teamMove?uid={0}&outdatacode={1}&req_id={2}
                /*
                    {
                        "team_id":1,
                        "from_spot_id":9016,
                        "to_spot_id":9020,
                        "move_type":1
                    } 
                    */
                // response Mission/teamMove
                /*
                    {
                        "building_defender_change": [],
                        "ap": 2,
                        "type5_score": "0",
                        "fairy_skill_return": [],
                        "fairy_skill_perform": [],
                        "fairy_skill_on_spot": [],
                        "fairy_skill_on_team": [],
                        "fairy_skill_on_enemy": [],
                        "fairy_skill_on_squad": [],
                        "ally_instance_transform": [],
                        "ally_instance_betray": [],
                        "mission_control": {
                            "1": 1,
                            "2": 2,
                            "3": 3
                        },
                        "change_belong": []
                    }
                    */
                #endregion
                try
                {
                    log.Debug("제대 이동");
                    JObject request = Parser.Json.ParseJObject(request_string);
                    JObject response = Parser.Json.ParseJObject(response_string);
                    if (request != null && response != null)
                    {
                        int teamId = Parser.Json.ParseInt(request["team_id"]);
                        int fromSpotId = Parser.Json.ParseInt(request["from_spot_id"]);
                        int toSpotId = Parser.Json.ParseInt(request["to_spot_id"]);
                        int moveType = Parser.Json.ParseInt(request["move_type"]);

                        // 일반 제대이동
                        if (moveType == 1)
                        {
                            UserData.CurrentMission.MoveTeam(teamId, toSpotId);
                        }
                        // 제대 스왑
                        else if (moveType == 2)
                        {
                            int swap_team_id = UserData.CurrentMission.GetTeamId(toSpotId);
                            if (swap_team_id > 0)
                            {
                                UserData.CurrentMission.MoveTeam(swap_team_id, fromSpotId);
                                UserData.CurrentMission.MoveTeam(teamId, toSpotId);
                            }
                        }

                        // 제대이동 횟수
                        UserData.CurrentMission.moveCount++;
                        // 제대이동 횟수 도달
                        if (Config.Alarm.notifyTeamMove &&
                            Config.Alarm.notifyTeamMoveCount == UserData.CurrentMission.moveCount &&
                            !Config.Alarm.notifyTeamMoveAndBattleFinish)
                        {
                            Notifier.Manager.notifyQueue.Enqueue(new Message()
                            {
                                send = MessageSend.All,
                                type = MessageType.mission_move_finish,
                                subject = LanguageResources.Instance["MESSAGE_MISSION_MOVE_TEAM_SUBJECT"],
                                content = string.Format(LanguageResources.Instance["MESSAGE_MISSION_MOVE_TEAM_CONTENT"], Config.Alarm.notifyTeamMoveCount)
                            });
                        }

                        PacketProcess.Mission.MoveTeam(request, response);
                        PacketProcess.Mission.SpotBelongChange(request, response);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get {0}", uri);
                }
            }
            // 친구제대 이동
            // ("Mission/friendTeamMove")
            else if (uri.EndsWith("Mission/friendTeamMove"))
            {
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    PacketProcess.Mission.MoveTeam(request, response);
                    PacketProcess.Mission.SpotBelongChange(request, response);
                }
            }
            // 제대 퇴각
            // ("Mission/withdrawTeam")
            else if (uri.EndsWith("Mission/withdrawTeam"))
            {
                #region Packet Example
                // request Mission/withdrawTeam?uid={0}&outdatacode={1}&req_id={2}
                /*
                    {"spot_id":9018} 
                    */
                // response Mission/withdrawTeam
                // null
                #endregion
                try
                {
                    log.Debug("제대 퇴각");
                    JObject request = Parser.Json.ParseJObject(request_string);
                    if (request != null)
                    {
                        int spotId = Parser.Json.ParseInt(request["spot_id"]);
                        int teamId = UserData.CurrentMission.GetTeamId(spotId);
                        if (GameData.Spot.IsProperWithdraw(spotId) == false)
                        {
                            UserData.Doll.LoseAmmoMre(teamId);
                        }

                        UserData.CurrentMission.WithdrawTeam(spotId);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get {0}", uri);
                }
            }
            // 긴급수복
            // ("Mission/quickFixGun")
            else if (uri.EndsWith("Mission/quickFixGun"))
            {
                #region Packet Example
                // request Mission/quickFixGun?uid={0}&outdatacode={1}&req_id={2}
                /*
                    {"gun_with_user_id":7856061,"spot_id":0} 
                    */
                // response Mission/quickFixGun
                // null
                #endregion
                try
                {
                    log.Debug("긴급수복");
                    JObject request = Parser.Json.ParseJObject(request_string);
                    if (request != null)
                    {
                        long gunWithUserId = Parser.Json.ParseLong(request["gun_with_user_id"]);
                        UserData.Doll.Fix(gunWithUserId);

                        // 임무 갱신
                        UserData.Quest.Daily.fixGun += 1;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get {0}", uri);
                }
            }
            // 요정스킬 사용
            // ("Mission/fairySkillPerform")
            else if (uri.EndsWith("Mission/fairySkillPerform"))
            {
                #region Packet Example (공수요정)
                // 공수요정
                // request Mission/fairySkillPerform?uid={0}&outdatacode={1}&req_id={2}
                /*
                    {
                        "fairy_team_id": 4,
                        "fairy_spot": 134,
                        "spot_id": [
                            149
                        ]
                    }
                    */
                // response Mission/fairySkillPerform
                /*
                    {
                        "fairy_skill_perform": [
                            {
                                "fairy_team_id": 3,
                                "building_spot_id": 0,
                                "squad_instance_id": 0,
                                "next_skill_cd_turn": 6,
                                "mission_skill_config_id": "910",
                                "perform_spot_id": 9390
                            }
                        ],
                        "fairy_skill_on_spot": [],
                        "fairy_skill_on_team": {
                            "3": {
                                "1": {
                                    "team_id": 3,
                                    "buff_id": "10086",
                                    "fairy_team_id": 3,
                                    "building_spot_id": 0,
                                    "squad_instance_id": 0,
                                    "start_turn": "1",
                                    "battle_count": 0,
                                    "mission_skill_config_id": "910"
                                }
                            }
                        },
                        "fairy_skill_on_enemy": [],
                        "fairy_skill_on_squad": [],
                        "fairy_skill_return": {
                            "fairy_effect": {
                                "1": {
                                    "origin_key": 0,
                                    "origin_spot": 9018,
                                    "origin_type": 4,
                                    "origin_stc_id": "910",
                                    "aim_spot": 9390,
                                    "aim_type": 2,
                                    "aim_stc_id": "10086"
                                }
                            }
                        }
                    }
                    */
                #endregion
                #region Packet Example (증원요정)
                // 증원요정
                // request Mission/fairySkillPerform?uid={0}&outdatacode={1}&req_id={2}
                /*
                    {
                        "fairy_team_id": 1,
                        "fairy_spot": 9,
                        "spot_id": [9]
                    }
                    */
                // response Mission/fairySkillPerform
                /*
                    {
                        "fairy_skill_return": {
                            "fairy_effect": {
                                "1": {
                                    "origin_key": 0,
                                    "origin_spot": 9,
                                    "origin_type": 4,
                                    "origin_stc_id": "804",
                                    "aim_spot": 9,
                                    "aim_type": 1,
                                    "aim_stc_id": 8
                                },
                                "2": {
                                    "origin_key": 1,
                                    "origin_spot": 9,
                                    "origin_type": 1,
                                    "origin_stc_id": 8,
                                    "aim_spot": 9,
                                    "aim_type": 2,
                                    "aim_stc_id": "8"
                                },
                                "3": {
                                    "origin_key": 1,
                                    "origin_spot": 9,
                                    "origin_type": 1,
                                    "origin_stc_id": 8,
                                    "aim_spot": 9,
                                    "aim_type": 3,
                                    "aim_stc_id": "8"
                                }
                            },
                            "mission_hurt": [
                                {
                                    "team_id": "1",
                                    "guns_life": [
                                        {
                                            "gun_with_user_id": "12131545",
                                            "life": 531
                                        }
                                    ],
                                    "ally_instance_id": 0,
                                    "ally_guns_life": [],
                                    "spot_id": 9,
                                    "enemy_instance_id": 0,
                                    "enemy_hp_percent": 1,
                                    "ally_enemy_hp_percent": 1,
                                    "squad_instance_id": 0,
                                    "squad_life": 0,
                                    "hostage_id": 0,
                                    "hostage_hp": 0,
                                    "building_spot_id": 0,
                                    "building_defender": 0,
                                    "hurt_id": "8",
                                    "mission_skill_config_id": "804"
                                }
                            ],
                            "mission_lose_result": []
                        },
                        "fairy_skill_perform": [
                            {
                                "fairy_team_id": 1,
                                "building_spot_id": 0,
                                "squad_instance_id": 0,
                                "next_skill_cd_turn": 2,
                                "mission_skill_config_id": "804",
                                "perform_spot_id": 9
                            }
                        ],
                        "fairy_skill_on_spot": {
                            "9": {
                                "1": {
                                    "fairy_team_id": "1",
                                    "building_spot_id": 0,
                                    "squad_instance_id": 0,
                                    "engine_spot_id": 9,
                                    "special_spot_config_id": "8",
                                    "start_turn": "1",
                                    "battle_count": "0",
                                    "move_count": "0",
                                    "detect_spots": [
                                        9
                                    ],
                                    "effect_spots": [
                                        9
                                    ],
                                    "conflict_type": "8",
                                    "priority": "16",
                                    "mission_skill_config_id": "804"
                                }
                            }
                        },
                        "fairy_skill_on_team": {
                            "1": {
                                "2": {
                                    "team_id": "1",
                                    "buff_id": "8",
                                    "fairy_team_id": 1,
                                    "building_spot_id": 0,
                                    "squad_instance_id": 0,
                                    "start_turn": "1",
                                    "battle_count": 0,
                                    "mission_skill_config_id": "804"
                                }
                            }
                        },
                        "fairy_skill_on_enemy": [],
                        "fairy_skill_on_squad": [],
                        "fairy_skill_on_ally": []
                    }
                    */
                #endregion
                try
                {
                    log.Debug("요정스킬 사용");
                    JObject request = Parser.Json.ParseJObject(request_string);
                    JObject response = Parser.Json.ParseJObject(response_string);
                    if (request != null && response != null)
                    {
                        int fairyTeamId = Parser.Json.ParseInt(request["fairy_team_id"]);
                        FairyWithUserInfo fairy = UserData.Fairy.GetFairyWithTeamId(fairyTeamId);
                        // 공수요정
                        if (fairy != null && fairy.no == 11)
                        {
                            int fairySpot = Parser.Json.ParseInt(request["fairy_spot"]);
                            int spotId = 0;
                            if (request.ContainsKey("spot_id"))
                            {
                                if (request["spot_id"] is JArray)
                                {
                                    JArray items = request["spot_id"].Value<JArray>();
                                    foreach (var item in items)
                                    {
                                        spotId = Parser.Json.ParseInt(item);
                                        break;
                                    }
                                }
                            }

                            int fromSpotId = fairySpot;
                            int toSpotId = spotId;

                            if (fromSpotId > 0 && toSpotId > 0)
                            {
                                UserData.CurrentMission.MoveTeam(fairyTeamId, toSpotId);
                            }
                            else
                            {
                                log.Warn("{0}제대 공수요정 이동 정보 찾을 수 없음", fairyTeamId);
                            }
                        }
                        // 증원요정
                        else if (fairy != null && fairy.no == 10)
                        {
                            JArray missionHurts = response["fairy_skill_return"]["mission_hurt"].Value<JArray>();
                            foreach (JObject missionHurt in missionHurts)
                            {
                                JArray gunsLifes = missionHurt["guns_life"].Value<JArray>();
                                foreach (JObject gunsLife in gunsLifes)
                                {
                                    long gunWithUserId = Parser.Json.ParseLong(gunsLife["gun_with_user_id"]);
                                    int life = Parser.Json.ParseInt(gunsLife["life"]);
                                    UserData.Doll.SetHp(gunWithUserId, life, true);
                                    MainWindow.echelonView.Update(fairyTeamId);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get {0}", uri);
                }
            }
            // 친구제대 정보
            // ("Friend/teamGuns")
            else if (uri.EndsWith("Friend/teamGuns"))
            {
                #region Packet Example
                // request Friend/teamGuns?uid={0}&signcode={1}&req_id={2}
                // null
                // response Friend/teamGuns
                /*
                    {
                        "borrow_team_today": 0,
                        "gun_with_friend": {
                            "guns_with_friend_available": [
                                {
                    ...
                    */
                #endregion
                try
                {
                    log.Debug("친구제대 정보");
                    JObject response = Parser.Json.ParseJObject(response_string);
                    if (response != null)
                    {
                        // 친구제대 소환 횟수 갱신
                        int borrowTeamToday = Parser.Json.ParseInt(response["borrow_team_today"]);
                        UserData.borrowTeamToday = borrowTeamToday;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get {0}", uri);
                }
            }
            // 친구제대 소환
            // ("Mission/reinforceFriendTeam")
            else if (uri.EndsWith("Mission/reinforceFriendTeam"))
            {
                #region Packet Example
                // request Mission/reinforceFriendTeam?uid={0}&outdatacode={1}&req_id={2} 
                /*
                    {
                        "spot_id": 54,
                        "friend_team_id": -10947,
                        "group_id": 1,
                        "friend_gunids": [
                            77166361,
                            280828394,
                            319123650,
                            326422869,
                            77891175
                        ]
                    }
                    */
                #endregion
                try
                {
                    log.Debug("친구제대 소환");
                    UserData.borrowTeamToday++;
                    if (UserData.borrowTeamToday > 20)
                    {
                        UserData.borrowTeamToday = 20;
                    }

                    // 임무 갱신
                    UserData.Quest.Daily.callReinforce += 1;
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get {0}", uri);
                }
            }
            // 자료추출 완료
            // ("Mission/coinBattleFinish")
            else if (uri.EndsWith("Mission/coinBattleFinish"))
            {
                PacketProcess.Mission.FinishCoinBattle(request_string, response_string);
            }
            // 방어훈련 시작
            // ("Mission/startTrial")
            else if (uri.EndsWith("Mission/startTrial"))
            {
                PacketProcess.Mission.StartTrial(request_string, response_string);
            }
            // 상점 패키지 구매
            // ("Mall/gemToGiftbag")
            else if (uri.EndsWith("Mall/gemToGiftbag"))
            {
                PacketProcess.Mall.BuyPackage(request_string, response_string);
            }
            // 상점 시설 구매 (병영, 창고 등)
            else if (uri.EndsWith("Mall/gemToMax"))
            {
                PacketProcess.Mall.BuyInfra(request_string, response_string);
            }
            // 국지전 웨이브 시작
            // ("Theater/startTheaterExercise")
            else if (uri.EndsWith("Theater/startTheaterExercise"))
            {
                #region Packet Example
                /*  request Theater/startTheaterExercise
                    {
                        "theater_area_id": 414,
                        "team_ids": [1,2,3],
                        "squads": [1,2,3]
                    }
                    */
                /*  response Theater/startTheaterExercise
                    {
                        "enemy_teams": "3,1,0,0,0,1,2,1,3,0"
                    }
                    */
                #endregion

                try
                {
                    log.Debug("국지전 웨이브 시작");

                    if (Config.Setting.exportTheaterExercise)
                    {
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.All,
                            subject = LanguageResources.Instance["MESSAGE_THEATER_EXERCISE_SAVE_WAVE_SUBJECT"],
                            content = LanguageResources.Instance["MESSAGE_THEATER_EXERCISE_SAVE_WAVE_CONTENT"],
                        });
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get {0}", uri);
                }
            }
            // 탐색 시작
            // ("Explore/start")
            else if (uri.EndsWith("Explore/start"))
            {
                PacketProcess.Explore.StartExplore(request_string, response_string);
            }
            // 탐색 정산
            // ("Explore/balance")
            else if (uri.EndsWith("Explore/balance"))
            {
                PacketProcess.Explore.BalanceExplore(request_string, response_string);
            }
            // 탐색 취소
            // ("Explore/cancel")
            else if (uri.EndsWith("Explore/cancel"))
            {
                PacketProcess.Explore.CancelExplore(request_string, response_string);
            }
            // 탐색 이벤트 가져오기
            // ("Explore/getEvent")
            else if (uri.EndsWith("Explore/getEvent"))
            {
                PacketProcess.Explore.GetEvent(request_string, response_string);
            }
            // 탐색 편성
            // ("Explore/setTeam")
            else if (uri.EndsWith("Explore/setTeam"))
            {
                PacketProcess.Explore.SetTeam(request_string, response_string);
            }
            // 탐색 우세 가져오기
            // ("Explore/getAdaptiveTeam")
            else if (uri.EndsWith("Explore/getAdaptiveTeam"))
            {
                PacketProcess.Explore.GetAdaptiveTeam(request_string, response_string);
            }
            // 탐색 커리어
            // ("Explore/career")
            else if (uri.EndsWith("Explore/career"))
            {
                #region Packet Example
                // request Explore/career?uid=343650&outdatacode=rfAzmtPLjc4hGJZbTy9C6FW7XHJwRDk8BhfbsIk%2bJifBvr64iY77Xyo%3d&req_id=156534535700009 
                // {"career_id":2} 
                // response Explore/career
                // 1
                #endregion
            }
            // 탐색 블랙마켓 구매
            // ("Explore/buy")
            else if (uri.EndsWith("Explore/buy"))
            {
                PacketProcess.Mall.BuyPackage(request_string, response_string);
            }
            // 숙소 선물
            // ("Dorm/giftToGun")
            else if (uri.EndsWith("Dorm/giftToGun"))
            {
                PacketProcess.Dorm.GiveGift(request_string, response_string);
            }
            // 스킨변경
            // ("Dorm/changeSkin")
            else if (uri.EndsWith("Dorm/changeSkin"))
            {
                PacketProcess.Dorm.ChangeSkin(request_string, response_string);
            }
            // 공유전지 획득
            // ("Dorm/get_build_coin")
            else if (uri.EndsWith("Dorm/get_build_coin"))
            {
                #region Packet Example
                // request Dorm/get_build_coin?uid={0}&outdatacode={1}&req_id={2}
                // {"v_user_id":"1468437","dorm_id":0}
                // response Dorm/get_build_coin
                // {"build_coin":10,"remain_coin":35} 
                #endregion
                try
                {
                    log.Debug("공유전지 획득");
                    JObject request = Parser.Json.ParseJObject(request_string);
                    JObject response = Parser.Json.ParseJObject(response_string);
                    if (response != null)
                    {
                        int dormId = request["dorm_id"].Value<int>();
                        int buildCoin = Parser.Json.ParseInt(response["build_coin"]);
                        int remainCoin = Parser.Json.ParseInt(response["remain_coin"]);

                        log.Debug("숙소 {0} (0: 친구 숙소)", dormId);
                        log.Debug("획득 전지 {0}", buildCoin);
                        log.Debug("남은 전지? {0} 번", remainCoin);

                        // 친구 숙소에서 전지를 획득한 경우에만
                        if (dormId == 0 && buildCoin > 0)
                        {
                            UserData.remainFriendBattery--;

                            // 임무 갱신
                            UserData.Quest.Daily.getBattery += 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get {0}", uri);
                }
            }
            // 공유 보석
            // ("Dorm/share")
            else if (uri.EndsWith("Dorm/share"))
            {
                #region Packet Example
                // request Dorm/share?uid=621225&signcode=0XU9NyD5FQiX9b2Rw%2fz9byKdCTGkjLTJFIJNxCU29JjqvsYIs41pHtLUWCXy4l0g6G9GMBWdB4uNYA%3d%3d&req_id=156564165900008
                // null
                // response Dorm/share
                // {"prize_id":10000} 
                #endregion
                try
                {
                    log.Debug("공유보석 획득");
                    JObject response = Parser.Json.ParseJObject(response_string);
                    if (response != null)
                    {
                        int prizeId = Parser.Json.ParseInt(response["prize_id"]);
                        if (prizeId == 10000)
                        {
                            MainWindow.dashboardView.isSharedGem = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get {0}", uri);
                }
            }
            // 인형 복구
            // ("Gun/coreRecoverGun")
            else if (uri.EndsWith("Gun/coreRecoverGun"))
            {
                PacketProcess.Gun.RecoverGun(request_string, response_string);
            }
        }

        /// <summary>
        /// 파일 패킷
        /// </summary>
        private static string[] filePaths = new string[] 
        {
            ".html",
            ".js",
            ".txt",
            ".jpg",
            ".png",
            ".css",
            ".ico",
        };
        private static bool IsFilePath(string path)
        {
            foreach (string item in filePaths)
            {
                if (path.Contains(item))
                    return true;
            }
            return false;
        }
    }
}