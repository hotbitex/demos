using HotbitApi.Utility;
using System;
using System.Collections.Generic;

namespace TestPrj
{
    class Program
    {
        private static string ApiKey = "xxxxx-xxxxx-xxxxx-xxxxx";
        private static string SecretKey = "qewrtyuiopzxcvbnm";
        static void Main(string[] args)
        {
            var restClient = new HotbitRestApiClient();

            //balance.query -- http with sign
            var assetResponse = restClient.BalanceQuery(ApiKey, SecretKey, new List<string>() { "BTC", "USDT", "ETH", "HTB" });

            //market.list -- http without sign
            var marketListResponse = restClient.MarketList();

            Console.ReadKey();
        }
    }
}
