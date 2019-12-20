using GFAlarm.Constants;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using MahApps.Metro.IconPacks;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GFAlarm.View.Menu
{
    /// <summary>
    /// DashboardView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region GroupBoxControl

        public static class GroupIdx
        {
            public const int DISPATCHED_ECHELON = 0;
            public const int PRODUCE_DOLL = 1;
            public const int PRODUCE_EQUIP = 2;
            public const int SKILL_TRAIN = 3;
            public const int DATA_ANALYSIS = 4;
            public const int RESTORE_DOLL = 5;
            public const int EXPLORE = 6;

            public const int Length = 7;
        };

        public static Dictionary<int, string> GroupNms = new Dictionary<int, string>()
        {
            { GroupIdx.DISPATCHED_ECHELON, "DispatchedEchelon" },
            { GroupIdx.PRODUCE_DOLL, "ProduceDoll" },
            { GroupIdx.PRODUCE_EQUIP, "ProduceEquip" },
            { GroupIdx.SKILL_TRAIN, "SkillTrain" },
            { GroupIdx.DATA_ANALYSIS, "DataAnalysis" },
            { GroupIdx.RESTORE_DOLL, "RestoreDoll" },
            { GroupIdx.EXPLORE, "Explore" },
        };

        /// <summary>
        /// 그룹 박스 보이기/숨기기
        /// </summary>
        /// <param name="list"></param>
        /// <param name="visible"></param>
        public void SetGroupVisible(int index, bool visible)
        {
            Border groupBox = this.FindName(string.Format("{0}GroupBox", GroupNms[index])) as Border;

            /// 그룹 박스가 비활성화 상태면 안 보이게
            if (groupBox.IsEnabled == false)
            {
                visible = false;
            }

            switch (visible)
            {
                case true:
                    groupBox.Visibility = Visibility.Visible;
                    break;
                case false:
                    groupBox.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        /// <summary>
        /// 그룹 박스 접기 가능 여부
        /// </summary>
        public bool isCollapsibleGroup
        {
            get
            {
                for (int i = 0; i < GroupIdx.Length; i++)
                {
                    if (Config.Dashboard.expand[i] == true)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 그룹 박스 모두 펴기/접기
        /// </summary>
        /// <param name="collapse"></param>
        public void ExpandAllGroup(bool collapse = false)
        {
            for (int i = 0; i < GroupIdx.Length; i++)
            {
                if (collapse)
                    Config.Dashboard.expand[i] = false;
                else
                    Config.Dashboard.expand[i] = true;

                int count = 0;
                if (!collapse)
                {
                    switch (i)
                    {
                        case GroupIdx.DISPATCHED_ECHELON:
                            count = this.DispatchedEchelonList.Count();
                            break;
                        case GroupIdx.PRODUCE_DOLL:
                            count = this.ProduceDollList.Count();
                            break;
                        case GroupIdx.PRODUCE_EQUIP:
                            count = this.ProduceEquipList.Count();
                            break;
                        case GroupIdx.SKILL_TRAIN:
                            count = this.SkillTrainList.Count();
                            break;
                        case GroupIdx.DATA_ANALYSIS:
                            count = this.DataAnalysisList.Count();
                            break;
                        case GroupIdx.RESTORE_DOLL:
                            count = this.RestoreDollList.Count();
                            break;
                        case GroupIdx.EXPLORE:
                            count = this.ExploreList.Count();
                            break;
                    }
                }
                // 그룹 박스 화살표
                PackIconMaterial arrow = this.FindName(string.Format("{0}GroupBox_Arrow", GroupNms[i])) as PackIconMaterial;
                arrow.Kind = collapse == false ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;

                SetListBoxHeight(i, count);
            }
        }

        /// <summary>
        /// 리스트 박스 크기 변경
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="count"></param>
        public void SetListBoxHeight(int idx, int count)
        {
            Border groupBox = this.FindName(string.Format("{0}GroupBox", GroupNms[idx])) as Border;
            ListBox listBox = this.FindName(string.Format("{0}ListBox", GroupNms[idx])) as ListBox;
            //Storyboard sb = new Storyboard();
            DoubleAnimation ChangeHeight = new DoubleAnimation
            {
                From = 0,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
            };
            double fromHeight = listBox.ActualHeight;
            double toHeight = count * 40;

            listBox.Visibility = Visibility.Visible;

            if (Config.Dashboard.expand[idx] == false || groupBox.IsEnabled == false)
            {
                toHeight = 0;
            }
            ChangeHeight.From = fromHeight;
            ChangeHeight.To = toHeight;

            listBox.BeginAnimation(ListBox.HeightProperty, null);
            listBox.BeginAnimation(ListBox.HeightProperty, ChangeHeight);
        }


        bool isDragging = false;
        private Point startPosition;

        int currentGroupIdx = 0;
        int upperGroupIdx = -1;
        int lowerGroupIdx = int.MaxValue;

        /// <summary>
        /// 그룹 박스 마우스 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border groupBox = sender as Border;

            isDragging = false;
            startPosition = e.GetPosition(this);

            if (Mouse.Capture(this))
            {
                groupBox.CaptureMouse();
            }
        }

        /// <summary>
        /// 그룹 박스 마우스 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Group_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(this.Parent as FrameworkElement);

                // 드래그 여부 체크
                if (Math.Abs(position.X - startPosition.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - startPosition.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    isDragging = true;
                    StackPanel group = (sender as Border).Parent as StackPanel;
                    group.BeginAnimation(StackPanel.OpacityProperty, null);
                    group.Opacity = 0.25;
                }

                // 드래그인 경우
                if (isDragging)
                {
                    currentGroupIdx = this.AlarmStackPanel.Children.IndexOf((sender as Border).Parent as StackPanel);
                    upperGroupIdx = -1;
                    lowerGroupIdx = int.MaxValue;
                    foreach (KeyValuePair<int, string> item in GroupNms)
                    {
                        StackPanel group = this.FindName(string.Format("{0}", item.Value)) as StackPanel;
                        if (group.Visibility == Visibility.Collapsed)
                        {
                            continue;
                        }
                        int groupIdx = this.AlarmStackPanel.Children.IndexOf(group);
                        Point groupPoint = group.TransformToAncestor(this).Transform(new Point(0, 0));
                        if (groupPoint.Y + 16 > position.Y)
                        {
                            if (lowerGroupIdx > groupIdx)
                            {
                                lowerGroupIdx = groupIdx;
                            }
                        }
                        else if (groupPoint.Y + 16 < position.Y)
                        {
                            if (upperGroupIdx < groupIdx)
                            {
                                upperGroupIdx = groupIdx;
                            }
                        }
                    }

                    foreach (KeyValuePair<int, string> item in GroupNms)
                    {
                        StackPanel group = this.FindName(string.Format("{0}", item.Value)) as StackPanel;
                        Border groupBox = this.FindName(string.Format("{0}GroupBox", item.Value)) as Border;
                        groupBox.BeginAnimation(Grid.OpacityProperty, null);
                        groupBox.Visibility = Visibility.Visible;
                        groupBox.Opacity = 1;
                        ListBox listBox = this.FindName(string.Format("{0}ListBox", item.Value)) as ListBox;
                        listBox.BeginAnimation(ListBox.HeightProperty, null);
                        listBox.Height = 0;
                        Border upperGuideLine = this.FindName(string.Format("{0}UpperGuide", item.Value)) as Border;
                        Border lowerGuideLine = this.FindName(string.Format("{0}LowerGuide", item.Value)) as Border;
                        int upperGuideLineIdx = this.AlarmStackPanel.Children.IndexOf(group);
                        int lowerGuideLineIdx = upperGuideLineIdx;
                        upperGuideLine.Visibility = Visibility.Collapsed;
                        lowerGuideLine.Visibility = Visibility.Collapsed;
                        if (upperGroupIdx == -1)
                        {
                            if (lowerGroupIdx == lowerGuideLineIdx)
                            {
                                lowerGuideLine.Visibility = Visibility.Visible;
                            }
                        }
                        else if (lowerGroupIdx == int.MaxValue)
                        {
                            if (upperGroupIdx == upperGuideLineIdx)
                            {
                                upperGuideLine.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            if (upperGroupIdx == upperGuideLineIdx)
                            {
                                upperGuideLine.Visibility = Visibility.Visible;
                            }
                            else if (lowerGroupIdx == lowerGuideLineIdx)
                            {
                                lowerGuideLine.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 그룹 박스 마우스 떼기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Border groupBox = sender as Border;

            Mouse.Capture(null);
            groupBox.ReleaseMouseCapture();

            string name = groupBox.Name;

            int idx = 0;
            int count = 0;
            bool expand;
            switch (name)
            {
                case "DispatchedEchelonGroupBox":
                    idx = GroupIdx.DISPATCHED_ECHELON;
                    count = this.DispatchedEchelonList.Count();
                    break;
                case "ProduceDollGroupBox":
                    idx = GroupIdx.PRODUCE_DOLL;
                    count = this.ProduceDollList.Count();
                    break;
                case "ProduceEquipGroupBox":
                    idx = GroupIdx.PRODUCE_EQUIP;
                    count = this.ProduceEquipList.Count();
                    break;
                case "SkillTrainGroupBox":
                    idx = GroupIdx.SKILL_TRAIN;
                    count = this.SkillTrainList.Count();
                    break;
                case "DataAnalysisGroupBox":
                    idx = GroupIdx.DATA_ANALYSIS;
                    count = this.DataAnalysisList.Count();
                    break;
                case "RestoreDollGroupBox":
                    idx = GroupIdx.RESTORE_DOLL;
                    count = this.RestoreDollList.Count();
                    break;
                case "ExploreGroupBox":
                    idx = GroupIdx.EXPLORE;
                    count = this.ExploreList.Count();
                    break;
            }

            // 드래그
            if (isDragging)
            {
                StackPanel group = this.FindName(string.Format("{0}", GroupNms[idx])) as StackPanel;

                if (upperGroupIdx == -1)
                {
                    MoveElement(lowerGroupIdx, group);
                }
                else if (lowerGroupIdx == int.MaxValue)
                {
                    MoveElement(upperGroupIdx, group);
                }
                else
                {
                    if (currentGroupIdx > upperGroupIdx)
                    {
                        MoveElement(lowerGroupIdx, group);
                    }
                    else
                    {
                        MoveElement(upperGroupIdx, group);
                    }
                }

                foreach (KeyValuePair<int, string> item in GroupNms)
                {
                    Border upperGuideLine = this.FindName(string.Format("{0}UpperGuide", item.Value)) as Border;
                    Border lowerGuideLine = this.FindName(string.Format("{0}LowerGuide", item.Value)) as Border;
                    upperGuideLine.Visibility = Visibility.Collapsed;
                    lowerGuideLine.Visibility = Visibility.Collapsed;

                    StackPanel itemGroup = this.FindName(string.Format("{0}", item.Value)) as StackPanel;
                    int index = this.AlarmStackPanel.Children.IndexOf(itemGroup);
                    Config.Dashboard.index[item.Key] = index;
                }
                CheckAll();
            }
            // 클릭
            else
            {
                Config.Dashboard.expand[idx] = Config.Dashboard.expand[idx] == true ? false : true;
                expand = Config.Dashboard.expand[idx];
                count = expand == true ? count : 0;

                SetListBoxHeight(idx, count);

                PackIconMaterial arrow = this.FindName(string.Format("{0}GroupBox_Arrow", GroupNms[idx])) as PackIconMaterial;
                arrow.Kind = expand == true ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;

                MainWindow.view.CheckExpandCollapseButtonStatus();
            }
        }

        /// <summary>
        /// 엘리먼트 이동
        /// </summary>
        /// <param name="moveIdx"></param>
        /// <param name="element"></param>
        public void MoveElement(int moveIdx, UIElement element)
        {
            //log.Info("move_idx {0}", moveIdx);
            int currentSelectedIndex = this.AlarmStackPanel.Children.IndexOf(element);
            int childCount = this.AlarmStackPanel.Children.Count;
            if (moveIdx < 0)
                moveIdx = 0;
            else if (moveIdx > childCount)
                moveIdx = childCount;
            //log.Info("current_selected_index {0} child_count {1}", currentSelectedIndex, childCount);
            this.AlarmStackPanel.Children.RemoveAt(currentSelectedIndex);
            this.AlarmStackPanel.Children.Insert(moveIdx, element);
            element.Opacity = 0;
            element.BeginAnimation(StackPanel.OpacityProperty, Animations.FadeIn);
        }

        #endregion

        #region DashboardList

        // 지원현황
        public ObservableCollection<DispatchedEchleonTemplate> DispatchedEchelonList { get; set; } = new ObservableCollection<DispatchedEchleonTemplate>();
        // 인형제조
        public ObservableCollection<ProduceDollTemplate> ProduceDollList { get; set; } = new ObservableCollection<ProduceDollTemplate>();
        // 장비제조
        public ObservableCollection<ProduceEquipTemplate> ProduceEquipList { get; set; } = new ObservableCollection<ProduceEquipTemplate>();
        // 스킬훈련
        public ObservableCollection<SkillTrainTemplate> SkillTrainList { get; set; } = new ObservableCollection<SkillTrainTemplate>();
        // 정보분석
        public ObservableCollection<DataAnalysisTemplate> DataAnalysisList { get; set; } = new ObservableCollection<DataAnalysisTemplate>();
        public HashSet<int> DataAnalysisEndTimes = new HashSet<int>();
        // 수복현황
        public ObservableCollection<RestoreDollTemplate> RestoreDollList { get; set; } = new ObservableCollection<RestoreDollTemplate>();
        // 탐색현황
        public ObservableCollection<ExploreTemplate> ExploreList { get; set; } = new ObservableCollection<ExploreTemplate>();

        /// <summary>
        /// 공유보석 수령 여부
        /// </summary>
        public bool isSharedGem
        {
            set
            {
                Dispatcher.Invoke(() =>
                {
                    this.ShareGemGroupBox.Visibility = value == true ? Visibility.Collapsed : Visibility.Visible;
                });
            }
        }

        /// <summary>
        /// 공유전지 수령 여부
        /// </summary>
        public bool isSharedBattery
        {
            set
            {
                Dispatcher.Invoke(() =>
                {
                    this.ShareBatteryGroupBox.Visibility = value == true ? Visibility.Collapsed : Visibility.Visible;
                });
            }
        }

        #endregion

        #region ListBox Add/Remove/Sort/Check

        /// <summary>
        /// 알림 추가
        /// </summary>
        /// <param name="data"></param>
        public void Add(Object data)
        {
            Dispatcher.Invoke(() =>
            {
                /// 지원현황
                if (data is DispatchedEchleonTemplate)
                {
                    DispatchedEchleonTemplate item = data as DispatchedEchleonTemplate;
                    Remove(item);
                    this.DispatchedEchelonList.Add(item);
                    // 군수지원
                    if (item.operationId > 0)
                    {
                        MainWindow.echelonView.SetTeamStatus(item.teamId, EchelonStatus.Logistics);
                    }
                    // 자율작전
                    else if (item.autoMissionId > 0)
                    {
                        //log.Debug("자율작전 참여제대 수 {0}", item.teamIds.Count());
                        foreach (int teamId in item.teamIds)
                        {
                            //log.Debug("제대 {0} 자율작전 전환", teamId);
                            MainWindow.echelonView.SetTeamStatus(teamId, EchelonStatus.AutoBattle);
                        }
                    }
                }
                /// 인형제조
                else if (data is ProduceDollTemplate)
                {
                    ProduceDollTemplate item = data as ProduceDollTemplate;
                    Remove(item);
                    this.ProduceDollList.Add(item);
                }
                /// 장비제조
                else if (data is ProduceEquipTemplate)
                {
                    ProduceEquipTemplate item = data as ProduceEquipTemplate;
                    Remove(item);
                    this.ProduceEquipList.Add(item);
                }
                /// 스킬훈련
                else if (data is SkillTrainTemplate)
                {
                    SkillTrainTemplate item = data as SkillTrainTemplate;
                    Remove(item);
                    this.SkillTrainList.Add(item);
                }
                /// 정보분석
                else if (data is DataAnalysisTemplate)
                {
                    DataAnalysisTemplate item = data as DataAnalysisTemplate;
                    Remove(item);
                    this.DataAnalysisList.Add(item);
                    if (TimeUtil.GetCurrentSec() < item.endTime - Config.Extra.earlyNotifySeconds)
                        this.DataAnalysisEndTimes.Add(item.endTime);
                }
                /// 수복현황
                else if (data is RestoreDollTemplate)
                {
                    RestoreDollTemplate item = data as RestoreDollTemplate;
                    Remove(item);
                    this.RestoreDollList.Add(item);
                }
                /// 탐색현황
                else if (data is ExploreTemplate)
                {
                    ExploreTemplate item = data as ExploreTemplate;
                    Remove(item);
                    this.ExploreList.Add(item);
                }
                Sort(data);
                Check(data);
            });
        }

        /// <summary>
        /// 알림 제거
        /// </summary>
        /// <param name="data"></param>
        public void Remove(Object data)
        {
            Dispatcher.Invoke(() =>
            {
                /// 지원현황
                if (data is DispatchedEchleonTemplate)
                {
                    DispatchedEchleonTemplate item = data as DispatchedEchleonTemplate;
                    for (int i = 0; i < this.DispatchedEchelonList.Count(); i++)
                    {
                        // 군수지원
                        if (item.operationId > 0 && item.operationId == this.DispatchedEchelonList[i].operationId)
                        {
                            MainWindow.echelonView.SetTeamStatus(this.DispatchedEchelonList[i].teamId, EchelonStatus.StandBy);
                            this.DispatchedEchelonList.RemoveAt(i);
                            break;
                        }
                        // 자율작전
                        else if (item.autoMissionId > 0 && item.autoMissionId == this.DispatchedEchelonList[i].autoMissionId)
                        {
                            foreach (int teamId in this.DispatchedEchelonList[i].teamIds)
                            {
                                MainWindow.echelonView.SetTeamStatus(teamId, EchelonStatus.StandBy);
                            }
                            this.DispatchedEchelonList.RemoveAt(i);
                            break;
                        }
                    }
                }
                /// 인형제조
                else if (data is ProduceDollTemplate)
                {
                    ProduceDollTemplate item = data as ProduceDollTemplate;
                    for (int i = 0; i < this.ProduceDollList.Count(); i++)
                    {
                        if (item.slot == this.ProduceDollList[i].slot)
                        {
                            this.ProduceDollList.RemoveAt(i);
                            break;
                        }
                    }
                }
                /// 장비제조
                else if (data is ProduceEquipTemplate)
                {
                    ProduceEquipTemplate item = data as ProduceEquipTemplate;
                    for (int i = 0; i < this.ProduceEquipList.Count(); i++)
                    {
                        if (item.slot == this.ProduceEquipList[i].slot)
                        {
                            this.ProduceEquipList.RemoveAt(i);
                            break;
                        }
                    }
                }
                /// 스킬훈련
                else if (data is SkillTrainTemplate)
                {
                    SkillTrainTemplate item = data as SkillTrainTemplate;
                    for (int i = 0; i < this.SkillTrainList.Count(); i++)
                    {
                        if (item.slot == this.SkillTrainList[i].slot && item.slotType == this.SkillTrainList[i].slotType)
                        {
                            this.SkillTrainList.RemoveAt(i);
                            break;
                        }
                    }
                }
                /// 정보분석
                else if (data is DataAnalysisTemplate)
                {
                    DataAnalysisTemplate item = data as DataAnalysisTemplate;
                    for (int i = 0; i < this.DataAnalysisList.Count(); i++)
                    {
                        if (item.slot == this.DataAnalysisList[i].slot)
                        {
                            int endTime = this.DataAnalysisList[i].endTime;
                            this.DataAnalysisEndTimes.RemoveWhere(delegate (int l) { return l == endTime; });
                            if (this.DataAnalysisList[i].canvasChipShape != null)
                            {
                                this.DataAnalysisList[i].canvasChipShape.Children.Clear();
                            }
                            this.DataAnalysisList.RemoveAt(i);
                            break;
                        }
                    }
                }
                /// 수복현황
                else if (data is RestoreDollTemplate)
                {
                    RestoreDollTemplate item = data as RestoreDollTemplate;
                    for (int i = 0; i < this.RestoreDollList.Count(); i++)
                    {
                        if (item.slot == this.RestoreDollList[i].slot)
                        {
                            this.RestoreDollList.RemoveAt(i);
                            break;
                        }
                    }
                }
                /// 탐색현황
                else if (data is ExploreTemplate)
                {
                    ExploreTemplate item = data as ExploreTemplate;
                    for (int i = 0; i < this.ExploreList.Count(); i++)
                    {
                        if (item.id == this.ExploreList[i].id)
                        {
                            this.ExploreList.RemoveAt(i);
                            break;
                        }
                    }
                }
                Sort(data);
                Check(data);
            });
        }

        /// <summary>
        /// 알림 비우기
        /// </summary>
        /// <param name="data"></param>
        public void Clear(Object data)
        {
            /// 지원현황
            if (data is DispatchedEchleonTemplate)
            {
                foreach (dynamic item in this.DispatchedEchelonList.ToList())
                {
                    Remove(item);
                }
            }
            /// 인형제조
            else if (data is ProduceDollTemplate)
            {
                foreach (dynamic item in this.ProduceDollList.ToList())
                {
                    Remove(item);
                }
            }
            /// 장비제조
            else if (data is ProduceEquipTemplate)
            {
                foreach (dynamic item in this.ProduceEquipList.ToList())
                {
                    Remove(item);
                }
            }
            /// 스킬훈련
            else if (data is SkillTrainTemplate)
            {
                foreach (dynamic item in this.SkillTrainList.ToList())
                {
                    Remove(item);
                }
            }
            /// 정보분석
            else if (data is DataAnalysisTemplate)
            {
                foreach (dynamic item in this.DataAnalysisList.ToList())
                {
                    Remove(item);
                }
            }
            /// 수복현황
            else if (data is RestoreDollTemplate)
            {
                foreach (dynamic item in this.RestoreDollList.ToList())
                {
                    Remove(item);
                }
            }
            /// 탐색현황
            else if (data is ExploreTemplate)
            {
                foreach (dynamic item in this.ExploreList.ToList())
                {
                    Remove(item);
                }
            }
        }

        /// <summary>
        /// 알림 전체 비우기
        /// </summary>
        public void ClearAll()
        {
            Clear(new DispatchedEchleonTemplate());
            Clear(new ProduceDollTemplate());
            Clear(new ProduceEquipTemplate());
            Clear(new SkillTrainTemplate());
            Clear(new DataAnalysisTemplate());
            Clear(new RestoreDollTemplate());
            Clear(new ExploreTemplate());
        }

        /// <summary>
        /// 알림 정렬
        /// </summary>
        /// <param name="data"></param>
        public void Sort(Object data)
        {
            Dispatcher.Invoke(() =>
            {
                /// 지원현황
                if (data is DispatchedEchleonTemplate)
                {
                    List<DispatchedEchleonTemplate> tempData;
                    switch (Config.Dashboard.sort)
                    {
                        case "remainTime":
                            tempData = this.DispatchedEchelonList.OrderBy(o => o.TBRemainTime).ThenBy(o => o.teamId).ToList();
                            break;
                        case "slot":
                        default:
                            tempData = this.DispatchedEchelonList.OrderBy(o => o.teamId).ToList();
                            break;
                    }
                    foreach (var temp in tempData)
                    {
                        int oldIdx = this.DispatchedEchelonList.IndexOf(temp);
                        int newIdx = tempData.IndexOf(temp);
                        this.DispatchedEchelonList.Move(oldIdx, newIdx);
                    }
                    foreach (var temp in this.DispatchedEchelonList)
                        temp.isLast = false;
                    int count = this.DispatchedEchelonList.Count();
                    if (count > 0)
                        this.DispatchedEchelonList[count - 1].isLast = true;
                }
                // 인형제조
                else if (data is ProduceDollTemplate)
                {
                    List<ProduceDollTemplate> tempData;
                    switch (Config.Dashboard.sort)
                    {
                        case "remainTime":
                            tempData = this.ProduceDollList.OrderBy(o => o.TBRemainTime).ThenBy(o => o.TBSlotType).ThenBy(o => o.slot).ToList();
                            break;
                        case "slot":
                        default:
                            tempData = this.ProduceDollList.OrderBy(o => o.TBSlotType).ThenBy(o => o.slot).ToList();
                            break;
                    }
                    foreach (var temp in tempData)
                    {
                        int oldIdx = this.ProduceDollList.IndexOf(temp);
                        int newIdx = tempData.IndexOf(temp);
                        this.ProduceDollList.Move(oldIdx, newIdx);
                    }
                    foreach (var temp in this.ProduceDollList)
                        temp.isLast = false;
                    int count = this.ProduceDollList.Count();
                    if (count > 0)
                        this.ProduceDollList[count - 1].isLast = true;
                }
                /// 장비제조
                else if (data is ProduceEquipTemplate)
                {
                    List<ProduceEquipTemplate> tempData;
                    switch (Config.Dashboard.sort)
                    {
                        case "remainTime":
                            tempData = this.ProduceEquipList.OrderBy(o => o.TBRemainTime).ThenBy(o => o.TBSlotType).ThenBy(o => o.slot).ToList();
                            break;
                        case "slot":
                        default:
                            tempData = this.ProduceEquipList.OrderBy(o => o.TBSlotType).ThenBy(o => o.slot).ToList();
                            break;
                    }
                    foreach (var temp in tempData)
                    {
                        int oldIdx = this.ProduceEquipList.IndexOf(temp);
                        int newIdx = tempData.IndexOf(temp);
                        this.ProduceEquipList.Move(oldIdx, newIdx);
                    }
                    foreach (var temp in this.ProduceEquipList)
                        temp.isLast = false;
                    int count = this.ProduceEquipList.Count();
                    if (count > 0)
                        this.ProduceEquipList[count - 1].isLast = true;
                }
                /// 스킬훈련
                else if (data is SkillTrainTemplate)
                {
                    List<SkillTrainTemplate> tempData;
                    switch (Config.Dashboard.sort)
                    {
                        case "remainTime":
                            tempData = this.SkillTrainList.OrderBy(o => o.TBRemainTime).ThenBy(o => o.slotType).ThenBy(o => o.slot).ToList();
                            break;
                        case "slot":
                        default:
                            tempData = this.SkillTrainList.OrderBy(o => o.slotType).ThenBy(o => o.slot).ToList();
                            break;
                    }
                    foreach (var temp in tempData)
                    {
                        int oldIdx = this.SkillTrainList.IndexOf(temp);
                        int newIdx = tempData.IndexOf(temp);
                        this.SkillTrainList.Move(oldIdx, newIdx);
                    }
                    foreach (var temp in this.SkillTrainList)
                        temp.isLast = false;
                    int count = this.SkillTrainList.Count();
                    if (count > 0)
                        this.SkillTrainList[count - 1].isLast = true;
                }
                /// 정보분석
                else if (data is DataAnalysisTemplate)
                {
                    List<DataAnalysisTemplate> tempData;
                    switch (Config.Dashboard.sort)
                    {
                        case "remainTime":
                            tempData = this.DataAnalysisList.OrderBy(o => o.TBRemainTime).ThenBy(o => o.slot).ToList();
                            break;
                        case "slot":
                        default:
                            tempData = this.DataAnalysisList.OrderBy(o => o.slot).ToList();
                            break;
                    }
                    foreach (var temp in tempData)
                    {
                        int oldIdx = this.DataAnalysisList.IndexOf(temp);
                        int newIdx = tempData.IndexOf(temp);
                        this.DataAnalysisList.Move(oldIdx, newIdx);
                    }
                    foreach (var temp in this.DataAnalysisList)
                        temp.isLast = false;
                    int count = this.DataAnalysisList.Count();
                    if (count > 0)
                        this.DataAnalysisList[count - 1].isLast = true;
                }
                /// 수복현황
                else if (data is RestoreDollTemplate)
                {
                    List<RestoreDollTemplate> tempData;
                    switch (Config.Dashboard.sort)
                    {
                        case "remainTime":
                            tempData = this.RestoreDollList.OrderBy(o => o.TBRemainTime).ThenBy(o => o.slot).ToList();
                            break;
                        case "slot":
                        default:
                            tempData = this.RestoreDollList.OrderBy(o => o.slot).ToList();
                            break;
                    }
                    foreach (var temp in tempData)
                    {
                        int oldIdx = this.RestoreDollList.IndexOf(temp);
                        int newIdx = tempData.IndexOf(temp);
                        this.RestoreDollList.Move(oldIdx, newIdx);
                    }
                    foreach (var temp in this.RestoreDollList)
                        temp.isLast = false;
                    int count = this.RestoreDollList.Count();
                    if (count > 0)
                        this.RestoreDollList[count - 1].isLast = true;
                }
                /// 탐색현황
                else if (data is ExploreTemplate)
                {
                    List<ExploreTemplate> tempData;
                    switch (Config.Dashboard.sort)
                    {
                        case "remainTime":
                            tempData = this.ExploreList.OrderBy(o => o.TBRemainTime).ThenBy(o => o.id).ToList();
                            break;
                        case "slot":
                        default:
                            tempData = this.ExploreList.OrderBy(o => o.id).ToList();
                            break;
                    }
                    foreach (var temp in tempData)
                    {
                        int oldIdx = this.ExploreList.IndexOf(temp);
                        int newIdx = tempData.IndexOf(temp);
                        this.ExploreList.Move(oldIdx, newIdx);
                    }
                    foreach (var temp in this.ExploreList)
                        temp.isLast = false;
                    int count = this.ExploreList.Count();
                    if (count > 0)
                        this.ExploreList[count - 1].isLast = true;
                }
            });
        }

        /// <summary>
        /// 모든 알림 정렬
        /// </summary>
        public void SortAll()
        {
            Sort(new DispatchedEchleonTemplate());
            Sort(new ProduceDollTemplate());
            Sort(new ProduceEquipTemplate());
            Sort(new SkillTrainTemplate());
            Sort(new DataAnalysisTemplate());
            Sort(new RestoreDollTemplate());
            Sort(new ExploreTemplate());
        }

        /// <summary>
        /// 알림 확인
        /// </summary>
        /// <param name="data"></param>
        public void Check(Object data)
        {
            Dispatcher.Invoke(() =>
            {
                int idx = 0;
                int count = 0;
                if (data is DispatchedEchleonTemplate)
                {
                    idx = GroupIdx.DISPATCHED_ECHELON;
                    count = this.DispatchedEchelonList.Count();
                    this.DispatchedEchelonGroupBox_Count.Text = count.ToString();
                }
                else if (data is ProduceDollTemplate)
                {
                    idx = GroupIdx.PRODUCE_DOLL;
                    count = this.ProduceDollList.Count();
                    this.ProduceDollGroupBox_Count.Text = count.ToString();
                }
                else if (data is ProduceEquipTemplate)
                {
                    idx = GroupIdx.PRODUCE_EQUIP;
                    count = this.ProduceEquipList.Count();
                    this.ProduceEquipGroupBox_Count.Text = count.ToString();
                }
                else if (data is SkillTrainTemplate)
                {
                    idx = GroupIdx.SKILL_TRAIN;
                    count = this.SkillTrainList.Count();
                    this.SkillTrainGroupBox_Count.Text = count.ToString();
                }
                else if (data is DataAnalysisTemplate)
                {
                    idx = GroupIdx.DATA_ANALYSIS;
                    count = this.DataAnalysisList.Count();
                    this.DataAnalysisGroupBox_Count.Text = count.ToString();
                }
                else if (data is RestoreDollTemplate)
                {
                    idx = GroupIdx.RESTORE_DOLL;
                    count = this.RestoreDollList.Count();
                    this.RestoreDollGroupBox_Count.Text = count.ToString();
                }
                else if (data is ExploreTemplate)
                {
                    idx = GroupIdx.EXPLORE;
                    count = this.ExploreList.Count();
                    this.ExploreGroupBox_Count.Text = count.ToString();
                }

                if (count > 0)
                {
                    SetGroupVisible(idx, true);
                    if (Config.Dashboard.expand[idx])
                        SetListBoxHeight(idx, count);
                    else
                        SetListBoxHeight(idx, 0);
                    MainWindow.view.SetFilterIconEnable(idx, true);
                }
                else
                {
                    SetGroupVisible(idx, false);
                    SetListBoxHeight(idx, 0);
                    MainWindow.view.SetFilterIconEnable(idx, false);
                }
                CheckAllCount();
            });
        }

        /// <summary>
        /// 전체 알림 확인
        /// </summary>
        public void CheckAll()
        {
            Check(new DispatchedEchleonTemplate());
            Check(new ProduceDollTemplate());
            Check(new ProduceEquipTemplate());
            Check(new SkillTrainTemplate());
            Check(new DataAnalysisTemplate());
            Check(new RestoreDollTemplate());
            Check(new ExploreTemplate());
        }

        /// <summary>
        /// 전체 알림 갯수 확인
        /// </summary>
        public void CheckAllCount()
        {
            int count = 0;
            if (this.DispatchedEchelonGroupBox.IsEnabled)
                count += this.DispatchedEchelonList.Count();
            if (this.ProduceDollGroupBox.IsEnabled)
                count += this.ProduceDollList.Count();
            if (this.ProduceEquipGroupBox.IsEnabled)
                count += this.ProduceEquipList.Count();
            if (this.SkillTrainGroupBox.IsEnabled)
                count += this.SkillTrainList.Count();
            if (this.DataAnalysisGroupBox.IsEnabled)
                count += this.DataAnalysisList.Count();
            if (this.RestoreDollGroupBox.IsEnabled)
                count += this.RestoreDollList.Count();
            if (this.ExploreGroupBox.IsEnabled)
                count += this.ExploreList.Count();

            if (count > 0)
            {
                this.EmptyOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.EmptyOverlay.Visibility = Visibility.Visible;
            }

            //long nowTime = Parser.Time.GetCurrentMs();
            int nowTime = TimeUtil.GetCurrentSec();
            // 지원현황
            if (Config.Alarm.notifyDispatchedEchelonComplete)
            {
                foreach (dynamic item in this.DispatchedEchelonList.ToList())
                {
                    if (nowTime > item.endTime - Config.Extra.earlyNotifySeconds)
                    {
                        MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        return;
                    }
                }
            }
            // 인형제조
            if (Config.Alarm.notifyProduceDollComplete)
            {
                foreach (dynamic item in this.ProduceDollList.ToList())
                {
                    if (nowTime > item.endTime - Config.Extra.earlyNotifySeconds)
                    {
                        MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        return;
                    }
                }
            }
            // 장비제조
            if (Config.Alarm.notifyProduceEquipComplete)
            {
                foreach (dynamic item in this.ProduceEquipList.ToList())
                {
                    if (nowTime > item.endTime - Config.Extra.earlyNotifySeconds)
                    {
                        MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        return;
                    }
                }
            }
            // 스킬훈련
            if (Config.Alarm.notifySkillTrainComplete)
            {
                foreach (dynamic item in this.SkillTrainList.ToList())
                {
                    if (nowTime > item.endTime - Config.Extra.earlyNotifySeconds)
                    {
                        MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        return;
                    }
                }
            }
            // 정보분석
            if (Config.Alarm.notifyDataAnalysisComplete)
            {
                foreach (long endTime in this.DataAnalysisEndTimes)
                {
                    if (nowTime > endTime - Config.Extra.earlyNotifySeconds)
                    {
                        MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        return;
                    }
                }
            }
            // 수복현황
            if (Config.Alarm.notifyRestoreDollComplete)
            {
                foreach (dynamic item in this.RestoreDollList.ToList())
                {
                    if (nowTime > item.endTime - Config.Extra.earlyNotifySeconds)
                    {
                        MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        return;
                    }
                }
            }
            // 탐색현황
            if (Config.Alarm.notifyExploreComplete)
            {
                foreach (dynamic item in this.ExploreList.ToList())
                {
                    if (nowTime > item.endTime - Config.Extra.earlyNotifySeconds)
                    {
                        MainWindow.view.SetIconNotify(Menus.DASHBOARD, true);
                        return;
                    }
                }
            }
            MainWindow.view.SetIconNotify(Menus.DASHBOARD, false);
        }

        #endregion

        #region ListBoxControl

        public object Find(object data)
        {
            if (data is ProduceDollTemplate)
            {
                ProduceDollTemplate target = data as ProduceDollTemplate;
                foreach (ProduceDollTemplate item in ProduceDollList)
                {
                    if (item.slot == target.slot)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 수복현황 인형 찾기
        /// </summary>
        /// <param name="restoreSlot"></param>
        /// <returns></returns>
        public long FindRestoreDoll(int restoreSlot)
        {
            for (int i = 0; i < this.RestoreDollList.Count(); i++)
            {
                if (restoreSlot == this.RestoreDollList[i].slot)
                {
                    return this.RestoreDollList[i].gunWithUserId;
                }
            }
            return 0;
        }

        /// <summary>
        /// 스킬훈련 슬롯 가져오기
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public SkillTrainTemplate GetSlotSkillTrain(int slot, int slotType = 0)
        {
            for (int i = 0; i < this.SkillTrainList.Count(); i++)
            {
                try
                {
                    if (slot == this.SkillTrainList[i].slot && slotType == this.SkillTrainList[i].slotType)
                    {
                        return this.SkillTrainList[i];
                    }
                }
                catch(Exception ex)
                {
                    log.Error(ex, "스킬훈련 슬롯 가져오기 에러");
                }
            }
            return null;
        }

        /// <summary>
        /// 정찰현황 이벤트 남은 시간 설정
        /// </summary>
        /// <param name="eventRemainTime"></param>
        public void SetExploreEventRemainTime(int eventRemainTime)
        {
            Dispatcher.Invoke(() =>
            {
                for(int i = 0; i < this.ExploreList.Count(); i++)
                {
                    this.ExploreList[i].nextTime = eventRemainTime;
                }
            });
        }

        /// <summary>
        /// 탐색현황 새로고침
        /// </summary>
        public void RefreshExplore()
        {
            foreach (ExploreTemplate item in ExploreList)
            {
                item.RefreshReward();
            }
        }

        #endregion

        #region Chip

        /// <summary>
        /// 칩셋 모양 가져오기
        /// </summary>
        /// <param name="gridId"></param>
        /// <param name="colorId"></param>
        /// <returns></returns>
        public Canvas GetChipShape(int gridId, int colorId)
        {
            Canvas canvas = null;
            Dispatcher.Invoke(() =>
            {
                canvas = new Canvas();
                int w = 6;
                int h = 6;
                int posX = 3;
                int posY = 8;
                try
                {
                    for (int y = 0; y < chipShapes.GetLength(1); y++)
                    {
                        for (int x = 0; x < chipShapes.GetLength(2); x++)
                        {
                            if (chipShapes[gridId - 1, y, x] == 1)
                            {
                                Rectangle rect = new Rectangle();
                                rect.Width = w;
                                rect.Height = h;
                                rect.Stroke = Application.Current.Resources["ChipBorderBrush"] as System.Windows.Media.Brush;
                                rect.Fill = colorId == 1 ? Application.Current.Resources["OrangeChipBrush"] as System.Windows.Media.Brush : Application.Current.Resources["BlueChipBrush"] as System.Windows.Media.Brush;
                                Canvas.SetTop(rect, posY + h * y - y);
                                Canvas.SetLeft(rect, posX + w * x - x);
                                canvas.Children.Add(rect);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            });
            return canvas;
        }

        /// <summary>
        /// 칩셋 모양 비우기
        /// </summary>
        /// <param name="canvas"></param>
        public void ClearChipShape(Canvas canvas)
        {
            Dispatcher.Invoke(() =>
            {
                canvas.Children.Clear();
            });
        }

        /// <summary>
        /// 칩셋 모양
        /// </summary>
        private static int[,,] chipShapes = new int[,,]
        {
            // 1 1
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 2 2
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 1, 0, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 3 3I
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 1, 1, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 4 3L
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 1, 1, 0, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 5 4I
            {
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 0, 0, },
            },
            // 6 4O
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 1, 0, },
                { 1, 1, 0, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 7 4Lm
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 0, 0, },
                { 1, 1, 1, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 8 4L
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 0, 1, },
                { 1, 1, 1, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 9 4Zm
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 1, 1, },
                { 1, 1, 0, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 10 4Z
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 1, 0, },
                { 0, 1, 1, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 11 4T
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 1, 1, 1, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 12 5Pm
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 1, 1, },
                { 1, 1, 0, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 13 5P
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 1, 0, },
                { 1, 1, 1, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 14 5I
            {
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
            },
            // 15 5C
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 0, 1, },
                { 1, 1, 1, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 16 5Z
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 0, 1, },
                { 1, 1, 1, },
                { 1, 0, 0, },
                { 0, 0, 0, },
            },
            // 17 5Zm
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 0, 0, },
                { 1, 1, 1, },
                { 0, 0, 1, },
                { 0, 0, 0, },
            },
            // 18 5V
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 0, 1, },
                { 0, 0, 1, },
                { 1, 1, 1, },
                { 0, 0, 0, },
            },
            // 19 5L
            {
                { 0, 0, 0, },
                { 1, 0, 0, },
                { 1, 0, 0, },
                { 1, 0, 0, },
                { 1, 1, 0, },
                { 0, 0, 0, },
            },
            // 20 5Lm
            {
                { 0, 0, 0, },
                { 1, 1, 0, },
                { 1, 0, 0, },
                { 1, 0, 0, },
                { 1, 0, 0, },
                { 0, 0, 0, },
            },
            // 21 5W
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 0, 1, },
                { 0, 1, 1, },
                { 1, 1, 0, },
                { 0, 0, 0, },
            },
            // 22 5Nm
            {
                { 0, 0, 0, },
                { 1, 0, 0, },
                { 1, 0, 0, },
                { 1, 1, 0, },
                { 0, 1, 0, },
                { 0, 0, 0, },
            },
            // 23 5N
            {
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 1, 1, 0, },
                { 1, 0, 0, },
                { 0, 0, 0, },
            },
            // 24 5Ym
            {
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 1, 1, 0, },
                { 0, 1, 0, },
                { 0, 0, 0, },
            },
            // 25 5Y
            {
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 1, },
                { 0, 1, 0, },
                { 0, 0, 0, },
            },
            // 26 5X
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 1, 1, 1, },
                { 0, 1, 0, },
                { 0, 0, 0, },
            },
            // 27 5T
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 1, 1, 1, },
                { 0, 0, 0, },
            },
            // 28 5F
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 0, 0, },
                { 1, 1, 1, },
                { 0, 1, 0, },
                { 0, 0, 0, },
            },
            // 29 5Fm
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 1, 1, 1, },
                { 1, 0, 0, },
                { 0, 0, 0, },
            },
            // 30 6O
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 1, 1, },
                { 1, 1, 1, },
                { 0, 0, 0, },
                { 0, 0, 0, },
            },
            // 31 6A
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 1, 0, 0, },
                { 1, 1, 0, },
                { 1, 1, 1, },
                { 0, 0, 0, },
            },
            // 32 6D
            {
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 0, 1, 1, },
                { 0, 1, 1, },
                { 0, 1, 0, },
                { 0, 0, 0, },
            },
            // 33 6Z
            {
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 1, 1, 0, },
                { 1, 1, 0, },
                { 1, 0, 0, },
                { 0, 0, 0, },
            },
            // 34 6Zm
            {
                { 0, 0, 0, },
                { 1, 0, 0, },
                { 1, 1, 0, },
                { 1, 1, 0, },
                { 0, 1, 0, },
                { 0, 0, 0, },
            },
            // 35 6Y
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 1, 1, 1, },
                { 1, 0, 1, },
                { 0, 0, 0, },
            },
            // 36 6T
            {
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 1, 1, 1, },
                { 0, 1, 0, },
                { 0, 0, 0, },
            },
            // 37 6I
            {
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 0, },
            },
            // 38 6C
            {
                { 0, 0, 0, },
                { 0, 1, 1, },
                { 0, 1, 0, },
                { 0, 1, 0, },
                { 0, 1, 1, },
                { 0, 0, 0, },
            },
            // 39 6R
            {
                { 0, 0, 0, },
                { 0, 0, 0, },
                { 0, 1, 0, },
                { 1, 1, 1, },
                { 1, 1, 0, },
                { 0, 0, 0, },
            },
        };

        #endregion

        /// <summary>
        /// 뷰 초기화
        /// </summary>
        public DashboardView()
        {
            InitializeComponent();

            /// 그룹 박스
            /// 설정에 맞춰 열고 닫기
            foreach (KeyValuePair<int, string> item in GroupNms)
            {
                bool expand = Config.Dashboard.expand[item.Key];
                PackIconMaterial arrow = this.AlarmStackPanel.FindName(string.Format("{0}GroupBox_Arrow", GroupNms[item.Key])) as PackIconMaterial;
                arrow.Kind = expand == true ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;
            }

            /// 그룹 박스
            /// 설정된 위치로 이동
            int index = 0;
            while (index < GroupNms.Count())
            {
                for (int i = 0; i < Config.Dashboard.index.Length; i++)
                {
                    if (index == Config.Dashboard.index[i])
                    {
                        StackPanel itemGroup = this.AlarmStackPanel.FindName(string.Format("{0}", GroupNms[i])) as StackPanel;
                        MoveElement(index, itemGroup);
                        break;
                    }
                }
                index++;
            }

            this.DispatchedEchelonListBox.ItemsSource = this.DispatchedEchelonList;
            this.ProduceDollListBox.ItemsSource = this.ProduceDollList;
            this.ProduceEquipListBox.ItemsSource = this.ProduceEquipList;
            this.SkillTrainListBox.ItemsSource = this.SkillTrainList;
            this.DataAnalysisListBox.ItemsSource = this.DataAnalysisList;
            this.RestoreDollListBox.ItemsSource = this.RestoreDollList;
            this.ExploreListBox.ItemsSource = this.ExploreList;
        }
    }
}
