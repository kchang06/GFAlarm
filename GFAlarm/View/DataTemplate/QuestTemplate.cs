using GFAlarm.Data;
using GFAlarm.Util;
using LocalizationResources;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.View.DataTemplate
{
    /// <summary>
    /// Quest Template
    /// </summary>
    public class QuestTemplate : INotifyPropertyChanged
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // ==============================================
        // ===== Quest
        // ==============================================
        #region Quest

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
        /// 획득 아이템 아이콘
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
        private string[] _rewardItemIcon = new string[] { "", "", "", "", };

        /// <summary>
        /// 획득 아이템 갯수
        /// </summary>
        public int[] rewardItemCount
        {
            get { return _rewardItemCount; }
            set
            {
                _rewardItemCount = value;
                OnPropertyChanged();
            }
        }
        private int[] _rewardItemCount = new int[] { 0, 0, 0, 0 };

        /// <summary>
        /// 임무 ID
        /// </summary>
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                if (value > 0)
                {
                    JObject data = GameData.Quest.GetData(value);
                    if (data != null)
                    {
                        this.TBQuestId = Parser.Json.ParseString(data["no"]);
                        this.TBQuestTitle = LanguageResources.Instance[string.Format("QUEST_{0}", Parser.Json.ParseInt(data["id"]))];
                        //this.tbQuestTitle = Parser.Json.ParseString(data["name"]);
                        this.cycle = Parser.Json.ParseString(data["cycle"]);
                        this.type = Parser.Json.ParseString(data["type"]);
                        this.startCount = Parser.Json.ParseInt(data["start_count"]);
                        this.maxCount = Parser.Json.ParseInt(data["count"]);

                        switch (this.cycle)
                        {
                            case "daily":
                                TBQuestCycle = LanguageResources.Instance["DAILY_QUEST"];
                                break;
                            case "weekly":
                                TBQuestCycle = LanguageResources.Instance["WEEKLY_QUEST"];
                                break;
                            case "research":
                                TBQuestCycle = LanguageResources.Instance["RESEARCH_QUEST"];
                                break;
                        }

                        try
                        {
                            int[] resources = Parser.Json.ParseString(data["resources"]).Split(',').Select(Int32.Parse).ToArray();
                            if (resources.Length == 4)
                                this.rewardResource = resources;
                            //OnPropertyChanged("rewardResource");
                        }
                        catch (Exception ex) { log.Error(ex, "임무 자원 파싱 중 에러"); }

                        try
                        {
                            string[] items = Parser.Json.ParseString(data["items"]).Split(',');
                            if (items.Length >= 8)
                            {
                                this.rewardItemIcon[3] = GetItemUri(items[6]);
                                this.rewardItemCount[3] = Parser.String.ParseInt(items[7]);
                            }
                            if (items.Length >= 6)
                            {
                                this.rewardItemIcon[2] = GetItemUri(items[4]);
                                this.rewardItemCount[2] = Parser.String.ParseInt(items[5]);
                            }
                            if (items.Length >= 4)
                            {
                                this.rewardItemIcon[1] = GetItemUri(items[2]);
                                this.rewardItemCount[1] = Parser.String.ParseInt(items[3]);
                            }
                            if (items.Length >= 2)
                            {
                                this.rewardItemIcon[0] = GetItemUri(items[0]);
                                this.rewardItemCount[0] = Parser.String.ParseInt(items[1]);
                            }
                            OnPropertyChanged("rewardItemIcon");
                            OnPropertyChanged("rewardItemCount");
                        }
                        catch(Exception ex) { log.Error(ex, "임무 아이템 파싱 중 에러"); }
                    }
                }
            }
        }
        private int _id = 0;

        /// <summary>
        /// 임무 주기
        /// </summary>
        public string cycle { get; set; } = "";

        /// <summary>
        /// 임무 종류
        /// </summary>
        public string type { get; set; } = "";

        /// <summary>
        /// 임무 시작 횟수
        /// </summary>
        public int startCount { get; set; } = 0;

        /// <summary>
        /// 임무 횟수
        /// </summary>
        public int count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                this.TBQuestCount = "0";
                this.TBIsCompleted = "0";
                if (value - startCount > 0)
                {
                    int count = value - startCount;
                    if (count >= this.maxCount)
                    {
                        count = this.maxCount;
                        this.TBIsCompleted = "1";
                    }
                    this.TBQuestCount = count.ToString();
                }
            }
        }
        private int _count = 0;

        /// <summary>
        /// 임무 최대 횟수
        /// </summary>
        public int maxCount
        {
            get
            {
                return _maxCount;
            }
            set
            {
                _maxCount = value;
                if (value > 0)
                {
                    this.TBQuestMaxCount = value.ToString();
                }
            }
        }
        private int _maxCount = 0;

        #endregion

        // ==============================================
        // ===== TextBlock
        // ==============================================
        #region TextBlock

        /// <summary>
        /// 임무 번호
        /// </summary>
        public string TBQuestId
        {
            get
            {
                return _TBQuestId;
            }
            set
            {
                _TBQuestId = value.PadLeft(2, '0');
                OnPropertyChanged();
            }
        }
        private string _TBQuestId = "";

        /// <summary>
        /// 임무 목표
        /// </summary>
        public string TBQuestTitle
        {
            get
            {
                return _TBQuestTitle;
            }
            set
            {
                _TBQuestTitle = value;
                OnPropertyChanged();
            }
        }
        private string _TBQuestTitle = "";

        /// <summary>
        /// 임무 주기
        /// </summary>
        public string TBQuestCycle
        {
            get
            {
                return _TBQuestCycle;
            }
            set
            {
                _TBQuestCycle = value;
                OnPropertyChanged();
            }
        }
        private string _TBQuestCycle = "";

        /// <summary>
        /// 임무 횟수
        /// </summary>
        public string TBQuestCount
        {
            get
            {
                return _TBQuestCount;
            }
            set
            {
                _TBQuestCount = value;
                OnPropertyChanged();
            }
        }
        private string _TBQuestCount = "0";

        /// <summary>
        /// 임무 최대 횟수
        /// </summary>
        public string TBQuestMaxCount
        {
            get
            {
                return _TBQuestMaxCount;
            }
            set
            {
                _TBQuestMaxCount = value;
                OnPropertyChanged();
            }
        }
        private string _TBQuestMaxCount = "0";

        /// <summary>
        /// 임무 완료 여부
        /// </summary>
        public string TBIsCompleted
        {
            get
            {
                return _TBIsCompleted;
            }
            set
            {
                _TBIsCompleted = value;
                OnPropertyChanged();
            }
        }
        private string _TBIsCompleted = "0";

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

        #endregion
    }
}
