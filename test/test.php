<?php
	$res = '';
	$strName = 'e1015052735';
	exec('/opt/mono/bin/mono mono-test.exe '.$strName.' 2>&1' ,$res);
	print_r($res);

	$text = file_get_contents('testlog.txt');
	header('Content-type: text/plain; charset=utf-8');	
	echo $text;
?>
