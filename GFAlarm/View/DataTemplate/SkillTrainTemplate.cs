using GFAlarm.Data;
using GFAlarm.Util;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GFAlarm.View.DataTemplate
{
    /// <summary>
    /// Skill Training Template
    /// </summary>
    public class SkillTrainTemplate : INotifyPropertyChanged
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // ==============================================
        // ===== Skill Train
        // ==============================================
        #region Skill Train

        /// <summary>
        /// 인형 ID
        /// </summary>
        public long gunWithUserId
        {
            get
            {
                return _gunWithUserId;
            }
            set
            {
                _gunWithUserId = value;
                if (value > 0)
                {
                    gunName = UserData.Doll.GetName(value);
                    fromSkillLevel = UserData.Doll.GetSkillLevel(value, skill);
                    if (fromSkillLevel < 1)
                    {
                        fromSkillLevel = 1;
                    }

                    this.TBTitle = gunName;
                }
            }
        }
        private long _gunWithUserId = 0;

        /// <summary>
        /// 인형 도감번호
        /// </summary>
        public int gunId = 0;

        /// <summary>
        /// 인형 이름
        /// </summary>
        public string gunName = "";

        /// <summary>
        /// 요정 ID
        /// </summary>
        public long fairyWithUserId
        {
            get
            {
                return _fairyWithUserId;
            }
            set
            {
                _fairyWithUserId = value;
                if (value > 0)
                {
                    fairyName = UserData.Fairy.GetName(value);
                    fromSkillLevel = UserData.Fairy.GetSkillLv(value);
                    if (fromSkillLevel < 1)
                    {
                        fromSkillLevel = 1;
                    }

                    this.TBTitle = fairyName;
                }
            }
        }
        private long _fairyWithUserId = 0;

        /// <summary>
        /// 요정 이름
        /// </summary>
        public string fairyName = "";

        /// <summary>
        /// 중장비부대 ID
        /// </summary>
        public long squadWithUserId
        {
            get
            {
                return _squadWithUserId;
            }
            set
            {
                _squadWithUserId = value;
                if (value > 0)
                {
                    this.slotType = 1;

                    // 중장비부대 이름
                    squadName = UserData.Squad.GetName(value);
                    // 스킬
                    fromSkillLevel = UserData.Squad.GetSkillLevel(value, skill);
                    if (fromSkillLevel < 1)
                    {
                        fromSkillLevel = 1;
                    }

                    this.TBTitle = squadName;
                }
            }
        }
        private long _squadWithUserId = 0;

        /// <summary>
        /// 중장비부대 이름
        /// </summary>
        public string squadName = "";

        /// <summary>
        /// 스킬 종류 
        /// (1: 1번 스킬, 2: 2번 스킬, 3: 3번 스킬)
        /// </summary>
        public int skill = 0;

        /// <summary>
        /// 기존 스킬 레벨
        /// </summary>
        public int fromSkillLevel
        {
            get
            {
                return _fromSkillLevel;
            }
            set
            {
                _fromSkillLevel = value;
                if (value != 0)
                {
                    toSkillLevel = value + 1;

                    this.TBSubtitle1 = value.ToString();
                }
            }
        }
        private int _fromSkillLevel = 0;

        /// <summary>
        /// 상승 스킬 레벨
        /// </summary>
        public int toSkillLevel
        {
            get
            {
                return _toSkillLevel;
            }
            set
            {
                _toSkillLevel = value;
                if (value != 0)
                {
                    if (gunWithUserId != 0)
                    {
                        // TODO: 스킬훈련시간 수정 (밀리초 => 초)
                        requireTime = GameData.Doll.GetDollSkillTrainTime(value);
                    }
                    else if (fairyWithUserId != 0)
                    {
                        requireTime = GameData.Fairy.GetFairySkillTrainTime(value);
                    }
                    else if (squadWithUserId != 0)
                    {
                        requireTime = GameData.Squad.GetSquadSkillTrainTime(this.skill, value);
                    }

                    this.TBSubtitle2 = value.ToString();
                }
            }
        }
        private int _toSkillLevel = 0;

        /// <summary>
        /// 경험훈련 여부
        /// </summary>
        public bool isExpTrain
        {
            get { return _isExpTrain; }
            set
            {
                _isExpTrain = value;
                OnPropertyChanged();
            }
        }
        private bool _isExpTrain = false;

        /// <summary>
        /// 획득 경험치
        /// </summary>
        public long addExp
        {
            get { return _addExp; }
            set
            {
                _addExp = value;
                if (value > 0)
                {
                    isExpTrain = true;
                }
            }
        }
        private long _addExp = 0;

        /// <summary>
        /// 특수작전보고서 갯수
        /// </summary>
        public int reportNum
        {
            get { return _reportNum; }
            set
            {
                _reportNum = value;
                if (value > 0)
                {
                    isExpTrain = true;
                }
            }
        }
        private int _reportNum = 0;

        #endregion

        // ==============================================
        // ===== Common
        // ==============================================
        #region Common

        /// <summary>
        /// 슬롯
        /// </summary>
        public int slot
        {
            get
            {
                return _slot;
            }
            set
            {
                _slot = value;
                if (value > 0)
                {
                    this.TBSlot = value.ToString();
                }
            }
        }
        private int _slot = 0;

        /// <summary>
        /// 슬롯 종류
        /// (0: 일반, 1: 중장비부대) 
        /// </summary>
        public int slotType
        {
            get
            {
                return _slotType;
            }
            set
            {
                _slotType = value;
                OnPropertyChanged();
            }
        }
        private int _slotType = 0;

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
                if (value != 0 && requireTime != 0)
                {
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
                    //this.TBRemainTime = Parser.Time.GetRemainHHMMSS(value);
                    //this.TBEndTime = Parser.Time.GetDateTime(value).ToString("MM-dd HH:mm");
                    //if (Parser.Time.GetCurrentMs() > value - Config.Extra.earlyNotifyMiliseconds)
                    //{
                    //    notified = true;
                    //}
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

        // 알림 여부
        public bool notified = false;

        #endregion

        // ==============================================
        // ===== TextBlock
        // ==============================================
        #region TextBlock

        /// <summary>
        /// 훈련 슬롯
        /// </summary>
        public string TBSlot
        {
            get
            {
                return _TBSlot;
            }
            set
            {
                _TBSlot = value;
                OnPropertyChanged();
            }
        }
        private string _TBSlot = "";

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
        private string _TBRemainTime = "00:00:00";

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
        private string _TBEndTime = "00-00 00:00";

        /// <summary>
        /// 인형/요정/중장비부대 이름
        /// </summary>
        public string TBTitle
        {
            get { return _TBTitle; }
            set
            {
                _TBTitle = value;
                OnPropertyChanged();
            }
        }
        private string _TBTitle = "";

        /// <summary>
        /// 스킬 레벨 이전
        /// </summary>
        public string TBSubtitle1
        {
            get { return _TBSubtitle1; }
            set
            {
                _TBSubtitle1 = value;
                OnPropertyChanged();
            }
        }
        private string _TBSubtitle1 = "";

        /// <summary>
        /// 스킬 레벨 이후
        /// </summary>
        public string TBSubtitle2
        {
            get { return _TBSubtitle2; }
            set
            {
                _TBSubtitle2 = value;
                OnPropertyChanged();
            }
        }
        private string _TBSubtitle2 = "";

        #endregion
    }
}
