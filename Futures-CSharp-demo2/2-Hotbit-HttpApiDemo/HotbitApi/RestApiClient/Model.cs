using System.Collections.Generic;
using Newtonsoft.Json;

namespace HotbitApi.Utility
{
    /// <summary>
    /// 出参模型
    /// </summary>
    public class ResponseModel
    {
        public object result { get; set; }
        public string error { get; set; }
        public long id { get; set; }
    }

    public class OrderPendingDetail
    {
        public int limit;
        public int offset;
        public int total;
        public List<EntrustDetailResponse> records;
    }

    public class EntrustDetailResponse
    {
        public object error { get; set; }
        public long id { get; set; }
        public string deal_fee { get; set; }
        public string market { get; set; }
        public string source { get; set; }
        public int type { get; set; }
        public string amount { get; set; }
        public int side { get; set; }
        public double ctime { get; set; }
        public string deal_money { get; set; }
        public int user { get; set; }
        public double mtime { get; set; }
        public string deal_stock { get; set; }
        public string price { get; set; }
        public string taker_fee { get; set; }
        public string maker_fee { get; set; }
        public string left { get; set; }
    }

    /// <summary>
    /// 用户验证出参模型
    /// </summary>
    public class ResponseAuthModel
    {
        public ResponseStatus result { get; set; }
        public string error { get; set; }
        public long id { get; set; }
    }

    public class ResponseStatus
    {
        public string status { get; set; }
    }

    public enum RequestType
    {
        /// <summary>
        /// 普通消息
        /// </summary>
        Message,
        /// <summary>
        /// 订阅消息
        /// </summary>
        Subscribe,
        /// <summary>
        /// 需要进行验证，才能订阅的消息
        /// </summary>
        AuthSubscribe
    }
    
}

























