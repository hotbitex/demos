using HotbitApi.Utility;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;

namespace HotbitApi.WSClient
{
    public class HotbitWSClient : IDisposable
    {
        public const string WS_URL = "wss://ws.hotbit.io/v2";

        private const int SUB_REQ_ID = 777;
        private const int SUB_AUTH_ID = 666;

        #region
        private Logger _logger = LogManager.GetLogger(nameof(HotbitWSClient));
        private WebSocket _hwsClient = null;
        public bool NeedAuth { get; set; }
        public object SubModel { get; set; }
        public string SubscribeType { get; set; }
        public object SubscribeSourceObject { get; set; }
        public string SubItemName { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public object SubAttachment { get; set; }
        public bool NeedAutoReconnect { get; set; }

        private System.Timers.Timer _reconnTimer = new System.Timers.Timer();
        private System.Timers.Timer _pingTimer = new System.Timers.Timer();
        private object _lockObj = new object();

        public event EventHandler<string> OnRecieveMessage;
        public event EventHandler<string> OnConnectionOpened;
        public event EventHandler<string> OnConnectionClosed;
        #endregion

        public HotbitWSClient()
        {
            _pingTimer.Interval = 20 * 1000;
            _pingTimer.Elapsed += _pingTimer_Elapsed;
            _pingTimer.Start();
        }

        private void _pingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_hwsClient != null || _hwsClient.State == WebSocketState.Open)
            {
                var sModel = new HotbitSubscribeModel();
                sModel.method = "server.ping";
                sModel.@params = new List<object>();
                _hwsClient.Send(sModel.ToJson());
            }
        }

        #region
        public void SubscribeDepth(string market)
        {
            if (_hwsClient != null)
            {
                _hwsClient.Open();
                return;
            }

            this.SubscribeSourceObject = market;
            this.SubItemName = market;
            this.SubscribeType = "DEPTH";

            InitWSClient();
        }

        public void SubscribePrice(string market)
        {
            if (_hwsClient != null)
            {
                _hwsClient.Open();
                return;
            }

            this.SubscribeSourceObject = market;
            this.SubscribeType = "PRICE";

            InitWSClient();
        }

        public void SubscribeAsset(string apiKey, string secretKey, List<string> lstToken)
        {
            if (_hwsClient != null)
            {
                _hwsClient.Open();
                return;
            }

            this.ApiKey = apiKey;
            this.SecretKey = secretKey;
            this.SubAttachment = lstToken;
            this.SubscribeType = "ASSET";
            this.SubItemName = apiKey;

            InitWSClient();
        }

        public void SubscribeOrder(string apiKey, string secretKey, List<string> lstMarket)
        {
            if (_hwsClient != null)
            {
                _hwsClient.Open();
                return;
            }

            this.ApiKey = apiKey;
            this.SecretKey = secretKey;
            this.SubAttachment = lstMarket;
            this.SubscribeType = "ORDER";

            InitWSClient();
        }

