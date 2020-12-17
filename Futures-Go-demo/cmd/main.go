package main

import (
    "Futures-Go-demo/http"
    "Futures-Go-demo/websocket"
    "fmt"
    "log"
    "os"
    "os/signal"
    "time"
)

/**
 *  @author Mr.Zhang
 */
func main() {

    interrupt := make(chan os.Signal, 1)
    signal.Notify(interrupt, os.Interrupt)

    //----------------------------------------------------------------------------------------
    // 公共API

    symbolReturn := http.GetSymbols()
    fmt.Println(time.Now().Format("2006-01-02 15:04:05.000000"),"  获取交易对列表: ", symbolReturn)


    balanceReturn := http.GetBalance("BTC")
    fmt.Println(time.Now().Format("2006-01-02 15:04:05.000000"),"  获取账户余额: ",balanceReturn)


    //orderReturn := http.PlaceOrder("ETHBTC","1","0.4","393.92","0")
    //fmt.Println(time.Now().Format("2006-01-02 15:04:05.000000"),"  下单返回: ",orderReturn)

    websocket.WsRun()

    for {
       select {
       case <-interrupt:
           websocket.WsClose()
           log.Println("interrupt")
           return
       }
    }
}