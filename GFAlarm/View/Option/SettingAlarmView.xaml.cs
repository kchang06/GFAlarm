using GFAlarm.Util;
using MahApps.Metro.IconPacks;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GFAlarm.View.Option
{
    /// <summary>
    /// SettingAlarmView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingAlarmView : UserControl
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public SettingAlarmView()
        {
            InitializeComponent();

            // 완료 알림
            this.useNotifyDispatchedEchelonComplete = Config.Alarm.notifyDispatchedEchelonComplete;
            this.useNotifyExploreComplete = Config.Alarm.notifyExploreComplete;
            this.useNotifyProduceDollComplete = Config.Alarm.notifyProduceDollComplete;
            this.useNotifyProduceEquipComplete = Config.Alarm.notifyProduceEquipComplete;
            this.useNotifySkillTrainComplete = Config.Alarm.notifySkillTrainComplete;
            this.useNotifyRestoreComplete = Config.Alarm.notifyRestoreDollComplete;
            this.useNotifyDataAnalysisComplete = Config.Alarm.notifyDataAnalysisComplete;
            this.useNotifyBattleReportComplete = Config.Alarm.notifyBattleReportComplete;
            this.useNotifyQuestComplete = Config.Extra.notifyQuestComplete;

            // 상한 알림
            this.useMaxPoint = Config.Alarm.notifyMaxBp;
            this.MaxPointSlider.Value = Config.Alarm.notifyMaxBpPoint;
            this.MaxPointSlider.Minimum = 1;
            this.MaxPointSlider.Maximum = 6;
            this.useMaxDoll = Config.Alarm.notifyMaxDoll;
            this.useMaxEquip = Config.Alarm.notifyMaxEquip;
            this.useMaxGlobalExp = Config.Alarm.notifyMaxGlobalExp;

            // 전역 알림
            this.useGetDoll = Config.Alarm.notifyRescueDoll;
            this.GetDollSlider.Value = Config.Alarm.notifyRescueDollStar;
            this.GetDollSlider.Minimum = 2;
            this.GetDollSlider.Maximum = 5;
            this.useGetEquip = Config.Alarm.notifyGetEquip;
            this.GetEquipSlider.Value = Config.Alarm.notifyGetEquipStar;
            this.GetEquipSlider.Minimum = 2;
            this.GetEquipSlider.Maximum = 5;
            this.useMissionSuccess = Config.Alarm.notifyMissionSuccess;
            this.useMoveFinish = Config.Alarm.notifyTeamMove;
            this.MoveFinishSlider.Value = Config.Alarm.notifyTeamMoveCount;
            this.MoveFinishSlider.Minimum = 1;
            this.MoveFinishSlider.Maximum = 40;
            this.useMoveAndBattleFinish = Config.Alarm.notifyTeamMoveAndBattleFinish;

            // 인형 알림
            this.useNeedExpand = Config.Alarm.notifyDollNeedDummyLink;
            this.useHpWarning = Config.Alarm.notifyDollWounded;
            this.HpWarningSlider.Value = Config.Alarm.notifyDollWoundedPercent;
            this.HpWarningSlider.Minimum = 1;
            this.HpWarningSlider.Maximum = 100;
            this.useMaxLevel = Config.Alarm.notifyMaxLevel;

            // 제조 알림
            this.useProducedDoll = Config.Alarm.notifyProduceDoll;
            this.useProducedDoll5Star = Config.Alarm.notifyProduceDoll5Star;
            this.useProducedEquip = Config.Alarm.notifyProduceEquip;
            this.useProducedEquip5Star = Config.Alarm.notifyProduceEquip5Star;
            this.useProducedShotgun = Config.Alarm.notifyProduceShotgun;
            this.useProducedFairy = Config.Alarm.notifyProduceFairy;

            // 지휘관 보너스
            this.useDollExpBonus = Config.Costume.coBonusDollExp;
            this.DollExpBonusSlider.Minimum = 1;
            this.DollExpBonusSlider.Maximum = 100;
            this.DollExpBonusSlider.Value = Config.Costume.coBonusDollExpPercent;
            this.useRestoreTimeBonus = Config.Costume.coBonusRestoreTime;
            this.RestoreTimeBonusSlider.Minimum = 1;
            this.RestoreTimeBonusSlider.Maximum = 100;
            this.RestoreTimeBonusSlider.Value = Config.Costume.coBonusRestoreTimePercent;
            this.useSkillTimeBonus = Config.Costume.coBonusSkillTrainTime;
            this.SkillTimeBonusSlider.Minimum = 1;
            this.SkillTimeBonusSlider.Maximum = 100;
            this.SkillTimeBonusSlider.Value = Config.Costume.coBonusSkillTrainTimePercent;

            // 기타
            this.useEarlyNotify = Config.Extra.earlyNotify;
            this.EarlyNotifySlider.Value = Config.Extra.earlyNotifySeconds;
            this.EarlyNotifySlider.Minimum = 1;
            this.EarlyNotifySlider.Maximum = 300;

            // 언어 새로고침
            InitLanguage();

            //this.Loaded += SettingAlarmView2_Loaded;
        }

        private void SettingAlarmView2_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 언어 새로고침
        /// </summary>
        public void InitLanguage()
        {
        }

        #region 그룹박스

        Dictionary<string, bool> expand = new Dictionary<string, bool>()
        {
            { "Complete", true },
            { "Maximum", true },
            { "Mission", true },
            { "Doll", true },
            { "Produce", true },
            { "Costume", true },
            { "Extra", true },
        };

        /// <summary>
        /// 그룹박스 닫기 가능 여부
        /// </summary>
        public bool isCollapsibleGroup
        {
            get
            {
                foreach (KeyValuePair<string, bool> item in expand)
                {
                    if (item.Value == true)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 그룹박스 모두 여닫기
        /// </summary>
        /// <param name="collapse"></param>
        public void ExpandAllGroupBox(bool collapse = false)
        {
            foreach (KeyValuePair<string, bool> item in expand.ToArray())
            {
                bool tempValue = collapse == true ? false : true;
                if (expand[item.Key] == tempValue)
                    continue;
                expand[item.Key] = tempValue;
                SetListBoxVisible(item.Key, expand[item.Key]);
            }
        }

        /// <summary>
        /// 그룹박스 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border groupbox = sender as Border;
            string name = groupbox.Name;
            if (name.IndexOf("GroupBox") != -1)
                name = name.Substring(0, name.IndexOf("GroupBox"));
            if (expand.ContainsKey(name))
            {
                expand[name] = expand[name] == true ? false : true;
                log.Debug("expand[{0}]={1}", name, expand[name]);
                SetListBoxVisible(name, expand[name]);
            }
            MainWindow.view.CheckExpandCollapseButtonStatus();
        }
        /// <summary>
        /// 그룹박스 여닫기
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expand"></param>
        private void SetListBoxVisible(string name, bool expand = true)
        {
            StackPanel listbox = this.FindName(string.Format("{0}ListBox", name)) as StackPanel;
            if (listbox != null)
            {
                listbox.BeginAnimation(StackPanel.HeightProperty, null);

                Animations.ChangeHeight.From = expand == true ? 0 : listbox.ActualHeight;
                Animations.ChangeHeight.To = expand == true ? listbox.ActualHeight : 0;

                listbox.BeginAnimation(StackPanel.HeightProperty, Animations.ChangeHeight);
            }
            PackIconMaterial arrow = this.FindName(string.Format("{0}GroupBox_Arrow", name)) as PackIconMaterial;
            if (arrow != null)
            {
                arrow.Kind = expand == true ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;
            }
        }

        #endregion

        #region [그룹] 완료 알림

        /// <summary>
        /// 지원현황 완료 알림
        /// </summary>
        public bool useNotifyDispatchedEchelonComplete
        {
            set
            {
                this.DispatchedEchelonCheckBox.IsChecked = value;
                Config.Alarm.notifyDispatchedEchelonComplete = value;
            }
        }
        private void DispatchedEchelonCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useNotifyDispatchedEchelonComplete = true;
        }
        private void DispatchedEchelonCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useNotifyDispatchedEchelonComplete = false;
        }

        /// <summary>
        /// 탐색현황 완료 알림
        /// </summary>
        public bool useNotifyExploreComplete
        {
            set
            {
                this.ExploreCheckBox.IsChecked = value;
                Config.Alarm.notifyExploreComplete = value;
            }
        }
        private void ExploreCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useNotifyExploreComplete = true;
        }
        private void ExploreCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useNotifyExploreComplete = false;
        }

        /// <summary>
        /// 수복현황 완료 알림
        /// </summary>
        public bool useNotifyRestoreComplete
        {
            set
            {
                this.RestoreCheckBox.IsChecked = value;
                Config.Alarm.notifyRestoreDollComplete = value;
            }
        }
        private void RestoreCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useNotifyRestoreComplete = true;
        }
        private void RestoreCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useNotifyRestoreComplete = false;
        }

        /// <summary>
        /// 훈련현황 완료 알림
        /// </summary>
        public bool useNotifySkillTrainComplete
        {
            set
            {
                SkillTrainCheckBox.IsChecked = value;
                Config.Alarm.notifySkillTrainComplete = value;
            }
        }
        private void SkillTrainCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useNotifySkillTrainComplete = true;
        }
        private void SkillTrainCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useNotifySkillTrainComplete = false;
        }

        /// <summary>
        /// 장비제조 완료 알림
        /// </summary>
        public bool useNotifyProduceEquipComplete
        {
            set
            {
                this.ProduceEquipCheckBox.IsChecked = value;
                Config.Alarm.notifyProduceEquipComplete = value;
            }
        }
        private void ProduceEquipCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useNotifyProduceEquipComplete = true;
        }
        private void ProduceEquipCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useNotifyProduceEquipComplete = false;
        }

        /// <summary>
        /// 인형제조 완료 알림
        /// </summary>
        public bool useNotifyProduceDollComplete
        {
            set
            {
                this.ProduceDollCheckBox.IsChecked = value;
                Config.Alarm.notifyProduceDollComplete = value;
            }
        }
        private void ProduceDollCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useNotifyProduceDollComplete = true;
        }
        private void ProduceDollCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useNotifyProduceDollComplete = false;
        }

        /// <summary>
        /// 분석현황 완료 알림
        /// </summary>
        public bool useNotifyDataAnalysisComplete
        {
            set
            {
                this.DataAnalysisCheckBox.IsChecked = value;
                Config.Alarm.notifyDataAnalysisComplete = value;
            }
        }
        private void DataAnalysisCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useNotifyDataAnalysisComplete = true;
        }
        private void DataAnalysisCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useNotifyDataAnalysisComplete = false;
        }

        /// <summary>
        /// 작전보고서 완료 알림
        /// </summary>
        public bool useNotifyBattleReportComplete
        {
            set
            {
                this.BattleReportCheckBox.IsChecked = value;
                Config.Alarm.notifyBattleReportComplete = value;
            }
        }
        private void BattleReportCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useNotifyBattleReportComplete = true;
        }
        private void BattleReportCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useNotifyBattleReportComplete = false;
        }

        /// <summary>
        /// 임무 완료 알림
        /// </summary>
        public bool useNotifyQuestComplete
        {
            set
            {
                this.QuestCheckBox.IsChecked = value;
                Config.Extra.notifyQuestComplete = value;
            }
        }
        private void QuestCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useNotifyQuestComplete = true;
        }
        private void QuestCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useNotifyQuestComplete = false;
        }

        #endregion

        #region [그룹] 상한 알림

        /// <summary>
        /// 모의작전점수 상한 알림
        /// </summary>
        public bool useMaxPoint
        {
            set
            {
                this.MaxPointCheckBox.IsChecked = value;
                this.MaxPointSlider.IsEnabled = value;
                Config.Alarm.notifyMaxBp = value;
            }
        }
        private void MaxPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useMaxPoint = true;
        }
        private void MaxPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useMaxPoint = false;
        }
        private void MaxPointSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Alarm.notifyMaxBpPoint = (int)this.MaxPointSlider.Value;
        }

        /// <summary>
        /// 인형 상한 알림
        /// </summary>
        public bool useMaxDoll
        {
            set
            {
                this.MaxDollCheckBox.IsChecked = value;
                Config.Alarm.notifyMaxDoll = value;
            }
        }
        private void MaxDollCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useMaxDoll = true;
        }
        private void MaxDollCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useMaxDoll = false;
        }

        /// <summary>
        /// 장비 상한 알림
        /// </summary>
        public bool useMaxEquip
        {
            set
            {
                this.MaxEquipCheckBox.IsChecked = value;
                Config.Alarm.notifyMaxEquip = value;
            }
        }
        private void MaxEquipCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useMaxEquip = true;
        }
        private void MaxEquipCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useMaxEquip = false;
        }

        /// <summary>
        /// 자유경험치 상한 알림
        /// </summary>
        public bool useMaxGlobalExp
        {
            set
            {
                this.MaxGlobalExpCheckBox.IsChecked = value;
                Config.Alarm.notifyMaxGlobalExp = value;
            }
        }
        private void MaxGlobalExpCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useMaxGlobalExp = true;
        }
        private void MaxGlobalExpCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useMaxGlobalExp = false;
        }

        #endregion

        #region [그룹] 전역 알림

        /// <summary>
        /// 획득 인형 알림
        /// </summary>
        public bool useGetDoll
        {
            set
            {
                this.GetDollCheckBox.IsChecked = value;
                this.GetDollSlider.IsEnabled = value;
                Config.Alarm.notifyRescueDoll = value;
            }
        }
        private void GetDollCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useGetDoll = true;
        }
        private void GetDollCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useGetDoll = false;
        }
        private void GetDollSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Alarm.notifyRescueDollStar = (int)this.GetDollSlider.Value;
        }

        /// <summary>
        /// 획득 장비 알림
        /// </summary>
        public bool useGetEquip
        {
            set
            {
                this.GetEquipCheckBox.IsChecked = value;
                this.GetEquipSlider.IsEnabled = value;
                Config.Alarm.notifyGetEquip = value;
            }
        }
        private void GetEquipCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useGetEquip = true;
        }
        private void GetEquipCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useGetEquip = false;
        }
        private void GetEquipSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Alarm.notifyGetEquipStar = (int)this.GetEquipSlider.Value;
        }

        /// <summary>
        /// 전역 승리 알림
        /// </summary>
        public bool useMissionSuccess
        {
            set
            {
                this.MissionSuccessCheckBox.IsChecked = value;
                Config.Alarm.notifyMissionSuccess = value;
            }
        }
        private void MissionSuccessCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useMissionSuccess = true;
        }
        private void MissionSuccessCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useMissionSuccess = false;
        }

        /// <summary>
        /// 전투 후 알림
        /// </summary>
        public bool useMoveAndBattleFinish
        {
            set
            {
                this.MoveAndBattleFinishCheckBox.IsChecked = value;
                Config.Alarm.notifyTeamMoveAndBattleFinish = value;
            }
        }
        private void MoveAndBattleFinishCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useMoveAndBattleFinish = true;
        }
        private void MoveAndBattleFinishCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useMoveAndBattleFinish = false;
        }

        /// <summary>
        /// 제대 이동 완료 알림
        /// </summary>
        public bool useMoveFinish
        {
            set
            {
                this.MoveFinishCheckBox.IsChecked = value;
                this.MoveFinishSlider.IsEnabled = value;
                this.MoveAndBattleFinishCheckBox.IsEnabled = value;
                Config.Alarm.notifyTeamMove = value;
            }
        }
        private void MoveFinishCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useMoveFinish = true;
        }
        private void MoveFinishCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useMoveFinish = false;
        }
        private void MoveFinishSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Alarm.notifyTeamMoveCount = (int)this.MoveFinishSlider.Value;
        }

        #endregion

        #region [그룹] 인형 알림

        /// <summary>
        /// 편제확대 필요
        /// </summary>
        public bool useNeedExpand
        {
            set
            {
                this.NeedExpandCheckBox.IsChecked = value;
                Config.Alarm.notifyDollNeedDummyLink = value;
            }
        }
        private void NeedExpandCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useNeedExpand = true;
        }
        private void NeedExpandCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useNeedExpand = false;
        }

        /// <summary>
        /// 최대 레벨 알림
        /// </summary>
        public bool useMaxLevel
        {
            set
            {
                this.MaxLevelCheckBox.IsChecked = value;
                Config.Alarm.notifyMaxLevel = value;
            }
        }
        private void MaxLevelCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useMaxLevel = true;
        }
        private void MaxLevelCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useMaxLevel = false;
        }

        /// <summary>
        /// 체력 경고
        /// </summary>
        public bool useHpWarning
        {
            set
            {
                this.HpWarningCheckBox.IsChecked = value;
                this.HpWarningSlider.IsEnabled = value;
                Config.Alarm.notifyDollWounded = value;
            }
        }
        private void HpWarningCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useHpWarning = true;
        }
        private void HpWarningCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useHpWarning = false;
        }
        private void HpWarningSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Alarm.notifyDollWoundedPercent = (int)this.HpWarningSlider.Value;
        }

        #endregion

        #region [그룹] 제조 알림

        /// <summary>
        /// 인형 제조 알림
        /// </summary>
        public bool useProducedDoll
        {
            set
            {
                this.ProducedDollCheckBox.IsChecked = value;
                Config.Alarm.notifyProduceDoll = value;
            }
        }
        private void ProducedDollCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useProducedDoll = true;
        }
        private void ProducedDollCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useProducedDoll = false;
        }

        /// <summary>
        /// 인형 제조 (5성) 알림
        /// </summary>
        public bool useProducedDoll5Star
        {
            set
            {
                this.ProducedDoll5StarCheckBox.IsChecked = value;
                Config.Alarm.notifyProduceDoll5Star = value;
            }
        }
        private void ProducedDoll5StarCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useProducedDoll5Star = true;
        }
        private void ProducedDoll5StarCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useProducedDoll5Star = false;
        }

        /// <summary>
        /// 장비 제조 알림
        /// </summary>
        public bool useProducedEquip
        {
            set
            {
                this.ProducedEquipCheckBox.IsChecked = value;
                Config.Alarm.notifyProduceEquip = value;
            }
        }
        private void ProducedEquipCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useProducedEquip = true;
        }
        private void ProducedEquipCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useProducedEquip = false;
        }

        /// <summary>
        /// 장비 제조 (5성) 알림
        /// </summary>
        public bool useProducedEquip5Star
        {
            set
            {
                this.ProducedEquip5StarCheckBox.IsChecked = value;
                Config.Alarm.notifyProduceEquip5Star = value;
            }
        }
        private void ProducedEquip5StarCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useProducedEquip5Star = true;
        }
        private void ProducedEquip5StarCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useProducedEquip5Star = false;
        }

        /// <summary>
        /// 샷건 제조 알림
        /// </summary>
        public bool useProducedShotgun
        {
            set
            {
                this.ProducedShotgunCheckBox.IsChecked = value;
                Config.Alarm.notifyProduceShotgun = value;
            }
        }
        private void ProducedShotgunCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useProducedShotgun = true;
        }
        private void ProducedShotgunCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useProducedShotgun = false;
        }

        /// <summary>
        /// 요정 제조 알림
        /// </summary>
        public bool useProducedFairy
        {
            set
            {
                this.ProducedFairyCheckBox.IsChecked = value;
                Config.Alarm.notifyProduceFairy = value;
            }
        }
        private void ProducedFairyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useProducedFairy = true;
        }
        private void ProducedFairyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useProducedFairy = false;
        }

        #endregion

        #region [그룹] 코스튬 설정

        /// <summary>
        /// 인형 경험치 보너스 사용 여부
        /// </summary>
        public bool useDollExpBonus
        {
            set
            {
                Config.Costume.coBonusDollExp = value;
                this.DollExpBonusCheckBox.IsChecked = value;
                this.DollExpBonusSlider.IsEnabled = value;
            }
        }
        private void DollExpBonusCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useDollExpBonus = true;
        }
        private void DollExpBonusCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useDollExpBonus = false;
        }
        private void DollExpBonusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Costume.coBonusDollExpPercent = (int)this.DollExpBonusSlider.Value;
        }

        /// <summary>
        /// 수복시간 보너스 사용 여부
        /// </summary>
        public bool useRestoreTimeBonus
        {
            set
            {
                Config.Costume.coBonusRestoreTime = value;
                this.RestoreTimeBonusCheckBox.IsChecked = value;
                this.RestoreTimeBonusSlider.IsEnabled = value;
            }
        }
        private void RestoreTimeBonusCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useRestoreTimeBonus = true;
        }
        private void RestoreTimeBonusCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useRestoreTimeBonus = false;
        }
        private void RestoreTimeBonusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Costume.coBonusRestoreTimePercent = (int)this.RestoreTimeBonusSlider.Value;
        }

        /// <summary>
        /// 스킬훈련시간 보너스 사용 여부
        /// </summary>
        public bool useSkillTimeBonus
        {
            set
            {
                Config.Costume.coBonusSkillTrainTime = value;
                this.SkillTimeBonusCheckBox.IsChecked = value;
                this.SkillTimeBonusSlider.IsEnabled = value;
            }
        }
        private void SkillTimeBonusCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSkillTimeBonus = true;
        }
        private void SkillTimeBonusCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSkillTimeBonus = false;
        }
        private void SkillTimeBonusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Costume.coBonusSkillTrainTimePercent = (int)this.SkillTimeBonusSlider.Value;
        }

        #endregion

        #region [그룹] 기타

        /// <summary>
        /// 미리 알림
        /// </summary>
        public bool useEarlyNotify
        {
            set
            {
                this.EarlyNotifyCheckBox.IsChecked = value;
                this.EarlyNotifySlider.IsEnabled = value;
                Config.Extra.earlyNotify = value;
            }
        }
        private void EarlyNotifyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useEarlyNotify = true;
        }
        private void EarlyNotifyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useEarlyNotify = false;
        }
        private void EarlyNotifySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Extra.earlyNotifySeconds = (int)this.EarlyNotifySlider.Value;
        }

        #endregion

        /// <summary>
        /// 링크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
