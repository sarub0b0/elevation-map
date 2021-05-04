<?php
	if(empty($_SERVER['PHP_AUTH_USER'] || $_SERVER['PHP_AUTH_USER']==""){
		header("WWW-Authenticate")
