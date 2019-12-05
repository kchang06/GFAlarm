using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Notifier;
using GFAlarm.Util;
using LocalizationResources;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using static GFAlarm.MainWindow;

namespace GFAlarm.Transaction.PacketProcess
{
    public class Mission
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 작전 시작
        /// ("Mission/startMission")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartMission(string request_string, string response_string)
        {
            #region Packet Example
            // request Mission/startMission?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "mission_id": 109,
                    "spots": [
                        {
                            "spot_id": 9016,
                            "team_id": 1
                        },
                        {
                            "spot_id": 9018,
                            "team_id": 2
                        },
                        {
                            "spot_id": 9017,
                            "team_id": 3
                        }
                    ],
                    "squad_spots": [],
                    "ally_id": 1564641889
                }
                */
            // response (야간)
            /*
                {
                    "night_spots": [
                        {
                            "spot_id": "7067",
                            "belong": "2",
                            "seed": 7212
                        },
                        {
                            "spot_id": "7068",
                            "belong": "3",
                            "seed": 9802
                        },
                        {
                            "spot_id": "7070",
                            "belong": "1",
                            "seed": 1964
                        },
                        ...
                    ],
                    "night_enemy": [
                        {
                            "enemy_team_id": "1987",
                            "enemy_hp_percent": "1",
                            "enemy_instance_id": "8",
                            "enemy_birth_turn": "1",
                            "enemy_ai": "0",
                            "enemy_ai_para": 0,
                            "from_spot_id": "7076",
                            "to_spot_id": "7076",
                            "boss_hp": "0",
                            "hostage_id": "0",
                            "hostage_hp": "0",
                            "ally_instance_ids": []
                        },
                        {
                            "enemy_team_id": "1987",
                            "enemy_hp_percent": "1",
                            "enemy_instance_id": "11",
                            "enemy_birth_turn": "1",
                            "enemy_ai": "0",
                            "enemy_ai_para": 0,
                            "from_spot_id": "7079",
                            "to_spot_id": "7079",
                            "boss_hp": "0",
                            "hostage_id": "0",
                            "hostage_hp": "0",
                            "ally_instance_ids": []
                        }
                    ],
                    "night_ally": [],
                    "ally_instance_info": [],
                    "ally_battle": [],
                    "building_info": [],
                    "squad_info": [],
                    "ap": 7,
                    "fairy_skill_return": [],
                    "fairy_skill_perform": [],
                    "fairy_skill_on_spot": [],
                    "fairy_skill_on_team": [],
                    "fairy_skill_on_enemy": [],
                    "fairy_skill_on_squad": []
                }
            */
            #endregion
            try
            {
                log.Debug("작전 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                JObject response = Parser.Json.ParseJObject(response_string);
                if (request != null && response != null)
                {
                    int missionId = Parser.Json.ParseInt(request["mission_id"]);

                    // 현재 작전 중인 전역
                    UserData.CurrentMission.id = missionId;
                    UserData.CurrentMission.turnCount = 1;

                    log.Debug("전역번호 {0}", UserData.CurrentMission.id);
                    log.Debug("전역명 {0}", UserData.CurrentMission.location);

                    // 모의작전
                    int use_bp = GameData.Mission.GetUsedAp(missionId);   // 모의작전점수 차감
                    if (use_bp > 0)
                    {
                        UserData.CombatSimulation.UseBp(use_bp);

                        // 임무 갱신
                        UserData.Quest.Daily.combatSim += 1;
                        UserData.Quest.Weekly.combatSim += 1;
                    }
                    // 일반작전
                    // 전역횟수 업데이트
                    MissionWithUserInfo mission = UserData.Mission.Get(missionId);
                    if (mission != null)
                    {
                        mission.counter++; // 전역횟수++
                        UserData.Mission.Set(mission);
                        log.Debug("전역횟수 {0}", mission.counter);
                    }

                    // 제대이동 횟수 초기화
                    UserData.CurrentMission.moveCount = 0;

                    // 아군 제대 위치
                    if (request.ContainsKey("spots"))
                    {
                        // 제대배치
                        JArray spots = Parser.Json.ParseJArray(Parser.Json.ParseString(request["spots"]));
                        foreach (var spot in spots)
                        {
                            int spotId = Parser.Json.ParseInt(spot["spot_id"]);
                            int teamId = Parser.Json.ParseInt(spot["team_id"]);

                            UserData.CurrentMission.ReinforceTeam(teamId, spotId);
                        }
                    }

                    PacketProcess.Mission.MoveTeam(request, response);
                    PacketProcess.Mission.SpotBelongChange(request, response);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "작전 시작 처리 실패");
            }
        }

