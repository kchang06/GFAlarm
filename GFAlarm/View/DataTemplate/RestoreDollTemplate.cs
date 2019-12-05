using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Util;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GFAlarm.View.DataTemplate
{
    public class RestoreDollTemplate : INotifyPropertyChanged
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // ==============================================
        // ===== Restore Doll
        // ==============================================
        #region Restore Doll

        /// <summary>
        /// 수복 슬롯
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
                    TBSlot = value.ToString();
                }
            }
        }
        private int _slot = 0;

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
                    DollWithUserInfo doll = UserData.Doll.Get(value);
                    if (doll != null)
                    {
                        gunId = doll.no;
                        gunName = doll.name;
                        TBSubtitle = doll.type;
                    }
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
        public string gunName
        {
            get
            {
                return _gunName;
            }
            set
            {
                _gunName = value;
                if (!string.IsNullOrEmpty(value))
                {
                    TBTitle = value;
                }
                else
                {
                    TBTitle = "UnknownDoll";
                }
            }
        }
        private string _gunName = "";

        /// <summary>
        /// 인형 병과
        /// </summary>
        public string gunType
        {
            get
            {
                return _gunType;
            }
            set
            {
                _gunType = value;
                if (!string.IsNullOrEmpty(value))
                {
                    TBSubtitle = value;
                }
                else
                {
                    TBSubtitle = "UnknownCategory";
                }
            }
        }
        private string _gunType = "";

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
                if (value > 0 && requireTime > 0)
                {
                    endTime = value + requireTime;
                }
            }
        }
        private int _startTime = 0;

        /// <summary>
        /// 필요 시간
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
        public bool notified = false;

        #endregion

        // ==============================================
        // ===== TextBlock
        // ==============================================
        #region TextBlock

        /// <summary>
        /// 수복 슬롯
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
        /// 제목
        /// 인형 이름, 중장비 이름
        /// </summary>
        public string TBTitle
        {
            get
            {
                return _TBTitle;
            }
            set
            {
                _TBTitle = value;
                OnPropertyChanged();
            }
        }
        private string _TBTitle = "";

        /// <summary>
        /// 부제목
        /// 인형 종류, 중장비 종류
        /// </summary>
        public string TBSubtitle
        {
            get
            {
                return _TBSubtitle;
            }
            set
            {
                _TBSubtitle = value;
                OnPropertyChanged();
            }
        }
        private string _TBSubtitle = "";

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
        private string _TBRemainTime = "--:--:--";

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
        private string _TBEndTime = "----- --:--";

        #endregion

        // ==============================================
        // ===== Initializer
        // ==============================================
        #region Initializer

        public RestoreDollTemplate()
        {

        }

        #endregion
    }
}
