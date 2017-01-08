<?php

// ini_set('display_errors', 1);
// ini_set('display_startup_errors', 1);
// error_reporting(E_ALL);

class ClientStatInfo {
	
	public $ips = array();
	
	public function __construct($ips) {
		$this->ips = $ips;
	}

	public function jsonSerialize() {
		return array('ips' => $this->ips);
	}
	
	public static function fromJSON($json) {
		$obj = json_decode($json, true);
		return new self($obj["ips"]);
	}
	
}

function LogInfo($message) {
	$date = date('Y-m-d H:i:s');
	file_put_contents("info.log", $date . " || " . $message . "\n", FILE_APPEND | LOCK_EX);
}

function LogError($message) {
	$date = date('Y-m-d H:i:s');
	file_put_contents("error.log", $date . " || " . $message . "\n", FILE_APPEND | LOCK_EX);
}

function GetHttpLogin() {
	if (isset($_SERVER['HTTP_AUTHORIZATION'])) {
		$ha = base64_decode(substr($_SERVER['HTTP_AUTHORIZATION'], 6));
		list($login, $password) = explode(':', $ha);
		unset($ha);
		return $login;
	}
	return "UNKNOWN";
}

function SendMail($dest, $dest_name, $subj, $message, $from_name) {
	require_once "/home/nginx/php_libs/PHPMailer/PHPMailerAutoload.php";
	$mail = new PHPMailer;
	$mail->CharSet = 'UTF-8';
	$mail->isSMTP();
	$mail->Host = 'smtp.yandex.com';
	$mail->SMTPAuth = true;
	$mail->Username = 'tech@axio.name';
	$mail->Password = 'tech125521';
	$mail->SMTPSecure = 'tls';
	$mail->From = 'tech@axio.name';
	$mail->FromName = $from_name;
	$mail->addAddress($dest, $dest_name);
	$mail->Subject = $subj;
	$mail->Body    = $message;
	$mail->send();
}

function format_dictionary($dict, $valueFunc) {
	$response = '{ ';
	foreach ($dict as $key => $value) {
		if ($valueFunc === NULL) {
			$response = $response . sprintf("[%s] => %s, ", $key, $value);
		}
		else {
			$response = $response . sprintf("[%s] => %s, ", $key, $valueFunc($value));
		}
	}
	return $response . "}";
}

function GetRedisClient() {
	require_once '/home/nginx/php_libs/predis-1.1.1/src/Autoloader.php';
	Predis\Autoloader::register();
	$redis = new Predis\Client(array('scheme' => 'tcp', 'host' => '127.0.0.1', 'port' => 6379), array('profile' => '2.2', 'prefix' => 'axtools-upd-client-info:'));
	return $redis;
}

function GetUpdateInfo() {
	$response = file_get_contents("update2.json");
	header('Content-Type: application/json');
	header('Content-Length: ' . strlen($response));
	return $response;
}

function GetUpdatePackageAndExit() {
	$filename = "update-package2.zip";
	if (file_exists($filename)) {
		$finfo = finfo_open(FILEINFO_MIME_TYPE);
		header('Content-Type: ' . finfo_file($finfo, $filename));
		header('Content-Length: ' . filesize($filename));
		finfo_close($finfo);
		readfile($filename);
		exit;
	}
	else {
		header('HTTP/1.0 404 Not Found');
		LogError(sprintf("%s || File not found: %s", $_SERVER['REMOTE_ADDR'], $filename));
		exit(1);
	}
}

function main() {
	$remoteIP = $_SERVER['REMOTE_ADDR'];
	$username = GetHttpLogin();
	$redis = GetRedisClient();
	$userinfo = $redis->get($username);
	if ($userinfo !== NULL) {
		$savedUserInfo = ClientStatInfo::fromJSON($userinfo);
		foreach ($savedUserInfo->ips as $ip => $timeLastSeen) {
			if (time() - $timeLastSeen >= 15 * 60) {
				unset($savedUserInfo->ips[$ip]);
			}
		}
		$savedUserInfo->ips[$remoteIP] = time();
		if (count($savedUserInfo->ips) > 1) {
			$message = sprintf("Username: %s\nIPs: %s", $username, format_dictionary($savedUserInfo->ips, function($v) {return date('Y-m-d H:i:s', $v);}));
			SendMail("axio@axio.name", "Developer", "Single license is used from multiple IPs!", $message, "AxTools updater");
			LogError(sprintf("%s || Single license is used from multiple IPs: %s", $remoteIP, $message));
		}
		$redis->set($username, json_encode($savedUserInfo));
		LogInfo(sprintf("%s || Usage statistic is updated for license: %s; IPs: %s", $remoteIP, $username, format_dictionary($savedUserInfo->ips, function($v) {return date('Y-m-d H:i:s', $v);})));
	}
	else {
		$redis->set($username, json_encode(new ClientStatInfo(array($remoteIP => time()))));
		LogInfo(sprintf("%s || New license is added: %s", $remoteIP, $username));
	}
	
	$postdata = file_get_contents("php://input");
	if (strpos($postdata, "get-update-info") !== false) {
		echo GetUpdateInfo();
		LogInfo(sprintf("%s || Update info is sent to %s", $remoteIP, $username));
	}
	elseif (strpos($postdata, "get-update-package") !== false) {
		LogInfo(sprintf("%s || Update package is sent to %s", $remoteIP, $username));
		GetUpdatePackageAndExit();
	}
	else {
		header('HTTP/1.0 400 Bad Request');
		LogError(sprintf("%s || Unknown action: %s", $remoteIP, $postdata));
		exit(1);
	}
}

main();

?>
