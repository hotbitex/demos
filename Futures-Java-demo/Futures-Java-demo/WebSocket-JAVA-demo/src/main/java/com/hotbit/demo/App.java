package com.hotbit.demo;


import org.java_websocket.client.WebSocketClient;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

import javax.annotation.PostConstruct;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/**
 * @author Mr.zhang
 */
@SpringBootApplication
public class App {
    public static void main(String[] args) {
        SpringApplication.run(App.class, args);
    }

    @Value("${ws.url}")
    String url;

    @Value("${accessKey}")
    String accessKey;

    @Value("${secretKey}")
    String secretKey;

    @Autowired
    Client client;

    @PostConstruct
    private void init() {
        ExecutorService executorService = Executors.newFixedThreadPool(50000);
        executorService.execute(new Runnable() {
            @Override
            public void run() {
                try {

                    URI uri = new URI(url);
                    WebSocketClient ws = new WebSocketSubscription(uri, accessKey, secretKey);
                    client.connect(ws);
                } catch (URISyntaxException e) {
                    e.printStackTrace();
                }
            }

        });

    }
}
