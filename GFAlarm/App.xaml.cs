using GFAlarm.Util;
using LocalizationResources;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GFAlarm
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>The unique mutex name.</summary>
        private string UniqueMutexName = Config.Window.mutexGuid;
        //private string UniqueMutexName = "f720414e-da0d-42fc-9090-33420b489254";

        /// <summary>The event wait handle.</summary>
        //private EventWaitHandle eventWaitHandle;

        /// <summary>The mutex.</summary>
        private Mutex mutex;

        /// <summary>Administator Privilege</summary>
        public static bool isElevated = false;

        private void AppOnStartup(object sender, StartupEventArgs e)
        {
            /// TextBlock 기본 스타일
            /// DPI에 따라 폰트 스타일 변경하기
            //Style textBlockBaseStyle = new Style
            //{
            //    TargetType = typeof(TextBlock)
            //};

            //textBlockBaseStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, View.Colors.White));
            //if (Util.Common.GetDpiScale() <= 1.0)
            //    textBlockBaseStyle.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily("Dotum")));
            //else
            //    textBlockBaseStyle.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily("Segoe UI,Malgun Gothic")));
            //textBlockBaseStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, (double)12.0));
            //Application.Current.Resources["TextBlockBaseStyle"] = textBlockBaseStyle;

            /// 로그 설정
            var config = new LoggingConfiguration();

            // console log
            /*
            var logconsole = new ColoredConsoleTarget("logconsole");
            logconsole.Layout = new SimpleLayout() { Text = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} ${level:uppercase=true:padding=-5} [${logger}] ${message} ${exception:format=tostring}" };
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logconsole);
            */

            var logfile = new FileTarget("logfile");
            logfile.FileName = "${basedir}/Logs/${date:format=yyyy-MM-dd}.log";
            logfile.Layout = new SimpleLayout() { Text = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} ${level:uppercase=true:padding=-5} [${logger}] ${message} ${exception:format=tostring}" };
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logfile);

            LogManager.Configuration = config;

            App.LogLevel = Config.Setting.logLevel;

            /// 언어 체크
            LanguageResources.Instance.CultureName = Config.Setting.language;

            /// 버전 체크
            Config.TryLoadVersion();
            if (Config.Setting.checkUpdate)
            {
                if (Config.IsLatestVersion() == false)
                {
                    MessageBoxResult choose = MessageBox.Show(
                        string.Format(LanguageResources.Instance["VERSION_CHECK_CONTENT"], Config.latest_version), LanguageResources.Instance["VERSION_CHECK_TITLE"],
                        MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
                    if (choose == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Config.latest_download_url);
                    }
                }
            }

            /// 툴팁 지속시간 무한
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            /// 창 색상 설정
            string color = Config.Window.windowColor;
            Regex colorRegex = new Regex("^(?:[0-9a-fA-F]{3}){1,2}$");
            if (colorRegex.Match(color).Success)
            {
                //MessageBox.Show("is color hex");
                Color primaryColor1 = (Color)ColorConverter.ConvertFromString("#FF" + color);
                Color primaryColor2 = (Color)ColorConverter.ConvertFromString("#AA" + color);
                Color primaryColor3 = (Color)ColorConverter.ConvertFromString("#00" + color);

                // 색상 변경
                SolidColorBrush primaryBrush1 = new SolidColorBrush(primaryColor1);
                SolidColorBrush primaryBrush2 = new SolidColorBrush(primaryColor2);
                Application.Current.Resources["PrimaryBrush"] = primaryBrush1;
                Application.Current.Resources["PrimaryDeactiveBrush"] = primaryBrush2;
                Application.Current.Resources["WindowActiveBrush"] = primaryBrush1;
                Application.Current.Resources["WindowDeactiveBrush"] = primaryBrush2;
            }

            // Administrator Privilege Check
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            /// 에러 핸들러
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;

            bool isOwned = false;
            this.mutex = new Mutex(true, UniqueMutexName, out isOwned);
            if (!isOwned)
            {
                this.Shutdown();
            }
        }

        /// <summary>
        /// 로그 레벨 설정
        /// </summary>
        public static int LogLevel
        {
            set
            {
                try
                {
                    foreach (var rule in LogManager.Configuration.LoggingRules)
                        rule.DisableLoggingForLevels(NLog.LogLevel.Trace, NLog.LogLevel.Fatal);

                    LogLevel from = NLog.LogLevel.Fatal;
                    LogLevel to = NLog.LogLevel.Fatal;
                    switch (value)
                    {
                        case 0: // Trace
                            from = NLog.LogLevel.Trace;
                            break;
                        case 1: // Debug
                            from = NLog.LogLevel.Debug;
                            break;
                        case 2: // Info
                            from = NLog.LogLevel.Info;
                            break;
                        case 3: // Warn
                            from = NLog.LogLevel.Warn;
                            break;
                        case 4: // Error
                            from = NLog.LogLevel.Error;
                            break;
                        case 5: // Fatal
                            from = NLog.LogLevel.Fatal;
                            break;
                    }

                    foreach (var rule in LogManager.Configuration.LoggingRules)
                        rule.EnableLoggingForLevels(from, to);

                    LogManager.ReconfigExistingLoggers();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.ToString());
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            log.Error(e.Exception);
            // OR whatever you want like logging etc. MessageBox it's just example
            // for quick debugging etc.
            e.Handled = true;
            Application.Current.Shutdown();
        }
    }
}
