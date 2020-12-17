//============================================================================
// utils 
//============================================================================
const crypto = require('crypto');
//--------------------make sign---------------
const MAKE_SIGN = (url) => {
    let obj = {}
    let returnUrl = url.split("?")[0]
    if (url.indexOf("?") != -1) {
        var str = url.split("?")[1],
            strs = str.split("&");
        // params split
        for (var i = 0; i < strs.length; i++) {
            obj[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
        //sort params
        if (obj["sign"] || obj["api_key"]) {
            let keys = Object.keys(obj);
            keys = keys.sort((a, b) => {
                let astr = a.split("")
                let bstr = b.split("")
                let l = astr.length > bstr.length ? astr.length : bstr.length;
                for (var i = 0; i < l; i++) {
                    if (astr[i] != bstr[i]) {
                        if (bstr[i] > astr[i]) return -1;
                        if (bstr[i] < astr[i]) return 1;
                    }
                }
                return astr.length - bstr.length;
            });
            //replace  sign  and apikey from config
            obj["api_key"] = config.apiKey;
            var checkStr = ""
            for (var i = 0; i < keys.length; i++) {
                //
                if (keys[i] != "sign") {
                    if (i != 0) {
                        checkStr += "&"
                    }
                    //
                    checkStr += keys[i]
                    checkStr += "="
                    checkStr += obj[keys[i]]
                }

            }
            //make new url 
            checkStr += "&secret_key=" + config.secreKey;
            checkStr = crypto.createHash('md5').update(checkStr).digest("hex")
            checkStr = checkStr.toUpperCase();
            obj["sign"] = checkStr;
            returnUrl += "?"
            i = 0;
            for (var key in obj) {
                if (i != 0) returnUrl += "&"
                i++;
                returnUrl += key;
                returnUrl += "="
                returnUrl += obj[key]
            }
            return returnUrl
        } else {
            return url
        }
    }
    return url
}


global.MAKE_SIGN = MAKE_SIGN;