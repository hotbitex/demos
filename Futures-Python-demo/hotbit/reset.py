import requests


def restApiGet(url):
    resp = requests.get(url)
    return resp.json()


def restApiPost(url, params):
    headers = {'Content-Type': 'application/json'}
    req = requests.post(url, params, headers)
    return req.json()
