using GFAlarm.Data;
using GFAlarm.Util;
using GFAlarm.View.DataTemplate;
using LocalizationResources;
using MahApps.Metro.IconPacks;
using NLog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GFAlarm.View.Menu
{
    /// <summary>
    /// this.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class QuestView : UserControl
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public ObservableCollection<QuestTemplate> questList = new ObservableCollection<QuestTemplate>();
        public ObservableCollection<QuestTemplate> weeklyList = new ObservableCollection<QuestTemplate>();
        public ObservableCollection<QuestTemplate> researchList = new ObservableCollection<QuestTemplate>();

        public static class LIST
        {
            public const int DAILY = 0;
            public const int RESEARCH = 1;
            public const int WEEKLY = 2;

            public const int Length = 3;
        }

        /// <summary>
        /// 리스트 이름
        /// </summary>
        public static Dictionary<int, string> listNames = new Dictionary<int, string>()
        {
            { LIST.DAILY, "Quest" },
            { LIST.RESEARCH, "Research" },
            { LIST.WEEKLY, "Weekly" },
        };

        public QuestView()
        {
            InitializeComponent();

            this.QuestListBox.ItemsSource = this.questList;
            this.WeeklyListBox.ItemsSource = this.weeklyList;
            this.ResearchListBox.ItemsSource = this.researchList;
        }

        /// <summary>
        /// 임무 초기화
        /// (일간, 주간)
        /// </summary>
        public void Init()
        {
            this.Clear();

            Dispatcher.Invoke(() =>
            {
                // 일간임무
                this.Add(new QuestTemplate() { id = 101 });
                this.Add(new QuestTemplate() { id = 102 });
                this.Add(new QuestTemplate() { id = 103 });
                this.Add(new QuestTemplate() { id = 104 });
                this.Add(new QuestTemplate() { id = 105 });
                this.Add(new QuestTemplate() { id = 106 });
                this.Add(new QuestTemplate() { id = 107 });
                this.Add(new QuestTemplate() { id = 108 });
                this.Add(new QuestTemplate() { id = 109 });
                this.Add(new QuestTemplate() { id = 110 });
                this.Add(new QuestTemplate() { id = 111 });
                this.Add(new QuestTemplate() { id = 112 });
                this.Add(new QuestTemplate() { id = 113 });
                this.Add(new QuestTemplate() { id = 115 });
                this.Add(new QuestTemplate() { id = 116 });
                this.Add(new QuestTemplate() { id = 117 });
                this.Add(new QuestTemplate() { id = 118 });
                this.Add(new QuestTemplate() { id = 125 });
                this.Add(new QuestTemplate() { id = 126 });
                this.Add(new QuestTemplate() { id = 127 });
                this.Add(new QuestTemplate() { id = 128 });
                this.Add(new QuestTemplate() { id = 129 });
                this.Add(new QuestTemplate() { id = 130 });
                // 주간임무
                this.Add(new QuestTemplate() { id = 201 });
                this.Add(new QuestTemplate() { id = 202 });
                this.Add(new QuestTemplate() { id = 203 });
                this.Add(new QuestTemplate() { id = 204 });
                this.Add(new QuestTemplate() { id = 205 });
                this.Add(new QuestTemplate() { id = 206 });
                this.Add(new QuestTemplate() { id = 207 });
                this.Add(new QuestTemplate() { id = 208 });
                this.Add(new QuestTemplate() { id = 209 });
                this.Add(new QuestTemplate() { id = 210 });
                this.Add(new QuestTemplate() { id = 211 });
                this.Add(new QuestTemplate() { id = 212 });
                this.Add(new QuestTemplate() { id = 213 });
                this.Add(new QuestTemplate() { id = 214 });
                this.Add(new QuestTemplate() { id = 215 });
                this.Add(new QuestTemplate() { id = 216 });
                this.Add(new QuestTemplate() { id = 217 });
                this.Add(new QuestTemplate() { id = 218 });
                this.Add(new QuestTemplate() { id = 219 });
                this.Add(new QuestTemplate() { id = 220 });
                this.Add(new QuestTemplate() { id = 221 });
            });
        }

        /// <summary>
        /// 임무 추가
        /// </summary>
        /// <param name="data"></param>
        public void Add(QuestTemplate data)
        {
            Dispatcher.Invoke(() =>
            {
                if (data.id >= 300)
                {
                    this.researchList.Add(data);
                    this.ResearchCount.Text = this.researchList.Count().ToString();
                }
                else if (data.id >= 200)
                {
                    this.weeklyList.Add(data);
                    this.WeeklyCount.Text = this.weeklyList.Count().ToString();
                }
                else if (data. id >= 100)
                {
                    this.questList.Add(data);
                    this.QuestCount.Text = this.questList.Count().ToString();
                }
                Sort();
                CheckAll();
            });
        }

        /// <summary>
        /// 임무 횟수 추가
        /// </summary>
        /// <param name="type"></param>
        public void AddCount(string type)
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < this.questList.Count(); i++)
                {
                    if (this.questList[i].type == type)
                        this.questList[i].count++;
                }
                for (int i = 0; i < this.weeklyList.Count(); i++)
                {
                    if (this.weeklyList[i].type == type)
                        this.weeklyList[i].count++;
                }
                for (int i = 0; i < this.researchList.Count(); i++)
                {
                    if (this.researchList[i].type == type)
                        this.researchList[i].count++;
                }

                bool done = false;
                while (done == false)
                {
                    bool removed = false;
                    for (int i = 0; i < this.questList.Count(); i++)
                    {
                        if (this.questList[i].TBIsCompleted == "1")
                        {
                            this.questList.RemoveAt(i);
                            removed = true;
                            break;
                        }
                    }
                    if (removed)
                        continue;
                    done = true;
                }
                done = false;
                while (done == false)
                {
                    bool removed = false;
                    for (int i = 0; i < this.weeklyList.Count(); i++)
                    {
                        if (this.weeklyList[i].TBIsCompleted == "1")
                        {
                            this.weeklyList.RemoveAt(i);
                            removed = true;
                            break;
                        }
                    }
                    if (removed)
                        continue;
                    done = true;
                }
                done = false;
                while (done == false)
                {
                    bool removed = false;
                    for (int i = 0; i < this.researchList.Count(); i++)
                    {
                        if (this.researchList[i].TBIsCompleted == "1")
                        {
                            this.researchList.RemoveAt(i);
                            removed = true;
                            break;
                        }
                    }
                    if (removed)
                        continue;
                    done = true;
                }
            });
        }

        /// <summary>
        /// 임무 횟수 설정
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        public void SetCount(string type, string cycle, int count)
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < this.questList.Count(); i++)
                {
                    if (this.questList[i].type == type && this.questList[i].cycle == cycle)
                        this.questList[i].count = count;
                }
                for (int i = 0; i < this.weeklyList.Count(); i++)
                {
                    if (this.weeklyList[i].type == type && this.weeklyList[i].cycle == cycle)
                        this.weeklyList[i].count = count;
                }
                for (int i = 0; i < this.researchList.Count(); i++)
                {
                    if (this.researchList[i].type == type && this.researchList[i].cycle == cycle)
                        this.researchList[i].count = count;
                }
                CheckQuestComplete();
            });
        }
        public void SetCount(int id, int count)
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < this.questList.Count(); i++)
                {
                    if (this.questList[i].id == id)
                    {
                        this.questList[i].count = count;
                        break;
                    }
                }
                for (int i = 0; i < this.weeklyList.Count(); i++)
                {
                    if (this.weeklyList[i].id == id)
                    {
                        this.weeklyList[i].count = count;
                        break;
                    }
                }
                for (int i = 0; i < this.researchList.Count(); i++)
                {
                    if (this.researchList[i].id == id)
                    {
                        this.researchList[i].count = count;
                        break;
                    }
                }
                CheckQuestComplete();
            });
        }

        /// <summary>
        /// 임무 제거
        /// </summary>
        /// <param name="data"></param>
        public void Remove(QuestTemplate data)
        {
            Remove(data.id);
        }
        public void Remove(int id)
        {
            log.Debug("임무 제거 시도... {0}", id);
            Dispatcher.Invoke(() =>
            {
                if (id >= 300)
                {
                    for (int i = 0; i < this.researchList.Count(); i++)
                    {
                        log.Debug("임무 확인 중... {0}", this.researchList[i].id);
                        if (this.researchList[i].id == id)
                        {
                            log.Debug("임무 발견 {0}", id);
                            this.researchList.RemoveAt(i);
                            this.ResearchCount.Text = this.researchList.Count().ToString();
                            break;
                        }
                    }
                }
                else if (id >= 200)
                {
                    for (int i = 0; i < this.weeklyList.Count(); i++)
                    {
                        log.Debug("임무 확인 중... {0}", this.weeklyList[i].id);
                        if (this.weeklyList[i].id == id)
                        {
                            log.Debug("임무 발견 {0}", id);
                            this.weeklyList.RemoveAt(i);
                            this.WeeklyCount.Text = this.weeklyList.Count().ToString();
                            break;
                        }
                    }
                }
                else if (id >= 100)
                {
                    for (int i = 0; i < this.questList.Count(); i++)
                    {
                        log.Debug("임무 확인 중... {0}", this.questList[i].id);
                        if (this.questList[i].id == id)
                        {
                            log.Debug("임무 발견 {0}", id);
                            this.questList.RemoveAt(i);
                            this.QuestCount.Text = this.questList.Count().ToString();
                            break;
                        }
                    }
                }
                Sort();
                CheckAll();
            });
        }

        /// <summary>
        /// 임무 비우기
        /// </summary>
        public void Clear()
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < this.questList.Count(); i++)
                {
                    this.questList.RemoveAt(i);
                }
                this.questList.Clear();
                for (int i = 0; i < this.weeklyList.Count(); i++)
                {
                    this.weeklyList.RemoveAt(i);
                }
                this.weeklyList.Clear();
            });
        }

        /// <summary>
        /// 정보임무 비우기
        /// </summary>
        public void ClearResearch()
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < this.researchList.Count(); i++)
                {
                    this.researchList.RemoveAt(i);
                }
                this.researchList.Clear();
            });
        }

        public void ClearAll()
        {
            Clear();
            ClearResearch();
        }

        /// <summary>
        /// 임무 달성 확인
        /// </summary>
        public void CheckQuestComplete()
        {
            Dispatcher.Invoke(() =>
            {
                bool done = false;
                while (done == false)
                {
                    bool removed = false;
                    for (int i = 0; i < this.questList.Count(); i++)
                    {
                        if (this.questList[i].TBIsCompleted == "1")
                        {
                            this.questList.RemoveAt(i);
                            removed = true;
                            break;
                        }
                    }
                    if (removed)
                        continue;
                    done = true;
                }
                done = false;
                while (done == false)
                {
                    bool removed = false;
                    for (int i = 0; i < this.weeklyList.Count(); i++)
                    {
                        if (this.weeklyList[i].TBIsCompleted == "1")
                        {
                            this.weeklyList.RemoveAt(i);
                            removed = true;
                            break;
                        }
                    }
                    if (removed)
                        continue;
                    done = true;
                }
                done = false;
                while (done == false)
                {
                    bool removed = false;
                    for (int i = 0; i < this.researchList.Count(); i++)
                    {
                        if (this.researchList[i].TBIsCompleted == "1")
                        {
                            this.researchList.RemoveAt(i);
                            removed = true;
                            break;
                        }
                    }
                    if (removed)
                        continue;
                    done = true;
                }
                CheckAll();
            });
        }

        /// <summary>
        /// 임무 달성 확인
        /// </summary>
        /// <param name="id"></param>
        public void CheckQuestComplete(int id)
        {
            if (id > 300)
            {
                for (int i = 0; i < this.researchList.Count(); i++)
                {
                    if (this.researchList[i].id == id)
                    {
                        if (this.researchList[i].TBIsCompleted == "1")
                            this.researchList.RemoveAt(i);
                        break;
                    }
                }
            }
            else if (id > 200)
            {
                for (int i = 0; i < this.weeklyList.Count(); i++)
                {
                    if (this.weeklyList[i].id == id)
                    {
                        if (this.weeklyList[i].TBIsCompleted == "1")
                            this.weeklyList.RemoveAt(i);
                        break;
                    }
                }
            }
            else if (id > 100)
            {
                for (int i = 0; i < this.questList.Count(); i++)
                {
                    if (this.questList[i].id == id)
                    {
                        if (this.questList[i].TBIsCompleted == "1")
                            this.questList.RemoveAt(i);
                        break;
                    }
                }
            }
            CheckAll();
        }

        /// <summary>
        /// 임무 갯수 확인
        /// </summary>
        public void CheckAll()
        {
            Dispatcher.Invoke(() =>
            {
                int count = 0;
                if (this.questList.Count() > 0)
                {
                    count += this.questList.Count();
                    SetGroupVisible(LIST.DAILY, true);
                    if (expandQuestGroup[LIST.DAILY])
                        SetListBoxHeight(LIST.DAILY, this.questList.Count());
                    else
                        SetListBoxHeight(LIST.DAILY, 0);
                }
                else
                {
                    SetGroupVisible(LIST.DAILY, false);
                    SetListBoxHeight(LIST.DAILY, 0);
                }
                if (this.weeklyList.Count() > 0)
                {
                    count += this.weeklyList.Count();
                    SetGroupVisible(LIST.WEEKLY, true);
                    if (expandQuestGroup[LIST.WEEKLY])
                        SetListBoxHeight(LIST.WEEKLY, this.weeklyList.Count());
                    else
                        SetListBoxHeight(LIST.WEEKLY, 0);
                }
                else
                {
                    SetGroupVisible(LIST.WEEKLY, false);
                    SetListBoxHeight(LIST.WEEKLY, 0);
                }
                if (this.researchList.Count() > 0)
                {
                    count += this.researchList.Count();
                    SetGroupVisible(LIST.RESEARCH, true);
                    if (expandQuestGroup[LIST.RESEARCH])
                        SetListBoxHeight(LIST.RESEARCH, this.researchList.Count());
                    else
                        SetListBoxHeight(LIST.RESEARCH, 0);
                }
                else
                {
                    SetGroupVisible(LIST.RESEARCH, false);
                    SetListBoxHeight(LIST.RESEARCH, 0);
                }

                if (UserData.Quest.isOpenQuestMenu)
                {
                    this.EmptyOverlay.Visibility = Visibility.Collapsed;
                    if (count > 0)
                        this.CompleteOverlay.Visibility = Visibility.Collapsed;
                    else
                        this.CompleteOverlay.Visibility = Visibility.Visible;
                }
                else
                {
                    this.EmptyOverlay.Visibility = Visibility.Visible;
                }
            });
        }

        /// <summary>
        /// 임무 정렬
        /// </summary>
        public void Sort()
        {
            Dispatcher.Invoke(() =>
            {
                List<QuestTemplate> questListData = this.questList.OrderBy(o => o.TBIsCompleted).ThenBy(o => o.TBQuestId).ToList();
                foreach (QuestTemplate temp in questListData)
                {
                    int oldIdx = this.questList.IndexOf(temp);
                    int newIdx = questListData.IndexOf(temp);
                    this.questList.Move(oldIdx, newIdx);
                }
                List<QuestTemplate> weeklyListData = this.weeklyList.OrderBy(o => o.TBIsCompleted).ThenBy(o => o.TBQuestId).ToList();
                foreach (QuestTemplate temp in weeklyListData)
                {
                    int oldIdx = this.weeklyList.IndexOf(temp);
                    int newIdx = weeklyListData.IndexOf(temp);
                    this.weeklyList.Move(oldIdx, newIdx);
                }
                List<QuestTemplate> researchListData = this.researchList.OrderBy(o => o.TBIsCompleted).ThenBy(o => o.TBQuestId).ToList();
                foreach (QuestTemplate temp in researchListData)
                {
                    int oldIdx = this.researchList.IndexOf(temp);
                    int newIdx = researchListData.IndexOf(temp);
                    this.researchList.Move(oldIdx, newIdx);
                }
            });

        }

        /// <summary>
        /// 그룹 박스 보이기/숨기기
        /// </summary>
        /// <param name="list"></param>
        /// <param name="visible"></param>
        public void SetGroupVisible(int list, bool visible)
        {
            Border groupBox = this.FindName(string.Format("{0}GroupBox", listNames[list])) as Border;

            /// 그룹 박스가 비활성화 상태면 안 보이게
            if (groupBox.IsEnabled == false)
                visible = false;

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

        bool[] expandQuestGroup = new bool[] { true, true, true };

        /// <summary>
        /// 리스트 박스 크기 변경
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="count"></param>
        public void SetListBoxHeight(int idx, int count)
        {
            Border groupBox = this.FindName(string.Format("{0}GroupBox", listNames[idx])) as Border;
            ListBox listBox = this.FindName(string.Format("{0}ListBox", listNames[idx])) as ListBox;

            listBox.BeginAnimation(ListBox.HeightProperty, null);

            double fromHeight = listBox.ActualHeight;
            double toHeight = count * 40;

            if (expandQuestGroup[idx] == false || groupBox.IsEnabled == false)
                toHeight = 0;

            Animations.ChangeHeight.From = fromHeight;
            Animations.ChangeHeight.To = toHeight;

            listBox.BeginAnimation(ListBox.HeightProperty, Animations.ChangeHeight);
        }

        /// <summary>
        /// 그룹 박스 모두 펴기/접기
        /// </summary>
        /// <param name="collapse"></param>
        public void ExpandAllGroup(bool collapse=false)
        {
            this.QuestArrow.Kind = collapse == false ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;
            this.WeeklyArrow.Kind = collapse == false ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;
            this.ResearchArrow.Kind = collapse == false ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;

            for (int i = 0; i < this.expandQuestGroup.Length; i++)
                this.expandQuestGroup[i] = collapse == false ? true : false;

            SetListBoxHeight(LIST.DAILY, collapse == false ? this.questList.Count() : 0);
            SetListBoxHeight(LIST.WEEKLY, collapse == false ? this.weeklyList.Count() : 0);
            SetListBoxHeight(LIST.RESEARCH, collapse == false ? this.researchList.Count() : 0);
        }

        /// <summary>
        /// 그룹 박스 접기 가능 여부
        /// </summary>
        public bool isCollapsibleGroup
        {
            get
            {
                for (int i = 0; i < this.expandQuestGroup.Length; i++)
                {
                    if (this.expandQuestGroup[i] == true)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 그룹 박스 마우스 떼기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string name = (sender as Border).Name;

            int idx = 0;
            int count = 0;
            bool expand = true;

            switch (name)
            {
                case "QuestGroupBox":
                    idx = LIST.DAILY;
                    count = this.questList.Count();
                    break;
                case "WeeklyGroupBox":
                    idx = LIST.WEEKLY;
                    count = this.weeklyList.Count();
                    break;
                case "ResearchGroupBox":
                    idx = LIST.RESEARCH;
                    count = this.researchList.Count();
                    break;
            }

            expandQuestGroup[idx] = expandQuestGroup[idx] == true ? false : true;
            expand = expandQuestGroup[idx];
            count = expand == true ? count : 0;

            SetListBoxHeight(idx, count);

            PackIconMaterial arrow = this.FindName(string.Format("{0}Arrow", listNames[idx])) as PackIconMaterial;
            if (arrow != null)
                arrow.Kind = expand == true ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;

            MainWindow.view.CheckExpandCollapseButtonStatus();
        }
    }
}
