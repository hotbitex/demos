const axios = require('axios')
axios.defaults.retry = 4;
axios.defaults.retryDelay = 1000;


module.exports = {
    http_post: async (url) => {
        let now = Date.now();
        let realUrl = MAKE_SIGN(url)
        console.log("http send =========> ", realUrl);
        let response = await axios.post(realUrl).catch((error) => { LG.error("err :: url%s error ", realUrl, error) });
        // console.log("http response  =======> " + realUrl + "::", response.data, Date.now());
        return response
    },

    http_get: async (url) => {
        let now = Date.now();
        let realUrl = MAKE_SIGN(url)
        console.log("http send =========> ", realUrl);
        let response = await axios.get(realUrl).catch((error) => { LG.error("err :: url%s error ", realUrl, error) });
        // console.log("http response  =======> " + realUrl + "::", response.data, Date.now());
        return response
    }
}