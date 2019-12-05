using GFAlarm.Data;
using GFAlarm.Util;
using LocalizationResources;
using NLog;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GFAlarm.View.DataTemplate
{
    /// <summary>
    /// Explore Template
    /// </summary>
    public class ExploreTemplate : INotifyPropertyChanged
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // ==============================================
        // ===== Explore
        // ==============================================
        #region Explore

        /// <summary>
        /// 아이디
        /// </summary>
        public int id { get; set; } = 1;

        /// <summary>
        /// 탐색 지역
        /// </summary>
        public int areaId
        {
            get { return _areaId; }
            set
            {
                _areaId = value;
                switch (value)
                {
                    case 1:
                        TBAreaName = LanguageResources.Instance["EXPLORE_CITY"];
                        break;
                    case 2:
                        TBAreaName = LanguageResources.Instance["EXPLORE_SNOW_LAND"];
                        break;
                    case 3:
                        TBAreaName = LanguageResources.Instance["EXPLORE_FOREST"];
                        break;
                    case 4:
                        TBAreaName = LanguageResources.Instance["EXPLORE_WASTE_LAND"];
                        break;
                }
            }
        }
        private int _areaId = 0;

        /// <summary>
        /// 탐색 목표
        /// </summary>
        public int targetId
        {
            get
            {
                return _targetId;
            }
            set
            {
                _targetId = value;
            }
        }
        private int _targetId = 0;

        /// <summary>
        /// 휴대물품
        /// </summary>
        public int itemId
        {
            get
            {
                return _itemId;
            }
            set
            {
                _itemId = value;
            }
        }
        private int _itemId = 0;

        /// <summary>
        /// 우세인형
        /// </summary>
        public int[] adaptiveGunIds
        {
            get
            {
                return _adaptiveGunIds;
            }
            set
            {
                _adaptiveGunIds = value;
                RefreshReward();
            }
        }
        private int[] _adaptiveGunIds = new int[] { };

        /// <summary>
        /// 우세동물
        /// </summary>
        public int[] adaptivePetIds
        {
            get
            {
                return _adaptivePetIds;
            }
            set
            {
                _adaptivePetIds = value;
                RefreshReward();
            }
        }
        private int[] _adaptivePetIds = new int[] { };

        /// <summary>
        /// 투입인형 (인형 ID)
        /// </summary>
        public int[] gunIds
        {
            get
            {
                return _gunIds;
            }
            set
            {
                _gunIds = value;
                RefreshReward();
            }
        }
        private int[] _gunIds = new int[] { };

        /// <summary>
        /// 투입인형 (인형 고유 ID)
        /// </summary>
        public long[] guns
        {
            get
            {
                return _guns;
            }
            set
            {
                _guns = value;
                RefreshReward();
            }
        }
        private long[] _guns = new long[] { };

        /// <summary>
        /// 투입동물
        /// </summary>
        public long[] pets
        {
            get
            {
                return _pets;
            }
            set
            {
                _pets = value;
                RefreshReward();
            }
        }
        private long[] _pets = new long[] { };

        /// <summary>
        /// 우세인형 인원
        /// </summary>
        public int adaptiveDollCount { get; set; } = 0;

        /// <summary>
        /// 우세동물 마리
        /// </summary>
        public int adaptivePetCount { get; set; } = 0;

        /// <summary>
        /// 인형 레벨 합
        /// </summary>
        public int totalDollLevel { get; set; } = 0;

        /// <summary>
        /// 시작 시간
        /// </summary>
        public int startTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                if (value > 0)
                {

                }
            }
        }
        private int _startTime = 0;

        /// <summary>
        /// 완료 시간
        /// </summary>
        public int endTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
                if (value > 0)
                {
                    this.TBRemainTime = TimeUtil.GetRemainHHMMSS(value);
                    this.TBEndTime = TimeUtil.GetDateTime(value, "MM-dd HH:mm");
                    if (TimeUtil.GetCurrentSec() > value - Config.Extra.earlyNotifySeconds)
                        notified = true;
                    //TBRemainTime = Parser.Time.GetRemainHHMMSS(value);
                    //TBEndTime = Parser.Time.GetDateTime(this.endTime).ToString("MM-dd HH:mm");
                    //if (Parser.Time.GetCurrentMs() > value - Config.Extra.earlyNotifyMiliseconds)
                    //{
                    //    notified = true;
                    //}
                }
            }
        }
        private int _endTime = 0;

        /// <summary>
        /// 취소 시간
        /// </summary>
        public int cancelTime
        {
            get
            {
                return _cancelTime;
            }
            set
            {
                _cancelTime = value;
                if (value > 0)
                {

                }
            }
        }
        private int _cancelTime = 0;

        /// <summary>
        /// 다음 발견 남은 시간
        /// </summary>
        public int nextTime
        {
            get
            {
                return _nextTime;
            }
            set
            {
                _nextTime = value;
                if (value > 0)
                {
                    this.TBEventRemainTime = TimeUtil.GetRemainHHMMSS(value);
                    //TBEventRemainTime = Parser.Time.GetRemainHHMMSS(nextTime);
                }
            }
        }
        private int _nextTime = 0;

        /// <summary>
        /// 자동 출발 시간
        /// </summary>
        public int nextExploreTime
        {
            get
            {
                return _nextExploreTime;
            }
            set
            {
                _nextExploreTime = value;
                if (value > 0)
                {

                }
            }
        }
        private int _nextExploreTime = 0;

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
        /// 탐색 지역
        /// </summary>
        public string TBAreaName
        {
            get
            {
                return _TBAreaName;
            }
            set
            {
                _TBAreaName = value;
                OnPropertyChanged();
            }
        }
        private string _TBAreaName = "";

        /// <summary>
        /// 다음 이벤트 남은 시간
        /// </summary>
        public string TBEventRemainTime
        {
            get
            {
                return _TBEventRemainTime;
            }
            set
            {
                _TBEventRemainTime = value;
                OnPropertyChanged();
            }
        }
        private string _TBEventRemainTime = "";

        /// <summary>
        /// 남은 시간
        /// </summary>
        public string TBRemainTime
        {
            get
            {
                return _TBRemainTime;
            }
            set
            {
                _TBRemainTime = value;
                OnPropertyChanged();
            }
        }
        private string _TBRemainTime = "";

        /// <summary>
        /// 완료 시간
        /// </summary>
        public string TBEndTime
        {
            get
            {
                return _TBEndTime;
            }
            set
            {
                _TBEndTime = value;
                OnPropertyChanged();
            }
        }
        private string _TBEndTime = "";

        /// <summary>
        /// 메인 보상 갯수
        /// </summary>
        public string TBMainRewardCount
        {
            get
            {
                return _TBMainRewardCount;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _TBMainRewardCount = value;
                }
                else
                {
                    _TBMainRewardCount = "";
                }
                OnPropertyChanged();
            }
        }
        private string _TBMainRewardCount = "";

        /// <summary>
        /// 메인 보상 아이콘
        /// </summary>
        public string TBMainRewardIcon
        {
            get
            {
                return _TBMainRewardIcon;
            }
            set
            {
                _TBMainRewardIcon = GetItemUri(value);
                OnPropertyChanged();
            }
        }
        private string _TBMainRewardIcon = "";

        /// <summary>
        /// 서브 보상 갯수
        /// </summary>
        public string TBSubRewardCount
        {
            get
            {
                return _TBSubRewardCount;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _TBSubRewardCount = value;
                }
                else
                {
                    _TBSubRewardCount = "";
                }
                OnPropertyChanged();
            }
        }
        private string _TBSubRewardCount = "";

        /// <summary>
        /// 서브 보상 1 아이콘
        /// </summary>
        public string TBSubReward1Icon
        {
            get
            {
                return _TBSubReward1Icon;
            }
            set
            {
                _TBSubReward1Icon = GetItemUri(value);
                OnPropertyChanged();
            }
        }
        private string _TBSubReward1Icon = "";

        /// <summary>
        /// 서브 보상 2 아이콘
        /// </summary>
        public string TBSubReward2Icon
        {
            get
            {
                return _TBSubReward2Icon;
            }
            set
            {
                _TBSubReward2Icon = GetItemUri(value);
                OnPropertyChanged();
            }
        }
        private string _TBSubReward2Icon = "";

        #endregion

        // ==============================================
        // ===== Function
        // ==============================================
        #region Function

        /// <summary>
        /// 탐색보상 새로고침
        /// </summary>
        public void RefreshReward()
        {
            log.Debug("탐색 새로고침");

            if (targetId == 0)
            {
                log.Warn("no target_id");
                return;
            }
            if (areaId == 0)
            {
                log.Warn("no area_id");
                return;
            }

            if (UserData.Doll.exploreTeam.Count() > 0 && UserData.Doll.exploreAdaptiveDolls.Count() > 0)
            {
                this.adaptiveDollCount = UserData.Doll.GetExploreAdaptiveDollCount();
                this.totalDollLevel = UserData.Doll.GetExploreDollTotalLevel();
            }
            else
            {
                log.Debug("no explorer team info");
                return;
            }

            log.Debug("adaptive doll count={0}", this.adaptiveDollCount);
            log.Debug("team sum level {0}", this.totalDollLevel);

            switch (areaId)
            {
                case 1: // 도시
                    TBMainRewardIcon = "explore_loot_city";
                    TBSubReward1Icon = "explore_loot_snowland";
                    TBSubReward2Icon = "explore_loot_wilderness";
                    break;
                case 2: // 설원
                    TBMainRewardIcon = "explore_loot_snowland";
                    TBSubReward1Icon = "explore_loot_city";
                    TBSubReward2Icon = "explore_loot_forest";
                    break;
                case 3: // 숲
                    TBMainRewardIcon = "explore_loot_forest";
                    TBSubReward1Icon = "explore_loot_snowland";
                    TBSubReward2Icon = "explore_loot_wilderness";
                    break;
                case 4: // 황무지
                    TBMainRewardIcon = "explore_loot_wilderness";
                    TBSubReward1Icon = "explore_loot_city";
                    TBSubReward2Icon = "explore_loot_forest";
                    break;
            }

            int modTargetId = targetId % 20;
            if (modTargetId == 0)
                modTargetId = 20;

            int[] reward = GameData.Explore.GetReward(modTargetId);

            double itemRatio = 0.0;
            switch (this.itemId)
            {
                case 6007:
                    itemRatio = 0.1;
                    break;
                case 6008:
                    itemRatio = 0.2;
                    break;
                case 6009:
                    itemRatio = 0.3;
                    break;
            }

            double mainReward = Math.Truncate(reward[0] * (1 + 0.08 * adaptiveDollCount + 0.048 * totalDollLevel / 120) * (1.0 + itemRatio));
            double subReward = Math.Truncate(reward[1] * (1 + 0.08 * adaptiveDollCount + 0.048 * totalDollLevel / 120) * (1.0 + itemRatio));

            TBMainRewardCount = mainReward.ToString();
            TBSubRewardCount = subReward.ToString();
        }

        /// <summary>
        /// 이미지 경로
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetItemUri(string item)
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
