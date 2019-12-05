using GFAlarm.Constants;
using GFAlarm.Util;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GFAlarm
{
    /// <summary>
    /// SubWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SubWindow : Window
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        internal static SubWindow view;

        private Grid WindowTitlebarGrid;
        private Border WindowBorder;

        public bool isLoaded = false;
        public int CurrentMenu = -1;

        public SubWindow()
        {
            InitializeComponent();
            view = this;

            this.Loaded += SubWindow_Loaded;
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

        private void SubWindow_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;

            // 윈도우 위치 및 크기
            this.Top = Config.Window.subWindowPosition[0];
            this.Left = Config.Window.subWindowPosition[1];
            this.Width = Config.Window.subWindowPosition[2];
            this.Height = Config.Window.subWindowPosition[3];
            //this.Top = Config.subWindowTop;
            //this.Left = Config.subWindowLeft;
            //this.Width = Config.subWindowWidth;
            //this.Height = Config.subWindowHeight;

            // 윈도우 요소 가져오기
            WindowBorder = this.Template.FindName("WindowBorder", this) as Border;
            WindowTitlebarGrid = this.Template.FindName("WindowTitlebarGrid", this) as Grid;
        }

        public void SetContent(int menu)
        {
            this.ViewContentControl.Content = null;
            switch (menu)
            {
                case Menus.DASHBOARD:
                    this.ViewContentControl.Content = MainWindow.dashboardView;
                    this.CurrentMenu = Menus.DASHBOARD;
                    break;
                case Menus.ECHELON:
                    this.ViewContentControl.Content = MainWindow.echelonView;
                    this.CurrentMenu = Menus.ECHELON;
                    break;
                case Menus.QUEST:
                    this.ViewContentControl.Content = MainWindow.questView;
                    this.CurrentMenu = Menus.QUEST;
                    break;
                case Menus.PROXY_GUIDE:
                    this.ViewContentControl.Content = MainWindow.proxyGuideView;
                    this.CurrentMenu = Menus.PROXY_GUIDE;
                    break;
            }
        }

        /// <summary>
        /// 윈도우 투명도
        /// </summary>
        private int _WindowOpacity = 100;
        public int WindowOpacity
        {
            get
            {
                return _WindowOpacity;
            }
            set
            {
                _WindowOpacity = value;
                Dispatcher.Invoke(() =>
                {
                    if (this.ViewMainWindow != null)
                    {
                        this.ViewMainWindow.Opacity = ((double)value) / 100;
                    }
                });
            }
        }

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
            // 운영체제 DPI 계산
            PresentationSource source = PresentationSource.FromVisual(this);
            double dpiX = 96;
            //double dpiY = 96;
            if (source != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                //dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }
            dpiMuliply = 96.0 / dpiX;

            Point cursorPos = PointToScreen(Mouse.GetPosition(this));
            cursorPos.X = cursorPos.X * dpiMuliply;
            cursorPos.Y = cursorPos.Y * dpiMuliply;
            Point windowPos = new Point(this.Left, this.Top);
            offset = (Point)(cursorPos - windowPos);

            /// get screen rect
            currentScreen = Extensions.GetScreen(this);

            //uint x, y;
            //Extensions.GetDpi(currentScreen, DpiType.Raw, out x, out y);
            //log.Debug("dpi x {0} y {1}", x, y);

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
                    int snappingMargin = 15;

                    //double workAreaTop = SystemParameters.WorkArea.Top;
                    //double workAreaLeft = SystemParameters.WorkArea.Left;
                    //double workAreaWidth = SystemParameters.WorkArea.Width;
                    //double workAreaHeight = SystemParameters.WorkArea.Height;
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
            //StatusGrid.Background = (SolidColorBrush)Application.Current.Resources["PrimaryBrush"];
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
            //StatusGrid.Background = (SolidColorBrush)Application.Current.Resources["PrimaryDeactiveBrush"];
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
            this.CurrentMenu = -1;
            this.Hide();
        }

        private void StickLeftButton_Click(object sender, RoutedEventArgs e)
        {
            //double workAreaTop = SystemParameters.WorkArea.Top;
            //double workAreaLeft = SystemParameters.WorkArea.Left;
            //double workAreaWidth = SystemParameters.WorkArea.Width;
            //double workAreaHeight = SystemParameters.WorkArea.Height;

            this.Height = MainWindow.view.Height;
            this.Top = MainWindow.view.Top;
            this.Left = MainWindow.view.Left - this.Width;
            //if (this.Left < 0)
            //    this.Left = 0;
        }

        private void StickRightButton_Click(object sender, RoutedEventArgs e)
        {
            //double workAreaTop = SystemParameters.WorkArea.Top;
            //double workAreaLeft = SystemParameters.WorkArea.Left;
            //double workAreaWidth = SystemParameters.WorkArea.Width;
            //double workAreaHeight = SystemParameters.WorkArea.Height;

            this.Height = MainWindow.view.Height;
            this.Top = MainWindow.view.Top;
            this.Left = MainWindow.view.Left + MainWindow.view.Width;
            //if (this.Left + this.Width >= workAreaWidth)
            //    this.Left = workAreaWidth - this.Width;
        }
    }
}
