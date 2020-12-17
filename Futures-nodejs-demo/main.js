require("./utils.js");
require("./socket.js");
const crypto = require('crypto');
const { http_post } = require("./http.js");
const { soc_sendMsg, soc_connect, soc_data } = require("./socket.js");

//============================================================================
// config
//============================================================================
global.config = {
    //
    HTTP: "https://api.hotbit.io/api/v2",
    WS: "wss://ws.hotbit.io/v2",
    //
    apiKey: "your apiKey",
    secreKey: "your secreKey",
}


//============================================================================
// main
//============================================================================
const main = async () => {
    //
    console.log("currrent config ", config);
    ///
    let rsp
    rsp = await http_post(config.HTTP + "/order.book?market=BTCUSDT&side=1&offset=0&limit=50");
    if (rsp.data.error == null) {
        console.log("order.book succ data is ", rsp.data)
    } else {
        console.log(rsp.data.error)
    }

    rsp = await http_post(config.HTTP + "/market.user_deals?api_key=1&sign=1&market=BTCUSDT&offset=0&limit=50");
    if (rsp.data.error == null) {
        console.log("market.user_deals succ")
    } else {
        console.log(rsp.data.error)
    }

    await soc_connect();
    soc_sendMsg(soc_data("server.ping"));

    let authStr = "api_key=" + config.apiKey + "&"
    authStr += "secret_key=" + config.secreKey
    authStr = crypto.createHash('md5').update(authStr).digest("hex")
    authStr = authStr.toUpperCase();
    soc_sendMsg(soc_data("server.auth", [config.apiKey, authStr]));

}

main();


