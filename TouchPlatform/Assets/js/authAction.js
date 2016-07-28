//ajax绑定设备列表
function BindDevice() {
    $.ajax({
        type: 'POST',
        url: domain + "/api/Devicelist",
        dataType: 'json',
        data: { groupid: $(".select-groups").val() },
        success: function (data) {
            var html = template('tr-template', data);
            $("#tr-panel").html(html);

            $(".devicecount").html(data.data.length);
            $(".groupname").html($(".select-groups").find("option:selected").text());
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert("XMLHttpRequest:" + XMLHttpRequest + ",textStatus" + textStatus + ",errorThrown" + errorThrown);
            console.log('error:BindDevice');
        }
    });
}


//ajax绑定分组Select
function BindGroup() {
    var source = '{{each list as d i}}'
                + '<option value="{{d.ID}}">{{d.groupname}}</option>'
                + '{{/each}}';
    $.ajax({
        type: 'POST',
        url: domain + "/api/Grouplist",
        async: false,
        dataType: 'json',
        data: {},
        success: function (data) {
            var render = template.compile(source);
            var html = render({
                list: data.data
            });
            html = '<option value="0">选择分组</option>' + html;
            $(".select-groups").html(html);
            $(".d-groups").html(html);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert("XMLHttpRequest:" + XMLHttpRequest + ",textStatus" + textStatus + ",errorThrown" + errorThrown);
            console.log('error:BindGroup');
        }
    });
}

