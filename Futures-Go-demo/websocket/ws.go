package websocket

import (
    "Futures-Go-demo/config"
    "bytes"
    "compress/gzip"
    "github.com/gorilla/websocket"
    "io/ioutil"
    "log"
    "net/url"
)

var ws *websocket.Conn

func WsRun() {

	u, err := url.Parse(config.WS_URL)
	if err != nil {
		panic(err)
	}

	//u:= url.URL{Scheme: "ws", Host: config.WS_URL, Path: "v2"}
	log.Printf("connecting to %s", u.String())
	ws, _, err = websocket.DefaultDialer.Dial(u.String(), nil)
	if err != nil {
		log.Fatal("dial: ", err)
	}
    var message []byte
    // kline订阅
	message = []byte("{\"method\": \"kline.query\", \"params\": [\"ETHBTC\",1601481600,1601485200,15], \"id\": 100}")
    send(message, ws)

    // depth订阅
    message = []byte("{\"method\": \"depth.query\", \"params\": [\"ETHBTC\",100,\"1\"], \"id\": 100}")
    send(message, ws)

    // deals订阅
    message = []byte("{\"method\": \"deals.subscribe\", \"params\": [\"BTCUSD\",\"ETHUSD\"], \"id\": 100}")
    send(message, ws)



    // 读取消息
	go readLoop(ws)


	// Cleanly close the connection by sending a close message and then
	// waiting (with timeout) for the server to close the connection.
	//if err = c.WriteMessage(websocket.CloseMessage, websocket.FormatCloseMessage(websocket.CloseNormalClosure, ""));err != nil {
	//    log.Println("write close:", err)
	//    return
	//}

}

func WsClose() {
    if err := ws.Close();err != nil {
       log.Printf("close err %v", err)
    }
}

func send(message []byte, ws *websocket.Conn) {
	if err := ws.WriteMessage(websocket.TextMessage, message); err != nil {
		log.Fatal(err)
	}
    log.Printf("Send: %s\n", message)
}

func readLoop(ws *websocket.Conn) {
	for {
        _, frame, err := ws.ReadMessage()
		if err != nil {
			log.Println("read:", err)
			return
		}
        message,e := UnzipByte(frame)
        if e != nil {
            log.Printf("UnzipByte err %v", e)
            return
        }
		log.Printf("Recv:   %s", message)
	}
}


func UnzipByte(data []byte) (unzipData []byte, err error) {
    if len(data) <= 0 {
        return
    }
    closer, err := gzip.NewReader(bytes.NewReader(data))
    if err != nil {
        return
    }
    defer closer.Close()
    return ioutil.ReadAll(closer)
}