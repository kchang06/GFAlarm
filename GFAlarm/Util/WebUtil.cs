using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Util
{
    public class WebUtil
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 웹 파일 가져오기
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string RequestFile(string url)
        {
            string responseString = "";

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Timeout = 3000;
                request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    responseString = reader.ReadToEnd();
                }

                response.Close();
            }
            catch (Exception ex)
            {
                log.Error(ex, "웹 요청 에러");
            }

            return responseString;
        }

        /// <summary>
        /// 웹 데이터베이스 버전 확인 (true: 업데이트 필요)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool RequestDatabaseVersion(string url, string path)
        {
            try
            {
                string lastVer = RequestFile(url);
                string currVer = FileUtil.GetFile(path);
                if (!string.IsNullOrEmpty(lastVer) && lastVer != currVer)
                    return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }

        /// <summary>
        /// 웹 데이터베이스 업데이트
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool RequestAndSaveDatabase(string url, string path)
        {
            try
            {
                log.Debug("Update DB => {0}", url);
                //long baseFileSize = new FileInfo(path).Length;
                string file = RequestFile(url);
                if (!string.IsNullOrEmpty(file))
                {
                    File.WriteAllText(path, file);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Debug("Update Failure");
            return false;
        }
    }
}
