import gzip
import json

import websockets
from hotbit import system


async def readLoop(websocket):
    while True:
        greeting = await websocket.recv()
        bytes = gzip.decompress(greeting)
        print(str(bytes, encoding="utf8"))


async def start():
    #uri = 'wss://testws.hotbit.pro/v2'
    async with websockets.connect(system.HOTBIT_WEBSOCKET_URI) as websocket:
        await websocket.send(json.dumps({"method": "kline.query", "params": ["ETHBTC", 1601481600, 1601485200, 15], "id": 100}))
        await websocket.send(json.dumps({"method": "depth.query", "params": ["ETHBTC", 100, "1"], "id": 100}))
        await websocket.send(json.dumps({"method": "deals.subscribe", "params": ["BTCUSD", "ETHUSD"], "id": 100}))
        # 读取数据
        await readLoop(websocket)
