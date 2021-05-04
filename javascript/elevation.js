google.maps.event.addDomListener(window, "load", initialize);

var map;
var eService;
var myLocation;
var imageBounds = new google.maps.LatLngBounds();
var northWest, northEast, southWest, southEast;
var markerList = new google.maps.MVCArray();
var groundOverlay = new google.maps.GroundOverlay();
var elevations;
var divLatlng;

//ページ読み込み完了時に実行する関数
function initialize() {

	//初期位置
	var initLat =34.482254;
	var initLng= 136.824878;
	myLocation = new google.maps.LatLng(34.482254, 136.824878);

	document.getElementById('latitude').value = initLat;
	document.getElementById('longitude').value =initLng;
	setBounds();

	// マップ表示
	map = new google.maps.Map(document.getElementById("map"), {
		center: myLocation,
		zoom:15,
		mapTypeId: google.maps.MapTypeId.ROADMAP
	});

	// ドラッグできるマーカーを表示
	moveMarker = new google.maps.Marker({
		position: myLocation,
		title: "My Location",
		draggable: true // ドラッグ可能にする
	});
	moveMarker.setMap(map);
	
	// マーカーのドロップ（ドラッグ終了）時のイベント
	google.maps.event.addListener( moveMarker, 'dragend', function(ev){
		// イベントの引数evの、プロパティ.latLngが緯度経度。
		document.getElementById('latitude').value = ev.latLng.lat();
		document.getElementById('longitude').value = ev.latLng.lng();
		myLocation = new google.maps.LatLng(ev.latLng.lat(),ev.latLng.lng());
		setBounds();

		map.setCenter(myLocation);
		});


	google.maps.event.addListener( moveMarker,'click', function(ev){
		//地図の中心を移動
		map.setCenter(myLocation);
	});
	
}

function getElevation(){
	var latLng = new Array();
	var div = 11;
	eService = new google.maps.ElevationService();
	elevations = new Array();	

	// PathElevationRequest
	divLatlng = (northWest.lat()-southWest.lat()) / (div-1); 
//	divLatLng = (northWest.lat()-southWest.lat()) / 100;
	latLng[0] = northWest;
	latLng[1] = northEast;
	
	var i = 0;
	var interval = setInterval(function(){
		if(i > 0){
			latLng[0] = new google.maps.LatLng(
				latLng[0].lat()-divLatlng,
				latLng[0].lng());
	        latLng[1] = new google.maps.LatLng(
				latLng[1].lat()-divLatlng,
				latLng[1].lng());
		}
		var req = {
			path: latLng,
			samples: div
		};
		eService.getElevationAlongPath(req, elevResultCallback);

		i++;
		if(i == div){
			document.getElementById('getData').innerText = 'get data';
			clearInterval(interval);
		}
	},800);
	
	setElevMarker();


/*
	setTimeout(function(){
		markerList.forEach(function(marker, index){
                	marker.setMap(null);
        	});
		var j = 0;
		var interval = setInterval(function(){       
	        	var latlng = new google.maps.LatLng(
				elevations[j].latitude,
				elevations[j].longitude
			);
			var marker = new google.maps.Marker({
        	                position: latlng,
        	                title: elevations[j].elevation.toString(),
        	        });
                	marker.setMap(map);
                	markerList.push(marker);
			j++;
			if(j == elevations.length){
				clearInterval(interval);
			}
		},50);

	},2000); 
*/
}

function elevResultCallback(results, status){
	if(status == google.maps.ElevationStatus.OK){
		for(var i = 0; i < results.length; i++){
			var res = {
				latitude: results[i].location.lat().toFixed(6),
				longitude: results[i].location.lng().toFixed(6),
				elevation: results[i].elevation,
				resolution: elevations.length			
			};
			elevations.push(res);
			
			var mmm = new google.maps.Marker({
                        position: results[i].location,
						map:map
                });
		}
		document.getElementById('dataLength').innerText = elevations.length;
	}


}
function setElevMarker(){
	for(var i=0; i < elevations.length; i++){
                var marker = new google.maps.Marker({
                        position: elevations[i].location,
                        title: elevations[i].elevation.toString()
                });
                marker.setMap(map);
                elevMarker.push(marker);
	}
}

function setBounds(){
	northWest = new google.maps.LatLng(myLocation.lat()+0.004492,myLocation.lng()-0.00545);
	northEast = new google.maps.LatLng(myLocation.lat()+0.004492,myLocation.lng()+0.00545);
	southWest = new google.maps.LatLng(myLocation.lat()-0.004492,myLocation.lng()-0.00545);
	southEast = new google.maps.LatLng(myLocation.lat()-0.004492,myLocation.lng()+0.00545);
	
	var divLat = (northWest.lat()-southWest.lat()) / (11-1);
	var lat = northWest.lat()+divLat;
	var newLat = new google.maps.LatLng(lat,northWest.lng());
	var length  = google.maps.geometry.spherical.computeDistanceBetween(northWest, newLat);
	
	
	var divLng = Math.abs((northWest.lng()-northEast.lng()) / (11-1));
	
	

	var lng = northWest.lng()+divLng;
	var newLng = new google.maps.LatLng(northWest.lat(),lng);
	var width = google.maps.geometry.spherical.computeDistanceBetween(northWest, newLng);
	
	document.getElementById('length').value = length;
	document.getElementById('width').value = width;
	
	
}
function setImageBounds(){
	imageBounds = new google.maps.LatLngBounds();
	boundsLength = divLatlng / 2;
	var imgSouthWest = new google.maps.LatLng(
			myLocation.lat()-0.004492-boundsLength,
			myLocation.lng()-0.00545-boundsLength);
	var imgNorthEast = new google.maps.LatLng(
			myLocation.lat()+0.004492+boundsLength,
			myLocation.lng()+0.00545+boundsLength);
	imageBounds.extend(imgSouthWest);
	imageBounds.extend(imgNorthEast);
}

// XMLHttpResponse
function postSample(data){
	var xml = new XMLHttpRequest();
	xml.open('POST','createMap.php',true);
	xml.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
	xml.send(data);
	xml.onreadystatechange = function(){
		if(xml.readyState == 4){
			var res = xml.responseTest;
			alert(res);
		}
	}
}


function sendData(){
	$.ajax({
	type:'POST',
	url:'createMap.php',
	data: {data: elevations},
	}).done(function(img_data){
	var data = 'data:image/jpeg;base64,'+img_data;

	setImageBounds();
	groundOverlay.setMap(null);
	groundOverlay = null;
	groundOverlay = new google.maps.GroundOverlay(data,imageBounds,{
		map: map,
		opacity: 0.4,
	});

	groundOverlay.setMap(map);
	map.setZoom(15);

	document.getElementById('getData').innerText = 'send data';
//	alert(elevations.length);
//	alert(img_data);	
	}).fail(function(data){
		alert('error');
	});
}
