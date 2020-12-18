using HotbitApi.WSClient;
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
            //ws client init
            var wsClient = new HotbitWSClient();
            wsClient.OnRecieveMessage += (e, msg) => 
            {
                Console.WriteLine($"ws update msg => {msg}");
            };
            wsClient.OnConnectionOpened += (e, subItem) =>
            {
                Console.WriteLine($"{subItem} ws opened");
            };
            wsClient.OnConnectionClosed += (e, subItem) =>
            {
                Console.WriteLine($"{subItem} ws closed");
            };

            //subscribe last price -- websocket without sign
            wsClient.SubscribePrice("BTCUSDT");

            //subscribe asset update -- websocket with sign
            wsClient.SubscribeAsset(ApiKey, SecretKey, new List<string>() { "BTC", "USDT", "ETH", "HTB" });

            Console.ReadKey();
        }
    }
}
