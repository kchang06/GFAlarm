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
    /// Equip Production Template
    /// </summary>
    public class ProduceEquipTemplate : INotifyPropertyChanged
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // ==============================================
        // ===== Equip
        // ==============================================
        #region Equip

        /// <summary>
        /// 장비 도감번호
        /// </summary>
        public int equipId
        {
            get
            {
                return _equipId;
            }
            set
            {
                _equipId = value;
                if (value != 0)
                {
                    this.TBName = "UnknownEquip";
                    type = "장비";

                    JObject equipData = GameData.Equip.GetData(value);
                    if (equipData != null)
                    {
                        equipName = LanguageResources.Instance[string.Format("EQUIP_{0}", value)];
                        equipNameShort = LanguageResources.Instance[string.Format("EQUIP_TYPE_{0}", value)];
                        category = Parser.Json.ParseString(equipData["category"]);
                        star = Parser.Json.ParseInt(equipData["star"]);
                        requireTime = TimeUtil.ParseHHMM(Parser.Json.ParseString(equipData["require_time"]));
                        //requireTime = Parser.String.ParseHHMM(Parser.Json.ParseString(equipData["require_time"]));

                        this.TBName = equipNameShort;
                    }

                    this.TBCategory = "장비";
                    this.TBSubtitle1 = "";
                }
            }
        }
        private int _equipId = 0;

        /// <summary>
        /// 장비 명
        /// </summary>
        public string equipName { get; set; } = "";

        /// <summary>
        /// 장비 명 (약식)
        /// </summary>
        public string equipNameShort { get; set; } = "";

        #endregion

        // ==============================================
        // ===== Fairy
        // ==============================================
        #region Fairy

        /// <summary>
        /// 요정 도감번호
        /// </summary>
        public int fairyId
        {
            get
            {
                return _fairyId;
            }
            set
            {
                _fairyId = value;
                if (value > 0)
                {
                    this.TBName = "Unknown";
                    type = "요정";

                    JObject fairyData = GameData.Fairy.GetData(value);
                    if (fairyData != null)
                    {
                        fairyName = LanguageResources.Instance[string.Format("FAIRY_{0}", value)];
                        category = Parser.Json.ParseString(fairyData["category"]);
                        star = Parser.Json.ParseInt(fairyData["star"]);
                        requireTime = TimeUtil.ParseHHMM(Parser.Json.ParseString(fairyData["require_time"]));
                        //requireTime = Parser.String.ParseHHMM(Parser.Json.ParseString(fairyData["require_time"]));

                        this.TBName = this.fairyName;
                    }

                    this.TBCategory = "요정";
                    this.TBSubtitle1 = LanguageResources.Instance["NEED_RELOGIN"];
                }
            }
        }
        private int _fairyId = 0;

        /// <summary>
        /// 요정 이름
        /// </summary>
        public string fairyName { get; set; } = "";

        /// <summary>
        /// 요정 특성
        /// </summary>
        public int passiveSkill
        {
            get
            {
                return _passiveSkill;
            }
            set
            {
                _passiveSkill = value;
                if (value != 0 && fairyId != 0)
                {
                    JObject trait = GameData.Fairy.GetTrait(value);
                    if (trait != null)
                    {
                        this.TBSubtitle1 = LanguageResources.Instance[string.Format("TRAIT_{0}", value)];
                        isRareTrait = Parser.Json.ParseInt(trait["is_rare"]) == 1 ? true : false;
                    }
                }
                else if (fairyId != 0)
                {
                    this.TBSubtitle1 = LanguageResources.Instance["NEED_RELOGIN"];
                }
            }
        }
        private int _passiveSkill = 0;

        /// <summary>
        /// 희귀 특성 여부
        /// </summary>
        public bool isRareTrait
        {
            get { return _isRareTrait; }
            set
            {
                _isRareTrait = value;
                OnPropertyChanged();
            }
        }
        private bool _isRareTrait = false;

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
                if (value != 0)
                {
                    this.TBSlot = (value / 2 + value % 2).ToString();
                    this.TBSlotType = value % 2 == 0 ? "heavy" : "normal";
                }
            }
        }
        private int _slot = 0;

        /// <summary>
        /// 종류
        /// </summary>
        public string type { get; set; } = "";

        /// <summary>
        /// 범주
        /// </summary>
        public string category { get; set; } = "";

        /// <summary>
        /// 레어
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
                if (star > 0)
                {
                    this.TBSubtitle2 = "";
                    for (int i = 0; i < value; i++)
                        this.TBSubtitle2 += "★";
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
                if (value != 0)
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
                        spendItem[0] = 1; // 장비제조계약
                        spendItem[1] = 0; // 코어
                        break;
                    case 1:     // 저투입
                        spendItem[0] = 1; // 장비제조계약
                        spendItem[1] = 2; // 코어
                        break;
                    case 2:     // 중투입
                        spendItem[0] = 20; // 장비제조계약
                        spendItem[1] = 4; // 코어
                        break;
                    case 3:     // 고투입
                        spendItem[0] = 50; // 장비제조계약
                        spendItem[1] = 6; // 코어
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
        /// 일반 제조 / 중형 제조
        /// </summary>
        public string TBSlotType
        {
            get { return _TBSlotType; }
            set
            {
                _TBSlotType = value;
                OnPropertyChanged();
            }
        }
        private string _TBSlotType = "";

        /// <summary>
        /// 장비 / 요정 이름
        /// </summary>
        public string TBName
        {
            get { return _TBName; }
            set
            {
                _TBName = value;
                OnPropertyChanged();
            }
        }
        private string _TBName = "";

        /// <summary>
        /// 장비 종류 / 요정 특성
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
        /// 장비 등급
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

        /// <summary>
        /// 인형 / 장비
        /// </summary>
        public string TBCategory
        {
            get { return _TBCategory; }
            set
            {
                _TBCategory = value;
                OnPropertyChanged();
            }
        }
        private string _TBCategory = "";

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

        #endregion

        // ==============================================
        // ===== Initializer
        // ==============================================
        #region Initializer

        public ProduceEquipTemplate() { }

        #endregion
    }
}
