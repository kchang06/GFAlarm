using GFAlarm.Data;
using GFAlarm.Notifier;
using GFAlarm.Util;
using LocalizationResources;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace GFAlarm.View.Option
{
    /// <summary>
    /// SettingView2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();

            // 버전
            this.VersionTextBlock.Text = Config.version;

            // 언어
            this.setLanguage = Config.Setting.language;

            // 윈도우 알림
            this.useWindowToast = Config.Setting.winToast;
            this.useStrongToast = Config.Setting.strongWinToast;
            this.useVoiceNotification = Config.Setting.voiceNotification;
            this.useAdjutantVoice = Config.Setting.adjutantVoiceOnly;
            this.useStartVoice = Config.Setting.startVoice;
            this.VolumeSlider.Value = Config.Setting.voiceVolume;
            this.VolumeSlider.Minimum = 0;
            this.VolumeSlider.Maximum = 100;
            this.useSoundPlayerApi = Config.Setting.useSoundPlayerApi;
            this.useTabNotification = Config.Setting.tabNotification;

            // 메일 알림
            this.useMailNotification = Config.Setting.mailNotification;
            this.SmtpHostTextBox.Text = Config.Setting.smtpServer;
            this.FromMailAddressTextBox.Text = Config.Setting.fromMailAddress;
            this.FromMailPasswordTextBox.Text = Config.Setting.fromMailPass;
            this.ToMailAddressTextBox.Text = Config.Setting.toMailAddress;

            // 파일 저장
            this.setFileEncoding = Config.Setting.fileEncoding;
            this.useSaveUserInfo = Config.Setting.exportUserInfo;
            this.useSaveItemInfo = Config.Setting.exportItemInfo;
            this.useSaveDollInfo = Config.Setting.exportDollInfo;
            this.useSaveEquipInfo = Config.Setting.exportEquipInfo;
            this.useSaveFairyInfo = Config.Setting.exportFairyInfo;
            this.useSaveRescueDoll = Config.Setting.exportRescuedDoll;
            this.useSaveTheaterExercise = Config.Setting.exportTheaterExercise;
            this.useSaveTesterPreset = Config.Setting.exportBattleTesterPreset;

            // 윈도우
            this.useHideToTray = Config.Window.minimizeToTray;
            this.useStickyWindow = Config.Window.stickyWindow;
            this.WindowOpacitySlider.Value = Config.Window.windowOpacity;
            this.WindowOpacitySlider.Minimum = 30;
            this.WindowOpacitySlider.Maximum = 100;
            this.WindowColorTextBox.Text = Config.Window.windowColor;
            if (rgbRegex.IsMatch(Config.Window.windowColor))
            {
                this.WindowColorExampleRectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + Config.Window.windowColor));
            }

            // 기타
            this.useCheckUpdate = Config.Setting.checkUpdate;
            this.LogLevelSlider.Value = Config.Setting.logLevel;
            this.LogLevelSlider.Minimum = 0;
            this.LogLevelSlider.Maximum = 5;
            this.useLogPacket = Config.Setting.logPacket;
        }

        private void SettingViewContent_Loaded(object sender, RoutedEventArgs e)
        {
            InitLanguage();
        }

        /// <summary>
        /// 언어 새로고침
        /// </summary>
        public void InitLanguage()
        {
            if (Config.Setting.language == "ko-KR")
            {
                this.TranslatorTextBlock.Visibility = Visibility.Collapsed;
                this.TranslatorBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.TranslatorTextBlock.Visibility = Visibility.Visible;
                this.TranslatorBorder.Visibility = Visibility.Visible;
            }
            this.VoiceCommentTextBlock.Html = string.Format(LanguageResources.Instance["SETTING_TOAST_USE_VOICE_COMMENT"], Config.voice_guide_url);
            this.MailCommentTextBlock.Html = string.Format(LanguageResources.Instance["SETTING_MAIL_USE_MAIL_COMMENT"], Config.mail_guide_url);
            this.SaveUserInfoCommentTextBlock.Html = string.Format(LanguageResources.Instance["SETTING_FILE_SAVE_USER_INFO_COMMENT"], Config.chip_download_url);
            this.SaveTesterPresetCommentTextBlock.Html = string.Format(LanguageResources.Instance["SETTING_FILE_SAVE_BATTLE_TESTER_COMMENT"], Config.battle_tester_url);
            this.BugReportTextBlock.Html = string.Format(LanguageResources.Instance["PROGRAM_BUG_REPORT"], Config.latest_download_url);
        }

        #region GroupBox

        Dictionary<string, bool> expand = new Dictionary<string, bool>()
        {
            { "Language", true },
            { "WindowToast", true },
            { "Mail", true },
            { "FileSave", true },
            { "Window", true },
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
                //log.Debug("expand[{0}]={1}", name, expand[name]);
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

        /// <summary>
        /// 활성화 여부 확인
        /// </summary>
        public void Check(string group)
        {
            switch (group)
            {
                case "WindowToast":
                    // 윈도우 알림 사용
                    if (this.WindowToastCheckBox.IsChecked == true)
                    {
                        this.WindowToastGroupBox_OnOff.Foreground = Application.Current.Resources["GreenBrush"] as Brush;
                        this.WindowToastGroupBox_OnOff.Text = "ON";

                        this.StrongToastCheckBox.IsEnabled = true;
                        this.VoiceCheckBox.IsEnabled = true;
                        this.ToastTestButton.IsEnabled = true;
                        // 강한 알림 사용
                        if (this.StrongToastCheckBox.IsChecked == true)
                        {
                            this.VoiceCheckBox.IsEnabled = false;
                            this.AdjutantVoiceCheckBox.IsEnabled = false;
                            this.StartVoiceCheckBox.IsEnabled = false;
                            this.VolumeSlider.IsEnabled = false;
                            this.SoundPlayerApiCheckBox.IsEnabled = false;
                        }
                        // 음성 알림 사용
                        else if (this.VoiceCheckBox.IsChecked == true)
                        {
                            this.StrongToastCheckBox.IsEnabled = false;
                            this.AdjutantVoiceCheckBox.IsEnabled = true;
                            this.StartVoiceCheckBox.IsEnabled = true;
                            this.SoundPlayerApiCheckBox.IsEnabled = true;
                            this.ToastTestButton.IsEnabled = true;
                            if (this.SoundPlayerApiCheckBox.IsChecked == true)
                            {
                                this.VolumeSlider.IsEnabled = false;
                            }
                            else
                            {
                                this.VolumeSlider.IsEnabled = true;
                            }
                        }
                        // 둘 다 미사용
                        else
                        {
                            this.AdjutantVoiceCheckBox.IsEnabled = false;
                            this.StartVoiceCheckBox.IsEnabled = false;
                            this.VolumeSlider.IsEnabled = false;
                            this.SoundPlayerApiCheckBox.IsEnabled = false;
                        }
                    }
                    // 윈도우 알림 미사용
                    else
                    {
                        this.WindowToastGroupBox_OnOff.Foreground = Application.Current.Resources["RedBrush"] as Brush;
                        this.WindowToastGroupBox_OnOff.Text = "OFF";

                        this.StrongToastCheckBox.IsEnabled = false;
                        this.VoiceCheckBox.IsEnabled = false;
                        this.AdjutantVoiceCheckBox.IsEnabled = false;
                        this.StartVoiceCheckBox.IsEnabled = false;
                        this.VolumeSlider.IsEnabled = false;
                        this.SoundPlayerApiCheckBox.IsEnabled = false;
                        this.ToastTestButton.IsEnabled = false;
                    }
                    break;
                case "Mail":
                    if (this.MailCheckBox.IsChecked == true)
                    {
                        this.MailGroupBox_OnOff.Foreground = Application.Current.Resources["GreenBrush"] as Brush;
                        this.MailGroupBox_OnOff.Text = "ON";

                        this.SmtpHostTextBox.IsEnabled = true;
                        this.FromMailAddressTextBox.IsEnabled = true;
                        this.FromMailPasswordTextBox.IsEnabled = true;
                        this.ToMailAddressTextBox.IsEnabled = true;
                        this.MailTestButton.IsEnabled = true;
                    }
                    else
                    {
                        this.MailGroupBox_OnOff.Foreground = Application.Current.Resources["RedBrush"] as Brush;
                        this.MailGroupBox_OnOff.Text = "OFF";

                        this.SmtpHostTextBox.IsEnabled = false;
                        this.FromMailAddressTextBox.IsEnabled = false;
                        this.FromMailPasswordTextBox.IsEnabled = false;
                        this.ToMailAddressTextBox.IsEnabled = false;
                        this.MailTestButton.IsEnabled = false;
                    }
                    break;
            }
        }

        #region [그룹] 언어

        /// <summary>
        /// 언어 설정
        /// </summary>
        public string setLanguage
        {
            set
            {
                switch (value)
                {
                    case "ko-KR":
                        this.LanguageComboBox.Text = "Korean";
                        break;
                    case "zh-CN":
                        this.LanguageComboBox.Text = "Chinese";
                        break;
                    case "en-US":
                        this.LanguageComboBox.Text = "English";
                        break;
                    case "ja-JP":
                        this.LanguageComboBox.Text = "Japanese";
                        break;
                    default:
                        this.LanguageComboBox.Text = "Korean";
                        break;
                }
            }
        }
        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)(sender as ComboBox).SelectedItem;
            if (item != null)
            {
                string select = item.Content.ToString();
                switch (select)
                {
                    case "Korean":
                        select = "ko-KR";
                        break;
                    case "Chinese":
                        select = "zh-CN";
                        break;
                    case "English":
                        select = "en-US";
                        break;
                    case "Japanese":
                        select = "ja-JP";
                        break;
                    default:
                        select = "ko-KR";
                        break;
                }
                if (Config.Setting.language != select)
                {
                    this.LanguageWarningTextBlock.Visibility = Visibility.Visible;
                }
                Config.Setting.language = select;
                LanguageResources.Instance.CultureName = select;

                if (MainWindow.view != null)
                    MainWindow.view.InitLanguage();
                if (MainWindow.proxyView != null)
                    MainWindow.proxyView.InitLanguage();
                if (MainWindow.settingAlarmView != null)
                    MainWindow.settingAlarmView.InitLanguage();
                if (MainWindow.settingView != null)
                    MainWindow.settingView.InitLanguage();
            }
        }

        #endregion

        #region [그룹] 윈도우 알림

        /// <summary>
        /// 윈도우 알림 사용 여부
        /// </summary>
        public bool useWindowToast
        {
            set
            {
                Config.Setting.winToast = value;
                this.WindowToastCheckBox.IsChecked = value;
                Check("WindowToast");
            }
        }
        private void WindowToastCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useWindowToast = true;
        }
        private void WindowToastCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useWindowToast = false;
        }

        /// <summary>
        /// 강한 알림 사용 여부
        /// </summary>
        public bool useStrongToast
        {
            set
            {
                Config.Setting.strongWinToast = value;
                this.StrongToastCheckBox.IsChecked = value;
                Check("WindowToast");
            }
        }
        private void StrongToastCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useStrongToast = true;
        }
        private void StrongToastCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useStrongToast = false;
        }

        /// <summary>
        /// 음성 알림 사용 여부
        /// </summary>
        public bool useVoiceNotification
        {
            set
            {
                Config.Setting.voiceNotification = value;
                this.VoiceCheckBox.IsChecked = value;
                Check("WindowToast");
            }
        }
        private void VoiceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useVoiceNotification = true;
        }
        private void VoiceCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useVoiceNotification = false;
        }

        /// <summary>
        /// 부관 음성만
        /// </summary>
        public bool useAdjutantVoice
        {
            set
            {
                Config.Setting.adjutantVoiceOnly = value;
                this.AdjutantVoiceCheckBox.IsChecked = value;
                Check("WindowToast");
            }
        }
        private void AdjutantVoiceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useAdjutantVoice = true;
        }
        private void AdjutantVoiceCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useAdjutantVoice = false;
        }

        /// <summary>
        /// 군수/자율 출발 음성
        /// </summary>
        public bool useStartVoice
        {
            set
            {
                Config.Setting.startVoice = value;
                StartVoiceCheckBox.IsChecked = value;
                Check("WindowToast");
            }
        }
        private void StartVoiceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useStartVoice = true;
        }
        private void StartVoiceCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useStartVoice = false;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Setting.voiceVolume = (int)this.VolumeSlider.Value;
        }

        /// <summary>
        /// SoundPlayer API 사용
        /// </summary>
        public bool useSoundPlayerApi
        {
            set
            {
                Config.Setting.useSoundPlayerApi = value;
                this.SoundPlayerApiCheckBox.IsChecked = value;
                Check("WindowToast");
            }
        }
        private void SoundPlayerApiCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSoundPlayerApi = true;
        }
        private void SoundPlayerApiCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSoundPlayerApi = false;
        }

        /// <summary>
        /// 테스트 알림
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToastTestButton_Click(object sender, RoutedEventArgs e)
        {
            // 음성 알림 사용
            if (Config.Setting.voiceNotification)
            {
                string voicePath = string.Format("{0}\\Resource\\Sound\\Voice", Util.Common.GetAbsolutePath());
                if (!Directory.Exists(voicePath))
                {
                    goto NO_VOICE;
                }
                string[] voiceDirs = Directory.GetDirectories(voicePath);
                if (voiceDirs.Length <= 0)
                {
                    goto NO_VOICE;
                }
                int number = new Random().Next(0, voiceDirs.Length);

                string path = voiceDirs[number];
                path = path.Substring(path.LastIndexOf("\\") + 1).Replace("_pedo", "");
                int gunId = Parser.String.ParseInt(path);
                if (Config.Setting.adjutantVoiceOnly)
                {
                    gunId = UserData.adjutantDoll;
                }

                Notifier.Manager.notifyQueue.Enqueue(new Message()
                {
                    send = MessageSend.Toast,
                    type = MessageType.random,
                    gunId = gunId,
                    //voice = "OperationStart",
                    subject = LanguageResources.Instance["MESSAGE_TEST_SUBJECT"],
                    content = string.Format(LanguageResources.Instance["MESSAGE_TEST_WITH_VOICE_CONTENT"], GameData.Doll.GetDollName(gunId)),
                });
                return;
            }
            // 음성 알림 미사용
            // 음성 없음
            NO_VOICE:
            Notifier.Manager.notifyQueue.Enqueue(new Message()
            {
                send = MessageSend.Toast,
                type = MessageType.random,
                subject = LanguageResources.Instance["MESSAGE_TEST_SUBJECT"],
                content = LanguageResources.Instance["MESSAGE_TEST_CONTENT"],
            });
        }

        /// <summary>
        /// 탭 알림 사용
        /// </summary>
        public bool useTabNotification
        {
            set
            {
                Config.Setting.tabNotification = value;
                this.TabNotificationCheckBox.IsChecked = value;
                Check("WindowToast");
            }
        }
        private void TabNotificationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useTabNotification = true;
        }
        private void TabNotificationCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useTabNotification = false;
        }

        #endregion

        #region [그룹] 메일 알림

        /// <summary>
        /// 메일 알림 사용
        /// </summary>
        public bool useMailNotification
        {
            set
            {
                Config.Setting.mailNotification = value;
                this.MailCheckBox.IsChecked = value;
                Check("Mail");
            }
        }
        private void MailCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useMailNotification = true;
        }
        private void MailCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useMailNotification = false;
        }

        /// <summary>
        /// SMTP 주소
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmtpHostTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Config.Setting.smtpServer = this.SmtpHostTextBox.Text;
        }

        /// <summary>
        /// 보낼 메일 주소
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromMailAddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Config.Setting.fromMailAddress = this.FromMailAddressTextBox.Text;
        }

        /// <summary>
        /// 보낼 메일 앱 비밀번호
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromMailPasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Config.Setting.fromMailPass = this.FromMailPasswordTextBox.Text;
        }

        /// <summary>
        /// 받을 메일 주소
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToMailAddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Config.Setting.toMailAddress = this.ToMailAddressTextBox.Text;
        }

        /// <summary>
        /// 테스트 메일
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MailTestButton_Click(object sender, RoutedEventArgs e)
        {
            Notifier.Manager.notifyQueue.Enqueue(new Message()
            {
                type = MessageType.other,
                send = MessageSend.Mail,
                subject = LanguageResources.Instance["MESSAGE_TEST_SUBJECT"],
                content = LanguageResources.Instance["MESSAGE_TEST_CONTENT"],
            });
        }

        #endregion

        #region [그룹] 파일 저장

        /// <summary>
        /// 파일 인코딩 설정
        /// </summary>
        public string setFileEncoding
        {
            set
            {
                switch (value)
                {
                    case "Default":
                        this.FileEncodingComboBox.Text = "Default";
                        break;
                    case "UTF-8":
                        this.FileEncodingComboBox.Text = "UTF-8";
                        break;
                    default:
                        this.FileEncodingComboBox.Text = "Default";
                        break;
                }
            }
        }
        private void FileEncodingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)(sender as ComboBox).SelectedItem;
            if (item != null)
            {
                string select = item.Content.ToString();
                switch (select)
                {
                    case "UTF-8":
                        Config.Setting.fileEncoding = "UTF-8";
                        break;
                    case "Default":
                        Config.Setting.fileEncoding = "Default";
                        break;
                }
            }
        }

        /// <summary>
        /// 사용자 정보 저장 사용
        /// </summary>
        public bool useSaveUserInfo
        {
            set
            {
                Config.Setting.exportUserInfo = value;
                this.SaveUserInfoCheckBox.IsChecked = value;
            }
        }
        private void SaveUserInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSaveUserInfo = true;
        }
        private void SaveUserInfoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSaveUserInfo = false;
        }

        /// <summary>
        /// 아이템 정보 저장 사용
        /// </summary>
        public bool useSaveItemInfo
        {
            set
            {
                Config.Setting.exportItemInfo = value;
                this.SaveItemInfoCheckBox.IsChecked = value;
            }
        }
        private void SaveItemInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSaveItemInfo = true;
        }
        private void SaveItemInfoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSaveItemInfo = false;
        }

        /// <summary>
        /// 인형 정보 저장 사용
        /// </summary>
        public bool useSaveDollInfo
        {
            set
            {
                Config.Setting.exportDollInfo = value;
                this.SaveDollInfoCheckBox.IsChecked = value;
            }
        }
        private void SaveDollInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSaveDollInfo = true;
        }

        private void SaveDollInfoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSaveDollInfo = false;
        }

        /// <summary>
        /// 장비 정보 저장 사용
        /// </summary>
        public bool useSaveEquipInfo
        {
            set
            {
                Config.Setting.exportEquipInfo = value;
                this.SaveEquipInfoCheckBox.IsChecked = value;
            }
        }
        private void SaveEquipInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSaveEquipInfo = true;
        }
        private void SaveEquipInfoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSaveEquipInfo = false;
        }

        /// <summary>
        /// 요정 정보 저장 사용
        /// </summary>
        public bool useSaveFairyInfo
        {
            set
            {
                Config.Setting.exportFairyInfo = value;
                this.SaveFairyInfoCheckBox.IsChecked = value;
            }
        }
        private void SaveFairyInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSaveFairyInfo = true;
        }
        private void SaveFairyInfoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSaveFairyInfo = false;
        }

        /// <summary>
        /// 획득 인형 저장 사용
        /// </summary>
        public bool useSaveRescueDoll
        {
            set
            {
                Config.Setting.exportRescuedDoll = value;
                this.SaveRescueDollCheckBox.IsChecked = value;
            }
        }
        private void SaveRescueDollCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSaveRescueDoll = true;
        }
        private void SaveRescueDollCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSaveRescueDoll = false;
        }

        /// <summary>
        /// 국지전 웨이브 저장 사용
        /// </summary>
        public bool useSaveTheaterExercise
        {
            set
            {
                Config.Setting.exportTheaterExercise = value;
                this.SaveTheaterExerciseCheckBox.IsChecked = value;
            }
        }
        private void SaveTheaterExerciseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSaveTheaterExercise = true;
        }
        private void SaveTheaterExerciseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSaveTheaterExercise = false;
        }

        /// <summary>
        /// 테스터 프리셋 저장 사용
        /// </summary>
        public bool useSaveTesterPreset
        {
            set
            {
                Config.Setting.exportBattleTesterPreset = value;
                this.SaveTesterPresetCheckBox.IsChecked = value;
            }
        }
        private void SaveTesterPresetCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSaveTesterPreset = true;
        }
        private void SaveTesterPresetCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSaveTesterPreset = false;
        }

        #endregion

        #region [그룹] 윈도우

        /// <summary>
        /// 트레이 숨김 사용
        /// </summary>
        public bool useHideToTray
        {
            set
            {
                Config.Window.minimizeToTray = value;
                this.HideToTrayCheckBox.IsChecked = value;
            }
        }
        private void HideToTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useHideToTray = true;
        }
        private void HideToTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useHideToTray = false;
        }

        /// <summary>
        /// 창 자석 사용
        /// </summary>
        public bool useStickyWindow
        {
            set
            {
                Config.Window.stickyWindow = value;
                this.StickyWindowCheckBox.IsChecked = value;
            }
        }
        private void StickyWindowCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useStickyWindow = true;
        }
        private void StickyWindowCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useStickyWindow = false;
        }

        /// <summary>
        /// 창 투명도
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.Window.windowOpacity = (int)e.NewValue;
            if (MainWindow.view != null)
                MainWindow.view.WindowOpacity = Config.Window.windowOpacity;
            if (SubWindow.view != null)
                SubWindow.view.WindowOpacity = Config.Window.windowOpacity;
        }

        /// <summary>
        /// 창 색상
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string rgb = this.WindowColorTextBox.Text.Trim();
            if (Config.Window.windowColor != rgb)
            {
                this.WindowColorWarningTextBlock.Visibility = Visibility.Visible;
            }
            if (rgbRegex.IsMatch(rgb))
            {
                this.WindowColorExampleRectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + rgb));
                Config.Window.windowColor = rgb;
            }
        }
        private Regex rgbRegex = new Regex("^(?:[0-9a-fA-F]{3}){1,2}$");

        #endregion

        #region [그룹] 기타

        /// <summary>
        /// 업데이트 확인 사용
        /// </summary>
        public bool useCheckUpdate
        {
            set
            {
                Config.Setting.checkUpdate = value;
                this.CheckUpdateCheckBox.IsChecked = value;
            }
        }
        private void CheckUpdateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useCheckUpdate = true;
        }
        private void CheckUpdateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useCheckUpdate = false;
        }

        /// <summary>
        /// 로그 레벨 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogLevelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            App.LogLevel = Convert.ToInt32(e.NewValue);
            Config.Setting.logLevel = Convert.ToInt32(e.NewValue);
        }

        /// <summary>
        /// 패킷 로그 사용
        /// </summary>
        public bool useLogPacket
        {
            set
            {
                Config.Setting.logPacket = value;
                this.LogPacketCheckBox.IsChecked = value;
            }
        }
        private void LogPacketCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useLogPacket = true;
        }
        private void LogPacketCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useLogPacket = false;
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
