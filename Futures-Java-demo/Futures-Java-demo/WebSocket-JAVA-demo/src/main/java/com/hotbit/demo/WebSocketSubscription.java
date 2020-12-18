package com.hotbit.demo;

import com.alibaba.fastjson.JSONArray;
import com.alibaba.fastjson.JSONObject;
import org.java_websocket.client.WebSocketClient;
import org.java_websocket.handshake.ServerHandshake;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import java.io.UnsupportedEncodingException;
import java.net.URI;
import java.nio.ByteBuffer;
import java.nio.charset.CharacterCodingException;


/**
 * WebSocketSubscription websocket
 */
public class WebSocketSubscription extends WebSocketClient {
    private static final Logger log = LoggerFactory.getLogger(WebSocketSubscription.class);

    private URI uri;
    private String accessKey;
    private String secretKey;


    public WebSocketSubscription(URI uri, String accessKey, String secretKey) {
        super(uri);
        this.uri = uri;
        this.accessKey = accessKey;
        this.secretKey = secretKey;
    }


    @Override
    /**
     * 建立连接
     * Build connection
     */
    public void onOpen(ServerHandshake shake) {
        //ping();

        subscriptionKline();
        subscriptionDepth();
        subscriptionDeals();
    }

    @Override
    /**
     * 此处不需要操作
     * No operation here
     */
    public void onMessage(String arg0) {
        if (arg0 != null) {
            log.info("receive message " + arg0);
        }
    }

    @Override
    /**
     * 发送异常处理
     *  Send exception handling
     */
    public void onError(Exception arg0) {
//     异常处理
// 	   Exception handling
        String message = "";
        try {
            message = new String(arg0.getMessage().getBytes(), "UTF-8");
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        }
        log.info("has error ,the message is :" + message);
        arg0.printStackTrace();
    }

    @Override
    /**
     * 关闭连接处理
     * Close connection handling
     */
    public void onClose(int arg0, String arg1, boolean arg2) {
//    TODO 关闭连接处理
        log.info("connection close ");
        log.info(arg0 + "   " + arg1 + "  " + arg2);
    }

    @Override
    /**
     * 接收服务器信息
     * Receive infomation from server
     */
    public void onMessage(ByteBuffer bytes) {

        try {
            //  可添加自定义消息处理
            //	Custom message handling can be added
            //  接收服务器信息 并进行解压
            // Receive server information and decompress

            String message = new String(ZipUtil.decompress(bytes.array()), "UTF-8");
            log.info("接收消息: " + message);


            // 将信息转为放入JSONObject
            // Convert information into JSONObject
            // JSONObject jsonObject = JSONObject.parseObject(message);

        } catch (CharacterCodingException e) {
            e.printStackTrace();
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    public void ping() {
        String method = "server.ping";
        JSONArray params = new JSONArray();
        sendRPC(method,params,100);
    }

    // kline订阅
    public void subscriptionKline() {
        String method = "kline.query";
        JSONArray params = new JSONArray();
        params.add("ETHBTC");
        params.add(1601481600);
        params.add(1601485200);
        params.add(15);
        sendRPC(method,params,100);
    }

    // depth订阅
    public void subscriptionDepth() {
        String method = "depth.query";
        JSONArray params = new JSONArray();
        params.add("ETHBTC");
        params.add(100);
        params.add("1");
        sendRPC(method,params,100);
    }

    // deals订阅
    public void subscriptionDeals() {
        String method = "deals.subscribe";
        JSONArray params = new JSONArray();
        params.add("BTCUSD");
        params.add("ETHUSD");
        sendRPC(method,params,100);
    }

    /**
     *  发送协议消息
     * @param method
     * @param params
     * @param id
     */
    public void sendRPC(String method,Object params,Integer id) {
        JSONObject req = new JSONObject();
        req.put("method",method);
        req.put("params",params);
        req.put("id",id);
        log.warn(req.toString());
        send(req.toString());
    }

}