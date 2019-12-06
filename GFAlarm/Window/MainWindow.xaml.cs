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

        internal static MainWindow view;                            // 메인 창
        internal static SubWindow subView                           // 서브 창
        {
            get
            {
                if (_subView == null)
                    _subView = new SubWindow();
                return _subView;
            }
        }
        private static SubWindow _subView = null;

        private Timer timer;                            // 타이머
        public bool forceStop = true;                   // 업데이트 중지 여부

        private Grid WindowTitlebarGrid;                // 윈도우 타이틀 바
        private Border WindowBorder;                    // 윈도우 경계
        private ToggleButton AlwaysOnTopButton;         // 항상 위 버튼
        
        public static DashboardView dashboardView = new DashboardView();            // 알림 뷰
        public static EchelonView echelonView = new EchelonView();                  // 제대 뷰
        public static QuestView questView = new QuestView();                        // 임무 뷰
        public static ProxyView proxyView = new ProxyView();                        // 연결 뷰
        public static SettingView settingView = new SettingView();                  // 설정 뷰
        public static SettingAlarmView settingAlarmView = new SettingAlarmView();   // 알림 설정 뷰

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

        /// <summary>
        /// 타이머
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
            //log.Debug("click {0}", name);
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

                Border groupBox = dashboardView.FindName(string.Format("{0}GroupBox", GroupNms[item.Key])) as Border;
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

            Border groupBox = dashboardView.FindName(string.Format("{0}GroupBox", GroupNms[idx])) as Border;
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

        /// <summary>
        /// 테스트 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void TestButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

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
                Border groupBox = dashboardView.FindName(string.Format("{0}GroupBox", GroupNms[i])) as Border;
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
            if (!Double.IsNaN(this.Left))
                Config.Window.windowPosition[1] = this.Left;
            if (!Double.IsNaN(this.Width))
                Config.Window.windowPosition[2] = this.Width;
            if (!Double.IsNaN(this.Height))
                Config.Window.windowPosition[3] = this.Height;

            // 서브화면
            if (subView.isLoaded)
            {
                if (!Double.IsNaN(subView.Top))
                    Config.Window.subWindowPosition[0] = subView.Top;
                if (!Double.IsNaN(subView.Left))
                    Config.Window.subWindowPosition[1] = subView.Left;
                if (!Double.IsNaN(subView.Width))
                    Config.Window.subWindowPosition[2] = subView.Width;
                if (!Double.IsNaN(subView.Height))
                    Config.Window.subWindowPosition[3] = subView.Height;
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
            Application.Current.Resources["PrimaryBrush"] = Application.Current.Resources["WindowActiveBrush"];
        }

        /// <summary>
        /// 윈도우 비활성화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Deactivated(object sender, EventArgs e)
        {
            Application.Current.Resources["PrimaryBrush"] = Application.Current.Resources["WindowDeactiveBrush"];
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
                        //Notifier.Manager.notifyQueue.Enqueue(new Message()
                        //{
                        //    subject = "소녀전선 알리미",
                        //    content = "트레이에서 실행 중"
                        //});
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
            Process.Start(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
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
