using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HotbitApi.Utility;
using NLog;
using Polly;

namespace HotbitApi.Utility
{
    public class HotbitRestApiClient
    {
        private static readonly ILogger Log = NLog.LogManager.GetCurrentClassLogger();

        private HttpUtilManager HttpUtilManager = new HttpUtilManager();
        /// <summary>
        /// Http retry interval
        /// </summary>
        public static List<TimeSpan> SleepDurations { get; set; }

        public HotbitRestApiClient()
        {
            try
            {
                SleepDurations = new List<TimeSpan>();
                SleepDurations.Add(TimeSpan.FromSeconds(0.5f));
                var retryInterval2= 1f;
                if (retryInterval2 > 0) 
                {
                    SleepDurations.Add(TimeSpan.FromSeconds(retryInterval2));
                }
                var retryInterval3 = 2f;
                if (retryInterval3 > 0) 
                {
                    SleepDurations.Add(TimeSpan.FromSeconds(retryInterval3));
                }
            }
            catch (Exception ex)
            {
                var mess = new StringBuilder();
                mess.AppendLine("http client init error：");
                mess.AppendLine(ex.Message);
                Log.Error(mess);
            }
        }

        /// <summary>
        /// query user assets
        /// </summary>
        /// <returns></returns>
        public string BalanceQuery(string api_key, string secretKey, List<string> assets)
        {
            var param = $"api_key={api_key}&sign=''&assets={assets.ToJson()}";
            param = CreateSign_Get(api_key, secretKey, param);
            return GetBase(RestCommand.balancequery, param);
        }

        /// <summary>
        /// get exchange market list
        /// </summary>
        /// <returns></returns>
        public string MarketList()
        {
            return GetBase(RestCommand.marketlist, "");
        }

        private string GetBase(RestCommand command, string param)
        {
            return HttpBase(command, param);
        }

        private string PostBase(RestCommand command, Dictionary<string, string> param)
        {
            return HttpBase(command, param, 1);
        }

        /// <summary>
        /// Http request base method
        /// </summary>
        /// <param name="command"></param>
        /// <param name="paramObj"></param>
        /// <param name="requestType">get=0;post=1,默认值为0</param>
        /// <returns></returns>
        private string HttpBase(RestCommand command, object paramObj, int requestType = 0)
        {
            var url = "";
            try
            {
                var policy = Policy<string>.Handle<Exception>().
                    WaitAndRetry(SleepDurations, (exception, timeSpan, context) =>
                    {
                        var mess = new StringBuilder();
                        mess.AppendLine("Http error：");
                        mess.AppendLine("Url: " + url + " method: " + (requestType == 0 ? "Get" : "Post") + " param: " + (requestType == 0 ? paramObj : paramObj.ToJson()));
                        mess.AppendLine($"retry times：{timeSpan}");
                        mess.AppendLine("error message：");
                        mess.AppendLine(exception.Exception.Message);
                        Log.Error(mess);
                    });

                var ret = policy.Execute(() =>
                {
                    url = HotBitRestCommand.GetUrl(command);

                    if (requestType == 0)
                    {
                        return HttpUtilManager.Get(url, (string)paramObj).Result;
                    }

                    return HttpUtilManager.Post(url, (Dictionary<string, string>)paramObj).Result;
                });

                return ret;
            }
            catch (Exception ex)
            {
                var mess = new StringBuilder();
                mess.AppendLine("http error：");
                mess.AppendLine("Url: " + url + " method: " + (requestType == 0 ? "Get" : "Post") + " param: " + (requestType == 0 ? paramObj : paramObj.ToJson()));
                mess.AppendLine("error message：");
                mess.AppendLine(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// create sign -- get
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public string CreateSign_Get(string apiKey, string secretKey, string param)
        {
            try
            {
                var paramDict = new Dictionary<string, string>();

                var array = param.Split('&');
                foreach (var t in array)
                {
                    var str = t.Split('=');
                    paramDict.Add(str[0], str[1]);
                }
                var retDict = CreateSign_Base(apiKey, secretKey, paramDict);
                var buffer = new StringBuilder();
                var i = 0;
                foreach (var s in retDict)
                {
                    buffer.AppendFormat(i > 0 ? "&{0}={1}" : "{0}={1}", s.Key, s.Value);
                    i++;
                }
                paramDict.Clear();
                retDict.Clear();
                return buffer.ToString();
            }
            catch (Exception ex)
            {
                var mess = new StringBuilder();
                mess.AppendLine("sign error : ");
                mess.AppendLine(ex.Message);
                Log.Error(mess);
                return null;
            }
        }

        /// <summary>
        /// create sign -- post
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="paramDict"></param>
        /// <returns></returns>
        public Dictionary<string, string> CreateSign_Post(string apiKey, string secretKey, Dictionary<string, string> paramDict)
        {
            return CreateSign_Base(apiKey, secretKey, paramDict);
        }

        /// <summary>
        /// create sign -- base
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="paramDict"></param>
        /// <returns></returns>
        private Dictionary<string, string> CreateSign_Base(string apiKey, string secretKey, Dictionary<string, string> paramDict)
        {
            try
            {
                paramDict.Remove("sign");

                var newDict = paramDict.OrderBy(item => item.Key);
                var buffer = new StringBuilder();
                var i = 0;
                foreach (var s in newDict)
                {
                    buffer.AppendFormat(i > 0 ? "&{0}={1}" : "{0}={1}", s.Key, s.Value);
                    i++;
                }

                var sk = paramDict.Where(c => c.Key == "secret_key");

                if (!sk.Any())
                {
                    buffer.Append($"&secret_key={secretKey}");
                }

                var sign = Md5.Create(buffer.ToString(), 32);
                buffer.Clear();

                paramDict.Add("sign", sign);

                return paramDict;
            }
            catch (Exception ex)
            {
                var mess = new StringBuilder();
                mess.AppendLine("sign error ：");
                mess.AppendLine(ex.Message);
                Log.Error(mess);
                return null;
            }
        }
        
    }
}
