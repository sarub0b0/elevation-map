$(function(){
	$("btnGetElevation").click(function(){

		$.ajax({
			type:"POST",
			url: "createMap.php",
			data:{
				"key":'a'
			},
			success: function(){

			}
		});
	});
});
