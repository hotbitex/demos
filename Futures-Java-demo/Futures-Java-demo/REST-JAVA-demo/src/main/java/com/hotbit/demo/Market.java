package com.hotbit.demo;

import com.alibaba.fastjson.JSONArray;
import org.apache.http.HttpStatus;
import org.apache.http.NameValuePair;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.util.EntityUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URI;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class Market {

    private static final Logger log = LoggerFactory.getLogger(Market.class);

    private String apiUri;
    private String accessKey;
    private String secretKey;
    private ApiSignature signature;

    public Market(String url,String accessKey, String secretKey) {
        this.apiUri = url;
        this.accessKey = accessKey;
        this.secretKey = secretKey;
        this.signature = new ApiSignature();

    }


    public void GetSymbol()  {
        String pathUrl = "/p1/market.list";
        String strUrl = apiUri + pathUrl;

        String resp = HttpClient.getInstance().doGet(strUrl,null);
        log.info("获取交易对列表 : " + resp);
    }

    // 查询余额
    public void GetBalance() {
        String pathUrl = "/p2/balance.query";
        String strUrl = apiUri + pathUrl;
        JSONArray symbols = new JSONArray();
        symbols.add("BTC");
        symbols.add("ETH");

        Map<String, String> params = new HashMap<>();
        params.put("assets",symbols.toJSONString());
        signature.CreateSignature(accessKey,secretKey,params);

        String resp = HttpClient.getInstance().doGet(strUrl,params);
        log.info("获取资产 : " + resp);
    }

    // 下单接口
    public void PutOrder() {
        String pathUrl = "/p2/order.put_limit";
        String strUrl = apiUri + pathUrl;
        Map<String, String> params = new HashMap<>();
        params.put("market","ETHBTC");
        params.put("side","1");
        params.put("amount","0.4");
        params.put("price","398.18");
        params.put("isfee","0");
        signature.CreateSignature(accessKey,secretKey,params);
        String resp = HttpClient.getInstance().doGet(strUrl,params);
        log.info("下单返回 : " + resp);
    }

}
