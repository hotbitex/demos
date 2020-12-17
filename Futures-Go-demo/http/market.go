package http

import (
    "Futures-Go-demo/config"
    "Futures-Go-demo/utils"
    "encoding/json"
)

// 获取账户余额
func GetBalance(symbol ...string) string {
    pathUrl := "/api/v2/balance.query"
    symbols := make([]string,0)
    for _, v := range symbol {
        symbols = append(symbols, v)
    }
    if len(symbols) == 0 {
        return "symbol error"
    }
    buffer,err := json.Marshal(symbols)
    if err != nil {
        return err.Error()
    }
    strRequest := config.API_URL + pathUrl
    params :=make(map[string]string)
    params["assets"] = string(buffer)
    jsonReturn := utils.ApiKeyPost(params,strRequest)
    return jsonReturn
}


// 下单接口
func PlaceOrder(symbol,side,amount,price,isFee string) string {
    pathUrl := "/api/v2/order.put_limit"
    strRequest := config.API_URL + pathUrl
    params :=make(map[string]string)
    params["market"] = symbol
    params["side"] = side
    params["amount"] = amount
    params["price"] = price
    params["isfee"] = isFee
    jsonReturn := utils.ApiKeyPost(params,strRequest)
    return jsonReturn
}