using GFAlarm.Data;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using LocalizationResources;
using MahApps.Metro.IconPacks;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace GFAlarm.View.Menu
{
    /// <summary>
    /// 제대 상태
    /// </summary>
    public enum EchelonStatus
    {
        Logistics,
        AutoBattle,
        Mission,
        StandBy
    }

    /// <summary>
    /// EchelonView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EchelonView : UserControl
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 제대
        /// </summary>
        public Dictionary<int, ObservableCollection<EchelonTemplate>> echelons = new Dictionary<int, ObservableCollection<EchelonTemplate>>()
        {
            { 1, new ObservableCollection<EchelonTemplate>() },
            { 2, new ObservableCollection<EchelonTemplate>() },
            { 3, new ObservableCollection<EchelonTemplate>() },
            { 4, new ObservableCollection<EchelonTemplate>() },
            { 5, new ObservableCollection<EchelonTemplate>() },
            { 6, new ObservableCollection<EchelonTemplate>() },
            { 7, new ObservableCollection<EchelonTemplate>() },
            { 8, new ObservableCollection<EchelonTemplate>() },
            { 9, new ObservableCollection<EchelonTemplate>() },
            { 10, new ObservableCollection<EchelonTemplate>() },
        };

        /// <summary>
        /// 제대원 설정
        /// </summary>
        /// <param name="echelon"></param>
        public void Set(EchelonTemplate echelon)
        {
            long id = 0;
            if (echelon.gunWithUserId > 0)
                id = echelon.gunWithUserId;
            else if (echelon.fairyWithUserId > 0)
                id = echelon.fairyWithUserId;
            Set(echelon.teamId, echelon.location, id);
        }

        /// <summary>
        /// 제대원 설정
        /// </summary>
        /// <param name="member"></param>
        public void Set(int teamId, int location, long id)
        {
            if (1 > teamId || teamId > 10)
                return;
            if (1 > location || location > 7)
                return;
            if (id == 0)
                return;

            //log.Info("제대 {0} 순번 {1} 인형 {2} 설정", teamId, location, id);

            //Dispatcher.Invoke(() =>
            //{
            /// 요정
            if (location == 6)
            {
                /// 이전에 다른 제대 소속되어 있었다면?
                int beforeTeamId = 0, beforeLocation = 0;
                FindId(ref beforeTeamId, ref beforeLocation, id);
                if (beforeTeamId > 0)
                {
                    Remove(beforeTeamId, 6);
                }
                /// 설정 위치에 이미 다른 제대원이 있다면?
                long alreadyId = 0;
                FindId(teamId, location, ref alreadyId);
                if (alreadyId > 0 && id != alreadyId)
                {
                    Remove(teamId, location);
                }
                Dispatcher.Invoke(() =>
                {
                    echelons[teamId].Add(new EchelonTemplate()
                    {
                        fairyWithUserId = id,
                        teamId = teamId,
                        location = location,
                    });
                    UserData.Fairy.dictionary[id].Refresh();
                });
            }
            /// 인형
            else
            {
                /// 이전에 다른 제대 소속되어 있었다면?
                int beforeTeamId = 0, beforeLocation = 0;
                FindId(ref beforeTeamId, ref beforeLocation, id);
                if (beforeTeamId > 0)
                {
                    Remove(beforeTeamId, beforeLocation);
                }
                /// 설정 위치에 이미 다른 제대원이 있다면?
                long alreadyId = 0;
                FindId(teamId, location, ref alreadyId);
                if (alreadyId > 0 && id != alreadyId)
                {
                    Remove(teamId, location);
                }
                Dispatcher.Invoke(() =>
                {
                    echelons[teamId].Add(new EchelonTemplate()
                    {
                        gunWithUserId = id,
                        teamId = teamId,
                        location = location,
                    });
                    UserData.Doll.dictionary[id].Refresh();
                });
            }

            Check(teamId);
            Update(teamId);
            Sort(teamId);
            //});
        }
        
        /// <summary>
        /// 제대원 제거
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="location"></param>
        public void Remove(int teamId, int location)
        {
            if (1 > teamId || teamId > 10)
                return;
            if (1 > location || location > 7)
                return;

            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < echelons[teamId].Count(); i++)
                {
                    if (echelons[teamId][i].location == location)
                    {
                        echelons[teamId].RemoveAt(i);
                    }
                }
            });

            Check(teamId);
            Update(teamId);
            Sort(teamId);
        }

        /// <summary>
        /// 제대 인원 확인
        /// </summary>
        /// <param name="teamId"></param>
        public void Check(int teamId)
        {
            if (1 > teamId || teamId > 10)
                return;

            int count = echelons[teamId].Count();
            if (count > 0)
                SetGroupVisible(teamId, true);
            else
                SetGroupVisible(teamId, false);
            SetListBoxHeight(teamId, count);
        }
        /// <summary>
        /// 전체 제대 인원 확인
        /// </summary>
        public void CheckAll()
        {
            for (int i = 1; i <= 10; i++)
            {
                Check(i);
            }
        }

        /// <summary>
        /// 제대 업데이트
        /// </summary>
        /// <param name="teamId"></param>
        public void Update(int teamId, bool forceUpdate = false)
        {
            if (1 > teamId || teamId > 10)
                return;

            Dispatcher.Invoke(() =>
            {
                foreach (EchelonTemplate echelon in echelons[teamId])
                {
                    echelon.Refresh(forceUpdate);
                }
            });
        }
        public void Update(int teamId, int location, bool forceUpdate = false)
        {
            if (1 > teamId || teamId > 10)
                return;
            if (1 > location || location > 7)
                return;

            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < echelons[teamId].Count(); i++)
                {
                    if (echelons[teamId][i].location == location)
                    {
                        echelons[teamId][i].Refresh(forceUpdate);
                    }
                }
            });
        }

        /// <summary>
        /// 전체 제대 업데이트
        /// </summary>
        public void UpdateAll(bool forceUpdate = false)
        {
            for (int i = 1; i <= 10; i++)
            {
                Update(i, forceUpdate);
            }
        }

        /// <summary>
        /// 제대 정렬
        /// </summary>
        /// <param name="teamId"></param>
        public void Sort(int teamId)
        {
            if (1 > teamId || teamId > 10)
                return;

            Dispatcher.Invoke(() =>
            {
                var tempData = echelons[teamId].OrderBy(o => o.location).ToList();
                foreach (var temp in tempData)
                {
                    int oldIdx = echelons[teamId].IndexOf(temp);
                    int newIdx = tempData.IndexOf(temp);
                    echelons[teamId].Move(oldIdx, newIdx);
                }
                foreach (var temp in echelons[teamId])
                    temp.isLast = false;
                int count = this.echelons[teamId].Count();
                if (count > 0)
                    this.echelons[teamId][count - 1].isLast = true;
            });
        }

        /// <summary>
        /// 제대 비우기
        /// </summary>
        /// <param name="teamId"></param>
        public void Clear(int teamId)
        {
            if (1 > teamId || teamId > 10)
                return;

            Dispatcher.Invoke(() =>
            {
                if (echelons.ContainsKey(teamId))
                {
                    for (int i = 0; i < echelons[teamId].Count(); i++)
                    {
                        echelons[teamId].RemoveAt(i);
                    }
                    echelons[teamId].Clear();
                }
                else
                {
                    log.Warn("제대 {0} 를 찾을 수 없음 - 제대 비우기 실패");
                }
            });
        }
        /// <summary>
        /// 전체 제대 비우기
        /// </summary>
        public void ClearAll()
        {
            for (int i = 1; i <= 10; i++)
            {
                Clear(i);
            }
        }

        /// <summary>
        /// 제대 교환
        /// </summary>
        /// <param name="teamA"></param>
        /// <param name="teamB"></param>
        public void ExchangeTeam(int teamA, int teamB)
        {
            Dispatcher.Invoke(() =>
            {
                List<EchelonTemplate> tempTeamA = new List<EchelonTemplate>(echelons[teamA].ToList());
                List<EchelonTemplate> tempTeamB = new List<EchelonTemplate>(echelons[teamB].ToList());

                echelons[teamA].Clear();
                echelons[teamB].Clear();

                foreach (EchelonTemplate member in tempTeamA)
                    echelons[teamB].Add(member);
                foreach (EchelonTemplate member in tempTeamB)
                    echelons[teamA].Add(member);
            });

            Sort(teamA);
            Sort(teamB);

            Check(teamA);
            Check(teamB);

            Update(teamA);
            Update(teamB);
        }

        /// <summary>
        /// 제대 찾기
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public List<long> FindTeam(int teamId, bool exceptFairy = false)
        {
            List<long> teamMembers = new List<long>();
            if (echelons != null && echelons.ContainsKey(teamId))
            {
                foreach (EchelonTemplate echelon in echelons[teamId])
                {
                    if (echelon != null)
                    {
                        long gunWithUserId = echelon.gunWithUserId;
                        long fairyWithUserId = echelon.fairyWithUserId;
                        if (gunWithUserId > 0)
                            teamMembers.Add(gunWithUserId);
                        else if (fairyWithUserId > 0 && exceptFairy == false)
                            teamMembers.Add(fairyWithUserId);
                    }
                }
            }
            else
            {
                log.Warn("제대 {0} 를 찾을 수 없음 - 친구 제대 또는 확인되지 않은 제대", teamId);
            }
            return teamMembers;
        }

        /// <summary>
        /// 제대, 제대순번으로
        /// 인형 / 요정 ID 찾기
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public void FindId(int teamId, int location, ref long id)
        {
            if (1 > teamId || teamId > 10)
            {
                id = 0;
                return;
            }
            if (1 > location || location > 7)
            {
                id = 0;
                return;
            }

            foreach (EchelonTemplate echelon in echelons[teamId])
            {
                if (echelon.location == location)
                {
                    if (echelon.gunWithUserId > 0)
                        id = echelon.gunWithUserId;
                    else if (echelon.fairyWithUserId > 0)
                        id = echelon.fairyWithUserId;
                    return;
                }
            }
            id = 0;
            //log.Info("제대 탐색 {0} {1} {2}", teamId, location, id);
        }
        /// <summary>
        /// 인형 / 요정 ID로
        /// 제대, 제대순번 찾기
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="location"></param>
        /// <param name="id"></param>
        public void FindId(ref int teamId, ref int location, long id)
        {
            for (int i = 1; i <= 10; i++)
            {
                foreach (EchelonTemplate echelon in echelons[i])
                {
                    if (echelon.gunWithUserId == id || echelon.fairyWithUserId == id)
                    {
                        teamId = echelon.teamId;
                        location = echelon.location;
                    }
                }
            }
        }

        /// <summary>
        /// 제대 리더 찾기
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public long FindTeamLeaderId(int teamId)
        {
            if (1 > teamId || teamId > 10)
                return 0;

            foreach (EchelonTemplate echelon in echelons[teamId])
            {
                if (echelon.location == 1)
                {
                    if (echelon.gunWithUserId > 0)
                        return echelon.gunWithUserId;
                }
            }
            return 0;
        }

        /// <summary>
        /// 리스트 박스 크기 변경
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="count"></param>
        public void SetListBoxHeight(int idx, int count)
        {
            Dispatcher.Invoke(() =>
            {
                Border groupBox = this.FindName(string.Format("Echelon{0}GroupBox", idx)) as Border;
                ListBox listBox = this.FindName(string.Format("Echelon{0}ListBox", idx)) as ListBox;
                if (groupBox != null && listBox != null)
                {
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
                    //listBox.BeginAnimation(ListBox.HeightProperty, null);

                    if (Config.Echelon.expand[idx - 1] == false || groupBox.IsEnabled == false)
                    {
                        toHeight = 0;
                    }

                    ChangeHeight.From = fromHeight;
                    ChangeHeight.To = toHeight;
                    //if (toHeight == 0)
                    //{
                    //    ChangeHeight.Completed += (sender, args) =>
                    //    {
                    //        listBox.Visibility = Visibility.Collapsed;
                    //    };
                    //}
                    listBox.BeginAnimation(ListBox.HeightProperty, ChangeHeight);
                }
            });
        }

        /// <summary>
        /// 그룹 박스 보이기/숨기기
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="visible"></param>
        public void SetGroupVisible(int idx, bool visible)
        {
            Dispatcher.Invoke(() =>
            {
                Border groupBox = this.FindName(string.Format("Echelon{0}GroupBox", idx)) as Border;

                //log.Debug("제대 그룹 {0} {1}", idx, visible == true ? "보임" : "숨김");

                if (groupBox.IsEnabled == false)
                {
                    visible = false;
                }
                //groupBox.BeginAnimation(Grid.OpacityProperty, null);

                switch (visible)
                {
                    case true:
                        //if (groupBox.Visibility == Visibility.Visible)
                        //{
                        //    groupBox.Opacity = 1;
                        //}
                        //else
                        //{
                            //DoubleAnimation FadeIn = new DoubleAnimation
                            //{
                            //    From = 0,
                            //    To = 1,
                            //    Duration = TimeSpan.FromSeconds(0.2)
                            //};
                            groupBox.Visibility = Visibility.Visible;
                            //groupBox.Opacity = 0;
                            //groupBox.BeginAnimation(Grid.OpacityProperty, FadeIn);
                        //}
                        //groupBox.Visibility = Visibility.Visible;
                        break;
                    case false:
                        //if (groupBox.Visibility != Visibility.Collapsed)
                        //{
                        //DoubleAnimation FadeOut = new DoubleAnimation
                        //{
                        //    From = 1,
                        //    To = 0,
                        //    Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                        //};
                        //FadeOut.Completed += (sender, args) =>
                        //{
                        //    groupBox.Visibility = Visibility.Collapsed;
                        //};
                        groupBox.Visibility = Visibility.Collapsed;
                        //groupBox.Opacity = 1;
                        //groupBox.BeginAnimation(Grid.OpacityProperty, FadeOut);
                        //}
                        //groupBox.Visibility = Visibility.Collapsed;
                        break;
                }
            });
        }

        /// <summary>
        /// 그룹 박스 모두 펴기/접기
        /// </summary>
        /// <param name="collapse"></param>
        public void ExpandAllGroup(bool collapse=false)
        {
            for (int i = 1; i <= 10; i++)
            {
                if (collapse)
                    Config.Echelon.expand[i - 1] = false;
                else
                    Config.Echelon.expand[i - 1] = true;

                bool expand = Config.Echelon.expand[i - 1];

                // 그룹 박스 화살표
                PackIconMaterial groupBoxArrow = this.FindName(string.Format("Echelon{0}GroupBox_Arrow", i)) as PackIconMaterial;
                groupBoxArrow.Kind = expand == true ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;

                int count = expand == true ? echelons[i].Count() : 0;
                SetListBoxHeight(i, count);
            }
        }

        /// <summary>
        /// 그룹 박스 접기 가능 여부
        /// </summary>
        public bool isCollapsibleGroup
        {
            get
            {
                for (int i = 1; i <= 10; i++)
                {
                    if (Config.Echelon.expand[i - 1] == true)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 그룹 박스 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string name = (sender as Border).Name;

            string idxString = string.Concat(name.Where(char.IsDigit));
            int idx = Parser.String.ParseInt(idxString);

            Config.Echelon.expand[idx - 1] = Config.Echelon.expand[idx - 1] == true ? false : true;
            bool expand = Config.Echelon.expand[idx - 1];

            // 그룹 박스 화살표
            PackIconMaterial groupBoxArrow = this.FindName(string.Format("Echelon{0}GroupBox_Arrow", idx)) as PackIconMaterial;
            groupBoxArrow.Kind = expand == true ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;

            // 리스트 박스 크기
            int count = expand == true ? echelons[idx].Count() : 0;
            SetListBoxHeight(idx, count);

            MainWindow.view.CheckExpandCollapseButtonStatus();
        }

        /// <summary>
        /// 제대 상태 설정
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="status"></param>
        public void SetTeamStatus(int teamId, EchelonStatus status)
        {
            Dispatcher.Invoke(() =>
            {
                Border statusBorder = this.FindName(string.Format("Echelon{0}GroupBox_StatusBorder", teamId)) as Border;
                TextBlock statusTextBlock = this.FindName(string.Format("Echelon{0}GroupBox_StatusTextBlock", teamId)) as TextBlock;

                switch (status)
                {
                    case EchelonStatus.Logistics:
                        statusBorder.Visibility = Visibility.Visible;
                        statusBorder.Background = Application.Current.Resources["EchelonLogisticsBrush"] as Brush;
                        statusTextBlock.Text = LanguageResources.Instance["LOGISTICS_SHORT"];
                        break;
                    case EchelonStatus.AutoBattle:
                        statusBorder.Visibility = Visibility.Visible;
                        statusBorder.Background = Application.Current.Resources["EchelonAutoBattleBrush"] as Brush;
                        statusTextBlock.Text = LanguageResources.Instance["AUTO_BATTLE_SHORT"];
                        break;
                    case EchelonStatus.Mission:
                        statusBorder.Visibility = Visibility.Visible;
                        statusBorder.Background = Application.Current.Resources["EchelonMissionBrush"] as Brush;
                        statusTextBlock.Text = LanguageResources.Instance["MISSION_SHORT"];
                        break;
                    case EchelonStatus.StandBy:
                        statusBorder.Visibility = Visibility.Collapsed;
                        break;
                }
            });
        }

        /// <summary>
        /// 뷰 초기화
        /// </summary>
        public EchelonView()
        {
            InitializeComponent();

            // TODO: 제대 탭 유동 추가 (동작하지 않음)
            //Grid templateGroupBox = this.FindName(string.Format("Echelon{0}GroupBox", 1)) as Grid;
            //string templateGroupBoxXaml = XamlWriter.Save(templateGroupBox);
            //ListBox templateListBox = this.FindName(string.Format("Echelon{0}ListBox", 1)) as ListBox;
            //string templateListBoxXaml = XamlWriter.Save(templateListBox);

            //for (int i = 2; i <= 10; i++)
            //{
            //    string newGroupBoxXaml = templateGroupBoxXaml.Replace("Echelon1", "Echelon" + i).Replace("Numeric1", "Numeric" + i);
            //    log.Info("new_group_box_xaml {0}", newGroupBoxXaml);
            //    StringReader sr = new StringReader(newGroupBoxXaml);
            //    XmlReader xr = XmlReader.Create(sr);
            //    Grid newGroupBox = XamlReader.Load(xr) as Grid;
            //    this.EchelonStackPanel.Children.Add(newGroupBox);

            //    string newListBoxXaml = templateListBoxXaml.Replace("Echelon1", "Echelon" + i);
            //    log.Info("new_list_box_xaml {0}", newListBoxXaml);
            //    sr = new StringReader(newListBoxXaml);
            //    xr = XmlReader.Create(sr);
            //    ListBox newListBox = XamlReader.Load(xr) as ListBox;
            //    this.EchelonStackPanel.Children.Add(newListBox);
            //}

            // 그룹 박스
            // 리스트 박스
            for (int i = 1; i <= 10; i++)
            {
                bool expand = Config.Echelon.expand[i - 1];
                // 그룹 박스 화살표
                PackIconMaterial arrow = this.EchelonStackPanel.FindName(string.Format("Echelon{0}GroupBox_Arrow", i)) as PackIconMaterial;
                arrow.Kind = expand == true ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;

                ListBox listbox = this.EchelonStackPanel.FindName(string.Format("Echelon{0}ListBox", i)) as ListBox;
                listbox.ItemsSource = echelons[i];
            }
        }

        /// <summary>
        /// 존재하지 않음 오버레이 보이기 여부
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisibleEmptyOverlay(bool visible)
        {
            Dispatcher.Invoke(() =>
            {
                this.EmptyOverlay.Visibility = visible == true ? Visibility.Visible : Visibility.Collapsed;
            });
        }
    }
}
