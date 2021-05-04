google.maps.event.addDomListener(window, "load", initialize);

var map;
var elev;

var latLng = new Array();

//ページ読み込み完了時に実行する関数
function initialize() {

	//初期位置
	var initLat =34.482254;
	var initLon= 136.824878;
	var myLocation = new google.maps.LatLng(34.482254, 136.824878);
	latLng[0] = new google.maps.LatLng(34.482254, 136.824878-0.005);
	latLng[1] = myLocation;
	//latLng[2] = new google.maps.LatLng(34.482254, 136.824878+1);

	document.getElementById('latitude').value = initLat;
	document.getElementById('longitude').value =initLon;


	// マップ表示
	map = new google.maps.Map(document.getElementById("map"), {
		center: myLocation,
		zoom:16,
		mapTypeId: google.maps.MapTypeId.ROADMAP
	});

	// ドラッグできるマーカーを表示
	var marker = new google.maps.Marker({
		position: myLocation,
		title: "My Location",
		draggable: true	// ドラッグ可能にする
	});
	marker.setMap(map);


	// マーカーのドロップ（ドラッグ終了）時のイベント
	google.maps.event.addListener( marker, 'dragend', function(ev){
		// イベントの引数evの、プロパティ.latLngが緯度経度。
		document.getElementById('latitude').value = ev.latLng.lat();
		document.getElementById('longitude').value = ev.latLng.lng();

	});

	google.maps.event.addListener( marker,'click', function(ev){
		//地図の中心を移動
		map.setCenter(myLocation);
	});
}