        /// <summary>
        /// 작전 중지
        /// ("Mission/abortMission")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void AbortMission(string request_string, string response_string)
        {
            #region Packet Example
            // request Mission/abortMission?uid={0}&signcode={1}&req_id={2}
            // null
            // response
            /*
                {
                    "mission_lose_result": {
                        "turn": "1",
                        "enemydie_num": "5",
                        "enemydie_num_killbyfriend": "0",
                        "gundie_num": "2"
                    }
                }
                */
            #endregion
            try
            {
                log.Debug("작전 중지");
                foreach (KeyValuePair<int, int> item in UserData.CurrentMission.teamSpots)
                {
                    // 제대 탄식 소실
                    UserData.Doll.LoseAmmoMre(item.Key);
                }
                UserData.CurrentMission.Clear();
            }
            catch (Exception ex)
            {
                log.Error(ex, "작전 중지 처리 실패");
            }
        }

        /// <summary>
        /// 제대 소환
        /// ("Mission/reinforceTeam")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void ReinforceTeam(string request_string, string response_string)
        {
            #region Packet Example
            // request Mission/reinforceTeam?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"spot_id":12,"team_id":2} 
            */
            // response Mission/reinforceTeam
            /*
                {
                    "ap": 2,
                    "fairy_skill_return": [],
                    "fairy_skill_perform": [],
                    "fairy_skill_on_spot": [],
                    "fairy_skill_on_team": [],
                    "fairy_skill_on_enemy": [],
                    "fairy_skill_on_squad": []
                }
            */
            #endregion
            try
            {
                log.Debug("제대 소환");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int spotId = Parser.Json.ParseInt(request["spot_id"]);
                    int teamId = Parser.Json.ParseInt(request["team_id"]);

                    UserData.CurrentMission.ReinforceTeam(teamId, spotId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "제대 소환 처리 실패");
            }
        }

        /// <summary>
        /// 제대 보급
        /// ("Mission/supplyTeam")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void SupplyTeam(string request_string, string response_string)
        {
            #region Packet Example
            // request Mission/supplyTeam?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"mission_id":10,"team_id":4,"spot_id":133}
            */
            // response Mission/supplyTeam  
            // 1
            #endregion

            try
            {
                log.Debug("제대 보급");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int teamId = Parser.Json.ParseInt(request["team_id"]);
                    UserData.Doll.SupplyAmmoMre(teamId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "제대 보급 처리 실패");
            }
        }

