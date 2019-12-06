using GFAlarm.Constants;
using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Notifier;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using LocalizationResources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static GFAlarm.MainWindow;

namespace GFAlarm.Transaction.PacketProcess
{
    public class Index
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Sign 키 가져오기
        /// </summary>
        /// <param name="packet"></param>
        public static void GetUid(GFPacket packet)
        {
            string body = packet.body;
            string decodedBody = AuthCode.Decode(body, "yundoudou");
            JObject response = Parser.Json.ParseJObject(decodedBody);
            if (response != null && response.ContainsKey("sign"))
            {
                UserData.sign = Parser.Json.ParseString(response.GetValue("sign"));
                //log.Debug(response.ToString());

                MainWindow.view.Connection = MainWindow.ConnectionStatus.Connect;
            }
        }

        /// <summary>
        /// 사용자 정보 가져오기
        /// (전체 진행상황 가져오기)
        /// </summary>
        /// <param name="packet"></param>
        public static void GetIndex(GFPacket packet)
        {
            log.Debug("전체 정보 가져오는 중...");

            MainWindow.view.forceStop = true;       // 타이머 중지

            // 기존 정보 초기화
            log.Debug("기존 정보 초기화 중...");
            try
            {
                dashboardView.ClearAll();
                echelonView.ClearAll();
                questView.ClearAll();
                UserData.ClearUserData();
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to reset user info");
            }

            // 패킷 복호화
            string body = packet.body;
            JObject index;
            try
            {
                string decodedBody = AuthCode.Decode(body, UserData.sign);
                if (!string.IsNullOrEmpty(decodedBody))
                    index = Parser.Json.ParseJObject(decodedBody);
                else
                    index = Parser.Json.ParseJObject(body);
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to decode packet");
                return;
            }

            // 사용자 설정
            log.Debug("사용자 설정 확인...");
            try
            {
                if (index.ContainsKey("user_record") && index["user_record"] is JObject)
                {
                    JObject user_record = index["user_record"].Value<JObject>();

                    if (user_record.ContainsKey("attendance_type1_time"))
                    {
                        log.Debug("출석시간 확인...");
                        int attendance_time = user_record["attendance_type1_time"].Value<int>();
                        log.Debug("출석시간(UTC) {0}", TimeUtil.GetDateTime(attendance_time, "MM-dd mm:ss", true));
                        UserData.attendanceTime = attendance_time;

                        //log.Debug("출석시간 확인...");
                        //long attendance_time = user_record["attendance_type1_time"].Value<long>() * 1000;
                        //log.Debug("출석시간(UTC) {0}", Parser.Time.GetDateTime(attendance_time, true).ToString("MM-dd mm:ss"));
                        //UserData.attendanceTime = attendance_time;
                    }

                    log.Debug("부관 정보 확인...");
                    List<string> adjutant = Parser.Json.ParseString(user_record["adjutant"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).Where(s => s != string.Empty).ToList();
                    List<string> adjutantFairy = Parser.Json.ParseString(user_record["adjutant_fairy"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).Where(s => s != string.Empty).ToList();

                    if (adjutant.Count() == 4)
                    {
                        // 인형 ID, 스킨 ID, ?, 개조
                        // 20064, 2407, 0, 3
                        // -1 카리나

                        UserData.adjutantDoll = Parser.String.ParseInt(adjutant[0]);
                        UserData.adjutantDollSkin = Parser.String.ParseInt(adjutant[1]);
                        log.Debug("인형 부관 {0} {1}", UserData.adjutantDoll, UserData.adjutantDollSkin);
                    }
                    else if (adjutantFairy.Count() == 3)
                    {
                        // 요정 ID, 요정 외형, ?
                        // 11, 33, 0

                        UserData.adjutantFairy = Parser.String.ParseInt(adjutantFairy[0]);
                        log.Debug("요정 부관 {0}", UserData.adjutantFairy);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get user record");
            }

            // 사용자 정보
            log.Debug("사용자 정보 확인...");
            try
            {
                if (index["user_info"] != null && index["user_info"] is JObject)
                {
                    JObject user_info = index["user_info"].Value<JObject>();
                    UserData.uid = Parser.Json.ParseInt(user_info["user_id"]);
                    log.Debug("UID {0}", UserData.uid);
                    UserData.name = Parser.Json.ParseString(user_info["name"]);
                    log.Debug("닉네임 {0}", UserData.name);
                    UserData.level = Parser.Json.ParseInt(user_info["lv"]);
                    log.Debug("레벨 {0}", UserData.level);
                    UserData.maxDollCount = Parser.Json.ParseInt(user_info["maxgun"]);
                    log.Debug("병영크기 {0}", UserData.maxDollCount);
                    UserData.maxEquipCount = Parser.Json.ParseInt(user_info["maxequip"]);
                    log.Debug("창고크기 {0}", UserData.maxEquipCount);
                    UserData.gem = Parser.Json.ParseInt(user_info["gem"]);
                    log.Debug("보석 {0}", UserData.gem);
                    try
                    {
                        UserData.CombatSimulation.pauseRefresh = true;
                        int point = Parser.Json.ParseInt(user_info["bp"]);
                        if (point == 6)
                        {
                            UserData.CombatSimulation.lastRecoverTime = TimeUtil.GetCurrentSec();
                            UserData.CombatSimulation.recoverTime = 0;
                        }
                        else
                        {
                            int now = TimeUtil.GetCurrentSec();
                            int lastRecoverTime = Parser.Json.ParseInt(user_info["last_bp_recover_time"]);
                            int recoverTime = lastRecoverTime + TimeUtil.HOUR * 2;
                            while (recoverTime < now && point < 6)
                            {
                                point++;
                                lastRecoverTime += TimeUtil.HOUR * 2;
                                recoverTime += TimeUtil.HOUR * 2;
                            }
                            if (recoverTime < now && point == 6)
                            {
                                recoverTime = 0;
                            }
                            UserData.CombatSimulation.point = point;
                            UserData.CombatSimulation.recoverTime = recoverTime;
                            UserData.CombatSimulation.lastRecoverTime = lastRecoverTime;
                        }
                        UserData.CombatSimulation.pauseRefresh = false;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    UserData.CombatSimulation.remainPointToday = UserData.CombatSimulation.GetRemainPoint();
                    //if (0 <= UserData.CombatSimulation.point && UserData.CombatSimulation.point < 6)
                    //{


                        //UserData.CombatSimulation.lastRecoverTime = Parser.Json.ParseInt(user_info["last_bp_recover_time"]);
                    //}
                    UserData.regTime = Parser.Json.ParseInt(user_info["reg_time"]);
                    UserData.mp = Parser.Json.ParseInt(user_info["mp"]);
                    UserData.ammo = Parser.Json.ParseInt(user_info["ammo"]);
                    UserData.mre = Parser.Json.ParseInt(user_info["mre"]);
                    UserData.part = Parser.Json.ParseInt(user_info["part"]);
                    log.Debug("인력 {0}, 탄약 {1}, 식량 {2}, 부품 {3}", 
                        UserData.mp, UserData.ammo, UserData.mre, UserData.part);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "사용자 정보 확인 중 에러 발생");
            }

            // 공유보석
            log.Debug("공유보석 확인...");
            try
            {
                if (index["share_with_user_info"] != null && index["share_with_user_info"] is JObject)
                {
                    JObject share_with_user_info = index["share_with_user_info"].Value<JObject>();
                    int now_time = TimeUtil.GetCurrentSec();
                    int last_time = Parser.Json.ParseInt(share_with_user_info["last_time"]);
                    //long now_time = Parser.Time.GetCurrentMs();
                    //long last_time = Parser.Json.ParseLong(share_with_user_info["last_time"]) * 1000;
                    log.Debug("공유보석 마지막 공유시간 {0}", last_time);
                    if (last_time < now_time)
                    {
                        log.Debug("공유보석 미수령");
                        MainWindow.dashboardView.isSharedGem = false;
                    }
                    else
                    {
                        log.Debug("공유보석 수령");
                        MainWindow.dashboardView.isSharedGem = true;
                    }
                }
                else
                {
                    log.Debug("공유보석 확인할 수 없음");
                    MainWindow.dashboardView.isSharedGem = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "공유보석 확인 중 에러 발생");
            }

            // 공유전지
            log.Debug("공유전지 확인...");
            try
            {
                if (index.ContainsKey("dorm_rest_friend_build_coin_count"))
                {
                    int remain_friend_battery = Parser.Json.ParseInt(index["dorm_rest_friend_build_coin_count"]);
                    UserData.remainFriendBattery = remain_friend_battery;
                }
                else
                {
                    log.Debug("공유전지 확인할 수 없음");
                    UserData.remainFriendBattery = 0;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "공유전지 확인 중 에러 발생");
            }

            // 모의작전점수
            log.Debug("모의작전점수 확인...");
            log.Debug("모의작전점수 {0} 점", UserData.CombatSimulation.point);

            // 소속인형
            log.Debug("인형 확인...");
            if (index.ContainsKey("gun_with_user_info") && index["gun_with_user_info"] is JArray)
            {
                UserData.Doll.Init(index["gun_with_user_info"].Value<JArray>());
                log.Debug("인형 {0} 명", UserData.Doll.Count());
            }

            // 소유장비
            log.Debug("장비 확인...");
            if (index.ContainsKey("equip_with_user_info"))
            {
                UserData.Equip.Init(index["equip_with_user_info"]);
                log.Debug("장비 {0} 개", UserData.Equip.Count());
            }

            // 소속요정
            log.Debug("요정 확인...");
            if (index.ContainsKey("fairy_with_user_info"))
            {
                UserData.Fairy.Init(index["fairy_with_user_info"]);
                log.Debug("요정 {0} 명", UserData.Fairy.Count());
            }

            // 소유아이템
            log.Debug("아이템 확인...");
            if (index.ContainsKey("item_with_user_info"))
            {
                UserData.Item.Init(index["item_with_user_info"]);
                log.Debug("아이템 {0} 개", UserData.Item.Count());

                try
                {
                    if (UserData.Item.ContainsKey(507))
                    {
                        UserData.GlobalExp.exp = UserData.Item.Get(507).number;
                    }
                    else
                    {
                        UserData.GlobalExp.exp = 0;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "자유경험치 데이터 에러");
                }
            }

            // 중장비부대
            log.Debug("중장비부대 확인...");
            if (index.ContainsKey("squad_with_user_info"))
            {
                UserData.Squad.Init(index["squad_with_user_info"]);
                log.Debug("중장비부대 {0} 부대", UserData.Squad.Count());
            }

            // 전역
            log.Debug("전역 정보 확인...");
            if (index.ContainsKey("mission_with_user_info"))
            {
                UserData.Mission.Init(index["mission_with_user_info"]);
                log.Debug("전역 정보 {0} 개", UserData.Mission.Count());
            }

            // 현재 전역
            log.Debug("현재 전역 확인...");
            if (index.ContainsKey("mission_act_info"))
            {
                try
                {
                    MissionActInfo missionActInfo = new MissionActInfo(index["mission_act_info"]);
                    UserData.CurrentMission.id = missionActInfo.missionId;
                    UserData.CurrentMission.turnCount = missionActInfo.turn;
                    foreach (KeyValuePair<int, int> item in missionActInfo.teamSpots)
                    {
                        UserData.CurrentMission.ReinforceTeam(item.Key, item.Value);
                    }
                    log.Debug("현재 작전 투입된 제대 {0} 팀", UserData.CurrentMission.teamSpots.Count());
                }
                catch (Exception ex)
                {
                    log.Error(ex, "현재 전역 확인 중 에러 발생");
                }
            }
            // 현재 전역 스팟
            log.Debug("현재 전역 거점 확인...");
            try
            {
                if (index.ContainsKey("spot_act_info") && index["spot_act_info"] is JArray)
                {
                    JArray items = index["spot_act_info"].Value<JArray>();
                    log.Debug("전역 거점 {0} 개", items.Count());
                    foreach (JObject item in items)
                    {
                        long enemyTeamId = Parser.Json.ParseLong(item["enemy_team_id"]);
                        if (enemyTeamId > 0)
                        {
                            int spotId = Parser.Json.ParseInt(item["spot_id"]);
                            UserData.CurrentMission.MoveEnemyTeam(enemyTeamId, 0, spotId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "현재 전역 거점 확인 중 에러 발생");
            }

            // 군수지원 진행상태
            log.Debug("군수지원 확인...");
            if (index.ContainsKey("operation_act_info") && index["operation_act_info"] is JArray)
            {
                #region Packet Example
                /*
                "operation_act_info": [
                    {
                        "id": "105423083",
                        "operation_id": "2",
                        "user_id": "343650",
                        "team_id": "7",
                        "start_time": "1559130911"
                    },
                    {
                        "id": "105425627",
                        "operation_id": "29",
                        "user_id": "343650",
                        "team_id": "8",
                        "start_time": "1559133209"
                    },
                    {
                        "id": "105425323",
                        "operation_id": "33",
                        "user_id": "343650",
                        "team_id": "10",
                        "start_time": "1559132922"
                    },
                    {
                        "id": "105424824",
                        "operation_id": "38",
                        "user_id": "343650",
                        "team_id": "9",
                        "start_time": "1559132498"
                    }
                ],
                */
                #endregion
                try
                {
                    JArray items = index["operation_act_info"].Value<JArray>();
                    log.Debug("진행 중인 군수지원 {0} 건", items.Count());
                    foreach (var item in items)
                    {
                        int team_id = Parser.Json.ParseInt(item["team_id"]);
                        if (Util.Common.IsValidTeamId(team_id))
                        {
                            int operation_id = Parser.Json.ParseInt(item["operation_id"]);
                            int start_time = Parser.Json.ParseInt(item["start_time"]);

                            dashboardView.Add(new DispatchedEchleonTemplate()
                            {
                                operationId = operation_id,
                                teamId = team_id,
                                startTime = start_time,
                            });
                        }
                        else
                        {
                            log.Warn("군수지원 추가 실패 - 잘못된 제대번호");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "군수지원 진행상태 확인 중 에러 발생");
                }
            }

            // 자율작전 진행상태
            log.Debug("자율작전 확인...");
            if (index.ContainsKey("auto_mission_act_info") && index["auto_mission_act_info"] is JArray)
            {
                #region Packet Example
                /*
                    "auto_mission_act_info": [
                        {
                            "id": "6313655",
                            "user_id": "343650",
                            "auto_mission_id": "109",
                            "team_ids": "5,6",
                            "end_time": "1558819450",
                            "number": "1"
                        }
                    ],
                 */
                #endregion
                try
                {
                    JArray items = index["auto_mission_act_info"].Value<JArray>();
                    log.Debug("진행 중인 자율작전 {0} 건", items.Count());
                    foreach (var item in items)
                    {
                        int auto_mission_id = Parser.Json.ParseInt(item["auto_mission_id"]);
                        string teamIdsString = Parser.Json.ParseString(item["team_ids"]);
                        int[] teamIds;
                        if (teamIdsString.Contains(","))
                            teamIds = teamIdsString.Split(',').Select(Int32.Parse).ToArray();
                        else
                            teamIds = new int[] { Parser.String.ParseInt(teamIdsString) };
                        //List<int> teamIds = new List<int>();
                        //if (teamIdsString.Contains(","))
                        //    teamIds = teamIdsString.Split(',').Select(Int32.Parse).ToList();
                        //else
                        //    teamIds.Add(Parser.String.ParseInt(teamIdsString));
                        int end_time = Parser.Json.ParseInt(item["end_time"]);
                        int number = Parser.Json.ParseInt(item["number"]);

                        dashboardView.Add(new DispatchedEchleonTemplate()
                        {
                            autoMissionNumber = number,
                            autoMissionId = auto_mission_id,
                            teamIds = teamIds,
                            endTime = end_time,
                        });

                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "자율작전 진행상태 확인 중 에러 발생");
                }
            }

            // 수복현황 진행상태
            log.Debug("수복 확인...");
            if (index.ContainsKey("fix_act_info") && index["fix_act_info"] is JArray)
            {
                #region Packet Example
                /*
                  "fix_act_info": [
                    {
                      "id": "62287353",
                      "user_id": "343650",
                      "gun_with_user_id": "190312255",
                      "fix_slot": "1",
                      "start_time": "1564725155"
                    }
                  ],
                 */
                #endregion
                try
                {
                    JArray items = index["fix_act_info"].Value<JArray>();
                    log.Debug("진행 중인 인형수복 {0} 건", items.Count());
                    foreach (var item in items)
                    {
                        int fixSlot = Parser.Json.ParseInt(item["fix_slot"]);
                        long gunWithUserId = Parser.Json.ParseLong(item["gun_with_user_id"]);
                        int startTime = Parser.Json.ParseInt(item["start_time"]);
                        int[] restore = GameData.Doll.Restore.GetRestore(gunWithUserId);
                        //UserData.Doll.GetRestoreRequireTime(gunWithUserId, ref requireTime, ref requireMp, ref requirePart);
                        int endTime = startTime + restore[0];

                        dashboardView.Add(new RestoreDollTemplate()
                        {
                            slot = fixSlot,
                            gunWithUserId = gunWithUserId,
                            endTime = endTime,
                        });
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "수복 진행상태 확인 중 에러 발생");
                }
            }

            // 인형제조 진행상태
            log.Debug("인형제조 확인...");
            if (index.ContainsKey("develop_act_info") && index["develop_act_info"] is JArray)
            {
                #region Packet Example
                /*
                "develop_act_info": [
                    {
                        "id": "61158623",
                        "build_slot": "1",
                        "user_id": "343650",
                        "gun_id": "22",
                        "equip_id": "0",
                        "start_time": "1559151184",
                        "mp": "130",
                        "ammo": "130",
                        "mre": "130",
                        "part": "30",
                        "input_level": "0",
                        "core": "0",
                        "item1_num": "1"
                    }
                ],
                 */
                #endregion
                try
                {
                    JArray items = index["develop_act_info"].Value<JArray>();
                    log.Debug("진행 중인 인형제조 {0} 건", items.Count());
                    foreach (var item in items)
                    {
                        int buildSlot = Parser.Json.ParseInt(item["build_slot"]);
                        int gunId = Parser.Json.ParseInt(item["gun_id"]);
                        int startTime = Parser.Json.ParseInt(item["start_time"]);

                        int mp = Parser.Json.ParseInt(item["mp"]);
                        int ammo = Parser.Json.ParseInt(item["ammo"]);
                        int mre = Parser.Json.ParseInt(item["mre"]);
                        int part = Parser.Json.ParseInt(item["part"]);
                        int input_level = Parser.Json.ParseInt(item["input_level"]);

                        dashboardView.Add(new ProduceDollTemplate()
                        {
                            slot = buildSlot,
                            gunId = gunId,
                            startTime = startTime,
                            inputLevel = input_level,
                            spendResource = new int[] { mp, ammo, mre, part },
                        });
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "인형제조 진행상태 확인 중 에러 발생");
                }
            }

            // 장비제조 진행상태
            log.Debug("장비제조 확인...");
            if (index.ContainsKey("develop_equip_act_info") && index["develop_equip_act_info"] is JObject)
            {
                #region Packet Example
                /*
                "develop_equip_act_info": {
                    "6": {
                        "type": 1,
                        "fairy_id": 4,
                        "passive_skill": 910114,
                        "quality_lv": 1,
                        "build_slot": 6,
                        "user_id": 343650,
                        "start_time": 1558833838,
                        "mp": 500,
                        "ammo": 500,
                        "mre": 500,
                        "part": 500,
                        "input_level": 1,
                        "core": "2",
                        "item_num": "1"
                    },
                    "2": {
                        "type": 1,
                        "fairy_id": 17,
                        "passive_skill": 910111,
                        "quality_lv": 1,
                        "build_slot": 2,
                        "user_id": 343650,
                        "start_time": 1558837900,
                        "mp": 500,
                        "ammo": 500,
                        "mre": 500,
                        "part": 500,
                        "input_level": 1,
                        "core": "2",
                        "item_num": "1"
                    },
                */
                #endregion
                try
                {
                    JObject develop_equip_act_info = index["develop_equip_act_info"].Value<JObject>();
                    List<string> keys = develop_equip_act_info.Properties().Select(p => p.Name).ToList();
                    log.Debug("진행 중인 장비제조 {0} 건", keys.Count());
                    foreach (string key in keys)
                    {
                        JObject token = develop_equip_act_info.GetValue(key).Value<JObject>();
                        int build_slot = Parser.Json.ParseInt(token["build_slot"]);
                        int equip_id = Parser.Json.ParseInt(token["equip_id"]);
                        int fairy_id = Parser.Json.ParseInt(token["fairy_id"]);
                        int passive_skill = 0;
                        if (token.ContainsKey("passive_skill"))
                            passive_skill = Parser.Json.ParseInt(token["passive_skill"]);

                        int input_level = Parser.Json.ParseInt(token["input_level"]);
                        int mp = Parser.Json.ParseInt(token["mp"]);
                        int ammo = Parser.Json.ParseInt(token["ammo"]);
                        int mre = Parser.Json.ParseInt(token["mre"]);
                        int part = Parser.Json.ParseInt(token["part"]);

                        int start_time = Parser.Json.ParseInt(token["start_time"]);

                        dashboardView.Add(new ProduceEquipTemplate()
                        {
                            slot = build_slot,
                            equipId = equip_id,
                            fairyId = fairy_id,
                            passiveSkill = passive_skill,
                            inputLevel = input_level,
                            spendResource = new int[] { mp, ammo, mre, part },
                            startTime = start_time,
                        });
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "장비제조 진행상태 확인 중 에러 발생");
                }
            }

            // 스킬훈련 진행상태
            log.Debug("스킬훈련 확인...");
            if (index.ContainsKey("upgrade_act_info") && index["upgrade_act_info"] is JArray)
            {
                #region Packet Example
                /*
                    "upgrade_act_info": [
                        {
                            "user_id": "343650",
                            "gun_with_user_id": "309875627",
                            "skill": "2",
                            "upgrade_slot": "1",
                            "fairy_with_user_id": "0",
                            "end_time": "1559142812"
                        },
                        {
                            "user_id": "343650",
                            "gun_with_user_id": "340775838",
                            "skill": "1",
                            "upgrade_slot": "3",
                            "fairy_with_user_id": "0",
                            "end_time": "1559164419"
                        },
                        {
                            "user_id": "343650",
                            "gun_with_user_id": "8812180",
                            "skill": "2",
                            "upgrade_slot": "2",
                            "fairy_with_user_id": "0",
                            "end_time": "1559169967"
                        }
                    ],
                 */
                #endregion
                try
                {
                    JArray items = index["upgrade_act_info"].Value<JArray>();
                    log.Debug("진행 중인 스킬훈련 {0} 건", items.Count());
                    foreach (var item in items)
                    {
                        int upgrade_slot = Parser.Json.ParseInt(item["upgrade_slot"]);
                        long gun_with_user_id = Parser.Json.ParseLong(item["gun_with_user_id"]);
                        long fairy_with_user_id = Parser.Json.ParseLong(item["fairy_with_user_id"]);
                        int skill = Parser.Json.ParseInt(item["skill"]);
                        int end_time = Parser.Json.ParseInt(item["end_time"]);

                        dashboardView.Add(new SkillTrainTemplate()
                        {
                            slot = upgrade_slot,
                            skill = skill,
                            gunWithUserId = gun_with_user_id,
                            fairyWithUserId = fairy_with_user_id,
                            endTime = end_time,
                        });
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "스킬훈련 진행상태 확인 중 에러 발생");
                }
            }

            // 중장비 스킬훈련 진행상태
            log.Debug("중장비 스킬훈련 확인...");
            if (index.ContainsKey("squad_skill_act_info") && index["squad_skill_act_info"] is JObject)
            {
                #region Packet Example
                /*
                    "squad_skill_act_info": {
                        "1": {
                            "user_id": "343650",
                            "squad_with_user_id": "28314",
                            "skill": "3",
                            "squad_skill_slot": "1",
                            "end_time": "1563854176"
                        }
                    },
                */
                #endregion
                try
                {
                    JObject squad_skill_act_info = index["squad_skill_act_info"].Value<JObject>();
                    List<string> keys = squad_skill_act_info.Properties().Select(p => p.Name).ToList();
                    log.Debug("진행 중인 중장비 스킬훈련 {0} 건", keys.Count());
                    foreach (string key in keys)
                    {
                        JObject token = squad_skill_act_info.GetValue(key).Value<JObject>();
                        long squad_with_user_id = Parser.Json.ParseLong(token["squad_with_user_id"]);
                        int squad_skill_slot = Parser.Json.ParseInt(token["squad_skill_slot"]);
                        int skill = Parser.Json.ParseInt(token["skill"]);
                        int end_time = Parser.Json.ParseInt(token["end_time"]);

                        dashboardView.Add(new SkillTrainTemplate()
                        {
                            slotType = 1,
                            slot = squad_skill_slot,
                            skill = skill,
                            squadWithUserId = squad_with_user_id,
                            endTime = end_time,
                        });
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "중장비 스킬훈련 진행상태 확인 중 에러 발생");
                }
            }

            // 중장비 경험훈련 진행상태
            log.Debug("중장비 경험훈련 확인...");
            if (index.ContainsKey("squad_train_act_info") && index["squad_train_act_info"] is JObject)
            {
                #region Packet Example
                /*
                    "squad_train_act_info": {
                        "1": {
                            "user_id": "343650",
                            "squad_with_user_id": "38664",
                            "squad_train_slot": "1",
                            "end_time": "1569904165",
                            "add_exp": "540000",
                            "num": "12",
                            "cost_item_info": "{\"46\":180,\"506\":60}"
                        }
                    },
                 */
                #endregion
                try
                {
                    JObject squad_train_act_info = index["squad_train_act_info"].Value<JObject>();
                    List<string> keys = squad_train_act_info.Properties().Select(p => p.Name).ToList();
                    log.Debug("진행 중인 중장비 경험훈련 {0} 건", keys.Count());
                    foreach (string key in keys)
                    {
                        JObject token = squad_train_act_info.GetValue(key).Value<JObject>();
                        long squad_with_user_id = Parser.Json.ParseLong(token["squad_with_user_id"]);
                        int squad_train_slot = Parser.Json.ParseInt(token["squad_train_slot"]);
                        long add_exp = Parser.Json.ParseLong(token["add_exp"]);
                        int num = Parser.Json.ParseInt(token["num"]);
                        int end_time = Parser.Json.ParseInt(token["end_time"]);

                        dashboardView.Add(new SkillTrainTemplate()
                        {
                            slotType = 1,
                            slot = squad_train_slot,
                            squadWithUserId = squad_with_user_id,
                            addExp = add_exp,
                            reportNum = num,
                            endTime = end_time,
                        });
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "중장비 경험훈련 진행상태 확인 중 에러 발생");
                }
            }

            // 시설정보 가져오기
            log.Debug("시설정보 확인...");
            if (index.ContainsKey("outhouse_establish_info") && index["outhouse_establish_info"] is JArray)
            {
                #region Packet Example
                /*
                    {
                        "id": "311188",
                        "user_id": "343650",
                        "room_id": "2",
                        "establish_id": "20110",
                        "establish_type": "201",
                        "furniture_id": "42013",
                        "upgrade_establish_id": "0",
                        "upgrade_starttime": "0",
                        "build_starttime": "1562341653",
                        "build_num": "80",
                        "build_tmp_data": "80,200001,240000,build_coin,240",
                        "build_get_time": "1562327080",
                        "update_furniture_id": "42013",
                        "furniture_postion": "8,8",
                        "establish_lv": "10",
                        "upgrade_coin": "1200",
                        "upgrade_time": "86400",
                        "upgrade_condition": "20209,20409",
                        "parameter_1": "200001:3",
                        "parameter_2": "0",
                        "parameter_3": "0"
                    },
                 */
                #endregion
                try
                {
                    int serverLv = 0;
                    //int computer_lv = 0;
                    int tableLv = 0;
                    int battleReportBuildCount = 0;
                    int battleReportBuildStartTime = 0;

                    JArray items = index["outhouse_establish_info"].Value<JArray>();
                    log.Debug("전체 시설 갯수 {0} 개", items.Count());
                    foreach (JObject item in items)
                    {
                        int furnitureId = Parser.Json.ParseInt(item["furniture_id"]);
                        int establishLv = Parser.Json.ParseInt(item["establish_lv"]);
                        switch (furnitureId)
                        {
                            // 서버 컴퓨터 (작전보고서 작성시간)
                            case 42040:
                            case 42041:
                            case 42042:
                            case 42043:
                                log.Debug("시설 - 자료실 - 서버 레벨 {0}", establishLv);
                                serverLv = establishLv;
                                break;
                            // 컴퓨터 시설 (작전보고서 작성갯수)
                            case 42010:
                            case 42011:
                            case 42012:
                            case 42013:
                                log.Debug("시설 - 자료실 - 컴퓨터 레벨 {0}", establishLv);
                                battleReportBuildCount = Parser.Json.ParseInt(item["build_num"]);
                                battleReportBuildStartTime = Parser.Json.ParseInt(item["build_starttime"]);
                                //battleReportBuildStartTime = Parser.Json.ParseLong(item["build_starttime"]) * 1000;
                                break;
                            // 데이터 테이블 시설 (자유경험치 상한)
                            case 42050:
                            case 42051:
                            case 42052:
                            case 42053:
                                log.Debug("시설 - 자료실 - 테이블 레벨 {0}", establishLv);
                                tableLv = establishLv;
                                break;
                            // 기상 체계 시설 (요정 경험치 배율)
                            case 43040:
                            case 43041:
                            case 43042:
                            case 43043:
                                log.Debug("시설 - 요정의 방 - 기상체계 레벨 {0}", establishLv);
                                switch (establishLv)
                                {
                                    case 0:
                                        GameData.Fairy.Exp.expRatio = 0.05;
                                        break;
                                    case 1:
                                        GameData.Fairy.Exp.expRatio = 0.065;
                                        break;
                                    case 2:
                                        GameData.Fairy.Exp.expRatio = 0.08;
                                        break;
                                    case 3:
                                        GameData.Fairy.Exp.expRatio = 0.095;
                                        break;
                                    case 4:
                                        GameData.Fairy.Exp.expRatio = 0.11;
                                        break;
                                    case 5:
                                        GameData.Fairy.Exp.expRatio = 0.125;
                                        break;
                                    case 6:
                                        GameData.Fairy.Exp.expRatio = 0.14;
                                        break;
                                    case 7:
                                        GameData.Fairy.Exp.expRatio = 0.155;
                                        break;
                                    case 8:
                                        GameData.Fairy.Exp.expRatio = 0.17;
                                        break;
                                    case 9:
                                        GameData.Fairy.Exp.expRatio = 0.185;
                                        break;
                                    case 10:
                                        GameData.Fairy.Exp.expRatio = 0.2;
                                        break;
                                    default:
                                        GameData.Fairy.Exp.expRatio = 0.2;
                                        break;
                                }
                                break;
                            // 분석기 시설 (정보분석 시간)
                            case 45040:
                            case 45041:
                            case 45042:
                            case 45043:
                                log.Debug("시설 - 정보센터 - 분석기 레벨 {0}", establishLv);
                                UserData.Facility.IntelCenter.analyzerLevel = establishLv;
                                break;
                        }
                    }

                    UserData.Facility.DataRoom.serverLevel = serverLv;
                    // 작전보고서 작성갯수
                    if (battleReportBuildCount > 0)
                    {
                        UserData.BattleReport.num = battleReportBuildCount;
                    }
                    // 작전보고서 작성시간
                    if (battleReportBuildStartTime > 0)
                    {
                        UserData.BattleReport.startTime = battleReportBuildStartTime;
                    }
                    // 작전보고서 완료시간
                    //if (UserData.BattleReport.endTime < Parser.Time.GetCurrentMs())
                    if (UserData.BattleReport.endTime < TimeUtil.GetCurrentSec())
                    {
                        UserData.BattleReport.notified = true;
                    }
                    UserData.Facility.DataRoom.dataTableLevel = tableLv;
                    // 로그인부터 자유경험치가 꽉 찬 경우 알리지 않음
                    if (UserData.GlobalExp.maxExp == UserData.GlobalExp.exp)
                    {
                        UserData.GlobalExp.notified = true;
                    }
                    //MainWindow.view.SetGlobalExp(UserData.GlobalExp.exp, UserData.GlobalExp.maxExp);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "시설정보 확인 중 에러 발생");
                }
            }

            // 정보분석 진행상태
            log.Debug("정보분석 확인...");
            if (index.ContainsKey("data_analysis_act_info") && index["data_analysis_act_info"] is JArray)
            {
                #region Packet Example
                /*
                "data_analysis_act_info": [
                    {
                        "id": "14590800",
                        "user_id": "343650",
                        "build_slot": "1",
                        "input_level": "0",
                        "piece": "0",
                        "piece_item_id": "0",
                        "chip_id": "3052",
                        "color_id": "2",
                        "grid_id": "26",
                        "shape_info": "0,0",
                        "assist_damage": "2",
                        "assist_def_break": "2",
                        "assist_hit": "1",
                        "assist_reload": "0",
                        "damage": "0",
                        "atk_speed": "0",
                        "hit": "0",
                        "def": "0",
                        "end_time": "1558845386"
                    },
                    {
                        "id": "14590801",
                        "user_id": "343650",
                        "build_slot": "2",
                        "input_level": "0",
                        "piece": "0",
                        "piece_item_id": "0",
                        "chip_id": "4061",
                        "color_id": "2",
                        "grid_id": "37",
                        "shape_info": "0,0",
                        "assist_damage": "2",
                        "assist_def_break": "1",
                        "assist_hit": "2",
                        "assist_reload": "1",
                        "damage": "0",
                        "atk_speed": "0",
                        "hit": "0",
                        "def": "0",
                        "end_time": "1558845386"
                    },
                 */
                #endregion
                try
                {
                    JArray items = index["data_analysis_act_info"].Value<JArray>();
                    log.Debug("진행 중인 정보분석 {0} 건", items.Count());
                    foreach (var item in items)
                    {
                        int buildSlot = Parser.Json.ParseInt(item["build_slot"]);
                        int piece = Parser.Json.ParseInt(item["piece"]);
                        int pieceItemId = Parser.Json.ParseInt(item["piece_item_id"]);
                        int chipId = Parser.Json.ParseInt(item["chip_id"]);
                        int colorId = Parser.Json.ParseInt(item["color_id"]);
                        int gridId = Parser.Json.ParseInt(item["grid_id"]);
                        string shapeInfo = Parser.Json.ParseString(item["shape_info"]);
                        int assistDamage = Parser.Json.ParseInt(item["assist_damage"]);
                        int assistDefBreak = Parser.Json.ParseInt(item["assist_def_break"]);
                        int assistHit = Parser.Json.ParseInt(item["assist_hit"]);
                        int assistReload = Parser.Json.ParseInt(item["assist_reload"]);
                        int endTime = Parser.Json.ParseInt(item["end_time"]);

                        //log.Debug("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                        //    buildSlot, piece, pieceItemId, chipId, colorId, gridId, shapeInfo, assistDamage, assistDefBreak, assistHit, assistReload, endTime);

                        DataAnalysisTemplate template = new DataAnalysisTemplate()
                        {
                            slot = buildSlot,
                            isPiece = piece,
                            pieceId = pieceItemId,
                            chipId = chipId,
                            colorId = colorId,
                            gridId = gridId,
                            assistDamage = assistDamage,
                            assistDefBreak = assistDefBreak,
                            assistHit = assistHit,
                            assistReload = assistReload,
                            endTime = endTime,
                        };

                        if (template.isPiece != 0 && template.canvasChipShape != null)
                        {
                            MainWindow.dashboardView.ClearChipShape(template.canvasChipShape);
                        }
                        template.TBRemainTime = TimeUtil.GetRemainHHMMSS(template.endTime);
                        template.TBEndTime = TimeUtil.GetDateTime(template.endTime, "MM-dd HH:mm");

                        dashboardView.Add(template);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "정보분석 진행상태 확인 중 에러 발생");
                }
            }

            // 탐색 진행상태
            log.Debug("탐색 확인...");
            if (index.ContainsKey("explore_info") && index["explore_info"] is JObject)
            {
                #region Packet Example
                /*
                   "explore_info": {
                        "list": [
                                    {
                                        "user_id": 621225,
                                        "start_time": 1565348535,
                                        "end_time": 1565355735,
                                        "cancel_time": 1565349483,
                                        "affairs": [],
                                        "gun_ids": [
                                            "2",
                                            "26",
                                            "75"
                                        ],
                                        "pet_ids": [],
                                        "area_id": "1",
                                        "target_id": "2",
                                        "draw_event_prize": 0,
                                        "item_id": 0
                                    },
                                    {
                                        "user_id": 621225,
                                        "start_time": 1565349516,
                                        "end_time": 1565356716,
                                        "cancel_time": 0,
                                        "affairs": [
                                            {
                                                "affair_id": "112",
                                                "affair_time": "1565350788"
                                            },
                                            {
                                                "affair_id": "64",
                                                "affair_time": "1565352452"
                                            },
                                            {
                                                "affair_id": "110",
                                                "affair_time": "1565355861"
                                            }
                                        ],
                                        "gun_ids": [
                                            "2",
                                            "26",
                                            "75"
                                        ],
                                        "pet_ids": [],
                                        "area_id": "4",
                                        "target_id": "62",
                                        "draw_event_prize": 1,
                                        "item_id": 0
                                    },
                                    {
                                        "user_id": 621225,
                                        "start_time": 1565357508,
                                        "end_time": 1565364708,
                                        "cancel_time": 0,
                                        "affairs": [],
                                        "gun_ids": [
                                            "2",
                                            "26",
                                            "75"
                                        ],
                                        "pet_ids": [],
                                        "area_id": "2",
                                        "target_id": "22",
                                        "draw_event_prize": 0,
                                        "item_id": 0
                                    }
                        ],
                        "next_time": "1565345213",
                        "ex_prize": {
                            "user_exp": 0,
                            "mp": 0,
                            "ammo": 0,
                            "mre": 0,
                            "part": 0,
                            "gem": 0,
                            "core": 0,
                            "gun_id": [],
                            "item_ids": "",
                            "gift": "",
                            "furniture": [],
                            "equip_ids": [],
                            "coins": "",
                            "skin": "",
                            "bp_pay": 0,
                            "fairy_ids": [],
                            "chip": []
                        },
                        "is_auto": 1,
                        "pets": [
                            "9534825",
                            "10217397",
                            "0"
                        ],
                        "items_taken": [],
                        "next_explore_time": 1565370677,
                        "end_time": 1565359877,
                        "explore_time_type": 2,
                        "mp": "115380",
                        "ammo": "111534",
                        "mre": "111547",
                        "part": "112238"
                    },
                 */
                #endregion
                try
                {
                    JObject explore_info = index["explore_info"].Value<JObject>();
                    // 보상?
                    if (explore_info.ContainsKey("ex_prize") && explore_info["ex_prize"] is JObject)
                    {

                    }
                    int next_time = Parser.Json.ParseInt(explore_info["next_time"]);                    // 화면이 바뀌는 시간? 중도보상 시간?
                    int next_explore_time = Parser.Json.ParseInt(explore_info["next_explore_time"]);     // 다음 탐색시간
                    //long next_time = Parser.Json.ParseLong(explore_info["next_time"]) * 1000;                    // 화면이 바뀌는 시간? 중도보상 시간?
                    //long next_explore_time = Parser.Json.ParseLong(explore_info["next_explore_time"]) * 1000;     // 다음 탐색시간
                    int is_auto = Parser.Json.ParseInt(explore_info["is_auto"]);                                 // 자동출발 시간
                    int explore_time_type = Parser.Json.ParseInt(explore_info["explore_time_type"]);              // 탐색시간 (1: 2시간, 2: 5시간, 3: 8시간)

                    int now_time = TimeUtil.GetCurrentSec();
                    //long now_time = Parser.Time.GetCurrentMs();
                    if (explore_info.ContainsKey("list") && explore_info["list"] is JArray)
                    {
                        JArray list = explore_info["list"].Value<JArray>();
                        foreach (JObject item in list)
                        {
                            int cancel_time = Parser.Json.ParseInt(item["cancel_time"]);       // 취소시간
                            if (cancel_time > 0)
                                continue;
                            int end_time = Parser.Json.ParseInt(item["end_time"]);             // 완료시간
                            if (end_time < now_time)
                                continue;

                            int area_id = Parser.Json.ParseInt(item["area_id"]);                        // 탐색지역
                            log.Debug("탐색지역 {0}", area_id);

                            string[] gunIdsString = Parser.Json.ParseStringArray(item["gun_ids"]);
                            List<int> gunIds = new List<int>();
                            foreach (string gunIdString in gunIdsString)
                            {
                                int gunId;
                                if (gunIdString.Contains("_"))
                                    gunId = Parser.String.ParseInt(gunIdString.Split('_')[0]);
                                else
                                    gunId = Parser.String.ParseInt(gunIdString);
                                //log.Info("gun_id {0}", gunId);
                                gunIds.Add(gunId);
                            }
                            int[] pet_ids = Parser.Json.ParseIntArray(item["pet_ids"]);
                            int target_id = Parser.Json.ParseInt(item["target_id"]);                    // 목표?
                            int item_id = Parser.Json.ParseInt(item["item_id"]);                        // 휴대물품

                            //long startTime = Parser.Json.ParseLong(item["start_time"]) * 1000;         // 시작 시간

                            dashboardView.Add(new ExploreTemplate()
                            {
                                areaId = area_id,
                                targetId = target_id,
                                gunIds = gunIds.ToArray(),
                                itemId = item_id,
                                endTime = end_time,
                                nextTime = next_time,
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "탐색현황 정보 가져오는 중 에러 발생");
                }
            }

            log.Debug("정보임무 확인 중");
            try
            {
                if (index.ContainsKey("squad_data_daily") && index["squad_data_daily"] is JArray)
                {
                    List<QuestTemplate> quests = new List<QuestTemplate>();
                    UserData.Quest.isNotify = false;

                    questView.ClearResearch();

                    JArray items = index["squad_data_daily"].Value<JArray>();
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
            catch (Exception ex)
            {
                log.Error(ex, "정보임무 확인 중 에러 발생");
            }

            // 타이머 다시 시작
            MainWindow.view.forceStop = false;

            // 프록시 메뉴인 경우 알림 메뉴로 이동
            if (MainWindow.view.CurrentMenu == Menus.PROXY)
            {
                MainWindow.view.CurrentMenu = Menus.DASHBOARD;
            }

            MainWindow.view.SetIconEnable(Menus.DASHBOARD, true);
            MainWindow.view.SetIconEnable(Menus.ECHELON, true);

            MainWindow.view.SetIconNotify(Menus.DASHBOARD, false);
            MainWindow.view.SetIconNotify(Menus.ECHELON, false);
            MainWindow.view.SetIconNotify(Menus.QUEST, false);

            ///
            /// 파일 저장
            /// 

            // 국지전 웨이브 저장
            if (Config.Setting.exportTheaterExercise)
            {
                #region Packet Example
                /*
                    "theater_exercise_info":{
                        "theater_squads_use_count":"2-3,1-3,3-2",
                        "battle_enemy_no":5,
                        "battle_team":1,
                        "battle_fairy":0,
                        "battle_squads":"1,2,3",
                        "last_battle_finish_time":1562687724,
                        "user_id":343650,
                        "theater_area_id":436,
                        "enemy_teams":"3,4,0,4,0,3,4,2,0,2",
                        "theater_teams":"1,2,3,4,5,6",
                        "theater_squads":"1,2,3",
                        "theater_fairy_use_count":0
                    }
                    */
                #endregion

                log.Debug("국지전 정보 확인...");
                if (index.ContainsKey("theater_exercise_info") && index["theater_exercise_info"] is JObject)
                {
                    try
                    {
                        JObject theater_exercise_info = index["theater_exercise_info"].Value<JObject>();
                        int theater_area_id = Parser.Json.ParseInt(theater_exercise_info["theater_area_id"]);
                        string tempEnemyTeams = Parser.Json.ParseString(theater_exercise_info["enemy_teams"]);
                        int[] enemy_teams = tempEnemyTeams.Split(',').Select(Int32.Parse).ToArray();
                        if (theater_area_id > 0 && enemy_teams.Length > 0)
                        {
                            log.Debug("국지전 웨이브 정보 저장 중...");
                            CsvExporter.ExportTheaterExerciseInfo(theater_area_id, enemy_teams);
                            log.Debug("국지전 웨이브 정보 파일로 저장 (theater_exercise.csv)");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "국지전 웨이브 정보 저장 에러");
                    }
                }
                else
                {
                    log.Debug("국지전 웨이브 정보 없음");
                }
            }

            // 아이템정보 저장
            if (Config.Setting.exportItemInfo)
            {
                log.Debug("아이템 정보 저장 중...");
                try
                {
                    Util.CsvExporter.ExportItemInfo(index);
                    log.Debug("아이템 정보 파일로 저장 (item_info.csv)");
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            // 인형정보 저장
            if (Config.Setting.exportDollInfo)
            {
                log.Debug("인형 정보 저장 중...");
                try
                {
                    Util.CsvExporter.ExportDollInfo(UserData.Doll.GetAll());
                    log.Debug("인형 정보 파일로 저장 (doll_info.csv)");
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }

            // 장비정보 저장
            if (Config.Setting.exportEquipInfo)
            {
                log.Debug("장비 정보 저장 중...");
                try
                {
                    Util.CsvExporter.ExportEquipInfo(UserData.Equip.GetAll());
                    log.Debug("장비 정보 파일로 저장 (equip_info.csv)");
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }

            // 요정정보 저장
            if (Config.Setting.exportFairyInfo)
            {
                log.Debug("요정 정보 저장 중...");
                try
                {
                    Util.CsvExporter.ExportFairyInfo(UserData.Fairy.GetAll());
                    log.Debug("요정 정보 파일로 저장 (fairy_info.csv)");
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }

            // 전투 테스터 프리셋 저장
            if (Config.Setting.exportBattleTesterPreset)
            {
                log.Debug("전투 테스터 프리셋 저장 중...");
                try
                {
                    BattleTesterExporter.ExportEchelonInfo(index);
                    BattleTesterExporter.ExportSquadInfo(index);
                    BattleTesterExporter.ExportChipInfo(index);
                    BattleTesterExporter.ExportEquipInfo(index);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "전투 테스터 프리셋 저장 에러");
                }
            }

            // 사용자 정보 저장
            if (Config.Setting.exportUserInfo)
            {
                try
                {
                    // 필요없는 정보 삭제
                    if (index.ContainsKey("spot_act_info"))
                        index.Remove("spot_act_info");
                    if (index.ContainsKey("mission_act_info"))
                        index.Remove("mission_act_info");
                    if (index.ContainsKey("theater_exercise_info"))
                        index.Remove("theater_exercise_info");

                    string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                    Directory.CreateDirectory(path);
                    string filename = string.Format(path + "\\{0}_{1}_user_info.json", UserData.name, UserData.uid);
                    File.WriteAllText(@filename, index.ToString(Formatting.None), Encoding.Default);
                    filename = string.Format(path + "\\{0}_{1}_user_info.txt", UserData.name, UserData.uid);
                    File.WriteAllText(@filename, body, Encoding.Default);
                    log.Debug("전체 사용자 정보 파일로 저장 (user_info.json)");
                }
                catch (Exception ex)
                {
                    log.Error(ex, "사용자 정보 저장 에러");
                }
            }

            log.Debug("전체 정보 가져오기 종료");

            index = null;

            // 연결 알림
            Notifier.Manager.notifyQueue.Enqueue(new Message()
            {
                send = MessageSend.All,
                type = MessageType.connect,
                gunId = UserData.adjutantDoll,
                skinId = UserData.adjutantDollSkin,
                subject = LanguageResources.Instance["MESSAGE_CONNECT_SUBJECT"],
                content = LanguageResources.Instance["MESSAGE_CONNECT_CONTENT"],
            });

            GC.Collect();

            return;
        }

        /// <summary>
        /// 임무화면
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void GetQuest(string request_string, string response_string)
        {
            #region Packet Example
            // request
            // null
            // response
            /*
                "daily": {
                    "fix": 31,
                    "borrow_friend_team": 20,
                    "eat": 4,
                    "mission": 9,
                    "operation": 38,
                    "from_friend_build_coin": 10,
                    "coin_mission": 4,
                    "squad_data_analyse": 10,
                    "develop_gun": 4,
                    "eat_equip": 5,
                    "develop_equip": 4,
                    "id": "34577",
                    "user_id": "343650",
                    "end_time": "1565276400",
                    "upgrade": "0",
                    "win_robot": "0",
                    "win_person": "0",
                    "win_boss": "0",
                    "win_armorrobot": "0",
                    "win_armorperson": "0"
                },
                "weekly": {
                    "id": "34577",
                    "user_id": "343650",
                    "end_time": 1565535600,
                    "fix": "101",
                    "win_robot": "104",
                    "win_person": "206",
                    "win_boss": "7",
                    "win_armorrobot": "24",
                    "win_armorperson": "196",
                    "operation": "50",
                    "s_win": "100",
                    "eat": "20",
                    "develop_gun": "20",
                    "upgrade": "5",
                    "coin_mission": "19",
                    "develop_equip": "16",
                    "special_develop_gun": 0,
                    "adjust_equip": 0,
                    "eat_equip": "20",
                    "special_develop_equip": "3",
                    "adjust_fairy": 0,
                    "eat_fairy": "1",
                    "squad_data_analyse": "20",
                    "squad_eat_chip": "24"
                },
            */
            #endregion
            try
            {
                log.Debug("임무화면 진입");
                JObject response = Parser.Json.ParseJObject(response_string);
                if (response != null)
                {
                    if (!response.ContainsKey("daily"))
                        return;

                    UserData.Quest.isOpenQuestMenu = true;
                    UserData.Quest.isNotify = false;

                    // 일일 임무
                    if (response["daily"] != null && response["daily"] is JObject)
                    {
                        //MainWindow.questView.Init();

                        JObject daily = response["daily"].Value<JObject>();
                        UserData.Quest.Daily.produceDoll = Parser.Json.ParseInt(daily["develop_gun"]);                      // 인형제조
                        UserData.Quest.Daily.produceEquip = Parser.Json.ParseInt(daily["develop_equip"]);                   // 장비제조
                        UserData.Quest.Daily.fixGun = Parser.Json.ParseInt(daily["fix"]);                                   // 수복
                        UserData.Quest.Daily.eatGun = Parser.Json.ParseInt(daily["eat"]);                                   // 인형강화
                        UserData.Quest.Daily.eatEquip = Parser.Json.ParseInt(daily["eat_equip"]);                           // 장비강화
                        UserData.Quest.Daily.mission = Parser.Json.ParseInt(daily["mission"]);                              // 전역승리
                        UserData.Quest.Daily.operation = Parser.Json.ParseInt(daily["operation"]);                          // 군수지원
                        UserData.Quest.Daily.getBattery = Parser.Json.ParseInt(daily["from_friend_build_coin"]);            // 전지획득
                        UserData.Quest.Daily.combatSim = Parser.Json.ParseInt(daily["coin_mission"]);                       // 모의작전
                        UserData.Quest.Daily.dataAnalysis = Parser.Json.ParseInt(daily["squad_data_analyse"]);              // 정보분석
                        UserData.Quest.Daily.callReinforce = Parser.Json.ParseInt(daily["borrow_friend_team"]);             // 친구제대 소환
                        UserData.borrowTeamToday = Parser.Json.ParseInt(daily["borrow_friend_team"]);                        // 친구제대 소환
                    }
                    // 주간 임무
                    if (response["weekly"] != null && response["weekly"] is JObject)
                    {
                        JObject weekly = response["weekly"].Value<JObject>();
                        UserData.Quest.Weekly.fixGun = Parser.Json.ParseInt(weekly["fix"]);                                // 수복
                        UserData.Quest.Weekly.killMech = Parser.Json.ParseInt(weekly["win_robot"]);                        // 철혈기계
                        UserData.Quest.Weekly.killDoll = Parser.Json.ParseInt(weekly["win_person"]);                       // 철혈인형
                        UserData.Quest.Weekly.killBoss = Parser.Json.ParseInt(weekly["win_boss"]);                         // 보스인형
                        UserData.Quest.Weekly.killArmorMech = Parser.Json.ParseInt(weekly["win_armorrobot"]);              // 장갑기계
                        UserData.Quest.Weekly.killArmorDoll = Parser.Json.ParseInt(weekly["win_armorperson"]);             // 장갑인형
                        UserData.Quest.Weekly.operation = Parser.Json.ParseInt(weekly["operation"]);                       // 군수지원
                        UserData.Quest.Weekly.sBattle = Parser.Json.ParseInt(weekly["s_win"]);                             // S랭크 승리
                        UserData.Quest.Weekly.eatGun = Parser.Json.ParseInt(weekly["eat"]);                                // 인형강화
                        UserData.Quest.Weekly.produceDoll = Parser.Json.ParseInt(weekly["develop_gun"]);                   // 인형제조
                        UserData.Quest.Weekly.skillTrain = Parser.Json.ParseInt(weekly["upgrade"]);                        // 스킬훈련
                        UserData.Quest.Weekly.combatSim = Parser.Json.ParseInt(weekly["coin_mission"]);                    // 모의작전
                        UserData.Quest.Weekly.produceEquip = Parser.Json.ParseInt(weekly["develop_equip"]);                // 장비제조
                        UserData.Quest.Weekly.produceHeavyDoll = Parser.Json.ParseInt(weekly["special_develop_gun"]);      // 인형중제조
                        UserData.Quest.Weekly.adjustEquip = Parser.Json.ParseInt(weekly["adjust_equip"]);                  // 장비교정
                        UserData.Quest.Weekly.eatEquip = Parser.Json.ParseInt(weekly["eat_equip"]);                        // 장비강화
                        UserData.Quest.Weekly.produceHeavyEquip = Parser.Json.ParseInt(weekly["special_develop_equip"]);   // 장비중제조
                        UserData.Quest.Weekly.adjustFairy = Parser.Json.ParseInt(weekly["adjust_fairy"]);                  // 요정교정
                        UserData.Quest.Weekly.eatFairy = Parser.Json.ParseInt(weekly["eat_fairy"]);                        // 요정강화
                        UserData.Quest.Weekly.dataAnalysis = Parser.Json.ParseInt(weekly["squad_data_analyse"]);           // 정보분석
                        UserData.Quest.Weekly.eatChip = Parser.Json.ParseInt(weekly["squad_eat_chip"]);                    // 칩셋강화
                    }

                    UserData.Quest.isNotify = true;
                    UserData.Quest.Init();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to get Index/Quest");
            }
        }
    }
}
