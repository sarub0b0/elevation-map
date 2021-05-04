<?php

try{

	$data = $_POST['data'];

	if($data == null ){
		throw new Exception;
	}

	$tableName = database($data);

	exec('/opt/mono/bin/mono database.exe '.$tableName);	
	
	$img = file_get_contents("image/map.jpg");
	$img_str = base64_encode($img);
	header('Content-type: image/jpeg');
	echo $img_str;

}catch(Exception $e){
	header('Content-type: text/plain');
	echo "error";
}

function database($mapData){
	require_once 'login.php';

	$conn = mysql_connect($dbHost,$dbUser,$dbPass) or die("connect failed");

	if(!mysql_select_db($dbName,$conn)){
		echo "select db".mysql_errno($conn).":".mysql_error($conn)."<br/>\n";

	}
	$timestamp = time();
	$tableName = date('mdHis',$timestamp);
	$strTableName = 'e'.strval($tableName);


	if(!mysql_select_db($dbName,$conn)){
		echo "select db".mysql_errno($conn).":".mysql_error($conn)."<br/>\n";
	}

	$query = 'CREATE TABLE '.$strTableName.'(
			id INT AUTO_INCREMENT PRIMARY KEY,
			latitude DOUBLE,
			longitude DOUBLE,
			elevation DOUBLE,
			resolution INT);';

	if(!mysql_query($query,$conn)){
		echo "create table".mysql_errno($conn).":".mysql_error($conn)."<br/>\n";
	}
	foreach($mapData as $data){
		foreach($data as $key => $value){
			switch($key){
				case "latitude":
					$latitude = addslashes($value);
					break;
				case "longitude":
					$longitude = addslashes($value);
					break;
				case "elevation":
					$elevation = addslashes($value);
					break;
				case "resolution":
					$resolution = addslashes($value);
					break;
			}
		}
		$addRecord = <<<EOS
			INSERT INTO `{$strTableName}`
			(
			 latitude,
			 longitude,
			 elevation,
			 resolution
			)
			VALUES
			(
			 '{$latitude}',
			 '{$longitude}',
			 '{$elevation}',
			 '{$resolution}'
			)
EOS;
		if(!mysql_query($addRecord,$conn)){
			echo "insert into".mysql_errno($conn).":".mysql_error($conn)."\n";
		}
	}
	mysql_close($conn) or die('not closed');
	
	return $strTableName;	
}

?>
