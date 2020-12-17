import json
from hotbit import system, reset, utils


# 查询用户资产
def queryBalance():
    path = '/api/v2/balance.query'  # 查询用户资产
    url = system.HOTBIT_API_URL + path
    symbols = ["BTC"]
    value = json.dumps(symbols)
    data = {"assets": value}
    postData = utils.createSignParams(params=data)
    resp = reset.restApiPost(url, params=postData)
    print(resp)
