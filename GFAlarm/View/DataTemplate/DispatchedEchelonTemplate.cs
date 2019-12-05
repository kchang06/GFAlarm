using GFAlarm.Data;
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
using static GFAlarm.Data.GameData.Mission;

namespace GFAlarm.View.DataTemplate
{
    /// <summary>
    /// Dispatched Echelon Template
    /// </summary>
    public class DispatchedEchleonTemplate : INotifyPropertyChanged
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // ==============================================
        // ===== Autonomous Mission
        // ==============================================
        #region Autonomous Mission

        /// <summary>
        /// Autonomous Mission ID
        /// </summary>
        public int autoMissionId
        {
            get { return _autoMissionId; }
            set
            {
                _autoMissionId = value;
                if (value > 0)
                {
                    JObject data = GameData.Mission.GetData(value);
                    if (data != null)
                    {
                        this.TBCode = Parser.Json.ParseString(data["location"]);
                        this.TBType = LanguageResources.Instance["AUTO_BATTLE"];
                        type = 2;

                        requireTime = GameData.Mission.GetRequireTime(value);

                        /*
                        try
                        {
                            spendResource = Parser.Json.ParseString(data["require_resource"]).Split(',').Select(Int32.Parse).ToArray();
                            if (spendResource.Length == 2)
                            {
                                //int curRes = (spendResource[0] + spendResource[1]) * autoMissionNumber;
                                int curRes = spendResource[0] * autoMissionNumber;
                                int minRes = int.MaxValue;
                                List<SaveRunMission> recommend = null;
                                foreach (KeyValuePair<int, List<SaveRunMission>> item in GameData.Mission.saveRunMissions)
                                {
                                    if (item.Key > curRes && item.Key < minRes)
                                    {
                                        minRes = item.Key;
                                        recommend = new List<SaveRunMission>(item.Value);
                                    }
                                }
                                MultiKeyDictionary<int, int, SaveRunMission> test2 = new MultiKeyDictionary<int, int, SaveRunMission>();
                                foreach (SaveRunMission item in recommend)
                                {
                                    if (test2.ContainsKey(item.number, item.requireEch))
                                    {
                                        if (test2[item.number, item.requireEch].lvPenalty > item.lvPenalty)
                                            test2[item.number, item.requireEch] = item;
                                    }
                                    else
                                    {
                                        test2.Add(item.number, item.requireEch, item);
                                    }
                                }
                                recommend = test2.Values.ToList();
                                if (recommend != null)
                                {
                                    int mp = spendResource[0] * autoMissionNumber;
                                    int part = spendResource[1] * autoMissionNumber;
                                    List<SaveRunMission> test = recommend.OrderBy(O => O.number).ThenBy(O => O.requireEch).ThenBy(O => O.lvPenalty).ToList();
                                    for (int i = 0; i < test.Count(); i++)
                                    {
                                        recommandMissionSaveRun += string.Format(
                                            "[font color='#ffffff'][b]{0}[/b][/font] × [font color='#ffb400']{1}[/font]회 ([font color='#ffb400']{2}[/font]제대 필요)[br]인탄식 {3}([font color='#8FC41F']{4}[/font]) 부 {5}([font color='#8FC41F']{6}[/font])",
                                            test[i].location, test[i].number,
                                            test[i].requireEch,
                                            test[i].mp, (test[i].mp - mp).ToString("+#;-#;0"),
                                            test[i].part, (test[i].part - part).ToString("+#;-#;0"));
                                        if (i != test.Count() - 1)
                                            recommandMissionSaveRun += "[br]";
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                        */
                    }
                }
            }
        }
        private int _autoMissionId = -1;

        /// <summary>
        /// Autonomous Number
        /// </summary>
        public int autoMissionNumber = 0;

        /// <summary>
        /// Spend Resource
        /// </summary>
        public int[] spendResource
        {
            get { return _spendResource; }
            set
            {
                _spendResource = value;
                OnPropertyChanged();
            }
        }
        private int[] _spendResource = new int[] { 0, 0 };

        /// <summary>
        /// 발판저축런 추천 자율작전
        /// </summary>
        public string recommandMissionSaveRun
        {
            get { return _recommandMissionSaveRun; }
            set
            {
                _recommandMissionSaveRun = value;
                OnPropertyChanged();
            }
        }
        private string _recommandMissionSaveRun = "";

        #endregion

        // ==============================================
        // ===== Logistic Support
        // ==============================================
        #region Logistic Support