        /// <summary>
        /// 방어훈련 시작
        /// ("Mission/startTrial")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void StartTrial(string request_string, string response_string)
        {
            #region Packet Example
            // request Mission/startTrial?uid={0}&outdatacode={1}&req_id={2}
            /*
                {"team_ids":"6","battle_team":6}
                */
            // response Mission/startTrial
            /*
                {"trial_id":101}
            */
            #endregion
            try
            {
                log.Debug("방어훈련 시작");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    // 모의작전점수 차감
                    UserData.CombatSimulation.UseBp(5);

                    // 임무 갱신
                    UserData.Quest.Daily.combatSim += 1;
                    UserData.Quest.Weekly.combatSim += 1;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "방어훈련 시작 처리 실패");
            }
        }

        /// <summary>
        /// 자료추출 완료
        /// ("Mission/coinBattleFinish")
        /// </summary>
        /// <param name="request_string"></param>
        /// <param name="response_string"></param>
        public static void FinishCoinBattle(string request_string, string response_string)
        {
            #region Packet Example
            // request Mission/coinBattleFinish?uid={0}&outdatacode={1}&req_id={2}
            /*
                {
                    "mission_id": 1302,
                    "boss_hp": 0,
                    "duration": 1.33,
                    "battle_time": {}
                }
            */
            // response
            /*
                {"coin_num":"98","coin_type":"2"} 
            */
            #endregion
            try
            {
                log.Debug("자료추출 완료");
                JObject request = Parser.Json.ParseJObject(request_string);
                if (request != null)
                {
                    int missionId = Parser.Json.ParseInt(request["mission_id"]);

                    // 현재 작전 중인 전역
                    UserData.CurrentMission.id = missionId;
                    // 모의작전점수 차감
                    int use_bp = GameData.Mission.GetUsedAp(missionId);
                    if (use_bp > 0) // 모의작전
                    {
                        UserData.CombatSimulation.UseBp(use_bp);
                    }

                    // 임무 갱신
                    UserData.Quest.Daily.combatSim += 1;
                    UserData.Quest.Weekly.combatSim += 1;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "자료추출 완료 처리 실패");
            }
        }

        /// <summary>
        /// 제대 이동 패킷 처리
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void MoveTeam(JObject request, JObject response)
        {
            if (response == null)
                response = new JObject();

            // 자원 증감 반영
            if (response.ContainsKey("mp"))
                UserData.mp += Parser.Json.ParseInt(response["mp"]);
            if (response.ContainsKey("ammo"))
                UserData.ammo += Parser.Json.ParseInt(response["ammo"]);
            if (response.ContainsKey("loseammo"))
                UserData.ammo -= Parser.Json.ParseInt(response["loseammo"]);
            if (response.ContainsKey("mre"))
                UserData.mre += Parser.Json.ParseInt(response["mre"]);
            if (response.ContainsKey("losemre"))
                UserData.mre -= Parser.Json.ParseInt(response["losemre"]);
            if (response.ContainsKey("part"))
                UserData.part += Parser.Json.ParseInt(response["part"]);

            // 거점 정보 반영
            if (response.ContainsKey("spot_act_info") && response["spot_act_info"] is JArray)
            {
                JArray spotActInfo = response["spot_act_info"].Value<JArray>();

                /// 거점 점령 상태
                UserData.CurrentMission.SetBelongSpot(spotActInfo);

                /// 적 제대 위치
                foreach (JObject item in spotActInfo)
                {
                    int spotId = Parser.Json.ParseInt(item["spot_id"]);
                    int enemyTeamId = Parser.Json.ParseInt(item["enemy_team_id"]);
                    if (enemyTeamId > 0)
                    {
                        UserData.CurrentMission.MoveEnemyTeam(enemyTeamId, 0, spotId);
                    }
                }
            }

            // 적 제대 스폰
            if (response.ContainsKey("grow_enemy") && response["grow_enemy"] is JArray)
            {
                JArray growEnemy = response["grow_enemy"].Value<JArray>();
                UserData.CurrentMission.SpawnEnemyTeam(growEnemy);
            }

            // 적 제대 위치 (주간)
            if (response.ContainsKey("enemy_move") && response["enemy_move"] is JArray)
            {
                JArray enemyMove = response["enemy_move"].Value<JArray>();
                UserData.CurrentMission.MoveEnemyTeam(enemyMove);
            }

            // 적 제대 위치 (야간)
            if (response.ContainsKey("night_enemy") && response["night_enemy"] is JArray)
            {
                JArray nightEnemy = response["night_enemy"].Value<JArray>();
                UserData.CurrentMission.MoveEnemyTeam(nightEnemy);
            }

            // 섬멸된 적 제대 위치
            if (response.ContainsKey("died_enemy") && response["died_enemy"] is JObject)
            {
                Dictionary<string, string> items = Parser.Json.ParseItems(response["died_enemy"].ToString());
                foreach (KeyValuePair<string, string> item in items)
                {
                    int teamId = Parser.String.ParseInt(item.Key);
                    // 적 제대 퇴각
                    if (UserData.CurrentMission.enemySpots.ContainsKey(teamId))
                    {
                        log.Debug("적 {0} 제대 처치됨", teamId);
                        UserData.CurrentMission.enemySpots.Remove(teamId);
                    }
                    else
                    {
                        log.Debug("적 {0} 제대 처치됨 - 처리 없음", teamId);
                    }
                }
            }
        }

        /// <summary>
        /// 전투 종료 패킷 처리
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void BattleFinish(JObject request, JObject response)
        {
            if (request == null)
                request = new JObject();
            if (response == null)
                response = new JObject();

            int spotId = Parser.Json.ParseInt(request["spot_id"]);
            int teamId = UserData.CurrentMission.GetTeamId(spotId);
            bool ifEnemyDie = Parser.Json.ParseBool(request["if_enemy_die"]);
            int battleRank = Parser.Json.ParseInt(response["battle_rank"]);
            // 적 제대 처치
            if (ifEnemyDie)
            {
                // 모의작전이 아닌 경우에만
                if (GameData.Mission.GetUsedAp(UserData.CurrentMission.id) == 0)
                {
                    // 제대 탄식 소비
                    UserData.Doll.SpendAmmoMre(teamId);
                    // 처치된 적 확인
                    long enemyTeamId = UserData.CurrentMission.GetEnemyTeamId(spotId);
                    if (enemyTeamId > 0)
                    {
                        log.Debug("적 제대번호 - {0}", enemyTeamId);
                        string[] members = GameData.Enemy.GetEnemyCodes(enemyTeamId);
                        if (members != null)
                        {
                            Dictionary<string, int> enemyMemberSet = new Dictionary<string, int>();
                            foreach (string member in members)
                            {
                                //log.Debug("적 처치 - {0}", member);
                                if (enemyMemberSet.ContainsKey(member))
                                    enemyMemberSet[member]++;
                                else
                                    enemyMemberSet.Add(member, 1);
                                UserData.Quest.Research.kill++;
                                switch (member)
                                {
                                    // 철혈기계
                                    case "Drone":
                                    case "Prowler":
                                    case "Prowler_SWAP":
                                    case "Scouts":
                                    case "Dinergate":
                                    case "Jaguar":
                                    case "Golyat":
                                    case "GolyatPlus":
                                        UserData.Quest.Weekly.killMech++;
                                        UserData.Quest.Research.killMech++;
                                        break;
                                    // 철혈인형
                                    case "Ripper":
                                    case "Vespid":
                                    case "Jaeger":
                                    case "Jaeger_SWAP":
                                    case "Guard":
                                    case "Guard_SWAP":
                                    case "Dragoon":
                                    case "Dragoon_SWAP":
                                    case "Striker":
                                    case "Striker_SWAP":
                                    case "Brute":
                                    case "unknown":
                                        UserData.Quest.Weekly.killDoll++;
                                        UserData.Quest.Research.killDoll++;
                                        break;
                                    // 철혈장갑기계
                                    case "Nemeum":
                                    case "Tarantula":
                                    case "Manticore":
                                        UserData.Quest.Weekly.killArmorMech++;
                                        UserData.Quest.Research.killArmorMech++;
                                        break;
                                    // 철혈장갑인형
                                    case "Aegis":
                                    case "Aegis_GA":
                                        UserData.Quest.Weekly.killArmorDoll++;
                                        UserData.Quest.Research.killArmorDoll++;
                                        break;
                                    // 철혈보스
                                    case "Scarecrow":
                                    case "Excutioner":
                                    case "ExcutionerElite":
                                    case "Hunter":
                                    case "HunterElite":
                                    case "Intruder":
                                    case "Destroyer":
                                    case "DestroyerPlus":
                                    case "BossGager":
                                    case "Justice":
                                    case "Weaver":
                                    case "WeaverElite":
                                    case "Alchemist":
                                    case "BossArchitect":
                                    case "Dreamer":
                                    case "Agent":
                                    case "M16A1_Boss":
                                        UserData.Quest.Weekly.killBoss++;
                                        UserData.Quest.Research.killBoss++;
                                        break;
                                }
                            }
                            string memberstring = "";
                            foreach (KeyValuePair<string, int> member in enemyMemberSet)
                                memberstring += string.Format("{0}(x{1}), ", member.Key, member.Value);
                            log.Debug("적 처치 - {0}", memberstring);
                        }

                        UserData.CurrentMission.WithdrawEnemyTeam(spotId);
                    }
                    else
                    {
                        log.Warn("적 제대 찾을 수 없음 - 위치 {0}", spotId);
                    }
                }
            }
            // 전투 실패
            else if (ifEnemyDie == false && battleRank == 2)
            {
                // 모의작전이 아닌 경우에만
                if (GameData.Mission.GetUsedAp(UserData.CurrentMission.id) == 0)
                {
                    // 제대 탄식 소실
                    UserData.Doll.LoseAmmoMre(teamId);
                }
            }
            // 전투 S 랭크
            if (battleRank == 5)
            {
                UserData.Quest.Weekly.sBattle += 1;
            }
            // 인형 체력 반영
            if (request.ContainsKey("guns") && request["guns"] is JArray)
            {
                JArray items = request["guns"].Value<JArray>();
                foreach (JObject item in items)
                {
                    long gunWithUserId = Parser.Json.ParseLong(item["id"]);
                    int life = Parser.Json.ParseInt(item["life"]);

                    //log.Debug("인형 {0} 체력 {1}", gunWithUserId, life);

                    UserData.Doll.SetHp(gunWithUserId, life);
                }
            }
            // 인형 경험치 획득
            if (response.ContainsKey("gun_exp") && response["gun_exp"] is JArray)
            {
                JArray items = response["gun_exp"].Value<JArray>();
                foreach (var item in items)
                {
                    long gunWithUserId = Parser.Json.ParseLong(item["gun_with_user_id"]);
                    int exp = Parser.Json.ParseInt(item["exp"]);

                    //log.Debug("인형 {0} 경험치 {1} 획득", gunWithUserId, exp);

                    UserData.Doll.GainExp(gunWithUserId, exp);
                }
            }
            // 요정 경험치 획득
            if (response.ContainsKey("fairy_exp"))
            {
                int fairyExp = Parser.Json.ParseInt(response["fairy_exp"]);
                if (teamId > 0)
                {
                    long fairyWithUserId = 0;
                    echelonView.FindId(teamId, 6, ref fairyWithUserId);

                    //log.Debug("요정 {0} 경험치 {1} 획득", fairyWithUserId, fairyExp);

                    UserData.Fairy.GainExp(fairyWithUserId, fairyExp);
                }
            }
            // 중장비 경험치 획득
            if (response.ContainsKey("squad_exp") && response["squad_exp"] is JArray)
            {
                #region Packet Example
                /*
                    "squad_exp": [
                        {
                            "squad": 1,
                            "exp": 12510500,
                            "life": "201"
                        }
                    ],
                 */
                #endregion
                JArray items = response["squad_exp"].Value<JArray>();
                foreach (JObject item in items)
                {
                    int squad = Parser.Json.ParseInt(item["squad"]); // 인스턴스 아이디
                    long exp = Parser.Json.ParseLong(item["exp"]);
                    int life = Parser.Json.ParseInt(item["life"]);


                }
            }
            // 자유경험치 획득
            if (response.ContainsKey("free_exp") && Util.Common.IsValidTeamId(teamId))
            {
                int freeExp = Parser.Json.ParseInt(response["free_exp"]);
                UserData.GlobalExp.exp += freeExp;
                if (UserData.GlobalExp.maxExp <= UserData.GlobalExp.exp)
                {
                    UserData.GlobalExp.exp = UserData.GlobalExp.maxExp;
                    if (Config.Alarm.notifyMaxGlobalExp && !UserData.GlobalExp.notified)
                    {
                        UserData.GlobalExp.notified = true;
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.All,
                            type = MessageType.reach_max_global_exp,
                            subject = LanguageResources.Instance["MESSAGE_MAX_SURPLUS_EXP_SUBJECT"],
                            content = LanguageResources.Instance["MESSAGE_MAX_SURPLUS_EXP_CONTENT"],
                        });
                    }
                }
                //MainWindow.view.SetGlobalExp(UserData.GlobalExp.exp, UserData.GlobalExp.maxExp);
            }
            // 인형 획득
            if (response.ContainsKey("battle_get_gun"))
            {
                if (response["battle_get_gun"] is JArray)
                {
                    JArray items = response["battle_get_gun"].Value<JArray>();
                    foreach (var item in items)
                    {
                        long gunWithUserId = Parser.Json.ParseLong(item["gun_with_user_id"]);
                        int gunId = Parser.Json.ParseInt(item["gun_id"]);

                        //log.Debug("인형 {0} 획득", gunId);

                        UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId), true);
                    }
                }
                else if (response["battle_get_gun"] is JObject)
                {
                    long gunWithUserId = Parser.Json.ParseLong(response["battle_get_gun"]["gun_with_user_id"]);
                    int gunId = Parser.Json.ParseInt(response["battle_get_gun"]["gun_id"]);

                    //log.Debug("인형 {0} 획득", gunId);

                    UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId), true);
                }
            }
            // 구출 이벤트 인형 획득
            if (response.ContainsKey("battle_get_prize") && response["battle_get_prize"] is JArray)
            {
                JArray battleGetPrize = response["battle_get_prize"].Value<JArray>();
                foreach (JObject item in battleGetPrize)
                {
                    long gunWithUserId = Parser.Json.ParseLong(item["gun_with_user_id"]);
                    if (item.ContainsKey("real_prize_info") && item["real_prize_info"] is JObject)
                    {
                        JObject realPrizeInfo = item["real_prize_info"].Value<JObject>();
                        int gunId = Parser.Json.ParseInt(realPrizeInfo["gun_id"]);

                        UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId), true);
                    }
                }
            }
            // 장비 획득
            if (response.ContainsKey("battle_get_equip"))
            {
                if (response["battle_get_equip"] is JArray)
                {
                    JArray items = response["battle_get_equip"].Value<JArray>();
                    foreach (JObject item in items)
                    {
                        long equipWithUserId = Parser.Json.ParseLong(item["id"]);
                        int equipId = Parser.Json.ParseInt(item["equip_id"]);

                        //log.Debug("장비 {0} 획득", equipId);

                        UserData.Equip.Add(equipWithUserId, new EquipWithUserInfo(equipWithUserId, equipId), true);
                    }
                }
                else if (response["battle_get_equip"] is JObject)
                {
                    long equipWithUserId = Parser.Json.ParseLong(response, "battle_get_equip", "id");
                    int equipId = Parser.Json.ParseInt(response, "battle_get_equip", "equip_id");

                    //log.Debug("장비 {0} 획득", equipId);

                    UserData.Equip.Add(equipWithUserId, new EquipWithUserInfo(equipWithUserId, equipId), true);
                }
            }
            // 전역 승리
            if (response.ContainsKey("mission_win_result") && response["mission_win_result"] is JObject)
            {
                JObject packet = response["mission_win_result"].Value<JObject>();
                // 경험모의작전 - 인형 경험치 반영
                if (packet.ContainsKey("after_duplicate1_gun_exp") && packet["after_duplicate1_gun_exp"] is JArray)
                {
                    JArray items = packet["after_duplicate1_gun_exp"].Value<JArray>();
                    foreach (JObject item in items)
                    {
                        long gunWithUserId = Parser.Json.ParseLong(item["gwu_id"]);
                        int exp = Parser.Json.ParseInt(item["exp"]);

                        UserData.Doll.SetExp(gunWithUserId, exp);
                    }
                }
                // 경험모의작전 - 자유경험치 반영
                if (packet.ContainsKey("free_exp"))
                {
                    int freeExp = Parser.Json.ParseInt(packet["free_exp"]);
                    UserData.GlobalExp.exp += freeExp;
                    if (UserData.GlobalExp.maxExp <= UserData.GlobalExp.exp)
                    {
                        UserData.GlobalExp.exp = UserData.GlobalExp.maxExp;
                        if (Config.Alarm.notifyMaxGlobalExp && !UserData.GlobalExp.notified)
                        {
                            UserData.GlobalExp.notified = true;
                            Notifier.Manager.notifyQueue.Enqueue(new Message()
                            {
                                send = MessageSend.All,
                                type = MessageType.reach_max_global_exp,
                                subject = LanguageResources.Instance["MESSAGE_MAX_SURPLUS_EXP_SUBJECT"],
                                content = LanguageResources.Instance["MESSAGE_MAX_SURPLUS_EXP_CONTENT"],
                            });
                        }
                    }
                    //MainWindow.view.SetGlobalExp(UserData.GlobalExp.exp, UserData.GlobalExp.maxExp);
                }
                // 전역 승리 랭크
                if (packet.ContainsKey("rank"))
                {
                    log.Debug("전역 승리");

                    // 전역 승리 알림
                    if (Config.Alarm.notifyMissionSuccess)
                    {
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.All,
                            type = MessageType.mission_win,
                            delay = 5000,
                            gunId = UserData.adjutantDoll,
                            skinId = UserData.adjutantDollSkin,
                            subject = LanguageResources.Instance["MESSAGE_MISSION_WIN_SUBJECT"],
                            content = LanguageResources.Instance["MESSAGE_MISSION_WIN_CONTENT"],
                        });
                    }
                    // 인형 획득
                    if (packet.ContainsKey("reward_gun"))
                    {
                        if (packet["reward_gun"] is JArray)
                        {
                            JArray items = packet["reward_gun"].Value<JArray>();
                            foreach (JObject item in items)
                            {
                                long gunWithUserId = Parser.Json.ParseLong(item["gun_with_user_id"]);
                                int gunId = Parser.Json.ParseInt(item["gun_id"]);

                                //log.Debug("인형 {0} 획득", gunId);

                                UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId), true);
                            }
                        }
                        else if (packet["reward_gun"] is JObject)
                        {
                            long gunWithUserId = Parser.Json.ParseLong(packet["reward_gun"]["gun_with_user_id"]);
                            int gunId = Parser.Json.ParseInt(packet["reward_gun"]["gun_id"]);

                            //log.Debug("인형 {0} 획득", gunId);

                            UserData.Doll.Add(new DollWithUserInfo(gunWithUserId, gunId), true);
                        }
                    }

                    // 모의작전이 아닌 경우만
                    if (GameData.Mission.GetUsedAp(UserData.CurrentMission.id) == 0)
                    {
                        // 임무 갱신
                        UserData.Quest.Daily.mission += 1;
                        UserData.Quest.Research.mission += 1;

                        switch (UserData.CurrentMission.id)
                        {
                            case 40: // 전역 4-6 승리
                                UserData.Quest.Research.mission46++;
                                break;
                            case 50: // 전역 5-6 승리
                                UserData.Quest.Research.mission56++;
                                break;
                            case 60: // 전역 6-6 승리
                                UserData.Quest.Research.mission66++;
                                break;
                            case 70: // 전역 7-6 승리
                                UserData.Quest.Research.mission76++;
                                break;
                        }

                        switch (UserData.CurrentMission.id)
                        {
                            case 1: case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10: case 15: case 16: case 17: case 18: case 19: case 20: case 25: case 26: case 27: case 28: case 29: case 30: case 35: case 36: case 37: case 38: case 39: case 40: case 45: case 46: case 47: case 48: case 49: case 50: case 55: case 56: case 57: case 58: case 59: case 60: case 65: case 66: case 67: case 68: case 69: case 70: case 75: case 76: case 77: case 78: case 79: case 80: case 85: case 86: case 87: case 88: case 89: case 90: case 95: case 96: case 97: case 98: case 99: case 100: case 105: case 106: case 107: case 108: case 109: case 110:
                                // 일반 전역승리
                                UserData.Quest.Research.missionNormal++;
                                break;
                            case 11: case 12: case 13: case 14: case 21: case 22: case 23: case 24: case 31: case 32: case 33: case 34: case 41: case 42: case 43: case 44: case 51: case 52: case 53: case 54: case 61: case 62: case 63: case 64: case 71: case 72: case 73: case 74: case 81: case 82: case 83: case 84: case 91: case 92: case 93: case 94: case 101: case 102: case 103: case 104: case 111: case 112: case 113: case 114:
                                // 긴급 전역승리
                                UserData.Quest.Research.missionEmergency++;
                                break;
                            case 90001: case 90002: case 90003: case 90004: case 90005: case 90006: case 90007: case 90008: case 90009: case 90010: case 90011: case 90012: case 90013: case 90014: case 90015: case 90016: case 90017: case 90018: case 90019: case 90020: case 90021: case 90022: case 90023: case 90024: case 90025: case 90026: case 90027: case 90028: case 90029: case 90030: case 90031: case 90032: case 90033: case 90034: case 90035: case 90036:
                                // 야간 전역승리
                                UserData.Quest.Research.missionNight++;
                                break;
                        }
                    }

                    UserData.CurrentMission.Clear();
                }
            }
            // 전역 실패
            if (response.ContainsKey("mission_lose_result") && response["mission_lose_result"] is JObject)
            {
                JObject packet = response["mission_lose_result"].Value<JObject>();
                if (packet.ContainsKey("turn"))
                {
                    log.Debug("전역 실패");

                    UserData.CurrentMission.Clear();
                }
            }
            // 붕괴액 피해
            if (response.ContainsKey("fairy_skill_return") && response["fairy_skill_return"] is JObject)
            {
                JObject response2 = response["fairy_skill_return"].Value<JObject>();
                if (response2.ContainsKey("mission_hurt") && response2["mission_hurt"] is JArray)
                {
                    JArray missionHurts = response2["mission_hurt"].Value<JArray>();
                    foreach (JObject missionHurt in missionHurts)
                    {
                        int teamId2 = Parser.Json.ParseInt(missionHurt["team_id"]);
                        log.Debug("제대 {0} 전역 중 피해", teamId2);
                        if (missionHurt.ContainsKey("guns_life") && missionHurt["guns_life"] is JArray)
                        {
                            JArray gunLifes = missionHurt["guns_life"].Value<JArray>();
                            foreach (JObject gunLife in gunLifes)
                            {
                                long gunWithUserId = Parser.Json.ParseLong(gunLife["gun_with_user_id"]);
                                int life = Parser.Json.ParseInt(gunLife["life"]);

                                UserData.Doll.SetHp(gunWithUserId, life);
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 거점 변화 패킷 처리
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void SpotBelongChange(JObject request, JObject response)
        {
            if (request == null)
                request = new JObject();
            if (response == null)
                response = new JObject();

            // 거점 변화 반영 1
            if (response.ContainsKey("change_belong1") && response["change_belong1"] is JObject)
            {
                // "change_belong1":{"80046":1,"80045":1}
                try
                {
                    List<string> keys = response["change_belong1"].Value<JObject>().Properties().Select(p => p.Name).ToList();
                    foreach (string key in keys)
                    {
                        int spot_id = Parser.String.ParseInt(key);
                        int belong = Parser.Json.ParseInt(response["change_belong1"][key]);
                        UserData.CurrentMission.SetBelongSpot(spot_id, belong);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "거점 변화 반영 중 에러 발생");
                }
            }

            // 거점 변화 반영 2
            if (response.ContainsKey("change_belong2") && response["change_belong2"] is JObject)
            {
                // "change_belong1":{"80046":1,"80045":1}
                try
                {
                    List<string> keys = response["change_belong2"].Value<JObject>().Properties().Select(p => p.Name).ToList();
                    foreach (string key in keys)
                    {
                        int spot_id = Parser.String.ParseInt(key);
                        int belong = Parser.Json.ParseInt(response["change_belong2"][key]);
                        UserData.CurrentMission.SetBelongSpot(spot_id, belong);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "거점 변화 반영 중 에러 발생");
                }
            }

            // 거점 변화 반영 (야간)
            if (response.ContainsKey("night_spots") && response["night_spots"] is JArray)
            {
                JArray nightSpots = response["night_spots"].Value<JArray>();
                foreach (JObject nightSpot in nightSpots)
                {
                    int spot_id = Parser.Json.ParseInt(nightSpot["spot_id"]);
                    int belong = Parser.Json.ParseInt(nightSpot["belong"]);

                    UserData.CurrentMission.SetBelongSpot(spot_id, belong);
                }
            }
        }
    }
}
