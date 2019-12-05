using GFAlarm.Data;
using GFAlarm.Util;
using LocalizationResources;
using Newtonsoft.Json;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace GFAlarm.View.DataTemplate
{
    /// <summary>
    /// Data Analysis Template
    /// </summary>
    public class DataAnalysisTemplate : INotifyPropertyChanged
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // ==============================================
        // ===== Data Piece
        // ==============================================
        #region Data Piece

        /// <summary>
        /// 핵심데이터 여부
        /// </summary>
        public int isPiece
        {
            get
            {
                return _isPiece;
            }
            set
            {
                _isPiece = value;
                if (value != 0)
                {
                    TBPieceGuide = LanguageResources.Instance["DATA_PIECE"];
                }
            }
        }
        private int _isPiece = 0;

        /// <summary>
        /// 핵심데이터 ID
        /// </summary>
        public int pieceId
        {
            get
            {
                return _pieceId;
            }
            set
            {
                _pieceId = value;
                if (value != 0)
                {
                    TBPiece = GameData.Chip.GetSquadPieceName(pieceId);
                }
            }
        }
        private int _pieceId = 0;

        #endregion

        // ==============================================
        // ===== Chip
        // ==============================================
        #region Chip

        /// <summary>
        /// 칩셋 ID
        /// </summary>
        public int chipId
        {
            get
            {
                return _chipId;
            }
            set
            {
                _chipId = value;
                if (value != 0)
                {
                    chipStar = Parser.String.ParseInt(value.ToString().Substring(0, 1));
                }
            }
        }
        private int _chipId = 0;

        /// <summary>
        /// 칩셋 레어도
        /// </summary>
        public int chipStar
        {
            get { return _chipStar; }
            set
            {
                _chipStar = value;
                if (value != 0)
                {
                    TBChipStar = "";
                    for (int i = 0; i < value; i++)
                    {
                        TBChipStar += "★";
                    }
                }
            }
        }
        private int _chipStar = 0;

        /// <summary>
        /// 칩셋 색상
        /// (1: 빨강 칩셋, 2: 파랑 칩셋)
        /// </summary>
        public int colorId { get; set; } = 0;

        /// <summary>
        /// 칩셋 모양
        /// </summary>
        public int gridId
        {
            get
            {
                return _gridId;
            }
            set
            {
                _gridId = value;
                if (value != 0 && colorId != 0)
                {
                    // 칩셋 모양 설정
                    canvasChipShape = MainWindow.dashboardView.GetChipShape(value, colorId);
                    OnPropertyChanged();
                }
            }
        }
        private int _gridId = 0;

        /// <summary>
        /// 칩셋 살상 포인트
        /// </summary>
        public int assistDamage
        {
            get { return _assistDamage; }
            set
            {
                if (value > 0 && _gridId > 0 && _chipStar > 0)
                {
                    damage = GameData.Chip.GetStat(_gridId, _chipStar, 1, value);
                    _assistDamage = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _assistDamage = 0;

        /// <summary>
        /// 칩셋 파쇄 포인트
        /// </summary>
        public int assistDefBreak
        {
            get { return _assistDefBreak; }
            set
            {
                if (value > 0 && _gridId > 0 && _chipStar > 0)
                {
                    defBreak = GameData.Chip.GetStat(_gridId, _chipStar, 2, value);
                    _assistDefBreak = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _assistDefBreak = 0;

        /// <summary>
        /// 칩셋 정밀 포인트
        /// </summary>
        public int assistHit
        {
            get { return _assistHit; }
            set
            {
                if (value > 0 && _gridId > 0 && _chipStar > 0)
                {
                    hit = GameData.Chip.GetStat(_gridId, _chipStar, 3, value);
                    _assistHit = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _assistHit = 0;

        /// <summary>
        /// 칩셋 장전 포인트
        /// </summary>
        public int assistReload
        {
            get { return _assistReload; }
            set
            {
                if (value > 0 && _gridId > 0 && _chipStar > 0)
                {
                    reload = GameData.Chip.GetStat(_gridId, _chipStar, 4, value);
                    _assistReload = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _assistReload = 0;

        /// <summary>
        /// 칩셋 살상 스탯
        /// </summary>
        public int damage
        {
            get { return _damage; }
            set
            {
                if (_damage == value)
                    return;
                _damage = value;
                OnPropertyChanged();
            }
        }
        private int _damage = 0;

        /// <summary>
        /// 칩셋 파쇄 스탯
        /// </summary>
        public int defBreak
        {
            get { return _defBreak; }
            set
            {
                if (_defBreak == value)
                    return;
                _defBreak = value;
                OnPropertyChanged();
            }
        }
        private int _defBreak = 0;

        /// <summary>
        /// 칩셋 정밀 스탯
        /// </summary>
        public int hit
        {
            get { return _hit; }
            set
            {
                if (_hit == value)
                    return;
                _hit = value;
                OnPropertyChanged();
            }
        }
        private int _hit = 0;

        /// <summary>
        /// 칩셋 장전 스탯
        /// </summary>
        public int reload
        {
            get { return _reload; }
            set
            {
                if (_reload == value)
                    return;
                _reload = value;
                OnPropertyChanged();
            }
        }
        private int _reload = 0;

        /// <summary>
        /// 칩셋 모양
        /// </summary>
        public Canvas canvasChipShape { get; set; }

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
                    TBSlot = value.ToString();
                    if (value >= 10)
                    {
                        TBSlot = "0";
                    }
                }
            }
        }
        private int _slot = 0;

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
                    TBRemainTime = TimeUtil.GetRemainHHMMSS(value);
                    TBEndTime = TimeUtil.GetDateTime(value, "MM-dd HH:mm");
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
        /// 슬롯
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
        /// 핵심데이터 종류
        /// </summary>
        public string TBPiece
        {
            get
            {
                return _TBPiece;
            }
            set
            {
                _TBPiece = value;
                OnPropertyChanged();
            }
        }
        private string _TBPiece = "";

        /// <summary>
        /// 재로그인 필요 문구
        /// </summary>
        public string TBPieceGuide
        {
            get { return _TBPieceGuide; }
            set
            {
                _TBPieceGuide = value;
                OnPropertyChanged();
            }
        }
        private string _TBPieceGuide = "";

        /// <summary>
        /// 칩셋 레어도
        /// </summary>
        public string TBChipStar
        {
            get
            {
                return _TBChipStar;
            }
            set
            {
                _TBChipStar = value;
                OnPropertyChanged();
            }
        }
        private string _TBChipStar = "";
        
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
        // ===== Function
        // ==============================================
        #region Function

        #endregion

        // ==============================================
        // ===== Initializer
        // ==============================================
        #region Initializer

        public DataAnalysisTemplate() { }

        #endregion
    }
}
