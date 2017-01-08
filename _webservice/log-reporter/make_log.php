<?php
	
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
	
	function GenerateRandomString($length = 6) {
		$characters = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
		$charactersLength = strlen($characters);
		$randomString = '';
		for ($i = 0; $i < $length; $i++) {
			$randomString .= $characters[rand(0, $charactersLength - 1)];
		}
		return $randomString;
	}
	
	if (intval($_SERVER['CONTENT_LENGTH']) <= 10*1024*1024) {
		parse_str($_SERVER['QUERY_STRING'], $query);
		$username = GetHttpLogin();
		$comment = $query["comment"];
		$postdata = file_get_contents("php://input");
		$log_filename = sprintf("%s_%s - %s.log", time(), GenerateRandomString(), $username);
		file_put_contents("logs/" . $log_filename, $postdata);
		chmod("logs/" . $log_filename, 0600);
		$mailBody = sprintf("https://axio.name/axtools/log-reporter/logs/%s", rawurlencode($log_filename));
		SendMail("axio@axio.name", "Axioma", sprintf("Error log from %s (%s)", $username, $comment), $mailBody, "AxTools logger");
		echo "OK";
	}
	
?>