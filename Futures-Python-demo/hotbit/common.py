from hotbit import system
from hotbit import reset


def getSymbols():
    path = '/p1/market.list'  # 查询所有交易对
    url = system.HOTBIT_API_URL + path
    resp = reset.restApiGet(url)
    print(resp)
