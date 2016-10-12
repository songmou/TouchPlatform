
function GroupRunlua() {

    var IsUSB = $("#connectType").prop('checked') || false;
    var connectType = IsUSB ? "USB" : "WIFI";

    var groupid = $(".select-groups").val();
    var luaName = $(".select-luas").val();
    if (groupid == 0 || luaName == 0) {
        alert('请选择要执行的脚本和分组!');
        return;
    }

    $('.btn-command-icon').html('<i class="fa fa-spinner fa-spin m-right-xs"></i>');
    $.ajax({
        type: 'POST',
        url: domain + "/api/SetLuaPath",
        dataType: 'json',
        data: { groupid: groupid, path: luaName, connectType: connectType },
        async: false,
        success: function (data) {
            if (data.code == 200) {
                //执行命令
                RunluaByGroupID(groupid);
                $('.btn-command-icon').html('<span class="caret"></span>');
            }
            else
                alert(data.message);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error:GroupLuaPath');
        }
    });

}

function GroupStoplua() {
    var groupid = $(".select-groups").val();
    StopluaByGroupID(groupid);
}

//勾选的设备
function Runlua() {

    var IsUSB = $("#connectType").prop('checked') || false;
    var connectType = IsUSB ? "USB" : "WIFI";

    var selectDevices = new Array();
    $('#tr-panel input[type=checkbox]').each(function () {
        if ($(this).prop('checked')) {
            selectDevices.push($(this).attr("deviceid"));
        }
    });

    //先设置执行的路径
    var groupid = $(".select-groups").val();
    var luaName = $(".select-luas").val();
    if (luaName == 0) {
        alert('请选择要执行的脚本和分组!');
        return;
    }

    var deviceids = selectDevices.join(',');
    $.ajax({
        type: 'POST',
        url: domain + "/api/SetLuaPath",
        dataType: 'json',
        data: { groupid: 0, deviceids: deviceids, path: luaName, connectType: connectType },
        success: function (data) {
            if (data.code == 200) {
                //执行命令
                RunluaByDeviceIds(deviceids);
            }
            else
                alert(data.message);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error:GroupLuaPath');
        }
    });
}

//勾选的设备
function Stoplua() {
    var selectDevices = new Array();
    $('#tr-panel input[type=checkbox]').each(function () {
        if ($(this).prop('checked')) {
            selectDevices.push($(this).attr("deviceid"));
        }
    });
    StopluaByDeviceIds(selectDevices.join(','));
}


function RunluaByGroupID(groupid) {

    var IsUSB = $("#connectType").prop('checked') || false;
    var connectType = IsUSB ? "USB" : "WIFI";

    $.ajax({
        type: 'POST',
        url: domain + "/api/GroupRunlua",
        dataType: 'json',
        data: { groupid: groupid },
        async: false,
        success: function (data) {
            alert(data.message);
            if (data.code == 200) {
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error:RunluaByGroupID');
        }
    });
}

function RunluaByDeviceIds(ids) {

    var IsUSB = $("#connectType").prop('checked') || false;
    var connectType = IsUSB ? "USB" : "WIFI";

    $.ajax({
        type: 'POST',
        url: domain + "/api/Runlua",
        dataType: 'json',
        data: { deviceids: ids },
        success: function (data) {
            alert(data.message);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error:RunluaByDeviceIds');
        }
    });
}

function StopluaByGroupID(groupid) {

    var IsUSB = $("#connectType").prop('checked') || false;
    var connectType = IsUSB ? "USB" : "WIFI";

    $.ajax({
        type: 'POST',
        url: domain + "/api/GroupStoplua",
        dataType: 'json',
        data: { groupid: groupid },
        success: function (data) {
            alert(data.message);
            if (data.code == 200) {
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error:StopluaByGroupID');
        }
    });
}

function StopluaByDeviceIds(ids) {

    var IsUSB = $("#connectType").prop('checked') || false;
    var connectType = IsUSB ? "USB" : "WIFI";

    $.ajax({
        type: 'POST',
        url: domain + "/api/Stoplua",
        dataType: 'json',
        data: { deviceids: ids, connectType: connectType },
        success: function (data) {
            alert(data.message);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error:StopluaByDeviceIds');
        }
    });
}


function reboot(type) {
    var IsUSB = $("#connectType").prop('checked') || false;
    var connectType = IsUSB ? "USB" : "WIFI";

    var selectDevices = new Array();
    $('#tr-panel input[type=checkbox]').each(function () {
        if ($(this).prop('checked')) {
            selectDevices.push($(this).attr("deviceid"));
        }
    });
    var groupid = $(".select-groups").val();

    $.ajax({
        type: 'POST',
        url: domain + "/api/reboot",
        dataType: 'json',
        data: {
            groupid: groupid,
            deviceids: selectDevices.join(','),
            type: type,
            connectType: connectType
        },
        success: function (data) {
            alert(data.message);
            if (data.code == 200) {
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error:reboot');
        }
    });
}


function LooperDevices() {
    //setInterval(function () {
    var IsUSB = $("#connectType").prop('checked') || false;
    var connectType = IsUSB ? "USB" : "WIFI";

    var groupid = $(".select-groups").val();

    var AllDevices = new Array();
    $('#tr-panel input[type=checkbox]').each(function () {
        AllDevices.push($(this).attr("deviceid"));
    });

    if (groupid != "0") {
        $('.tip-panel').show();

        $.ajax({
            type: 'POST',
            url: domain + "/api/getStatus",
            //async: false,
            dataType: 'json',
            //data: { groupid: groupid, deviceids: AllDevices.join(',') },
            data: { deviceids: AllDevices.join(','), connectType: connectType },
            success: function (data) {
                if (data.code == 200) {
                    $(data.list).each(function (index, d) {
                        if (d != null) {
                            $('.tr' + d.ID).find('.action-status').html(d.status);
                        }
                    })
                }
                $('.tip-panel').hide();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log('Looper Error');
                $('.tip-panel').hide();
            }
        });
    }
    //}, 10000);
}

function ViewerDevice() {

    var IsUSB = $("#connectType").prop('checked') || false;
    var connectType = IsUSB ? "USB" : "WIFI";

    //connectType = connectType || "WIFI";
    var selectDevices = new Array();
    $('#tr-panel input[type=checkbox]').each(function () {
        if ($(this).prop('checked')) {
            selectDevices.push($(this).attr("deviceid"));
        }
    });
    if (selectDevices.length == 0) {
        alert('请勾选你要查看的设备');
        return false;
    }
    //location.href = "snapshot.html?deviceids=" + selectDevices.join(',');
    //location.href = "iframe.html?q=/Master/socket&deviceids=" + selectDevices.join(',');
    window.open(domain + "/Master/socket?deviceids=" + selectDevices.join(',') + "&t=" + connectType);
}
