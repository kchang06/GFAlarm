using GFAlarm.Constants;
using GFAlarm.Data;
using GFAlarm.Data.Element;
using GFAlarm.Notifier;
using GFAlarm.Transaction;
using GFAlarm.Util;
using GFAlarm.View;
using GFAlarm.View.DataTemplate;
using GFAlarm.View.Guide;
using GFAlarm.View.Menu;
using GFAlarm.View.Option;
using LocalizationResources;
using MahApps.Metro.IconPacks;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using static GFAlarm.View.Menu.DashboardView;

namespace GFAlarm
{
    /// <summary>
    /// 메인화면
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Variables

        internal static MainWindow view;                            // 메인 창
        internal static SubWindow subView = new SubWindow();        // 서브 창
        //internal static MapWindow mapView = new MapWindow();      // 맵 창

        private Timer timer;                           // 타이머
        public bool forceStop = true;                  // 업데이트 중지 여부

        private Grid WindowTitlebarGrid;                // 윈도우 타이틀 바
        private Border WindowBorder;                    // 윈도우 경계
        private ToggleButton AlwaysOnTopButton;         // 항상 위 버튼
        
        public static DashboardView dashboardView = new DashboardView();            // 알림 뷰
        public static EchelonView echelonView = new EchelonView();                  // 제대 뷰
        public static QuestView questView = new QuestView();                        // 임무 뷰
        public static ProxyView2 proxyView = new ProxyView2();                      // 연결 뷰
        public static SettingView2 settingView = new SettingView2();                // 설정 뷰
        public static SettingAlarmView2 settingAlarmView = new SettingAlarmView2(); // 알림 설정 뷰

        public static ProxyGuideView proxyGuideView = new ProxyGuideView();         // 프록시 가이드 뷰

        #region Footer

        public bool isMaxBp                              // 모의작전점수 최대 여부
        {
            get { return _isMaxBp; }
            set
            {
                if (_isMaxBp == value)
                    return;
                _isMaxBp = value;
                Dispatcher.Invoke(() =>
                {
                    if (value)
                    { 
                        this.BpPointTextBlock.Foreground = Application.Current.Resources["OrangeBrush"] as Brush;
                        this.BpPointRechargeTimeTextBlock.Foreground = Application.Current.Resources["OrangeBrush"] as Brush;
                    }
                    else
                    {
                        this.BpPointTextBlock.Foreground = Application.Current.Resources["NormalBrush"] as Brush;
                        this.BpPointRechargeTimeTextBlock.Foreground = Application.Current.Resources["NormalBrush"] as Brush;
                    }
                });
            }
        }
        private bool _isMaxBp = false;
        public bool isMaxGlobalExp                       // 자유경험치 최대 여부
        {
            get { return _isMaxGlobalExp; }
            set
            {
                if (_isMaxGlobalExp == value)
                    return;
                _isMaxGlobalExp = value;
                Dispatcher.Invoke(() =>
                {
                    if (value)
                    {
                        this.GlobalExpPercentTextBlock.Foreground = Application.Current.Resources["OrangeBrush"] as Brush;
                        this.GlobalExpPointTextBlock.Foreground = Application.Current.Resources["OrangeBrush"] as Brush;
                    }
                    else
                    {
                        this.GlobalExpPercentTextBlock.Foreground = Application.Current.Resources["NormalBrush"] as Brush;
                        this.GlobalExpPointTextBlock.Foreground = Application.Current.Resources["NormalBrush"] as Brush;
                    }
                });
            }
        }
        private bool _isMaxGlobalExp = false;
        public bool isCompleteBattleReport               // 작전보고서 완료 여부
        {
            get { return _isCompleteBattleReport; }
            set
            {
                if (_isCompleteBattleReport == value)
                    return;
                _isCompleteBattleReport = value;
                Dispatcher.Invoke(() =>
                {
                    if (value)
                        this.BattleReportRemainTimeTextBlock.Foreground = Application.Current.Resources["OrangeBrush"] as Brush;
                    else
                        this.BattleReportRemainTimeTextBlock.Foreground = Application.Current.Resources["NormalBrush"] as Brush;
                });
            }
        }
        private bool _isCompleteBattleReport = false;
        public bool isMaxReinforce                       // 지원 최대 여부
        {
            get { return _isMaxReinforce; }
            set
            {
                if (_isMaxReinforce == value)
                    return;
                _isMaxReinforce = value;
                Dispatcher.Invoke(() =>
                {
                    if (value)
                        this.ReinforceCountTextBlock.Foreground = Application.Current.Resources["OrangeBrush"] as Brush;
                    else
                        this.ReinforceCountTextBlock.Foreground = Application.Current.Resources["NormalBrush"] as Brush;
                });
            }
        }
        private bool _isMaxReinforce = false;
        public bool isMaxDoll                            // 인형 최대 여부
        {
            get { return _isMaxDoll; }
            set
            {
                if (_isMaxDoll == value)
                    return;
                _isMaxDoll = value;
                Dispatcher.Invoke(() =>
                {
                    if (value)
                        this.DollCounTextBlock.Foreground = Application.Current.Resources["OrangeBrush"] as Brush;
                    else
                        this.DollCounTextBlock.Foreground = Application.Current.Resources["NormalBrush"] as Brush;
                });
            }
        }
        private bool _isMaxDoll = false;
        public bool isMaxEquip                           // 장비 최대 여부
        {
            get { return _isMaxEquip; }
            set
            {
                if (_isMaxEquip == value)
                    return;
                _isMaxEquip = value;
                Dispatcher.Invoke(() =>
                {
                    if (value)
                        this.EquipCounTextBlock.Foreground = Application.Current.Resources["OrangeBrush"] as Brush;
                    else 
                        this.EquipCounTextBlock.Foreground = Application.Current.Resources["NormalBrush"] as Brush;
                });
            }
        }
        private bool _isMaxEquip = false;

        #endregion

        #endregion

