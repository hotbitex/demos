package http

import (
    "Futures-Go-demo/config"
    "Futures-Go-demo/utils"
)

// 获取交易对列表
func GetSymbols() string {
    pathUrl := "/p1/market.list"
    strUrl := config.API_URL + pathUrl
    jsonReturn := utils.HttpGetRequest(strUrl)
    return jsonReturn
}