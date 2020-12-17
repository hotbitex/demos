import hashlib
import json

from hotbit import system


def createSignParams(params):
    params['api_key'] = system.ACCESS_KEY
    paramsStr = json.dumps(params, sort_keys=True, indent=4)
    # print(paramsStr)

    out_str = ""
    params_json = json.loads(paramsStr)
    items = params_json.items()
    for key, value in items:
        out_str += key + '=' + value + '&'
    out_str += 'secret_key=' + system.SECRET_KEY
    # print(out_str)
    hash_md5 = hashlib.md5(out_str.encode(encoding='utf-8'))
    sign = hash_md5.hexdigest().upper()
    # print(sign)
    params['sign'] = sign
    # print(params)
    return params