function SaveDevice() {
    $.ajax({
        type: 'POST',
        url: domain + "/api/addGroup",
        dataType: 'json',
        data: $("#g-form").serialize(),
        success: function (data) {
            if (data.code == 200) {
                BindInfo();

                $('.addgroupPanel').hide();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert("XMLHttpRequest:" + XMLHttpRequest + ",textStatus" + textStatus + ",errorThrown" + errorThrown);
            console.log('error:.btn-addGroup');
        }
    });
}


//删除设备
function DelDevice() {
    var ids = new Array();
    $('table .inbox-check').each(function () {
        if ($(this).prop('checked')) {
            ids.push($(this).val());
        }
    });
    if (ids.length == 0) {
        alert("请选择要删除的设备");
        return false;
    }
    $.ajax({
        type: 'GET',
        url: domain + "/api/DelDevice",
        dataType: 'json',
        data: { ids: ids.join(',') },
        success: function (data) {
            if (data.code == 200) {
                alert('删除成功');
                BindDevice();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log('error:DelDevice');
        }
    });
}

//打开设备面板
function de2group(deviceid) {
    $('.mod-de2group-panel').show();
    $.ajax({
        type: 'GET',
        url: domain + "/api/GetDeviceGroupsDetail?deviceid=" + deviceid,
        async: false,
        dataType: 'json',
        data: {},
        success: function (data) {
            var d = data.data;
            $(".d-deviceid").val(d.deviceid);
            $(".d-groups").val(d.groupid);
            $(".d-username").val(d.username);
            $(".d-ip").val(d.ip);
            $(".d-sortcode").val(d.sortcode);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert("XMLHttpRequest:" + XMLHttpRequest + ",textStatus" + textStatus + ",errorThrown" + errorThrown);
            console.log('error:de2group');
        }
    });

}

//保存设备分组
function SaveGroupDevice() {
    $.ajax({
        type: 'POST',
        url: domain + "/api/SaveGroupDevice",
        dataType: 'json',
        data: $("#d-form").serialize(),
        success: function (data) {
            if (data.code == 200) {
                alert(data.message);
                BindInfo();
                $('.mod-de2group-panel').hide();
            }
            else
                alert(data.message);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert("XMLHttpRequest:" + XMLHttpRequest + ",textStatus" + textStatus + ",errorThrown" + errorThrown);
            console.log('error:SaveGroupDevice');
        }
    });
}

function GroupRunlua() {
    var groupid = $(".select-groups").val();
    var luaName = $(".select-luas").val();
    if (groupid == 0 || luaName == 0)
    {
        alert('请选择要执行的脚本和分组!');
        return;
    }

    $('.btn-command-icon').html('<i class="fa fa-spinner fa-spin m-right-xs"></i>');
    $.ajax({
        type: 'POST',
        url: domain + "/api/SetLuaPath",
        dataType: 'json',
        data: { groupid: groupid, path: luaName },
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
    var selectDevices = new Array();
    $('#tr-panel input[type=checkbox]').each(function () {
        if ($(this).prop('checked')) {
            selectDevices.push($(this).attr("deviceid"));
        }
    });

    //先设置执行的路径
    var groupid = $(".select-groups").val();
    var luaName = $(".select-luas").val();
    if ( luaName == 0) {
        alert('请选择要执行的脚本和分组!');
        return;
    }

    var deviceids = selectDevices.join(',');
    $.ajax({
        type: 'POST',
        url: domain + "/api/SetLuaPath",
        dataType: 'json',
        data: { groupid: 0, deviceids: deviceids, path: luaName },
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
    $.ajax({
        type: 'POST',
        url: domain + "/api/Stoplua",
        dataType: 'json',
        data: { deviceids: ids },
        success: function (data) {
            alert(data.message);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error:StopluaByDeviceIds');
        }
    });
}



function LooperDevices() {
    setInterval(function () {
        var groupid = $(".select-groups").val();

        var AllDevices = new Array();
        $('#tr-panel input[type=checkbox]').each(function () {
            AllDevices.push($(this).attr("deviceid"));
        });

        if (groupid != "0") {
            $.ajax({
                type: 'POST',
                url: domain + "/api/getStatus",
                //async: false,
                dataType: 'json',
                //data: { groupid: groupid, deviceids: AllDevices.join(',') },
                data: { deviceids: AllDevices.join(',') },
                success: function (data) {
                    if (data.code == 200) {
                        $(data.list).each(function (index, d) {
                            if (d != null) {
                                $('.tr' + d.ID).find('.action-status').html(d.status);
                            }
                        })
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log('Looper Error');
                }
            });
        }
    }, 5000);
}

function reboot(type) {
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
        data: { groupid: groupid, deviceids: selectDevices.join(','), type: type },
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


//打开设置脚本路径panel
function OpenSetLuaPathPanel()
{
    var groupid = $(".select-groups").val();
    if (groupid == 0)
    {
        alert('请选择分组');
        return;
    }
    $('.luaPath-Panel').show(200);

    var treestr = $.ajax({
        url: domain + '/API/getSourceList',
        async: false,
        method: 'GET',
        data: { path: 'lua' },
    }).responseText;
    var treejson = JSON.parse(treestr);
    var html = template('luatree-template', treejson.data);
    $('.luapath-body').html(html);
}

function SetLuaPath(isRun)
{
    var radio = $('.luapath-body input[type=radio]:checked');
    if (radio.val() == undefined) {
        alert("请选择脚本文件夹");
        return;
    }
    var groupid = $(".select-groups").val();

    $('.btn-SetPath').html('<i class="fa fa-spinner fa-spin m-right-xs"></i>发送中...');
    $('.btn-SetPathRun').html('<i class="fa fa-spinner fa-spin m-right-xs"></i>发送中...');

    $.ajax({
        type: 'POST',
        url: domain + "/api/SetLuaPath",
        dataType: 'json',
        data: { groupid: groupid, path: radio.val(),send:"1" },
        //async: false,
        success: function (data) {
            if (data.code == 200) {
                if (isRun == 1) {
                    $(".select-luas").val(radio.val());
                    GroupRunlua();
                    //alert("保存并更新成功")
                }
                else {
                    alert("发送成功")
                }
            }
            else
                alert(data.message);

            $('.btn-SetPath').html('发送');
            $('.btn-SetPathRun').html('发送并执行');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error:SetLuaPath');
            $('.btn-SetPath').html('发送');
            $('.btn-SetPathRun').html('发送并执行');
        }
    });

    
}


function BindLuaDir()
{
    var treestr = $.ajax({
        url: domain + '/API/getSourceList',
        async: false,
        method: 'GET',
        data: { path: 'lua' },
    }).responseText;
    var treejson = JSON.parse(treestr);


    var source = '{{each dics as d i}}'
                + '<option value="{{d}}">{{d}}</option>'
                + '{{/each}}';
    var render = template.compile(source);
    var html = render({
        dics: treejson.data.dics
    });
    html = '<option value="0">选择脚本</option>' + html;
    $(".select-luas").html(html);
}


function ViewerDevice()
{
    var selectDevices = new Array();
    $('#tr-panel input[type=checkbox]').each(function () {
        if ($(this).prop('checked')) {
            selectDevices.push($(this).attr("deviceid"));
        }
    });
    if (selectDevices.length == 0)
    {
        alert('请勾选你要查看的设备');
        return false;
    }
    //location.href = "snapshot.html?deviceids=" + selectDevices.join(',');
    //location.href = "iframe.html?q=/Master/socket&deviceids=" + selectDevices.join(',');
    window.open("iframe.html?q=/Master/socket&deviceids=" + selectDevices.join(','));
}