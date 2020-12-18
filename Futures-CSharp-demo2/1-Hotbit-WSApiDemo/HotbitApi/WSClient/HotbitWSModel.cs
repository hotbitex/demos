using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotbitApi.WSClient
{
    public class HotbitSubscribeModel
    {
        [JsonProperty("method")]
        public string @method { get; set; }
        [JsonProperty("params")]
        public List<object> @params { get; set; }
        public long id { get; set; }
    }

    public class AuthSubscribeModel
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public HotbitSubscribeModel AuthModel { get; set; }
        public HotbitSubscribeModel req { get; set; }

        public List<HotbitSubscribeModel> reqList = new List<HotbitSubscribeModel>();
    }

    public class HotbitUpdateModel
    {
        public string method;
        [JsonProperty("params")]
        public List<object> @params;
        public long id;
        public object error;
    }

    public class HotbitDepthDetail
    {
        public List<List<string>> asks;
        public List<List<string>> bids;
    }

    public class HotbitAssetDetail
    {
        public string freeze;
        public string available;
    }

    public class HotbitWSResModel
    {
        public string method;
        public long id;
        [JsonProperty("params")]
        public object @params;
    }

    public class HotbitWSOrderDetail
    {
        public long id;
        public string market;
        public string source;
        public int type;
        public int side;
        public double ctime;
        public double mtime;
        public string price;
        public string amount;
        public string taker_fee;
        public string maker_fee;
        public string left;
        public string deal_stock;
        public string deal_money;
        public string deal_fee;
    }
}
