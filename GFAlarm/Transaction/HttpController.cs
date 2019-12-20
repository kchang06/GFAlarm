using GFAlarm.Util;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GFAlarm.Transaction
{
    public class HttpController
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
            #region extension to MIME type list
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        #endregion
        };
        // 웹 서버 디렉토리
        private string _rootDir;
        private Thread _serverThread;
        private HttpListener _listener;
        private int _port;

        /// <summary>
        /// 싱글톤
        /// </summary>
        public static HttpController instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HttpController();
                return _instance;
            }
        }
        private static volatile HttpController _instance;

        public bool Start()
        {
            // Administrator Privilege Check
            if (!App.isElevated)
            {
                log.Warn("access denied (need administrator privilege)");
                return false;
            }
            _rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\resource\\http";
            if (!Directory.Exists(_rootDir))
                Directory.CreateDirectory(_rootDir);
            string pacDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\resource";
            string pacFilename = pacDir + "\\GFPAC.js";
            if (File.Exists(pacFilename))
            {
                string pacFilestream = FileUtil.GetFile(pacFilename);
                try
                {
                    string pacAddress = "";
                    if (!string.IsNullOrEmpty(Config.Proxy.pacDomain))
                        pacAddress = string.Format("\"PROXY {0}:{1}\"", Config.Proxy.pacDomain, Config.Proxy.port);
                    else
                        pacAddress = string.Format("\"PROXY {0}:{1}\"", Config.ip, Config.Proxy.port);
                    pacFilestream = pacFilestream.Replace("{0}", pacAddress);
                    File.WriteAllText(_rootDir + "/GFPAC.js", pacFilestream);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            _port = Config.Proxy.pacPort;
            if (_listener != null)
            {
                _listener.Stop();
                _listener.Close();
                _listener = null;
            }
            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add(string.Format("http://*:{0}/", _port));
                _listener.Start();
                _serverThread = new Thread(this.Listen);
                _serverThread.Start();
                log.Debug("http server started (port: {0}, path: {1})", _port, _rootDir);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }

        public void Stop()
        {
            if (_serverThread != null)
            {
                _serverThread.Abort();
                _serverThread = null;
            }
            if (_listener != null)
            {
                _listener.Stop();
                _listener.Close();
                _listener = null;
            }
        }

        private void Listen()
        {
            while (true)
            {
                try
                {
                    if (_listener == null)
                        return;
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch { }
            }
        }

        private void Process(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath;
            string filename = string.Format("{0}{1}", _rootDir, path);
            if (File.Exists(filename))
            {
                try
                {
                    Stream input = new FileStream(filename, FileMode.Open);

                    //Adding permanent http response headers
                    string mime;
                    context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
                    context.Response.ContentLength64 = input.Length;
                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                    context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));

                    byte[] buffer = new byte[1024 * 16];
                    int nbytes;
                    while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        context.Response.OutputStream.Write(buffer, 0, nbytes);
                    input.Close();

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
        }
    }
}