        /// <summary>
        /// Logistic Support ID
        /// </summary>
        public int operationId
        {
            get { return _operationId; }
            set
            {
                _operationId = value;
                if (value > 0)
                {
                    JObject data = GameData.Logistics.GetData(value);
                    if (data != null)
                    {
                        // 기본 정보
                        this.TBCode = Parser.Json.ParseString(data["location"]);
                        this.TBType = LanguageResources.Instance["LOGISTICS"];
                        this.TBNumber = "";
                        type = 1;

                        // 소요 시간
                        requireTime = TimeUtil.ParseHHMM(Parser.Json.ParseString(data["require_time"]));
                        //requireTime = Parser.String.ParseHHMM(Parser.Json.ParseString(data["require_time"]));

                        // 획득 자원
                        try
                        {
                            int[] resources = Parser.Json.ParseString(data["resources"]).Split(',').Select(Int32.Parse).ToArray();
                            if (resources.Length == 4)
                            {
                                this.rewardResource = resources;

                                // 시간당 획득 자원 계산
                                //double ratio = 3600 / ((double)requireTime / 1000);
                                double ratio = 3600 / (double)requireTime;

                                this.rewardResourcePerHour[0] = Convert.ToInt32(Math.Round((double)resources[0] * ratio));
                                this.rewardResourcePerHour[1] = Convert.ToInt32(Math.Round((double)resources[1] * ratio));
                                this.rewardResourcePerHour[2] = Convert.ToInt32(Math.Round((double)resources[2] * ratio));
                                this.rewardResourcePerHour[3] = Convert.ToInt32(Math.Round((double)resources[3] * ratio));

                                OnPropertyChanged("rewardResourcePerHour");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        // 획득 아이템
                        this.rewardItemIcon[0] = GetItemUri(Parser.Json.ParseString(data["item1"]));
                        this.rewardItemIcon[1] = GetItemUri(Parser.Json.ParseString(data["item2"]));
                        this.rewardItemCount[0] = 1;
                        this.rewardItemCount[1] = 1;

                        OnPropertyChanged("rewardItemIcon");
                        OnPropertyChanged("rewardItemCount");
                    }
                }
            }
        }
        private int _operationId = -1;

        #endregion

        // ==============================================
        // ===== Common
        // ==============================================
        #region Common

        /// <summary>
        /// Type
        /// 1: Logistic Support, 2: Autonomous Mission
        /// </summary>
        public int type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }
        private int _type = -1;

        /// <summary>
        /// 투입 제대
        /// </summary>
        public int[] teamIds
        {
            get { return _teamIds; }
            set
            {
                _teamIds = value;
                for (int i = 0; i < value.Length; i++)
                {
                    TBTeamIds += value[i];
                    if (i != value.Length - 1)
                        TBTeamIds += ", ";
                }
                if (value.Length > 0)
                    teamId = value[0];
            }
        }
        private int[] _teamIds = new int[] { 0 };
        public int teamId
        {
            get { return _teamId; }
            set
            {
                _teamId = value;
                if (value >= 10)
                    TBSlot = "0";
                else
                    TBSlot = value.ToString();
            }
        }
        private int _teamId = 0;

        /// <summary>
        /// 시작 시간
        /// </summary>
        public int startTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                if (value > 0 && requireTime > 0)
                {
                    if (autoMissionNumber > 0)
                        endTime = value + (requireTime * autoMissionNumber);
                    else
                        endTime = value + requireTime;
                }
            }
        }
        private int _startTime = 0;

        /// <summary>
        /// 소요 시간
        /// </summary>
        public int requireTime = 0;

        /// <summary>
        /// 완료 시간
        /// </summary>
        public int endTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                if (value > 0)
                {
                    int nowTime = TimeUtil.GetCurrentSec();
                    // 자율작전
                    if (autoMissionId > 0 && autoMissionNumber > 0 && requireTime > 0)
                    {
                        int number = autoMissionNumber - (value - nowTime / requireTime) - 1;
                        this.TBNumber = string.Format("{0}/{1}", number, autoMissionNumber);
                    }
                    this.TBRemainTime = TimeUtil.GetRemainHHMMSS(value);
                    this.TBEndTime = TimeUtil.GetDateTime(value, "MM-dd HH:mm");
                    if (nowTime > value - Config.Extra.earlyNotifySeconds)
                        this.notified = true;

                    //long now = Parser.Time.GetCurrentMs();
                    //// 자율작전
                    //if (autoMissionId > 0 && autoMissionNumber > 0 && requireTime > 0)
                    //{
                    //    int number = autoMissionNumber - Convert.ToInt32((value - now) / requireTime) - 1;
                    //    this.TBNumber = string.Format("{0}/{1}", number, autoMissionNumber);
                    //}
                    //this.TBRemainTime = Parser.Time.GetRemainHHMMSS(value);
                    //this.TBEndTime = Parser.Time.GetDateTime(value).ToString("MM-dd HH:mm");
                    //if (now > value - Config.Extra.earlyNotifyMiliseconds)
                    //    notified = true;
                }
            }
        }
        private int _endTime = 0;

        /// <summary>
        /// 마지막 목록 여부
        /// </summary>
        public bool isLast
        {
            get { return _isLast; }
            set
            {
                _isLast = value;
                OnPropertyChanged();
            }
        }
        private bool _isLast = false;

        /// <summary>
        /// 알림 여부
        /// </summary>
        public bool notified { get; set; } = false;

        #endregion

        // ==============================================
        // ===== TextBlock
        // ==============================================
        #region TextBlock

        /// <summary>
        /// 슬롯
        /// </summary>
        public string TBSlot
        {
            get { return _TBSlot; }
            set
            {
                _TBSlot = value;
                OnPropertyChanged();
            }
        }
        private string _TBSlot = "";

        /// <summary>
        /// 지원코드
        /// (Format: 0-2)
        /// </summary>
        public string TBCode
        {
            get { return _TBCode; }
            set
            {
                _TBCode = value;
                OnPropertyChanged();
            }
        }
        private string _TBCode = "";

        /// <summary>
        /// 지원종류
        /// (Format: 군수지원)
        /// </summary>
        public string TBType
        {
            get { return _TBType; }
            set
            {
                _TBType = value;
                OnPropertyChanged();
            }
        }
        private string _TBType = "";

        /// <summary>
        /// 자율횟수
        /// (Format: N/N)
        /// </summary>
        public string TBNumber
        {
            get { return _TBNumber; }
            set
            {
                _TBNumber = value;
                OnPropertyChanged();
            }
        }
        private string _TBNumber = "";

        /// <summary>
        /// 남은 시간
        /// </summary>
        public string TBRemainTime
        {
            get { return _TBRemainTime; }
            set
            {
                _TBRemainTime = value;
                OnPropertyChanged();
            }
        }
        private string _TBRemainTime = "00:00:00";

        /// <summary>
        /// 완료 시간
        /// </summary>
        public string TBEndTime
        {
            get { return _TBEndTime; }
            set
            {
                _TBEndTime = value;
                OnPropertyChanged();
            }
        }
        private string _TBEndTime = "00-00 00:00";

        /// <summary>
        /// 투입 제대
        /// </summary>
        public string TBTeamIds
        {
            get { return _TBTeamIds; }
            set
            {
                _TBTeamIds = value;
                OnPropertyChanged();
            }
        }
        private string _TBTeamIds = "";

        /// <summary>
        /// 획득 자원
        /// </summary>
        public int[] rewardResource
        {
            get { return _rewardResource; }
            set
            {
                _rewardResource = value;
                OnPropertyChanged();
            }
        }
        private int[] _rewardResource = new int[] { 0, 0, 0, 0 };

        /// <summary>
        /// 시간당 획득자원
        /// </summary>
        public int[] rewardResourcePerHour
        {
            get { return _rewardResourcePerHour; }
            set
            {
                _rewardResourcePerHour = value;
                OnPropertyChanged();
            }
        }
        private int[] _rewardResourcePerHour = new int[] { 0, 0, 0, 0 };

        /// <summary>
        /// 획득 아이템
        /// </summary>
        public string[] rewardItemIcon
        {
            get { return _rewardItemIcon; }
            set
            {
                _rewardItemIcon = value;
                OnPropertyChanged();
            }
        }
        private string[] _rewardItemIcon = new string[] { "", "" };
        public int[] rewardItemCount
        {
            get { return _rewardItemCount; }
            set
            {
                _rewardItemCount = value;
                OnPropertyChanged();
            }
        }
        private int[] _rewardItemCount = new int[] { 0, 0 };

        #endregion

        // ==============================================
        // ===== Function
        // ==============================================
        #region Function

        /// <summary>
        /// 이미지 경로
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string GetItemUri(string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return "";
            }
            else
            {
                return string.Format("/GFAlarm;component/Resource/image/icon/item/{0}.png", item);
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}
