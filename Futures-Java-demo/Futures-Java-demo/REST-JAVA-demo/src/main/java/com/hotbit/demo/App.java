package com.hotbit.demo;


import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

import javax.annotation.PostConstruct;
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

    @Value("${api.url}")
    String url;

    @Value("${accessKey}")
    String accessKey;

    @Value("${secretKey}")
    String secretKey;

//    @Autowired
//    Market market;

    @PostConstruct
    private void init() {
        ExecutorService executorService = Executors.newFixedThreadPool(50000);
        executorService.execute(new Runnable() {
            @Override
            public void run() {

                Market market = new Market(url,accessKey,secretKey);
                market.GetSymbol();
                market.GetBalance();
                market.PutOrder();
            }
        });

    }
}
