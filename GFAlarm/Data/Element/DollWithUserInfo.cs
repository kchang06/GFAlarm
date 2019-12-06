using GFAlarm.Util;
using LocalizationResources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Data.Element
{
    #region Packet Example
    /*
        "gun_with_user_info": [
            {
                "id": "4971671",
                "user_id": "343650",
                "gun_id": "20002",
                "gun_exp": "30283300",
                "gun_level": "120",
                "team_id": "0",
                "if_modification": "3",
                "location": "0",
                "position": "9",
                "life": "375",
                "ammo": "0",
                "mre": "0",
                "pow": "19",
                "hit": "46",
                "dodge": "69",
                "rate": "19",
                "skill1": "10",
                "skill2": "10",
                "fix_end_time": "0",
                "is_locked": "1",
                "number": "5",
                "equip1": "17733650",
                "equip2": "36518739",
                "equip3": "60477812",
                "equip4": "0",
                "favor": "2000000",
                "max_favor": "2000000",
                "favor_toplimit": "2000000",
                "soul_bond": "1",
                "skin": "0",
                "can_click": "0",
                "soul_bond_time": "0"
            },
            {
                "id": "4994792",
                "user_id": "343650",
                "gun_id": "20029",
                "gun_exp": "30283300",
                "gun_level": "120",
                "team_id": "8",
                "if_modification": "3",
                "location": "5",
                "position": "7",
                "life": "975",
                "ammo": "0",
                "mre": "0",
                "pow": "19",
                "hit": "15",
                "dodge": "70",
                "rate": "29",
                "skill1": "10",
                "skill2": "10",
                "fix_end_time": "0",
                "is_locked": "1",
                "number": "5",
                "equip1": "0",
                "equip2": "0",
                "equip3": "71234127",
                "equip4": "0",
                "favor": "1000000",
                "max_favor": "1000000",
                "favor_toplimit": "1000000",
                "soul_bond": "0",
                "skin": "0",
                "can_click": "2",
                "soul_bond_time": "0"
            },
     */
    #endregion

    public class DollWithUserInfo
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public enum REFRESH
        {
            ALL,
            INFO,
            EXP,
            STAT,
            HP,
            LINK,
            SUPPLY,
        }

        #region Variable

        public long id = 0;                              // ID
        public int no = 0;                               // 도감번호
        public string name = "unknown";                  // 이름
        public int star = 0;                             // 레어도
        public bool collabo = false;                     // 콜라보 여부
        public string type = "";                         // 병과

        public int beforeLevel = 0;                      // 이전 레벨
        public int level = 1;                            // 레벨
        public int expandLevel = 0;                      // 다음 편제확대 레벨
        public int maxLevel = 100;                       // 최대 레벨
        public int mod = 0;                              // 모드

        public int team = 0;                             // 제대
        public int location = 0;                         // 순번
        public int beforeLocation = 0;                   // 이전 순번
        public int position = 0;                         // 진형

        public int currentExp = 0;                       // 현재 경험치
        public int exp = 0;                              // 누적 경험치
        public int remainExp = 0;                        // 남은 경험치
        public int maxExp = 0;                           // 최대 경험치

        public int dummyHp = 0;                          // 더미 체력
        public int hp = 0;                               // 체력
        public int maxHp = 0;                            // 최대 체력
        public int hpWarningLevel = 0;                   // 체력 경고 레벨 (2: 중상, 1: 부상, 0: 평범)

        public int link = 1;                             // 링크
        public int maxLink = 1;                          // 최대 링크

        public int[] restore = new int[] { 0, 0, 0 };    // 수복 (시간, 인력, 부품)

        public int ammo = 5;                             // 탄약
        public int mre = 10;                             // 식량

        public int pow = 0;                              // 화력
        public int hit = 0;                              // 명중
        public int dodge = 0;                            // 회피
        public int rate = 0;                             // 사속
        public int speed = 0;                            // 이동속도
        public int armor = 0;                            // 장갑
        public int bullet = 0;                           // 장탄
        public int crit = 0;                             // 치명률

        public int skill1 = 1;                           // 스킬1
        public int skill2 = 1;                           // 스킬2

        public long equip1 = 0;                          // 장비1 ID
        public long equip2 = 0;                          // 장비2 ID
        public long equip3 = 0;                          // 장비3 ID


        public int skin = 0;                             // 스킨

        public int favor = 0;                            // 호감도
        public int maxFavor = 0;                         // 최대 호감도

        public bool married = false;                     // 서약 여부
        public int marriedTime = 0;                      // 서약 날짜시각
        public bool isLocked = false;                    // 잠금 여부

        public int refreshCount = 0;

        /// <summary>
        /// 거지런 횟수
        /// </summary>


        //public int[] earnExp = new int[] { 0, 0, 0, 0, 0, 0 };
        //public int[] toNextLevelRunCount = new int[] { 0, 0, 0, 0, 0, 0 };
        //public int[] toNextExpandRunCount = new int[] { 0, 0, 0, 0, 0, 0 };
        //public int[] toMaxLevelRunCount = new int[] { 0, 0, 0, 0, 0, 0 };

        public int runEarnExp = 0;                          // 전역 획득 경험치
        public long[] runCount = new long[] { 0, 0, 0 };      // 전역 횟수
        public long[] reportCount = new long[] { 0, 0, 0 };   // 작보 갯수
        //public int expBattleReport = 0;

        //public int toNextLevelBattleReportCount = 0;
        //public int toNextExpandBattleReportCount = 0;
        //public int toMaxLevelBattleReportCount = 0;

        public bool notifiedWounded = false;            // 중상 알림
        public bool notifiedNeedExpandLink = false;     // 편제확대 알림
        public bool notifiedMaxLevel = false;           // 최대레벨 알림

        #endregion

        #region Initializer

        public DollWithUserInfo() { }

        public DollWithUserInfo(DollWithUserInfo doll)
        {
            this.id = doll.id;
            this.no = doll.no;

            this.exp = doll.exp;
            this.level = doll.level;
            this.mod = doll.mod;

            this.hp = doll.hp;

            this.maxLink = doll.maxLink;

            this.ammo = doll.ammo;
            this.mre = doll.mre;

            this.team = doll.team;
            this.location = doll.location;

            this.equip1 = doll.equip1;
            this.equip2 = doll.equip2;
            this.equip3 = doll.equip3;

            this.skill1 = doll.skill1;
            this.skill2 = doll.skill2;

            this.skin = doll.skin;

            this.favor = doll.favor;
            this.married = doll.married;
            this.marriedTime = doll.marriedTime;

            this.isLocked = doll.isLocked;

            this.maxLevel = GameData.Doll.GetMaxLevel(this.mod);
            if (this.maxLevel == this.level)
                this.notifiedMaxLevel = true;
        }

        /// <summary>
        /// 반영
        /// </summary>
        /// <param name="data"></param>
        public void Init(DollWithUserInfo data)
        {
            
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public void Clear()
        {

        }

        public DollWithUserInfo(long id, int no)
        {
            this.id = id;
            this.no = no;

            this.level = 1;
            this.skill1 = 1;
            this.link = 1;
            this.maxLink = 1;

            Refresh();

            this.hp = this.maxHp;
        }

        public DollWithUserInfo(JObject data)
        {
            this.id = Parser.Json.ParseLong(data["id"]);
            this.no = Parser.Json.ParseInt(data["gun_id"]);

            this.exp = Parser.Json.ParseInt(data["gun_exp"]);
            this.level = Parser.Json.ParseInt(data["gun_level"]);
            this.mod = Parser.Json.ParseInt(data["if_modification"]);

            this.hp = Parser.Json.ParseInt(data["life"]);

            this.maxLink = Parser.Json.ParseInt(data["number"]);

            this.ammo = Parser.Json.ParseInt(data["ammo"]);
            this.mre = Parser.Json.ParseInt(data["mre"]);

            this.team = Parser.Json.ParseInt(data["team_id"]);
            this.location = Parser.Json.ParseInt(data["location"]);

            this.equip1 = Parser.Json.ParseLong(data["equip1"]);
            this.equip2 = Parser.Json.ParseLong(data["equip2"]);
            this.equip3 = Parser.Json.ParseLong(data["equip3"]);

            this.skill1 = Parser.Json.ParseInt(data["skill1"]);
            this.skill2 = Parser.Json.ParseInt(data["skill2"]);

            this.skin = Parser.Json.ParseInt(data["skin"]);

            this.favor = Parser.Json.ParseInt(data["favor"]);
            this.married = Parser.Json.ParseInt(data["soul_bond"]) == 1 ? true : false;
            this.marriedTime = Parser.Json.ParseInt(data["soul_bond_time"]);

            this.isLocked = Parser.Json.ParseInt(data["is_locked"]) == 1 ? true : false;

            this.maxLevel = GameData.Doll.GetMaxLevel(this.mod);
            if (this.maxLevel == this.level)
                this.notifiedMaxLevel = true;

            Refresh();
        }

        #endregion

        #region Function

        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 새로고침
        /// </summary>
        public void Refresh(REFRESH refresh = REFRESH.ALL, bool forceUpdate = false)
        {
            switch (refresh)
            {
                case REFRESH.ALL:
                    Refresh(REFRESH.INFO, forceUpdate);
                    Refresh(REFRESH.STAT, forceUpdate);
                    Refresh(REFRESH.HP, forceUpdate);
                    Refresh(REFRESH.EXP, forceUpdate);
                    Refresh(REFRESH.SUPPLY, forceUpdate);
                    refreshCount++;
                    //log.Debug("id={0}, name={1}, refresh_type={2}, refresh_count={3}", this.id, this.name, refresh, this.refreshCount);
                    break;
                case REFRESH.INFO:
                    // 인형 정보
                    //Dictionary<string, string> data = GameData.Doll.GetData(no);
                    JObject data = GameData.Doll.GetDollData(no);
                    try
                    {
                        this.type = Parser.Json.ParseString(data["type"]);
                        this.name = LanguageResources.Instance[string.Format("DOLL_{0}", this.no)];
                        this.star = Parser.Json.ParseInt(data["star"]);
                        this.collabo = Parser.Json.ParseInt(data["collabo"]) == 1 ? true : false;
                    }
                    catch(Exception ex)
                    {
                        log.Warn(ex, "인형 데이터베이스 불러오기 실패");
                    }
                    break;
                case REFRESH.EXP:
                    this.maxLevel = GameData.Doll.GetMaxLevel(this);
                    if (GameData.Doll.Exp.GetTotalExp(this.maxLevel) <= this.exp)
                    {
                        this.level = this.maxLevel;
                        this.exp = GameData.Doll.Exp.GetTotalExp(this.maxLevel);
                        this.remainExp = 0;

                        this.currentExp = 1;
                        this.maxExp = 1;
                    }
                    else if (0 < level && level < 120)
                    {
                        this.remainExp = GameData.Doll.Exp.GetTotalExp(level + 1) - this.exp;
                        while (this.remainExp <= 0 && this.level != this.maxLevel)
                        {
                            this.level += 1;
                            this.remainExp = GameData.Doll.Exp.GetTotalExp(this.level + 1) - this.exp;
                        }
                        int currentLevelExp = GameData.Doll.Exp.GetTotalExp(this.level);
                        this.currentExp = exp - currentLevelExp;
                        if (this.level == this.maxLevel)
                            this.maxExp = GameData.Doll.Exp.GetTotalExp(this.maxLevel) - currentLevelExp;
                        else
                            this.maxExp = GameData.Doll.Exp.GetTotalExp(this.level + 1) - currentLevelExp;
                    }
                    if (1 <= location && location <= 5 && 1 <= team && team <= 10)
                    {
                        if (this.beforeLevel != this.level || this.beforeLocation != this.location || forceUpdate)
                        {
                            runEarnExp = GameData.Doll.GetRunExp(this, Config.Echelon.baseExp, Config.Echelon.levelPenalty, true) * Config.Echelon.battleCount;

                            //earnExp[0] = GameData.Doll.GetRunExp(this, 370, 75, true) * 4;
                            //earnExp[1] = GameData.Doll.GetRunExp(this, 490, 112, true) * 5;
                            //earnExp[2] = GameData.Doll.GetRunExp(this, 500, 120, true) * 5;
                            //earnExp[3] = GameData.Doll.GetRunExp(this, 500, 120, true) * 5;
                            //earnExp[4] = GameData.Doll.GetRunExp(this, 550, 120, true) * 5;
                            //earnExp[5] = GameData.Doll.GetRunExp(this, 500, 115, true) * 5;

                            this.beforeLevel = this.level;
                            this.beforeLocation = this.location;
                        }

                        if (this.remainExp > 0)
                        {
                            //int[] tempEarnExp = earnExp;
                            //for (int i = 0; i < 6; i++)
                            //{
                            //    toNextLevelRunCount[i] = Convert.ToInt32(Math.Ceiling((double)expRemain / earnExp[i]));
                            //}

                            int nextLevel = this.level + 1;
                            runCount[0] = UserData.Doll.GetRequireRun(id, Config.Echelon.baseExp, Config.Echelon.levelPenalty, Config.Echelon.battleCount, nextLevel);

                            //toNextLevelRunCount[0] = UserData.Doll.GetRequireRun(this.id, 370, 75, 4, nextLevel);   // 4-3e
                            //toNextLevelRunCount[1] = UserData.Doll.GetRequireRun(this.id, 490, 112, 5, nextLevel);  // 0-2
                            //toNextLevelRunCount[2] = UserData.Doll.GetRequireRun(this.id, 500, 120, 5, nextLevel);  // 8-1n
                            //toNextLevelRunCount[3] = UserData.Doll.GetRequireRunCountToLevel(this.id, 500, 120, 5, nextLevel);  // 10-4e
                            //toNextLevelRunCount[4] = UserData.Doll.GetRequireRun(this.id, 550, 120, 5, nextLevel);  // 11-5
                            //toNextLevelRunCount[5] = UserData.Doll.GetRequireRun(this.id, 500, 115, 5, nextLevel);  // sg 3-2
                            //if (this.no == 278)
                            //{
                            //    log.Warn("six12 next_level {0}", nextLevel);
                            //    log.Warn("six12 to_next_level_run {0}", toNextLevelRunCount[0]);
                            //}

                            int nextExpandLevel = UserData.Doll.GetExpandLevel(id);
                            runCount[1] = UserData.Doll.GetRequireRun(id, Config.Echelon.baseExp, Config.Echelon.levelPenalty, Config.Echelon.battleCount, nextExpandLevel);
                            //toNextExpandRunCount[0] = UserData.Doll.GetRequireRun(this.id, 370, 75, 4, nextExpandLevel);
                            //toNextExpandRunCount[1] = UserData.Doll.GetRequireRun(this.id, 490, 112, 5, nextExpandLevel);
                            //toNextExpandRunCount[2] = UserData.Doll.GetRequireRun(this.id, 500, 120, 5, nextExpandLevel);
                            //toNextExpandRunCount[3] = UserData.Doll.GetRequireRunCountToLevel(this.id, 500, 120, 5, nextExpandLevel);
                            //toNextExpandRunCount[4] = UserData.Doll.GetRequireRun(this.id, 550, 120, 5, nextExpandLevel);
                            //toNextExpandRunCount[5] = UserData.Doll.GetRequireRun(this.id, 500, 115, 5, nextExpandLevel);

                            int maxLevel = UserData.Doll.GetMaxLevel(id);
                            runCount[2] = UserData.Doll.GetRequireRun(id, Config.Echelon.baseExp, Config.Echelon.levelPenalty, Config.Echelon.battleCount, maxLevel);
                            //toMaxLevelRunCount[0] = UserData.Doll.GetRequireRun(this.id, 370, 75, 4, maxLevel);
                            //toMaxLevelRunCount[1] = UserData.Doll.GetRequireRun(this.id, 490, 112, 5, maxLevel);
                            //toMaxLevelRunCount[2] = UserData.Doll.GetRequireRun(this.id, 500, 120, 5, maxLevel);
                            //toMaxLevelRunCount[3] = UserData.Doll.GetRequireRunCountToLevel(this.id, 500, 120, 5, maxLevel);
                            //toMaxLevelRunCount[4] = UserData.Doll.GetRequireRun(this.id, 550, 120, 5, maxLevel);
                            //toMaxLevelRunCount[5] = UserData.Doll.GetRequireRun(this.id, 500, 115, 5, maxLevel);

                            //exp43eCount = Convert.ToInt32(Math.Ceiling((double)expRemain / earnExp[0]));
                            //exp02Count = Convert.ToInt32(Math.Ceiling((double)expRemain / earnExp[1]));
                            //exp81nCount = Convert.ToInt32(Math.Ceiling((double)expRemain / earnExp[2]));
                            //exp104eCount = Convert.ToInt32(Math.Ceiling((double)expRemain / earnExp[3]));
                            //exp115Count = Convert.ToInt32(Math.Ceiling((double)expRemain / earnExp[4]));
                            //exp4draCount = Convert.ToInt32(Math.Ceiling((double)expRemain / earnExp[5]));

                            //exp43eMaxCount = UserData.Doll.GetRunCountToMaxLevel(this.id, 370, 75, 4);
                            //exp02MaxCount = UserData.Doll.GetRunCountToMaxLevel(this.id, 490, 112, 5);
                            //exp81nMaxCount = UserData.Doll.GetRunCountToMaxLevel(this.id, 500, 120, 5);
                            //exp104eMaxCount = UserData.Doll.GetRunCountToMaxLevel(this.id, 500, 120, 5);
                            //exp115MaxCount = UserData.Doll.GetRunCountToMaxLevel(this.id, 550, 120, 5);
                            //exp4draMaxCount = UserData.Doll.GetRunCountToMaxLevel(this.id, 500, 115, 5);

                            reportCount[0] = UserData.Doll.GetRequireBattleReport(this.id, this.level + 1);
                            reportCount[1] = UserData.Doll.GetRequireBattleReport(this.id, nextExpandLevel);
                            reportCount[2] = UserData.Doll.GetRequireBattleReport(this.id, maxLevel);
                            //toNextLevelBattleReportCount = UserData.Doll.GetRequireBattleReport(this.id, this.level + 1);
                            //toNextExpandBattleReportCount = UserData.Doll.GetRequireBattleReport(this.id, UserData.Doll.GetExpandLevel(this.id));
                            //toMaxLevelBattleReportCount = UserData.Doll.GetRequireBattleReport(this.id, this.maxLevel);
                        }
                    }
                    break;
                case REFRESH.STAT:
                    // 인형 스탯
                    //Dictionary<string, double> stat = GameData.Doll.GetStat(this.no, this.level);
                    //this.dummyHp = Convert.ToInt32(stat["hp"]);
                    //this.maxHp = this.dummyHp * this.maxLink;
                    //this.pow = Convert.ToInt32(stat["pow"]);
                    //this.hit = Convert.ToInt32(stat["hit"]);
                    //this.dodge = Convert.ToInt32(stat["dodge"]);
                    //this.speed = Convert.ToInt32(stat["speed"]);
                    //this.rate = Convert.ToInt32(stat["rate"]);
                    //this.armor = Convert.ToInt32(stat["armor"]);
                    //this.bullet = Convert.ToInt32(stat["bullet"]);
                    //this.crit = Convert.ToInt32(stat["crit"]);
                    int[] stat = GameData.Doll.Stat.GetStat(this.no, this.level, this.favor);
                    //log.Debug("dummy_hp {0}", stat[0]);
                    this.dummyHp = stat[0];
                    this.maxHp = this.dummyHp * this.maxLink;
                    this.pow = stat[1];
                    this.hit = stat[2];
                    this.dodge = stat[3];
                    this.speed = stat[4];
                    this.rate = stat[5];
                    this.armor = stat[6];
                    this.bullet = stat[7];
                    this.crit = stat[8];
                    break;
                case REFRESH.HP:
                    // 체력 경고
                    double hpPercent = (double)this.hp / this.maxHp;
                    if (hpPercent < 0.3)
                        this.hpWarningLevel = 2;
                    else if (hpPercent < 0.8)
                        this.hpWarningLevel = 1;
                    else
                        this.hpWarningLevel = 0;
                    // 링크
                    //log.Debug("{0} dummy_hp {1} hp {2}", this.name, this.dummyHp, this.hp);
                    if (this.dummyHp > 0)
                        this.link = (int)Math.Ceiling((double)this.hp / (double)this.dummyHp);
                    // 수복
                    this.restore = GameData.Doll.Restore.GetRestore(this.type, this.level, this.hp, this.maxHp, this.maxLink, this.married);
                    break;
                case REFRESH.SUPPLY:
                    // 보급
                    //GameData.Doll.GetSupplyCount(this, ref ammoCount, ref mreCount);
                    break;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}
