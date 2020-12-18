using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace HotbitApi.Utility
{
    /// <summary>
    /// 封装HTTP Get和Post请求
    /// </summary>
    public class HttpUtilManager
    {
        private static readonly ILogger Log = NLog.LogManager.GetCurrentClassLogger();

        private HttpClient _http = new HttpClient();

        public HttpUtilManager()
        {
            //_http.Timeout = TimeSpan.FromMilliseconds(20); //超时设定为20秒
        }
       
        /// <summary>
        /// Http请求基础方法
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="HttpMethod"></param>
        /// <param name="ContentType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<byte[]> HttpRequest(string Url, HttpMethodEnum HttpMethod, string ContentType, byte[] data)
        {
            byte[] result = null;
            try
            {
                HttpResponseMessage message = null;
                if (HttpMethod == HttpMethodEnum.POST)
                {
                    using (Stream dataStream = new MemoryStream(data ?? new byte[0]))
                    {
                        using (HttpContent content = new StreamContent(dataStream))
                        {
                            content.Headers.Add("Content-Type", ContentType);
                            
                            message = await _http.PostAsync(Url, content);
                        }
                    }
                }
                else if (HttpMethod == HttpMethodEnum.GET)
                {
                    message =await _http.GetAsync(Url);
                }
                //处理返回的数据
                using (message)
                {
                    using (var responseStream = await message.Content.ReadAsStreamAsync())
                    {
                        if (responseStream != null)
                        {
                            byte[] responseData = new byte[responseStream.Length];
                            responseStream.Read(responseData, 0, responseData.Length);
                            result = responseData;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new byte[0];
            }
            return result;
        }

        /// <summary>
        /// Post方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paras"></param>
        /// <param name="urlPrex"></param>
        /// <returns></returns>
        public async Task<string> Post(string url, Dictionary<string, string> param, string urlPrex = "https://")
        {
            try
            {
                if (!url.StartsWith("http") && !url.StartsWith("https")) url = urlPrex + url;

                var ret = "";
                if (param.Count == 0) return "";
                //遍历参数集合

                var buffer = new StringBuilder();
                var i = 0;
                foreach (var key in param.Keys)
                {
                    buffer.AppendFormat(i > 0 ? "&{0}={1}" : "{0}={1}", key, param[key]);
                    i++;
                }
                var data = Encoding.UTF8.GetBytes(buffer.ToString());

                Log.Info($"转发Post请求url: {url}, 参数：{buffer}");

                var result = await HttpRequest(url, HttpMethodEnum.POST, "application/x-www-form-urlencoded", data);
       
                if (result != null) ret = Encoding.UTF8.GetString(result);

                Log.Info($"Post转发返回结果: {ret}, 请求url: {url}, 参数：{buffer}");
                return ret;
            }
            catch (Exception ex)
            {
                var mess = new StringBuilder();
                mess.AppendLine("转发Post方法调用时发生错误,数据内容如下：");
                mess.AppendLine($"urlPrex: {urlPrex} url: {url} param: {param.ToJson()}");
                mess.AppendLine("错误内容如下：");
                mess.AppendLine("");
                mess.AppendLine(ex.Message);
                Log.Error(mess);
                return "";
            }
        }

        /// <summary>
        /// Get方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="urlPrex"></param>
        /// <returns></returns>
        public async Task<string> Get(string url, string param, string urlPrex = "https://")
        {
            try
            {
                if (!url.StartsWith("http") && !url.StartsWith("https")) url = urlPrex + url;

                var ret = "";

                if (param != null && !param.Equals(""))
                {
                    url = url + "?" + param;
                }

                Log.Info($"转发Get请求: {url}");

                var result = await HttpRequest(url, HttpMethodEnum.GET, "application/x-www-form-urlencoded", null);

                if (result != null) ret = Encoding.UTF8.GetString(result);

                Log.Info($"Get转发返回结果: {ret}, 请求url: {url}");

                return ret;
            }
            catch (Exception ex)
            {
                var mess = new StringBuilder();
                mess.AppendLine("转发Get方法调用时发生错误,数据内容如下：");
                mess.AppendLine($"urlPrex: {urlPrex} url: {url} param: {param}");
                mess.AppendLine("错误内容如下：");
                mess.AppendLine("");
                mess.AppendLine(ex.Message);
                Log.Error(mess);
                return "";
            }
        }
    }

    /// <summary>
    /// Http方法枚举
    /// </summary>
    public enum HttpMethodEnum
    {
        GET,
        POST
    }
}
