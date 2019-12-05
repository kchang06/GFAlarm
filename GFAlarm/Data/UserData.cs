using GFAlarm.Constants;
using GFAlarm.Data.Element;
using GFAlarm.Notifier;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using GFAlarm.View.Menu;
using LocalizationResources;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GFAlarm.Data
{
    public static class UserData
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void OnStaticPropertyChanged([CallerMemberName] string name = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }
        public static event PropertyChangedEventHandler StaticPropertyChanged;

        #region Userinfo

        // 사용자 전체 정보 JSON
        public static JObject index = null;

        // 사용자 UID (response Index/index > user_info > user_id)
        public static long uid = 0;
        // 닉네임 (response Index/index > user_info > name)
        public static string name = "";
        // 레벨 (response Index/index > user_info > lv)
        public static int level = -1;
        // 보석 (response Index/index > user_info > gem)
        public static int gem = -1;
        // 가입일 (response Index/index > user_info > reg_time)
        public static long regTime = 0;
        // Sign Key
        public static string sign = "";

        /*
         * 출석시간 (response Index/index > user_record > attendance_type1_time)
         * 내일 00:00
         */
        public static int attendanceTime
        {
            get
            {
                //if (_attendanceTime == -1)
                //    return _attendanceTime;
                // 출석시간 존재하지 않음
                //if (_attendanceTime == 0)
                //{
                //    log.Debug("출석시간을 찾을 수 없음 - 컴퓨터 시간대 기준으로 설정");
                //    int now = TimeUtil.GetCurrentSec();
                //    DateTime midnight = DateTime.Today;
                //    _attendanceTime = TimeUtil.GetSec(midnight);
                //    while (_attendanceTime < now)
                //    {
                //        midnight.AddDays(1);
                //        _attendanceTime = TimeUtil.GetSec(midnight);
                //        log.Debug("출석시간 조정 {0}", TimeUtil.GetDateTime(_attendanceTime, "MM-dd HH:mm"));
                //    }
                //}
                // 출석시간이 현재 시간보다 이전
                //else if (_attendanceTime < TimeUtil.GetCurrentSec())
                //{
                //    log.Debug("출석시간이 현재시간보다 이전 - 다음 날로 설정");
                //    int now = TimeUtil.GetCurrentSec();
                //    while (_attendanceTime < now)
                //    {
                //        _attendanceTime += TimeUtil.DAY;
                //        log.Debug("출석시간 조정 {0}", TimeUtil.GetDateTime(_attendanceTime, "MM-dd HH:mm"));
                //        // TODO: 다음 날 처리
                //    }
                //}
                return _attendanceTime;
            }
            set
            {
                if (_attendanceTime == value)
                    return;
                _attendanceTime = value;

                //UserData.CombatSimulation.remainPointToday = UserData.CombatSimulation.GetRemainPoint();
                //UserData.CombatSimulation.midnightTime = value;

                //int remainBp = UserData.CombatSimulation.GetRemainBp();
                //UserData.CombatSimulation.remainPointToday = remainBp;
                //UserData.CombatSimulation.midnightTime = value;
            }
        }
        private static int _attendanceTime = -1;

        // 출석시간 도래 여부
        public static bool isReachAttendanceTime = false;

        /*
         * 공유보석 공유시간 (response Index/index > share_with_user_info > last_time)
         * 금주 공유보석 미수령 시 금주 월요일 00:00으로 시간이 맞춰짐
         * 금주 공유보석 수령 시 차주 월요일 00:00으로 시간이 맞춰짐
         */
        public static long shareGemTime
        {
            get
            {
                return _shareGemTime;
            }
            set
            {
                _shareGemTime = value;
            }
        }
        private static long _shareGemTime = -1;

        /*
         * 부관 인형 (response Index/index > user_record > adjutant)
         * "{0},{1},{2},{3}", 인형도감번호,인형스킨번호,불명,불명
         * (예시 - 155,3706,0,0)
         * 인형도감번호가 -1인 경우 카리나
         */
        public static int adjutantDoll = 0;
        public static int adjutantDollSkin = 0;

        /*
         * 부관 요정 (response Index/index > user_record > adjutant_fairy)
         * "{0},{1},{2}", 요정도감번호,요정스킨번호,불명
         */
        public static int adjutantFairy = 0;

        /*
         * 금일 지원제대 횟수
         * (response Friend/teamGuns > borrow_team_today)
         * (response Index/Quest > daily > borrow_friend_team)
         * (response Index/index > user_friend_info > borrow_team_today 사용하지 말 것 - 부정확한 정보)
         */
        public static int borrowTeamToday
        {
            get { return _borrowTeamToday; }
            set
            {
                if (_borrowTeamToday == value)
                    return;
                _borrowTeamToday = value;
                if (value >= 20)
                    MainWindow.view.isMaxReinforce = true;
                else
                    MainWindow.view.isMaxReinforce = false;
                OnStaticPropertyChanged();
            }
        }
        private static int _borrowTeamToday = 0;

        /*
         * 금일 남은 공유전지 (response Index/index > dorm_rest_friend_build_coin_count)
         * request Dorm/get_build_coin 요청마다 차감
         */
        public static int remainFriendBattery
        {
            get { return _remainFriendBattery; }
            set
            {
                //log.Debug("remain_friend_battery {0}", value);
                _remainFriendBattery = value;
                if (value <= 0)
                {
                    MainWindow.dashboardView.isSharedBattery = true;
                }
                else
                {
                    MainWindow.dashboardView.isSharedBattery = false;
                }
            }
        }
        private static int _remainFriendBattery = 0;

        /// <summary>
        /// 인형 갯수
        /// </summary>
        public static int dollCount
        {
            get { return _dollCount; }
            set
            {
                if (_dollCount == value)
                    return;
                _dollCount = value;
                if (value >= maxDollCount && maxDollCount > 0)
                    MainWindow.view.isMaxDoll = true;
                else
                    MainWindow.view.isMaxDoll = false;
                OnStaticPropertyChanged();
            }
        }
        private static int _dollCount = 0;

        // 인형창고 크기 (user_info > maxgun)
        public static int maxDollCount
        {
            get { return _maxDollCount; }
            set
            {
                if (_maxDollCount == value)
                    return;
                _maxDollCount = value;
                OnStaticPropertyChanged();
            }
        }
        private static int _maxDollCount = 0;

        /// <summary>
        /// 장비 갯수
        /// </summary>
        public static int equipCount
        {
            get { return _equipCount; }
            set
            {
                if (_equipCount == value)
                    return;
                _equipCount = value;
                if (value >= maxEquipCount && maxEquipCount > 0)
                    MainWindow.view.isMaxEquip = true;
                else
                    MainWindow.view.isMaxEquip = false;
                OnStaticPropertyChanged();
            }
        }
        private static int _equipCount = 0;

        // 장비창고 크기 (user_info > maxequip)
        public static int maxEquipCount
        {
            get { return _maxEquipCount; }
            set
            {
                if (_maxEquipCount == value)
                    return;
                _maxEquipCount = value;
                OnStaticPropertyChanged();
            }
        }
        private static int _maxEquipCount = 0;

        // 인력 (user_info > mp)
        public static int mp
        {
            get { return _mp; }
            set
            {
                if (_mp == value)
                    return;
                _mp = value;
                OnStaticPropertyChanged();
            }
        }
        private static int _mp = 0;

        // 탄약 (user_info > ammo)
        public static int ammo
        {
            get { return _ammo; }
            set
            {
                if (_ammo == value)
                    return;
                _ammo = value;
                OnStaticPropertyChanged();
            }
        }
        private static int _ammo = 0;

        // 식량 (user_info > mre)
        public static int mre
        {
            get { return _mre; }
            set
            {
                if (_mre == value)
                    return;
                _mre = value;
                OnStaticPropertyChanged();
            }
        }
        private static int _mre = 0;

        // 부품 (user_info > part)
        public static int part
        {
            get { return _part; }
            set
            {
                if (_part == value)
                    return;
                _part = value;
                OnStaticPropertyChanged();
            }
        }
        private static int _part = 0;

        /// <summary>
        /// 현재 시간 (초)
        /// </summary>
        public static int currentSec
        {
            get { return _currentSec; }
            set
            {
                if (_currentSec == value)
                    return;
                _currentSec = value;
                currentTime = TimeUtil.GetDateTime(value + TimeUtil.testSec, "MM-dd HH:mm:ss");
            }
        }
        private static int _currentSec = 0;

        /// <summary>
        /// 현재 시간 (날짜시각)
        /// </summary>
        public static string currentTime
        {
            get { return _currentTime; }
            set
            {
                if (_currentTime == value)
                    return;
                _currentTime = value;
                OnStaticPropertyChanged();
            }
        }
        private static string _currentTime = "00-00 00:00:00";

        #endregion

        /// <summary>
        /// 모의작전
        /// </summary>
        public static class CombatSimulation
        {
            public static void OnStaticPropertyChanged([CallerMemberName] string name = "")
            {
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
            }
            public static event PropertyChangedEventHandler StaticPropertyChanged;

            // 알림
            public static bool notified = false;
            // 새로고침 중지
            public static bool pauseRefresh = true;

            /// <summary>
            /// 모의작전점수
            /// </summary>
            public static int point
            {
                get { return _point; }
                set
                {
                    if (0 <= value && value <= 6)
                        _point = value;
                    if (value == 6)
                        MainWindow.view.isMaxBp = true;
                    else
                        MainWindow.view.isMaxBp = false;
                    OnStaticPropertyChanged();
                }
            }
            private static int _point = -1;

            /// <summary>
            /// 모의작전점수 충전까지 남은 시간 (HH:mm:ss)
            /// </summary>
            public static string remainTime
            {
                get { return _remainTime; }
                set
                {
                    if (_remainTime == value)
                        return;
                    _remainTime = value;
                    OnStaticPropertyChanged();
                }
            }
            private static string _remainTime = "00:00:00";

            /// <summary>
            /// 오늘 충전가능한 모의작전점수
            /// </summary>
            public static int remainPointToday
            {
                get { return _remainPointToday; }
                set
                {
                    _remainPointToday = value;
                    if (value >= 0)
                    {
                        MainWindow.view.SetBpPointToolTip(value, TimeUtil.GetDateTime(UserData.attendanceTime, "MM-dd HH:mm"));
                        //MainWindow.view.SetBpPointToolTip(value, Parser.Time.GetDateTime(UserData.CombatSimulation._midnightTime).ToString("MM-dd HH:mm"));
                    }
                }
            }
            private static int _remainPointToday = -1;

            /// <summary>
            /// 오늘 충전가능한 모의작전점수 기준 시간대
            /// </summary>
            //public static int midnightTime
            //{
            //    get { return _midnightTime; }
            //    set
            //    {
            //        _midnightTime = value;
            //        if (value > 0)
            //        {
            //            MainWindow.view.SetBpPointToolTip(UserData.CombatSimulation.remainPointToday, TimeUtil.GetDateTime(value, "MM-dd HH:mm"));
            //            //MainWindow.view.SetBpPointToolTip(UserData.CombatSimulation.remainBpToday, Parser.Time.GetDateTime(value).ToString("MM-dd HH:mm"));
            //        }
            //    }
            //}
            //private static int _midnightTime = -1;

            /// <summary>
            /// 모의작전점수 충전시간
            /// </summary>
            public static int recoverTime
            {
                get { return _recoverTime; }
                set
                {
                    if (_recoverTime == value)
                        return;
                    _recoverTime = value;
                    //if (value > 0)
                    //    UserData.CombatSimulation.remainPointToday = UserData.CombatSimulation.GetRemainBp();
                }
            }
            private static int _recoverTime = -1;

            /// <summary>
            /// 마지막 모의작전점수 충전시간
            /// </summary>
            public static int lastRecoverTime
            {
                get { return _lastRecoverTime; }
                set
                {
                    if (_lastRecoverTime == value)
                        return;
                    _lastRecoverTime = value;
                    //if (value > 0)
                    //{
                    //    pauseRefresh = true;
                    //    int now = TimeUtil.GetCurrentSec();
                    //    int recoverTime = value + TimeUtil.HOUR * 2;
                    //    if (point == 6)
                    //    {
                    //        recoverTime = 0;
                    //    }
                    //    else
                    //    {
                    //        while (recoverTime < now && point < 6)
                    //        {
                    //            point++;
                    //            recoverTime += TimeUtil.HOUR * 2;
                    //        }
                    //        if (recoverTime < now && point == 6)
                    //        {
                    //            recoverTime = 0;
                    //        }
                    //    }
                    //    UserData.CombatSimulation.recoverTime = recoverTime;
                    //    pauseRefresh = false;
                    //}
                }
            }
            private static int _lastRecoverTime = -1;

            /// <summary>
            /// 모의작전점수 사용
            /// </summary>
            /// <param name="use"></param>
            public static void UseBp(int useBp)
            {
                pauseRefresh = true;

                log.Debug("모의작전점수 {0} 점 사용", useBp);
                int bp = UserData.CombatSimulation.point;
                if (bp >= 6)
                {
                    UserData.CombatSimulation.recoverTime = TimeUtil.GetCurrentSec() + TimeUtil.HOUR * 2;
                    //UserData.CombatSimulation.recoverTime = Parser.Time.GetCurrentMs() + Parser.Time.HOR * 2;
                }
                bp -= useBp;
                if (bp < 0)
                {
                    bp = 0;
                }
                UserData.CombatSimulation.point = bp;

                pauseRefresh = false;
            }

            /// <summary>
            /// 오늘 충전가능한 모의작전점수 가져오기
            /// </summary>
            /// <returns></returns>
            public static int GetRemainPoint()
            {
                try
                {
                    log.Debug("오늘 충전가능한 모의작전점수 가져오는 중...");
                    if (UserData.attendanceTime > 0 && lastRecoverTime > 0)
                    {
                        int now = TimeUtil.GetCurrentSec() + TimeUtil.testSec;
                        int tempLastRecoverTime = lastRecoverTime;
                        log.Debug("last_recover_time={0}", TimeUtil.GetDateTime(tempLastRecoverTime, "MM-dd HH:mm:ss"));

                        int remainPoint = 0;
                        while (remainPoint <= 12)
                        {
                            tempLastRecoverTime += TimeUtil.HOUR * 2;
                            if (tempLastRecoverTime < attendanceTime)
                            {
                                remainPoint++;
                                log.Debug("last_recover_time={0}, remain_point={1}", TimeUtil.GetDateTime(tempLastRecoverTime, "MM-dd HH:mm:ss"), remainPoint);
                            }
                            else
                            {
                                break;
                            }
                        }
                        return remainPoint;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return 0;
            }

            /// <summary>
            /// 오늘 충전가능한 모의작전점수 가져오기
            /// </summary>
            /// <returns></returns>
            public static int GetRemainBp()
            {
                int remainBp = 0;
                try
                {
                    log.Debug("오늘 충전가능한 모의작전점수 가져오는 중...");
                    if (UserData.attendanceTime > 0)
                    {
                        int now = TimeUtil.GetCurrentSec();
                        int midnight = UserData.attendanceTime;
                        log.Debug("현재시간 {0} 출석시간 {1}", TimeUtil.GetDateTime(now, "MM-dd HH:mm"), TimeUtil.GetDateTime(midnight, "MM-dd HH:mm"));
                        int remainTime = 0;
                        if (recoverTime > 0 && recoverTime > midnight)
                        {
                            // 충전시간이 자정을 넘기면 0 점
                            log.Debug("충전시간 {0} > 자정 {1}",
                                TimeUtil.GetDateTime(recoverTime, "MM-dd HH:mm"),
                                TimeUtil.GetDateTime(UserData.attendanceTime, "MM-dd HH:mm"));
                            return remainBp;
                        }
                        remainTime = TimeUtil.GetTodayRemainSec(UserData.attendanceTime);
                        if (recoverTime > 0)
                        {
                            log.Debug("다음 충전시간 {0}", TimeUtil.GetRemainSec(remainTime));
                            if (now + TimeUtil.GetRemainSec(remainTime) < UserData.attendanceTime)
                                remainBp = 1;
                            remainTime -= TimeUtil.GetRemainSec(recoverTime);
                        }
                        remainBp += remainTime / TimeUtil.HOUR * 2;
                        if (remainBp > 13)
                        {
                            log.Debug("남은 시간에 문제가 있음 - 충전가능 모의점수 0");
                            remainBp = 0;
                        }
                    }
                    else
                    {
                        log.Debug("출석시간 존재하지 않음");
                    }
                    //log.Debug("오늘 충전가능한 모의작전점수 가져오는 중...");
                    //if (UserData.attendanceTime > 0)
                    //{
                    //    long now = Parser.Time.GetCurrentMs();
                    //    long midnight = UserData.attendanceTime;
                    //    long remainTime = 0;
                    //    if (recoverTime > 0 && recoverTime > midnight)
                    //    {
                    //        // 충전시간이 자정을 넘기면 0 점
                    //        log.Debug("충전시간 {0} > 자정 {1}",
                    //            Parser.Time.GetDateTime(recoverTime).ToString("MM-dd HH:mm"), 
                    //            Parser.Time.GetDateTime(UserData.attendanceTime).ToString("MM-dd HH:mm"));
                    //        return remainBp;
                    //    }
                    //    remainTime = Parser.Time.GetTodayRemainMs(UserData.attendanceTime);
                    //    if (recoverTime > 0)
                    //    {
                    //        if (now + Parser.Time.GetRemainMs(recoverTime) < UserData.attendanceTime)
                    //            remainBp = 1;
                    //        remainTime -= Parser.Time.GetRemainMs(recoverTime);
                    //    }
                    //    log.Debug("자정까지 남은 시간 {0} ms, {1}", remainTime, Parser.Long.ParseMS(remainTime));
                    //    remainBp += Convert.ToInt32(remainTime / (Parser.Time.HOUR * 2));
                    //    if (remainBp > 13)
                    //    {
                    //        log.Debug("남은 시간에 문제가 있음 - 충전가능 모의점수 0");
                    //        remainBp = 0;
                    //    }
                    //}
                    //else
                    //{
                    //    log.Debug("출석시간 존재하지 않음");
                    //}
                }
                catch (Exception ex)
                {
                    log.Error(ex, "failed to get today remain bp");
                }
                return remainBp;
            }

            /// <summary>
            /// 초기화
            /// </summary>
            public static void Clear()
            {
                point = -1;
                recoverTime = -1;
                lastRecoverTime = -1;
                notified = false;
            }
        }

        /// <summary>
        /// 기지시설
        /// </summary>
        public static class Facility
        {
            /// <summary>
            /// 구호소
            /// </summary>
            public static class RescueStation
            {

            }

            /// <summary>
            /// 자료실
            /// </summary>
            public static class DataRoom
            {
                /// <summary>
                /// 데이터 테이블
                /// (자유경험치 누적 상한)
                /// </summary>
                private static int _dataTableLv = -1;
                public static int dataTableLevel
                {
                    get { return _dataTableLv; }
                    set
                    {
                        _dataTableLv = value;
                        if (Util.Common.IsBetween(value, 0, 10))
                        {
                            UserData.GlobalExp.maxExp = GameData.Facility.DataRoom.GetMaxSurplusExp(value);
                        }
                    }
                }

                /// <summary>
                /// 서버 
                /// (작전보고서 작성 시간)
                /// </summary>
                private static int _serverLv = -1;
                public static int serverLevel
                {
                    get { return _serverLv; }
                    set
                    {
                        _serverLv = value;
                        if (Util.Common.IsBetween(value, 0, 10))
                        {
                            UserData.BattleReport.requireTime = GameData.Facility.DataRoom.GetCombatReportTime(value);
                        }
                    }
                }
            }

            /// <summary>
            /// 요정의 방
            /// </summary>
            public static class FairyChamber
            {

            }

            /// <summary>
            /// 카페
            /// </summary>
            public static class Cafe
            {

            }

            /// <summary>
            /// 정보 센터
            /// </summary>
            public static class IntelCenter
            {
                /// <summary>
                /// 분석기 레벨
                /// (정보분석 소요시간)
                /// </summary>
                private static int _analyzerLevel = -1;
                public static int analyzerLevel
                {
                    get { return _analyzerLevel; }
                    set
                    {
                        _analyzerLevel = value;
                        if (Util.Common.IsBetween(value, 0, 10))
                        {
                            DataAnalysis.requireTime = GameData.Facility.IntelCenter.GetDataAnalysisTime(value);
                        }
                    }
                }
            }

            /// <summary>
            /// 격납고
            /// </summary>
            public static class Garage
            {

            }

            /// <summary>
            /// 전진 기지
            /// </summary>
            public static class Basecamp
            {

            }

            public static void Clear()
            {
                IntelCenter.analyzerLevel = -1;
                DataRoom.dataTableLevel = -1;
                DataRoom.serverLevel = -1;
            }
        }

        /// <summary>
        /// 자유경험치
        /// </summary>
        public static class GlobalExp
        {
            public static void OnStaticPropertyChanged([CallerMemberName] string name = "")
            {
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
            }
            public static event PropertyChangedEventHandler StaticPropertyChanged;

            /// <summary>
            /// 자유경험치
            /// </summary>
            public static int exp
            {
                get { return _exp; }
                set
                {
                    MainWindow.view.ShowFreeExpGrid();  // 자유경험치 보이기
                    if (_exp == value)
                        return;
                    _exp = value;
                    if (maxExp > 0 && value > maxExp)
                        _exp = maxExp;
                    percent = Convert.ToInt32(Math.Truncate((double)exp / (double)maxExp * 100));
                    //if (value >= maxExp && maxExp > 0)
                    OnStaticPropertyChanged();
                }
            }
            private static int _exp = -1;

            /// <summary>
            /// 자유경험치 상한
            /// </summary>
            public static int maxExp
            {
                get { return _maxExp; }
                set
                {
                    if (_maxExp == value)
                        return;
                    _maxExp = value;
                    percent = Convert.ToInt32(Math.Truncate((double)exp / (double)maxExp * 100));
                    OnStaticPropertyChanged();
                }
            }
            private static int _maxExp = -1;

            /// <summary>
            /// 자유경험치 퍼센트 
            /// </summary>
            public static int percent
            {
                get { return _percent; }
                set
                {
                    if (_percent == value || value < 0 || 100 < value)
                        return;
                    _percent = value;
                    if (value == 100)
                        MainWindow.view.isMaxGlobalExp = true;
                    else
                        MainWindow.view.isMaxGlobalExp = false;
                    OnStaticPropertyChanged();
                }
            }
            private static int _percent = 0;

            /// <summary>
            /// 알림 여부
            /// </summary>
            public static bool notified = false;

            /// <summary>
            /// 초기화
            /// </summary>
            public static void Clear()
            {
                exp = -1;
                maxExp = -1;
                notified = false;
            }
        }

        /// <summary>
        /// 임무
        /// </summary>
        public static class Quest
        {
            /// <summary>
            /// 임무 메뉴 열림 여부
            /// (임무 메뉴가 열린 적 없다면 임무 갱신 막기)
            /// </summary>
            private static bool _isOpenQuestMenu = false;
            public static bool isOpenQuestMenu
            {
                get
                {
                    return _isOpenQuestMenu;
                }
                set
                {
                    _isOpenQuestMenu = value;
                    MainWindow.view.SetIconEnable(Menus.QUEST, value);
                }
            }
            /// <summary>
            /// 알림 활성화 여부
            /// (false면 조건이 달성되어도 알리지 않는다)
            /// </summary>
            public static bool isNotify = false;

            /// <summary>
            /// 초기화
            /// (일간, 주간 임무만)
            /// </summary>
            public static void Init()
            {
                MainWindow.questView.Clear();

                List<QuestTemplate> quests = new List<QuestTemplate>();

                /// 일간임무
                quests.Add(new QuestTemplate() { id = 101, count = Daily.mission });
                quests.Add(new QuestTemplate() { id = 102, count = Daily.mission });
                quests.Add(new QuestTemplate() { id = 103, count = Daily.mission });
                quests.Add(new QuestTemplate() { id = 104, count = Daily.produceDoll });
                quests.Add(new QuestTemplate() { id = 105, count = Daily.produceDoll });
                quests.Add(new QuestTemplate() { id = 106, count = Daily.operation });
                quests.Add(new QuestTemplate() { id = 107, count = Daily.operation });
                quests.Add(new QuestTemplate() { id = 108, count = Daily.operation });
                quests.Add(new QuestTemplate() { id = 109, count = Daily.eatGun });
                quests.Add(new QuestTemplate() { id = 110, count = Daily.eatGun });
                quests.Add(new QuestTemplate() { id = 111, count = Daily.fixGun });
                quests.Add(new QuestTemplate() { id = 112, count = Daily.fixGun });
                quests.Add(new QuestTemplate() { id = 113, count = Daily.fixGun });
                quests.Add(new QuestTemplate() { id = 115, count = Daily.combatSim });
                quests.Add(new QuestTemplate() { id = 116, count = Daily.combatSim });
                quests.Add(new QuestTemplate() { id = 117, count = Daily.produceEquip });
                quests.Add(new QuestTemplate() { id = 118, count = Daily.produceEquip });
                quests.Add(new QuestTemplate() { id = 125, count = Daily.eatEquip });
                quests.Add(new QuestTemplate() { id = 126, count = Daily.eatEquip });
                quests.Add(new QuestTemplate() { id = 127, count = Daily.getBattery });
                quests.Add(new QuestTemplate() { id = 128, count = Daily.callReinforce });
                quests.Add(new QuestTemplate() { id = 129, count = Daily.dataAnalysis });
                quests.Add(new QuestTemplate() { id = 130, count = Daily.dataAnalysis });

                /// 주간임무
                quests.Add(new QuestTemplate() { id = 201, count = Weekly.fixGun });
                quests.Add(new QuestTemplate() { id = 202, count = Weekly.killMech });
                quests.Add(new QuestTemplate() { id = 203, count = Weekly.killDoll });
                quests.Add(new QuestTemplate() { id = 204, count = Weekly.killBoss });
                quests.Add(new QuestTemplate() { id = 205, count = Weekly.operation });
                quests.Add(new QuestTemplate() { id = 206, count = Weekly.sBattle });
                quests.Add(new QuestTemplate() { id = 207, count = Weekly.eatGun });
                quests.Add(new QuestTemplate() { id = 208, count = Weekly.produceDoll });
                quests.Add(new QuestTemplate() { id = 209, count = Weekly.skillTrain });
                quests.Add(new QuestTemplate() { id = 210, count = Weekly.combatSim });
                quests.Add(new QuestTemplate() { id = 211, count = Weekly.produceEquip });
                quests.Add(new QuestTemplate() { id = 212, count = Weekly.killArmorMech });
                quests.Add(new QuestTemplate() { id = 213, count = Weekly.killArmorDoll });
                quests.Add(new QuestTemplate() { id = 214, count = Weekly.produceHeavyDoll });
                quests.Add(new QuestTemplate() { id = 215, count = Weekly.adjustEquip });
                quests.Add(new QuestTemplate() { id = 216, count = Weekly.eatEquip });
                quests.Add(new QuestTemplate() { id = 217, count = Weekly.produceHeavyEquip });
                quests.Add(new QuestTemplate() { id = 218, count = Weekly.adjustFairy });
                quests.Add(new QuestTemplate() { id = 219, count = Weekly.eatFairy });
                quests.Add(new QuestTemplate() { id = 220, count = Weekly.dataAnalysis });
                quests.Add(new QuestTemplate() { id = 221, count = Weekly.eatChip });

                foreach (QuestTemplate quest in quests)
                {
                    if (quest.TBIsCompleted != "1")
                        MainWindow.questView.Add(quest);
                }

                MainWindow.questView.Sort();
            }

            /// <summary>
            /// 일간임무
            /// </summary>
            public static class Daily
            {
                private static int _mission = 0;        // 전역승리
                public static int mission
                {
                    get
                    {
                        return _mission;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _mission = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("mission", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 9:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_win,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_103"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _produceDoll = 0;    // 인형제조
                public static int produceDoll
                {
                    get
                    {
                        return _produceDoll;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _produceDoll = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("produce_doll", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 4:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_produce_doll,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_105"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _produceEquip = 0;   // 장비제조
                public static int produceEquip
                {
                    get
                    {
                        return _produceEquip;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _produceEquip = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("produce_equip", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 4:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_produce_equip,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_118"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _operation = 0;      // 군수지원
                public static int operation
                {
                    get
                    {
                        return _operation;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _operation = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("operation", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 14:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_operation,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_108"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _eatGun = 0;         // 인형강화
                public static int eatGun
                {
                    get
                    {
                        return _eatGun;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _eatGun = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("eat_gun", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 4:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_eat_doll,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_110"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _eatEquip = 0;       // 장비강화
                public static int eatEquip
                {
                    get
                    {
                        return _eatEquip;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _eatEquip = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("eat_equip", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 4:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_eat_equip,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_126"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _fixGun = 0;         // 인형수복
                public static int fixGun
                {
                    get
                    {
                        return _fixGun;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _fixGun = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("fix_gun", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 28:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_fix_doll,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_113"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _combatSim = 0;      // 모의작전
                public static int combatSim
                {
                    get
                    {
                        return _combatSim;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _combatSim = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("combat_sim", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 4:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_combat_sim,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_116"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _getBattery = 0;     // 공유전지
                public static int getBattery
                {
                    get
                    {
                        return _getBattery;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _getBattery = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("get_battery", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 5:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_get_battery,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_127"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _callReinforce = 0;  // 친구제대
                public static int callReinforce
                {
                    get
                    {
                        return _callReinforce;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _callReinforce = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("call_reinforce", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 5:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_reinforce,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_128"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _dataAnalysis = 0;   // 정보분석
                public static int dataAnalysis
                {
                    get
                    {
                        return _dataAnalysis;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _dataAnalysis = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("data_analysis", "daily", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 4:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_daily_data_analysis,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_DAILY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_130"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// 주간임무
            /// </summary>
            public static class Weekly
            {
                private static int _sBattle = 0;           // S랭크 전투 (100회)
                public static int sBattle
                {
                    get
                    {
                        return _sBattle;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _sBattle = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("s_battle", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 100:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_s_battle,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_206"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _fixGun = 0;             // 인형수복 (100회)
                public static int fixGun
                {
                    get
                    {
                        return _fixGun;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _fixGun = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("fix_gun", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 100:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_fix_gun,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_201"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _combatSim = 0;          // 모의작전 (20회)
                public static int combatSim
                {
                    get
                    {
                        return _combatSim;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _combatSim = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("combat_sim", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 20:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_combat_sim,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_210"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _killMech = 0;           // 철혈기계 처치 (200기)
                public static int killMech
                {
                    get
                    {
                        return _killMech;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _killMech = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill_mech", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 200:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_kill_mech,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_202"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _killDoll = 0;           // 철혈인형 처치 (200기)
                public static int killDoll
                {
                    get
                    {
                        return _killDoll;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _killDoll = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill_doll", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 200:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_kill_doll,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_203"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _killBoss = 0;           // 철혈보스 처치 (10기)
                public static int killBoss
                {
                    get
                    {
                        return _killBoss;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _killBoss = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill_boss", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 10:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_kill_boss,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_204"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _operation = 0;          // 군수지원 (50회)
                public static int operation
                {
                    get
                    {
                        return _operation;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _operation = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("operation", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 50:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_operation,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_205"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _eatChip = 0;            // 칩셋강화 (20회)
                public static int eatChip
                {
                    get
                    {
                        return _eatChip;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _eatChip = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("eat_chip", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 20:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_eat_chip,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_221"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _eatGun = 0;             // 인형강화 (20회)
                public static int eatGun
                {
                    get
                    {
                        return _eatGun;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _eatGun = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("eat_gun", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 20:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_eat_gun,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_207"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _eatEquip = 0;           // 장비강화 (20회)
                public static int eatEquip
                {
                    get
                    {
                        return _eatEquip;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _eatEquip = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("eat_equip", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 20:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_eat_equip,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_216"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _adjustEquip = 0;        // 장비교정 (5회)
                public static int adjustEquip
                {
                    get
                    {
                        return _adjustEquip;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _adjustEquip = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("adjust_equip", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 5:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_adjust_equip,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_215"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _eatFairy = 0;           // 요정강화 (1회)
                public static int eatFairy
                {
                    get
                    {
                        return _eatFairy;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _eatFairy = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("eat_fairy", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 1:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_eat_fairy,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_219"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _adjustFairy = 0;        // 요정교정 (3회)
                public static int adjustFairy
                {
                    get
                    {
                        return _adjustFairy;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _adjustFairy = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("adjust_fairy", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 3:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_adjust_fairy,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_218"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _produceDoll = 0;        // 인형제조 (20회)
                public static int produceDoll
                {
                    get
                    {
                        return _produceDoll;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _produceDoll = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("produce_doll", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 20:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_produce_doll,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_208"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _produceEquip = 0;       // 장비제조 (20회)
                public static int produceEquip
                {
                    get
                    {
                        return _produceEquip;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _produceEquip = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("produce_equip", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 20:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_produce_equip,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_211"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _produceHeavyDoll = 0;   // 중형인형제조 (1회)
                public static int produceHeavyDoll
                {
                    get
                    {
                        return _produceHeavyDoll;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _produceHeavyDoll = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("produce_heavy_doll", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 1:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_produce_heavy_doll,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_214"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _produceHeavyEquip = 0;  // 중형장비제조 (3회)
                public static int produceHeavyEquip
                {
                    get
                    {
                        return _produceHeavyEquip;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _produceHeavyEquip = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("produce_heavy_equip", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 3:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_produce_heavy_equip,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_217"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _skillTrain = 0;         // 스킬훈련 (5회)
                public static int skillTrain
                {
                    get
                    {
                        return _skillTrain;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _skillTrain = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("skill_train", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 5:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_skill_train,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_209"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _killArmorMech = 0;      // 철혈장갑기계 처치 (200기)
                public static int killArmorMech
                {
                    get
                    {
                        return _killArmorMech;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _killArmorMech = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill_armor_mech", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 200:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_kill_armor_mech,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_212"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _killArmorDoll = 0;      // 쳘혈장갑인형 처치 (200기)
                public static int killArmorDoll
                {
                    get
                    {
                        return _killArmorDoll;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _killArmorDoll = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill_armor_doll", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 200:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_kill_armor_doll,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_213"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _dataAnalysis = 0;       // 정보분석 (20회)
                public static int dataAnalysis
                {
                    get
                    {
                        return _dataAnalysis;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            _dataAnalysis = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("data_analysis", "weekly", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 20:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_weekly_data_analysis,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_WEEKLY_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_220"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// 정보임무
            /// </summary>
            public static class Research
            {
                private static int _mission = 999;            // 전역승리
                public static int mission
                {
                    get
                    {
                        return _mission;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_mission < 999)
                                log.Debug("정보임무 갱신 - 아무 전역승리 {0} 회", value);
                            _mission = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("mission", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 1:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_mission,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_301"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _kill = 999;               // 아무 적 처치
                public static int kill
                {
                    get
                    {
                        return _kill;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_kill < 999)
                                log.Debug("정보임무 갱신 - 아무 적 처치 {0} 회", value);    
                            _kill = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 5:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_kill,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_302"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _killBoss = 999;           // 철혈보스 처치
                public static int killBoss
                {
                    get
                    {
                        return _killBoss;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_killBoss < 999)
                                log.Debug("정보임무 갱신 - 철혈보스 처치 {0} 회", value);
                            _killBoss = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill_boss", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 1:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_kill_boss,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_307"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _killArmorDoll = 999;      // 철혈장갑인형 처치
                public static int killArmorDoll
                {
                    get
                    {
                        return _killArmorDoll;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_killArmorDoll < 999)
                                log.Debug("정보임무 갱신 - 철혈장갑인형 처치 {0} 회", value);
                            _killArmorDoll = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill_armor_doll", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 3:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_kill_armor_doll,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_308"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _killArmorMech = 999;      // 철혈장갑기계 처치
                public static int killArmorMech
                {
                    get
                    {
                        return _killArmorMech;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_killArmorMech < 999)
                                log.Debug("정보임무 갱신 - 철혈장갑기계 처치 {0} 회", value);
                            _killArmorMech = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill_armor_mech", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 3:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_kill_armor_mech,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_309"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _killDoll = 999;           // 철혈인형 처치
                public static int killDoll
                {
                    get
                    {
                        return _killDoll;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_killDoll < 999)
                                log.Debug("정보임무 갱신 - 철혈인형 처치 {0} 회", value);
                            _killDoll = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill_doll", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 3:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_kill_doll,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_310"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _killMech = 999;           // 철혈기계 처치
                public static int killMech
                {
                    get
                    {
                        return _killMech;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_killMech < 999)
                                log.Debug("정보임무 갱신 - 철혈기계 처치 {0} 회", value);
                            _killMech = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("kill_mech", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 3:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_kill_mech,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_311"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _mission46 = 999;          // 일반 4-6 승리
                public static int mission46
                {
                    get
                    {
                        return _mission46;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_mission46 < 999)
                                log.Debug("정보임무 갱신 - 전역 4-6 승리 {0} 회", value);
                            _mission46 = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("mission_46", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 2:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_mission_46,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_303"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _mission56 = 999;          // 일반 5-6 승리
                public static int mission56
                {
                    get
                    {
                        return _mission56;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_mission56 < 999)
                                log.Debug("정보임무 갱신 - 전역 5-6 승리 {0} 회", value);
                            _mission56 = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("mission_56", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 2:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_mission_56,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_304"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _mission66 = 999;          // 일반 6-6 승리
                public static int mission66
                {
                    get
                    {
                        return _mission66;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_mission66 < 999)
                                log.Debug("정보임무 갱신 - 전역 6-6 승리 {0} 회", value);
                            _mission66 = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("mission_66", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 1:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_mission_66,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_305"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _mission76 = 999;          // 일반 7-6 승리
                public static int mission76
                {
                    get
                    {
                        return _mission76;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_mission76 < 999)
                                log.Debug("정보임무 갱신 - 전역 7-6 승리 {0} 회", value);
                            _mission76 = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("mission_76", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 1:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_mission_76,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_306"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _missionNormal = 999;      // 일반 1회 승리
                public static int missionNormal
                {
                    get
                    {
                        return _missionNormal;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_missionNormal < 999)
                                log.Debug("정보임무 갱신 - 일반 전역승리 {0} 회", value);
                            _missionNormal = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("mission_normal", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 1:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_mission_normal,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_312"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _missionEmergency = 999;   // 긴급 1회 승리
                public static int missionEmergency
                {
                    get
                    {
                        return _missionEmergency;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_missionEmergency < 999)
                                log.Debug("정보임무 갱신 - 긴급 전역승리 {0} 회", value);
                            _missionEmergency = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("mission_emergency", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 1:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_mission_emergency,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_313"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
                private static int _missionNight = 999;       // 야간 1회 승리
                public static int missionNight
                {
                    get
                    {
                        return _missionNight;
                    }
                    set
                    {
                        if (Quest.isOpenQuestMenu)
                        {
                            if (_missionNight < 999)
                                log.Debug("정보임무 갱신 - 야간 전역승리 {0} 회", value);
                            _missionNight = value;
                            Refresh(true);
                            MainWindow.questView.SetCount("mission_night", "research", value);
                            if (Quest.isNotify && Config.Extra.notifyQuestComplete)
                            {
                                switch (value)
                                {
                                    case 1:
                                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                                        {
                                            send = MessageSend.All,
                                            type = MessageType.acheive_research_mission_night,
                                            subject = LanguageResources.Instance["MESSAGE_QUEST_RESEARCH_SUBJECT"],
                                            content = string.Format(LanguageResources.Instance["MESSAGE_QUEST_CONTENT"],
                                                                    LanguageResources.Instance["QUEST_314"])
                                        });
                                        MainWindow.view.SetIconNotify(Menus.QUEST, true);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// 새로고침
            /// </summary>
            public static void Refresh(bool visible = false)
            {
                if (Quest.Daily.mission >= 8
                    && Quest.Daily.produceDoll >= 4
                    && Quest.Daily.produceEquip >= 4
                    && Quest.Daily.operation >= 14
                    && Quest.Daily.eatGun >= 4
                    && Quest.Daily.eatEquip >= 4
                    && Quest.Daily.fixGun >= 28
                    && Quest.Daily.combatSim >= 4
                    && Quest.Daily.getBattery >= 5
                    && Quest.Daily.callReinforce >= 5
                    && Quest.Daily.dataAnalysis >= 4)
                {
                    if (MainWindow.questView.researchList.Count() == 0)
                        MainWindow.view.CompleteQuestTab = true;
                    else
                        MainWindow.view.CompleteQuestTab = false;
                }
                else
                {
                    MainWindow.view.CompleteQuestTab = false;
                }
            }

            /// <summary>
            /// 일간, 주간임무 비우기
            /// </summary>
            public static void Clear()
            {
                Quest.isNotify = false;

                Quest.Daily.mission = 0;
                Quest.Daily.produceDoll = 0;
                Quest.Daily.produceEquip = 0;
                Quest.Daily.operation = 0;
                Quest.Daily.eatGun = 0;
                Quest.Daily.eatEquip = 0;
                Quest.Daily.fixGun = 0;
                Quest.Daily.combatSim = 0;
                Quest.Daily.getBattery = 0;
                Quest.Daily.callReinforce = 0;
                Quest.Daily.dataAnalysis = 0;

                Quest.Weekly.adjustEquip = 0;
                Quest.Weekly.adjustFairy = 0;
                Quest.Weekly.combatSim = 0;
                Quest.Weekly.dataAnalysis = 0;
                Quest.Weekly.eatChip = 0;
                Quest.Weekly.eatEquip = 0;
                Quest.Weekly.eatFairy = 0;
                Quest.Weekly.eatGun = 0;
                Quest.Weekly.fixGun = 0;
                Quest.Weekly.killArmorDoll = 0;
                Quest.Weekly.killArmorMech = 0;
                Quest.Weekly.killBoss = 0;
                Quest.Weekly.killDoll = 0;
                Quest.Weekly.killMech = 0;
                Quest.Weekly.operation = 0;
                Quest.Weekly.produceDoll = 0;
                Quest.Weekly.produceEquip = 0;
                Quest.Weekly.produceHeavyDoll = 0;
                Quest.Weekly.produceHeavyEquip = 0;
                Quest.Weekly.sBattle = 0;
                Quest.Weekly.skillTrain = 0;

                Quest.Refresh(false);
                MainWindow.questView.Clear();
                MainWindow.questView.CheckAll();

                Quest.isOpenQuestMenu = false;
            }

            /// <summary>
            /// 정보임무 비우기
            /// </summary>
            public static void ClearResearch()
            {
                Quest.isNotify = false;

                Quest.Research.kill = 999;
                Quest.Research.killArmorDoll = 999;
                Quest.Research.killArmorMech = 999;
                Quest.Research.killBoss = 999;
                Quest.Research.killDoll = 999;
                Quest.Research.killMech = 999;
                Quest.Research.mission = 999;
                Quest.Research.mission46 = 999;
                Quest.Research.mission56 = 999;
                Quest.Research.mission66 = 999;
                Quest.Research.mission76 = 999;
                Quest.Research.missionNormal = 999;
                Quest.Research.missionEmergency = 999;
                Quest.Research.missionNight = 999;

                Quest.Refresh(false);
                MainWindow.questView.ClearResearch();
                MainWindow.questView.CheckAll();

                Quest.isOpenQuestMenu = false;
            }
        }

        /// <summary>
        /// 작전보고서
        /// </summary>
        public static class BattleReport
        {
            public static void OnStaticPropertyChanged([CallerMemberName] string name = "")
            {
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
            }
            public static event PropertyChangedEventHandler StaticPropertyChanged;

            /// <summary>
            /// 작전보고서 갯수
            /// </summary>
            public static int num
            {
                get { return _num; }
                set
                {
                    _num = value;
                }
            }
            private static int _num = -1;

            /// <summary>
            /// 작전보고서 남은 시간
            /// </summary>
            public static string remainTime
            {
                get { return _remainTime; }
                set
                {
                    if (_remainTime == value)
                        return;
                    _remainTime = value;
                    if (value == "00:00:00")
                        MainWindow.view.isCompleteBattleReport = true;
                    else
                        MainWindow.view.isCompleteBattleReport = false;
                    OnStaticPropertyChanged();
                }
            }
            private static string _remainTime = "00:00:00";

            /// <summary>
            /// 작전보고서 소요시간
            /// </summary>
            public static int requireTime
            {
                get { return _requireTime; }
                set
                {
                    _requireTime = value;
                }
            }
            private static int _requireTime = 0;

            /// <summary>
            /// 작전보고서 시작 시간
            /// </summary>
            public static int startTime
            {
                get
                {
                    return _startTime;
                }
                set
                {
                    _startTime = value;
                    //log.Debug("작전보고서 시작시간 {0}, 소요시간 {1}", Parser.Time.GetDateTime(value).ToString("MM-dd HH:mm:ss"), Parser.Long.ParseMS(_requireTime));
                    if (value != 0 && _requireTime != 0)
                    {
                        endTime = startTime + requireTime;
                        //log.Debug("작전보고서 작성 예정완료시간 {0}", Parser.Time.GetDateTime(endTime).ToString("MM-dd HH:mm"));
                    }
                }
            }
            private static int _startTime = 0;

            /// <summary>
            /// 작전보고서 완료 시간
            /// </summary>
            public static int endTime
            {
                get { return _endTime; }
                set
                {
                    _endTime = value;
                }
            }
            private static int _endTime = 0;

            /// <summary>
            /// 알림 여부
            /// </summary>
            public static bool notified = false;

            /// <summary>
            /// 리셋
            /// (작전보고서 작성완료)
            /// </summary>
            public static void Reset()
            {
                num = -1;
                startTime = 0;
                endTime = 0;
                notified = false;
            }

            /// <summary>
            /// 초기화
            /// </summary>
            public static void Clear()
            {
                num = -1;
                requireTime = 0;
                startTime = 0;
                endTime = 0;
                notified = false;
            }
        }

        /// <summary>
        /// 정보분석
        /// </summary>
        public static class DataAnalysis
        {
            /// <summary>
            /// 분석시간
            /// </summary>
            private static int _requireTime = 0; 
            public static int requireTime
            {
                get { return _requireTime; }
                set
                {
                    _requireTime = value;
                    //log.Debug("정보분석 소요시간 {0}", Parser.Long.ParseMS(value));
                }
            }

            /// <summary>
            /// 비우기
            /// </summary>
            public static void Clear()
            {
                requireTime = 0;
            }
        }

        /// <summary>
        /// 진행 중 전역
        /// </summary>
        public static class CurrentMission
        {
            /// <summary>
            /// 전역 ID
            /// </summary>
            private static int _id = -1;
            public static int id
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
                        JObject data = GameData.Mission.GetData(value);
                        if (data != null)
                        {
                            location = Parser.Json.ParseString(data["location"]);
                            MainWindow.view.CurrentMission = location;
                        }
                    } 
                    else
                    {
                        MainWindow.view.CurrentMission = "";
                    }
                }
            }
            /// <summary>
            /// 전역 명
            /// </summary>
            public static string location = "";
            /// <summary>
            /// 전역 중 전투 횟수
            /// </summary>
            public static int battleCount = 0;
            /// <summary>
            /// 전역 중 이동 횟수
            /// </summary>
            public static int moveCount = 0;
            /// <summary>
            /// 현재 턴
            /// </summary>
            public static int turnCount = 0;

            /// <summary>
            /// 제대 위치 정보
            /// team_id, spot_id
            /// </summary>
            public static Dictionary<int, int> teamSpots = new Dictionary<int, int>();

            /// <summary>
            /// 적 제대 위치 정보
            /// team_id, spot_id
            /// </summary>
            //public static Dictionary<long, int> enemySpots = new Dictionary<long, int>();
            public static Dictionary<int, long> enemySpots = new Dictionary<int, long>();

            /// <summary>
            /// 거점 정보
            /// spot_id, belong
            /// </summary>
            public static Dictionary<int, int> belongSpots = new Dictionary<int, int>();

            /// <summary>
            /// 중장비 인스턴스
            /// 
            /// </summary>
            public static Dictionary<int, long> squadInstance = new Dictionary<int, long>();

            /// <summary>
            /// 아군 인스턴스
            /// (제 3세력)
            /// <ally_instance_id, ally_enemy>
            /// </summary>
            public static Dictionary<int, long> allyInstance = new Dictionary<int, long>();

            /// <summary>
            /// 아군 제대 배치
            /// </summary>
            /// <param name="teamId"></param>
            /// <returns></returns>
            public static void ReinforceTeam(int teamId, int spotId)
            {
                log.Debug("아군 {0} 제대 배치 - 위치 {1}", teamId, spotId);
                MainWindow.echelonView.SetTeamStatus(teamId, EchelonStatus.Mission);
                if (teamSpots.ContainsKey(teamId))
                {
                    log.Warn("아군 {0} 제대가 이미 전역에 존재함 - 위치 {1}", teamId, teamSpots[teamId]);
                    teamSpots.Remove(teamId);
                }
                teamSpots.Add(teamId, spotId);
            }

            /// <summary>
            /// 중장비 제대 배치
            /// </summary>
            /// <param name="squadInstanceId"></param>
            /// <param name="squadWithUserId"></param>
            /// <param name="spotId"></param>
            public static void ReinforceSquad(int squadInstanceId, long squadWithUserId, int spotId)
            {
                log.Debug("중장비 {0}({1}) 제대 배치 - 위치 {2}", squadInstanceId, squadWithUserId, spotId);
                if (squadInstance.ContainsKey(squadInstanceId))
                {
                    log.Warn("중장비 {0}({1}) 제대가 이미 전역에 존재함 - 위치 {2}", squadInstanceId, squadWithUserId, spotId);
                    squadInstance.Remove(squadInstanceId);
                }
                squadInstance.Add(squadInstanceId, squadWithUserId);
            }

            /// <summary>
            /// 아군 제대 이동
            /// </summary>
            /// <param name="teamId"></param>
            /// <param name="toSpotId"></param>
            public static void MoveTeam(int teamId, int toSpotId)
            {
                if (teamSpots.ContainsKey(teamId))
                {
                    teamSpots[teamId] = toSpotId;
                    log.Debug("아군 {0} 제대 이동 - 위치 {1}", teamId, toSpotId);
                }
                else
                {
                    log.Warn("아군 {0} 제대 이동 실패 - 위치 {1}", teamId, toSpotId);
                }
                //moveCount++;
            }

            /// <summary>
            /// 아군 제대 위치 가져오기
            /// </summary>
            /// <param name="spotId"></param>
            public static int GetTeamId(int spotId)
            {
                foreach(KeyValuePair<int, int> item in teamSpots)
                {
                    if (item.Value == spotId)
                    {
                        return item.Key;
                    }
                }
                return 0;
            }

            /// <summary>
            /// 적 제대 이동
            /// </summary>
            /// <param name="enemy_team_id"></param>
            /// <param name="to_spot_id"></param>
            public static void MoveEnemyTeam(long enemy_team_id, int from_spot_id, int to_spot_id)
            {
                if (enemySpots.ContainsKey(to_spot_id))
                {
                    enemySpots[to_spot_id] = enemy_team_id;
                    if (!MainWindow.view.forceStop)
                        log.Debug("적 {0} 제대 이동 - 위치 {1}", enemy_team_id, to_spot_id);
                }
                else
                {
                    enemySpots.Add(to_spot_id, enemy_team_id);
                    if (!MainWindow.view.forceStop)
                        log.Debug("적 {0} 제대 이동 - 위치 {1}", enemy_team_id, to_spot_id);
                }
            }
            public static void MoveEnemyTeam(JArray enemyMoveInfo)
            {
                #region Packet Example
                /*
                // 주간
                "enemy_move": [
                    {
                        "from_spot_id": "135",
                        "to_spot_id": "139",
                        "enemy_ai": 0,
                        "enemy_ai_para": "",
                        "hostage_id": "0",
                        "hostage_hp": "0",
                        "squad_instance_id": "0"
                    },
                    ...
                // 야간
                */
                #endregion

                Dictionary<int, long> tempEnemySpots = new Dictionary<int, long>();
                foreach (KeyValuePair<int, long> enemySpot in enemySpots)
                {
                    tempEnemySpots.Add(enemySpot.Key, enemySpot.Value);
                }
                if (enemyMoveInfo != null)
                {
                    foreach (JObject enemyMove in enemyMoveInfo)
                    {
                        int from_spot_id = Parser.Json.ParseInt(enemyMove["from_spot_id"]);
                        int to_spot_id = Parser.Json.ParseInt(enemyMove["to_spot_id"]);
                        long enemy_team_id = -9999;
                        if (enemyMove.ContainsKey("enemy_team_id"))
                            enemy_team_id = Parser.Json.ParseLong(enemyMove["enemy_team_id"]);
                        else if (enemyMove.ContainsKey("enemy_instance_id"))
                            enemy_team_id = Parser.Json.ParseLong(enemyMove["enemy_instance_id"]);
                        else if (enemySpots.ContainsKey(from_spot_id))
                            enemy_team_id = enemySpots[from_spot_id];
                        if (enemy_team_id != -9999)
                        {
                            log.Debug("적 {0} 제대 이동 - 위치 {1} -> {2}", enemy_team_id, from_spot_id, to_spot_id);
                            if (tempEnemySpots.ContainsKey(to_spot_id))
                                tempEnemySpots[to_spot_id] = enemy_team_id;
                            else
                                tempEnemySpots.Add(to_spot_id, enemy_team_id);
                        }
                    }
                    enemySpots = tempEnemySpots;
                }
            }
            /// <summary>
            /// 적 제대 스폰
            /// </summary>
            /// <param name="grow_enemy"></param>
            public static void SpawnEnemyTeam(JArray grow_enemy)
            {
                #region Packet Example
                /*
                    "grow_enemy": [
                        {
                            "spot_id": "267",
                            "enemy_team_id": "267",
                            "enemy_instance_id": "21"
                        },
                        {
                            "spot_id": "269",
                            "enemy_team_id": "268",
                            "enemy_instance_id": "22"
                        },
                        {
                            "spot_id": "272",
                            "enemy_team_id": "266",
                            "enemy_instance_id": "23"
                        }
                    ],
                 */
                #endregion

                try
                {
                    if (grow_enemy != null)
                    {
                        foreach (JObject item in grow_enemy)
                        {
                            int spot_id = item["spot_id"].Value<int>();
                            long enemy_team_id = item["enemy_team_id"].Value<long>();
                            if (enemySpots.ContainsKey(spot_id))
                                enemySpots[spot_id] = enemy_team_id;
                            else
                                enemySpots.Add(spot_id, enemy_team_id);
                            log.Debug("적 {0} 제대 스폰 - 위치 {1}", enemy_team_id, spot_id);
                        }
                    }
                }
                catch(Exception ex)
                {
                    log.Error(ex, "적 제대 스폰 처리 중 에러 발생");
                }
            }

            /// <summary>
            /// 적 제대 위치 가져오기
            /// </summary>
            /// <param name="spot_id"></param>
            /// <returns></returns>
            public static long GetEnemyTeamId(int spot_id)
            {
                if (enemySpots.ContainsKey(spot_id))
                {
                    return enemySpots[spot_id];
                }
                return 0;
            }

            /// <summary>
            /// 아군 제대 퇴각
            /// </summary>
            /// <param name="spotId"></param>
            public static void WithdrawTeam(int spotId)
            {
                foreach(KeyValuePair<int, int> item in teamSpots)
                {
                    if (item.Value == spotId)
                    {
                        int teamId = item.Key;
                        if (teamSpots.ContainsKey(teamId))
                        {
                            log.Debug("아군 {0} 제대 퇴각 - 위치 {1}", teamId, spotId);
                            teamSpots.Remove(teamId);
                            MainWindow.echelonView.SetTeamStatus(teamId, EchelonStatus.StandBy);
                        }
                        else
                        {
                            log.Warn("아군 {0} 제대 퇴각 실패 - key 값 없음", teamId);
                        }
                        break;
                    }
                }
            }

            /// <summary>
            /// 적 제대 퇴각
            /// </summary>
            /// <param name="spot_id"></param>
            public static void WithdrawEnemyTeam(int spot_id)
            {
                if (enemySpots.ContainsKey(spot_id))
                {
                    log.Debug("적 {0} 제대 퇴각 - 위치 {1}", enemySpots[spot_id], spot_id);
                    enemySpots.Remove(spot_id);
                }
                else
                {
                    log.Warn("적 제대 퇴각할 수 없음 - spot_id {0} 값 없음", spot_id);
                }
            }

            /// <summary>
            /// 모든 제대 퇴각
            /// </summary>
            public static void WithdrawAllTeam()
            {
                foreach(KeyValuePair<int, int> item in teamSpots)
                {
                    int teamId = item.Key;
                    log.Debug("아군 {0} 제대 퇴각", teamId);
                    MainWindow.echelonView.SetTeamStatus(teamId, EchelonStatus.StandBy);
                }
                teamSpots.Clear();
                enemySpots.Clear();
            }

            /// <summary>
            /// 거점 점령 반영하기
            /// </summary>
            /// <param name="spotActInfo"></param>
            public static void SetBelongSpot(JArray spotActInfo, bool clear = false)
            {
                if (clear)
                    belongSpots.Clear();

                if (spotActInfo != null)
                {
                    foreach (JObject item in spotActInfo)
                    {
                        int spot_id = Parser.Json.ParseInt(item["spot_id"]);
                        int belong = Parser.Json.ParseInt(item["belong"]);
                        SetBelongSpot(spot_id, belong);
                    }
                }
            }

            /// <summary>
            /// 거점 점령 반영하기
            /// </summary>
            /// <param name="spot_id"></param>
            /// <param name="belong"></param>
            public static void SetBelongSpot(int spot_id, int belong)
            {
                //log.Debug("거점 {0} 세력 {1} 에 의해 점령됨", spot_id, belong);
                if (belongSpots.ContainsKey(spot_id))
                    belongSpots[spot_id] = belong;
                else
                    belongSpots.Add(spot_id, belong);
            }

            /// <summary>
            /// 거점 점령 가져오기
            /// </summary>
            /// <param name="spot_id"></param>
            /// <returns></returns>
            public static int GetBelongSpot(int spot_id)
            {
                if (belongSpots.ContainsKey(spot_id))
                    return belongSpots[spot_id];
                return 0;
            }

            /// <summary>
            /// 비우기
            /// </summary>
            public static void Clear()
            {
                id = -1;
                location = "";
                battleCount = 0;
                moveCount = 0;
                turnCount = 0;
                WithdrawAllTeam();
                enemySpots.Clear();
                belongSpots.Clear();
            }
        }

        /// <summary>
        /// 소속인형
        /// </summary>
        public static class Doll
        {
            /// <summary>
            /// 인형 (id, data)
            /// </summary>
            public static Dictionary<long, DollWithUserInfo> dictionary = new Dictionary<long, DollWithUserInfo>();

            /// <summary>
            /// 인형 상한 알림 여부
            /// </summary>
            public static bool notified = false;

            /// <summary>
            /// 생성
            /// </summary>
            /// <param name="packet"></param>
            public static void Init(JArray packet)
            {
                Dictionary<long, DollWithUserInfo> tempDictionary = new Dictionary<long, DollWithUserInfo>();

                // 인형 생성
                try
                {
                    if (packet != null)
                    {
                        var items = packet;
                        foreach (JObject item in items)
                        {
                            try
                            {
                                long id = Parser.Json.ParseLong(item["id"]);

                                tempDictionary.Add(id, new DollWithUserInfo(item));
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                        dictionary = tempDictionary;
                        UserData.dollCount = dictionary.Count();
                    }
                }
                catch(Exception ex)
                {
                    log.Error(ex, "소속인형 목록 초기화 에러");
                }

                // 제대 생성
                try
                {
                    foreach (KeyValuePair<long, DollWithUserInfo> item in dictionary)
                    {
                        MainWindow.echelonView.Set(item.Value.team, item.Value.location, item.Value.id);
                    }
                    MainWindow.echelonView.SetVisibleEmptyOverlay(false);
                }
                catch(Exception ex)
                {
                    log.Error(ex, "제대 초기화 에러");
                }

                // 탐색 제대 생성
                try
                {
                    foreach (KeyValuePair<long, DollWithUserInfo> item in dictionary)
                    {
                        if (item.Value.team == 101)
                        {
                            UserData.Doll.exploreTeam.Add(item.Key);
                        }
                    }
                    MainWindow.dashboardView.RefreshExplore();
                }
                catch (Exception ex)
                {
                    log.Error(ex, "탐색 제대 초기화 에러");
                }
            }

            /// <summary>
            /// 인형 정보 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static DollWithUserInfo Get(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    return dictionary[id];
                }
                return null;
            }

            /// <summary>
            /// 전체 인형 정보 가져오기
            /// </summary>
            /// <returns></returns>
            public static Dictionary<long, DollWithUserInfo> GetAll()
            {
                return dictionary;
            }

            /// <summary>
            /// 인형 정보 설정하기
            /// </summary>
            /// <param name="doll"></param>
            public static void Set(DollWithUserInfo doll)
            {
                if (dictionary.ContainsKey(doll.id))
                {
                    dictionary[doll.id] = doll;
                }
                else
                {
                    Add(doll);
                    UserData.dollCount = dictionary.Count();
                }
            }

            /// <summary>
            /// 인형 추가
            /// </summary>
            /// <param name="doll"></param>
            /// <param name="inMission">전역 중 획득</param>
            public static void Add(DollWithUserInfo doll, bool inMission = false)
            {
                dictionary.Add(doll.id, doll);
                log.Debug("인형 {0} ({1}) 획득", doll.name, doll.id);

                // 전역 중 획득
                if (inMission)
                {
                    // 인형상한 알림
                    if (Config.Alarm.notifyMaxDoll && UserData.Doll.notified == false)
                    {
                        if (UserData.maxDollCount <= dictionary.Count())
                        {
                            Notifier.Manager.notifyQueue.Enqueue(new Message()
                            {
                                send = MessageSend.All,
                                type = MessageType.reach_max_doll,
                                subject = LanguageResources.Instance["MESSAGE_MAX_DOLL_SUBJECT"],
                                content = LanguageResources.Instance["MESSAGE_MAX_DOLL_CONTENT"],
                            });
                            UserData.Doll.notified = true;
                        }
                    }

                    // 획득인형 알림
                    if (Config.Alarm.notifyRescueDoll &&
                        Config.Alarm.notifyRescueDollStar <= doll.star &&
                        2 <= doll.star)
                    {
                        string gunName = LanguageResources.Instance[string.Format("DOLL_{0}", doll.no)];
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.All,
                            type = MessageType.mission_get_doll,
                            subject = LanguageResources.Instance["MESSAGE_MISSION_RESCUE_DOLL_SUBJECT"],
                            content = string.Format(LanguageResources.Instance["MESSAGE_MISSION_RESCUE_DOLL_CONTENT"], doll.star, gunName)
                        });
                    }

                    // 획득인형 저장
                    if (Config.Setting.exportRescuedDoll)
                    {
                        Csv.WriteGetDollInfo(doll.no);
                    }
                }
                UserData.dollCount = dictionary.Count();
            }

            /// <summary>
            /// 인형 제거
            /// </summary>
            /// <param name="id"></param>
            public static void Remove(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    dictionary.Remove(id);
                }
                UserData.dollCount = dictionary.Count();
            }

            /// <summary>
            /// 인형 갯수
            /// </summary>
            /// <returns></returns>
            public static int Count()
            {
                return dictionary.Count();
            }

            /// <summary>
            /// 초기화
            /// </summary>
            public static void Clear()
            {
                dictionary.Clear();
                exploreTeam.Clear();
                exploreAdaptiveDolls.Clear();
                exploreAdaptivePets.Clear();
            }

            /// <summary>
            /// 제대편성 변경
            /// </summary>
            /// <param name="teamId"></param>
            /// <param name="location"></param>
            /// <param name="gunId"></param>
            public static void SwapTeam(int teamId, int location, long gunId)
            {
                if (teamId > 10 || teamId < 1)
                {
                    //log.Warn("잘못된 제대 {0} - 인형 {1}", teamId, dictionary[gunId].name);
                    return;
                }
                if (location > 6 || location < 1)
                {
                    //log.Warn("잘못된 제대순번 {0} - 인형 {1}", location, dictionary[gunId].name);
                    return;
                }

                // 추가
                if (gunId > 0)
                {
                    /// 이전에 다른 제대 소속되어 있었다면?
                    int beforeTeamId = 0, beforeLocation = 0;
                    MainWindow.echelonView.FindId(ref beforeTeamId, ref beforeLocation, gunId);

                    /// 설정 위치에 이미 다른 제대원이 있다면?
                    long alreadyGunId = 0;
                    MainWindow.echelonView.FindId(teamId, location, ref alreadyGunId);
                    if (alreadyGunId > 0 && alreadyGunId != gunId)
                    {
                        if (beforeTeamId > 0 && beforeLocation > 0)
                        {
                            dictionary[alreadyGunId].team = beforeTeamId;
                            dictionary[alreadyGunId].location = beforeLocation;
                            MainWindow.echelonView.Set(beforeTeamId, beforeLocation, alreadyGunId);
                            log.Debug("기존 제대원 {0} ({1}) 제대 {2} 위치 {3}", UserData.Doll.GetName(alreadyGunId), alreadyGunId, beforeTeamId, beforeLocation);
                        }
                        else
                        {
                            dictionary[alreadyGunId].team = 0;
                            dictionary[alreadyGunId].location = 0;
                            MainWindow.echelonView.Remove(teamId, location);
                            log.Debug("제대 {0} 위치 {1} 비우기", teamId, location);
                        }
                    }
                    dictionary[gunId].team = teamId;
                    dictionary[gunId].location = location;
                    MainWindow.echelonView.Set(new EchelonTemplate(dictionary[gunId]));

                    log.Debug("신규 제대원 {0} ({1}) 제대 {2} 위치 {3}", UserData.Doll.GetName(gunId), gunId, teamId, location);
                }
                // 제거
                else if (gunId == 0)
                {
                    long alreadyGunId = 0;
                    MainWindow.echelonView.FindId(teamId, location, ref alreadyGunId);
                    if (alreadyGunId > 0)
                    {
                        dictionary[alreadyGunId].team = 0;
                        dictionary[alreadyGunId].location = 0;
                    }
                    MainWindow.echelonView.Remove(teamId, location);

                    log.Debug("제대 {0} 위치 {1} 비우기", teamId, location);
                }
            }

            /// <summary>
            /// 제대 리더 인형 고유 ID 가져오기
            /// </summary>
            /// <param name="teamId"></param>
            /// <returns></returns>
            public static long GetTeamLeaderGunWithUserId(int teamId)
            {
                try
                {
                    long gun_with_user_id = MainWindow.echelonView.FindTeamLeaderId(teamId);
                    return gun_with_user_id;
                }
                catch(Exception ex)
                {
                    log.Error(ex, "제대 리더 가져오기 에러");
                }
                return 0;
            }

            /// <summary>
            /// 제대 교환
            /// </summary>
            /// <param name="teamA"></param>
            /// <param name="teamB"></param>
            public static void ExchangeTeam(int teamA, int teamB)
            {
                //log.Info("제대 {0}, {1} 교환", teamA, teamB);

                List<long> teamMemberA = MainWindow.echelonView.FindTeam(teamA);
                List<long> teamMemberB = MainWindow.echelonView.FindTeam(teamB);

                foreach(long teamMember in teamMemberA)
                {
                    //log.Info("인형 {0}를 제대 {1}로 이동", teamMember, teamB);
                    if (dictionary.ContainsKey(teamMember))
                    {
                        dictionary[teamMember].team = teamB;
                    }
                    else if (Fairy.dictionary.ContainsKey(teamMember))
                    {
                        Fairy.dictionary[teamMember].team = teamB;
                    }
                }
                foreach(long teamMember in teamMemberB)
                {
                    //log.Info("인형 {0}를 제대 {1}로 이동", teamMember, teamA);
                    if (dictionary.ContainsKey(teamMember))
                    {
                        dictionary[teamMember].team = teamA;
                    }
                    else if (Fairy.dictionary.ContainsKey(teamMember))
                    {
                        Fairy.dictionary[teamMember].team = teamA;
                    }
                }

                MainWindow.echelonView.ExchangeTeam(teamA, teamB);
                MainWindow.echelonView.Check(teamA);
                MainWindow.echelonView.Update(teamA);
                MainWindow.echelonView.Check(teamB);
                MainWindow.echelonView.Update(teamB);
            }

            /// <summary>
            /// 인형 최대 레벨 가져오기
            /// </summary>
            /// <param name="id"></param>
            public static int GetMaxLevel(long id)
            {
                DollWithUserInfo doll = Get(id);
                int maxLevel = 100;
                if (doll != null)
                {
                    int mod = doll.mod;
                    switch (mod)
                    {
                        case 0:
                            maxLevel = 100;
                            break;
                        case 1:
                            maxLevel = 110;
                            break;
                        case 2:
                            maxLevel = 115;
                            break;
                        case 3:
                            maxLevel = 120;
                            break;
                    }
                }
                return maxLevel;
            }

            /// <summary>
            /// 인형 경험치 획득
            /// </summary>
            /// <param name="id"></param>
            /// <param name="exp"></param>
            public static void GainExp(long id, int exp)
            {
                if (id > 0 && exp > 0 && dictionary.ContainsKey(id))
                {
                    SetExp(id, dictionary[id].exp + exp);
                }
            }

            /// <summary>
            /// 제대 탄식 소비
            /// </summary>
            /// <param name="teamId"></param>
            public static void SpendAmmoMre(int teamId, bool onlyMre = false)
            {
                log.Debug("제대 {0} 탄식 소비", teamId);
                List<long> members = MainWindow.echelonView.FindTeam(teamId, true);
                foreach (long member in members)
                {
                    if (dictionary.ContainsKey(member))
                    {
                        if (onlyMre == false)
                            dictionary[member].ammo -= 1;
                        dictionary[member].mre -= 1;
                    }
                    else
                    {
                        //log.Warn("제대 {0} 인형 {1} 찾을 수 없음 - 탄식 소비 X", teamId, member);
                    }
                }
                MainWindow.echelonView.Update(teamId);
            }
            
            /// <summary>
            /// 제대 탄식 소실
            /// </summary>
            /// <param name="teamId"></param>
            public static void LoseAmmoMre(int teamId)
            {
                log.Debug("제대 {0} 탄식 소실", teamId);
                List<long> members = MainWindow.echelonView.FindTeam(teamId, true);
                foreach (long member in members)
                {
                    if (dictionary.ContainsKey(member))
                    {
                        dictionary[member].ammo = 0;
                        dictionary[member].mre = 0;
                    }
                    else
                    {
                        //log.Warn("제대 {0} 인형 {1} 찾을 수 없음 - 탄식 소실 X", teamId, member);
                    }
                }
                MainWindow.echelonView.Update(teamId);
            }

            /// <summary>
            /// 제대 탄식 보급
            /// </summary>
            /// <param name="teamId"></param>
            public static void SupplyAmmoMre(int teamId)
            {
                log.Debug("제대 {0} 탄식 보급", teamId);
                List<long> members = MainWindow.echelonView.FindTeam(teamId, true);
                foreach (long member in members)
                {
                    if (dictionary.ContainsKey(member))
                    {
                        dictionary[member].ammo = 5;
                        dictionary[member].mre = 10;
                    }
                    else
                    {
                        //log.Warn("제대 {0} 인형 {1} 찾을 수 없음 - 탄식 보급 X", teamId, member);
                    }
                }
                MainWindow.echelonView.Update(teamId);
            }

            /// <summary>
            /// 인형 개조 설정
            /// </summary>
            /// <param name="id"></param>
            public static void UpgradeMod(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    log.Debug("인형 {0} ({1}) 개조 {2} → {3}", UserData.Doll.GetName(id), id, dictionary[id].mod, dictionary[id].mod + 1);

                    dictionary[id].mod += 1;
                    if (dictionary[id].mod == 1)
                    {
                        if (dictionary[id].no < 20000)
                            dictionary[id].no += 20000;
                        dictionary[id].skill2 = 1;
                    }
                    dictionary[id].Refresh();

                    int teamId = dictionary[id].team;
                    MainWindow.echelonView.Update(teamId);
                }
            }

            /// <summary>
            /// 인형 경험치 설정
            /// </summary>
            /// <param name="id"></param>
            /// <param name="exp"></param>
            public static void SetExp(long id, int exp, bool forceUpdate=false)
            {
                if (dictionary.ContainsKey(id))
                {
                    long gainExp = exp - dictionary[id].exp;
                    log.Debug("인형 {0} ({1}) 경험치 {2} → {3} (+{4})", UserData.Doll.GetName(id), id, dictionary[id].exp, exp, gainExp);

                    dictionary[id].exp = exp;
                    dictionary[id].Refresh(DollWithUserInfo.REFRESH.ALL, forceUpdate);

                    // 편제확대 필요 알림
                    if (Config.Alarm.notifyDollNeedDummyLink && dictionary[id].notifiedNeedExpandLink == false && CheckNeedExpandLink(id))
                    {
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.All,
                            type = MessageType.doll_need_expand,
                            subject = LanguageResources.Instance["MESSAGE_DOLL_NEED_DUMMY_LINK_SUBJECT"],
                            content = string.Format(LanguageResources.Instance["MESSAGE_DOLL_NEED_DUMMY_LINK_CONTENT"], 
                                                    dictionary[id].team, dictionary[id].name)
                        });
                        dictionary[id].notifiedNeedExpandLink = true;
                    }
                    // 최대레벨 알림
                    if (Config.Alarm.notifyMaxLevel && dictionary[id].notifiedMaxLevel == false && CheckMaxLevel(id))
                    {
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.All,
                            type = MessageType.doll_max_level,
                            subject = LanguageResources.Instance["MESSAGE_DOLL_MAX_LEVEL_SUBJECT"],
                            content = string.Format(LanguageResources.Instance["MESSAGE_DOLL_MAX_LEVEL_CONTENT"], 
                                                    dictionary[id].team, dictionary[id].name),
                        });
                        dictionary[id].notifiedMaxLevel = true;
                    }
                    MainWindow.echelonView.Update(dictionary[id].team, dictionary[id].location);
                }
            }

            /// <summary>
            /// 인형 현재 레벨 가져오기
            /// </summary>
            /// <param name="lv"></param>
            /// <param name="exp"></param>
            /// <returns></returns>
            public static int GetCurrentLevel(int lv, long exp)
            {
                for (int i = lv; i <= 120; i++)
                {
                    if (exp < GameData.Doll.Exp.totalExp[i])
                    {
                        return i - 1;
                    }
                    else if (exp == GameData.Doll.Exp.totalExp[i])
                    {
                        return i;
                    }
                }
                return 120;
            }

            /// <summary>
            /// 최대 레벨까지 작전보고서 갯수
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static int GetRequireBattleReport(long id, int toLevel=100)
            {
                int totalCount = 0;
                if (dictionary.ContainsKey(id))
                {
                    DollWithUserInfo gun = new DollWithUserInfo(dictionary[id]);

                    int beforeLevel = 0;
                    int currentLevel = gun.level;
                    long currentExp = gun.exp;
                    long getExp = 0;
                    while (currentLevel != toLevel && currentLevel < toLevel)
                    {
                        if (beforeLevel != currentLevel)
                        {
                            if (currentLevel > 100 && gun.married == true)
                                getExp = 6000;
                            else 
                                getExp = 3000;
                            beforeLevel = currentLevel;
                        }
                        if (!GameData.Doll.Exp.totalExp.ContainsKey(currentLevel + 1))
                            break;
                        long remainExp = GameData.Doll.Exp.GetTotalExp(currentLevel + 1) - currentExp;

                        int count = Convert.ToInt32(remainExp / getExp + 1);
                        currentExp += count * getExp;

                        currentLevel = GetCurrentLevel(currentLevel, currentExp);

                        totalCount += count;
                    }
                }
                return totalCount;
            }

            /// <summary>
            /// 특정 레벨까지 거지런 횟수
            /// </summary>
            /// <param name="id"></param>
            /// <param name="baseExp"></param>
            /// <param name="levelPenalty"></param>
            /// <returns></returns>
            public static int GetRequireRun(long id, int baseExp, int levelPenalty=120, int battleCount=5, int toLevel=100)
            {
                int totalRunCount = 0;
                if (dictionary.ContainsKey(id))
                {
                    DollWithUserInfo doll = new DollWithUserInfo(dictionary[id]);
                    if (toLevel > doll.maxLevel)
                    {
                        toLevel = doll.maxLevel;
                    }

                    int beforeLevel = 0;
                    int currentLevel = doll.level;
                    if (currentLevel >= 90 && doll.maxLink <= 5)
                    {
                        doll.maxLink = 5;
                    }
                    else if (currentLevel >= 70 && doll.maxLink <= 4)
                    {
                        doll.maxLink = 4;
                    }
                    else if (currentLevel >= 30 && doll.maxLink <= 3)
                    {
                        doll.maxLink = 3;
                    }
                    else if (currentLevel >= 10 && doll.maxLink <= 2)
                    {
                        doll.maxLink = 2;
                    }
                    long currentExp = doll.exp;
                    long getExp = 0;
                    while (currentLevel != toLevel && currentLevel < toLevel)
                    {
                        if (beforeLevel != currentLevel)
                        {
                            getExp = GetRunExp(doll, baseExp, levelPenalty);
                            beforeLevel = currentLevel;
                        }
                        if (!GameData.Doll.Exp.totalExp.ContainsKey(currentLevel + 1))
                            break;
                        long remainExp = GameData.Doll.Exp.GetTotalExp(currentLevel + 1) - currentExp;

                        int runCount = Convert.ToInt32(remainExp / getExp);
                        if (runCount == 0)
                        {
                            runCount = 1;
                        }
                        currentExp += runCount * getExp;

                        currentLevel = GetCurrentLevel(currentLevel, currentExp);
                        if (currentLevel >= 90 && doll.maxLink <= 5)
                        {
                            doll.maxLink = 5;
                        }
                        else if (currentLevel >= 70 && doll.maxLink <= 4)
                        {
                            doll.maxLink = 4;
                        }
                        else if (currentLevel >= 30 && doll.maxLink <= 3)
                        {
                            doll.maxLink = 3;
                        }
                        else if (currentLevel >= 10 && doll.maxLink <= 2)
                        {
                            doll.maxLink = 2;
                        }

                        totalRunCount += runCount;
                    }
                    totalRunCount = Convert.ToInt32(Math.Ceiling((double)totalRunCount / (double)battleCount));
                }
                return totalRunCount;
            }

            /// <summary>
            /// 거지런 1전투 획득경험치
            /// </summary>
            /// <param name="gun"></param>
            /// <param name="baseExp"></param>
            /// <param name="levelPenalty"></param>
            /// <returns></returns>
            public static int GetRunExp(DollWithUserInfo gun, int baseExp, int levelPenalty = 120)
            {
                double expPenalty = 1.0;
                if (gun.level >= levelPenalty + 40)
                {
                    return 10;
                }
                else if (gun.level >= levelPenalty + 30)
                {
                    expPenalty = 0.2;
                }
                else if (gun.level >= levelPenalty + 20)
                {
                    expPenalty = 0.4;
                }
                else if (gun.level >= levelPenalty + 10)
                {
                    expPenalty = 0.6;
                }
                else if (gun.level >= levelPenalty)
                {
                    expPenalty = 0.8;
                }

                // 리더 보너스
                double leaderBonus = 1.0;
                if (gun.location == 1)
                {
                    leaderBonus = 1.2;
                }

                // 개조 시 서약 보너스
                double marriedBonus = 1.0;
                if (gun.married == true && gun.mod > 0)
                {
                    marriedBonus = 2.0;
                }

                // 링크 보너스
                double linkBonus = 1.0;
                switch (gun.maxLink)
                {
                    case 2:
                        linkBonus = 1.5;
                        break;
                    case 3:
                        linkBonus = 2.0;
                        break;
                    case 4:
                        linkBonus = 2.5;
                        break;
                    case 5:
                        linkBonus = 3.0;
                        break;
                    default:
                        break;
                }

                // 지휘관 보너스
                double commanderBonus = 1.0;
                if (Config.Costume.coBonusDollExp)
                {
                    commanderBonus = Convert.ToDouble(Config.Costume.coBonusDollExpPercent) / 100 + 1.0;
                }

                // 경험치 업 보너스
                double expBonusEvent = 1.0;
                if (Config.Echelon.expUpEvent)
                {
                    expBonusEvent = 1.5;
                }
                //if (Config.Extra.expBonusEvent)
                //{                      
                //    expBonusEvent = Convert.ToDouble(Config.Extra.expBonusEventPercent) / 100 + 1.0;
                //}

                return Convert.ToInt32(Math.Truncate(baseExp * linkBonus * leaderBonus * expPenalty * marriedBonus * commanderBonus * expBonusEvent));
            }

            /// <summary>
            /// 인형 체력 설정
            /// </summary>
            /// <param name="id"></param>
            /// <param name="hp"></param>
            public static void SetHp(long id, int hp, bool fix = false)
            {
                // 모의작전이 아닌 경우에만 체력 반영
                if (dictionary.ContainsKey(id) && GameData.Mission.GetUsedAp(UserData.CurrentMission.id) == 0)
                {
                    if (dictionary[id].hp != hp)
                    {
                        log.Debug("인형 {0} ({1}) 체력 {2} → {3}", UserData.Doll.GetName(id), id, dictionary[id].hp, hp);
                        dictionary[id].hp = hp;
                        dictionary[id].Refresh(DollWithUserInfo.REFRESH.HP);

                        int maxHp = dictionary[id].maxHp;

                        if (Config.Alarm.notifyDollWounded &&
                            dictionary[id].notifiedWounded == false &&
                            maxHp > 0)
                        {
                            double percent = (double)hp / maxHp;
                            if (percent * 100 < Config.Alarm.notifyDollWoundedPercent)
                            {
                                int gunId = dictionary[id].no;
                                int skinId = dictionary[id].skin;
                                Notifier.Manager.notifyQueue.Enqueue(new Message()
                                {
                                    send = MessageSend.All,
                                    type = MessageType.doll_wounded,
                                    gunId = gunId,
                                    skinId = skinId,
                                    subject = LanguageResources.Instance["MESSAGE_DOLL_HP_WARNING_SUBJECT"],
                                    content = string.Format(LanguageResources.Instance["MESSAGE_DOLL_HP_WARNING_CONTENT"], 
                                                            dictionary[id].team, dictionary[id].name, Config.Alarm.notifyDollWoundedPercent),
                                });
                                dictionary[id].notifiedWounded = true;
                            }
                        }
                        if (fix)
                        {
                            dictionary[id].notifiedWounded = false;
                        }
                        MainWindow.echelonView.Update(dictionary[id].team, dictionary[id].location);
                    }
                }
            }

            /// <summary>
            /// 인형 수복
            /// </summary>
            /// <param name="id"></param>
            public static void Fix(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    log.Debug("인형 {0} ({1}) 수복", UserData.Doll.GetName(id), id);
                    dictionary[id].hp = dictionary[id].maxHp;
                    dictionary[id].Refresh(DollWithUserInfo.REFRESH.HP);

                    dictionary[id].notifiedWounded = false;

                    MainWindow.echelonView.Update(dictionary[id].team, dictionary[id].location);
                }
                else
                {
                    log.Error("수복할 인형이 존재하지 않음 {0}", id);
                }
            }

            /// <summary>
            /// 인형 편제확대
            /// </summary>
            /// <param name="id"></param>
            /// <param name="foodIds"></param>
            public static void ExpandLink(long id, long[] foodIds)
            {
                if (dictionary.ContainsKey(id))
                {
                    log.Debug("인형 {0} ({1}) 편제확대 {2} → {3} 링크", UserData.Doll.GetName(id), id, dictionary[id].maxLink, dictionary[id].maxLink+1);
                    dictionary[id].maxLink += 1;
                    dictionary[id].Refresh(DollWithUserInfo.REFRESH.STAT);
                    dictionary[id].hp = dictionary[id].maxHp;
                    dictionary[id].Refresh(DollWithUserInfo.REFRESH.HP);
                    foreach (long foodId in foodIds)
                    {
                        Remove(foodId);
                    }
                    dictionary[id].notifiedNeedExpandLink = false;
                    // 탄식 충전
                    dictionary[id].ammo = 5;
                    dictionary[id].mre = 10;
                    MainWindow.echelonView.Update(dictionary[id].team, dictionary[id].location);
                }
            }

            /// <summary>
            /// 편제확대 레벨 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static int GetExpandLevel(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    int level = dictionary[id].level;
                    int mod = dictionary[id].mod;
                    if (level < 10)
                        return 10;
                    else if (level < 30)
                        return 30;
                    else if (level < 70)
                        return 70;
                    else if (level < 90)
                        return 90;
                    else if (level < 100)
                        return 100;
                    else if (level < 110 && mod == 1)
                        return 110;
                    else if (level < 115 && mod == 2)
                        return 115;
                    else if (level < 120 && mod == 3)
                        return 120;
                }
                return 0;
            }

            /// <summary>
            /// 인형 편제확대 필요 체크
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static bool CheckNeedExpandLink(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    int no = dictionary[id].no;
                    int level = dictionary[id].level;
                    int link = dictionary[id].maxLink;
                    // 1링 Zas 예외처리
                    if (no == 196 && link == 1)
                    {
                        return false;
                    }
                    // 2링크 필요
                    if (link <= 1 && level >= 10)
                    {
                        log.Debug("인형 {0} 편제확대 필요 ({1} 링크)", id, link);
                        return true;
                    }
                    else if (link <= 2 && level >= 30)
                    {
                        log.Debug("인형 {0} 편제확대 필요 ({1} 링크)", id, link);
                        return true;
                    }
                    else if (link <= 3 && level >= 70)
                    {
                        log.Debug("인형 {0} 편제확대 필요 ({1} 링크)", id, link);
                        return true;
                    }
                    else if (link <= 4 && level >= 90)
                    {
                        log.Debug("인형 {0} 편제확대 필요 ({1} 링크)", id, link);
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 인형 최대레벨 체크
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static bool CheckMaxLevel(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    int level = dictionary[id].level;
                    int mod = dictionary[id].mod;
                    if (level == 100 && mod == 0)
                    {
                        log.Debug("인형 {0} 최대 레벨 도달 {1}", id, level);
                        return true;
                    }
                    else if (level == 110 && mod == 1)
                    {
                        log.Debug("인형 {0} 최대 레벨 도달 {1}", id, level);
                        return true;
                    }
                    else if (level == 115 && mod == 2)
                    {
                        log.Debug("인형 {0} 최대 레벨 도달 {1}", id, level);
                        return true;
                    }
                    else if (level == 120 && mod == 3)
                    {
                        log.Debug("인형 {0} 최대 레벨 도달 {1}", id, level);
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 인형 이름 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static string GetName(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    int gunId = dictionary[id].no;
                    return LanguageResources.Instance[string.Format("DOLL_{0}", gunId)];
                }
                return "Unknown";
            }

            /// <summary>
            /// 인형 스킬레벨 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <param name="skillType"></param>
            /// <returns></returns>
            public static int GetSkillLevel(long id, int skillType)
            {
                if (dictionary.ContainsKey(id))
                {
                    if (skillType == 1)
                    {
                        return dictionary[id].skill1;
                    }
                    else if (skillType == 2)
                    {
                        return dictionary[id].skill2;
                    }
                }
                return -1;
            }

            /// <summary>
            /// 인형 스킨 설정
            /// </summary>
            /// <param name="id"></param>
            /// <param name="skinId"></param>
            public static void ChangeSkin(long id, int skinId)
            {
                if (dictionary.ContainsKey(id))
                {
                    dictionary[id].skin = skinId;
                }
            }

            /// <summary>
            /// 인형 스킨 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static int GetSkin(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    return dictionary[id].skin;
                }
                return 0;
            }

            public static List<long> exploreTeam = new List<long>();
            public static List<long> explorePets = new List<long>();
            public static List<int> exploreAdaptiveDolls = new List<int>();
            public static List<int> exploreAdaptivePets = new List<int>();

            /// <summary>
            /// 탐색 투입 우세인형 인원 가져오기
            /// </summary>
            public static int GetExploreAdaptiveDollCount()
            {
                int count = 0;
                foreach (long id in exploreTeam)
                {
                    if (dictionary.ContainsKey(id))
                    {
                        int no = dictionary[id].no;
                        if (no > 20000)
                        {
                            no = no % 20000;
                        }
                        if (exploreAdaptiveDolls.Contains(no))
                        {
                            count++;
                        }
                    }
                }
                return count;
            }

            /// <summary>
            /// 탐색 투입 인형 총 레벨 가져오기
            /// </summary>
            /// <returns></returns>
            public static int GetExploreDollTotalLevel()
            {
                int count = 0;
                foreach (long id in exploreTeam)
                {
                    if (dictionary.ContainsKey(id))
                    {
                        count += dictionary[id].level;
                    }
                }
                return count;
            }

            /// <summary>
            /// 인형 해체 회수 자원 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static int[] GetRetireDollCollect(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    string type = dictionary[id].type;
                    switch (type)
                    {
                        case "HG":
                            return new int[] { 2, 0, 2, 0 };
                        case "SMG":
                            return new int[] { 5, 5, 2, 0 };
                        case "AR":
                            return new int[] { 2, 5, 5, 0 };
                        case "RF":
                            return new int[] { 5, 2, 5, 0 };
                        case "MG":
                            return new int[] { 5, 5, 2, 5 };
                        case "SG":
                            return new int[] { 5, 2, 5, 5 };
                    }
                }
                return new int[] { 0, 0, 0, 0 };
            }
        }

        /// <summary>
        /// 장비
        /// </summary>
        public static class Equip
        {
            private static Dictionary<long, EquipWithUserInfo> dictionary = new Dictionary<long, EquipWithUserInfo>();

            /// <summary>
            /// 장비상한 알림 여부
            /// </summary>
            public static bool notified = false;

            /// <summary>
            /// 생성
            /// </summary>
            /// <param name="packet"></param>
            public static void Init(dynamic packet)
            {
                Dictionary<long, EquipWithUserInfo> tempDictionary = new Dictionary<long, EquipWithUserInfo>();
                try
                {
                    if (packet != null && packet is JObject)
                    {
                        JObject obj = packet;
                        List<string> ids = obj.Properties().Select(p => p.Name).ToList();
                        foreach (string id in ids)
                        {
                            try
                            {
                                long equipWithUserId = Parser.Json.ParseLong(obj[id]["id"]);
                                tempDictionary.Add(equipWithUserId, new EquipWithUserInfo(obj[id]));
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                        dictionary = tempDictionary;
                        UpdateEquipCount();
                    }
                    else
                    {
                        log.Debug("소유장비 목록 패킷 잘못된 형태");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "소유장비 목록 초기화 실패");
                }
            }

            /// <summary>
            /// 전체 장비 정보 가져오기
            /// </summary>
            /// <returns></returns>
            public static Dictionary<long, EquipWithUserInfo> GetAll()
            {
                return dictionary;
            }

            /// <summary>
            /// 장비 추가
            /// </summary>
            /// <param name="id"></param>
            /// <param name="equip"></param>
            public static void Add(long id, EquipWithUserInfo equip, bool inMission = false)
            {
                dictionary.Add(id, equip);
                UpdateEquipCount();

                // 전역 중 획득
                if (inMission)
                {
                    JObject equipData = GameData.Equip.GetData(equip.equipId);
                    string name = "Unknown";
                    int star = 0;
                    string category = "";
                    if (equipData != null)
                    {
                        name = LanguageResources.Instance[string.Format("EQUIP_{0}", equip.equipId)];
                        star = Parser.Json.ParseInt(equipData["star"]);
                        category = Parser.Json.ParseString(equipData["category"]);
                        log.Debug("장비 {0} ({1}성 {2}) 획득", name, star, category);
                    }

                    // 획득장비 알림
                    if (Config.Alarm.notifyGetEquip && 
                        Config.Alarm.notifyGetEquipStar <= star && 
                        2 <= star)
                    {
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.All,
                            type = MessageType.mission_get_equip,
                            subject = LanguageResources.Instance["MESSAGE_MISSION_GET_EQUIP_SUBJECT"],
                            content = string.Format(LanguageResources.Instance["MESSAGE_MISSION_GET_EQUIP_CONTENT"], 
                                                    name, star, category)
                        });
                    }
                }

                // 장비상한 알림
                if (Config.Alarm.notifyMaxEquip && UserData.Equip.notified == false)
                {
                    if (UserData.maxEquipCount <= dictionary.Count())
                    {
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.All,
                            type = MessageType.reach_max_equip,
                            subject = LanguageResources.Instance["MESSAGE_MAX_EQUIP_SUBJECT"],
                            content = LanguageResources.Instance["MESSAGE_MAX_EQUIP_CONTENT"],
                        });
                        UserData.Equip.notified = true;
                    }
                }
            }

            /// <summary>
            /// 장비 제거
            /// </summary>
            /// <param name="equip"></param>
            public static void Remove(long id)
            {
                dictionary.Remove(id);
                UpdateEquipCount();
            }

            /// <summary>
            /// 장비 갯수
            /// </summary>
            /// <returns></returns>
            public static int Count()
            {
                return dictionary.Count();
            }

            /// <summary>
            /// 장비 초기화
            /// </summary>
            public static void Clear()
            {
                dictionary.Clear();
            }

            /// <summary>
            /// 장비 도감번호 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static int GetEquipId(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    int equipId = dictionary[id].equipId;
                    return equipId;
                }
                return 0;
            }

            /// <summary>
            /// 장비 갯수 업데이트
            /// </summary>
            private static void UpdateEquipCount()
            {
                UserData.equipCount = dictionary.Count();
            }
        }

        /// <summary>
        /// 요정
        /// </summary>
        public static class Fairy
        {
            /// <summary>
            /// 요정 (id, data) 
            /// </summary>
            public static Dictionary<long, FairyWithUserInfo> dictionary = new Dictionary<long, FairyWithUserInfo>();

            /// <summary>
            /// 초기화
            /// </summary>
            /// <param name="packet"></param>
            public static void Init(dynamic packet)
            {
                Dictionary<long, FairyWithUserInfo> tempDictionary = new Dictionary<long, FairyWithUserInfo>();

                // 요정
                try
                {
                    if (packet != null && packet is JObject)
                    {
                        JObject obj = packet;
                        List<string> ids = obj.Properties().Select(p => p.Name).ToList();
                        foreach (string id in ids)
                        {
                            try
                            {
                                long fairyWithUserId = Parser.Json.ParseLong(obj[id]["id"]);
                                tempDictionary.Add(fairyWithUserId, new FairyWithUserInfo(obj[id]));
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                        dictionary = tempDictionary;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                // 제대
                try
                {
                    foreach (KeyValuePair<long, FairyWithUserInfo> item in dictionary)
                    {
                        MainWindow.echelonView.Set(new EchelonTemplate(item.Value));
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            public static FairyWithUserInfo Get(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    return dictionary[id];
                }
                return null;
            }

            public static Dictionary<long, FairyWithUserInfo> GetAll()
            {
                return dictionary;
            }

            public static void Set(FairyWithUserInfo fairy)
            {
                if (dictionary.ContainsKey(fairy.id))
                {
                    dictionary[fairy.id] = fairy;
                }
                else
                {
                    dictionary.Add(fairy.id, fairy);
                }
            }

            public static void Set(long id, FairyWithUserInfo fairy)
            {
                if (dictionary.ContainsKey(id))
                {
                    dictionary[id] = fairy;
                }
                else
                {
                    dictionary.Add(id, fairy);
                }
            }

            public static void Add(long id, FairyWithUserInfo fairy)
            {
                dictionary.Add(id, fairy);
            }

            public static void Remove(FairyWithUserInfo fairy)
            {
                dictionary.Remove(fairy.id);
            }

            public static void Remove(long id)
            {
                dictionary.Remove(id);
            }

            public static int Count()
            {
                return dictionary.Count();
            }

            public static void Clear()
            {
                dictionary.Clear();
            }

            /// <summary>
            /// 제대편성 변경
            /// </summary>
            /// <param name="teamId"></param>
            /// <param name="id"></param>
            public static void SwapTeam(int teamId, long id)
            {
                /// 2) 소속변경 요정이 소속되어 있었던 제대에서 제거
                int beforeTeamId = 0, beforeLocation = 0;
                MainWindow.echelonView.FindId(ref beforeTeamId, ref beforeLocation, id);
                MainWindow.echelonView.Remove(beforeTeamId, 6);

                /// 1) 기존 제대에 소속된 요정 제거
                long beforeFairyWithUserId = 0;
                MainWindow.echelonView.FindId(beforeTeamId, 6, ref beforeFairyWithUserId);
                if (beforeFairyWithUserId > 0 && dictionary.ContainsKey(beforeFairyWithUserId))
                {
                    dictionary[beforeFairyWithUserId].team = 0;
                    MainWindow.echelonView.Remove(teamId, 6);
                }

                MainWindow.echelonView.Remove(teamId, 6);
                if (id > 0 && dictionary.ContainsKey(id))
                {
                    dictionary[id].team = teamId;
                    MainWindow.echelonView.Set(new EchelonTemplate(dictionary[id]));
                }
                MainWindow.echelonView.Check(beforeTeamId);
                MainWindow.echelonView.Update(beforeTeamId);
                MainWindow.echelonView.Sort(beforeTeamId);
                MainWindow.echelonView.Check(teamId);
                MainWindow.echelonView.Update(teamId);
                MainWindow.echelonView.Sort(teamId);
            }

            /// <summary>
            /// 제대 요정 가져오기
            /// </summary>
            /// <param name="teamId"></param>
            public static FairyWithUserInfo GetFairyWithTeamId(int teamId)
            {
                try
                {
                    foreach(KeyValuePair<long, FairyWithUserInfo> item in dictionary)
                    {
                        if (item.Value.team == teamId)
                        {
                            return item.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return null;
            }

            // 요정 이름 가져오기
            public static string GetName(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    int fairyId = dictionary[id].no;
                    return LanguageResources.Instance[string.Format("FAIRY_{0}", fairyId)];
                }
                //return "미확인 요정";
                return "Unknown Fairy";
            }

            // 요정 스킬레벨 가져오기
            public static int GetSkillLv(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    return dictionary[id].skill;
                }
                return -1;
            }

            /// <summary>
            /// 요정 경험치 획득
            /// </summary>
            /// <param name="id"></param>
            /// <param name="exp"></param>
            public static void GainExp(long id, int exp)
            {
                if (id > 0 && exp > 0)
                {
                    SetExp(id, dictionary[id].exp + exp);
                }
            }

            /// <summary>
            /// 요정 경험치 설정
            /// </summary>
            /// <param name="id"></param>
            /// <param name="exp"></param>
            public static void SetExp(long id, int exp)
            {
                if (dictionary.ContainsKey(id))
                {
                    int gainExp = exp - dictionary[id].exp;
                    log.Debug("요정 {0} ({1}) 경험치 {2} → {3} (+{4})", UserData.Fairy.GetName(id), id, dictionary[id].exp, exp, gainExp);

                    dictionary[id].exp = exp;
                    dictionary[id].Refresh(FairyWithUserInfo.REFRESH.EXP);
                    dictionary[id].Refresh(FairyWithUserInfo.REFRESH.CHANGE_TEAM_EXP);

                    MainWindow.echelonView.Update(dictionary[id].team, 6);
                }
            }

            /// <summary>
            /// 요정 레벨 설정
            /// </summary>
            /// <param name="id"></param>
            /// <param name="level"></param>
            public static void SetLevel(long id, int level)
            {
                if (dictionary.ContainsKey(id))
                {
                    dictionary[id].level = level;

                    // 능력치 업데이트
                    MainWindow.echelonView.Update(dictionary[id].team, 6);
                }
            }

            public static int GetTeamRunEarnExp(int teamId)
            {
                int runEarnExp = 0;

                List<long> members = MainWindow.echelonView.FindTeam(teamId, true);
                foreach (long member in members)
                {
                    if (UserData.Doll.dictionary.ContainsKey(member))
                    {
                        runEarnExp += UserData.Doll.dictionary[member].runEarnExp;
                    }
                }
                runEarnExp = (int)((double)runEarnExp * GameData.Fairy.Exp.expRatio);
                return runEarnExp;
            }

            /// <summary>
            /// 제대 거지런 1전투 획득경험치
            /// </summary>
            /// <param name="teamId"></param>
            public static int[] GetTeamEarnExp(int teamId)
            {
                int[] earnExp = new int[] { 0, 0, 0, 0, 0, 0 };

                List<long> members = MainWindow.echelonView.FindTeam(teamId, true);
                foreach(long member in members)
                {
                    if (UserData.Doll.dictionary.ContainsKey(member))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            //earnExp[i] += UserData.Doll.dictionary[member].earnExp[i];
                        }
                    }
                }

                for (int i = 0; i < 6; i++)
                {
                    if (earnExp[i] > 0)
                    {
                        earnExp[i] = Convert.ToInt32((double)earnExp[i] * GameData.Fairy.Exp.expRatio);
                    }
                }

                return earnExp;
            }

            /// <summary>
            /// 요정 최대 레벨까지 필요한 작전보고서 갯수
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static int GetBattleReportCountToMaxLevel(long id, int toLevel=100)
            {
                if (toLevel > 100)
                {
                    toLevel = 100;
                }

                int totalCount = 0;
                if (dictionary.ContainsKey(id))
                {
                    long currentExp = dictionary[id].exp;
                    long sumExp = GameData.Fairy.Exp.GetTotalExp(toLevel);

                    totalCount = Convert.ToInt32((sumExp - currentExp) / 3000 + 1);
                }
                return totalCount;
            }

            /// <summary>
            /// 요정 최대 레벨까지 필요한 거지런 횟수
            /// </summary>
            /// <param name="id"></param>
            /// <param name="exp"></param>
            /// <returns></returns>
            public static int GetRunCountToMaxLevel(long id, long exp)
            {
                int totalCount = 0;
                if (dictionary.ContainsKey(id))
                {
                    long currentExp = dictionary[id].exp;
                    long sumExp = GameData.Fairy.Exp.GetTotalExp(100);

                    totalCount = Convert.ToInt32(((double)sumExp - (double)currentExp) / (double)exp + 1);
                    //log.Info("요정 {0} 남은 경험치 {1} 거지런 1회 경험치 {2}", id, sumExp - currentExp, exp);
                }
                return totalCount;
            }
        }

        /// <summary>
        /// 아이템
        /// </summary>
        public static class Item
        {
            private static Dictionary<long, ItemWithUserInfo> dictionary = new Dictionary<long, ItemWithUserInfo>();

            /// <summary>
            /// 초기화
            /// </summary>
            /// <param name="packet"></param>
            public static void Init(dynamic packet)
            {
                Dictionary<long, ItemWithUserInfo> tempDictionary = new Dictionary<long, ItemWithUserInfo>();
                try
                {
                    if (packet != null && packet is JArray)
                    {
                        var array = packet;
                        foreach (var item in array)
                        {
                            long itemId = Parser.Json.ParseLong(item, "item_id");
                            switch (itemId)
                            {
                                case 507: // 자유경험치
                                    tempDictionary.Add(itemId, new ItemWithUserInfo(item));
                                    break;
                                default:
                                    continue;
                            }
                        }
                        dictionary = tempDictionary;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            public static ItemWithUserInfo Get(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    return dictionary[id];
                }
                return null;
            }

            public static Dictionary<long, ItemWithUserInfo> GetAll()
            {
                return dictionary;
            }

            public static void Set(ItemWithUserInfo item)
            {
                if (dictionary.ContainsKey(item.id))
                {
                    dictionary[item.id] = item;
                }
                else
                {
                    dictionary.Add(item.id, item);
                }
            }

            public static void Set(long id, ItemWithUserInfo item)
            {
                if (dictionary.ContainsKey(id))
                {
                    dictionary[id] = item;
                }
                else
                {
                    dictionary.Add(id, item);
                }
            }

            public static void Add(long id, ItemWithUserInfo item)
            {
                dictionary.Add(id, item);
            }

            public static void Remove(ItemWithUserInfo item)
            {
                dictionary.Remove(item.id);
            }

            public static void Remove(long id)
            {
                dictionary.Remove(id);
            }

            public static bool ContainsKey(long id)
            {
                if (dictionary.ContainsKey(id))
                    return true;
                return false;
            }

            public static int Count()
            {
                return dictionary.Count();
            }

            public static void Clear()
            {
                dictionary.Clear();
            }
        }

        /// <summary>
        /// 칩셋
        /// </summary>
        public static class Chip
        {
            private static Dictionary<long, ChipWithUserInfo> dictionary = new Dictionary<long, ChipWithUserInfo>();

            /// <summary>
            /// 초기화
            /// </summary>
            /// <param name="packet"></param>
            public static void Init(dynamic packet)
            {
                Dictionary<long, ChipWithUserInfo> tempDictionary = new Dictionary<long, ChipWithUserInfo>();
                try
                {
                    if (packet != null && packet is JObject)
                    {
                        JObject obj = JObject.Parse(packet);
                        List<string> ids = obj.Properties().Select(p => p.Name).ToList();
                        foreach (string id in ids)
                        {
                            try
                            {
                                long chipWithUserId = Parser.Json.ParseLong(obj[id]["id"]);
                                tempDictionary.Add(chipWithUserId, new ChipWithUserInfo(obj[id]));
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                        dictionary = tempDictionary;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            public static Dictionary<long, ChipWithUserInfo> GetAll()
            {
                return dictionary;
            }

            public static void Add(long id, ChipWithUserInfo chip)
            {
                dictionary.Add(id, chip);
            }

            public static void Remove(ChipWithUserInfo chip)
            {
                dictionary.Remove(chip.id);
            }

            public static void Remove(long id)
            {
                dictionary.Remove(id);
            }

            public static int Count()
            {
                return dictionary.Count();
            }

            public static void Clear()
            {
                dictionary.Clear();
            }
        }

        /// <summary>
        /// 중장비부대
        /// </summary>
        public static class Squad
        {
            private static Dictionary<long, SquadWithUserInfo> dictionary = new Dictionary<long, SquadWithUserInfo>();

            /// <summary>
            /// 초기화
            /// </summary>
            /// <param name="packet"></param>
            public static void Init(dynamic packet)
            {
                Dictionary<long, SquadWithUserInfo> tempDictionary = new Dictionary<long, SquadWithUserInfo>();
                try
                {
                    if (packet != null && packet is JObject)
                    {
                        JObject obj = packet;
                        List<string> ids = obj.Properties().Select(p => p.Name).ToList();
                        foreach (string id in ids)
                        {
                            try
                            {
                                long squadWithUserId = Parser.Json.ParseLong(obj[id]["id"]);
                                SquadWithUserInfo squad = new SquadWithUserInfo(obj[id]);
                                tempDictionary.Add(squadWithUserId, squad);
                                //log.Info("squad {0}", squad.ToString());
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex, "중장비부대 초기화 중 에러");
                            }
                        }
                        dictionary = tempDictionary;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            public static void SetExp(int squadId, long exp)
            {

            }

            public static SquadWithUserInfo Get(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    return dictionary[id];
                }
                return null;
            }

            public static Dictionary<long, SquadWithUserInfo> GetAll()
            {
                return dictionary;
            }

            public static void Set(SquadWithUserInfo squad)
            {
                if (dictionary.ContainsKey(squad.id))
                {
                    dictionary[squad.id] = squad;
                }
                else
                {
                    dictionary.Add(squad.id, squad);
                }
            }

            public static void Add(long id, SquadWithUserInfo squad)
            {
                dictionary.Add(id, squad);
            }

            public static void Remove(SquadWithUserInfo squad)
            {
                dictionary.Remove(squad.id);
            }

            public static void Remove(long id)
            {
                dictionary.Remove(id);
            }

            public static int Count()
            {
                return dictionary.Count();
            }

            public static void Clear()
            {
                dictionary.Clear();
            }

            /// <summary>
            /// 중장비부대 이름 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static string GetName(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    int squadId = dictionary[id].squadId;
                    return GameData.Squad.GetData(squadId, "name");
                }
                return "Unknown";
            }

            /// <summary>
            /// 중장비부대 스킬 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <param name="skillType"></param>
            /// <returns></returns>
            internal static int GetSkillLevel(long id, int skillType)
            {
                if (dictionary.ContainsKey(id))
                {
                    switch (skillType)
                    {
                        case 1:
                            return dictionary[id].skill1;
                        case 2:
                            return dictionary[id].skill2;
                        case 3:
                            return dictionary[id].skill3;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// 전역 진행도
        /// </summary>
        public static class Mission
        {
            private static Dictionary<long, MissionWithUserInfo> dictionary = new Dictionary<long, MissionWithUserInfo>();

            /// <summary>
            /// 초기화
            /// </summary>
            /// <param name="packet"></param>
            public static void Init(dynamic packet)
            {
                Dictionary<long, MissionWithUserInfo> tempDictionary = new Dictionary<long, MissionWithUserInfo>();
                try
                {
                    if (packet != null && packet is JArray)
                    {
                        JArray items = packet;
                        foreach (var item in items)
                        {
                            long id = Parser.Json.ParseLong(item["mission_id"]);

                            tempDictionary.Add(id, new MissionWithUserInfo(item));
                        }
                        dictionary = tempDictionary;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            public static MissionWithUserInfo Get(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    return dictionary[id];
                }
                return null;
            }

            public static void Set(MissionWithUserInfo mission)
            {
                if (dictionary.ContainsKey(mission.id))
                {
                    dictionary[mission.id] = mission;
                }
                else
                {
                    dictionary.Add(mission.id, mission);
                }
            }

            public static void Set(long id, MissionWithUserInfo mission)
            {
                if (dictionary.ContainsKey(id))
                {
                    dictionary[id] = mission;
                }
                else
                {
                    dictionary.Add(id, mission);
                }
            }

            public static void Add(long id, MissionWithUserInfo mission)
            {
                dictionary.Add(id, mission);
            }

            public static void Remove(MissionWithUserInfo mission)
            {
                dictionary.Remove(mission.id);
            }

            public static void Remove(long id)
            {
                dictionary.Remove(id);
            }

            public static bool ContainsKey(long id)
            {
                if (dictionary.ContainsKey(id))
                    return true;
                return false;
            }
            
            public static int Count()
            {
                return dictionary.Count();
            }
            
            public static void Clear()
            {
                dictionary.Clear();
            }
        }

        /// <summary>
        /// 우편
        /// </summary>
        public static class Mail
        {
            private static Dictionary<long, MailWithUserInfo> dictionary = new Dictionary<long, MailWithUserInfo>();

            /// <summary>
            /// 초기화
            /// </summary>
            /// <param name="packet"></param>
            public static void Init(dynamic items)
            {
                Dictionary<long, MailWithUserInfo> tempDictionary = new Dictionary<long, MailWithUserInfo>();
                try
                {
                    if (items != null && items is JArray)
                    {
                        foreach(var item in items)
                        {
                            long id = Parser.Json.ParseLong(item["id"]);
                            tempDictionary.Add(id, new MailWithUserInfo(item));
                        }

                        var merged = dictionary.Concat(tempDictionary)
                            .ToLookup(x => x.Key, x => x.Value)
                            .ToDictionary(x => x.Key, g => g.First());

                        dictionary = merged;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            /// <summary>
            /// 우편 가져오기
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static MailWithUserInfo Get(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    return dictionary[id];
                }
                return null;
            }

            /// <summary>
            /// 우편 추가
            /// </summary>
            /// <param name="id"></param>
            /// <param name="mail"></param>
            public static void Add(long id, MailWithUserInfo mail)
            {
                dictionary.Add(id, mail);
            }

            /// <summary>
            /// 우편 제거
            /// </summary>
            /// <param name="id"></param>
            public static void Remove(long id)
            {
                if (dictionary.ContainsKey(id))
                {
                    dictionary.Remove(id);
                }
            }
            public static void Remove(MailWithUserInfo mail)
            {
                if (mail != null)
                {
                    Remove(mail.id);
                }
            }

            /// <summary>
            /// 우편 갯수
            /// </summary>
            /// <returns></returns>
            public static int Count()
            {
                if (dictionary != null)
                {
                    return dictionary.Count();
                }
                return 0;
            }

            /// <summary>
            /// 우편 비우기
            /// </summary>
            public static void Clear()
            {
                dictionary.Clear();
            }
        }

        public static void ClearUserData()
        {
            uid = 0;
            name = "";
            level = -1;
            maxDollCount = 0;
            maxEquipCount = 0;
            gem = -1;
            regTime = 0;

            adjutantDoll = 0;
            adjutantDollSkin = 0;

            adjutantFairy = 0;

            borrowTeamToday = -1;

            _attendanceTime = -1;

            CombatSimulation.Clear();
            GlobalExp.Clear();
            BattleReport.Clear();
            DataAnalysis.Clear();
            CurrentMission.Clear();

            Facility.Clear();
            Doll.Clear();
            Equip.Clear();
            Fairy.Clear();
            Item.Clear();
            Squad.Clear();
            Mission.Clear();
            Mail.Clear();
            Quest.Clear();
            Quest.ClearResearch();

            MainWindow.dashboardView.isSharedGem = false;
            MainWindow.dashboardView.isSharedBattery = false;
        }
    }
}
