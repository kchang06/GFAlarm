using GFAlarm.Util;
using LocalizationResources;
using NLog;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows;

namespace GFAlarm.Notifier
{
    public static class Mail
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 메일 보내기
        /// </summary>
        /// <param name="msg"></param>
        public static void Send(Message msg)
        {
            SmtpClient smtp = null;
            MailMessage mailMessage = null;
            try
            {
                // 메일 설정 문제
                if (string.IsNullOrEmpty(Config.Setting.fromMailAddress)
                    || string.IsNullOrEmpty(Config.Setting.fromMailPass)
                    || string.IsNullOrEmpty(Config.Setting.toMailAddress))
                {
                    throw new Exception("잘못된 메일 설정");
                }

                smtp = new SmtpClient
                {
                    //Host = "smtp.gmail.com",
                    Host = Config.Setting.smtpServer,
                    Port = 587,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(Config.Setting.fromMailAddress, Config.Setting.fromMailPass),
                    Timeout = 20000
                };
                mailMessage = new MailMessage(Config.Setting.fromMailAddress, Config.Setting.toMailAddress)
                {
                    Subject = string.Format(LanguageResources.Instance["MAIL_FORMAT"], msg.content),
                    SubjectEncoding = Encoding.UTF8,
                    Body = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    BodyEncoding = Encoding.UTF8
                };
                smtp.Send(mailMessage);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "에러");
                log.Error(ex, "메일 보내기 에러");
            }
            finally
            {
                if (smtp != null) smtp.Dispose();
                if (mailMessage != null) mailMessage.Dispose();
            }
        }
    }
}
