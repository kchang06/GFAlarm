using GFAlarm.Util;
using LocalizationResources;
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

namespace GFAlarm.View.Guide
{
    /// <summary>
    /// ProxyGuide.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProxyGuideView : UserControl
    {
        public ProxyGuideView()
        {
            InitializeComponent();
        }

        private void ProxyGuideViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.ProxyGuideTextBlock.Html = string.Format(LanguageResources.Instance["SETTING_PROXY_SERVER_GUIDE_COMMENT"], Config.ip, Config.Proxy.port);
        }
    }
}
