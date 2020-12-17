const WebSocket = require('ws');
const Zlib = require('zlib');
const max_retry = 99999;
let retry_count = 0;
let retry_internal = 5000
let socket_cli = {}
let socket;
let waitingList = [];
let updateMap = {}
let RID = 0;

socket_cli.makeMsg = () => {
    if (waitingList.length == 0) return;
    let buffArr = waitingList.shift();
    const inflated = Zlib.gunzipSync(buffArr);
    let msgObj = JSON.parse(inflated.toString(), Date.now())

    if (msgObj.method == 'depth.update') {
        if (updateMap[msgObj.params[2]]) {
            console.log(msgObj.params[2], Date.now() - updateMap[msgObj.params[2]]);
            updateMap[msgObj.params[2]] = Date.now();
        } else {
            updateMap[msgObj.params[2]] = Date.now();
        }
    }

    console.log("socket resive <========= ", inflated.toString(), Date.now())
    if (waitingList.length != 0) {
        socket_cli.makeMsg();
    }
}

socket_cli.soc_data = (method, params) => {
    RID++;
    return JSON.stringify({ "method": method, "params": params || [], "id": RID })
}

socket_cli.connect = async () => {
    //
    return new Promise((resolve, reject) => {
        socket = new WebSocket(config.WS)
        //
        socket.on("open", () => {
            console.log("socket is opened ");
            resolve();
            retry_count = 0;
        });
        //
        socket.on("message", (msg) => { waitingList.push(msg); if (waitingList.length == 1) socket_cli.makeMsg(); });
        //
        socket.on("close", (reason) => {
            console.error("socket is closed", reason);
            setTimeout(() => { socket_cli.reConnect(); }, retry_internal);
        });
        //
        socket.on("error", (reason) => { console.error("socket on error", reason) });
    })
}
socket_cli.reConnect = async () => {
    socket = null;
    socket_cli.connect();
    retry_count++;
}
socket_cli.sendMsg = (data) => {
    console.log("socket send =========> ", data);
    socket.send(data);
}

module.exports = {
    soc_sendMsg: socket_cli.sendMsg,
    soc_connect: socket_cli.connect,
    soc_data:socket_cli.soc_data,
}