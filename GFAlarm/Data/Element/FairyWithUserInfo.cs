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
    "fairy_with_user_info": {
        "8329": {
            "id": "8329",
            "user_id": "343650",
            "fairy_id": "15",
            "team_id": "3",
            "fairy_lv": "100",
            "fairy_exp": "9999000",
            "quality_lv": "5",
            "quality_exp": "3000",
            "skill_lv": "10",
            "passive_skill": "910102",
            "is_locked": "1",
            "equip_id": "0",
            "adjust_count": "23",
            "last_adjust": "0",
            "passive_skill_collect": "910101,910116,910115,910105,910107,910112,910111,910114,910108,910103,910102",
            "skin": "0"
        },
        "11547": {
            "id": "11547",
            "user_id": "343650",
            "fairy_id": "16",
            "team_id": "6",
            "fairy_lv": "100",
            "fairy_exp": "9999000",
            "quality_lv": "5",
            "quality_exp": "3000",
            "skill_lv": "10",
            "passive_skill": "910102",
            "is_locked": "1",
            "equip_id": "0",
            "adjust_count": "13",
            "last_adjust": "0",
            "passive_skill_collect": "910101,910111,910106,910117,910109,910110,910107,910114,910113,910102",
            "skin": "0"
        },
     */
    #endregion

    public class FairyWithUserInfo
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public enum REFRESH
        {
            ALL,
            INFO,
            EXP,
            CHANGE_TEAM_EXP,
            STAT,
        }

        public long id = 0;                 // 요정 ID
        public int no = 0;                  // 요정 번호
        public string name = "unknown";       // 요정 이름
        public string category = "";                  // 요정 종류
        public int star = 0;                // 별
        public int starExp = 0;             // 개발수치

        public int level = 0;               // 레벨
        public int maxLevel = 100;          // 최대 레벨
        public int exp = 0;                // 누적 경험치
        public int remainExp = 0;          // 남은 경험치
        public int expRemainBefore = 0;

        public int currentExp = 0;         // 현재 경험치
        public int maxExp = 0;             // 최대 경험치

        public int traitId = 0;             // 특성 ID
        public string traitName = "unknown";  // 특성 이름
        public bool isRareTrait = false;    // 희귀 특성 여부

        public int team = 0;                // 제대

        public int skill = 0;               // 스킬 레벨

        public int adjustCount = 0;         // 교정 횟수
        public int isLocked = 0;            // 잠김 여부

        public int runEarnExp = 0;                          // 전역 획득 경험치
        public long[] runCount = new long[] { 0, 0, 0 };      // 전역 횟수
        public long[] reportCount = new long[] { 0, 0, 0 };   // 작보 갯수

        //public int[] earnExp = new int[] { 0, 0, 0, 0, 0, 0 };
        //public int[] toNextLevelRunCount = new int[] { 0, 0, 0, 0, 0, 0 };
        //public int[] toMaxLevelRunCount = new int[] { 0, 0, 0, 0, 0, 0 };

        //public int exp43e = 0;
        //public int exp02 = 0;
        //public int exp81n = 0;
        //public int exp104e = 0;
        //public int exp115 = 0;
        //public int exp4dra = 0;

        //public int exp43eCount = 0;
        //public int exp02Count = 0;
        //public int exp81nCount = 0;
        //public int exp104eCount = 0;
        //public int exp115Count = 0;
        //public int exp4draCount = 0;

        //public int exp43eMaxCount = 0;
        //public int exp02MaxCount = 0;
        //public int exp81nMaxCount = 0;
        //public int exp104eMaxCount = 0;
        //public int exp115MaxCount = 0;
        //public int exp4draMaxCount = 0;

        //public int toNextLevelBattleReportCount = 0;
        //public int toMaxLevelBattleReportCount = 0;

        // 최대 레벨 알림 여부
        public bool notifiedLevelMax = false;

        public FairyWithUserInfo(long fairyWithUserId, int no)
        {
            this.id = fairyWithUserId;
            this.no = no;
            this.level = 1;
            this.star = 1;
            this.skill = 1;

            Refresh();
        }

        public FairyWithUserInfo(dynamic data)
        {
            this.id = Parser.Json.ParseLong(data["id"]);
            this.no = Parser.Json.ParseInt(data["fairy_id"]);
            this.level = Parser.Json.ParseInt(data["fairy_lv"]);
            this.exp = Parser.Json.ParseInt(data["fairy_exp"]);
            this.skill = Parser.Json.ParseInt(data["skill_lv"]);
            this.star = Parser.Json.ParseInt(data["quality_lv"]);
            this.starExp = Parser.Json.ParseInt(data["quality_exp"]);
            this.team = Parser.Json.ParseInt(data["team_id"]);
            this.traitId = Parser.Json.ParseInt(data["passive_skill"]);
            this.isLocked = Parser.Json.ParseInt(data["is_locked"]);

            Refresh();
        }

        /// <summary>
        /// 새로고침
        /// </summary>
        /// <param name="refresh"></param>
        public void Refresh(REFRESH refresh = REFRESH.ALL)
        {
            switch (refresh)
            {
                case REFRESH.ALL:
                    Refresh(REFRESH.INFO);
                    Refresh(REFRESH.EXP);
                    break;
                case REFRESH.INFO:
                    // 요정 정보
                    //Dictionary<string, string> data = GameData.Fairy.GetData(no);
                    JObject fairy = GameData.Fairy.GetData(this.no);
                    if (fairy != null)
                    {
                        this.name = GameData.Fairy.GetFairyName(this.no);
                        this.category = Parser.Json.ParseString(fairy["category"]);
                    }
                    JObject trait = GameData.Fairy.GetTrait(this.traitId);
                    if (trait != null)
                    {
                        this.traitName = GameData.Fairy.GetFairyTraitName(this.traitId);
                        this.isRareTrait = Parser.Json.ParseInt(trait["is_rare"]) == 1 ? true : false;
                    }
                    break;
                case REFRESH.EXP:
                    maxLevel = 100;
                    
                    if (GameData.Fairy.Exp.totalExp[maxLevel] <= exp)
                    {
                        level = maxLevel;
                        exp = GameData.Fairy.Exp.totalExp[maxLevel];
                        remainExp = 0;

                        currentExp = 1;
                        maxExp = 1;
                    }
                    else if (level > 0 && level < 100 && GameData.Fairy.Exp.totalExp.ContainsKey(level + 1))
                    {
                        remainExp = GameData.Fairy.Exp.totalExp[level + 1] - exp;
                        while (remainExp < 0 && level != maxLevel)
                        {
                            level += 1;
                            remainExp = GameData.Fairy.Exp.totalExp[level + 1] - exp;
                        }

                        int currentLevelExp = GameData.Fairy.Exp.totalExp[level];
                        currentExp = exp - currentLevelExp;
                        if (level == maxLevel)
                            maxExp = GameData.Fairy.Exp.totalExp[maxLevel] - currentLevelExp;
                        else
                            maxExp = GameData.Fairy.Exp.totalExp[level + 1] - currentLevelExp;
                    }
                    break;
                case REFRESH.CHANGE_TEAM_EXP:
                    // 같은 제대 인형 경험치 가져오기
                    if (1 <= team && team <= 10)
                    {
                        try
                        {
                            runEarnExp = UserData.Fairy.GetTeamRunEarnExp(team);
                            if (runEarnExp > 0)
                                runCount[0] = (int)((double)remainExp / runEarnExp + 1);
                            runCount[1] = UserData.Fairy.GetRunCountToMaxLevel(id, runEarnExp);
                            runCount[2] = runCount[1];
                            reportCount[0] = UserData.Fairy.GetBattleReportCountToMaxLevel(id, level + 1);
                            reportCount[1] = UserData.Fairy.GetBattleReportCountToMaxLevel(id);
                            reportCount[2] = reportCount[1];
                        }
                        catch(Exception ex)
                        {
                            log.Error(ex, "요정 거지런 횟수 계산 중 에러");
                        }
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
