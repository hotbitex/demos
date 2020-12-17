<?
	
/**
 *  hotbit API库
 *  @author Mr.Zhang
 */
class ApiRequest
{

	// hotbit.io  api url
	//private $url = 'https://api.hotbit.io/v2';

	// hotbit.pro  api url
	private $url = 'https://api.hotbit.pro/v2';
	public $api_method = '';
	public $req_method = '';

	function __construct() {
		$this->api = parse_url($this->url);
		date_default_timezone_set("Asia/Singapore");
	}

	/**
	* 普通请求
	*/
	// 获取交易对列表
	function get_common_symbols() {
		$this->api_method = "/api/v2/market.list";
		$this->req_method = 'GET';
		$param = [];
		$url = $this->create_sign_url($param);
		return json_decode($this->curl($url));
	}	

	

	/**
	*	交易类API
	*/
	// 下单
	function place_order($market = '',$side = 1,$amount = 0,$price = 0,$isfee = 0) {
		$this->api_method = "/api/v2/order.put_limit";
		$this->req_method = 'POST';
		$param = [
			'market' => $market,
			'side' => $side,
			'amount' => $amount,
			'price' => $price,
			'isfee' => $isfee,
		];
		$url = $this->create_sign_url($param);
		echo $url."\n";
		$return = $this->curl($url);
		return $return;
	}


	// 获取账户余额
	function get_balance($assets = []) {
		$this->api_method = "/api/v2/balance.query";
		$this->req_method = 'POST';
		$param = [
			'assets' =>json_encode($assets),
		];
		$url = $this->create_sign_url($param);
		echo $url."\n";
		$return = $this->curl($url);
		return $return;
	}


	/**
	* 类库方法
	*/
	// 生成验签URL
	function create_sign_url($append_param = []) {
		// 验签参数
		$param = [
			'api_key' => ACCESS_KEY,
		];
		if ($append_param) {
			foreach($append_param as $k=>$ap) {
				$param[$k] = $ap; 
			}
		}
		return $this->url.$this->api_method.'?'.$this->bind_param($param);
	}

	// 组合参数
	function bind_param($param) {
		$u = [];
		$sort_rank = [];
		ksort($param);
		foreach($param as $k=>$v) {
			$u[] = $k."=".$v;
			$sort_rank[] = $k."=".$v;
		}
		asort($u);
		$u[] = "sign=".urlencode($this->create_sig($sort_rank));
		//var_dump(implode('&', $u));
		return implode('&', $u);
	}

	// 生成签名
	function create_sig($param) {
		$param[] = "secret_key=".SECRET_KEY;
		$sign_param = implode('&', $param);
		echo ($sign_param) ."\n";
		$signature = md5($sign_param);
		echo $signature."\n";
		return strtoupper($signature);
	}

	function curl($url,$postdata=[]) {
		$ch = curl_init();
		curl_setopt($ch,CURLOPT_URL, $url);
		if ($this->req_method == 'POST') {
			curl_setopt($ch, CURLOPT_POST, 1);
		}
		curl_setopt($ch,CURLOPT_RETURNTRANSFER,1);
		curl_setopt($ch,CURLOPT_HEADER,0);
		curl_setopt($ch, CURLOPT_TIMEOUT,60);
		curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
		curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, FALSE);  
		curl_setopt ($ch, CURLOPT_HTTPHEADER, [
			"Content-Type: application/x-www-form-urlencoded",
			]);
		$output = curl_exec($ch);
		$info = curl_getinfo($ch);
		curl_close($ch);
		return $output;
	}
}	

?>