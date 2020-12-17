import asyncio

from hotbit.websocket import start

try:
    asyncio.get_event_loop().run_until_complete(start())
except:
    print("websocket close~~")