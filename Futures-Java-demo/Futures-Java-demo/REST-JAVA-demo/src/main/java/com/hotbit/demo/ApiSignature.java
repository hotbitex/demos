package com.hotbit.demo;

import org.springframework.util.DigestUtils;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Map;
import java.util.SortedMap;
import java.util.TreeMap;

public class ApiSignature {


    public void CreateSignature(String accessKey, String secretKey, Map<String, String> params) {
        StringBuilder sb = new StringBuilder(1024);
        params.put("api_key", accessKey);
        System.out.println(params);
        // 按照上面的顺序，将每个参数与字符“&”连接。
        // Following the sequence above, link each parameter and string with "&"
        SortedMap<String, String> map = new TreeMap<>(params);
        for (Map.Entry<String, String> entry : map.entrySet()) {
            String key = entry.getKey();
            String value = entry.getValue();
            sb.append(key).append('=').append(value).append('&');
        }
        // 添加密钥
        // add the secret key
        sb.append("secret_key=").append(secretKey);

        System.out.println(sb.toString());
        // 签名:
        // Signature:
        String sign = DigestUtils.md5DigestAsHex(sb.toString().getBytes()).toUpperCase();
        System.out.println(sign);
        params.put("sign",sign);
    }

    public String BindParam(String strUrl,Map<String, String> params) {
        StringBuilder sb = new StringBuilder(1024);
        sb.append("?");
        SortedMap<String, String> map = new TreeMap<>(params);
        for (Map.Entry<String, String> entry : map.entrySet()) {
            String key = entry.getKey();
            String value = entry.getValue();
            sb.append(key).append('=').append(value).append('&');
        }
        // 删除最后的 `&`
        // Delete the last '&'
        sb.deleteCharAt(sb.length() - 1);
        return strUrl + sb.toString();
    }
}
