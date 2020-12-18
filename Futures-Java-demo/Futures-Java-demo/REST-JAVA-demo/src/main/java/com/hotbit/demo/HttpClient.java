package com.hotbit.demo;

import com.alibaba.fastjson.JSON;
import org.apache.commons.collections.MapUtils;
import okhttp3.*;

import java.io.IOException;
import java.net.SocketTimeoutException;
import java.util.Map;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.stream.Collectors;

public class HttpClient {
    private OkHttpClient httpClient;

    private AtomicInteger numRequestFaild = new AtomicInteger(0);

    static final MediaType JSON_TYPE = MediaType.parse("application/json");

    private HttpClient() {
        OkHttpClient.Builder builder = new OkHttpClient.Builder()
                .connectionPool(new ConnectionPool(200, 10, TimeUnit.SECONDS)).connectTimeout(3, TimeUnit.SECONDS)
                .readTimeout(5, TimeUnit.SECONDS);
        httpClient = builder.build();
    }

    public static HttpClient getInstance() {
        return new HttpClient();
    }

    public String doGet(String url, Map<String, String> params) {
        Request.Builder reqBuild = new Request.Builder();
        HttpUrl.Builder urlBuilder = HttpUrl.parse(url).newBuilder();
        if (MapUtils.isNotEmpty(params)) {
            params.forEach((k, v) -> {
                urlBuilder.addQueryParameter(k, v);
            });
        }
        reqBuild.url(urlBuilder.build());

        Response response = null;
        try {
            response = httpClient.newCall(reqBuild.build()).execute();
        } catch (IOException e) {
            if (e instanceof SocketTimeoutException) {
                numRequestFaild.getAndIncrement();
            }
            throw new HttpRequestException("http执行异常，url=" + url, e);
        }
        if (response.isSuccessful()) {
            try {
                reset();
                return response.body().string();
            } catch (IOException e) {
                throw new HttpRequestException("http结果解析异常", e);
            }
        } else {
            int statusCode = response.code();
            throw new HttpRequestException("响应码不为200，返回响应码：" + statusCode + "，url：" + urlBuilder.build());
        }
    }

    public String doPost(String url, Map<String, String> params) {
        FormBody.Builder builder = new FormBody.Builder();
        if (MapUtils.isNotEmpty(params)) {
            params.forEach((k, v) -> {
                builder.add(k, v);
            });
        }

        Request.Builder reqBuilder = new Request.Builder().url(url);
        if (MapUtils.isNotEmpty(params)) {
            //RequestBody body = RequestBody.create(JSON_TYPE, JSON.toJSONString(params));
            reqBuilder.post(builder.build());
        }

        Response response = null;
        try {
            response = httpClient.newCall(reqBuilder.build()).execute();
        } catch (IOException e) {
            if (e instanceof SocketTimeoutException) {
                numRequestFaild.getAndIncrement();
            }
            throw new HttpRequestException("http执行异常，url=" + url, e);
        }
        if (response.isSuccessful()) {
            try {
                reset();
                return response.body().string();
            } catch (IOException e) {
                throw new HttpRequestException("http结果解析异常", e);
            }
        } else {
            int statusCode = response.code();
            throw new HttpRequestException("响应码不为200，返回响应码：" + statusCode + "，url：" + reqBuilder.build());
        }
    }

    private String toQueryString(Map<String, String> params) {
        return String.join("&",
                params.entrySet().stream()
                        .map((entry) -> entry.getKey() + "=" + entry.getValue())
                        .collect(Collectors.toList()));
    }

    private void reset() {
        numRequestFaild.set(0);
    }

    public int getRequestFaildTotal() {
        return numRequestFaild.get();
    }
}
