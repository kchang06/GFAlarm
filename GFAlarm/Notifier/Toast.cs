using GFAlarm.ShellHelpers;
using GFAlarm.Util;
using LocalizationResources;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace GFAlarm.Notifier
{
    public static class Toast
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private const String APP_ID = "소녀전선 알리미";
        private static ToastNotifier toastNotifier = null;

        static Toast()
        {
            try
            {
                TryCreateShortcut();
                toastNotifier = ToastNotificationManager.CreateToastNotifier(APP_ID);
            }
            catch(Exception ex)
            {
                log.Error(ex, "윈도우 토스트 초기화 실패");
            }
        }

        // Create and show the toast.
        // See the "Toasts" sample for more detail on what can be done with toasts
        public static void ShowToast(Message msg, bool silent = false, bool loop = false)
        {
            // 알림 콘텐츠 디자인
            // https://docs.microsoft.com/ko-kr/windows/uwp/design/shell/tiles-and-notifications/adaptive-interactive-toasts
            string toastString = "";
            if (loop)
                toastString += "<toast duration='short'>";
            else
                toastString += "<toast>";
            toastString += string.Format("<visual>"
                                            + "<binding template='ToastGeneric'>"
                                               + "<text hint-maxLines='1'>{0}</text>"
                                               + "<text>{1}</text>"
                                               + "<image placement='appLogoOverride' hint-crop='circle' src='{2}'/>"
                                            + "</binding>"
                                        + "</visual>", msg.subject, msg.content, string.Format("file:\\\\{0}\\Resource\\Image\\Toast\\icon_{1}.png",
                                        HttpUtility.HtmlEncode(Util.Common.GetAbsolutePath()), new Random().Next(1, 15)));

            if (Config.Setting.voiceNotification || Config.Setting.strongWinToast)
            {
                string audioString = loop == true ? "ms-winsoundevent:Notification.Looping.Alarm4" : "ms-winsoundevent:Notification.Looping.Alarm1";
                toastString += string.Format("<audio src='{0}' loop='{1}' silent='{2}'/>", audioString, loop == true ? "true" : "false", silent == true ? "true" : "false");
            }

            toastString += "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastString);

            // Create the toast
            ToastNotification toast = new ToastNotification(toastXml);
            toast.Activated += ToastActivated;
            toast.Dismissed += ToastDismissed;
            toast.Failed += ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            toastNotifier.Show(toast);
        }

        // In order to display toasts, a desktop application must have a shortcut on the Start menu.
        // Also, an AppUserModelID must be set on that shortcut.
        // The shortcut should be created as part of the installer. The following code shows how to create
        // a shortcut and assign an AppUserModelID using Windows APIs. You must download and include the 
        // Windows API Code Pack for Microsoft .NET Framework for this code to function
        //
        // Included in this project is a wxs file that be used with the WiX toolkit
        // to make an installer that creates the necessary shortcut. One or the other should be used.
        private static bool TryCreateShortcut()
        {
            String shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + string.Format("\\Microsoft\\Windows\\Start Menu\\Programs\\{0}.lnk", LanguageResources.Instance["PROGRAM_TITLE"]);
            if (!File.Exists(shortcutPath))
            {
                InstallShortcut(shortcutPath);
                return true;
            }
            return false;
        }

        private static void InstallShortcut(String shortcutPath)
        {
            // Find the path to the current executable
            String exePath = Process.GetCurrentProcess().MainModule.FileName;
            IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

            // Create a shortcut to the exe
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

            // Open the shortcut property store, set the AppUserModelId property
            IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

            using (PropVariant appId = new PropVariant(APP_ID))
            {
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
            }

            // Commit the shortcut to disk
            IPersistFile newShortcutSave = (IPersistFile)newShortcut;

            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
        }

        private static void ToastActivated(ToastNotification sender, object e)
        {
            //RemoveToast(sender);
        }

        private static void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs e)
        {
            //RemoveToast(sender);
        }

        private static void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
        {
            //RemoveToast(sender);
        }
    }
}
