<?
	
	define('ACCESS_KEY','*'); // your ACCESS_KEY
	define('SECRET_KEY', '*'); // your SECRET_KEY

	include "lib.php";

	//实例化类库
	$req = new ApiRequest();
	
	// 获取交易对列表
	//var_dump($req->get_common_symbols());

	// 下单接口
	//var_dump($req->place_order('ETHBTC',1,0.4,393.92,0));

	// 获取账户余额示例
	var_dump($req->get_balance(["BTC","ETH"]));

?>