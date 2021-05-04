<?php

try{

	$key = $_POST['key'];

	if($key != 1){
		throw new Exception;
	}

	header("Content-tpe: image/jpg");

	$img = imagecreatetruecolor(200,200);

	$backgroundcolor = imagecolorallocate($img, 255, 0, 0);

	imagefilledrectangle($img, 0, 0, 300, 300, $backgroundcolor);

	imagejpg($img);

	imagejpg($img, './map.jpg');
	
	imagedestroy($img);

	header('Content-Type: image/jpg');
	readfile('image/map.jpg');
}catch(Exception $e){
	header('Content-Type: image/jpg');
	readfile('image/err.jpg');
}
?>
