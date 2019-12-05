using GFAlarm.Data;
using GFAlarm.Transaction;
using GFAlarm.Transaction.PacketProcess;
using GFAlarm.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace GFAlarm.Proxy
{
    // Titanium Web Proxy
    // https://github.com/justcoding121/Titanium-Web-Proxy

    // Nekoxy
    // https://github.com/veigr/Nekoxy

    public class ProxyController
    {

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private ProxyServer proxyServer;
        private ExplicitProxyEndPoint endPointIPv4, endPointIPv6;

        private Queue<GFPacket> queue;
        private Thread thread;
        //private Timer timer;

        // singleton
        public static ProxyController instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProxyController();
                return _instance;
            }
        }
        private static volatile ProxyController _instance;
        
        /// <summary>
        /// 패킷 처리 스레드
        /// </summary>
        private void PacketThread(object state)
        {
            while (true)
            {
                if (queue.Count() > 0)
                {
                    ReceivePacket.Process(queue.Dequeue());
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public ProxyController()
        {
            this.queue = new Queue<GFPacket>();
            //this.timer = new Timer(this.PacketThread, null, 0, 100);
            this.thread = new Thread(PacketThread);
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        /// <summary>
        /// 재시작
        /// </summary>
        /// <returns></returns>
        public bool Restart()
        {
            if (proxyServer != null && proxyServer.ProxyRunning)
                Stop();
            return Start();
        }

        /// <summary>
        /// 사용가능 포트 여부
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool IsAvailablePort(int port)
        {
            // check available port
            //IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            //TcpConnectionInformation[] tcpConnInfos = ipGlobalProperties.GetActiveTcpConnections();
            //foreach (TcpConnectionInformation tcpi in tcpConnInfos)
            //{
            //    if (tcpi.LocalEndPoint.Port == port)
            //    {
            //        MessageBox.Show("사용 중인 포트 번호입니다.");
            //        return false;
            //    }
            //}
            //IPEndPoint[] ipEndPoints = ipGlobalProperties.GetActiveTcpListeners();
            //foreach (IPEndPoint ipEndp in ipEndPoints)
            //{
            //    //log.Info("using port {0}", ipEndp.Port);
            //    if (ipEndp.Port == port)
            //        return false;
            //}
            return true;
        }

        /// <summary>
        /// 프록시 시작
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            log.Debug("trying to start proxy server");
            if (proxyServer != null && proxyServer.ProxyRunning)
            {
                log.Warn("proxy server already started");
                return false;
            }
            proxyServer = null;
            if (!IsAvailablePort(Config.Proxy.port))
            {
                log.Warn("not available port", Config.Proxy.port);
                return false;
            }

            try
            {
                proxyServer = new ProxyServer(false, false, false);
                proxyServer.CertificateManager.PfxPassword = "testpassword";
                proxyServer.ForwardToUpstreamGateway = true;
                proxyServer.CertificateManager.SaveFakeCertificates = false;

                proxyServer.BeforeRequest += OnRequest;
                proxyServer.BeforeResponse += OnResponse;

                if (Config.Proxy.upstreamProxy)
                {
                    proxyServer.UpStreamHttpProxy = new ExternalProxy() { HostName = Config.Proxy.upstreamHost, Port = Config.Proxy.upstreamPort };
                    proxyServer.UpStreamHttpsProxy = new ExternalProxy() { HostName = Config.Proxy.upstreamHost, Port = Config.Proxy.upstreamPort };
                }
                endPointIPv4 = new ExplicitProxyEndPoint(IPAddress.Any, Config.Proxy.port, Config.Proxy.decryptSsl);
                endPointIPv6 = new ExplicitProxyEndPoint(IPAddress.IPv6Any, Config.Proxy.port, Config.Proxy.decryptSsl);
                proxyServer.AddEndPoint(endPointIPv4);
                proxyServer.AddEndPoint(endPointIPv6);
                proxyServer.Start();

                foreach (var endPoint in proxyServer.ProxyEndPoints)
                    log.Debug("proxy server ip={0}, port={1}", endPoint.IpAddress, endPoint.Port);
                log.Debug("proxy server started");

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex, "failed to start proxy server");
            }
            proxyServer = null;
            return false;
        }

        /// <summary>
        /// 프록시 중지
        /// </summary>
        public void Stop()
        {
            log.Debug("trying to stop proxy server");
            if (proxyServer == null)
                return;
            try
            {
                proxyServer.BeforeRequest -= OnRequest;
                proxyServer.BeforeResponse -= OnResponse;
                proxyServer.Stop();
                log.Debug("proxy server stopped");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            proxyServer = null;
        }

        string requestBodyString = "";

        /// <summary>
        /// 요청 (Request)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            string uri = e.HttpClient.Request.RequestUri.AbsoluteUri;
            if (!IsAllowHost(uri))
                return;
            string method = e.HttpClient.Request.Method.ToUpper();
            if (method == "POST" || method == "PUT" || method == "GET")
                requestBodyString = await e.GetRequestBodyAsString();
        }

        /// <summary>
        /// 응답 (Response)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnResponse(object sender, SessionEventArgs e)
        {
            string uri = e.HttpClient.Request.RequestUri.AbsoluteUri;
            if (!IsAllowHost(uri))
                return;
            string method = e.HttpClient.Request.Method.ToUpper();
            if (method == "POST" || method == "PUT" || method == "GET")
            {
                try
                {
                    string param = requestBodyString;

                    long reqId = 0;
                    string outdatacode = "";
                    if (!string.IsNullOrEmpty(param))
                    {
                        string uriAndParam = string.Format("{0}?{1}", uri, param);
                        Dictionary<string, string> queries = Util.Common.GetUriQueries(uriAndParam);
                        if (queries.ContainsKey("outdatacode"))
                            outdatacode = queries["outdatacode"];
                        if (queries.ContainsKey("req_id"))
                            reqId = Parser.String.ParseLong(queries["req_id"]);
                    }
                    bool editedBody = false;
                    string body = await e.GetResponseBodyAsString();

                    // 로그인
                    // (Sign 키 가져오기)
                    if (uri.EndsWith("Index/getUidTianxiaQueue") ||
                        uri.EndsWith("Index/getDigitalSkyNbUid") ||
                        uri.EndsWith("Index/getUidEnMicaQueue"))
                    {
                        Transaction.PacketProcess.Index.GetUid(new GFPacket()
                        {
                            req_id = reqId,
                            uri = uri,
                            outdatacode = outdatacode,
                            body = body,
                        });
                    }
                    // 검열해제
                    if (Config.Extra.unlockCensorMode && uri.EndsWith("Index/index"))
                    {
                        string newBody = ForgeUncensorMode(body);
                        if (!string.IsNullOrEmpty(newBody))
                        {
                            body = newBody;
                            editedBody = true;
                        }
                        else
                        {
                            log.Debug("검열해제 실패");
                        }
                    }
                    // 랜덤부관
                    if (Config.Adjutant.useRandomAdjutant && uri.EndsWith("Index/index"))
                    {
                        //log.Debug("index_body={0}", body);
                        string newBody = ForgeRandomAdjutant(body);
                        if (!string.IsNullOrEmpty(newBody))
                        {
                            body = newBody;
                            editedBody = true;
                        }
                        else
                        {
                            log.Debug("랜덤부관 실패");
                        }
                    }
                    if (editedBody)
                    {
                        e.SetResponseBodyString(body);
                    }

                    queue.Enqueue(new GFPacket()
                    {
                        req_id = reqId,
                        uri = uri,
                        outdatacode = outdatacode,
                        body = body,
                    });
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                finally
                {
                    requestBodyString = "";
                }
            }
        }

        /// <summary>
        /// 랜덤부관 패킷으로 수정
        /// </summary>
        /// <param name="body"></param>
        /// <param name="newBody"></param>
        /// <returns></returns>
        private string ForgeRandomAdjutant(string body)
        {
            if (string.IsNullOrEmpty(UserData.sign))
            {
                return "";
            }
            try
            {
                log.Debug("랜덤부관 시도 중...");
                string decodeBody = AuthCode.Decode(body, UserData.sign);
                if (string.IsNullOrEmpty(decodeBody))
                    return "";
                JObject response = Parser.Json.ParseJObject(decodeBody);
                if (response != null && response.ContainsKey("user_record"))
                {
                    string randomAdjutant = GameData.Doll.GetRandomAdjutant(response);
                    if (!string.IsNullOrEmpty(randomAdjutant))
                    {
                        log.Debug("랜덤부관 설정");
                        response["user_record"]["adjutant"] = randomAdjutant;
                    }
                    return response.ToString(Formatting.None);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return "";
        }

        /// <summary>
        /// 검열해제 패킷으로 수정
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private string ForgeUncensorMode(string body)
        {
            if (string.IsNullOrEmpty(UserData.sign))
                return "";
            try
            {
                log.Debug("검열해제 모드 시도 중...");
                string decodeBody = AuthCode.Decode(body, UserData.sign);
                if (string.IsNullOrEmpty(decodeBody))
                    return "";
                JObject response = Parser.Json.ParseJObject(decodeBody);
                if (response != null && response.ContainsKey("naive_build_gun_formula"))
                {
                    response["naive_build_gun_formula"] = "130:130:130:30";
                    log.Debug("검열해제 모드 설정");
                    return response.ToString(Formatting.None);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return "";
        }

        /// <summary>
        /// 필터링
        /// [중섭-비리비리] http://gf-adrbili-cn-zs-game-0001.ppgame.com/index.php/5000/
        /// [한섭]
        /// [일섭] http://gfjp-game.sunborngame.com/index.php/1001/
        /// [글섭] http://gf-game.sunborngame.com/index.php/1001/
        /// [대만섭] https://sn-game.txwy.tw/index.php/1001/
        /// </summary>
        private static Regex hostFilter = new Regex("(gf-|gfjp-).*(girlfrontline\\.co\\.kr|ppgame\\.com|txwy\\.tw|sunborngame\\.com).*");
        private static Regex pathFilter = new Regex(".*(\\.html|\\.js|\\.txt|\\.jpg|\\.png|\\.css|\\.ico).*");
        private static bool IsAllowHost(string host)
        {
            if (hostFilter.IsMatch(host))
            {
                if (!pathFilter.IsMatch(host))
                    return true;
            }
            return false;
        }
    }
}