        /// <summary>
        /// 타이머 틱
        /// </summary>
        /// <param name="state"></param>
        private void Tick(object state)
        {
            if (forceStop)
                return;

            int nowTime = TimeUtil.GetCurrentSec();
            UserData.currentSec = nowTime;

            #region CombatSimulation
            if (!UserData.CombatSimulation.pauseRefresh)
            {
                int tempNowTime = nowTime + TimeUtil.testSec;
                // 알림 점수 도래
                if (tempNowTime > UserData.CombatSimulation.recoverTime)
                {
                    UserData.CombatSimulation.point++;
                    if (Config.Alarm.notifyMaxBp &&
                        Config.Alarm.notifyMaxBpPoint <= UserData.CombatSimulation.point &&
                        UserData.CombatSimulation.notified == false)
                    {
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.All,
                            type = MessageType.reach_max_bp_point,
                            subject = LanguageResources.Instance["MESSAGE_MAX_SIM_POINT_SUBJECT"],
                            content = string.Format(LanguageResources.Instance["MESSAGE_MAX_SIM_POINT_CONTENT"],
                                                        UserData.CombatSimulation.point)
                        });
                        UserData.CombatSimulation.notified = true;
                    }
                    // 점수 꽉 참
                    if (UserData.CombatSimulation.point >= 6)
                    {
                        UserData.CombatSimulation.point = 6;
                        UserData.CombatSimulation.recoverTime = int.MaxValue;
                        UserData.CombatSimulation.lastRecoverTime = tempNowTime;
                        UserData.CombatSimulation.remainPointToday = UserData.CombatSimulation.GetRemainPoint();
                        UserData.CombatSimulation.notified = true;
                    }
                    else
                    {
                        UserData.CombatSimulation.recoverTime += TimeUtil.HOUR * 2;
                        UserData.CombatSimulation.lastRecoverTime = tempNowTime;
                        UserData.CombatSimulation.remainPointToday = UserData.CombatSimulation.GetRemainPoint();
                        UserData.CombatSimulation.notified = false;
                    }
                }
                if (UserData.CombatSimulation.point >= 6)
                {
                    UserData.CombatSimulation.remainTime = "00:00:00";
                }
                else
                {
                    UserData.CombatSimulation.remainTime = TimeUtil.GetRemainHHMMSS(UserData.CombatSimulation.recoverTime, tempNowTime);    
                }
            }
            //if (0 <= UserData.CombatSimulation.point && 
            //    UserData.CombatSimulation.point <= 5 && 
            //    UserData.CombatSimulation.recoverTime > 0)
            //{
            //    if (nowTime > UserData.CombatSimulation.recoverTime)
            //    {
            //        UserData.CombatSimulation.point++;
            //        if (Config.Alarm.notifyMaxBp &&
            //            Config.Alarm.notifyMaxBpPoint <= UserData.CombatSimulation.point &&
            //            UserData.CombatSimulation.notified == false)
            //        {
            //            Notifier.Manager.notifyQueue.Enqueue(new Message()
            //            {
            //                send = MessageSend.All,
            //                type = MessageType.reach_max_bp_point,
            //                subject = LanguageResources.Instance["MESSAGE_MAX_SIM_POINT_SUBJECT"],
            //                content = string.Format(LanguageResources.Instance["MESSAGE_MAX_SIM_POINT_CONTENT"],
            //                                            UserData.CombatSimulation.point)
            //            });
            //            UserData.CombatSimulation.notified = true;
            //        }
            //        if (UserData.CombatSimulation.point >= 6)
            //        {
            //            UserData.CombatSimulation.point = 6;
            //            UserData.CombatSimulation.recoverTime = int.MaxValue;
            //            UserData.CombatSimulation.lastRecoverTime = nowTime;
            //            UserData.CombatSimulation.notified = true;
            //        }
            //        else if (UserData.CombatSimulation.point < 6)
            //        {
            //            UserData.CombatSimulation.recoverTime += TimeUtil.HOUR * 2;
            //            UserData.CombatSimulation.lastRecoverTime = nowTime;
            //            UserData.CombatSimulation.notified = false;
            //        }
            //    }
            //    UserData.CombatSimulation.remainTime = TimeUtil.GetRemainHHMMSS(UserData.CombatSimulation.recoverTime, nowTime);
            //}
            //else if (UserData.CombatSimulation.point >= 6)
            //{
            //    UserData.CombatSimulation.remainTime = "00:00:00";
            //}
            #endregion

            #region BattleReport
            if (UserData.BattleReport.num > 0)
            {
                // 알림시간 도래
                if (nowTime > UserData.BattleReport.endTime - Config.Extra.earlyNotifySeconds)
                {
                    // 알림
                    if (Config.Alarm.notifyBattleReportComplete && UserData.BattleReport.notified == false)
                    {
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            send = MessageSend.All,
                            type = MessageType.complete_battle_report,
                            subject = LanguageResources.Instance["MESSAGE_COMPLETE_COMBAT_REPORT_SUBJECT"],
                            content = string.Format(LanguageResources.Instance["MESSAGE_COMPLETE_COMBAT_REPORT_CONTENT"], 
                                                        UserData.BattleReport.num),
                        });
                        UserData.BattleReport.notified = true;
                    }
                    //log.Debug("작전보고서 남은 시간 {0}", "00:00:00");
                    UserData.BattleReport.remainTime = "00:00:00";
                }
                else
                {
                    string tempRemainTime = TimeUtil.GetRemainHHMMSS(UserData.BattleReport.endTime, nowTime);
                    //log.Debug("작전보고서 남은 시간 {0}", tempRemainTime);
                    UserData.BattleReport.remainTime = tempRemainTime;
                }
            }
            #endregion

            #region DispatchedEchelon
            for (int i = 0; i < dashboardView.DispatchedEchelonList.Count(); i++)
            {
                var item = dashboardView.DispatchedEchelonList[i];
                int endTime = item.endTime;

                item.TBRemainTime = TimeUtil.GetRemainHHMMSS(endTime, nowTime);
                // 자율작전인 경우, 완료 횟수 갱신
                if (item.type == 2)
                {
                    int number = item.autoMissionNumber;
                    int missionCount = number - ((endTime - nowTime) / item.requireTime) - 1;
                    if (nowTime > endTime)
                        item.TBNumber = string.Format("{0}/{1}", number, number);
                    else
                        item.TBNumber = string.Format("{0}/{1}", missionCount, number);
                }
                // 알림시간 도래
                if (nowTime > endTime - Config.Extra.earlyNotifySeconds)
                {
                    // 알림
                    if (!item.notified)
                    {
                        int operationId = item.operationId;
                        int autoMissionId = item.autoMissionId;

                        item.notified = true;
                        if (Config.Alarm.notifyDispatchedEchelonComplete)
                        {
                            // 군수지원
                            if (operationId > 0)
                            {
                                long gunWithUserId = UserData.Doll.GetTeamLeaderGunWithUserId(item.teamId);
                                DollWithUserInfo leaderDoll = UserData.Doll.Get(gunWithUserId);
                                int gunId = 0;
                                int skinId = 0;
                                if (leaderDoll != null)
                                {
                                    gunId = leaderDoll.no;
                                    skinId = leaderDoll.skin;
                                }
                                Notifier.Manager.notifyQueue.Enqueue(new Message()
                                {
                                    send = MessageSend.All,
                                    type = MessageType.complete_operation,
                                    gunId = gunId,
                                    skinId = skinId,
                                    subject = LanguageResources.Instance["MESSAGE_COMPLETE_LOGISTICS_SUBJECT"],
                                    content = string.Format(LanguageResources.Instance["MESSAGE_COMPLETE_LOGISTICS_CONTENT"],
                                                                item.TBCode)
                                });
                            }
                            // 자율작전
                            else if (autoMissionId > 0)
                            {
                                int teamId = item.teamId;
                                long gunWithUserId = UserData.Doll.GetTeamLeaderGunWithUserId(teamId);
                                DollWithUserInfo doll = UserData.Doll.Get(gunWithUserId);
                                int gunId = 0;
                                int skinId = 0;
                                if (doll != null)
                                {
                                    gunId = doll.no;
                                    skinId = doll.skin;
                                }
                                Notifier.Manager.notifyQueue.Enqueue(new Message()
                                {
                                    send = MessageSend.All,
                                    type = MessageType.complete_auto_mission,
                                    gunId = gunId,
                                    skinId = skinId,
                                    subject = LanguageResources.Instance["MESSAGE_COMPLETE_AUTO_BATTLE_SUBJECT"],
                                    content = string.Format(LanguageResources.Instance["MESSAGE_COMPLETE_AUTO_BATTLE_CONTENT"],
                                                                item.TBCode)
                                });
                            }
                            // 탭 알림 활성
                            MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        }
                    }
                    continue;
                }
            }
            #endregion

            #region RestoreDoll
            for (int i = 0; i < dashboardView.RestoreDollList.Count(); i++)
            {
                var item = dashboardView.RestoreDollList[i];

                long gunWithUserId = item.gunWithUserId;
                int endTime = item.endTime;

                item.TBRemainTime = TimeUtil.GetRemainHHMMSS(endTime, nowTime);
                // 알림시간 도래
                if (nowTime > endTime - Config.Extra.earlyNotifySeconds)
                {
                    if (!item.notified)
                    {
                        item.notified = true;
                        if (Config.Alarm.notifyRestoreDollComplete)
                        {
                            DollWithUserInfo doll = UserData.Doll.Get(gunWithUserId);
                            int gunId = 0;
                            int skinId = 0;
                            string gunName = "unknown";
                            if (doll != null)
                            {
                                gunId = doll.no;
                                gunName = doll.name;
                                skinId = doll.skin;
                            }
                            Notifier.Manager.notifyQueue.Enqueue(new Message()
                            {
                                send = MessageSend.All,
                                type = MessageType.complete_restore_doll,
                                gunId = gunId,
                                skinId = skinId,
                                subject = LanguageResources.Instance["MESSAGE_COMPLETE_RESTORE_SUBJECT"],
                                content = string.Format(LanguageResources.Instance["MESSAGE_COMPLETE_RESTORE_CONTENT"],
                                                            gunName)
                            });
                            // 탭 알림 활성
                            MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        }
                    }
                    continue;
                }
            }
            #endregion

            #region ProduceDoll
            for (int i = 0; i < dashboardView.ProduceDollList.Count(); i++)
            {
                var item = dashboardView.ProduceDollList[i];

                item.TBRemainTime = TimeUtil.GetRemainHHMMSS(item.endTime, nowTime);

                // 알림시간 도래
                if (nowTime > item.endTime - Config.Extra.earlyNotifySeconds)
                {
                    // 알림
                    if (!item.notified)
                    {
                        item.notified = true;
                        if (Config.Alarm.notifyProduceDollComplete)
                        {
                            // 인형제조 완료
                            Notifier.Manager.notifyQueue.Enqueue(new Message()
                            {
                                send = MessageSend.All,
                                type = MessageType.complete_produce_doll,
                                gunId = UserData.adjutantDoll,
                                skinId = UserData.adjutantDollSkin,
                                subject = LanguageResources.Instance["MESSAGE_COMPLETE_PRODUCE_DOLL_SUBJECT"],
                                content = string.Format(LanguageResources.Instance["MESSAGE_COMPLETE_PRODUCE_DOLL_CONTENT"],
                                                            item.star,
                                                            item.gunName,
                                                            item.category)
                            });
                            // 탭 알림 활성
                            MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        }
                    }
                    continue;
                }
            }
            #endregion

            #region ProduceEquip
            for (int i = 0; i < dashboardView.ProduceEquipList.Count(); i++)
            {
                var item = dashboardView.ProduceEquipList[i];

                item.TBRemainTime = TimeUtil.GetRemainHHMMSS(item.endTime, nowTime);

                // 알림시간 도래
                if (nowTime > item.endTime - Config.Extra.earlyNotifySeconds)
                {
                    // 알림
                    if (!item.notified)
                    {
                        item.notified = true;
                        if (Config.Alarm.notifyProduceEquipComplete)
                        {
                            // 장비제조 완료
                            if (item.equipId != 0)
                            {
                                Notifier.Manager.notifyQueue.Enqueue(new Message()
                                {
                                    send = MessageSend.All,
                                    type = MessageType.complete_produce_equip,
                                    subject = LanguageResources.Instance["MESSAGE_COMPLETE_PRODUCE_EQUIP_SUBJECT"],
                                    content = string.Format(LanguageResources.Instance["MESSAGE_COMPLETE_PRODUCE_EQUIP_CONTENT"],
                                                                item.star,
                                                                item.equipNameShort),
                                });
                            }
                            // 요정제조 완료
                            else if (item.fairyId != 0)
                            {
                                Notifier.Manager.notifyQueue.Enqueue(new Message()
                                {
                                    send = MessageSend.All,
                                    type = MessageType.complete_produce_equip,
                                    subject = LanguageResources.Instance["MESSAGE_COMPLETE_PRODUCE_EQUIP_SUBJECT"],
                                    content = string.Format(LanguageResources.Instance["MESSAGE_COMPLETE_PRODUCE_FAIRY_CONTENT"],
                                                                item.fairyName),
                                });
                            }
                            // 탭 알림 활성
                            MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        }
                    }
                    continue;
                }
            }
            #endregion

            #region SkillTrain
            for (int i = 0; i < dashboardView.SkillTrainList.Count(); i++)
            {
                var item = dashboardView.SkillTrainList[i];

                int endTime = item.endTime;

                item.TBRemainTime = TimeUtil.GetRemainHHMMSS(endTime, nowTime);

                // 알림시간 도래
                if (nowTime > endTime - Config.Extra.earlyNotifySeconds)
                {
                    // 알림
                    if (!item.notified)
                    {
                        item.notified = true;
                        if (Config.Alarm.notifySkillTrainComplete)
                        {
                            string content = "";
                            if (item.gunWithUserId != 0)
                            {
                                content = string.Format(LanguageResources.Instance["MESSAGE_COMPLETE_TRAIN_DOLL_CONTENT"],
                                                            item.gunName);
                            }
                            else if (item.fairyWithUserId != 0)
                            {
                                content = string.Format(LanguageResources.Instance["MESSAGE_COMPLETE_TRAIN_FAIRY_CONTENT"],
                                                            item.fairyName);
                            }
                            else if (item.squadWithUserId != 0)
                            {
                                content = string.Format(LanguageResources.Instance["MESSAGE_COMPLETE_TRAIN_SQUAD_CONTENT"],
                                                            item.squadName);
                            }
                            if (item.isExpTrain == true)
                            {
                                Notifier.Manager.notifyQueue.Enqueue(new Message()
                                {
                                    send = MessageSend.All,
                                    type = MessageType.complete_exp_train,
                                    subject = LanguageResources.Instance["MESSAGE_COMPLETE_TRAIN_EXP_SUBJECT"],
                                    content = content
                                });
                            }
                            else
                            {
                                Notifier.Manager.notifyQueue.Enqueue(new Message()
                                {
                                    send = MessageSend.All,
                                    type = MessageType.complete_skill_train,
                                    subject = LanguageResources.Instance["MESSAGE_COMPLETE_TRAIN_SKILL_SUBJECT"],
                                    content = content
                                });
                            }
                            // 탭 알림 활성
                            MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        }
                    }
                    continue;
                }
            }
            #endregion

            #region Data Analysis
            foreach (int endTime in dashboardView.DataAnalysisEndTimes)
            {
                // 알림시간 도래
                if (nowTime > endTime - Config.Extra.earlyNotifySeconds)
                {
                    // 알림
                    if (Config.Alarm.notifyDataAnalysisComplete)
                    {
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            type = MessageType.complete_data_analysis,
                            subject = LanguageResources.Instance["MESSAGE_COMPLETE_DATA_ANALYSIS_SUBJECT"],
                            content = LanguageResources.Instance["MESSAGE_COMPLETE_DATA_ANALYSIS_CONTENT"],
                        });
                        // 탭 알림 활성
                        MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                    }
                    dashboardView.DataAnalysisEndTimes.RemoveWhere(delegate (int i) { return i == endTime; });
                    break;
                }
            }
            for (int i = 0; i < dashboardView.DataAnalysisList.Count(); i++)
            {
                dashboardView.DataAnalysisList[i].TBRemainTime = TimeUtil.GetRemainHHMMSS(dashboardView.DataAnalysisList[i].endTime, nowTime);
            }
            #endregion

            #region Explore
            for (int i = 0; i < dashboardView.ExploreList.Count(); i++)
            {
                var item = dashboardView.ExploreList[i];

                int endTime = item.endTime;
                int nextTime = item.nextTime;

                item.TBRemainTime = TimeUtil.GetRemainHHMMSS(endTime, nowTime);
                item.TBEventRemainTime = TimeUtil.GetRemainHHMMSS(nextTime, nowTime);

                // 알림시간 도래
                if (nowTime > endTime - Config.Extra.earlyNotifySeconds)
                {
                    // 알림
                    if (!item.notified)
                    {
                        item.notified = true;
                        if (Config.Alarm.notifyExploreComplete)
                        {
                            Notifier.Manager.notifyQueue.Enqueue(new Message()
                            {
                                send = MessageSend.All,
                                type = MessageType.complete_explore,
                                subject = LanguageResources.Instance["MESSAGE_COMPLETE_EXPLORE_SUBJECT"],
                                content = LanguageResources.Instance["MESSAGE_COMPLETE_EXPLORE_CONTENT"],
                            });
                            // 탭 알림 활성
                            MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        }
                    }
                }
            }
            #endregion
        }

        #region Side Menu

        /// <summary>
        /// 사이드 메뉴 버튼 > 왼쪽 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuToggleButton_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            if (button != null)
            {
                button.IsChecked = true;
                string name = button.Name;
                switch (name)
                {
                    case "DashboardMenuToggleButton":
                        this.CurrentMenu = Menus.DASHBOARD;
                        break;
                    case "EchelonMenuToggleButton":
                        this.CurrentMenu = Menus.ECHELON;
                        break;
                    case "QuestMenuToggleButton":
                        this.CurrentMenu = Menus.QUEST;
                        break;
                    //case "WarehouseMenuToggleButton":
                    //    this.CurrentMenu = Menus.WAREHOUSE;
                    //    break;
                    case "ProxyMenuToggleButton":
                        this.CurrentMenu = Menus.PROXY;
                        break;
                    case "SettingMenuToggleButton":
                        this.CurrentMenu = Menus.SETTING;
                        break;
                    case "AlarmSettingToggleButton":
                        this.CurrentMenu = Menus.SETTING_ALARM;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 사이트 메뉴 > 오른쪽 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuToggleButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (subView == null)
                subView = new SubWindow();

            string name = (sender as ToggleButton).Name;
            switch (name)
            {
                case "DashboardMenuToggleButton":
                    if (this.CurrentMenu != Menus.DASHBOARD)
                    {
                        subView.SetContent(Menus.DASHBOARD);
                        subView.Show();
                        MainWindow.view.SetIconNotify(Menus.DASHBOARD, false);
                    }
                    break;
                case "EchelonMenuToggleButton":
                    if (this.CurrentMenu != Menus.ECHELON)
                    {
                        subView.SetContent(Menus.ECHELON);
                        subView.Show();
                        MainWindow.view.SetIconNotify(Menus.ECHELON, false);
                    }
                    break;
                case "QuestMenuToggleButton":
                    if (this.CurrentMenu != Menus.QUEST)
                    {
                        subView.SetContent(Menus.QUEST);
                        subView.Show();
                        MainWindow.view.SetIconNotify(Menus.QUEST, false);
                    }
                    break;
                //case "WarehouseMenuToggleButton":
                //    if (this.CurrentMenu != Menus.WAREHOUSE)
                //    {
                //        subView.SetContent(Menus.WAREHOUSE);
                //        subView.Show();
                //    }
                //    break;
                case "ProxyMenuToggleButton":
                    break;
                case "SettingMenuToggleButton":
                    break;
                case "AlarmSettingToggleButton":
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 사이드 메뉴 > 마우스 휠
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideMenuGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int currentMenu = this.CurrentMenu;
            // wheel up
            if (e.Delta > 0)
            {
                currentMenu--;
                if (currentMenu == subView.CurrentMenu)
                    currentMenu--;
                if (currentMenu < Menus.DASHBOARD)
                    currentMenu = Menus.SETTING;
            }
            // wheel down
            else if (e.Delta < 0)
            {
                currentMenu++;
                if (currentMenu > Menus.SETTING)
                    currentMenu = Menus.DASHBOARD;
                if (currentMenu == subView.CurrentMenu)
                    currentMenu++;
            }
            this.CurrentMenu = currentMenu;
        }

        /// <summary>
        /// 사이드 메뉴 > 초기화
        /// </summary>
        public void ResetSideMenu()
        {
            Dispatcher.Invoke(() =>
            {
                if (this.DashboardMenuToggleButton.IsChecked == true) this.DashboardMenuToggleButton.IsChecked = false;
                if (this.EchelonMenuToggleButton.IsChecked == true) this.EchelonMenuToggleButton.IsChecked = false;
                if (this.QuestMenuToggleButton.IsChecked == true) this.QuestMenuToggleButton.IsChecked = false;
                if (this.ProxyMenuToggleButton.IsChecked == true) this.WarehouseMenuToggleButton.IsChecked = false;
                if (this.ProxyMenuToggleButton.IsChecked == true) this.ProxyMenuToggleButton.IsChecked = false;
                if (this.AlarmSettingToggleButton.IsChecked == true) this.AlarmSettingToggleButton.IsChecked = false;
                if (this.SettingMenuToggleButton.IsChecked == true) this.SettingMenuToggleButton.IsChecked = false;

                if (this.FilterGrid.Visibility == Visibility.Visible) this.FilterGrid.Visibility = Visibility.Collapsed;
                if (this.SortGrid.Visibility == Visibility.Visible) this.SortGrid.Visibility = Visibility.Collapsed;
                if (this.ExpCalGrid.Visibility == Visibility.Visible) this.ExpCalGrid.Visibility = Visibility.Collapsed;
                if (this.ExpandCollapseButton.Visibility == Visibility.Visible) this.ExpandCollapseButton.Visibility = Visibility.Collapsed;

            });
        }

        /// <summary>
        /// 사이드 메뉴 > 현재 메뉴
        /// </summary>
        private int _CurrentMenu = -1;
        public int CurrentMenu
        {
            get { return _CurrentMenu; }
            set
            {
                Dispatcher.Invoke(() =>
                {
                    switch (value)
                    {
                        case Menus.DASHBOARD:
                            MainWindow.view.SetIconNotify(Menus.DASHBOARD, false);
                            break;
                        case Menus.ECHELON:
                            MainWindow.view.SetIconNotify(Menus.ECHELON, false);
                            break;
                        case Menus.QUEST:
                            MainWindow.view.SetIconNotify(Menus.QUEST, false);
                            break;
                    }
                });
                if (_CurrentMenu == value)
                    return;
                ResetSideMenu();
                Dispatcher.Invoke(() =>
                {
                    switch (value)
                    {
                        case Menus.DASHBOARD:
                            this.DashboardMenuToggleButton.IsChecked = true;
                            this.TitleTextBlock.Text = LanguageResources.Instance["TAB_DASHBOARD"];
                            this.ViewContentControl.Content = dashboardView;
                            this.FilterGrid.Visibility = Visibility.Visible;
                            this.SortGrid.Visibility = Visibility.Visible;
                            this.ExpandCollapseButton.Visibility = Visibility.Visible;
                            this.ExpandCollapseButtonIcon.Kind = dashboardView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                            MainWindow.view.SetIconNotify(Menus.DASHBOARD, false);
                            break;
                        case Menus.ECHELON:
                            this.EchelonMenuToggleButton.IsChecked = true;
                            this.TitleTextBlock.Text = LanguageResources.Instance["TAB_ECHELON"];
                            this.ViewContentControl.Content = echelonView;
                            this.ExpCalGrid.Visibility = Visibility.Visible;
                            this.ExpandCollapseButton.Visibility = Visibility.Visible;
                            this.ExpandCollapseButtonIcon.Kind = echelonView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                            break;
                        case Menus.QUEST:
                            this.QuestMenuToggleButton.IsChecked = true;
                            this.TitleTextBlock.Text = LanguageResources.Instance["TAB_QUEST"];
                            this.ViewContentControl.Content = questView;
                            this.ExpandCollapseButton.Visibility = Visibility.Visible;
                            this.ExpandCollapseButtonIcon.Kind = questView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                            MainWindow.view.SetIconNotify(Menus.QUEST, false);
                            break;
                        //case Menus.WAREHOUSE:
                        //    this.WarehouseMenuToggleButton.IsChecked = true;
                        //    this.TBTitle.Text = LanguageResources.Instance["TAB_WAREHOUSE"];
                        //    this.ViewContentControl.Content = warehouseView;
                        //    this.ExpandCollapseButton.Visibility = Visibility.Visible;
                        //    break;
                        case Menus.PROXY:
                            this.ProxyMenuToggleButton.IsChecked = true;
                            this.TitleTextBlock.Text = LanguageResources.Instance["TAB_PROXY"];
                            this.ViewContentControl.Content = proxyView;
                            this.ExpandCollapseButton.Visibility = Visibility.Visible;
                            this.ExpandCollapseButtonIcon.Kind = proxyView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                            break;
                        case Menus.SETTING_ALARM:
                            this.AlarmSettingToggleButton.IsChecked = true;
                            this.TitleTextBlock.Text = LanguageResources.Instance["TAB_NOTIFICATION_SETTING"];
                            this.ViewContentControl.Content = settingAlarmView;
                            this.ExpandCollapseButton.Visibility = Visibility.Visible;
                            this.ExpandCollapseButtonIcon.Kind = settingAlarmView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                            break;
                        case Menus.SETTING:
                            this.SettingMenuToggleButton.IsChecked = true;
                            this.TitleTextBlock.Text = LanguageResources.Instance["TAB_SETTING"];
                            this.ViewContentControl.Content = settingView;
                            this.ExpandCollapseButton.Visibility = Visibility.Visible;
                            this.ExpandCollapseButtonIcon.Kind = settingView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                            break;
                        default:
                            break;
                    }

                    _CurrentMenu = value;
                });
            }
        }

        /// <summary>
        /// 탭 활성화
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="enable"></param>
        public void SetIconEnable(int menu, bool enable)
        {
            Dispatcher.Invoke(() =>
            {
                double opacity = enable == true ? 1 : 0.25;
                switch (menu)
                {
                    case Menus.DASHBOARD:
                        this.DashboardMenuIcon.Opacity = opacity;
                        break;
                    case Menus.ECHELON:
                        this.EchelonMenuIcon.Opacity = opacity;
                        break;
                    case Menus.QUEST:
                        this.QuestMenuIcon.Opacity = opacity;
                        break;
                        //case Menus.WAREHOUSE:
                        //    this.WarehouseMenuIcon.Opacity = opacity;
                        //    break;
                }
            });
        }

        /// <summary>
        /// 탭 하이라이트
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="notify"></param>
        public void SetIconNotify(int menu, bool notify)
        {
            Dispatcher.Invoke(() =>
            {
                Rectangle MenuToggleButton_Notify = null;
                switch (menu)
                {
                    case Menus.DASHBOARD:
                        MenuToggleButton_Notify = this.FindName("DashboardMenuToggleButton_Notify") as Rectangle;
                        break;
                    case Menus.ECHELON:
                        MenuToggleButton_Notify = this.FindName("EchelonMenuToggleButton_Notify") as Rectangle;
                        break;
                    case Menus.QUEST:
                        MenuToggleButton_Notify = this.FindName("QuestMenuToggleButton_Notify") as Rectangle;
                        break;
                    default:
                        break;
                }
                if (MenuToggleButton_Notify != null && QuestMenuToggleButton_Notify is Rectangle)
                {
                    if (notify)
                    {
                        if (!Config.Setting.tabNotification)
                            return;
                        MenuToggleButton_Notify.BeginAnimation(Rectangle.OpacityProperty, Animations.Flicking);
                        MenuToggleButton_Notify.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MenuToggleButton_Notify.BeginAnimation(Rectangle.OpacityProperty, null);
                        MenuToggleButton_Notify.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }

        /// <summary>
        /// 임무 탭 완료 여부
        /// </summary>
        public bool CompleteQuestTab
        {
            set
            {
                PackIconMaterialKind icon;
                switch (value)
                {
                    case true:
                        icon = PackIconMaterialKind.ClipboardCheckOutline;
                        break;
                    case false:
                    default:
                        icon = PackIconMaterialKind.ClipboardTextOutline;
                        break;
                }
                Dispatcher.Invoke(() =>
                {
                    this.QuestMenuIcon.Kind = icon;
                    if (UserData.Quest.isOpenQuestMenu == false)
                        this.QuestMenuIcon.Opacity = 0.25;
                });
            }
        }

        #endregion

        #region Footer Menu (Combat Simulation, Global Exp)

        /// <summary>
        /// 연결 상태
        /// </summary>
        public enum ConnectionStatus
        {
            Connect,
            Unstable,
            Disconnect,
        }

        /// <summary>
        /// 연결상태
        /// </summary>
        public ConnectionStatus Connection
        {
            set
            {
                Dispatcher.Invoke(() =>
                {
                    switch (value)
                    {
                        case ConnectionStatus.Connect:
                            this.ConnectionStatusIcon.Foreground = Application.Current.Resources["ConnectBrush"] as Brush;
                            this.ConnectionStatusIcon.Kind = PackIconMaterialKind.Wifi;
                            break;
                        case ConnectionStatus.Unstable:
                            this.ConnectionStatusIcon.Foreground = Application.Current.Resources["DisconnectBrush"] as Brush;
                            this.ConnectionStatusIcon.Kind = PackIconMaterialKind.Wifi;
                            break;
                        case ConnectionStatus.Disconnect:
                            this.ConnectionStatusIcon.Foreground = Application.Current.Resources["DisconnectBrush"] as Brush;
                            this.ConnectionStatusIcon.Kind = PackIconMaterialKind.WifiOff;
                            break;
                    }
                });
            }
        }

        /// <summary>
        /// 오늘 충전가능한 모의작전점수
        /// </summary>
        public void SetBpPointToolTip(int remain_point, string attendance_time)
        {
            Dispatcher.Invoke(() =>
            {
                this.BpPointToolTipTextBlock.Html = string.Format(LanguageResources.Instance["FOOTER_SIM_POINT_TOOLTIP"],
                    "[font color='#FFB400']" + remain_point + "[/font]",
                    "[font color='#FFB400']" + attendance_time + "[/font]");
            });
        }

        /// <summary>
        /// 현재 작전
        /// </summary>
        public string CurrentMission
        {
            set
            {
                //Dispatcher.Invoke(() =>
                //{
                //    if (!string.IsNullOrEmpty(value))
                //    {
                //        this.CurrentMissionTextBlock.Text = value;
                //    }
                //    else
                //    {
                //        this.CurrentMissionTextBlock.Text = "대기 중";
                //    }
                //});
            }
        }

        /// <summary>
        /// 푸터 열림 여부
        /// </summary>
        public bool expandFooter
        {
            get
            {
                return _expandFooter;
            }
            set
            {
                _expandFooter = value;
                if (value)
                {
                    Animations.ChangeHeight.From = this.FooterGrid.Height;
                    Animations.ChangeHeight.To = 54;
                }
                else
                {
                    Animations.ChangeHeight.From = this.FooterGrid.Height;
                    Animations.ChangeHeight.To = 32;
                }
                this.FooterGrid.BeginAnimation(Grid.HeightProperty, null);
                this.FooterGrid.BeginAnimation(Grid.HeightProperty, Animations.ChangeHeight);
            }
        }
        private bool _expandFooter = Config.Footer.expand;

        /// <summary>
        /// 푸터 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FooterGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (animateFreeExpGrid)
                return;
            Dispatcher.Invoke(() =>
            {
                if (this.expandFooter)
                {
                    Config.Footer.expand = false;
                    this.expandFooter = false;
                }
                else
                {
                    Config.Footer.expand = true;
                    this.expandFooter = true;
                }
            });
        }

        /// <summary>
        /// 자유경험치 보이기
        /// </summary>
        /// <returns></returns>
        bool animateFreeExpGrid = false;
        public void ShowFreeExpGrid()
        {
            try
            {
                Dispatcher.Invoke(async () =>
                {
                    animateFreeExpGrid = true;
                    this.expandFooter = true;
                    await Task.Delay(5000);
                    if (this.expandFooter == true)
                        this.expandFooter = false;
                    animateFreeExpGrid = false;
                });
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        #endregion

        #region Header Menu (Filter/Sort)

        /// <summary>
        /// 필터링 메뉴 활성/비활성화
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="enable"></param>
        public void SetFilterIconEnable(int menu, bool enable)
        {
            Dispatcher.Invoke(() =>
            {
                double opacity = enable == true ? 1 : 0.25;
                switch (menu)
                {
                    case GroupIdx.DISPATCHED_ECHELON:
                        this.DispatchedEchelonFilterIcon.Opacity = opacity;
                        break;
                    case GroupIdx.PRODUCE_DOLL:
                        this.ProduceDollFilterIcon.Opacity = opacity;
                        break;
                    case GroupIdx.PRODUCE_EQUIP:
                        this.ProduceEquipFilterIcon.Opacity = opacity;
                        break;
                    case GroupIdx.SKILL_TRAIN:
                        this.SkillTrainFilterIcon.Opacity = opacity;
                        break;
                    case GroupIdx.DATA_ANALYSIS:
                        this.DataAnalysisFilterIcon.Opacity = opacity;
                        break;
                    case GroupIdx.RESTORE_DOLL:
                        this.RestoreDollFilterIcon.Opacity = opacity;
                        break;
                    case GroupIdx.EXPLORE:
                        this.ExploreFilterIcon.Opacity = opacity;
                        break;
                    default:
                        break;
                }
            });
        }

        /// <summary>
        /// 필터 버튼 오버
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        bool insideFilterGrid = false;
        private void FilterGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            //this.FilterToggleButton_Dropdown.IsEnabled = true;
            this.FilterToggleButton.Background = Application.Current.Resources["PrimaryDeactiveBrush"] as Brush;
            this.FilterToggleButton_Dropdown.Visibility = Visibility.Visible;
            Panel.SetZIndex(this.FilterGrid, 1000);
            Panel.SetZIndex(this.SortGrid, 999);
            Panel.SetZIndex(this.ExpCalGrid, 998);
            insideFilterGrid = true;
        }
        private void FilterGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.FilterToggleButton.IsChecked == false)
            {
                //this.FilterToggleButton_Dropdown.IsEnabled = false;
                this.FilterToggleButton.Background = Application.Current.Resources["TransparentBrush"] as Brush;
                this.FilterToggleButton_Dropdown.Visibility = Visibility.Collapsed;
            }
            insideFilterGrid = false;
        }

        /// <summary>
        /// 정렬 버튼 오버
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        bool insideSortGrid = false;
        private void SortGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            //this.SortToggleButton_Dropdown.IsEnabled = true;
            this.SortToggleButton.Background = Application.Current.Resources["PrimaryDeactiveBrush"] as Brush;
            this.SortToggleButton_Dropdown.Visibility = Visibility.Visible;
            Panel.SetZIndex(this.SortGrid, 1000);
            Panel.SetZIndex(this.FilterGrid, 999);
            Panel.SetZIndex(this.ExpCalGrid, 998);
            insideSortGrid = true;
        }

        private void SortGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.SortToggleButton.IsChecked == false)
            {
                //this.SortToggleButton_Dropdown.IsEnabled = false;
                this.SortToggleButton.Background = Application.Current.Resources["TransparentBrush"] as Brush;
                this.SortToggleButton_Dropdown.Visibility = Visibility.Collapsed;
            }
            insideSortGrid = false;
        }


        /// <summary>
        /// 경험치 계산 버튼 오버
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        bool insideExpCalGrid = false;
        private void ExpCalGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ExpCalToggleButton.Background = Application.Current.Resources["PrimaryDeactiveBrush"] as Brush;
            this.ExpCalToggleButton_Dropdown.Visibility = Visibility.Visible;
            Panel.SetZIndex(this.ExpCalGrid, 1000);
            Panel.SetZIndex(this.SortGrid, 999);
            Panel.SetZIndex(this.FilterGrid, 998);
            insideExpCalGrid = true;
        }

        private void ExpCalGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.ExpCalToggleButton.IsChecked == false)
            {
                this.ExpCalToggleButton.Background = Application.Current.Resources["TransparentBrush"] as Brush;
                this.ExpCalToggleButton_Dropdown.Visibility = Visibility.Collapsed;
            }
            insideExpCalGrid = false;
        }

        /// <summary>
        /// 경험치 계산기 프리셋 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PresetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int baseExp = 0;
            int battleCount = 0;
            int levelPenalty = 0;

            int index = (sender as ComboBox).SelectedIndex;
            switch (index)
            {
                case 0: // 4-3E
                    baseExp = 370;
                    battleCount = 4;
                    levelPenalty = 75;
                    break;
                case 1: // 0-2
                    baseExp = 490;
                    battleCount = 5;
                    levelPenalty = 112;
                    break;
                case 2: // 10-4E
                case 3: // 8-1N
                    baseExp = 500;
                    battleCount = 5;
                    levelPenalty = 120;
                    break;
                case 4: // 11-5
                    baseExp = 550;
                    battleCount = 5;
                    levelPenalty = 120;
                    break;
                case 5: // 4드라
                    baseExp = 500;
                    battleCount = 5;
                    levelPenalty = 115;
                    break;
            }
            Config.Echelon.baseExp = baseExp;
            Config.Echelon.battleCount = battleCount;
            Config.Echelon.levelPenalty = levelPenalty;

            BaseExpTextBox.Text = Config.Echelon.baseExp.ToString();
            BattleCountTextBox.Text = Config.Echelon.battleCount.ToString();
            LevelPenaltyTextBox.Text = Config.Echelon.levelPenalty.ToString();
        }

        /// <summary>
        /// 경험치 계산기 적용 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpCalApplyButton_Click(object sender, RoutedEventArgs e)
        {
            int baseExp = 0;
            if (int.TryParse(BaseExpTextBox.Text, out baseExp))
            {
                if (baseExp > 0)
                    Config.Echelon.baseExp = baseExp;
            }
            int battleCount = 0;
            if (int.TryParse(BattleCountTextBox.Text, out battleCount))
            {
                if (battleCount > 0)
                    Config.Echelon.battleCount = battleCount;
            }
            int levelPenalty = 0;
            if (int.TryParse(LevelPenaltyTextBox.Text, out levelPenalty))
            {
                if (levelPenalty > 0)
                    Config.Echelon.levelPenalty = levelPenalty;
            }
            bool expUpEvent = ExpCalExpUpCheckBox.IsChecked == true ? true : false;
            Config.Echelon.expUpEvent = expUpEvent;

            bool isSuccess = false;
            if (baseExp > 0 && battleCount > 0 && levelPenalty > 0)
            {
                echelonView.UpdateAll(true);
                isSuccess = true;
            }
            else
            {
                BaseExpTextBox.Text = Config.Echelon.baseExp.ToString();
                BattleCountTextBox.Text = Config.Echelon.battleCount.ToString();
                LevelPenaltyTextBox.Text = Config.Echelon.levelPenalty.ToString();
            }

            Dispatcher.Invoke(async () =>
            {
                if (isSuccess)
                {
                    ExpCalApplyButton.Background = Application.Current.Resources["GreenBrush"] as Brush;
                    ExpCalApplyButton.Content = "적용 성공";
                }
                else
                {
                    ExpCalApplyButton.Background = Application.Current.Resources["RedBrush"] as Brush;
                    ExpCalApplyButton.Content = "적용 실패";
                }
                await Task.Delay(1500);
                ExpCalApplyButton.Background = Application.Current.Resources["TransparentBrush"] as Brush;
                ExpCalApplyButton.Content = "적용";
            });
        }

        /// <summary>
        /// 아무 곳이나 클릭
        /// (정렬/필터링 메뉴 닫기)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewMainWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.FilterToggleButton.IsChecked == true && insideFilterGrid == false)
            {
                this.FilterToggleButton.IsChecked = false;
                this.FilterToggleButton.Background = Application.Current.Resources["TransparentBrush"] as Brush;
                this.FilterToggleButton_Dropdown.Visibility = Visibility.Collapsed;
            }
            if (this.SortToggleButton.IsChecked == true && insideSortGrid == false)
            {
                this.SortToggleButton.IsChecked = false;
                this.SortToggleButton.Background = Application.Current.Resources["TransparentBrush"] as Brush;
                this.SortToggleButton_Dropdown.Visibility = Visibility.Collapsed;
            }
            if (this.ExpCalToggleButton.IsChecked == true && insideExpCalGrid == false)
            {
                this.ExpCalToggleButton.IsChecked = false;
                this.ExpCalToggleButton.Background = Application.Current.Resources["TransparentBrush"] as Brush;
                this.ExpCalToggleButton_Dropdown.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 필터링 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        bool isClickCheckBox = false;
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as ToggleButton).Name;
            log.Debug("click {0}", name);
            //log.Info("click togglebutton");
            if (isClickCheckBox)
            {
                isClickCheckBox = false;
                return;
            }

            ToggleButton filterToggleButton;
            for (int i = 0; i < GroupIdx.Length; i++)
            {
                filterToggleButton = this.FindName(string.Format("{0}FilterToggleButton", GroupNms[i])) as ToggleButton;
                if (filterToggleButton == null) 
                    log.Debug("{0}FilterToggleButton is null", GroupNms[i]);
                filterToggleButton.IsChecked = false;
            }

            int idx = 0;
            switch (name)
            {
                case "DispatchedEchelonFilterToggleButton":
                    idx = 0;
                    break;
                case "ProduceDollFilterToggleButton":
                    idx = 1;
                    break;
                case "ProduceEquipFilterToggleButton":
                    idx = 2;
                    break;
                case "SkillTrainFilterToggleButton":
                    idx = 3;
                    break;
                case "DataAnalysisFilterToggleButton":
                    idx = 4;
                    break;
                case "RestoreDollFilterToggleButton":
                    idx = 5;
                    break;
                case "ExploreFilterToggleButton":
                    idx = 6;
                    break;
            }

            int checkedCount = 0;
            foreach (KeyValuePair<int, string> item in GroupNms)
            {
                if (Config.Dashboard.filter[item.Key] == true)
                {
                    if (item.Key == idx)
                        continue;
                    checkedCount++;
                }
            }

            foreach (KeyValuePair<int, string> item in GroupNms)
            {
                if (item.Key == idx)
                {
                    Config.Dashboard.filter[item.Key] = true;
                }
                else
                {
                    if (checkedCount > 0)
                    {
                        Config.Dashboard.filter[item.Key] = false;
                    }
                    else
                    {
                        Config.Dashboard.filter[item.Key] = true;
                    }
                }

                Grid groupBox = dashboardView.FindName(string.Format("{0}_GroupBox", GroupNms[item.Key])) as Grid;
                CheckBox checkBox = this.FindName(string.Format("{0}FilterCheckBox", GroupNms[item.Key])) as CheckBox;

                groupBox.IsEnabled = Config.Dashboard.filter[item.Key];
                checkBox.IsChecked = Config.Dashboard.filter[item.Key];
            }

            dashboardView.CheckAll();
        }

        /// <summary>
        /// 필터링 체크박스 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterCheckBox_Click(object sender, RoutedEventArgs e)
        {
            isClickCheckBox = true;

            string name = (sender as ToggleButton).Name;
            int idx = 0;
            Object data = new DispatchedEchleonTemplate();
            switch (name)
            {
                case "DispatchEchelonFilterCheckBox":
                    idx = 0;
                    data = new DispatchedEchleonTemplate();
                    break;
                case "ProduceDollFilterCheckBox":
                    idx = 1;
                    data = new ProduceDollTemplate();
                    break;
                case "ProduceEquipFilterCheckBox":
                    idx = 2;
                    data = new ProduceEquipTemplate();
                    break;
                case "SkillTrainFilterCheckBox":
                    idx = 3;
                    data = new SkillTrainTemplate();
                    break;
                case "DataAnalysisFilterCheckBox":
                    idx = 4;
                    data = new DataAnalysisTemplate();
                    break;
                case "RestoreDollFilterCheckBox":
                    idx = 5;
                    data = new RestoreDollTemplate();
                    break;
                case "ExploreFilterCheckBox":
                    idx = 6;
                    data = new ExploreTemplate();
                    break;
            }

            Config.Dashboard.filter[idx] = Config.Dashboard.filter[idx] == true ? false : true;

            Grid groupBox = dashboardView.FindName(string.Format("{0}_GroupBox", GroupNms[idx])) as Grid;
            CheckBox checkBox = this.FindName(string.Format("{0}FilterCheckBox", GroupNms[idx])) as CheckBox;

            groupBox.IsEnabled = Config.Dashboard.filter[idx];
            checkBox.IsChecked = Config.Dashboard.filter[idx];

            dashboardView.Check(data);
        }

        /// <summary>
        /// 정렬 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortToggleButton_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as ToggleButton).Name;
            switch (name)
            {
                case "SlotSortToggleButton":
                    this.sortTypeDashborad = "slot";
                    break;
                case "RemainTimeSortToggleButton":
                    this.sortTypeDashborad = "remainTime";
                    break;
            }
            
        }

        /// <summary>
        /// 정렬 방식
        /// </summary>
        private string _sortTypeDashboard = "slot";
        public string sortTypeDashborad
        {
            get
            {
                return _sortTypeDashboard;
            }
            set
            {
                this.SlotSortToggleButton.IsChecked = false;
                this.RemainTimeSortToggleButton.IsChecked = false;

                switch (value)
                {
                    case "slot":
                        Config.Dashboard.sort = "slot";
                        this.SlotSortToggleButton.IsChecked = true;
                        break;
                    case "remainTime":
                        Config.Dashboard.sort = "remainTime";
                        this.RemainTimeSortToggleButton.IsChecked = true;
                        break;
                }
                dashboardView.SortAll();
            }
        }

        /// <summary>
        /// 그룹 박스 일괄 펴기/접기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpandCollapseButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                switch (this.CurrentMenu)
                {
                    case Menus.DASHBOARD:
                        dashboardView.ExpandAllGroup(dashboardView.isCollapsibleGroup);
                        CheckExpandCollapseButtonStatus();
                        break;
                    case Menus.ECHELON:
                        echelonView.ExpandAllGroup(echelonView.isCollapsibleGroup);
                        CheckExpandCollapseButtonStatus();
                        break;
                    case Menus.QUEST:
                        questView.ExpandAllGroup(questView.isCollapsibleGroup);
                        CheckExpandCollapseButtonStatus();
                        break;
                    case Menus.PROXY:
                        proxyView.ExpandAllGroupBox(proxyView.isCollapsibleGroup);
                        CheckExpandCollapseButtonStatus();
                        break;
                    case Menus.SETTING_ALARM:
                        settingAlarmView.ExpandAllGroupBox(settingAlarmView.isCollapsibleGroup);
                        CheckExpandCollapseButtonStatus();
                        break;
                    case Menus.SETTING:
                        settingView.ExpandAllGroupBox(settingView.isCollapsibleGroup);
                        CheckExpandCollapseButtonStatus();
                        break;
                }
            });
        }

        /// <summary>
        /// 그룹 박스 일괄 펴기/접기 상태 확인
        /// </summary>
        public void CheckExpandCollapseButtonStatus()
        {
            Dispatcher.Invoke(() =>
            {
                switch (this.CurrentMenu)
                {
                    case Menus.DASHBOARD:
                        this.ExpandCollapseButtonIcon.Kind = dashboardView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                        break;
                    case Menus.ECHELON:
                        this.ExpandCollapseButtonIcon.Kind = echelonView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                        break;
                    case Menus.QUEST:
                        this.ExpandCollapseButtonIcon.Kind = questView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                        break;
                    case Menus.PROXY:
                        this.ExpandCollapseButtonIcon.Kind = proxyView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                        break;
                    case Menus.SETTING_ALARM:
                        this.ExpandCollapseButtonIcon.Kind = settingAlarmView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                        break;
                    case Menus.SETTING:
                        this.ExpandCollapseButtonIcon.Kind = settingView.isCollapsibleGroup == true ? PackIconMaterialKind.UnfoldLessHorizontal : PackIconMaterialKind.UnfoldMoreHorizontal;
                        break;
                }
            });
        }

        #endregion

        /// <summary>
        /// 메인화면
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            view = this;

            this.DataContext = this;

            // 트레이 아이콘
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            notifyIcon.Visible = true;

            // 트레이 아이콘 더블 클릭
            notifyIcon.DoubleClick += delegate (object sender, EventArgs e)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.ShowInTaskbar = true;
            };

            // 항상 위
            if (Config.Window.alwaysOnTop)
            {
                this.Topmost = true;
                subView.Topmost = true;
            }

            // 푸터 열림 여부
            this.expandFooter = Config.Footer.expand;

            // 정렬방식
            this.sortTypeDashborad = Config.Dashboard.sort;

            // 타이머 셋팅
            timer = new Timer(Tick, null, 0, 1000);
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            //TimeUtil.testSec += TimeUtil.MINUTE * 10;

            //UserData.CombatSimulation.recoverTime -= TimeUtil.MINUTE * 10;

            //UserData.GlobalExp.exp++;

            GFPacket login = new GFPacket()
            {
                req_id = 0,
                uri = "Index/getUidTianxiaQueue",
                body = "#fmYJ0M3gZjZXDaylMpIPnKjLsWWnQLzIaDznarYZD3QtVc7Ppu8SWtmgcXrcP2FJzaPgQFG5JxxmEjtU1KMPXbIg2LpMczrTXD6y9bR0qvj5bROjUMGtiJsL0KgWzywf98gdIQ16PlweKD1NhPvVRIJJNL+N29pdWK5skJGvt2wh3C3WHWiSoilckmXI7grXqsyyhZxlX9ldIA4ulEaSrGbZtIbbp0K689dC8k7lnTXAW+pCJUmZKOWdcuZ32ulGdU5TEwOgnVRSN4Ryts4bYa6I/Plo+usVql1B43JdEIBcvI1yxPhi/MnFX2ltAkqZBJPvtehaUoYhsYTXb7LtS9FvtIKUo6Godwy7IKNYGozamrTK/NNTaw",
            };
            Transaction.PacketProcess.Index.GetUid(login);
            GFPacket userinfo = new GFPacket()
            {
                req_id = 1,
                uri = "Index/index",
                body = "#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",
            };
            Transaction.PacketProcess.Index.GetIndex(userinfo);

            //GFPacket userinfo = new GFPacket()
            //{
            //    req_id = 1,
            //    uri = "Index/index",
            //    body = File.ReadAllText("test.json"),
            //};
            //Transaction.PacketProcess.Index.GetIndex(userinfo);

            //// {"daily":{"fix":35,"coin_mission":5,"develop_equip":5,"develop_gun":5,"squad_data_analyse":10,"mission":9,"operation":31,"id":"34577","user_id":"343650","end_time":"1572620400","eat":"4","upgrade":"0","win_robot":"0","win_person":"0","win_boss":"0","win_armorrobot":"0","win_armorperson":"0","eat_equip":"4","from_friend_build_coin":"10","borrow_friend_team":"3"},"weekly":{"id":"34577","user_id":"343650","end_time":"1572793200","fix":"103","win_robot":"213","win_person":"204","win_boss":"10","win_armorrobot":"208","win_armorperson":"201","operation":"50","s_win":"100","eat":"20","develop_gun":"20","upgrade":"5","coin_mission":"20","develop_equip":"20","special_develop_gun":"0","adjust_equip":"0","eat_equip":"20","special_develop_equip":"3","adjust_fairy":"0","eat_fairy":"1","squad_data_analyse":"20","squad_eat_chip":"35"},"career":{"upgrade":1755,"upgrade_fairy":204,"mission_duplicate_type3":3093,"develop_equip":5881,"develop_gun":9207,"squad_base_data_analyse":5928,"squad_eat_chip_num":6836,"auto_mission_1":"2167","operation_5":"4","mission_emergency_1_1_1":"4","mission_emergency_3_1_1":"125","mission_emergency_3_2_1":"1","mission_emergency_3_3_1":"2","mission_emergency_3_4_1":"5","mission_emergency_3_5_1":"1","mission_duplicate_type1":"202","mission_duplicate_type2":"87","trial_mission":"99","gun5_into_team":"63777","gun_equip_type_4":"1052","gun5_into_team2":"1514","combine_gun":"1019","retire_gun":"147006","retire_equip":"27179","combine_gun_type_1":"258","combine_gun_type_2":"165","combine_gun_type_3":"176","combine_gun_type_4":"209","combine_gun_type_5":"130","gasha_count":"6788","open_gift":"49298","dorm_change":"1111","friend_visit":"47081","friend_praise":"7998","friend_card_change":"106","friend_headpic_change":"39","eat_equip":"21471","adjust_equip":"2000","adjust_equip_attr_100":"1146","reinforce_team2":"22008","special_develop_gun":"133","eat_gun":"8473","gun_equip_type_5":"1207","team_from_1":"1816","team_from_2":"78","team_from_3":"19","team_from_4":"408","team_from_5":"44","mission_emergency_1_2_1":"1","mission_emergency_1_3_1":"6","mission_emergency_1_4_1":"1","mission_emergency_1_5_1":"1","mission_emergency_1_6_1":"3","mission_emergency_1_7_1":"2","mission_emergency_1_8_1":"2","develop_fairy":"1342","eat_fairy":"1017","adjust_fairy":"212","retire_fairy":"0","establish_up_type_301":"10","establish_up_type_302":"10","establish_up_type_303":"10","establish_up_type_304":"10","friend_apply":"84","up_mod1":"33","up_mod2":"30","up_mod3":"29","upgrade2":"263","sun_friend_team_into":"26","night_friend_team_into":"18","squad_senior_data_analyse":"2676","squad_get_chip":"7547","squad_get_secret_data":"1207","squad_get_type1_unit":"6","squad_up_rank":"20","squad_rank_max":"5","squad_level_max":"5","squad_train_time":"1967","squad_up_skill_times":"123","squad_up_advanced_times":"36","squad_change_data_times":"752","squad_chip_retire_times":"80","uniform_get_color":"0","activation_uniform_skill":"2","explore_auto_team":"1","explore_first_times":"148","explore_replay":"4","explore_get_prize":"453","explore_change_item":"72","change_gender":"2","mission_emergency_0_1_3":0,"mission_emergency_1_1_2":0,"mission_emergency_3_1_2":0},"static_career_quest":[{"id":"1","unlock_lv":"3","unlock_ids":"","unlock_label":"","type":"auto_mission_1","count":"1","prize_id":"701","title":"career_quest-10000001","content":"career_quest-20000001","unlock_course":"","new_type":"0"},{"id":"2","unlock_lv":"","unlock_ids":"auto_mission_1:1","unlock_label":"","type":"operation_5","count":"1","prize_id":"702","title":"career_quest-10000002","content":"career_quest-20000002","unlock_course":"","new_type":"1"},{"id":"3","unlock_lv":"","unlock_ids":"operation_5:1","unlock_label":"","type":"reinforce_team2","count":"1","prize_id":"703","title":"career_quest-10000003","content":"career_quest-20000003","unlock_course":"","new_type":"1"},{"id":"4","unlock_lv":"","unlock_ids":"","unlock_label":"mission:15","type":"mission_emergency_1_1_1","count":"1","prize_id":"704","title":"career_quest-10000004","content":"career_quest-20000004","unlock_course":"","new_type":"1"},{"id":"5","unlock_lv":"","unlock_ids":"mission_emergency_1_1_1:1","unlock_label":"","type":"mission_emergency_1_2_1","count":"1","prize_id":"705","title":"career_quest-10000005","content":"career_quest-20000005","unlock_course":"","new_type":"1"},{"id":"6","unlock_lv":"","unlock_ids":"mission_emergency_1_2_1:1","unlock_label":"","type":"mission_emergency_1_3_1","count":"1","prize_id":"706","title":"career_quest-10000006","content":"career_quest-20000006","unlock_course":"","new_type":"1"},{"id":"7","unlock_lv":"","unlock_ids":"mission_emergency_1_3_1:1","unlock_label":"","type":"mission_emergency_1_4_1","count":"1","prize_id":"707","title":"career_quest-10000007","content":"career_quest-20000007","unlock_course":"","new_type":"1"},{"id":"8","unlock_lv":"","unlock_ids":"mission_emergency_1_4_1:1","unlock_label":"","type":"mission_emergency_1_5_1","count":"1","prize_id":"708","title":"career_quest-10000008","content":"career_quest-20000008","unlock_course":"","new_type":"1"},{"id":"9","unlock_lv":"","unlock_ids":"mission_emergency_1_5_1:1","unlock_label":"","type":"mission_emergency_1_6_1","count":"1","prize_id":"709","title":"career_quest-10000009","content":"career_quest-20000009","unlock_course":"","new_type":"1"},{"id":"10","unlock_lv":"","unlock_ids":"mission_emergency_1_6_1:1","unlock_label":"","type":"mission_emergency_1_7_1","count":"1","prize_id":"710","title":"career_quest-10000010","content":"career_quest-20000010","unlock_course":"","new_type":"1"},{"id":"11","unlock_lv":"","unlock_ids":"mission_emergency_1_7_1:1","unlock_label":"","type":"mission_emergency_1_8_1","count":"1","prize_id":"711","title":"career_quest-10000011","content":"career_quest-20000011","unlock_course":"","new_type":"1"},{"id":"12","unlock_lv":"","unlock_ids":"","unlock_label":"mission:90001","type":"mission_emergency_3_1_1","count":"1","prize_id":"712","title":"career_quest-10000012","content":"career_quest-20000012","unlock_course":"","new_type":"1"},{"id":"13","unlock_lv":"","unlock_ids":"mission_emergency_3_1_1:1","unlock_label":"","type":"mission_emergency_3_2_1","count":"1","prize_id":"713","title":"career_quest-10000013","content":"career_quest-20000013","unlock_course":"","new_type":"1"},{"id":"14","unlock_lv":"","unlock_ids":"mission_emergency_3_2_1:1","unlock_label":"","type":"mission_emergency_3_3_1","count":"1","prize_id":"714","title":"career_quest-10000014","content":"career_quest-20000014","unlock_course":"","new_type":"1"},{"id":"15","unlock_lv":"","unlock_ids":"mission_emergency_3_3_1:1","unlock_label":"","type":"mission_emergency_3_4_1","count":"1","prize_id":"715","title":"career_quest-10000015","content":"career_quest-20000015","unlock_course":"","new_type":"1"},{"id":"16","unlock_lv":"","unlock_ids":"mission_emergency_3_4_1:1","unlock_label":"","type":"mission_emergency_3_5_1","count":"1","prize_id":"716","title":"career_quest-10000016","content":"career_quest-20000016","unlock_course":"","new_type":"1"},{"id":"17","unlock_lv":"12","unlock_ids":"","unlock_label":"","type":"mission_duplicate_type1","count":"1","prize_id":"717","title":"career_quest-10000017","content":"career_quest-20000017","unlock_course":"","new_type":"1"},{"id":"18","unlock_lv":"12","unlock_ids":"","unlock_label":"","type":"mission_duplicate_type2","count":"1","prize_id":"718","title":"career_quest-10000018","content":"career_quest-20000018","unlock_course":"","new_type":"1"},{"id":"19","unlock_lv":"12","unlock_ids":"","unlock_label":"","type":"mission_duplicate_type3","count":"1","prize_id":"719","title":"career_quest-10000019","content":"career_quest-20000019","unlock_course":"","new_type":"1"},{"id":"20","unlock_lv":"","unlock_ids":"","unlock_label":"mission:90008","type":"trial_mission","count":"1","prize_id":"720","title":"career_quest-10000020","content":"career_quest-20000020","unlock_course":"","new_type":"1"},{"id":"21","unlock_lv":"3","unlock_ids":"","unlock_label":"","type":"gun5_into_team","count":"1","prize_id":"721","title":"career_quest-10000021","content":"career_quest-20000021","unlock_course":"","new_type":"1"},{"id":"22","unlock_lv":"","unlock_ids":"mission_emergency_3_1_1:1","unlock_label":"","type":"gun_equip_type_4","count":"1","prize_id":"722","title":"career_quest-10000022","content":"career_quest-20000022","unlock_course":"","new_type":"1"},{"id":"23","unlock_lv":"","unlock_ids":"mission_emergency_3_1_1:1","unlock_label":"","type":"gun_equip_type_5","count":"1","prize_id":"723","title":"career_quest-10000023","content":"career_quest-20000023","unlock_course":"","new_type":"1"},{"id":"24","unlock_lv":"","unlock_ids":"gun5_into_team:1","unlock_label":"","type":"gun5_into_team2","count":"1","prize_id":"724","title":"career_quest-10000024","content":"career_quest-20000024","unlock_course":"","new_type":"1"},{"id":"25","unlock_lv":"","unlock_ids":"gun5_into_team2:1","unlock_label":"","type":"team_from_1","count":"1","prize_id":"725","title":"career_quest-10000025","content":"career_quest-20000025","unlock_course":"","new_type":"1"},{"id":"26","unlock_lv":"","unlock_ids":"team_from_1:1","unlock_label":"","type":"team_from_2","count":"1","prize_id":"726","title":"career_quest-10000026","content":"career_quest-20000026","unlock_course":"","new_type":"1"},{"id":"27","unlock_lv":"","unlock_ids":"team_from_2:1","unlock_label":"","type":"team_from_3","count":"1","prize_id":"727","title":"career_quest-10000027","content":"career_quest-20000027","unlock_course":"","new_type":"1"},{"id":"28","unlock_lv":"","unlock_ids":"team_from_3:1","unlock_label":"","type":"team_from_4","count":"1","prize_id":"728","title":"career_quest-10000028","content":"career_quest-20000028","unlock_course":"","new_type":"1"},{"id":"29","unlock_lv":"","unlock_ids":"team_from_4:1","unlock_label":"","type":"team_from_5","count":"1","prize_id":"729","title":"career_quest-10000029","content":"career_quest-20000029","unlock_course":"","new_type":"1"},{"id":"50","unlock_lv":"3","unlock_ids":"","unlock_label":"","type":"develop_gun","count":"5","prize_id":"730","title":"career_quest-10000050","content":"career_quest-20000050","unlock_course":"","new_type":"1"},{"id":"51","unlock_lv":"","unlock_ids":"develop_gun:1","unlock_label":"","type":"combine_gun","count":"1","prize_id":"731","title":"career_quest-10000051","content":"career_quest-20000051","unlock_course":"","new_type":"1"},{"id":"52","unlock_lv":"","unlock_ids":"develop_gun:1","unlock_label":"","type":"eat_gun","count":"5","prize_id":"732","title":"career_quest-10000052","content":"career_quest-20000052","unlock_course":"","new_type":"1"},{"id":"53","unlock_lv":"","unlock_ids":"eat_gun:1","unlock_label":"","type":"retire_gun","count":"5","prize_id":"733","title":"career_quest-10000053","content":"career_quest-20000053","unlock_course":"","new_type":"1"},{"id":"54","unlock_lv":"","unlock_ids":"","unlock_label":"mission:20","type":"develop_equip","count":"5","prize_id":"734","title":"career_quest-10000054","content":"career_quest-20000054","unlock_course":"","new_type":"1"},{"id":"55","unlock_lv":"","unlock_ids":"","unlock_label":"mission:20","type":"retire_equip","count":"5","prize_id":"735","title":"career_quest-10000055","content":"career_quest-20000055","unlock_course":"","new_type":"1"},{"id":"56","unlock_lv":"","unlock_ids":"combine_gun:1","unlock_label":"","type":"combine_gun_type_4","count":"1","prize_id":"736","title":"career_quest-10000056","content":"career_quest-20000056","unlock_course":"","new_type":"1"},{"id":"57","unlock_lv":"","unlock_ids":"combine_gun:1","unlock_label":"","type":"combine_gun_type_2","count":"1","prize_id":"737","title":"career_quest-10000057","content":"career_quest-20000057","unlock_course":"","new_type":"1"},{"id":"58","unlock_lv":"","unlock_ids":"combine_gun:1","unlock_label":"","type":"combine_gun_type_5","count":"1","prize_id":"738","title":"career_quest-10000058","content":"career_quest-20000058","unlock_course":"","new_type":"1"},{"id":"59","unlock_lv":"","unlock_ids":"combine_gun:1","unlock_label":"","type":"combine_gun_type_3","count":"1","prize_id":"739","title":"career_quest-10000059","content":"career_quest-20000059","unlock_course":"","new_type":"1"},{"id":"60","unlock_lv":"","unlock_ids":"combine_gun:1","unlock_label":"","type":"combine_gun_type_1","count":"1","prize_id":"740","title":"career_quest-10000060","content":"career_quest-20000060","unlock_course":"","new_type":"1"},{"id":"61","unlock_lv":"","unlock_ids":"","unlock_label":"bestrank1:30","type":"special_develop_gun","count":"1","prize_id":"741","title":"career_quest-10000061","content":"career_quest-20000061","unlock_course":"","new_type":"1"},{"id":"100","unlock_lv":"3","unlock_ids":"","unlock_label":"","type":"gasha_count","count":"1","prize_id":"742","title":"career_quest-10000100","content":"career_quest-20000100","unlock_course":"","new_type":"1"},{"id":"101","unlock_lv":"3","unlock_ids":"","unlock_label":"","type":"open_gift","count":"5","prize_id":"743","title":"career_quest-10000101","content":"career_quest-20000101","unlock_course":"","new_type":"1"},{"id":"102","unlock_lv":"3","unlock_ids":"","unlock_label":"","type":"dorm_change","count":"1","prize_id":"744","title":"career_quest-10000102","content":"career_quest-20000102","unlock_course":"","new_type":"1"},{"id":"103","unlock_lv":"3","unlock_ids":"","unlock_label":"","type":"friend_visit","count":"5","prize_id":"745","title":"career_quest-10000103","content":"career_quest-20000103","unlock_course":"","new_type":"1"},{"id":"104","unlock_lv":"3","unlock_ids":"friend_visit:1","unlock_label":"","type":"friend_praise","count":"5","prize_id":"746","title":"career_quest-10000104","content":"career_quest-20000104","unlock_course":"","new_type":"1"},{"id":"150","unlock_lv":"","unlock_ids":"","unlock_label":"mission:7","type":"friend_apply","count":"1","prize_id":"758","title":"career_quest-10000150","content":"career_quest-20000150","unlock_course":"","new_type":"1"},{"id":"151","unlock_lv":"","unlock_ids":"","unlock_label":"mission:7","type":"friend_card_change","count":"1","prize_id":"759","title":"career_quest-10000151","content":"career_quest-20000151","unlock_course":"","new_type":"1"},{"id":"152","unlock_lv":"","unlock_ids":"","unlock_label":"mission:7","type":"friend_headpic_change","count":"1","prize_id":"760","title":"career_quest-10000152","content":"career_quest-20000152","unlock_course":"","new_type":"1"},{"id":"153","unlock_lv":"12","unlock_ids":"","unlock_label":"","type":"upgrade","count":"5","prize_id":"761","title":"career_quest-10000153","content":"career_quest-20000153","unlock_course":"","new_type":"1"},{"id":"154","unlock_lv":"","unlock_ids":"","unlock_label":"mission:90004","type":"eat_equip","count":"5","prize_id":"762","title":"career_quest-10000154","content":"career_quest-20000154","unlock_course":"","new_type":"1"},{"id":"155","unlock_lv":"","unlock_ids":"","unlock_label":"mission:90008","type":"adjust_equip","count":"5","prize_id":"763","title":"career_quest-10000155","content":"career_quest-20000155","unlock_course":"","new_type":"1"},{"id":"156","unlock_lv":"","unlock_ids":"adjust_equip:1","unlock_label":"","type":"adjust_equip_attr_100","count":"1","prize_id":"764","title":"career_quest-10000156","content":"career_quest-20000156","unlock_course":"","new_type":"1"},{"id":"157","unlock_lv":"","unlock_ids":"","unlock_label":"bestrank1:60","type":"develop_fairy","count":"1","prize_id":"765","title":"career_quest-10000157","content":"career_quest-20000157","unlock_course":"80","new_type":"1"},{"id":"158","unlock_lv":"","unlock_ids":"","unlock_label":"bestrank1:60","type":"eat_fairy","count":"5","prize_id":"766","title":"career_quest-10000158","content":"career_quest-20000158","unlock_course":"86,87,88","new_type":"1"},{"id":"159","unlock_lv":"","unlock_ids":"","unlock_label":"bestrank1:60","type":"upgrade_fairy","count":"5","prize_id":"767","title":"career_quest-10000159","content":"career_quest-20000159","unlock_course":"86,87,88","new_type":"1"},{"id":"160","unlock_lv":"","unlock_ids":"","unlock_label":"bestrank1:60","type":"adjust_fairy","count":"5","prize_id":"768","title":"career_quest-10000160","content":"career_quest-20000160","unlock_course":"86,87,88","new_type":"1"},{"id":"161","unlock_lv":"","unlock_ids":"","unlock_label":"bestrank1:60","type":"retire_fairy","count":"5","prize_id":"769","title":"career_quest-10000161","content":"career_quest-20000161","unlock_course":"86,87,88","new_type":"1"},{"id":"162","unlock_lv":"","unlock_ids":"","unlock_label":"bestrank1:60","type":"establish_up_type_301","count":"1","prize_id":"770","title":"career_quest-10000162","content":"career_quest-20000162","unlock_course":"","new_type":"1"},{"id":"163","unlock_lv":"","unlock_ids":"","unlock_label":"bestrank1:60","type":"establish_up_type_302","count":"1","prize_id":"771","title":"career_quest-10000163","content":"career_quest-20000163","unlock_course":"","new_type":"1"},{"id":"164","unlock_lv":"","unlock_ids":"","unlock_label":"bestrank1:60","type":"establish_up_type_303","count":"1","prize_id":"772","title":"career_quest-10000164","content":"career_quest-20000164","unlock_course":"","new_type":"1"},{"id":"165","unlock_lv":"","unlock_ids":"","unlock_label":"bestrank1:60","type":"establish_up_type_304","count":"1","prize_id":"773","title":"career_quest-10000165","content":"career_quest-20000165","unlock_course":"","new_type":"1"},{"id":"166","unlock_lv":"60","unlock_ids":"","unlock_label":"","type":"up_mod1","count":"1","prize_id":"774","title":"career_quest-10000166","content":"career_quest-20000166","unlock_course":"97","new_type":"1"},{"id":"167","unlock_lv":"","unlock_ids":"up_mod1:1","unlock_label":"","type":"up_mod2","count":"1","prize_id":"775","title":"career_quest-10000167","content":"career_quest-20000167","unlock_course":"97","new_type":"1"},{"id":"168","unlock_lv":"","unlock_ids":"up_mod2:1","unlock_label":"","type":"up_mod3","count":"1","prize_id":"776","title":"career_quest-10000168","content":"career_quest-20000168","unlock_course":"97","new_type":"1"},{"id":"169","unlock_lv":"","unlock_ids":"up_mod2:1","unlock_label":"","type":"upgrade2","count":"1","prize_id":"777","title":"career_quest-10000169","content":"career_quest-20000169","unlock_course":"","new_type":"1"},{"id":"170","unlock_lv":"","unlock_ids":"","unlock_label":"","type":"sun_friend_team_into","count":"1","prize_id":"778","title":"career_quest-10000170","content":"career_quest-20000170","unlock_course":"100","new_type":"1"},{"id":"171","unlock_lv":"","unlock_ids":"","unlock_label":"","type":"night_friend_team_into","count":"1","prize_id":"779","title":"career_quest-10000171","content":"career_quest-20000171","unlock_course":"100","new_type":"1"},{"id":"172","unlock_lv":"","unlock_ids":"","unlock_label":"","type":"squad_base_data_analyse","count":"1","prize_id":"780","title":"career_quest-10000172","content":"career_quest-20000172","unlock_course":"","new_type":"1"},{"id":"173","unlock_lv":"","unlock_ids":"","unlock_label":"","type":"squad_senior_data_analyse","count":"1","prize_id":"781","title":"career_quest-10000173","content":"career_quest-20000173","unlock_course":"","new_type":"1"},{"id":"174","unlock_lv":"","unlock_ids":"","unlock_label":"","type":"squad_get_chip","count":"1","prize_id":"782","title":"career_quest-10000174","content":"career_quest-20000174","unlock_course":"","new_type":"1"},{"id":"175","unlock_lv":"","unlock_ids":"","unlock_label":"","type":"squad_get_secret_data","count":"1","prize_id":"783","title":"career_quest-10000175","content":"career_quest-20000175","unlock_course":"","new_type":"1"},{"id":"176","unlock_lv":"","unlock_ids":"squad_get_secret_data:1","unlock_label":"","type":"squad_get_type1_unit","count":"1","prize_id":"784","title":"career_quest-10000176","content":"career_quest-20000176","unlock_course":"","new_type":"1"},{"id":"177","unlock_lv":"","unlock_ids":"squad_get_secret_data:1","unlock_label":"","type":"squad_up_rank","count":"1","prize_id":"785","title":"career_quest-10000177","content":"career_quest-20000177","unlock_course":"","new_type":"1"},{"id":"178","unlock_lv":"","unlock_ids":"squad_get_secret_data:1","unlock_label":"","type":"squad_rank_max","count":"1","prize_id":"786","title":"career_quest-10000178","content":"career_quest-20000178","unlock_course":"","new_type":"1"},{"id":"179","unlock_lv":"","unlock_ids":"squad_get_type1_unit:1","unlock_label":"","type":"squad_train_time","count":"1","prize_id":"787","title":"career_quest-10000179","content":"career_quest-20000179","unlock_course":"","new_type":"1"},{"id":"180","unlock_lv":"","unlock_ids":"squad_get_type1_unit:1","unlock_label":"","type":"squad_level_max","count":"1","prize_id":"788","title":"career_quest-10000180","content":"career_quest-20000180","unlock_course":"","new_type":"1"},{"id":"181","unlock_lv":"","unlock_ids":"squad_get_type1_unit:1","unlock_label":"","type":"squad_up_skill_times","count":"1","prize_id":"789","title":"career_quest-10000181","content":"career_quest-20000181","unlock_course":"","new_type":"1"},{"id":"182","unlock_lv":"","unlock_ids":"squad_get_type1_unit:1","unlock_label":"","type":"squad_up_advanced_times","count":"1","prize_id":"790","title":"career_quest-10000182","content":"career_quest-20000182","unlock_course":"","new_type":"1"},{"id":"183","unlock_lv":"","unlock_ids":"squad_get_secret_data:1","unlock_label":"","type":"squad_change_data_times","count":"1","prize_id":"791","title":"career_quest-10000183","content":"career_quest-20000183","unlock_course":"","new_type":"1"},{"id":"184","unlock_lv":"","unlock_ids":"squad_get_chip:1","unlock_label":"","type":"squad_chip_retire_times","count":"1","prize_id":"792","title":"career_quest-10000184","content":"career_quest-20000184","unlock_course":"","new_type":"1"},{"id":"185","unlock_lv":"","unlock_ids":"squad_get_chip:1","unlock_label":"","type":"squad_eat_chip_num","count":"1","prize_id":"793","title":"career_quest-10000185","content":"career_quest-20000185","unlock_course":"","new_type":"1"},{"id":"186","unlock_lv":"20","unlock_ids":"","unlock_label":"","type":"explore_auto_team","count":"1","prize_id":"794","title":"career_quest-10000186","content":"career_quest-20000186","unlock_course":"","new_type":"1"},{"id":"187","unlock_lv":"20","unlock_ids":"","unlock_label":"","type":"explore_first_times","count":"1","prize_id":"795","title":"career_quest-10000187","content":"career_quest-20000187","unlock_course":"","new_type":"1"},{"id":"188","unlock_lv":"20","unlock_ids":"","unlock_label":"","type":"explore_replay","count":"1","prize_id":"796","title":"career_quest-10000188","content":"career_quest-20000188","unlock_course":"","new_type":"1"},{"id":"189","unlock_lv":"20","unlock_ids":"","unlock_label":"","type":"explore_get_prize","count":"1","prize_id":"797","title":"career_quest-10000189","content":"career_quest-20000189","unlock_course":"","new_type":"1"},{"id":"190","unlock_lv":"20","unlock_ids":"","unlock_label":"","type":"explore_change_item","count":"1","prize_id":"798","title":"career_quest-10000190","content":"career_quest-20000190","unlock_course":"","new_type":"1"},{"id":"191","unlock_lv":"","unlock_ids":"","unlock_label":"","type":"uniform_get_color","count":"1","prize_id":"799","title":"career_quest-10000191","content":"career_quest-20000191","unlock_course":"130","new_type":"1"},{"id":"192","unlock_lv":"","unlock_ids":"","unlock_label":"","type":"activation_uniform_skill","count":"1","prize_id":"800","title":"career_quest-10000192","content":"career_quest-20000192","unlock_course":"130","new_type":"1"},{"id":"193","unlock_lv":"","unlock_ids":"","unlock_label":"","type":"change_gender","count":"1","prize_id":"11002","title":"career_quest-10000193","content":"career_quest-20000193","unlock_course":"130","new_type":"1"},{"id":"194","unlock_lv":"","unlock_ids":"","unlock_label":"mission:5","type":"mission_emergency_0_1_3","count":"1","prize_id":"11003","title":"career_quest-10000194","content":"career_quest-20000194","unlock_course":"","new_type":"2"},{"id":"203","unlock_lv":"","unlock_ids":"","unlock_label":"mission:15","type":"mission_emergency_1_1_2","count":"1","prize_id":"11012","title":"career_quest-10000203","content":"career_quest-20000203","unlock_course":"","new_type":"2"},{"id":"232","unlock_lv":"","unlock_ids":"","unlock_label":"mission:90001","type":"mission_emergency_3_1_2","count":"1","prize_id":"11041","title":"career_quest-10000232","content":"career_quest-20000232","unlock_course":"","new_type":"2"}]} 
            //GFPacket quest = new GFPacket()
            //{
            //    req_id = 2,
            //    uri = "Index/Quest",
            //    body = "{\"daily\":{\"fix\":35,\"coin_mission\":5,\"develop_equip\":5,\"develop_gun\":5,\"squad_data_analyse\":10,\"mission\":9,\"operation\":31,\"id\":\"34577\",\"user_id\":\"343650\",\"end_time\":\"1572620400\",\"eat\":\"4\",\"upgrade\":\"0\",\"win_robot\":\"0\",\"win_person\":\"0\",\"win_boss\":\"0\",\"win_armorrobot\":\"0\",\"win_armorperson\":\"0\",\"eat_equip\":\"4\",\"from_friend_build_coin\":\"10\",\"borrow_friend_team\":\"3\"},\"weekly\":{\"id\":\"34577\",\"user_id\":\"343650\",\"end_time\":\"1572793200\",\"fix\":\"103\",\"win_robot\":\"213\",\"win_person\":\"204\",\"win_boss\":\"10\",\"win_armorrobot\":\"208\",\"win_armorperson\":\"201\",\"operation\":\"50\",\"s_win\":\"100\",\"eat\":\"20\",\"develop_gun\":\"20\",\"upgrade\":\"5\",\"coin_mission\":\"20\",\"develop_equip\":\"20\",\"special_develop_gun\":\"0\",\"adjust_equip\":\"0\",\"eat_equip\":\"20\",\"special_develop_equip\":\"3\",\"adjust_fairy\":\"0\",\"eat_fairy\":\"1\",\"squad_data_analyse\":\"20\",\"squad_eat_chip\":\"35\"},\"career\":{\"upgrade\":1755,\"upgrade_fairy\":204,\"mission_duplicate_type3\":3093,\"develop_equip\":5881,\"develop_gun\":9207,\"squad_base_data_analyse\":5928,\"squad_eat_chip_num\":6836,\"auto_mission_1\":\"2167\",\"operation_5\":\"4\",\"mission_emergency_1_1_1\":\"4\",\"mission_emergency_3_1_1\":\"125\",\"mission_emergency_3_2_1\":\"1\",\"mission_emergency_3_3_1\":\"2\",\"mission_emergency_3_4_1\":\"5\",\"mission_emergency_3_5_1\":\"1\",\"mission_duplicate_type1\":\"202\",\"mission_duplicate_type2\":\"87\",\"trial_mission\":\"99\",\"gun5_into_team\":\"63777\",\"gun_equip_type_4\":\"1052\",\"gun5_into_team2\":\"1514\",\"combine_gun\":\"1019\",\"retire_gun\":\"147006\",\"retire_equip\":\"27179\",\"combine_gun_type_1\":\"258\",\"combine_gun_type_2\":\"165\",\"combine_gun_type_3\":\"176\",\"combine_gun_type_4\":\"209\",\"combine_gun_type_5\":\"130\",\"gasha_count\":\"6788\",\"open_gift\":\"49298\",\"dorm_change\":\"1111\",\"friend_visit\":\"47081\",\"friend_praise\":\"7998\",\"friend_card_change\":\"106\",\"friend_headpic_change\":\"39\",\"eat_equip\":\"21471\",\"adjust_equip\":\"2000\",\"adjust_equip_attr_100\":\"1146\",\"reinforce_team2\":\"22008\",\"special_develop_gun\":\"133\",\"eat_gun\":\"8473\",\"gun_equip_type_5\":\"1207\",\"team_from_1\":\"1816\",\"team_from_2\":\"78\",\"team_from_3\":\"19\",\"team_from_4\":\"408\",\"team_from_5\":\"44\",\"mission_emergency_1_2_1\":\"1\",\"mission_emergency_1_3_1\":\"6\",\"mission_emergency_1_4_1\":\"1\",\"mission_emergency_1_5_1\":\"1\",\"mission_emergency_1_6_1\":\"3\",\"mission_emergency_1_7_1\":\"2\",\"mission_emergency_1_8_1\":\"2\",\"develop_fairy\":\"1342\",\"eat_fairy\":\"1017\",\"adjust_fairy\":\"212\",\"retire_fairy\":\"0\",\"establish_up_type_301\":\"10\",\"establish_up_type_302\":\"10\",\"establish_up_type_303\":\"10\",\"establish_up_type_304\":\"10\",\"friend_apply\":\"84\",\"up_mod1\":\"33\",\"up_mod2\":\"30\",\"up_mod3\":\"29\",\"upgrade2\":\"263\",\"sun_friend_team_into\":\"26\",\"night_friend_team_into\":\"18\",\"squad_senior_data_analyse\":\"2676\",\"squad_get_chip\":\"7547\",\"squad_get_secret_data\":\"1207\",\"squad_get_type1_unit\":\"6\",\"squad_up_rank\":\"20\",\"squad_rank_max\":\"5\",\"squad_level_max\":\"5\",\"squad_train_time\":\"1967\",\"squad_up_skill_times\":\"123\",\"squad_up_advanced_times\":\"36\",\"squad_change_data_times\":\"752\",\"squad_chip_retire_times\":\"80\",\"uniform_get_color\":\"0\",\"activation_uniform_skill\":\"2\",\"explore_auto_team\":\"1\",\"explore_first_times\":\"148\",\"explore_replay\":\"4\",\"explore_get_prize\":\"453\",\"explore_change_item\":\"72\",\"change_gender\":\"2\",\"mission_emergency_0_1_3\":0,\"mission_emergency_1_1_2\":0,\"mission_emergency_3_1_2\":0},\"static_career_quest\":[{\"id\":\"1\",\"unlock_lv\":\"3\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"auto_mission_1\",\"count\":\"1\",\"prize_id\":\"701\",\"title\":\"career_quest-10000001\",\"content\":\"career_quest-20000001\",\"unlock_course\":\"\",\"new_type\":\"0\"},{\"id\":\"2\",\"unlock_lv\":\"\",\"unlock_ids\":\"auto_mission_1:1\",\"unlock_label\":\"\",\"type\":\"operation_5\",\"count\":\"1\",\"prize_id\":\"702\",\"title\":\"career_quest-10000002\",\"content\":\"career_quest-20000002\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"3\",\"unlock_lv\":\"\",\"unlock_ids\":\"operation_5:1\",\"unlock_label\":\"\",\"type\":\"reinforce_team2\",\"count\":\"1\",\"prize_id\":\"703\",\"title\":\"career_quest-10000003\",\"content\":\"career_quest-20000003\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"4\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:15\",\"type\":\"mission_emergency_1_1_1\",\"count\":\"1\",\"prize_id\":\"704\",\"title\":\"career_quest-10000004\",\"content\":\"career_quest-20000004\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"5\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_1_1_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_1_2_1\",\"count\":\"1\",\"prize_id\":\"705\",\"title\":\"career_quest-10000005\",\"content\":\"career_quest-20000005\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"6\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_1_2_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_1_3_1\",\"count\":\"1\",\"prize_id\":\"706\",\"title\":\"career_quest-10000006\",\"content\":\"career_quest-20000006\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"7\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_1_3_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_1_4_1\",\"count\":\"1\",\"prize_id\":\"707\",\"title\":\"career_quest-10000007\",\"content\":\"career_quest-20000007\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"8\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_1_4_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_1_5_1\",\"count\":\"1\",\"prize_id\":\"708\",\"title\":\"career_quest-10000008\",\"content\":\"career_quest-20000008\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"9\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_1_5_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_1_6_1\",\"count\":\"1\",\"prize_id\":\"709\",\"title\":\"career_quest-10000009\",\"content\":\"career_quest-20000009\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"10\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_1_6_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_1_7_1\",\"count\":\"1\",\"prize_id\":\"710\",\"title\":\"career_quest-10000010\",\"content\":\"career_quest-20000010\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"11\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_1_7_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_1_8_1\",\"count\":\"1\",\"prize_id\":\"711\",\"title\":\"career_quest-10000011\",\"content\":\"career_quest-20000011\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"12\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:90001\",\"type\":\"mission_emergency_3_1_1\",\"count\":\"1\",\"prize_id\":\"712\",\"title\":\"career_quest-10000012\",\"content\":\"career_quest-20000012\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"13\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_3_1_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_3_2_1\",\"count\":\"1\",\"prize_id\":\"713\",\"title\":\"career_quest-10000013\",\"content\":\"career_quest-20000013\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"14\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_3_2_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_3_3_1\",\"count\":\"1\",\"prize_id\":\"714\",\"title\":\"career_quest-10000014\",\"content\":\"career_quest-20000014\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"15\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_3_3_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_3_4_1\",\"count\":\"1\",\"prize_id\":\"715\",\"title\":\"career_quest-10000015\",\"content\":\"career_quest-20000015\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"16\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_3_4_1:1\",\"unlock_label\":\"\",\"type\":\"mission_emergency_3_5_1\",\"count\":\"1\",\"prize_id\":\"716\",\"title\":\"career_quest-10000016\",\"content\":\"career_quest-20000016\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"17\",\"unlock_lv\":\"12\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"mission_duplicate_type1\",\"count\":\"1\",\"prize_id\":\"717\",\"title\":\"career_quest-10000017\",\"content\":\"career_quest-20000017\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"18\",\"unlock_lv\":\"12\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"mission_duplicate_type2\",\"count\":\"1\",\"prize_id\":\"718\",\"title\":\"career_quest-10000018\",\"content\":\"career_quest-20000018\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"19\",\"unlock_lv\":\"12\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"mission_duplicate_type3\",\"count\":\"1\",\"prize_id\":\"719\",\"title\":\"career_quest-10000019\",\"content\":\"career_quest-20000019\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"20\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:90008\",\"type\":\"trial_mission\",\"count\":\"1\",\"prize_id\":\"720\",\"title\":\"career_quest-10000020\",\"content\":\"career_quest-20000020\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"21\",\"unlock_lv\":\"3\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"gun5_into_team\",\"count\":\"1\",\"prize_id\":\"721\",\"title\":\"career_quest-10000021\",\"content\":\"career_quest-20000021\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"22\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_3_1_1:1\",\"unlock_label\":\"\",\"type\":\"gun_equip_type_4\",\"count\":\"1\",\"prize_id\":\"722\",\"title\":\"career_quest-10000022\",\"content\":\"career_quest-20000022\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"23\",\"unlock_lv\":\"\",\"unlock_ids\":\"mission_emergency_3_1_1:1\",\"unlock_label\":\"\",\"type\":\"gun_equip_type_5\",\"count\":\"1\",\"prize_id\":\"723\",\"title\":\"career_quest-10000023\",\"content\":\"career_quest-20000023\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"24\",\"unlock_lv\":\"\",\"unlock_ids\":\"gun5_into_team:1\",\"unlock_label\":\"\",\"type\":\"gun5_into_team2\",\"count\":\"1\",\"prize_id\":\"724\",\"title\":\"career_quest-10000024\",\"content\":\"career_quest-20000024\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"25\",\"unlock_lv\":\"\",\"unlock_ids\":\"gun5_into_team2:1\",\"unlock_label\":\"\",\"type\":\"team_from_1\",\"count\":\"1\",\"prize_id\":\"725\",\"title\":\"career_quest-10000025\",\"content\":\"career_quest-20000025\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"26\",\"unlock_lv\":\"\",\"unlock_ids\":\"team_from_1:1\",\"unlock_label\":\"\",\"type\":\"team_from_2\",\"count\":\"1\",\"prize_id\":\"726\",\"title\":\"career_quest-10000026\",\"content\":\"career_quest-20000026\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"27\",\"unlock_lv\":\"\",\"unlock_ids\":\"team_from_2:1\",\"unlock_label\":\"\",\"type\":\"team_from_3\",\"count\":\"1\",\"prize_id\":\"727\",\"title\":\"career_quest-10000027\",\"content\":\"career_quest-20000027\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"28\",\"unlock_lv\":\"\",\"unlock_ids\":\"team_from_3:1\",\"unlock_label\":\"\",\"type\":\"team_from_4\",\"count\":\"1\",\"prize_id\":\"728\",\"title\":\"career_quest-10000028\",\"content\":\"career_quest-20000028\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"29\",\"unlock_lv\":\"\",\"unlock_ids\":\"team_from_4:1\",\"unlock_label\":\"\",\"type\":\"team_from_5\",\"count\":\"1\",\"prize_id\":\"729\",\"title\":\"career_quest-10000029\",\"content\":\"career_quest-20000029\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"50\",\"unlock_lv\":\"3\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"develop_gun\",\"count\":\"5\",\"prize_id\":\"730\",\"title\":\"career_quest-10000050\",\"content\":\"career_quest-20000050\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"51\",\"unlock_lv\":\"\",\"unlock_ids\":\"develop_gun:1\",\"unlock_label\":\"\",\"type\":\"combine_gun\",\"count\":\"1\",\"prize_id\":\"731\",\"title\":\"career_quest-10000051\",\"content\":\"career_quest-20000051\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"52\",\"unlock_lv\":\"\",\"unlock_ids\":\"develop_gun:1\",\"unlock_label\":\"\",\"type\":\"eat_gun\",\"count\":\"5\",\"prize_id\":\"732\",\"title\":\"career_quest-10000052\",\"content\":\"career_quest-20000052\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"53\",\"unlock_lv\":\"\",\"unlock_ids\":\"eat_gun:1\",\"unlock_label\":\"\",\"type\":\"retire_gun\",\"count\":\"5\",\"prize_id\":\"733\",\"title\":\"career_quest-10000053\",\"content\":\"career_quest-20000053\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"54\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:20\",\"type\":\"develop_equip\",\"count\":\"5\",\"prize_id\":\"734\",\"title\":\"career_quest-10000054\",\"content\":\"career_quest-20000054\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"55\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:20\",\"type\":\"retire_equip\",\"count\":\"5\",\"prize_id\":\"735\",\"title\":\"career_quest-10000055\",\"content\":\"career_quest-20000055\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"56\",\"unlock_lv\":\"\",\"unlock_ids\":\"combine_gun:1\",\"unlock_label\":\"\",\"type\":\"combine_gun_type_4\",\"count\":\"1\",\"prize_id\":\"736\",\"title\":\"career_quest-10000056\",\"content\":\"career_quest-20000056\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"57\",\"unlock_lv\":\"\",\"unlock_ids\":\"combine_gun:1\",\"unlock_label\":\"\",\"type\":\"combine_gun_type_2\",\"count\":\"1\",\"prize_id\":\"737\",\"title\":\"career_quest-10000057\",\"content\":\"career_quest-20000057\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"58\",\"unlock_lv\":\"\",\"unlock_ids\":\"combine_gun:1\",\"unlock_label\":\"\",\"type\":\"combine_gun_type_5\",\"count\":\"1\",\"prize_id\":\"738\",\"title\":\"career_quest-10000058\",\"content\":\"career_quest-20000058\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"59\",\"unlock_lv\":\"\",\"unlock_ids\":\"combine_gun:1\",\"unlock_label\":\"\",\"type\":\"combine_gun_type_3\",\"count\":\"1\",\"prize_id\":\"739\",\"title\":\"career_quest-10000059\",\"content\":\"career_quest-20000059\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"60\",\"unlock_lv\":\"\",\"unlock_ids\":\"combine_gun:1\",\"unlock_label\":\"\",\"type\":\"combine_gun_type_1\",\"count\":\"1\",\"prize_id\":\"740\",\"title\":\"career_quest-10000060\",\"content\":\"career_quest-20000060\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"61\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"bestrank1:30\",\"type\":\"special_develop_gun\",\"count\":\"1\",\"prize_id\":\"741\",\"title\":\"career_quest-10000061\",\"content\":\"career_quest-20000061\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"100\",\"unlock_lv\":\"3\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"gasha_count\",\"count\":\"1\",\"prize_id\":\"742\",\"title\":\"career_quest-10000100\",\"content\":\"career_quest-20000100\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"101\",\"unlock_lv\":\"3\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"open_gift\",\"count\":\"5\",\"prize_id\":\"743\",\"title\":\"career_quest-10000101\",\"content\":\"career_quest-20000101\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"102\",\"unlock_lv\":\"3\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"dorm_change\",\"count\":\"1\",\"prize_id\":\"744\",\"title\":\"career_quest-10000102\",\"content\":\"career_quest-20000102\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"103\",\"unlock_lv\":\"3\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"friend_visit\",\"count\":\"5\",\"prize_id\":\"745\",\"title\":\"career_quest-10000103\",\"content\":\"career_quest-20000103\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"104\",\"unlock_lv\":\"3\",\"unlock_ids\":\"friend_visit:1\",\"unlock_label\":\"\",\"type\":\"friend_praise\",\"count\":\"5\",\"prize_id\":\"746\",\"title\":\"career_quest-10000104\",\"content\":\"career_quest-20000104\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"150\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:7\",\"type\":\"friend_apply\",\"count\":\"1\",\"prize_id\":\"758\",\"title\":\"career_quest-10000150\",\"content\":\"career_quest-20000150\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"151\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:7\",\"type\":\"friend_card_change\",\"count\":\"1\",\"prize_id\":\"759\",\"title\":\"career_quest-10000151\",\"content\":\"career_quest-20000151\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"152\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:7\",\"type\":\"friend_headpic_change\",\"count\":\"1\",\"prize_id\":\"760\",\"title\":\"career_quest-10000152\",\"content\":\"career_quest-20000152\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"153\",\"unlock_lv\":\"12\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"upgrade\",\"count\":\"5\",\"prize_id\":\"761\",\"title\":\"career_quest-10000153\",\"content\":\"career_quest-20000153\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"154\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:90004\",\"type\":\"eat_equip\",\"count\":\"5\",\"prize_id\":\"762\",\"title\":\"career_quest-10000154\",\"content\":\"career_quest-20000154\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"155\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:90008\",\"type\":\"adjust_equip\",\"count\":\"5\",\"prize_id\":\"763\",\"title\":\"career_quest-10000155\",\"content\":\"career_quest-20000155\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"156\",\"unlock_lv\":\"\",\"unlock_ids\":\"adjust_equip:1\",\"unlock_label\":\"\",\"type\":\"adjust_equip_attr_100\",\"count\":\"1\",\"prize_id\":\"764\",\"title\":\"career_quest-10000156\",\"content\":\"career_quest-20000156\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"157\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"bestrank1:60\",\"type\":\"develop_fairy\",\"count\":\"1\",\"prize_id\":\"765\",\"title\":\"career_quest-10000157\",\"content\":\"career_quest-20000157\",\"unlock_course\":\"80\",\"new_type\":\"1\"},{\"id\":\"158\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"bestrank1:60\",\"type\":\"eat_fairy\",\"count\":\"5\",\"prize_id\":\"766\",\"title\":\"career_quest-10000158\",\"content\":\"career_quest-20000158\",\"unlock_course\":\"86,87,88\",\"new_type\":\"1\"},{\"id\":\"159\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"bestrank1:60\",\"type\":\"upgrade_fairy\",\"count\":\"5\",\"prize_id\":\"767\",\"title\":\"career_quest-10000159\",\"content\":\"career_quest-20000159\",\"unlock_course\":\"86,87,88\",\"new_type\":\"1\"},{\"id\":\"160\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"bestrank1:60\",\"type\":\"adjust_fairy\",\"count\":\"5\",\"prize_id\":\"768\",\"title\":\"career_quest-10000160\",\"content\":\"career_quest-20000160\",\"unlock_course\":\"86,87,88\",\"new_type\":\"1\"},{\"id\":\"161\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"bestrank1:60\",\"type\":\"retire_fairy\",\"count\":\"5\",\"prize_id\":\"769\",\"title\":\"career_quest-10000161\",\"content\":\"career_quest-20000161\",\"unlock_course\":\"86,87,88\",\"new_type\":\"1\"},{\"id\":\"162\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"bestrank1:60\",\"type\":\"establish_up_type_301\",\"count\":\"1\",\"prize_id\":\"770\",\"title\":\"career_quest-10000162\",\"content\":\"career_quest-20000162\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"163\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"bestrank1:60\",\"type\":\"establish_up_type_302\",\"count\":\"1\",\"prize_id\":\"771\",\"title\":\"career_quest-10000163\",\"content\":\"career_quest-20000163\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"164\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"bestrank1:60\",\"type\":\"establish_up_type_303\",\"count\":\"1\",\"prize_id\":\"772\",\"title\":\"career_quest-10000164\",\"content\":\"career_quest-20000164\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"165\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"bestrank1:60\",\"type\":\"establish_up_type_304\",\"count\":\"1\",\"prize_id\":\"773\",\"title\":\"career_quest-10000165\",\"content\":\"career_quest-20000165\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"166\",\"unlock_lv\":\"60\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"up_mod1\",\"count\":\"1\",\"prize_id\":\"774\",\"title\":\"career_quest-10000166\",\"content\":\"career_quest-20000166\",\"unlock_course\":\"97\",\"new_type\":\"1\"},{\"id\":\"167\",\"unlock_lv\":\"\",\"unlock_ids\":\"up_mod1:1\",\"unlock_label\":\"\",\"type\":\"up_mod2\",\"count\":\"1\",\"prize_id\":\"775\",\"title\":\"career_quest-10000167\",\"content\":\"career_quest-20000167\",\"unlock_course\":\"97\",\"new_type\":\"1\"},{\"id\":\"168\",\"unlock_lv\":\"\",\"unlock_ids\":\"up_mod2:1\",\"unlock_label\":\"\",\"type\":\"up_mod3\",\"count\":\"1\",\"prize_id\":\"776\",\"title\":\"career_quest-10000168\",\"content\":\"career_quest-20000168\",\"unlock_course\":\"97\",\"new_type\":\"1\"},{\"id\":\"169\",\"unlock_lv\":\"\",\"unlock_ids\":\"up_mod2:1\",\"unlock_label\":\"\",\"type\":\"upgrade2\",\"count\":\"1\",\"prize_id\":\"777\",\"title\":\"career_quest-10000169\",\"content\":\"career_quest-20000169\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"170\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"sun_friend_team_into\",\"count\":\"1\",\"prize_id\":\"778\",\"title\":\"career_quest-10000170\",\"content\":\"career_quest-20000170\",\"unlock_course\":\"100\",\"new_type\":\"1\"},{\"id\":\"171\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"night_friend_team_into\",\"count\":\"1\",\"prize_id\":\"779\",\"title\":\"career_quest-10000171\",\"content\":\"career_quest-20000171\",\"unlock_course\":\"100\",\"new_type\":\"1\"},{\"id\":\"172\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"squad_base_data_analyse\",\"count\":\"1\",\"prize_id\":\"780\",\"title\":\"career_quest-10000172\",\"content\":\"career_quest-20000172\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"173\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"squad_senior_data_analyse\",\"count\":\"1\",\"prize_id\":\"781\",\"title\":\"career_quest-10000173\",\"content\":\"career_quest-20000173\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"174\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"squad_get_chip\",\"count\":\"1\",\"prize_id\":\"782\",\"title\":\"career_quest-10000174\",\"content\":\"career_quest-20000174\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"175\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"squad_get_secret_data\",\"count\":\"1\",\"prize_id\":\"783\",\"title\":\"career_quest-10000175\",\"content\":\"career_quest-20000175\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"176\",\"unlock_lv\":\"\",\"unlock_ids\":\"squad_get_secret_data:1\",\"unlock_label\":\"\",\"type\":\"squad_get_type1_unit\",\"count\":\"1\",\"prize_id\":\"784\",\"title\":\"career_quest-10000176\",\"content\":\"career_quest-20000176\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"177\",\"unlock_lv\":\"\",\"unlock_ids\":\"squad_get_secret_data:1\",\"unlock_label\":\"\",\"type\":\"squad_up_rank\",\"count\":\"1\",\"prize_id\":\"785\",\"title\":\"career_quest-10000177\",\"content\":\"career_quest-20000177\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"178\",\"unlock_lv\":\"\",\"unlock_ids\":\"squad_get_secret_data:1\",\"unlock_label\":\"\",\"type\":\"squad_rank_max\",\"count\":\"1\",\"prize_id\":\"786\",\"title\":\"career_quest-10000178\",\"content\":\"career_quest-20000178\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"179\",\"unlock_lv\":\"\",\"unlock_ids\":\"squad_get_type1_unit:1\",\"unlock_label\":\"\",\"type\":\"squad_train_time\",\"count\":\"1\",\"prize_id\":\"787\",\"title\":\"career_quest-10000179\",\"content\":\"career_quest-20000179\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"180\",\"unlock_lv\":\"\",\"unlock_ids\":\"squad_get_type1_unit:1\",\"unlock_label\":\"\",\"type\":\"squad_level_max\",\"count\":\"1\",\"prize_id\":\"788\",\"title\":\"career_quest-10000180\",\"content\":\"career_quest-20000180\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"181\",\"unlock_lv\":\"\",\"unlock_ids\":\"squad_get_type1_unit:1\",\"unlock_label\":\"\",\"type\":\"squad_up_skill_times\",\"count\":\"1\",\"prize_id\":\"789\",\"title\":\"career_quest-10000181\",\"content\":\"career_quest-20000181\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"182\",\"unlock_lv\":\"\",\"unlock_ids\":\"squad_get_type1_unit:1\",\"unlock_label\":\"\",\"type\":\"squad_up_advanced_times\",\"count\":\"1\",\"prize_id\":\"790\",\"title\":\"career_quest-10000182\",\"content\":\"career_quest-20000182\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"183\",\"unlock_lv\":\"\",\"unlock_ids\":\"squad_get_secret_data:1\",\"unlock_label\":\"\",\"type\":\"squad_change_data_times\",\"count\":\"1\",\"prize_id\":\"791\",\"title\":\"career_quest-10000183\",\"content\":\"career_quest-20000183\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"184\",\"unlock_lv\":\"\",\"unlock_ids\":\"squad_get_chip:1\",\"unlock_label\":\"\",\"type\":\"squad_chip_retire_times\",\"count\":\"1\",\"prize_id\":\"792\",\"title\":\"career_quest-10000184\",\"content\":\"career_quest-20000184\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"185\",\"unlock_lv\":\"\",\"unlock_ids\":\"squad_get_chip:1\",\"unlock_label\":\"\",\"type\":\"squad_eat_chip_num\",\"count\":\"1\",\"prize_id\":\"793\",\"title\":\"career_quest-10000185\",\"content\":\"career_quest-20000185\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"186\",\"unlock_lv\":\"20\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"explore_auto_team\",\"count\":\"1\",\"prize_id\":\"794\",\"title\":\"career_quest-10000186\",\"content\":\"career_quest-20000186\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"187\",\"unlock_lv\":\"20\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"explore_first_times\",\"count\":\"1\",\"prize_id\":\"795\",\"title\":\"career_quest-10000187\",\"content\":\"career_quest-20000187\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"188\",\"unlock_lv\":\"20\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"explore_replay\",\"count\":\"1\",\"prize_id\":\"796\",\"title\":\"career_quest-10000188\",\"content\":\"career_quest-20000188\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"189\",\"unlock_lv\":\"20\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"explore_get_prize\",\"count\":\"1\",\"prize_id\":\"797\",\"title\":\"career_quest-10000189\",\"content\":\"career_quest-20000189\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"190\",\"unlock_lv\":\"20\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"explore_change_item\",\"count\":\"1\",\"prize_id\":\"798\",\"title\":\"career_quest-10000190\",\"content\":\"career_quest-20000190\",\"unlock_course\":\"\",\"new_type\":\"1\"},{\"id\":\"191\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"uniform_get_color\",\"count\":\"1\",\"prize_id\":\"799\",\"title\":\"career_quest-10000191\",\"content\":\"career_quest-20000191\",\"unlock_course\":\"130\",\"new_type\":\"1\"},{\"id\":\"192\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"activation_uniform_skill\",\"count\":\"1\",\"prize_id\":\"800\",\"title\":\"career_quest-10000192\",\"content\":\"career_quest-20000192\",\"unlock_course\":\"130\",\"new_type\":\"1\"},{\"id\":\"193\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"\",\"type\":\"change_gender\",\"count\":\"1\",\"prize_id\":\"11002\",\"title\":\"career_quest-10000193\",\"content\":\"career_quest-20000193\",\"unlock_course\":\"130\",\"new_type\":\"1\"},{\"id\":\"194\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:5\",\"type\":\"mission_emergency_0_1_3\",\"count\":\"1\",\"prize_id\":\"11003\",\"title\":\"career_quest-10000194\",\"content\":\"career_quest-20000194\",\"unlock_course\":\"\",\"new_type\":\"2\"},{\"id\":\"203\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:15\",\"type\":\"mission_emergency_1_1_2\",\"count\":\"1\",\"prize_id\":\"11012\",\"title\":\"career_quest-10000203\",\"content\":\"career_quest-20000203\",\"unlock_course\":\"\",\"new_type\":\"2\"},{\"id\":\"232\",\"unlock_lv\":\"\",\"unlock_ids\":\"\",\"unlock_label\":\"mission:90001\",\"type\":\"mission_emergency_3_1_2\",\"count\":\"1\",\"prize_id\":\"11041\",\"title\":\"career_quest-10000232\",\"content\":\"career_quest-20000232\",\"unlock_course\":\"\",\"new_type\":\"2\"}]}"
            //};
            //Transaction.PacketProcess.Index.GetQuest("", quest.body);

            //UserData.Doll.Add(new DollWithUserInfo(123456, 283));
            //UserData.Doll.SwapTeam(1, 1, 123456);
            //UserData.Doll.SetExp(123456, GameData2.Doll.TotalExp[100]);

            //UserData.Doll.GainExp(374192201, 3000);
            //Grid testGrid = new Grid();
            //testGrid.Name = "";
        }
        private void TestButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //TimeUtil.testSec += TimeUtil.MINUTE * 10;
            //UserData.CombatSimulation.UseBp(1);
        }

        #region Window

        /// <summary>
        /// 메인화면 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 윈도우 위치 및 크기
            this.Top = Config.Window.windowPosition[0];
            this.Left = Config.Window.windowPosition[1];
            this.Width = Config.Window.windowPosition[2];
            this.Height = Config.Window.windowPosition[3];
            //this.Top = Config.windowTop;
            //this.Left = Config.windowLeft;
            //this.Width = Config.windowWidth;
            //this.Height = Config.windowHeight;

            // 윈도우 요소 가져오기
            WindowBorder = this.Template.FindName("WindowBorder", this) as Border;
            WindowTitlebarGrid = this.Template.FindName("WindowTitlebarGrid", this) as Grid;

            // 트레이 컨텍스트
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem closeItem = new System.Windows.Forms.MenuItem();          // 종료 버튼
            contextMenu.MenuItems.Add(closeItem);
            closeItem.Index = 0;
            closeItem.Text = LanguageResources.Instance["TRAY_CLOSE"];
            closeItem.Click += delegate (object click, EventArgs e2)
            {
                Environment.Exit(Environment.ExitCode);
            };
            System.Windows.Forms.MenuItem positionResetItem = new System.Windows.Forms.MenuItem();  // 위치 리셋 버튼
            contextMenu.MenuItems.Add(positionResetItem);
            positionResetItem.Index = 1;
            positionResetItem.Text = LanguageResources.Instance["TRAY_RESET_POSITION"];
            positionResetItem.Click += delegate (object click, EventArgs e2)
            {
                MainWindow.view.Top = 0;
                MainWindow.view.Left = 0;
                subView.Top = 20;
                subView.Left = 20;
            };
            notifyIcon.ContextMenu = contextMenu;

            // 항상 위
            AlwaysOnTopButton = this.Template.FindName("AlwaysOnTopButton", this) as ToggleButton;
            if (Config.Window.alwaysOnTop)
            {
                this.Topmost = true;
                subView.Topmost = true;

                this.AlwaysOnTopButton.IsChecked = true;
                this.AlwaysOnTopButton.Content = Icons.Pin;
            }

            // 창 투명도
            this.WindowOpacity = Config.Window.windowOpacity;
            subView.WindowOpacity = Config.Window.windowOpacity;

            // 필터링
            for (int i = 0; i < GroupIdx.Length; i++)
            {
                Grid groupBox = dashboardView.FindName(string.Format("{0}_GroupBox", GroupNms[i])) as Grid;
                CheckBox checkBox = this.FindName(string.Format("{0}FilterCheckBox", GroupNms[i])) as CheckBox;

                groupBox.IsEnabled = Config.Dashboard.filter[i];
                checkBox.IsChecked = Config.Dashboard.filter[i];
            }

            // 경험치 계산기
            this.BaseExpTextBox.Text = Config.Echelon.baseExp.ToString();
            this.BattleCountTextBox.Text = Config.Echelon.battleCount.ToString();
            this.LevelPenaltyTextBox.Text = Config.Echelon.levelPenalty.ToString();
            this.ExpCalExpUpCheckBox.IsChecked = Config.Echelon.expUpEvent;

            // 언어 설정
            InitLanguage();

            //mapView.Show();

            // 메뉴 기본 위치
            this.CurrentMenu = Menus.PROXY;
        }

        /// <summary>
        /// Brings main window to foreground.
        /// </summary>
        public void BringToForeground()
        {
            if (this.WindowState == WindowState.Minimized || this.Visibility == Visibility.Hidden)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            }

            // According to some sources these steps gurantee that an app will be brought to foreground.
            this.Activate();
            this.Focus();
        }

        /// <summary>
        /// 언어 설정
        /// </summary>
        public void InitLanguage()
        {
            this.BpPointToolTipTextBlock.Html = string.Format(LanguageResources.Instance["FOOTER_SIM_POINT_TOOLTIP"],
                "[font color='#ffb400']" + UserData.CombatSimulation.remainPointToday + "[/font]",
                "[font color='#ffb400']" + TimeUtil.GetDateTime(UserData.attendanceTime, "MM-dd HH:mm") + "[/font]");
                //"[font color='#ffb400']" + Parser.Time.GetDateTime(UserData.CombatSimulation.midnightTime).ToString("MM-dd HH:mm") + "[/font]");
        }

        /// <summary>
        /// 윈도우 종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            // 메인화면
            if (!Double.IsNaN(this.Top))
                Config.Window.windowPosition[0] = this.Top;
            //Config.windowTop = this.Top;
            if (!Double.IsNaN(this.Left))
                Config.Window.windowPosition[1] = this.Left;
            //Config.windowLeft = this.Left;
            if (!Double.IsNaN(this.Width))
                Config.Window.windowPosition[2] = this.Width;
            //Config.windowWidth = this.Width;
            if (!Double.IsNaN(this.Height))
                Config.Window.windowPosition[3] = this.Height;
            //Config.windowHeight = this.Height;

            // 서브화면
            if (subView.isLoaded)
            {
                if (!Double.IsNaN(subView.Top))
                    Config.Window.subWindowPosition[0] = subView.Top;
                //Config.subWindowTop = subView.Top;
                if (!Double.IsNaN(subView.Left))
                    Config.Window.subWindowPosition[1] = subView.Left;
                //Config.subWindowLeft = subView.Left;
                if (!Double.IsNaN(subView.Width))
                    Config.Window.subWindowPosition[2] = subView.Width;
                //Config.subWindowWidth = subView.Width;
                if (!Double.IsNaN(subView.Height))
                    Config.Window.subWindowPosition[3] = subView.Height;
                //Config.subWindowHeight = subView.Height;
            }

            notifyIcon.Visible = false;
            notifyIcon.Dispose();

            Environment.Exit(Environment.ExitCode);
        }

        /// <summary>
        /// 윈도우 활성화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Activated(object sender, EventArgs e)
        {
            if (WindowBorder != null)
                WindowBorder.BorderBrush = (SolidColorBrush)Application.Current.Resources["PrimaryBrush"];
            if (WindowTitlebarGrid != null)
                WindowTitlebarGrid.Background = (SolidColorBrush)Application.Current.Resources["PrimaryBrush"];
            StatusGrid.Background = (SolidColorBrush)Application.Current.Resources["PrimaryBrush"];
        }

        /// <summary>
        /// 윈도우 비활성화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (WindowBorder != null)
                WindowBorder.BorderBrush = (SolidColorBrush)Application.Current.Resources["PrimaryDeactiveBrush"];
            if (WindowTitlebarGrid != null)
                WindowTitlebarGrid.Background = (SolidColorBrush)Application.Current.Resources["PrimaryDeactiveBrush"];
            StatusGrid.Background = (SolidColorBrush)Application.Current.Resources["PrimaryDeactiveBrush"];
        }

        /// <summary>
        /// 윈도우 상태변화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Minimized:
                    if (Config.Window.minimizeToTray)
                    {
                        this.Hide();
                        Notifier.Manager.notifyQueue.Enqueue(new Message()
                        {
                            subject = "소녀전선 알리미",
                            content = "트레이에서 실행 중"
                        });
                    }
                    break;
                case WindowState.Maximized:
                case WindowState.Normal:
                    this.Show();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 윈도우 투명도
        /// </summary>
        public int WindowOpacity
        {
            set
            {
                Dispatcher.Invoke(() =>
                {
                    if (this.ViewMainWindow != null)
                        this.ViewMainWindow.Opacity = ((double)value) / 100;
                });
            }
        }

        /// <summary>
        /// [버튼] 폴더 열기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Util.Common.GetAbsolutePath());
        }

        /// <summary>
        /// [버튼] 항상 위
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlwaysOnTopButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.Window.alwaysOnTop = false;
            this.Topmost = false;
            subView.Topmost = false;
            this.AlwaysOnTopButton.Content = Icons.PinOff;
        }
        private void AlwaysOnTopButton_Checked(object sender, RoutedEventArgs e)
        {
            Config.Window.alwaysOnTop = true;
            this.Topmost = true;
            subView.Topmost = true;
            this.AlwaysOnTopButton.Content = Icons.Pin;
        }

        /// <summary>
        /// [버튼] 최소화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowMinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// [버튼] 종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WidnowCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region TrayTip

        /// <summary>
        /// 트레이 아이콘
        /// </summary>
        private static System.Windows.Forms.NotifyIcon notifyIcon;

        /// <summary>
        /// 트레이 알림
        /// </summary>
        /// <param name="msg"></param>
        public void ShowTrayTip(Message msg)
        {
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(30000, msg.subject, msg.content, System.Windows.Forms.ToolTipIcon.Info);
        }

        #endregion

        #region MediaPlayer

        /// <summary>
        /// 알림 소리
        /// </summary>
        public static MediaPlayer mediaPlayer = new MediaPlayer();
        public void PlayMediaPlayer()
        {
            Dispatcher.Invoke(() =>
            {
                mediaPlayer.MediaEnded += MediaEnded;
                mediaPlayer.Play();
            });
        }
        private void MediaEnded(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                mediaPlayer.Stop();
                mediaPlayer.Close();
            });
        }
        public void OpenMediaPlayer(string filename)
        {
            Dispatcher.Invoke(() =>
            {
                mediaPlayer.Open(new Uri(filename));
            });
        }
        public void VolumeMediaPlayer(int volume)
        {
            Dispatcher.Invoke(() =>
            {
                mediaPlayer.Volume = volume / 100.0f;
                //mediaPlayer.Volume = 100 / 100.0f;
            });
        }

        #endregion

        #region Sticky Window

        // this is the offset of the mouse cursor from the top left corner of the window
        private Point offset = new Point();
        private double dpiMuliply = 1.0;

        private System.Windows.Forms.Screen currentScreen = null;

        double workAreaTop = -1;
        double workAreaLeft = -1;
        double workAreaWidth = -1;
        double workAreaHeight = -1;

        /// <summary>
        /// 윈도우 마우스 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PresentationSource source = PresentationSource.FromVisual(this);
            double dpi = 96;
            if (source != null)
                dpi = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            dpiMuliply = 96.0 / dpi;

            Point cursorPos = PointToScreen(Mouse.GetPosition(this));
            cursorPos.X = cursorPos.X * dpiMuliply;
            cursorPos.Y = cursorPos.Y * dpiMuliply;
            Point windowPos = new Point(this.Left, this.Top);
            offset = (Point)(cursorPos - windowPos);

            /// get screen rect
            currentScreen = Extensions.GetScreen(this);
            workAreaTop = currentScreen.WorkingArea.Top * dpiMuliply;
            workAreaLeft = currentScreen.WorkingArea.Left * dpiMuliply;
            workAreaWidth = currentScreen.WorkingArea.Width * dpiMuliply;
            workAreaHeight = currentScreen.WorkingArea.Height * dpiMuliply;

            // capturing the mouse here will redirect all events to this window, even if
            // the mouse cursor should leave the window area
            Mouse.Capture(this, CaptureMode.Element);
        }

        /// <summary>
        /// 윈도우 마우스 떼기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        /// <summary>
        /// 윈도우 마우스 움직임
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.Captured == this && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point cursorPos = PointToScreen(Mouse.GetPosition(this));

                double newLeft = cursorPos.X * dpiMuliply - offset.X;
                double newTop = cursorPos.Y * dpiMuliply - offset.Y;

                // here you can change the window position and implement
                // the snapping behaviour that you need

                if (Config.Window.stickyWindow)
                {
                    int snappingMargin = 10;

                    if (Math.Abs(workAreaLeft - newLeft) < snappingMargin)
                        newLeft = workAreaLeft;
                    else if (Math.Abs(newLeft + this.ActualWidth - workAreaLeft - workAreaWidth) < snappingMargin)
                        newLeft = workAreaLeft + workAreaWidth - this.ActualWidth;

                    if (Math.Abs(workAreaTop - newTop) < snappingMargin)
                        newTop = workAreaTop;
                    else if (Math.Abs(newTop + this.ActualHeight - workAreaTop - workAreaHeight) < snappingMargin)
                        newTop = workAreaTop + workAreaHeight - this.ActualHeight;

                }

                this.Left = newLeft;
                this.Top = newTop;
            }
        }


        #endregion
    }

    #region Extensions

    static class Extensions
    {
        public static System.Windows.Forms.Screen GetScreen(this System.Windows.Window window)
        {
            return System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(window).Handle);
        }

        public static void GetDpi(this System.Windows.Forms.Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In]System.Drawing.Point pt, [In]uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);
    }

    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }

    #endregion
}
