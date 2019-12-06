using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace GFAlarm.Util
{
    public class Config
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static string version = "2.51";

        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        public static JObject data = null;
        public static string filename = "";

        /// <summary>
        /// 버전/주소 정보
        /// </summary>
        public static bool isLoaded = false;
        public static bool duringTheater = false;
        public static bool needFakeVersion = false;

        public static string latest_version = "";
        public static string latest_download_url = "";
        public static string voice_guide_url = "";
        public static string mail_guide_url = "";
        public static string chip_download_url = "";
        public static string battle_tester_url = "";
        public static string theater_detail_url = "";
        public static string uncensor_patch_url = "";
        public static string random_adjutant_guide_url = "";
        public static string decrypt_ssl_guide_url = "";
        public static string chimera_client_url = "";

        /// <summary>
        /// 내 IP
        /// </summary>
        public static string ip = Util.Common.GetLocalIPv4();

        /// <summary>
        /// 최신버전 여부
        /// </summary>
        /// <returns></returns>
        public static bool IsLatestVersion()
        {
            if (version == latest_version)
                return true;
            return false;
        }

        /// <summary>
        /// 버전/주소 정보 가져오기
        /// </summary>
        public static void TryLoadVersion()
        {
            try
            {
                if (isLoaded)
                    return;
                string txt = Util.Common.RequestWeb("https://pastebin.com/raw/R2nrVUh4");
                JObject json = Parser.Json.ParseJObject(txt);
                if (json != null)
                {
                    if (json.ContainsKey("version") && json.ContainsKey("download_url"))
                    {
                        latest_version = Parser.Json.ParseString(json["version"]);
                        latest_download_url = Parser.Json.ParseString(json["download_url"]);
                    }
                    if (json.ContainsKey("voice_guide_url"))
                    {
                        voice_guide_url = Parser.Json.ParseString(json["voice_guide_url"]);
                    }
                    if (json.ContainsKey("mail_guide_url"))
                    {
                        mail_guide_url = Parser.Json.ParseString(json["mail_guide_url"]);
                    }
                    if (json.ContainsKey("chip_download_url"))
                    {
                        chip_download_url = Parser.Json.ParseString(json["chip_download_url"]);
                    }
                    if (json.ContainsKey("battle_tester_url"))
                    {
                        battle_tester_url = Parser.Json.ParseString(json["battle_tester_url"]);
                    }
                    if (json.ContainsKey("theater_detail_url"))
                    {
                        theater_detail_url = Parser.Json.ParseString(json["theater_detail_url"]);
                    }
                    if (json.ContainsKey("uncensor_patch_url"))
                    {
                        uncensor_patch_url = Parser.Json.ParseString(json["uncensor_patch_url"]);
                    }
                    if (json.ContainsKey("random_adjutant_guide_url"))
                    {
                        random_adjutant_guide_url = Parser.Json.ParseString(json["random_adjutant_guide_url"]);
                    }
                    if (json.ContainsKey("decrypt_ssl_guide_url"))
                    {
                        decrypt_ssl_guide_url = Parser.Json.ParseString(json["decrypt_ssl_guide_url"]);
                    }
                    if (json.ContainsKey("chimera_client_url"))
                    {
                        chimera_client_url = Parser.Json.ParseString(json["chimera_client_url"]);
                    }
                    if (json.ContainsKey("now_theater"))
                    {
                        int now_theater = Parser.Json.ParseInt(json["now_theater"]);
                        switch (now_theater)
                        {
                            case 1:
                                duringTheater = true;
                                break;
                            default:
                                duringTheater = false;
                                Config.Setting.exportTheaterExercise = false;
                                break;
                        }
                    }
                }
                isLoaded = true;
            }
            catch { }
        }

        #region ArrayType

        /// <summary>
        /// Double 배열 형태 설정
        /// </summary>
        public class DoubleArray
        {
            private string key = "";
            private string group = "";
            private double[] defaultValues;
            private double[] values;
            public double[] ToArray()
            {
                return values;
            }
            public bool Contain(double val)
            {
                if (values.Contains(val))
                    return true;
                return false;
            }
            public int Length
            {
                get { return values.Length; }
            }
            public double this[int idx]
            {
                get
                {
                    if (this.Length < defaultValues.Length)
                        values = defaultValues;
                    return values[idx];
                }
                set
                {
                    values[idx] = value;
                    SetConfig(key, values, group);
                }
            }
            public DoubleArray(string key, string group, double[] defaultValues)
            {
                this.key = key;
                this.group = group;
                this.defaultValues = defaultValues;
                this.values = GetConfig(key, defaultValues, group);
                SetConfig(key, values, group);
            }
        }

        /// <summary>
        /// Int 배열 형태 설정
        /// </summary>
        public class IntArray
        {
            private string key = "";
            private string group = "";
            private int[] defaultValues;
            private int[] values;
            public int[] ToArray()
            {
                return values;
            }
            public bool Contain(int val)
            {
                if (values.Contains(val))
                    return true;
                return false;
            }
            public int Length
            {
                get { return values.Length; }
            }
            public int this[int idx]
            {
                get
                {
                    if (this.Length < defaultValues.Length)
                        values = defaultValues;
                    return values[idx];
                }
                set
                {
                    values[idx] = value;
                    SetConfig(key, values, group);
                }
            }
            public IntArray(string key, string group, int[] defaultValues)
            {
                this.key = key;
                this.group = group;
                this.defaultValues = defaultValues;
                this.values = GetConfig(key, defaultValues, group);
                SetConfig(key, values, group);
            }
        }

        /// <summary>
        /// Bool 배열 형태 설정
        /// </summary>
        public class BoolArray
        {
            private string key = "";
            private string group = "";
            private bool[] defaultValues;
            private bool[] values;
            public int Length
            {
                get { return values.Length; }
            }
            public bool this[int idx]
            {
                get
                {
                    if (this.Length < defaultValues.Length)
                        values = defaultValues;
                    return values[idx];
                }
                set
                {
                    values[idx] = value;
                    SetConfig(key, values, group);
                }
            }
            public BoolArray(string key, string group, bool[] defaultValues)
            {
                this.key = key;
                this.group = group;
                this.defaultValues = defaultValues;
                this.values = GetConfig(key, defaultValues, group);
            }
        }

        #endregion

        public class Window
        {
            /// <summary>
            /// Mutex GUID
            /// </summary>
            public static string mutexGuid
            {
                get
                {
                    string guid = GetConfig("mutex_guid", "", "window");
                    if (string.IsNullOrEmpty(guid))
                    {
                        guid = Guid.NewGuid().ToString();
                        SetConfig("mutex_guid", guid, "window");
                    }
                    return guid;
                }
            }

            /// <summary>
            /// 윈도우 위치
            /// </summary>
            public static DoubleArray windowPosition = new DoubleArray("position", "window", new double[] { 100, 100, 225, 720 });

            /// <summary>
            /// 서브 윈도우 위치
            /// </summary>
            public static DoubleArray subWindowPosition = new DoubleArray("sub_position", "window", new double[] { 100, 100, 225, 720 });

            /// <summary>
            /// 항상 위
            /// </summary>
            public static bool alwaysOnTop
            {
                get { return _alwaysOnTop; }
                set
                {
                    _alwaysOnTop = value;
                    SetConfig("always_on_top", value, "window");
                }
            }
            private static bool _alwaysOnTop = GetConfig("always_on_top", false, "window");

            /// <summary>
            /// 자석 기능
            /// </summary>
            public static bool stickyWindow
            {
                get { return _stickyWindow; }
                set
                {
                    _stickyWindow = value;
                    SetConfig("sticky_window", value, "window");
                }
            }
            private static bool _stickyWindow = GetConfig("sticky_window", true, "window");

            /// <summary>
            /// 트레이 숨김
            /// </summary>
            public static bool minimizeToTray
            {
                get { return _minimizeToTray; }
                set
                {
                    _minimizeToTray = value;
                    SetConfig("minimize_to_tray", value, "window");
                }
            }
            private static bool _minimizeToTray = GetConfig("minimize_to_tray", false, "window");

            /// <summary>
            /// 창 투명도
            /// </summary>
            public static int windowOpacity
            {
                get { return _windowOpacity; }
                set
                {
                    _windowOpacity = value;
                    SetConfig("opacity", value, "window");
                }
            }
            private static int _windowOpacity = GetConfig("opacity", 100, "window");

            /// <summary>
            /// 창 색상
            /// </summary>
            public static string windowColor
            {
                get { return _windowColor; }
                set
                {
                    _windowColor = value;
                    SetConfig("color", value, "window");
                }
            }
            private static string _windowColor = GetConfig("color", "007ACC", "window");
        }

        public class Setting
        {
            #region Language

            /// <summary>
            /// 언어
            /// </summary>
            public static string language
            {
                get { return _language; }
                set
                {
                    _language = value;
                    SetConfig("language", value);
                }
            }
            private static string _language = GetConfig("language", "ko-KR");

            #endregion

            #region Settings (Toast)

            /// <summary>
            /// 윈도우 알림
            /// </summary>
            private static bool _winToast = GetConfig("win_toast", true, "toast");
            public static bool winToast
            {
                get { return _winToast; }
                set
                {
                    _winToast = value;
                    SetConfig("win_toast", value, "toast");
                }
            }

            /// <summary>
            /// 강한 알림
            /// </summary>
            private static bool _strongWinToast = GetConfig("strong_win_toast", false, "toast");
            public static bool strongWinToast
            {
                get 
                {
                    if (_voiceNotification || _winToast == false)
                        return false;
                    return _strongWinToast; 
                }
                set
                {
                    _strongWinToast = value;
                    SetConfig("strong_win_toast", value, "toast");
                }
            }

            /// <summary>
            /// 음성 알림
            /// </summary>
            private static bool _voiceNotification = GetConfig("voice_notification", true, "toast");
            public static bool voiceNotification
            {
                get 
                {
                    if (_strongWinToast || _winToast == false)
                        return false;
                    return _voiceNotification; 
                }
                set
                {
                    _voiceNotification = value;
                    SetConfig("voice_notification", value, "toast");
                }
            }

            /// <summary>
            /// 부관 음성만
            /// </summary>
            private static bool _adjutantVoiceOnly = GetConfig("adjutant_voice_only", false, "toast");
            public static bool adjutantVoiceOnly
            {
                get 
                {
                    if (_strongWinToast || _winToast == false)
                        return false;
                    return _adjutantVoiceOnly; 
                }
                set
                {
                    _adjutantVoiceOnly = value;
                    SetConfig("adjutant_voice_only", value, "toast");
                }
            }

            /// <summary>
            /// 군수/자율 시작 음성
            /// </summary>
            private static bool _startVoice = GetConfig("start_voice", true, "toast");
            public static bool startVoice
            {
                get
                {
                    if (_strongWinToast || _winToast == false)
                        return false;
                    return _startVoice;
                }
                set
                {
                    _startVoice = value;
                    SetConfig("start_voice", value, "toast");
                }
            }

            /// <summary>
            /// 음성 볼륨
            /// </summary>
            private static int _voiceVolume = GetConfig("voice_volume", 100, "toast");
            public static int voiceVolume
            {
                get { return _voiceVolume; }
                set
                {
                    _voiceVolume = value;
                    SetConfig("voice_volume", value, "toast");
                }
            }

            /// <summary>
            /// 구버전 사운드 API 사용하기
            /// </summary>
            private static bool _useSoundPlayerApi = GetConfig("use_soundplayer_api", false, "toast");
            public static bool useSoundPlayerApi
            {
                get 
                {
                    if (_voiceNotification == false)
                        return false;
                    return _useSoundPlayerApi; 
                }
                set
                {
                    _useSoundPlayerApi = value;
                    SetConfig("use_soundplayer_api", value, "toast");
                }
            }

            #endregion

            #region Settings (Mail)

            /// <summary>
            /// 메일 알림
            /// </summary>
            private static bool _mailNotification = GetConfig("mail_notification", false, "mail");
            public static bool mailNotification
            {
                get
                {
                    return _mailNotification;
                }
                set
                {
                    _mailNotification = value;
                    SetConfig("mail_notification", value, "mail");
                }
            }

            /// <summary>
            /// 메일 SMTP 서버주소
            /// </summary>
            private static string _smtpServer = GetConfig("smtp_server", "smtp.gmail.com", "mail");
            public static string smtpServer
            {
                get { return _smtpServer; }
                set
                {
                    _smtpServer = value;
                    SetConfig("smtp_server", value, "mail");
                }
            }

            /// <summary>
            /// 알림 보낼 메일주소 (알리미용)
            /// </summary>
            private static string _fromMailAddress = GetConfig("from_mail_address", "", "mail");
            public static string fromMailAddress
            {
                get { return _fromMailAddress; }
                set
                {
                    _fromMailAddress = value;
                    SetConfig("from_mail_address", value, "mail");
                }
            }

            /// <summary>
            /// 알림 보낼 메일 앱 비밀번호 (알리미용)
            /// </summary>
            private static string _fromMailPass = GetConfig("from_mail_pass", "", "mail");
            public static string fromMailPass
            {
                get { return _fromMailPass; }
                set
                {
                    _fromMailPass = value;
                    SetConfig("from_mail_pass", value, "mail");
                }
            }

            /// <summary>
            /// 알림 받을 메일주소
            /// </summary>
            private static string _toMailAddress = GetConfig("to_mail_address", "", "mail");
            public static string toMailAddress
            {
                get { return _toMailAddress; }
                set
                {
                    _toMailAddress = value;
                    SetConfig("to_mail_address", value, "mail");
                }
            }

            #endregion

            #region Settings (Tab)

            /// <summary>
            /// 탭 알림
            /// </summary>
            private static bool _tabNotification = GetConfig("tab_notification", true, "tab");
            public static bool tabNotification
            {
                get { return _tabNotification; }
                set
                {
                    _tabNotification = value;
                    SetConfig("tab_notification", value, "tab");
                }
            }

            #endregion

            #region Settings (File)

            /// <summary>
            /// 파일 인코딩
            /// </summary>
            private static string _fileEncoding = GetConfig("file_encoding", "Default", "file");
            public static string fileEncoding
            {
                get { return _fileEncoding; }
                set
                {
                    _fileEncoding = value;
                    SetConfig("file_encoding", value, "file");
                }
            }

            /// <summary>
            /// 사용자 정보 저장
            /// </summary>
            private static bool _exportUserInfo = GetConfig("export_user_info", false, "file");
            public static bool exportUserInfo
            {
                get { return _exportUserInfo; }
                set
                {
                    _exportUserInfo = value;
                    SetConfig("export_user_info", value, "file");
                }
            }

            /// <summary>
            /// 아이템 정보 저장
            /// </summary>
            private static bool _exportItemInfo = GetConfig("export_item_info", false, "file");
            public static bool exportItemInfo
            {
                get { return _exportItemInfo; }
                set
                {
                    _exportItemInfo = value;
                    SetConfig("export_item_info", value, "file");
                }
            }

            /// <summary>
            /// 인형 정보 저장
            /// </summary>
            private static bool _exportDollInfo = GetConfig("export_doll_info", false, "file");
            public static bool exportDollInfo
            {
                get { return _exportDollInfo; }
                set
                {
                    _exportDollInfo = value;
                    SetConfig("export_doll_info", value, "file");
                }
            }

            /// <summary>
            /// 장비 정보 저장
            /// </summary>
            private static bool _exportEquipInfo = GetConfig("export_equip_info", false, "file");
            public static bool exportEquipInfo
            {
                get { return _exportEquipInfo; }
                set
                {
                    _exportEquipInfo = value;
                    SetConfig("export_equip_info", value, "file");
                }
            }

            /// <summary>
            /// 요정 정보 저장
            /// </summary>
            private static bool _exportFairyInfo = GetConfig("export_fairy_info", false, "file");
            public static bool exportFairyInfo
            {
                get { return _exportFairyInfo; }
                set
                {
                    _exportFairyInfo = value;
                    SetConfig("export_fairy_info", value, "file");
                }
            }

            /// <summary>
            /// 획득 인형 저장
            /// </summary>
            private static bool _exportRescuedDoll = GetConfig("export_rescued_doll", false, "file");
            public static bool exportRescuedDoll
            {
                get { return _exportRescuedDoll; }
                set
                {
                    _exportRescuedDoll = value;
                    SetConfig("export_rescued_doll", value, "file");
                }
            }

            /// <summary>
            /// 국지전 웨이브 저장
            /// </summary>
            private static bool _exportTheaterExercise = GetConfig("export_theater_exercise", false, "file");
            public static bool exportTheaterExercise
            {
                get { return _exportTheaterExercise; }
                set
                {
                    _exportTheaterExercise = value;
                    SetConfig("export_theater_exercise", value, "file");
                }
            }

            /// <summary>
            /// 전투 테스터 프리셋 저장
            /// </summary>
            private static bool _exportBattleTesterPreset = GetConfig("export_battle_tester", false, "file");
            public static bool exportBattleTesterPreset
            {
                get { return _exportBattleTesterPreset; }
                set
                {
                    _exportBattleTesterPreset = value;
                    SetConfig("export_battle_tester", value, "file");
                }
            }

            #endregion

            #region Settings (Extra)

            /// <summary>
            /// 업데이트 확인
            /// </summary>
            private static bool _checkUpdate = GetConfig("check_update", true, "");
            public static bool checkUpdate
            {
                get { return _checkUpdate; }
                set
                {
                    _checkUpdate = value;
                    SetConfig("check_update", value, "");
                }
            }

            /// <summary>
            /// 로그 패킷
            /// </summary>
            private static bool _logPacket = GetConfig("log_packet", false, "logging");
            public static bool logPacket
            {
                get { return _logPacket; }
                set
                {
                    _logPacket = value;
                    SetConfig("log_packet", value, "logging");
                }
            }

            /// <summary>
            /// 로그 레벨
            /// 0: Trace, 1: Debug, 2: Info, 3: Warn, 4: Error, 5: Fatal
            /// </summary>
            private static int _logLevel = GetConfig("log_level", 2, "logging");
            public static int logLevel
            {
                get { return _logLevel; }
                set
                {
                    _logLevel = value;
                    SetConfig("log_level", value, "logging");
                }
            }

            #endregion
        }

        public class Alarm
        {
            #region Alarm (Complete)

            /// <summary>
            /// 지원현황 알림
            /// </summary>
            private static bool _notifyDispatchedEchelonComplete = GetConfig("notify_dispatched_echelon", true, "complete");
            public static bool notifyDispatchedEchelonComplete
            {
                get { return _notifyDispatchedEchelonComplete; }
                set
                {
                    _notifyDispatchedEchelonComplete = value;
                    SetConfig("notify_dispatched_echelon", value, "complete");
                }
            }

            /// <summary>
            /// 인형제조 알림
            /// </summary>
            private static bool _notifyProduceDollComplete = GetConfig("notify_produce_doll", true, "complete");
            public static bool notifyProduceDollComplete
            {
                get { return _notifyProduceDollComplete; }
                set
                {
                    _notifyProduceDollComplete = value;
                    SetConfig("notify_produce_doll", value, "complete");
                }
            }

            /// <summary>
            /// 장비제조 알림
            /// </summary>
            private static bool _notifyProduceEquipComplete = GetConfig("notify_produce_equip", true, "complete");
            public static bool notifyProduceEquipComplete
            {
                get { return _notifyProduceEquipComplete; }
                set
                {
                    _notifyProduceEquipComplete = value;
                    SetConfig("notify_produce_equip", value, "complete");
                }
            }

            /// <summary>
            /// 스킬훈련 알림
            /// </summary>
            private static bool _notifySkillTrainComplete = GetConfig("notify_skill_train", true, "complete");
            public static bool notifySkillTrainComplete
            {
                get { return _notifySkillTrainComplete; }
                set
                {
                    _notifySkillTrainComplete = value;
                    SetConfig("notify_skill_train", value, "complete");
                }
            }

            /// <summary>
            /// 분석현황 알림
            /// </summary>
            private static bool _notifyDataAnalysisComplete = GetConfig("notify_data_analysis", true, "complete");
            public static bool notifyDataAnalysisComplete
            {
                get { return _notifyDataAnalysisComplete; }
                set
                {
                    _notifyDataAnalysisComplete = value;
                    SetConfig("notify_data_analysis", value, "complete");
                }
            }

            /// <summary>
            /// 인형수복 알림
            /// </summary>
            private static bool _notifyRestoreDollComplete = GetConfig("notify_restore_doll", false, "complete");
            public static bool notifyRestoreDollComplete
            {
                get { return _notifyRestoreDollComplete; }
                set
                {
                    _notifyRestoreDollComplete = value;
                    SetConfig("notify_restore_doll", value, "complete");
                }
            }

            /// <summary>
            /// 탐색현황 알림
            /// </summary>
            private static bool _notifyExploreComplete = GetConfig("notify_explore", true, "complete");
            public static bool notifyExploreComplete
            {
                get { return _notifyExploreComplete; }
                set
                {
                    _notifyExploreComplete = value;
                    SetConfig("notify_explore", value, "complete");
                }
            }

            /// <summary>
            /// 작전보고서 알림
            /// </summary>
            private static bool _notifyBattleReportComplete = GetConfig("notify_battle_report", true, "complete");
            public static bool notifyBattleReportComplete
            {
                get { return _notifyBattleReportComplete; }
                set
                {
                    _notifyBattleReportComplete = value;
                    SetConfig("notify_battle_report", value, "complete");
                }
            }

            #endregion

            #region Alarm (Maximum)

            /// <summary>
            /// 모의작전점수 상한 알림
            /// </summary>
            private static bool _notifyMaxBp = GetConfig("notify_bp", true, "maximum");
            public static bool notifyMaxBp
            {
                get { return _notifyMaxBp; }
                set
                {
                    _notifyMaxBp = value;
                    SetConfig("notify_bp", value, "maximum");
                }
            }

            /// <summary>
            /// 모의작전점수 상한 점수
            /// </summary>
            private static int _notifyMaxBpPoint = GetConfig("notify_bp_point", 3, "maximum");
            public static int notifyMaxBpPoint
            {
                get { return _notifyMaxBpPoint; }
                set
                {
                    _notifyMaxBpPoint = value;
                    SetConfig("notify_bp_point", value, "maximum");
                }
            }

            /// <summary>
            /// 인형 상한 알림
            /// </summary>
            private static bool _notifyMaxDoll = GetConfig("notify_max_doll", true, "maximum");
            public static bool notifyMaxDoll
            {
                get { return _notifyMaxDoll; }
                set
                {
                    _notifyMaxDoll = value;
                    SetConfig("notify_max_doll", value, "maximum");
                }
            }

            /// <summary>
            /// 장비 상한 알림
            /// </summary>
            private static bool _notifyMaxEquip = GetConfig("notify_max_equip", true, "maximum");
            public static bool notifyMaxEquip
            {
                get { return _notifyMaxEquip; }
                set
                {
                    _notifyMaxEquip = value;
                    SetConfig("notify_max_equip", value, "maximum");
                }
            }

            /// <summary>
            /// 자유경험치 상한 알림
            /// </summary>
            private static bool _notifyMaxGlobalExp = GetConfig("notify_max_global_exp", true, "maximum");
            public static bool notifyMaxGlobalExp
            {
                get { return _notifyMaxGlobalExp; }
                set
                {
                    _notifyMaxGlobalExp = value;
                    SetConfig("notify_max_global_exp", value, "maximum");
                }
            }

            #endregion

            #region Alarm (Mission)

            /// <summary>
            /// 인형 획득 알림 사용 여부
            /// </summary>
            private static bool _noitfyRescueDoll = GetConfig("notify_rescue_doll", true, "mission");
            public static bool notifyRescueDoll
            {
                get { return _noitfyRescueDoll; }
                set
                {
                    _noitfyRescueDoll = value;
                    SetConfig("notify_rescue_doll", value, "mission");
                }
            }

            /// <summary>
            /// 인형 획득 레어도
            /// </summary>
            private static int _notifyRescueDollStar = GetConfig("notify_rescue_doll_star", 5, "mission");
            public static int notifyRescueDollStar
            {
                get { return _notifyRescueDollStar; }
                set
                {
                    _notifyRescueDollStar = value;
                    SetConfig("notify_rescue_doll_star", value, "mission");
                }
            }

            /// <summary>
            /// 장비 획득 알림 사용 여부
            /// </summary>
            private static bool _notifyGetEquip = GetConfig("notify_get_equip", true, "mission");
            public static bool notifyGetEquip
            {
                get { return _notifyGetEquip; }
                set
                {
                    _notifyGetEquip = value;
                    SetConfig("notify_get_equip", value, "mission");
                }
            }

            /// <summary>
            /// 장비 획득 레어도
            /// </summary>
            private static int _notifyGetEquipStar = GetConfig("notify_get_equip_star", 5, "mission");
            public static int notifyGetEquipStar
            {
                get { return _notifyGetEquipStar; }
                set
                {
                    _notifyGetEquipStar = value;
                    SetConfig("notify_get_equip_star", value, "mission");
                }
            }

            /// <summary>
            /// 전역 승리 알림
            /// </summary>
            private static bool _notifyMissionSuccess = GetConfig("notify_mission_success", false, "mission");
            public static bool notifyMissionSuccess
            {
                get { return _notifyMissionSuccess; }
                set
                {
                    _notifyMissionSuccess = value;
                    SetConfig("notify_mission_success", value, "mission");
                }
            }

            /// <summary>
            /// 제대 이동 완료 알림 사용 여부
            /// </summary>
            private static bool _notifyTeamMove = GetConfig("notify_team_move", false, "mission");
            public static bool notifyTeamMove
            {
                get { return _notifyTeamMove; }
                set
                {
                    _notifyTeamMove = value;
                    SetConfig("notify_team_move", value, "mission");
                }
            }

            /// <summary>
            /// 제대 이동 완료 횟수
            /// </summary>
            private static int _notifyTeamMoveCount = GetConfig("notify_team_move_count", 6, "mission");
            public static int notifyTeamMoveCount
            {
                get { return _notifyTeamMoveCount; }
                set
                {
                    _notifyTeamMoveCount = value;
                    SetConfig("notify_team_move_count", value, "mission");
                }
            }

            /// <summary>
            /// 제대 이동 완료 후 전투 종료 알림
            /// </summary>
            private static bool _notifyTeamMoveAndBattleFinish = GetConfig("notify_team_move_and_battle_finish", false, "mission");
            public static bool notifyTeamMoveAndBattleFinish
            {
                get { return _notifyTeamMoveAndBattleFinish; }
                set
                {
                    _notifyTeamMoveAndBattleFinish = value;
                    SetConfig("notify_team_move_and_battle_finish", value, "mission");
                }
            }

            #endregion

            #region Alarm (Doll)

            /// <summary>
            /// 편제확대 필요 알림
            /// </summary>
            private static bool _notifyDollNeedDummyLink = GetConfig("notify_need_dummy_link", true, "doll");
            public static bool notifyDollNeedDummyLink
            {
                get { return _notifyDollNeedDummyLink; }
                set
                {
                    _notifyDollNeedDummyLink = value;
                    SetConfig("notify_need_dummy_link", value, "doll");
                }
            }

            /// <summary>
            /// 중상 알림 사용 여부
            /// </summary>
            private static bool _notifyDollWounded = GetConfig("notify_wounded", true, "doll");
            public static bool notifyDollWounded
            {
                get { return _notifyDollWounded; }
                set
                {
                    _notifyDollWounded = value;
                    SetConfig("notify_wounded", value, "doll");
                }
            }

            /// <summary>
            /// 중상 알림 퍼센트
            /// </summary>
            private static int _notifyDollWoundedPercent = GetConfig("notify_wounded_percent", 30, "doll");
            public static int notifyDollWoundedPercent
            {
                get { return _notifyDollWoundedPercent; }
                set
                {
                    _notifyDollWoundedPercent = value;
                    SetConfig("notify_wounded_percent", value, "doll");
                }
            }

            /// <summary>
            /// 최대 레벨 알림
            /// </summary>
            private static bool _notifyMaxLevel = GetConfig("notify_max_level", true, "doll");
            public static bool notifyMaxLevel
            {
                get { return _notifyMaxLevel; }
                set
                {
                    _notifyMaxLevel = value;
                    SetConfig("notify_max_level", value, "doll");
                }
            }

            #endregion

            #region Alarm (Produce)

            /// <summary>
            /// 인형 제조 알림
            /// </summary>
            private static bool _notifyProduceDoll = GetConfig("notify_produce_doll", false, "produce");
            public static bool notifyProduceDoll
            {
                get { return _notifyProduceDoll; }
                set
                {
                    _notifyProduceDoll = value;
                    SetConfig("notify_produce_doll", value, "produce");
                }
            }

            /// <summary>
            /// 인형 제조 알림 (5성)
            /// </summary>
            private static bool _notifyProduceDoll5Star = GetConfig("notify_produce_doll_5star", false, "produce");
            public static bool notifyProduceDoll5Star
            {
                get { return _notifyProduceDoll5Star; }
                set
                {
                    _notifyProduceDoll5Star = value;
                    SetConfig("notify_produce_doll_5star", value, "produce");
                }
            }

            /// <summary>
            /// 장비 제조 알림
            /// </summary>
            private static bool _notifyProduceEquip = GetConfig("notify_produce_equip", false, "produce");
            public static bool notifyProduceEquip
            {
                get { return _notifyProduceEquip; }
                set
                {
                    _notifyProduceEquip = value;
                    SetConfig("notify_produce_equip", value, "produce");
                }
            }

            /// <summary>
            /// 장비 제조 알림 (5성)
            /// </summary>
            private static bool _notifyProduceEquip5Star = GetConfig("notify_produce_equip_5star", false, "produce");
            public static bool notifyProduceEquip5Star
            {
                get { return _notifyProduceEquip5Star; }
                set
                {
                    _notifyProduceEquip5Star = value;
                    SetConfig("notify_produce_equip_5star", value, "produce");
                }
            }

            /// <summary>
            /// 샷건 제조 알림
            /// </summary>
            private static bool _notifyProduceShotgun = GetConfig("notify_produce_shotgun", false, "produce");
            public static bool notifyProduceShotgun
            {
                get { return _notifyProduceShotgun; }
                set
                {
                    _notifyProduceShotgun = value;
                    SetConfig("notify_produce_shotgun", value, "produce");
                }
            }

            /// <summary>
            /// 요정 제조 알림
            /// </summary>
            private static bool _notifyProduceFairy = GetConfig("notify_produce_fairy", false, "produce");
            public static bool notifyProduceFairy
            {
                get { return _notifyProduceFairy; }
                set
                {
                    _notifyProduceFairy = value;
                    SetConfig("notify_produce_fairy", value, "produce");
                }
            }

            #endregion
        }

        public class Costume
        {
            /// <summary>
            /// 지휘관 보너스 (경험치) 사용 여부
            /// </summary>
            private static bool _coBonusDollExp = GetConfig("doll_exp", false, "costume_bonus");
            public static bool coBonusDollExp
            {
                get { return _coBonusDollExp; }
                set
                {
                    _coBonusDollExp = value;
                    SetConfig("doll_exp", value, "costume_bonus");
                }
            }

            /// <summary>
            /// 지휘관 보너스 (경험치) 퍼센트
            /// </summary>
            private static int _coBonusDollExpPercent = GetConfig("doll_exp_percent", 3, "costume_bonus");
            public static int coBonusDollExpPercent
            {
                get { return _coBonusDollExpPercent; }
                set
                {
                    _coBonusDollExpPercent = value;
                    SetConfig("doll_exp_percent", value, "costume_bonus");
                }
            }

            /// <summary>
            /// 지휘관 보너스 (수복시간) 사용 여부
            /// </summary>
            private static bool _coBonusRestoreTime = GetConfig("restore_time", false, "costume_bonus");
            public static bool coBonusRestoreTime
            {
                get { return _coBonusRestoreTime; }
                set
                {
                    _coBonusRestoreTime = value;
                    SetConfig("restore_time", value, "costume_bonus");
                }
            }

            /// <summary>
            /// 지휘관 보너스 (수복시간) 퍼센트
            /// </summary>
            private static int _coBonusRestoreTimePercent = GetConfig("restore_time_percent", 10, "costume_bonus");
            public static int coBonusRestoreTimePercent
            {
                get { return _coBonusRestoreTimePercent; }
                set
                {
                    _coBonusRestoreTimePercent = value;
                    SetConfig("restore_time_percent", value, "costume_bonus");
                }
            }

            /// <summary>
            /// 지휘관 보너스 (스킬훈련시간) 사용 여부
            /// </summary>
            private static bool _coBonusSkillTrainTime = GetConfig("skill_train_time", false, "costume_bonus");
            public static bool coBonusSkillTrainTime
            {
                get { return _coBonusSkillTrainTime; }
                set
                {
                    _coBonusSkillTrainTime = value;
                    SetConfig("skill_train_time", value, "costume_bonus");
                }
            }

            /// <summary>
            /// 지휘관 보너스 (스킬훈련시간) 퍼센트
            /// </summary>
            private static int _coBonusSkillTrainTimePercent = GetConfig("skill_train_time_percent", 3, "costume_bonus");
            public static int coBonusSkillTrainTimePercent
            {
                get { return _coBonusSkillTrainTimePercent; }
                set
                {
                    _coBonusSkillTrainTimePercent = value;
                    SetConfig("skill_train_time_percent", value, "costume_bonus");
                }
            }
        }

        public class Extra
        {
            /// <summary>
            /// 임무 달성
            /// </summary>
            private static bool _notifyQuestComplete = GetConfig("notify_quest_complete", true, "extra");
            public static bool notifyQuestComplete
            {
                get
                {
                    return _notifyQuestComplete;
                }
                set
                {
                    _notifyQuestComplete = value;
                    SetConfig("notify_quest_complete", value, "extra");
                }
            }

            /// <summary>
            /// 경험치 업 이벤트 사용 여부
            /// </summary>
            private static bool _expBonusEvent = GetConfig("exp_bonus_event", false, "extra");
            public static bool expBonusEvent
            {
                get { return _expBonusEvent; }
                set
                {
                    _expBonusEvent = value;
                    SetConfig("exp_bonus_event", value, "extra");
                }
            }

            /// <summary>
            /// 경험치 업 이벤트 퍼센트
            /// </summary>
            private static int _expBonusEventPercent = GetConfig("exp_bonus_event_percent", 50, "extra");
            public static int expBonusEventPercent
            {
                get { return _expBonusEventPercent; }
                set
                {
                    _expBonusEventPercent = value;
                    SetConfig("exp_bonus_event_percent", value, "extra");
                }
            }

            /// <summary>
            /// 미리 알림
            /// </summary>
            public static bool earlyNotify
            {
                get { return _earlyNotify; }
                set
                {
                    _earlyNotify = value;
                    SetConfig("early_notify", value, "extra");
                }
            }
            private static bool _earlyNotify = GetConfig("early_notify", false, "extra");

            /// <summary>
            /// 미리 알림 초
            /// </summary>
            public static int earlyNotifySeconds
            {
                get { return _earlyNotifySeconds; }
                set
                {
                    _earlyNotifySeconds = value;
                    earlyNotifyMiliseconds = value * 1000 + 3000;
                    SetConfig("early_notify_seconds", value, "extra");
                }
            }
            private static int _earlyNotifySeconds = GetConfig("early_notify_seconds", 0, "extra");

            /// <summary>
            /// 미리 알림 밀리초
            /// </summary>
            public static long earlyNotifyMiliseconds
            {
                get
                {
                    if (Config.Extra.earlyNotify)
                        return _earlyNotifyMiliseconds;
                    else
                        return 3000;
                }
                set
                {
                    _earlyNotifyMiliseconds = value;
                }
            }
            private static long _earlyNotifyMiliseconds = GetConfig("early_notify_seconds", 0, "extra") * 1000 + 3000;

            /// <summary>
            /// 검열해제 모드
            /// </summary>
            public static bool unlockCensorMode = false;
        }

        public class Dashboard
        {
            /// <summary>
            /// 알림 탭 그룹 열림 여부
            /// </summary>
            public static BoolArray expand = new BoolArray("expand", "dashboard", new bool[] { true, true, true, true, true, true, true });

            /// <summary>
            /// 알림 탭 그룹 순서
            /// </summary>
            public static IntArray index = new IntArray("index", "dashboard", new int[] { 0, 1, 2, 3, 4, 5, 6 });

            /// <summary>
            /// 알림 탭 필터링
            /// </summary>
            public static BoolArray filter = new BoolArray("filter", "dashboard", new bool[] { true, true, true, true, true, true, true });

            /// <summary>
            /// 알림 탭 정렬
            /// </summary>
            private static string _sort = GetConfig("sort", "slot", "dashboard");
            public static string sort
            {
                get { return _sort; }
                set
                {
                    _sort = value;
                    SetConfig("sort", value, "dashboard");
                }
            }
        }

        public class Echelon
        {
            /// <summary>
            /// 제대 탭 그룹 열림 여부
            /// </summary>
            public static BoolArray expand = new BoolArray("expand", "echelon", new bool[] { true, true, true, true, true, true, true, true, true, true });

            /// <summary>
            /// 기본 경험치
            /// </summary>
            public static int baseExp
            {
                get { return _baseExp; }
                set
                {
                    _baseExp = value;
                    SetConfig("base_exp", value, "echelon");
                }
            }
            private static int _baseExp = GetConfig("base_exp", 490, "echelon");

            /// <summary>
            /// 교전 횟수
            /// </summary>
            public static int battleCount
            {
                get { return _battleCount; }
                set
                {
                    _battleCount = value;
                    SetConfig("battle_count", value, "echelon");
                }
            }
            private static int _battleCount = GetConfig("battle_count", 5, "echelon");

            /// <summary>
            /// 레벨 패널티
            /// </summary>
            public static int levelPenalty
            {
                get { return _levelPenalty; }
                set
                {
                    _levelPenalty = value;
                    SetConfig("level_penalty", value, "echelon");
                }
            }
            private static int _levelPenalty = GetConfig("level_penalty", 112, "echelon");

            /// <summary>
            /// 경험치 업 이벤트
            /// </summary>
            public static bool expUpEvent
            {
                get { return _expUpEvent; }
                set
                {
                    _expUpEvent = value;
                    SetConfig("exp_up_event", value, "echelon");
                }
            }
            private static bool _expUpEvent = GetConfig("exp_up_event", false, "echelon");
        }

        public class Footer
        {
            /// <summary>
            /// 푸터 열림 여부
            /// </summary>
            private static bool _expand = GetConfig("expand", false, "footer");
            public static bool expand
            {
                get { return _expand; }
                set
                {
                    _expand = value;
                    SetConfig("expand", value, "footer");
                }
            }
        }

        public class Proxy
        {
            #region Proxy

            /// <summary>
            /// 포트
            /// </summary>
            private static int _port = GetConfig("port", 9000, "proxy");
            public static int port
            {
                get
                {
                    return _port;
                }
                set
                {
                    _port = value;
                    SetConfig("port", value, "proxy");
                }
            }

            /// <summary>
            /// 자동 시작
            /// </summary>
            private static bool _startup = GetConfig("startup", false, "proxy");
            public static bool startup
            {
                get { return _startup; }
                set
                {
                    _startup = value;
                    SetConfig("startup", value, "proxy");
                }
            }

            /// <summary>
            /// SSL 복호화
            /// </summary>
            private static bool _decryptSsl = GetConfig("decrypt_ssl", false, "proxy");
            public static bool decryptSsl
            {
                get { return _decryptSsl; }
                set
                {
                    _decryptSsl = value;
                    SetConfig("decrypt_ssl", value, "proxy");
                }
            }

            private static bool _upstreamProxy = GetConfig("upstream_proxy", false, "proxy");
            public static bool upstreamProxy
            {
                get
                {
                    return _upstreamProxy;
                }
                set
                {
                    _upstreamProxy = value;
                    SetConfig("upstream_proxy", value, "proxy");
                }
            }

            /// <summary>
            /// 경유 프록시 호스트
            /// </summary>
            private static string _upstreamHost = GetConfig("upstream_proxy_host", "", "proxy");
            public static string upstreamHost
            {
                get { return _upstreamHost; }
                set
                {
                    _upstreamHost = value;
                    SetConfig("upstream_proxy_host", value, "proxy");
                }
            }

            /// <summary>
            /// 경유 프록시 포트
            /// </summary>
            private static int _upstreamPort = GetConfig("upstream_proxy_port", 10000, "proxy");
            public static int upstreamPort
            {
                get { return _upstreamPort; }
                set
                {
                    _upstreamPort = value;
                    SetConfig("upstream_proxy_port", value, "proxy");
                }
            }

            /// <summary>
            /// PAC 서버 사용 여부
            /// </summary>
            public static bool usePac
            {
                get { return _usePac; }
                set
                {
                    if (_usePac == value)
                        return;
                    _usePac = value;
                    SetConfig("use_pac", value, "proxy");
                }
            }
            private static bool _usePac = GetConfig("use_pac", false, "proxy");

            /// <summary>
            /// PAC 서버 도메인
            /// </summary>
            public static string pacDomain
            {
                get { return _pacDomain; }
                set
                {
                    if (_pacDomain == value)
                        return;
                    _pacDomain = value;
                    SetConfig("pac_domain", value, "proxy");
                }
            }
            private static string _pacDomain = GetConfig("pac_domain", "", "proxy");

            /// <summary>
            /// PAC 서버 포트번호
            /// </summary>
            public static int pacPort
            {
                get { return _pacPort; }
                set
                {
                    if (_pacPort == value)
                        return;
                    _pacPort = value;
                    SetConfig("pac_port", value, "proxy");
                }
            }
            private static int _pacPort = GetConfig("pac_port", 9001, "proxy");

            /// <summary>
            /// PAC 서버 자동 시작
            /// </summary>
            public static bool pacAutoStart
            {
                get { return _pacAutoStart; }
                set
                {
                    if (_pacAutoStart == value)
                        return;
                    _pacAutoStart = value;
                    SetConfig("pac_startup", value, "proxy");
                }
            }
            private static bool _pacAutoStart = GetConfig("pac_startup", false, "proxy");

            #endregion
        }

        /// <summary>
        /// 부관 설정
        /// </summary>
        public class Adjutant
        {
            /// <summary>
            /// 랜덤 부관 사용 여부
            /// </summary>
            public static bool useRandomAdjutant
            {
                get { return _useRandomAdjutant; }
                set
                {
                    _useRandomAdjutant = value;
                    SetConfig("random_adjutant", value, "adjutant");
                }
            }
            private static bool _useRandomAdjutant = GetConfig("random_adjutant", false, "adjutant");

            /// <summary>
            /// 부상 설정 (0: 통상만, 1: 부상만, 2: 랜덤)
            /// </summary>
            public static int adjutantShape
            {
                get { return _adjutantShape; }
                set
                {
                    if (_adjutantShape == value)
                        return;
                    _adjutantShape = value;
                    SetConfig("adjutant_shape", value, "adjutant");
                }
            }
            private static int _adjutantShape = GetConfig("adjutant_shape", 2, "adjutant");
            
            /// <summary>
            /// 스킨 설정 (0: 통상만, 1: 스킨만, 2: Live2D만, 3: 아동절만, 4: 개조, 5: 랜덤)
            /// </summary>
            public static int adjutantSkin
            {
                get { return _adjutantSkin; }
                set
                {
                    if (_adjutantSkin == value)
                        return;
                    _adjutantSkin = value;
                    SetConfig("adjutant_skin", value, "adjutant");
                }
            }
            private static int _adjutantSkin = GetConfig("adjutant_skin", 5, "adjutant");

            /// <summary>
            /// 스킨 범주
            /// =====================================
            /// normal: 통상만
            /// skin: 스킨만
            /// live2d_skin: Live2D 스킨만
            /// child_skin: 아동절 스킨만
            /// mod_skin: 개조 스킨만
            /// random: 랜덤
            /// =====================================
            /// </summary>
            public static string adjutantSkinCategory
            {
                get { return _adjutantSkinCategory; }
                set
                {
                    if (_adjutantSkinCategory == value)
                        return;
                    _adjutantSkinCategory = value;
                    SetConfig("adjutant_skin_category", value, "adjutant");
                }
            }
            private static string _adjutantSkinCategory = GetConfig("adjutant_skin_category", "random", "adjutant");

            /// <summary>
            /// 인형 강제리스트
            /// </summary>
            //public static int[] dollForcelist
            //{
            //    get
            //    {
            //        try
            //        {
            //            return _dollForcelist.Split(',').Select(Int32.Parse).ToArray();
            //        }
            //        catch { }
            //        return new int[] { };
            //    }
            //    set
            //    {
            //        try
            //        {
            //            string tempValue = string.Join(",", value.Select(x => x.ToString()));
            //            _dollForcelist = tempValue;
            //            SetConfig("doll_forcelist", tempValue, "adjutant");
            //        }
            //        catch { }
            //    }
            //}
            //private static string _dollForcelist = GetConfig("doll_forcelist", "", "adjutant");
            /// <summary>
            /// 스킨 강제리스트
            /// </summary>
            //public static int[] skinForcelist
            //{
            //    get
            //    {
            //        try
            //        {
            //            return _skinForcelist.Split(',').Select(Int32.Parse).ToArray();
            //        }
            //        catch { }
            //        return new int[] { };
            //    }
            //    set
            //    {
            //        try
            //        {
            //            string tempValue = string.Join(",", value.Select(x => x.ToString()));
            //            _skinForcelist = tempValue;
            //            SetConfig("skin_forcelist", tempValue, "adjutant");
            //        }
            //        catch { }
            //    }
            //}
            //private static string _skinForcelist = GetConfig("skin_forcelist", "", "adjutant");

            /// <summary>
            /// 인형 화이트리스트
            /// </summary>
            public static int[] dollWhitelist
            {
                get
                {
                    try
                    {
                        return _dollWhitelist.Split(',').Select(Int32.Parse).ToArray();
                    }
                    catch { }
                    return new int[] { };
                }
                set
                {
                    try
                    {
                        string tempValue = string.Join(",", value.Select(x => x.ToString()));
                        _dollWhitelist = tempValue;
                        SetConfig("doll_whitelist", tempValue, "adjutant");
                    }
                    catch { }
                }
            }
            private static string _dollWhitelist = GetConfig("doll_whitelist", "", "adjutant");

            /// <summary>
            /// 스킨 화이트리스트
            /// </summary>
            public static int[] skinWhitelist
            {
                get
                {
                    try
                    {
                        return _skinWhitelist.Split(',').Select(Int32.Parse).ToArray();
                    }
                    catch { }
                    return new int[] { };
                }
                set
                {
                    try
                    {
                        string tempValue = string.Join(",", value.Select(x => x.ToString()));
                        _skinWhitelist = tempValue;
                        SetConfig("skin_whitelist", tempValue, "adjutant");
                    }
                    catch { }
                }
            }
            private static string _skinWhitelist = GetConfig("skin_whitelist", "", "adjutant");

            /// <summary>
            /// 인형 블랙리스트
            /// </summary>
            public static int[] dollBlacklist
            {
                get
                {
                    try
                    {
                        return _dollBlacklist.Split(',').Select(Int32.Parse).ToArray();
                    }
                    catch { }
                    return new int[] { };
                }
                set
                {
                    try
                    {
                        string tempValue = string.Join(",", value.Select(x => x.ToString()));
                        _dollBlacklist = tempValue;
                        SetConfig("doll_blacklist", tempValue, "adjutant");
                    }
                    catch { }
                }
            }
            private static string _dollBlacklist = GetConfig("doll_blacklist", "", "adjutant"); // 106,142,75,1003,204

            /// <summary>
            /// 스킨 블랙리스트
            /// </summary>
            public static int[] skinBlacklist
            {
                get
                {
                    try
                    {
                        return _skinBlacklist.Split(',').Select(Int32.Parse).ToArray();
                    }
                    catch { }
                    return new int[] { };
                }
                set
                {
                    try
                    {
                        string tempValue = string.Join(",", value.Select(x => x.ToString()));
                        _skinBlacklist = tempValue;
                        SetConfig("skin_blacklist", tempValue, "adjutant");
                    }
                    catch { }
                }
            }
            private static string _skinBlacklist = GetConfig("skin_blacklist", "", "adjutant");
        }

        #region GetConfiguration

        public static bool GetConfig(string key, bool defaultValue = false, string group = "")
        {
            try
            {
                string value = GetConfig(key, "", group);
                if (string.IsNullOrEmpty(value))
                    return defaultValue;
                return Boolean.Parse(value);
            }
            catch { }
            return defaultValue;
        }

        public static bool[] GetConfig(string key, bool[] defaultValue = null, string group = "")
        {
            try
            {
                string value = GetConfig(key, "", group);
                if (string.IsNullOrEmpty(value))
                    return defaultValue;
                return value.Split(',').Select(Boolean.Parse).ToArray();
            }
            catch { }
            return defaultValue;
        }

        public static int GetConfig(string key, int defaultValue = 0, string group = "")
        {
            try
            {
                string value = GetConfig(key, "", group);
                if (string.IsNullOrEmpty(value))
                    return defaultValue;
                return Int32.Parse(value);
            }
            catch { }
            return defaultValue;
        }

        public static int[] GetConfig(string key, int[] defaultValue = null, string group = "")
        {
            try
            {
                string value = GetConfig(key, "", group);
                if (string.IsNullOrEmpty(value))
                    return defaultValue;
                return value.Split(',').Select(Int32.Parse).ToArray();
            }
            catch { }
            return defaultValue;
        }

        public static double GetConfig(string key, double defaultValue = 0, string group = "")
        {
            try
            {
                string value = GetConfig(key, "", group);
                if (string.IsNullOrEmpty(value))
                    return defaultValue;
                return Double.Parse(value);
            }
            catch { }
            return defaultValue;
        }

        public static double[] GetConfig(string key, double[] defaultValue = null, string group = "")
        {
            try
            {
                string value = GetConfig(key, "", group);
                if (string.IsNullOrEmpty(value))
                    return defaultValue;
                return value.Split(',').Select(Double.Parse).ToArray();
            }
            catch { }
            return defaultValue;
        }

        public static string GetConfig(string key, string defaultValue = "", string group = "")
        {
            try
            {
                if (data == null)
                    Load();
                if (!string.IsNullOrEmpty(group))
                {
                    if (!data.ContainsKey(group))
                        return defaultValue;
                    if (data[group] is JObject && data[group][key] != null)
                        return data[group][key].ToString();
                }
                else
                {
                    if (data.ContainsKey(key))
                        return data[key].ToString();
                }
            }
            catch (Exception ex) { log.Error(ex); }
            return defaultValue;
        }

        #endregion

        #region SetConfiguration

        public static void SetConfig(string key, bool value, string group = "")
        {
            try
            {
                SetConfig(key, value.ToString(), group);
            }
            catch { }
        }

        public static void SetConfig(string key, bool[] value, string group = "")
        {
            try
            {
                SetConfig(key, String.Join(",", value), group);
            }
            catch { }
        }

        public static void SetConfig(string key, int value, string group = "")
        {
            try
            {
                SetConfig(key, value.ToString(), group);
            }
            catch { }
        }

        public static void SetConfig(string key, int[] value, string group = "")
        {
            try
            {
                SetConfig(key, String.Join(",", value), group);
            }
            catch { }
        }

        public static void SetConfig(string key, double value, string group = "")
        {
            try
            {
                SetConfig(key, value.ToString(), group);
            }
            catch { }
        }

        public static void SetConfig(string key, double[] value, string group = "")
        {
            try
            {
                SetConfig(key, String.Join(",", value), group);
            }
            catch { }
        }

        public static void SetConfig(string key, string value, string group = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(group))
                {
                    if (!data.ContainsKey(group))
                        data.Add(group, new JObject());
                    if (data[group] is JObject)
                    {
                        if (data[group][key] != null)
                            data[group][key] = value;
                        else
                            data[group].Value<JObject>().Add(key, value);
                    }
                }
                else
                {
                    if (data.ContainsKey(key))
                        data[key] = value;
                    else
                        data.Add(key, value);
                }
                Save();
            }
            catch (Exception ex) { log.Error(ex); }
        }

        #endregion

        public static void Load()
        {
            try
            {
                _lock.EnterWriteLock();
                filename = string.Format("{0}/Config.json", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                if (File.Exists(filename))
                {
                    data = JObject.Parse(File.ReadAllText(filename));
                }
                else
                {
                    data = new JObject();
                }
            }
            catch (Exception ex) { log.Error(ex); }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public static void Save()
        {
            try
            {
                _lock.EnterWriteLock();
                File.WriteAllText(filename, data.ToString());
            }
            catch (Exception ex) { log.Error(ex); }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
