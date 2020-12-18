package com.hotbit.demo;

import static org.springframework.beans.factory.config.BeanDefinition.SCOPE_PROTOTYPE;

import org.java_websocket.client.WebSocketClient;
import org.springframework.context.annotation.Scope;
import org.springframework.stereotype.Service;

@Service
@Scope(SCOPE_PROTOTYPE)
public class Client {


    /**
     * 创建连接
     * Create connection
     */
    public void connect(WebSocketClient ws) {
        //进行连接
        //Connecting
        ws.connect();
    }

}