using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Util;
using Newtonsoft.Json;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GFAlarm.View.DataTemplate
{
    /// <summary>
    /// Echelon Member Template
    /// </summary>
    public class EchelonTemplate : INotifyPropertyChanged
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // ==============================================
        // ===== Echelon
        // ==============================================
        #region Echelon

        /// <summary>
        /// 인형 ID
        /// </summary>
        public long gunWithUserId { get; set; } = 0;

        /// <summary>
        /// 요정 ID
        /// </summary>
        public long fairyWithUserId { get; set; } = 0;

        /// <summary>
        /// 제대
        /// </summary>
        public int teamId = 0;

        /// <summary>
        /// 제대 순번
        /// </summary>
        public int location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnPropertyChanged();
            }
        }
        private int _location = 0;

        /// <summary>
        /// 레어도
        /// </summary>
        public int star
        {
            get { return _star; }
            set
            {
                _star = value;
                OnPropertyChanged();
            }
        }
        private int _star = 0;

        /// <summary>
        /// 보직
        /// (0: 리더, 1: 멤버, 2: 요정)
        /// </summary>
        public string assign
        {
            get { return _assign; }
            set
            {
                _assign = value;
                OnPropertyChanged();
            }
        }
        private string _assign = "";

        /// <summary>
        /// 콜라보 여부
        /// </summary>
        public bool collabo
        {
            get { return _collabo; }
            set
            {
                _collabo = value;
                OnPropertyChanged();
            }
        }
        private bool _collabo = false;

        /// <summary>
        /// 레벨
        /// </summary>
        public int level
        {
            get { return _level; }
            set
            {
                _level = value;
                OnPropertyChanged();
            }
        }
        private int _level = 0;

        /// <summary>
        /// 최대 레벨
        /// </summary>
        public int maxLevel
        {
            get { return _maxLevel; }
            set
            {
                _maxLevel = value;
                OnPropertyChanged();
            }
        }
        private int _maxLevel = 120;

        /// <summary>
        /// 병과
        /// HG, SMG, AR, RF, MG, SG, FAIRY
        /// </summary>
        public string type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }
        private string _type = "";

        /// <summary>
        /// 요정 종류
        /// (combat, strategy)
        /// </summary>
        public string fairyType
        {
            get { return _fairyType; }
            set
            {
                _fairyType = value;
            }
        }
        private string _fairyType = "";

        /// <summary>
        /// 요정 특성
        /// </summary>
        public string fairyTrait
        {
            get { return _fairyTrait; }
            set
            {
                _fairyTrait = value;
                OnPropertyChanged();
            }
        }
        private string _fairyTrait = "";

        /// <summary>
        /// 병과 아이콘
        /// </summary>
        public string typeIcon
        {
            get { return _typeIcon; }
            set
            {
                _typeIcon = GetTypeUri(value);
                OnPropertyChanged();
            }
        }
        private string _typeIcon = "";

        /// <summary>
        /// 이름
        /// </summary>
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        private string _name = "";

        /// <summary>
        /// 남은 경험치
        /// </summary>
        public string remainExp
        {
            get { return _remainExp; }
            set
            {
                _remainExp = value;
                OnPropertyChanged();
            }
        }
        private string _remainExp = "";

        /// <summary>
        /// 인형 링크
        /// </summary>
        public string number
        {
            get { return _number; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _number = string.Format("x{0}", value);
                else
                    _number = "";
                OnPropertyChanged();
            }
        }
        private string _number = "";

        /// <summary>
        /// 인형 최대 링크
        /// </summary>
        public string maxNumber
        {
            get { return _maxNumber; }
            set
            {
                _maxNumber = value;
                OnPropertyChanged();
            }
        }
        private string _maxNumber = "";

        /// <summary>
        /// 인형 체력
        /// </summary>
        public string hp
        {
            get { return _hp; }
            set
            {
                _hp = value;
                OnPropertyChanged();
            }
        }
        private string _hp = "";

        /// <summary>
        /// 인형 최대 체력
        /// </summary>
        public string maxHp
        {
            get { return _maxHp; }
            set
            {
                _maxHp = value;
                OnPropertyChanged();
            }
        }
        private string _maxHp = "";

        /// <summary>
        /// 체력 바 높이
        /// </summary>
        public double maxHpHeight = 29;
        public double hpHeight
        {
            get { return _hpHeight; }
            set
            {
                _hpHeight = value;
                OnPropertyChanged();
            }
        }
        private double _hpHeight = 0;

        /// <summary>
        /// 경험치 바 높이
        /// </summary>
        public double maxExpHeight = 29;
        public double expHeight
        {
            get { return _expHeight; }
            set
            {
                _expHeight = value;
                OnPropertyChanged();
            }
        }
        private double _expHeight = 0;

        /// <summary>
        /// 탄식
        /// (0: 탄약, 1: 식량)
        /// </summary>
        public int[] supply { get; set; } = new int[] { 0, 0 };

        /// <summary>
        /// 탄약
        /// </summary>
        public string ammo { get; set; } = "0";
        /// <summary>
        /// 식량
        /// </summary>
        public string mre { get; set; } = "0";

        /// <summary>
        /// 마지막 목록 여부
        /// </summary>
        public bool isLast
        {
            get { return _isLast; }
            set
            {
                if (_isLast == value)
                    return;
                _isLast = value;
                OnPropertyChanged();
            }
        }
        private bool _isLast = false;

        /// <summary>
        /// 전역 횟수
        /// </summary>
        public int[] runCount { get; set; } = new int[] { 0, 0, 0 };
        /// <summary>
        /// 작보 갯수
        /// </summary>
        public int[] reportCount { get; set; } = new int[] { 0, 0, 0 };

        #endregion

        // ==============================================
        // ===== TextBlock
        // ==============================================
        #region TextBlock

        /// <summary>
        /// 수복 정보
        /// </summary>
        public string TBRestoreRequire
        {
            get
            {
                return _TBRestoreRequire;
            }
            set
            {
                _TBRestoreRequire = value;
                if (value.Contains(","))
                {
                    // 수복시간,인력,부품
                    string[] items = value.Split(',');
                    if (items.Length == 3)
                    {
                        TBRestoreRequireTime = items[0];
                        TBRestoreRequireManpower = items[1];
                        TBRestoreRequirePart = items[2];
                    }
                }

                OnPropertyChanged("TBRestoreRequireTime");
                OnPropertyChanged("TBRestoreRequireManpower");
                OnPropertyChanged("TBRestoreRequirePart");
            }
        }
        private string _TBRestoreRequire = "";

        public string TBRestoreRequireTime { get; set; } = "";
        public string TBRestoreRequireManpower { get; set; } = "";
        public string TBRestoreRequirePart { get; set; } = "";

        /// <summary>
        /// 체력 상태 (2:중상/1:주의/0:정상)
        /// </summary>
        public string TBHpWarning
        {
            get { return _TBHpWarning; }
            set
            {
                _TBHpWarning = value;
                OnPropertyChanged();
            }
        }
        private string _TBHpWarning = "";

        /// <summary>
        /// 희귀 특성 여부
        /// </summary>
        public string isRareTrait
        {
            get { return _isRareTrait; }
            set
            {
                _isRareTrait = value;
                OnPropertyChanged();
            }
        }
        private string _isRareTrait = "";

        #endregion

        // ==============================================
        // ===== Initializer
        // ==============================================
        #region Initializer

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="id"></param>
        public EchelonTemplate(DollWithUserInfo doll)
        {
            this.gunWithUserId = doll.id;
            this.teamId = doll.team;
            this.location = doll.location;
            Refresh();
        }

        public EchelonTemplate(FairyWithUserInfo fairy)
        {
            this.fairyWithUserId = fairy.id;
            this.teamId = fairy.team;
            this.location = 6;
            Refresh();
        }

        public EchelonTemplate() { }

        #endregion

        // ==============================================
        // ===== Function
        // ==============================================
        #region Function

        /// <summary>
        /// 새로고침
        /// </summary>
        /// <param name="refresh"></param>
        public void Refresh(bool forceUpdate = false)
        {
            if (this.gunWithUserId > 0)
            {
                DollWithUserInfo data = UserData.Doll.Get(this.gunWithUserId);
                if (data != null)
                {
                    data.Refresh(DollWithUserInfo.REFRESH.ALL, forceUpdate);
                    this.assign = data.location == 1 ? "LEADER" : "";
                    this.location = data.location;
                    this.star = data.star;
                    this.collabo = data.collabo;
                    this.level = data.level;
                    this.maxLevel = data.maxLevel;
                    this.number = data.link.ToString();
                    this.maxNumber = data.maxLink.ToString();
                    this.name = data.name;
                    if (data.remainExp == 0 && data.level == data.maxLevel)
                        this.remainExp = "MAX";
                    else
                        this.remainExp = data.remainExp.ToString();
                    this.type = data.type;
                    this.hp = data.hp.ToString();
                    this.maxHp = data.maxHp.ToString();
                    this.TBHpWarning = data.hpWarningLevel.ToString();

                    this.hpHeight = (double)data.hp / (double)data.maxHp * this.maxHpHeight;
                    this.expHeight = (double)data.currentExp / (double)data.maxExp * this.maxExpHeight;

                    this.typeIcon = GetTypeIcon(this.type, "", this.star, this.collabo);

                    this.supply[0] = data.ammo;
                    this.supply[1] = data.mre;
                    OnPropertyChanged("supply");

                    this.runCount = data.runCount;
                    OnPropertyChanged("runCount");
                    this.reportCount = data.reportCount;
                    OnPropertyChanged("reportCount");

                    int[] restore = data.restore;
                    if (restore.Length == 3)
                    {
                        this.TBRestoreRequireTime = TimeUtil.GetRemainHHMMSS(data.restore[0] + TimeUtil.GetCurrentSec());
                        this.TBRestoreRequireManpower = restore[1].ToString();
                        this.TBRestoreRequirePart = restore[2].ToString();
                    }
                    OnPropertyChanged("TBRestoreRequireTime");
                    OnPropertyChanged("TBRestoreRequireManpower");
                    OnPropertyChanged("TBRestoreRequirePart");
                }
                else
                {
                    log.Warn("인형 정보가 존재하지 않음 {0}", this.gunWithUserId);
                }
            }
            else if (this.fairyWithUserId > 0)
            {
                FairyWithUserInfo data = UserData.Fairy.Get(this.fairyWithUserId);
                if (data != null)
                {
                    this.type = "FAIRY";
                    this.fairyType = data.category;
                    this.location = 6;
                    this.level = data.level;
                    this.maxLevel = data.maxLevel;
                    this.name = data.name;
                    if (data.remainExp == 0 && data.level == data.maxLevel)
                        this.remainExp = "MAX";
                    else
                        this.remainExp = data.remainExp.ToString();
                    this.fairyTrait = data.traitName;
                    this.isRareTrait = data.isRareTrait.ToString();

                    this.expHeight = (double)data.currentExp / (double)data.maxExp * this.maxExpHeight;

                    this.typeIcon = GetTypeIcon(this.type, this.fairyType, 0, false);

                    data.Refresh(FairyWithUserInfo.REFRESH.CHANGE_TEAM_EXP);

                    if (data.remainExp == 0)
                    {
                        this.runCount = new int[] { 0, 0, 0 };
                        this.reportCount = new int[] { 0, 0, 0 };
                    }
                    else
                    {
                        this.runCount = data.runCount;
                        this.reportCount = data.reportCount;
                    }

                    this.TBRestoreRequireTime = "00:00:00";
                    OnPropertyChanged("TBRestoreRequireTime");
                }
                else
                {
                    log.Warn("요정 정보가 존재하지 않음 {0}", this.fairyWithUserId);
                }
            }
        }

        /// <summary>
        /// 병과 아이콘 가져오기
        /// </summary>
        /// <param name="type"></param>
        /// <param name="star"></param>
        /// <param name="collabo"></param>
        /// <returns></returns>
        private static string GetTypeIcon(string type, string fairyType = "", int star = 2, bool collabo = false)
        {
            string prefix = string.Format("{0}", type.ToLower());
            string postfix = string.Format("{0}", star);
            if (collabo == true)
                postfix = "collabo";
            if (!string.IsNullOrEmpty(fairyType))
                postfix = string.Format("{0}", fairyType);
            return string.Format("{0}_{1}", prefix, postfix);
        }

        /// <summary>
        /// 이미지 경로
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string GetTypeUri(string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return "";
            }
            else
            {
                return string.Format("/GFAlarm;component/Resource/image/gun/type/{0}.png", item);
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}
