
var LuaConfig = function (luaName) {

    var data = new FormData($("#form" + luaName)[0]);

    $.ajax({
        type: 'POST',
        url: domain + "/lua/"+luaName,
        dataType: 'json',
        cache: false,
        processData: false,
        contentType: false,
        enctype: 'multipart/form-data',
        data: data,
        success: function (data) {
            alert(data.message);
            if (data.code == 200) {
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error:' + luaName);
        }
    });
}