        private void InitWSClient()
        {
            _hwsClient = new WebSocket(WS_URL, "", null, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Sec-WebSocket-Protocol", "chat,limit-no,gzip") });
            _hwsClient.Opened += _hwsClient_Opened;
            _hwsClient.Closed += _hwsClient_Closed;
            _hwsClient.DataReceived += _hwsClient_DataReceived;

            _hwsClient.Open();

            if (!_reconnTimer.Enabled)
            {
                _reconnTimer.Interval = 1000 * 10;
                _reconnTimer.Elapsed += _reconnTimer_Elapsed;
                _reconnTimer.Enabled = true;
            }
        }
        #endregion

        #region
        private void _reconnTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_lockObj)
            {
                if (_hwsClient.State != WebSocketState.Open && _hwsClient.State != WebSocketState.Connecting && this.NeedAuth)
                {
                    InitWSClient();
                }
            }
        }

        private void _hwsClient_DataReceived(object sender, DataReceivedEventArgs e)
        {
            var strMsg = Encoding.UTF8.GetString(GZip.Decompress(e.Data));
            if (strMsg.Contains("state.update") || strMsg.Contains("state.updateall") || strMsg.Contains("pong")) return;

            if (strMsg.Contains("status") && strMsg.Contains("success"))
            {
                var updateModel = JSONHelper.ConvertToObject<HotbitUpdateModel>(strMsg);
                if (updateModel.id == SUB_REQ_ID)
                {
                    OnConnectionOpened?.Invoke(this, this.SubItemName);
                }
                else if (updateModel.id == SUB_AUTH_ID)
                {
                    var authModel = this.SubModel as AuthSubscribeModel;
                    authModel.reqList.ForEach(p => _hwsClient.Send(p.ToJson()));
                }
                return;
            }

            //_logger.Info($"recieve@{strMsg}@{Title}");

            if (this.SubscribeType == "ASSET" && strMsg.Contains("result"))
            {
                strMsg = strMsg.Replace("error", "method").Replace("null", "\"asset.update\"").Replace("result", "params");
                strMsg = strMsg.Replace("\"params\":", "\"params\":[").Replace("}}", "}}]");
                strMsg = strMsg.Replace("}, ", "}},{");
            }

            OnRecieveMessage?.Invoke(this, strMsg);
        }

        private void _hwsClient_Closed(object sender, EventArgs e)
        {
            OnConnectionClosed?.Invoke(this, this.SubItemName);

            if (NeedAutoReconnect)
            {
                InitWSClient();
            }
        }

        private void _hwsClient_Opened(object sender, EventArgs e)
        {
            if (this.SubscribeType == "DEPTH")
            {
                this.NeedAuth = false;
                var market = this.SubscribeSourceObject as string;

                var sModel = new HotbitSubscribeModel();
                sModel.method = "depth.subscribe";
                sModel.@params = new List<object>() { market, 30, 0 };
                sModel.id = 777;

                this.SubModel = sModel;

                _hwsClient.Send(sModel.ToJson());
            }
            else if (this.SubscribeType == "PRICE")
            {
                this.NeedAuth = false;
                var market = this.SubscribeSourceObject as string;

                var sModel = new HotbitSubscribeModel();
                sModel.method = "price.subscribe";
                sModel.@params = new List<object>();
                sModel.@params.Add(market);
                sModel.id = 777;

                this.SubModel = sModel;
                _hwsClient.Send(sModel.ToJson());
            }
            else if (this.SubscribeType == "ORDER")
            {
                this.NeedAuth = true;
                var lstMarket = this.SubAttachment as List<string>;

                var req = new HotbitSubscribeModel
                {
                    method = "order.subscribe",
                    @params = new List<object>(),
                    id = SUB_REQ_ID
                };

                foreach (var s in lstMarket)
                {
                    req.@params.Add(s);
                }

                var authModel = new AuthSubscribeModel();
                authModel.ApiKey = this.ApiKey;
                authModel.SecretKey = this.SecretKey;
                authModel.reqList.Add(req);
                authModel.AuthModel = new HotbitSubscribeModel()
                {
                    method = "server.auth",
                    @params = new List<object>() { this.ApiKey, GetSign(this.ApiKey, this.SecretKey) },
                    id = SUB_AUTH_ID
                };

                this.SubModel = authModel;
                _hwsClient.Send(authModel.AuthModel.ToJson());
            }
            else if (this.SubscribeType == "ASSET")
            {
                this.NeedAuth = true;
                var lstToken = this.SubAttachment as List<string>;
                var req = new HotbitSubscribeModel
                {
                    method = "asset.query",
                    @params = new List<object>()
                };

                foreach (var s in lstToken)
                {
                    req.@params.Add(s);
                }

                var req1 = new HotbitSubscribeModel
                {
                    method = "asset.subscribe",
                    @params = new List<object>(),
                    id = SUB_REQ_ID
                };

                foreach (var s in lstToken)
                {
                    req1.@params.Add(s);
                }

                var authModel = new AuthSubscribeModel();
                authModel.ApiKey = this.ApiKey;
                authModel.SecretKey = this.SecretKey;
                authModel.req = req;
                authModel.reqList.Add(req);
                authModel.reqList.Add(req1);
                authModel.AuthModel = new HotbitSubscribeModel()
                {
                    method = "server.auth",
                    @params = new List<object>() { this.ApiKey, GetSign(this.ApiKey, this.SecretKey) },
                    id = SUB_AUTH_ID
                };

                this.SubModel = authModel;
                _hwsClient.Send(authModel.AuthModel.ToJson());
            }
        }

        public void Dispose()
        {
            if (_hwsClient != null)
            {
                _hwsClient.Dispose();
                _hwsClient = null;
            }
        }

        private string GetSign(string apiKey, string secretKey)
        {
            var content = String.Format("api_key={0}&secret_key={1}", apiKey, secretKey);
            return Md5.Create(content, 32);
        }
        #endregion
    }
}
