using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotbitApi.Utility
{
    /// <summary>
    /// JSON 帮助类
    /// </summary>
    public class JSONHelper
    {       
        /// <summary>
        /// 将JSON转换为指定类型的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T ConvertToObject<T>(string json)
        {
            var jsetting = new JsonSerializerSettings();
            jsetting.NullValueHandling = NullValueHandling.Include;
            jsetting.NullValueHandling = NullValueHandling.Ignore;
            try
            {
                return JsonConvert.DeserializeObject<T>(json, jsetting);
            }
            catch (Exception e)
            {
                var ee = e.Message.ToString();
                return JsonConvert.DeserializeObject<T>(json, jsetting);
            }
        }


        /// <summary>
        /// 生成压缩的json 字符串
        /// </summary>
        /// <param name="obj">生成json的对象</param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            return ToJson(obj, false);
        }

        /// <summary>
        /// 生成JSON字符串
        /// </summary>
        /// <param name="obj">生成json的对象</param>
        /// <param name="formatjson">是否格式化</param>
        /// <returns></returns>
        public static string ToJson(object obj, bool formatjson)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            IsoDateTimeConverter idtc = new IsoDateTimeConverter();
            idtc.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(idtc);
            JsonWriter jw = new JsonTextWriter(sw);

            if (formatjson)
            {
                jw.Formatting = Formatting.Indented;
            }

            serializer.Serialize(jw, obj);

            //JsonConvert.SerializeObject(dt, idtc).ToString();

            return sb.ToString();
        }


        public static string GetJsonforBootstrapTable(int total,object obj)
        {
            return ToJson(new { total = total, rows = obj });
        }

    }
}
