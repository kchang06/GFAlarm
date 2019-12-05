using GFAlarm.Data;
using GFAlarm.Util;
using LocalizationResources;
using Newtonsoft.Json.Linq;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GFAlarm.View.DataTemplate
{
    /// <summary>
    /// Doll Production Template
    /// </summary>
    public class ProduceDollTemplate : INotifyPropertyChanged
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // ==============================================
        // ===== Doll Production
        // ==============================================
        #region Doll Production

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
                if (value != 0)
                {
                    TBSlot = (value / 2 + value % 2).ToString();
                    TBSlotType = value % 2 == 0 ? "heavy" : "normal";
                }
            }
        }
        private int _slot = 0;

        /// <summary>
        /// 인형 도감번호
        /// </summary>
        public int gunId
        {
            get
            {
                return _gunId;
            }
            set
            {
                _gunId = value;
                if (value != 0)
                {
                    JObject dollData = GameData.Doll.GetDollData(value);
                    if (dollData != null)
                    {
                        gunName = LanguageResources.Instance[string.Format("DOLL_{0}", value)];
                        category = Parser.Json.ParseString(dollData["type"]);
                        star = Parser.Json.ParseInt(dollData["star"]);
                        requireTime = TimeUtil.ParseHHMM(Parser.Json.ParseString(dollData["require_time"]));
                        //requireTime = Parser.String.ParseHHMM(Parser.Json.ParseString(dollData["require_time"]));
                    }
                }
            }
        }
        private int _gunId = 0;

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
                this.TBName = value;
            }
        }
        private string _gunName = "";

        /// <summary>
        /// 인형 병과 
        /// </summary>
        public string category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
                this.TBCategory = value;
            }
        }
        private string _category = "";

        /// <summary>
        /// 인형 레어도
        /// </summary>
        public int star
        {
            get
            {
                return _star;
            }
            set
            {
                _star = value;
                this.TBStar = "";
                for (int i = 0; i < value; i++)
                {
                    this.TBStar += "★";
                }
            }
        }
        private int _star = 0;

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
        public int requireTime { get; set; } = 0;

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
                if (value != 0)
                {
                    this.TBRemainTime = TimeUtil.GetRemainHHMMSS(value);
                    this.TBEndTime = TimeUtil.GetDateTime(value, "MM-dd HH:mm");
                    if (TimeUtil.GetCurrentSec() > value - Config.Extra.earlyNotifySeconds)
                        notified = true;
                    //this.TBRemainTime = Parser.Time.GetRemainHHMMSS(value);
                    //this.TBEndTime = Parser.Time.GetDateTime(value).ToString("MM-dd HH:mm");
                    //if (Parser.Time.GetCurrentMs() > value - Config.Extra.earlyNotifyMiliseconds)
                    //    notified = true;
                }
            }
        }
        private int _endTime = 0;

        /// <summary>
        /// 투입 자원
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
        private int[] _spendResource = new int[] { 0, 0, 0, 0 };

        /// <summary>
        /// 투입 아이템
        /// </summary>
        public int[] spendItem
        {
            get { return _spendItem; }
            set
            {
                _spendItem = value;
                OnPropertyChanged();
            }
        }
        private int[] _spendItem = new int[] { 0, 0 };

        /// <summary>
        /// 투입 레벨
        /// </summary>
        public int inputLevel
        {
            get { return _inputLevel; }
            set
            {
                _inputLevel = value;
                switch (value)
                {
                    case 0:     // 일반
                        spendItem[0] = 1; // 인형제조계약
                        spendItem[1] = 0; // 코어
                        break;
                    case 1:     // 저투입
                        spendItem[0] = 1; // 인형제조계약
                        spendItem[1] = 3; // 코어
                        break;
                    case 2:     // 중투입
                        spendItem[0] = 20; // 인형제조계약
                        spendItem[1] = 5; // 코어
                        break;
                    case 3:     // 고투입
                        spendItem[0] = 50; // 인형제조계약
                        spendItem[1] = 10; // 코어
                        break;
                }
            }
        }
        private int _inputLevel = 0;

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
        /// 슬롯 종류
        /// (heavy: 중형제조, normal: 일반제조)
        /// </summary>
        public string TBSlotType
        {
            get
            {
                return _TBSlotType;
            }
            set
            {
                _TBSlotType = value;
                OnPropertyChanged();
            }
        }
        private string _TBSlotType = "";

        /// <summary>
        /// 인형 이름
        /// </summary>
        public string TBName
        {
            get
            {
                return _TBName;
            }
            set
            {
                _TBName = value;
                OnPropertyChanged();
            }
        }
        private string _TBName = "";

        /// <summary>
        /// 인형 병과
        /// (HG, SMG, AR, RF, MG, SG)
        /// </summary>
        public string TBCategory
        {
            get
            {
                return _TBCategory;
            }
            set
            {
                _TBCategory = value;
                OnPropertyChanged();
            }
        }
        private string _TBCategory = "";

        /// <summary>
        /// 인형 레어도
        /// (★★★★★)
        /// </summary>
        public string TBStar
        {
            get
            {
                return _TBStar;
            }
            set
            {
                _TBStar = value;
                OnPropertyChanged();
            }
        }
        private string _TBStar = "";
        
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

        #endregion

        // ==============================================
        // ===== Initializer
        // ==============================================
        #region Initializer

        public ProduceDollTemplate() { }

        #endregion
    }
}
