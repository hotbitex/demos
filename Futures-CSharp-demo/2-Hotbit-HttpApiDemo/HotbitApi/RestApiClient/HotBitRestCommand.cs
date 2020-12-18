using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace HotbitApi.Utility
{
    public class HotBitRestCommand
    {
        public static  Dictionary<string, string> Dict { get; set; }
        public static  string RestfulUrl = "https://api.hotbit.io/api/v2/";
        static HotBitRestCommand()
        {
            Dict = new Dictionary<string, string>();
            Dict.Add("balancequery","balance.query");
            Dict.Add("marketlist", "market.list");
        }

        /// <summary>
        /// 根据指令获取url地址
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string GetUrl(RestCommand command)
        {
            Dict.TryGetValue(command.ToString(), out var cmdPath);
            //return "api.hotbit.io/api/v1/" + cmdPath;
            return RestfulUrl + cmdPath;
        }
    }

    public enum RestCommand
    {
        balancequery,
        marketlist
    }
}
