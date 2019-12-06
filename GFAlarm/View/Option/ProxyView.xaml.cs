using GFAlarm.Constants;
using GFAlarm.Proxy;
using GFAlarm.Transaction;
using GFAlarm.Util;
using LocalizationResources;
using MahApps.Metro.IconPacks;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
    /// ProxyView2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProxyView : UserControl
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        //string proxyIp = "0.0.0.0";     // 프록시 주소
        //string proxyPort = "0";         // 프록시 포트번호

        //string pacIp = "0.0.0.0";       // PAC 주소
        //string pacPort = "0";           // PAC 포트번호

        public ProxyView()
        {
            InitializeComponent();
            
            if (App.isElevated)
            {
                this.PacAdminPrivilegeTextBlock.Foreground = Application.Current.Resources["GreenBrush"] as Brush;
                this.PacAdminPrivilegeTextBlock.Text = LanguageResources.Instance["SETTING_PAC_SERVER_NOW_ADMIN"];
                this.PacPortNumberTextBox.IsEnabled = true;
            }
            else
            {
                this.PacAdminPrivilegeTextBlock.Foreground = Application.Current.Resources["RedBrush"] as Brush;
                this.PacAdminPrivilegeTextBlock.Text = LanguageResources.Instance["SETTING_PAC_SERVER_NEED_ADMIN"];
                this.PacPortNumberTextBox.IsEnabled = false;
            }

            // 프록시 서버
            this.useStartup = Config.Proxy.startup;
            if (Config.Proxy.startup)
            {
                this.useProxyServer = true;
            }
            // PAC 서버
            this.usePac = Config.Proxy.usePac;
            this.PacDomainTextBox.Text = Config.Proxy.pacDomain;
            this.PacPortNumberTextBox.Text = Config.Proxy.pacPort.ToString();
            this.useDecryptSsl = Config.Proxy.decryptSsl;
            this.PortNumberTextBox.Text = Config.Proxy.port.ToString();
            // 경유 프록시
            this.useUpstreamProxy = Config.Proxy.upstreamProxy;
            this.UpstreamProxyHostTextBox.Text = Config.Proxy.upstreamHost;
            this.UpstreamProxyPortTextBox.Text = Config.Proxy.upstreamPort.ToString();
            // 패킷 변조
            this.useRandomAdjutant = Config.Adjutant.useRandomAdjutant;
            this.setRandomAdjutant = "0,0,0,0";
            try
            {
                this.DollWhitelistTextBox.Text = string.Join(",", Config.Adjutant.dollWhitelist.Select(x => x.ToString()));
                this.DollBlacklistTextBox.Text = string.Join(",", Config.Adjutant.dollBlacklist.Select(x => x.ToString()));
                this.SkinWhitelistTextBox.Text = string.Join(",", Config.Adjutant.skinWhitelist.Select(x => x.ToString()));
                this.SkinBlacklistTextBox.Text = string.Join(",", Config.Adjutant.skinBlacklist.Select(x => x.ToString()));
            }
            catch { }
            this.setAdjutantSkin = Config.Adjutant.adjutantSkinCategory;
            this.setAdjutantShape = Config.Adjutant.adjutantShape;

            HttpController http = new HttpController();

            // 언어 새로고침
            InitLanguage();

            this.Loaded += ProxyView2_Loaded;
        }

        private void ProxyView2_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 언어 새로고침
        /// </summary>
        public void InitLanguage()
        {
            if (Config.isLoaded)
            {
                Dispatcher.Invoke(() =>
                {
                    if (!string.IsNullOrEmpty(Config.decrypt_ssl_guide_url))
                        this.SslDecryptCommentTextBlock.Html = string.Format(LanguageResources.Instance["SETTING_PROXY_SERVER_USE_SSL_DECRYPT_COMMENT"],
                                                                                Config.decrypt_ssl_guide_url);
                    if (!string.IsNullOrEmpty(Config.uncensor_patch_url))
                        this.UncensorModeCommentTextBlock.Html = string.Format(LanguageResources.Instance["SETTING_PACKET_FORGER_USE_UNLOCK_CENSOR_COMMENT"],
                                                                                Config.uncensor_patch_url);
                    //if (!string.IsNullOrEmpty(Config.random_adjutant_guide_url))
                    //    this.RandomAdjutantCommentTextBlock.Html = string.Format(LanguageResources.Instance["SETTING_PACKET_FORGER_USE_RANDOM_ADJUTANT_COMMENT"],
                    //                                                            Config.random_adjutant_guide_url);

                    if (App.isElevated)
                        this.PacAdminPrivilegeTextBlock.Text = LanguageResources.Instance["SETTING_PAC_SERVER_NOW_ADMIN"];
                    else
                        this.PacAdminPrivilegeTextBlock.Text = LanguageResources.Instance["SETTING_PAC_SERVER_NEED_ADMIN"];
                });
            }
        }

        /// <summary>
        /// 활성화 여부 확인
        /// </summary>
        public void Check(string group)
        {
            switch (group)
            {
                case "Proxy":
                    if (this.ProxyServerCheckBox.IsChecked == true)
                    {
                        this.PortNumberTextBox.IsEnabled = false;
                        this.SslDecryptCheckBox.IsEnabled = false;

                        this.UsePacCheckBox.IsEnabled = false;
                        this.PacDomainTextBox.IsEnabled = false;
                        this.PacPortNumberTextBox.IsEnabled = false;

                        this.UpstreamProxyCheckBox.IsEnabled = false;
                        this.UpstreamProxyHostTextBox.IsEnabled = false;
                        this.UpstreamProxyPortTextBox.IsEnabled = false;

                        this.ProxyGuideBorder.Visibility = Visibility.Visible;
                        this.ProxyGuide.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.PortNumberTextBox.IsEnabled = true;
                        this.SslDecryptCheckBox.IsEnabled = true;

                        if (App.isElevated)
                        {
                            this.UsePacCheckBox.IsEnabled = true;
                            this.PacDomainTextBox.IsEnabled = true;
                            this.PacPortNumberTextBox.IsEnabled = true;
                        }

                        this.UpstreamProxyCheckBox.IsEnabled = true;
                        this.UpstreamProxyHostTextBox.IsEnabled = true;
                        this.UpstreamProxyPortTextBox.IsEnabled = true;

                        this.ProxyGuideBorder.Visibility = Visibility.Collapsed;
                        this.ProxyGuide.Visibility = Visibility.Collapsed;
                    }
                    break;
                case "PacketForger":
                    if (this.RandomAdjutantCheckBox.IsChecked == true)
                    {
                        this.AdjutantSkinComboBox.IsEnabled = true;
                        this.DollWhitelistTextBox.IsEnabled = true;
                        this.SkinWhitelistTextBox.IsEnabled = true;
                        this.DollBlacklistTextBox.IsEnabled = true;
                        this.SkinBlacklistTextBox.IsEnabled = true;
                        this.AdjutantShapeComboBox.IsEnabled = true;
                    }
                    else
                    {
                        this.AdjutantSkinComboBox.IsEnabled = false;
                        this.DollWhitelistTextBox.IsEnabled = false;
                        this.SkinWhitelistTextBox.IsEnabled = false;
                        this.DollBlacklistTextBox.IsEnabled = false;
                        this.SkinBlacklistTextBox.IsEnabled = false;
                        this.AdjutantShapeComboBox.IsEnabled = false;
                    }
                    if (this.useUncensorMode || this.useRandomAdjutant)
                    {
                        this.PacketForgerGroupBox_Content.Foreground = Application.Current.Resources["GreenBrush"] as Brush;
                        this.PacketForgerGroupBox_Content.Text = "ON";
                    }
                    else
                    {
                        this.PacketForgerGroupBox_Content.Foreground = Application.Current.Resources["RedBrush"] as Brush;
                        this.PacketForgerGroupBox_Content.Text = "OFF";
                    }
                    break;
            }
        }

        #region 그룹박스

        Dictionary<string, bool> expand = new Dictionary<string, bool>()
        {
            { "ProxyServer", true },
            { "ProxyAutoConfig", true },
            { "PacServer", true },
            { "UpstreamProxy", true },
            { "PacketForger", true },
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

        #region [그룹] 프록시 서버

        /// <summary>
        /// 프록시 서버 사용여부
        /// </summary>
        public bool useProxyServer
        {
            set
            {
                if (_useProxyServer == value)
                    return;
                bool tempValue = value;
                if (value)
                {
                    bool proxySuccess = ProxyController.instance.Start();
                    bool pacSuccess = false;
                    if (Config.Proxy.usePac)
                    {
                        pacSuccess = HttpController.instance.Start();
                    }

                    if (!proxySuccess)
                    {
                        //log.Warn("proxy server failed");
                        this.ProxyServerGroupBox_Content.Foreground = Application.Current.Resources["RedBrush"] as Brush;
                        this.ProxyServerGroupBox_Content.Text = "FAILED";
                        this.PacServerGroupBox_Content.Foreground = Application.Current.Resources["RedBrush"] as Brush;
                        this.PacServerGroupBox_Content.Text = "FAILED";
                    }
                    else
                    {
                        if (pacSuccess)
                        {
                            string port = Config.Proxy.pacPort.ToString();
                            this.ProxyServerGroupBox_Content.Foreground = Application.Current.Resources["GreenBrush"] as Brush;
                            this.ProxyServerGroupBox_Content.Text = string.Format("{0}:{1}", Config.ip, Config.Proxy.port);
                            this.PacServerGroupBox_Content.Foreground = Application.Current.Resources["GreenBrush"] as Brush;
                            string pacAddress = "";
                            if (!string.IsNullOrEmpty(Config.Proxy.pacDomain))
                                pacAddress = string.Format("{0}:{1}/{2}", Config.Proxy.pacDomain, port, "GFPAC.js");
                            else 
                                pacAddress = string.Format("{0}:{1}/{2}", Config.ip, port, "GFPAC.js");
                            this.PacServerGroupBox_Content.Text = pacAddress;
                        }
                        else
                        {
                            //log.Warn("http server failed");
                            string port = Config.Proxy.port.ToString();
                            this.ProxyServerGroupBox_Content.Foreground = Application.Current.Resources["GreenBrush"] as Brush;
                            this.ProxyServerGroupBox_Content.Text = string.Format("{0}:{1}", Config.ip, Config.Proxy.port);
                            this.PacServerGroupBox_Content.Foreground = Application.Current.Resources["RedBrush"] as Brush;
                            this.PacServerGroupBox_Content.Text = "OFF";
                        }
                    }
                    tempValue = proxySuccess;
                }
                else
                {
                    this.ProxyServerGroupBox_Content.Foreground = Application.Current.Resources["RedBrush"] as Brush;
                    this.ProxyServerGroupBox_Content.Text = "OFF";
                    this.PacServerGroupBox_Content.Foreground = Application.Current.Resources["RedBrush"] as Brush;
                    this.PacServerGroupBox_Content.Text = "OFF";
                    ProxyController.instance.Stop();
                    HttpController.instance.Stop();
                }
                _useProxyServer = tempValue;
                this.ProxyServerCheckBox.IsChecked = tempValue;
                Check("Proxy");
            }
        }
        private bool _useProxyServer = false;
        private void ProxyServerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useProxyServer = true;
        }
        private void ProxyServerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useProxyServer = false;
        }

        /// <summary>
        /// 자동시작 사용여부
        /// </summary>
        public bool useStartup
        {
            set
            {
                this.AutoStartCheckBox.IsChecked = value;
                Config.Proxy.startup = value;
            }
        }
        private void AutoStartCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useStartup = true;
        }
        private void AutoStartCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useStartup = false;
        }

        /// <summary>
        /// 포트번호 수정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortNumberTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Config.Proxy.port = Parser.String.ParseInt(this.PortNumberTextBox.Text);
        }

        /// <summary>
        /// PAC 서버 사용 여부
        /// </summary>
        public bool usePac
        {
            set
            {
                this.UsePacCheckBox.IsChecked = value;
                Config.Proxy.usePac = value;
            }
        }
        private void UsePacCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.usePac = true;
        }
        private void UsePacCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.usePac = false;
        }

        /// <summary>
        /// PAC 서버 도베인 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PacDomainTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Config.Proxy.pacDomain = (sender as TextBox).Text;
        }

        /// <summary>
        /// PAC 서버 포트번호 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PacPortNumberTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Config.Proxy.pacPort = Parser.String.ParseInt(this.PacPortNumberTextBox.Text);
        }

        /// <summary>
        /// SSL 복호화 사용여부
        /// </summary>
        public bool useDecryptSsl
        {
            set
            {
                this.SslDecryptCheckBox.IsChecked = value;
                Config.Proxy.decryptSsl = value;
            }
        }
        private void SslDecryptCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useDecryptSsl = true;
        }
        private void SslDecryptCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useDecryptSsl = false;
        }

        /// <summary>
        /// 프록시 사용 가이드 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProxyGuide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.subView.SetContent(Menus.PROXY_GUIDE);
            MainWindow.subView.Show();
        }

        #endregion

        #region [그룹] 경유 프록시

        /// <summary>
        /// 경유 프록시 사용여부
        /// </summary>
        public bool useUpstreamProxy
        {
            set
            {
                bool tempValue = Config.Proxy.upstreamProxy;
                this.UpstreamProxyCheckBox.IsChecked = value;
                //this.UpstreamProxyHostTextBox.IsEnabled = value == true ? false : true;
                //this.UpstreamProxyPortTextBox.IsEnabled = value == true ? false : true;
                Config.Proxy.upstreamProxy = value;
                if (value)
                {
                    this.UpstreamProxyGroupBox_Content.Foreground = Application.Current.Resources["GreenBrush"] as Brush;
                    this.UpstreamProxyGroupBox_Content.Text = string.Format("{0}:{1}", Config.Proxy.upstreamHost, Config.Proxy.upstreamPort);
                }
                else
                {
                    this.UpstreamProxyGroupBox_Content.Foreground = Application.Current.Resources["RedBrush"] as Brush;
                    this.UpstreamProxyGroupBox_Content.Text = "OFF";
                }
            }
        }
        private void UpstreamProxyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useUpstreamProxy = true;
        }

        private void UpstreamProxyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useUpstreamProxy = false;
        }

        /// <summary>
        /// 경유 프록시 호스트 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpstreamProxyHostTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Config.Proxy.upstreamHost = this.UpstreamProxyHostTextBox.Text;
        }

        /// <summary>
        /// 경유 프록시 포트 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpstreamProxyPortTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Config.Proxy.upstreamPort = Parser.String.ParseInt(this.UpstreamProxyPortTextBox.Text);
        }

        #endregion

        #region [그룹] 패킷 변조

        /// <summary>
        /// 검열해제 사용여부
        /// </summary>
        public bool useUncensorMode
        {
            get
            {
                return _useUncensorMode;
            }
            set
            {
                this.UncensorModeCheckBox.IsChecked = value;
                Config.Extra.unlockCensorMode = value;
                _useUncensorMode = value;
                Check("PacketForger");
            }
        }
        private bool _useUncensorMode = false;
        private void UncensorModeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useUncensorMode = true;
        }
        private void UncensorModeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useUncensorMode = false;
        }

        /// <summary>
        /// 랜덤 부관 사용여부
        /// </summary>
        public bool useRandomAdjutant
        {
            get
            {
                return _useRandomAdjutant;
            }
            set
            {
                this.RandomAdjutantCheckBox.IsChecked = value;
                this.AdjutantShapeComboBox.IsEnabled = value;
                Config.Adjutant.useRandomAdjutant = value;
                _useRandomAdjutant = value;
                Check("PacketForger");
            }
        }
        private bool _useRandomAdjutant = false;
        private void RandomAdjutantCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useRandomAdjutant = true;
        }
        private void RandomAdjutantCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useRandomAdjutant = false;
        }

        /// <summary>
        /// 설정된 부관 표시
        /// </summary>
        public string setRandomAdjutant
        {
            set
            {
                Dispatcher.Invoke(() =>
                {
                    string[] adjutant = value.Split(',');
                    if (adjutant.Length == 4)
                    {
                        this.RandomAdjutantCommentTextBlock.Html = string.Format(
                            LanguageResources.Instance["SETTING_PACKET_FORGER_USE_RANDOM_ADJUTANT_COMMENT"],
                            adjutant[0], adjutant[1]);
                    }
                });
            }
        }

        /// <summary>
        /// 스킨 설정
        /// </summary>
        public string setAdjutantSkin
        {
            set
            {
                int select = 5;
                switch (value)
                {
                    case "normal":
                        select = 0;
                        break;
                    case "skin":
                        select = 1;
                        break;
                    case "live2d_skin":
                        select = 2;
                        break;
                    case "child_skin":
                        select = 3;
                        break;
                    case "mod_skin":
                        select = 4;
                        break;
                    case "random":
                        select = 5;
                        break;
                }
                this.AdjutantSkinComboBox.SelectedIndex = select;
                Config.Adjutant.adjutantSkinCategory = value;
            }
        }
        private void AdjutantSkinComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = (sender as ComboBox).SelectedIndex;
            string select = "";
            switch (index)
            {
                case 0:
                    select = "normal";
                    break;
                case 1:
                    select = "skin";
                    break;
                case 2:
                    select = "live2d_skin";
                    break;
                case 3:
                    select = "child_skin";
                    break;
                case 4:
                    select = "mod_skin";
                    break;
                case 5:
                    select = "random";
                    break;
                default:
                    select = "random";
                    break;
            }
            this.setAdjutantSkin = select;
        }

        /// <summary>
        /// 중상 설정
        /// </summary>
        public int setAdjutantShape
        {
            set
            {
                this.AdjutantShapeComboBox.SelectedIndex = value;
                Config.Adjutant.adjutantShape = value;
            }
        }
        private void AdjutantShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = (sender as ComboBox).SelectedIndex;
            switch (index)
            {
                case 0:     // 통상만
                    this.setAdjutantShape = 0;
                    break;
                case 1:     // 중상만
                    this.setAdjutantShape = 1;
                    break;
                case 2:     // 랜덤(모두)
                    this.setAdjutantShape = 2;
                    break;
            }
        }

        /// <summary>
        /// 부관 화이트리스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DollWhitelistTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string value = this.DollWhitelistTextBox.Text;
                if (string.IsNullOrEmpty(value))
                    Config.Adjutant.dollWhitelist = new int[] { };
                else
                    Config.Adjutant.dollWhitelist = this.DollWhitelistTextBox.Text.Split(',').Select(Int32.Parse).ToArray();
            }
            catch { }
        }

        /// <summary>
        /// 부관 블랙리스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DollBlacklistTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string value = this.DollBlacklistTextBox.Text;
                if (string.IsNullOrEmpty(value))
                    Config.Adjutant.dollBlacklist = new int[] { };
                else
                    Config.Adjutant.dollBlacklist = this.DollBlacklistTextBox.Text.Split(',').Select(Int32.Parse).ToArray();
            }
            catch { }
        }

        /// <summary>
        /// 스킨 화이트리스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkinWhitelistTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string value = this.SkinWhitelistTextBox.Text;
                if (string.IsNullOrEmpty(value))
                    Config.Adjutant.skinWhitelist = new int[] { };
                else
                    Config.Adjutant.skinWhitelist = this.SkinWhitelistTextBox.Text.Split(',').Select(Int32.Parse).ToArray();
            }
            catch { }
        }

        /// <summary>
        /// 스킨 블랙리스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkinBlacklistTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string value = this.SkinBlacklistTextBox.Text;
                if (string.IsNullOrEmpty(value))
                    Config.Adjutant.skinBlacklist = new int[] { };
                else
                    Config.Adjutant.skinBlacklist = this.SkinBlacklistTextBox.Text.Split(',').Select(Int32.Parse).ToArray();
            }
            catch { }
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
