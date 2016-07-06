
var LuaConfig = function (luaName) {

    $.ajax({
        type: 'POST',
        url: domain + "/lua/"+luaName,
        dataType: 'json',
        data: $("#form" + luaName).serialize(),
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