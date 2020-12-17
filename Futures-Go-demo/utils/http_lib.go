package utils

import (
    "Futures-Go-demo/config"
    "crypto/md5"
    "encoding/hex"
    "fmt"
    "io/ioutil"
    "net/http"
    "sort"
    "strings"
)

const ContentType = "application/x-www-form-urlencoded"


// POST请求基础函数, 通过封装Go语言Http请求
// url: 请求的URL
// params: 请求参数
// return: 请求结果
func HttpPostRequest(url string,params string) string {
    resp, err := http.Post(url, ContentType, strings.NewReader(params))
    if err != nil {
        return err.Error()
    }
    defer resp.Body.Close()
    body, err := ioutil.ReadAll(resp.Body)
    if err != nil {
        return err.Error()
    }
    return string(body)
}

// GET请求基础函数, 通过封装Go语言Http请求
// url: 请求的URL
// return: 请求结果
func HttpGetRequest(url string) string {
    resp, err := http.Get(url)
    if err != nil {
        return err.Error()
    }
    defer resp.Body.Close()
    body, err := ioutil.ReadAll(resp.Body)
    if err != nil {
        return err.Error()
    }
    return string(body)
}

// 进行签名后的HTTP POST请求
// mapParams: map类型的请求参数, key:value
// strRequest: API路由路径
// return: 请求结果
func ApiKeyPost(params map[string]string,strRequestPath string) string {
    mapParams2Sign := make(map[string]string)
    mapParams2Sign["api_key"] = config.ACCESS_KEY
    for k,v := range params {
        mapParams2Sign[k] = v
    }
    strParams := CreateSign(mapParams2Sign)
    return HttpPostRequest(strRequestPath,strParams)
}

// 构造签名
// mapParams: 送进来参与签名的参数, Map类型
// strSecretKey: 进行签名的密钥
func CreateSign(mapParams map[string]string) string {
    // 参数处理, 按API要求, 参数名应按ASCII码进行排序(使用UTF-8编码, 其进行URI编码, 16进制字符必须大写)

    strParams := Map2UrlQueryBySort(mapParams)
    strPayload := ComputeMd5(strParams)
    fmt.Println("strPayload = ",strPayload)
    strParams += "&sign=" + strPayload
    fmt.Println(strParams)
    return strParams
}

func Map2UrlQueryBySort(mapParams map[string]string) string {
    var keys []string
    for key := range mapParams {
        keys = append(keys, key)
    }
    sort.Strings(keys)

    var strParams string
    for _, key := range keys {
        strParams += key + "=" + mapParams[key] + "&"
    }
    //strParams += "secret_key=" + config.SECRET_KEY
    if 0 < len(strParams) {
        strParams = string([]rune(strParams)[:len(strParams)-1])
    }
    return strParams
}


func ComputeMd5(strParams string) string {
    strParams += "&secret_key=" + config.SECRET_KEY
    fmt.Println("strParams = ",strParams)
    h := md5.New()
    h.Write([]byte(strParams))
    return strings.ToUpper(hex.EncodeToString(h.Sum(nil)))
}