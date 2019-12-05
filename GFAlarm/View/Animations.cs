using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace GFAlarm.View
{
    public class Animations
    {
        /// <summary>
        /// 페이드 인
        /// </summary>
        public static DoubleAnimation FadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(0.2)
        };

        /// <summary>
        /// 페이드 아웃
        /// </summary>
        public static DoubleAnimation FadeOut = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(200))
        };

        /// <summary>
        /// 페이드 아웃
        /// </summary>
        public static Storyboard StoryboardFadeOut = new Storyboard()
        {
            Children =
            {
                FadeOut
            },
        };

        /// <summary>
        /// 깜빡임
        /// </summary>
        public static DoubleAnimation Flicking = new DoubleAnimation
        {
            From = 0,
            To = 1,
            AutoReverse = true,
            Duration = TimeSpan.FromMilliseconds(500),
            RepeatBehavior = RepeatBehavior.Forever,
        };

        /// <summary>
        /// 높이 변경
        /// </summary>
        public static DoubleAnimation ChangeHeight = new DoubleAnimation
        {
            From = 0,
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
        };
    }
}
