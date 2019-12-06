using GFAlarm.Data;
using GFAlarm.Data.Element;
using LocalizationResources;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GFAlarm.Util
{
    public class CsvExporter
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 아이템정보 저장
        /// </summary>
        /// <param name="packet"></param>
        public static void ExportItemInfo(JObject packet)
        {
            Encoding encoding;
            switch (Config.Setting.fileEncoding)
            {
                case "UTF-8":
                    encoding = Encoding.UTF8;
                    break;
                case "Default":
                default:
                    encoding = Encoding.Default;
                    break;
            }

            if (packet == null)
                return;

            try
            {
                Dictionary<string, string> items = new Dictionary<string, string>();
                if (packet["user_info"] != null && packet["user_info"] is JObject)
                {
                    JObject user_info = packet["user_info"].Value<JObject>();

                    //items.Add("UID", Parser.Json.ParseString(user_info["user_id"]));
                    //items.Add("닉네임", Parser.Json.ParseString(user_info["name"]));

                    //items.Add("지휘관 레벨", Parser.Json.ParseString(user_info["lv"]));
                    //items.Add("지휘관 경험치", Parser.Json.ParseString(user_info["experience"]));

                    //items.Add("인형 상한", Parser.Json.ParseString(user_info["maxgun"]));
                    //items.Add("장비 상한", Parser.Json.ParseString(user_info["maxequip"]));
                    //items.Add("요정 상한", Parser.Json.ParseString(user_info["max_fairy"]));
                    //items.Add("제대", Parser.Json.ParseString(user_info["maxteam"]));
                    //items.Add("인형제조 슬롯", Parser.Json.ParseString(user_info["max_build_slot"]));
                    //items.Add("장비제조 슬롯", Parser.Json.ParseString(user_info["max_equip_build_slot"]));
                    //items.Add("수복 슬롯", Parser.Json.ParseString(user_info["max_fix_slot"]));
                    //items.Add("스킬 슬롯", Parser.Json.ParseString(user_info["max_upgrade_slot"]));
                    //items.Add("제대 프리셋", Parser.Json.ParseString(user_info["max_gun_preset"]));
                    //items.Add("숙소", Parser.Json.ParseString(user_info["maxdorm"]));
                    //items.Add("전체 안락도", Parser.Json.ParseString(user_info["dorm_max_score"]));

                    //items.Add("인력", Parser.Json.ParseString(user_info["mp"]));
                    //items.Add("탄약", Parser.Json.ParseString(user_info["ammo"]));
                    //items.Add("식량", Parser.Json.ParseString(user_info["mre"]));
                    //items.Add("부품", Parser.Json.ParseString(user_info["part"]));
                    items.Add("대체코어", Parser.Json.ParseString(user_info["core"]));
                    items.Add("초급훈련자료", Parser.Json.ParseString(user_info["coin1"]));
                    items.Add("중급훈련자료", Parser.Json.ParseString(user_info["coin2"]));
                    items.Add("고급훈련자료", Parser.Json.ParseString(user_info["coin3"]));
                }
                if (packet["item_with_user_info"] != null && packet["item_with_user_info"] is JArray)
                {
                    JArray item_with_user_info = packet["item_with_user_info"].Value<JArray>();
                    foreach (JObject item in item_with_user_info)
                    {
                        string item_id = Parser.Json.ParseString(item["item_id"]);
                        string name = "";
                        string number = Parser.Json.ParseString(item["number"]);
                        switch (item_id)
                        {
                            case "1": // 인형제조계약
                                name = "인형제조계약";
                                break;
                            case "2": // 장비제조계약
                                name = "장비제조계약";
                                break;
                            case "3": // 쾌속제조계약
                                name = "쾌속제조계약";
                                break;
                            case "4": // 쾌속수복계약
                                name = "쾌속수복계약";
                                break;
                            case "7": // 서약의 증표
                                name = "서약의 증표";
                                break;
                            case "8": // 쾌속훈련계약
                                name = "쾌속훈련계약";
                                break;
                            case "9": // 증폭캡슐
                                name = "증폭캡슐";
                                break;
                            case "13": // 이름 변경권
                                name = "이름 변경권";
                                break;
                            case "14": // 원시 데이터샘플
                                name = "원시 데이터샘플";
                                break;
                            case "15": // 순정 데이터샘플
                                name = "순정 데이터샘플";
                                break;
                            case "16": // 쾌속분석계약
                                name = "쾌속분석계약";
                                break;
                            case "17": // 데이터 패치
                                name = "데이터 패치";
                                break;
                            case "41": // 구매토큰
                                name = "구매토큰";
                                break;
                            case "42": // 교환권
                                name = "교환권";
                                break;
                            case "43": // 블랙카드
                                name = "블랙카드";
                                break;
                            case "44": // 교정권
                                name = "교정권";
                                break;
                            case "45": // 기억파편
                                name = "기억파편";
                                break;
                            case "46": // 특수작전보고서
                                name = "특수작전보고서";
                                break;
                            case "47": // 국지전 소재
                                name = "국지전 소재";
                                break;
                            case "50": // 표준 부품
                                name = "표준 부품";
                                break;
                            case "51": // 신형 사통 소자
                                name = "신형 사통 소자";
                                break;
                            case "100": // 그리폰 훈장
                                name = "그리폰 훈장";
                                break;
                            case "101": // 전투 훈장
                                name = "전투 훈장";
                                break;
                            case "102": // 수집랭킹 훈장
                                name = "수집랭킹 훈장";
                                break;
                            case "103": // 챌린저 훈장
                                name = "챌린저 훈장";
                                break;
                            case "104": // 기묘한 모험 훈장
                                name = "기묘한 모험 훈장";
                                break;

                            case "301": // 핵심데이터 BGM-71
                                name = "핵심데이터 BGM-71";
                                break;
                            case "302": // 핵심데이터 AGS-30
                                name = "핵심데이터 AGS-30";
                                break;
                            case "303": // 핵심데이터 2B14
                                name = "핵심데이터 2B14";
                                break;
                            case "304": // 핵심데이터 M2
                                name = "핵심데이터 M2";
                                break;
                            case "305": // 핵심데이터 AT4
                                name = "핵심데이터 AT4";
                                break;
                            case "306": // 핵심데이터 QLZ-04
                                name = "핵심데이터 QLZ-04";
                                break;

                            case "506": // 전지
                                name = "전지";
                                break;
                            case "507": // 자유경험치
                                name = "자유경험치";
                                break;
                            case "508": // 우정점수
                                name = "우정점수";
                                break;
                            case "509": // 요정지령
                                name = "요정지령";
                                break;
                        }
                        if (!string.IsNullOrEmpty(name))
                        {
                            if (items.ContainsKey(name))
                            {
                                log.Warn("{0} 이미 존재함 ({1} 개)", name, items[name]);
                                items[name] = number;
                            }
                            else
                            {
                                items.Add(name, number);
                            }
                        }
                    }
                }
                if (packet["gift_with_user_info"] != null && packet["gift_with_user_info"] is JArray)
                {
                    JArray gift_with_user_info = packet["gift_with_user_info"].Value<JArray>();
                    foreach (JObject item in gift_with_user_info)
                    {
                        string item_id = Parser.Json.ParseString(item["item_id"]);
                        string name = "";
                        string number = Parser.Json.ParseString(item["number"]);
                        switch (item_id)
                        {
                            case "100001": // 막대사탕
                                name = "막대사탕";
                                break;
                            case "100002": // 소프트 콘
                                name = "소프트 콘";
                                break;
                            case "100003": // 딸기치즈케잌
                                name = "딸기치즈케잌";
                                break;
                            case "100005": // 뮬란
                                name = "뮬란";
                                break;
                            case "200001": // 작전보고서
                                name = "작전보고서";
                                break;
                            case "200003": // 맥주
                                name = "맥주";
                                break;
                        }
                        if (!string.IsNullOrEmpty(name))
                        {
                            if (items.ContainsKey(name))
                            {
                                log.Warn("{0} 이미 존재함 ({1} 개)", name, items[name]);
                                items[name] = number;
                            }
                            else
                            {
                                items.Add(name, number);
                            }
                        }
                    }
                }
                if (packet["kalina_with_user_info"] != null && packet["kalina_with_user_info"] is JObject)
                {
                    JObject kalina_with_user_info = packet["kalina_with_user_info"].Value<JObject>();
                    int level = Parser.Json.ParseInt(kalina_with_user_info["level"]);
                    int favor = Parser.Json.ParseInt(kalina_with_user_info["favor"]);
                    items.Add("카리나 호감레벨", level.ToString());
                    items.Add("카리나 호감도", favor.ToString());
                }

                var csv = new StringBuilder();

                var headerLine = string.Format("{0},{1}", "아이템 이름", "아이템 갯수");
                csv.AppendLine(headerLine);
                foreach (KeyValuePair<string, string> item in items)
                {
                    string newLine = string.Format("{0},{1}", item.Key, item.Value);
                    csv.AppendLine(newLine);
                }

                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                Directory.CreateDirectory(path);
                string filename = string.Format(path + "\\{0}_{1}_item_info.csv", UserData.name, UserData.uid);
                File.WriteAllText(@filename, csv.ToString(), encoding);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 탐색 정보
        /// </summary>
        /// <param name="packet"></param>
        public static void ExportExploreInfo(JObject packet)
        {
            try
            {
                // 탐색 목록
                if (packet != null)
                {
                    // 다음 발견 시간
                    long nextTime = Parser.Json.ParseLong(packet["next_time"]);
                    // 다음 탐색 시간
                    long nextExploreTime = Parser.Json.ParseLong(packet["next_explore_time"]);
                    // 탐색 종류 (1: 2시간, 2: 5시간, 3: 8시간)
                    int exploreTimeType = Parser.Json.ParseInt(packet["explore_time_type"]);

                    if (packet.ContainsKey("list") && packet["list"] is JArray)
                    {
                        log.Info("idx, start_time, end_time, cancel_time, area_id, area_name, target_id, item_id, affair_count, draw_event_prize");

                        JArray list = packet["list"].Value<JArray>();
                        int idx = 1;
                        foreach (JObject item in list)
                        {
                            int startTime = Parser.Json.ParseInt(item["start_time"]);
                            int endTime = Parser.Json.ParseInt(item["end_time"]);
                            int cancelTime = Parser.Json.ParseInt(item["cancel_time"]);

                            int areaId = Parser.Json.ParseInt(item["area_id"]);
                            int targetId = Parser.Json.ParseInt(item["target_id"]);
                            int itemId = Parser.Json.ParseInt(item["item_id"]);
                            int drawEventPrize = Parser.Json.ParseInt(item["draw_event_prize"]);

                            string areaName = "";
                            switch (areaId)
                            {
                                case 1:
                                    areaName = "도시";
                                    break;
                                case 2:
                                    areaName = "설원";
                                    break;
                                case 3:
                                    areaName = "숲";
                                    break;
                                case 4:
                                    areaName = "황무지";
                                    break;
                            }

                            int affairCount = 0;
                            if (item.ContainsKey("affairs") && item["affairs"] is JArray)
                            {
                                JArray affairs = item["affairs"].Value<JArray>();
                                foreach (JObject affair in affairs)
                                {
                                    affairCount++;
                                }
                            }

                            List<int> gunIds = new List<int>();
                            List<string> gunNames = new List<string>();
                            if (item.ContainsKey("gun_ids") && item["gun_ids"] is JArray)
                            {
                                //log.Info("gun_id, gun_name");

                                JArray guns = item["gun_ids"].Value<JArray>();
                                foreach (string gun in guns)
                                {
                                    int gunId = 0;
                                    if (gun.Contains("_"))
                                        gunId = Parser.String.ParseInt(gun.Split('_')[0]);
                                    else
                                        gunId = Parser.String.ParseInt(gun);
                                    string gunName = LanguageResources.Instance[string.Format("DOLL_{0}", gunId)];

                                    gunIds.Add(gunId);
                                    gunNames.Add(gunName);
                                }
                            }

                            List<int> petIds = new List<int>();
                            if (item.ContainsKey("pet_ids") && item["pet_ids"] is JArray)
                            {
                                JArray pets = item["pet_ids"].Value<JArray>();
                                foreach (string petId in pets)
                                {
                                    petIds.Add(Parser.String.ParseInt(petId));
                                }
                            }

                            log.Info("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
                                idx,
                                TimeUtil.GetDateTime(startTime, "MM-dd HH:mm"),
                                TimeUtil.GetDateTime(endTime, "MM-dd HH:mm"),
                                TimeUtil.GetDateTime(cancelTime, "MM-dd HH:mm"),
                                //Parser.Time.GetDateTime(startTime).ToString("MM-dd HH:mm"),
                                //Parser.Time.GetDateTime(endTime).ToString("MM-dd HH:mm"),
                                //Parser.Time.GetDateTime(cancelTime).ToString("MM-dd HH:mm"),
                                areaId, areaName, targetId, itemId, affairCount, drawEventPrize);

                            idx++;
                        }
                    }
                }

                var csv = new StringBuilder();

                
            }
            catch(Exception ex)
            {
                log.Error(ex, "탐색 정보 파싱 중 에러");
            }
        }

        /// <summary>
        /// 국지전 웨이브 정보
        /// </summary>
        /// <param name="area_id"></param>
        /// <param name="enemy_team"></param>
        public static void ExportTheaterExerciseInfo(int area_id, int[] enemy_teams)
        {
            Encoding encoding;
            switch (Config.Setting.fileEncoding)
            {
                case "UTF-8":
                    encoding = Encoding.UTF8;
                    break;
                case "Default":
                default:
                    encoding = Encoding.Default;
                    break;
            }

            try
            {
                var csv = new StringBuilder();

                var headerLine = string.Format("{0},{1},{2},{3},{4},{5}",
                    "지역번호", "웨이브", "구성번호", "구성", "야전", "최대역장");
                csv.AppendLine(headerLine);
                int waveNumber = 1;
                foreach (int enemy_team in enemy_teams)
                {
                    JObject data = GameData.Theater.GetData(area_id, enemy_team);
                    if (data != null)
                    {
                        string member = Parser.Json.ParseString(data["member"]);
                        string type = Parser.Json.ParseString(data["type"]);
                        string def_max_percent = Parser.Json.ParseString(data["def_max_percent"]);
                        if (!string.IsNullOrEmpty(def_max_percent))
                            def_max_percent += "%";

                        var newLine = string.Format("{0},{1}차,{2},{3},{4},{5}",
                            area_id, (waveNumber++).ToString().PadLeft(2, '0'), enemy_team, member, type, def_max_percent);
                        csv.AppendLine(newLine);
                    }
                }

                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                Directory.CreateDirectory(path);
                string filename = string.Format(path + "\\{0}_{1}_theater_exercise.csv", UserData.name, UserData.uid);
                File.WriteAllText(@filename, csv.ToString(), encoding);
            }
            catch (Exception ex)
            {
                log.Error(ex, "국지전 웨이브 정보 저장 중 에러 발생");
            }
        }

        /// <summary>
        /// 장비정보 저장
        /// </summary>
        /// <param name="equipWithUserInfos"></param>
        public static void ExportEquipInfo(Dictionary<long, EquipWithUserInfo> equipWithUserInfos)
        {
            Encoding encoding;
            switch (Config.Setting.fileEncoding)
            {
                case "UTF-8":
                    encoding = Encoding.UTF8;
                    break;
                case "Default":
                default:
                    encoding = Encoding.Default;
                    break;
            }

            try
            {
                var csv = new StringBuilder();

                var headerLine = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6}," +
                    "{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}," +
                    "{21},{22},{23},{24}", 
                    LanguageResources.Instance["EQUIP_INFO_ID"],
                    LanguageResources.Instance["EQUIP_INFO_NO"],
                    LanguageResources.Instance["EQUIP_INFO_TYPE"],
                    LanguageResources.Instance["EQUIP_INFO_STAR"],
                    LanguageResources.Instance["EQUIP_INFO_NAME"],
                    LanguageResources.Instance["EQUIP_INFO_USE_DOLL_ID"],
                    LanguageResources.Instance["EQUIP_INFO_USE_DOLL_NAME"],
                    LanguageResources.Instance["EQUIP_INFO_POW"],
                    LanguageResources.Instance["EQUIP_INFO_HIT"],
                    LanguageResources.Instance["EQUIP_INFO_DODGE"],
                    LanguageResources.Instance["EQUIP_INFO_SPEED"],
                    LanguageResources.Instance["EQUIP_INFO_RATE"],
                    LanguageResources.Instance["EQUIP_INFO_CRIT_PERCENT"],
                    LanguageResources.Instance["EQUIP_INFO_CRIT_HARM_RATE"],
                    LanguageResources.Instance["EQUIP_INFO_ARMOR_PIERCING"],
                    LanguageResources.Instance["EQUIP_INFO_ARMOR"],
                    LanguageResources.Instance["EQUIP_INFO_SHIELD"],
                    LanguageResources.Instance["EQUIP_INFO_DAMAGE_AMPLIFY"],
                    LanguageResources.Instance["EQUIP_INFO_DAMAGE_REDUCTION"],
                    LanguageResources.Instance["EQUIP_INFO_NIGHT_VIEW_PERCENT"],
                    LanguageResources.Instance["EQUIP_INFO_BULLET_NUMBER_UP"],
                    LanguageResources.Instance["EQUIP_INFO_EXP"],
                    LanguageResources.Instance["EQUIP_INFO_LEVEL"],
                    LanguageResources.Instance["EQUIP_INFO_ADJUST_COUNT"],
                    LanguageResources.Instance["EQUIP_INFO_IS_LOCKED"]);
                csv.AppendLine(headerLine);
                foreach (KeyValuePair<long, EquipWithUserInfo> item in equipWithUserInfos)
                {
                    long id = item.Key;
                    int equipId = item.Value.equipId;

                    //Dictionary<string, string> equipData = GameData.Equip.GetData(equipId);
                    JObject equipData = GameData.Equip.GetData(equipId);
                    string equipName = "";
                    string category = "";
                    int star = 0;
                    string starString = "";
                    try
                    {
                        equipName = LanguageResources.Instance[string.Format("EQUIP_{0}", equipId)];
                        //equipName = Parser.Json.ParseString(equipData["name"]);
                        category = LanguageResources.Instance[string.Format("EQUIP_TYPE_{0}", equipId)];
                        //category = Parser.Json.ParseString(equipData["category"]);
                        star = Parser.Json.ParseInt(equipData["star"]);
                        for (int i = 0; i < star; i++)
                            starString += "★";
                    }
                    catch(Exception ex)
                    {
                        log.Error(ex, "장비 정보 불러오기 실패");
                    }

                    long gunWithUserId = item.Value.gunWithUserId;
                    string gunName = "-";
                    if (gunWithUserId > 0)
                        gunName = UserData.Doll.GetName(gunWithUserId);

                    int pow = item.Value.pow;         // 화력
                    int hit = item.Value.hit;         // 명중
                    int dodge = item.Value.dodge;     // 회피
                    int speed = item.Value.speed;     // 이동속도
                    int rate = item.Value.rate;       // 사속
                    int criticalPercent = item.Value.criticalPercent;         // 치명률
                    int criticalHarmRate = item.Value.criticalHarmRate;       // 치명상
                    int armorPiercing = item.Value.armorPiercing;             // 관통
                    int armor = item.Value.armor;     // 장갑
                    int shield = item.Value.shield;   // 보호막
                    int damageAmplify = item.Value.damageAmplify;             // 피해 증폭
                    int damageReduction = item.Value.damageReduction;         // 피해 감쇄
                    int nightViewPercent = item.Value.nightViewPercent;       // 야시능력
                    int bulletNumberUp = item.Value.bulletNumberUp;           // 장탄수

                    long equipExp = item.Value.equipExp;
                    int equipLevel = item.Value.equipLevel;
                    int adjustCount = item.Value.adjustCount;
                    string isLocked = item.Value.isLocked == true ? "잠김" : "";

                    var newLine = string.Format(
                        "{0},{1},{2},{3},{4},{5},{6}," +
                        "{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}," +
                        "{21},{22},{23},{24}",
                        id, equipId, category, starString, equipName, gunWithUserId, gunName, 
                        pow, hit, dodge, speed, rate, criticalHarmRate, criticalPercent, armorPiercing, armor, shield, damageAmplify, damageReduction, nightViewPercent, bulletNumberUp,
                        equipExp, equipLevel, adjustCount, isLocked);
                    csv.AppendLine(newLine);
                }

                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                Directory.CreateDirectory(path);
                string filename = string.Format(path + "\\{0}_{1}_equip_info.csv", UserData.name, UserData.uid);
                File.WriteAllText(@filename, csv.ToString(), encoding);
            }
            catch (Exception ex)
            {
                log.Error(ex, "장비정보 저장 중 에러 발생");
            }
        }

        /// <summary>
        /// 요정정보 저장
        /// </summary>
        /// <param name="dictionary"></param>
        public static void ExportFairyInfo(Dictionary<long, FairyWithUserInfo> dictionary)
        {
            Encoding encoding;
            switch (Config.Setting.fileEncoding)
            {
                case "UTF-8":
                    encoding = Encoding.UTF8;
                    break;
                case "Default":
                default:
                    encoding = Encoding.Default;
                    break;
            }

            try
            {
                var csv = new StringBuilder();

                var headerLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", 
                    LanguageResources.Instance["FAIRY_INFO_ID"],
                    LanguageResources.Instance["FAIRY_INFO_NO"],
                    LanguageResources.Instance["FAIRY_INFO_QUALITY_LEVEL"],
                    LanguageResources.Instance["FAIRY_INFO_QUALITY_EXP"],
                    LanguageResources.Instance["FAIRY_INFO_LEVEL"],
                    LanguageResources.Instance["FAIRY_INFO_NAME"],
                    LanguageResources.Instance["FAIRY_INFO_TRAIT"],
                    LanguageResources.Instance["FAIRY_INFO_SKILL"],
                    LanguageResources.Instance["FAIRY_INFO_IS_LOCKED"]);
                csv.AppendLine(headerLine);

                foreach (KeyValuePair<long, FairyWithUserInfo> item in dictionary)
                {
                    long id = item.Value.id;
                    int fairyId = item.Value.no;
                    string name = LanguageResources.Instance[string.Format("FAIRY_{0}", fairyId)];
                    //string name = GameData.Fairy.GetData(fairyId, "name");
                    int level = item.Value.level;
                    int skill = item.Value.skill;
                    int star = item.Value.star;
                    string starString = "";
                    for (int i = 0; i < star; i++)
                        starString += "★";
                    int starExp = item.Value.starExp;
                    int traitId = item.Value.traitId;
                    //JObject trait = GameData.Fairy.GetTrait(traitId);
                    string traitName = LanguageResources.Instance[string.Format("TRAIT_{0}", traitId)];
                    //if (trait != null)
                    //{
                    //    traitName = Parser.Json.ParseString(trait["name"]);
                    //}
                    string isLocked = item.Value.isLocked == 1 ? LanguageResources.Instance["LOCKED"] : "";

                    var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", id, fairyId, starString, starExp, level, name, traitName, skill, isLocked);
                    csv.AppendLine(newLine);
                }

                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                Directory.CreateDirectory(path);
                string filename = string.Format(path + "\\{0}_{1}_fairy_info.csv", UserData.name, UserData.uid);
                //string filename = string.Format("infos/{0}_{1}_fairy_info_{2}_{3}.csv", UserData.name, UserData.userId, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"));
                File.WriteAllText(@filename, csv.ToString(), encoding);
            }
            catch(Exception ex)
            {
                log.Error(ex, "요정정보 저장 중 에러 발생");
            }
        }

        /// <summary>
        /// 인형정보 저장
        /// </summary>
        /// <param name="dictionary"></param>
        public static void ExportDollInfo(Dictionary<long, DollWithUserInfo> dictionary)
        {
            Encoding encoding;
            switch (Config.Setting.fileEncoding)
            {
                case "UTF-8":
                    encoding = Encoding.UTF8;
                    break;
                case "Default":
                default:
                    encoding = Encoding.Default;
                    break;
            }

            try
            {
                var csv = new StringBuilder();

                var headerLine = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31}", 
                    LanguageResources.Instance["DOLL_INFO_ID"], 
                    LanguageResources.Instance["DOLL_INFO_NO"],
                    LanguageResources.Instance["DOLL_INFO_TYPE"],
                    LanguageResources.Instance["DOLL_INFO_STAR"],
                    LanguageResources.Instance["DOLL_INFO_NAME"],
                    LanguageResources.Instance["DOLL_INFO_LEVEL"],
                    LanguageResources.Instance["DOLL_INFO_SUM_EXP"], 
                    LanguageResources.Instance["DOLL_INFO_CURRENT_EXP"],
                    LanguageResources.Instance["DOLL_INFO_REQUIRE_EXP"],
                    LanguageResources.Instance["DOLL_INFO_DUMMY_LINK"],
                    LanguageResources.Instance["DOLL_INFO_MOD"],
                    LanguageResources.Instance["DOLL_INFO_MAX_HP"],
                    LanguageResources.Instance["DOLL_INFO_HP"],
                    LanguageResources.Instance["DOLL_INFO_POW"],
                    LanguageResources.Instance["DOLL_INFO_HIT"],
                    LanguageResources.Instance["DOLL_INFO_DODGE"],
                    LanguageResources.Instance["DOLL_INFO_RATE"],
                    LanguageResources.Instance["DOLL_INFO_SPEED"],
                    LanguageResources.Instance["DOLL_INFO_ARMOR"],
                    LanguageResources.Instance["DOLL_INFO_BULLET"],
                    LanguageResources.Instance["DOLL_INFO_SKILL1"],
                    LanguageResources.Instance["DOLL_INFO_SKILL2"],
                    LanguageResources.Instance["DOLL_INFO_EQUIP1_ID"],
                    LanguageResources.Instance["DOLL_INFO_EQUIP1_NAME"],
                    LanguageResources.Instance["DOLL_INFO_EQUIP2_ID"],
                    LanguageResources.Instance["DOLL_INFO_EQUIP2_NAME"],
                    LanguageResources.Instance["DOLL_INFO_EQUIP3_ID"],
                    LanguageResources.Instance["DOLL_INFO_EQUIP3_NAME"],
                    LanguageResources.Instance["DOLL_INFO_FAVOR"],
                    LanguageResources.Instance["DOLL_INFO_IS_MARRIED"],
                    LanguageResources.Instance["DOLL_INFO_MARRIED_DATETIME"],
                    LanguageResources.Instance["DOLL_INFO_IS_LOCK"]);
                csv.AppendLine(headerLine);
                foreach(KeyValuePair<long, DollWithUserInfo> item in dictionary)
                {
                    long id = item.Value.id;
                    int gunId = item.Value.no;
                    //Dictionary<string, string> dollData = GameData.Doll.GetData(gunId);
                    JObject dollData = GameData.Doll.GetDollData(gunId);
                    string name = LanguageResources.Instance[string.Format("DOLL_{0}", gunId)];
                    string type = "Unknown";
                    if (dollData.ContainsKey("type"))
                        type = Parser.Json.ParseString(dollData["type"]);
                    int star = 0;
                    if (dollData.ContainsKey("star"))
                        star = Parser.Json.ParseInt(dollData["star"]);
                    string starString = "";
                    for (int i = 0; i < star; i++)
                        starString += "★";

                    int lv = item.Value.level;
                    int mod = item.Value.mod;
                    string modString = "";
                    if (mod > 0)
                        modString = "Mod" + mod;
                    long totalExp = item.Value.exp;
                    long exp = item.Value.exp - GameData.Doll.Exp.GetTotalExp(lv);
                    long requireExp = GameData.Doll.Exp.GetRequireExp(lv + 1, mod);
                    int number = item.Value.maxLink;
                    string numberString = "x" + number;
                    int skill1 = item.Value.skill1;
                    int skill2 = item.Value.skill2;

                    int maxLife = item.Value.maxHp;
                    int life = item.Value.hp;

                    int pow = item.Value.pow;
                    int hit = item.Value.hit;
                    int dodge = item.Value.dodge;
                    int rate = item.Value.rate;

                    int speed = item.Value.speed;
                    int armor = item.Value.armor;
                    int bullet = item.Value.bullet;

                    long equip1 = item.Value.equip1;
                    string equipName1 = "";
                    if (equip1 != 0)
                        equipName1 = LanguageResources.Instance[string.Format("EQUIP_{0}", UserData.Equip.GetEquipId(equip1))];
                    //string equipName1 = UserData.GetEquipName(equip1);
                    long equip2 = item.Value.equip2;
                    string equipName2 = "";
                    if (equip2 != 0)
                        equipName2 = LanguageResources.Instance[string.Format("EQUIP_{0}", UserData.Equip.GetEquipId(equip2))];
                    //string equipName2 = UserData.GetEquipName(equip2);
                    long equip3 = item.Value.equip3;
                    string equipName3 = "";
                    if (equip3 != 0) 
                        equipName3 = LanguageResources.Instance[string.Format("EQUIP_{0}", UserData.Equip.GetEquipId(equip3))];
                    //string equipName3 = UserData.GetEquipName(equip3);
                    long favor = item.Value.favor;
                    string soulBond = item.Value.married == true ? LanguageResources.Instance["MARRIED"] : "";
                    string soulBondTime = "";
                    if (item.Value.married == true)
                        soulBondTime = TimeUtil.GetDateTime(item.Value.marriedTime, "yyyy-MM-dd HH:mm");
                    //soulBondTime = Parser.Time.GetDateTime(item.Value.marriedTime).ToString("yyyy-MM-dd HH:mm");
                    string isLocked = item.Value.isLocked == true ? LanguageResources.Instance["LOCKED"] : "";

                    var newLine = string.Format(
                        "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31}",
                        id, gunId,
                        type, starString, name, 
                        lv, totalExp, exp, requireExp, numberString, modString,
                        maxLife, life,
                        pow, hit, dodge, rate, 
                        speed, armor, bullet,
                        skill1, skill2,
                        equip1, equipName1, equip2, equipName2, equip3, equipName3, 
                        favor, soulBond, soulBondTime, isLocked);
                    csv.AppendLine(newLine);

                    //try
                    //{
                    //    log.Info("인형스탯정보 가져오기 {0} {1}", lv, name);
                    //    Dictionary<string, double> stats = GameData.Doll.GetStat(gunId, lv);
                    //    if (stats != null && stats.ContainsKey("hp"))
                    //    {
                    //        foreach (KeyValuePair<string, double> entry in stats)
                    //        {
                    //            log.Info("key {0}, value {1}", entry.Key, stats[entry.Key]);
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    log.Error(ex, "인형스탯정보 에러");
                    //}
                }

                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
                Directory.CreateDirectory(path);
                string filename = string.Format(path + "\\{0}_{1}_doll_info.csv", UserData.name, UserData.uid);
                //string filename = string.Format("infos/{0}_{1}_doll_info_{2}_{3}.csv", UserData.name, UserData.userId, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"));
                File.WriteAllText(@filename, csv.ToString(), encoding);
            }
            catch(Exception ex)
            {
                log.Error(ex, "인형정보 저장 중 에러 발생");
            }
        }

        /// <summary>
        /// 획득 인형 저장
        /// </summary>
        /// <param name="gunId"></param>
        public static void WriteGetDollInfo(int gunId)
        {
            Encoding encoding;
            switch (Config.Setting.fileEncoding)
            {
                case "UTF-8":
                    encoding = Encoding.UTF8;
                    break;
                case "Default":
                default:
                    encoding = Encoding.Default;
                    break;
            }

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Infos";
            Directory.CreateDirectory(path);
            //string filename = String.Format(path + "\\{0}_{1}_rescued_doll_{2}.csv", UserData.name, UserData.userId, DateTime.Now.ToString("yyyyMMdd"));
            string filename = String.Format(path + "\\{0}_{1}_rescued_doll.csv", UserData.name, UserData.uid);
            if (!File.Exists(filename))
            {
                using (StreamWriter sw = new StreamWriter(filename, true, Encoding.GetEncoding("euc-kr")))
                {
                    string headerLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", 
                        LanguageResources.Instance["RESCUE_DOLL_DATETIME"],
                        LanguageResources.Instance["RESCUE_DOLL_MISSION_ID"],
                        LanguageResources.Instance["RESCUE_DOLL_MISSION_NAME"],
                        LanguageResources.Instance["RESCUE_DOLL_MISSION_COUNT"],
                        LanguageResources.Instance["RESCUE_DOLL_DOLL_ID"],
                        LanguageResources.Instance["RESCUE_DOLL_DOLL_NAME"],
                        LanguageResources.Instance["RESCUE_DOLL_DOLL_STAR"],
                        LanguageResources.Instance["RESCUE_DOLL_DOLL_CORE"]);
                    sw.WriteLine(headerLine, false, Encoding.UTF8);
                    sw.Close();
                }
            }

            using (StreamWriter sw = new StreamWriter(filename, true, Encoding.GetEncoding("euc-kr")))
            {
                //Dictionary<string, string> dollData = GameData.Doll.GetData(gunId);
                JObject dollData = GameData.Doll.GetDollData(gunId);
                string gunName = LanguageResources.Instance[string.Format("DOLL_{0}", gunId)];
                //string gunName = Parser.Json.ParseString(dollData["name"]);
                int star = Parser.Json.ParseInt(dollData["star"]);
                int core = GameData.Doll.GetCore(star);
                long counter = -1;
                int currentMissionId = UserData.CurrentMission.id;
                string currentMissionLocation = UserData.CurrentMission.location;
                MissionWithUserInfo mission = UserData.Mission.Get(currentMissionId);
                if (mission != null)
                {
                    counter = mission.counter;
                }

                string newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        currentMissionId,
                                        currentMissionLocation,
                                        counter,
                                        gunId,
                                        gunName,
                                        star,
                                        core);

                sw.WriteLine(newLine, false);
                sw.Close();
            }
        }
    }
}
