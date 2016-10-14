//获取auth
function GetAuth(deviceids) {
    var d = $.ajax({
        type: 'POST',
        url: domain + "/api/GetAuth",
        dataType: 'json',
        data: { deviceids: deviceids },
        async: false
    }).responseText;
    var json = JSON.parse(d);

    return json.data;
}

//跨域问题   直接前端发送请求
function requestDevice(deviceids, params) {
    var options = $.extend({
        query: "",          //请求的地址+参数
        method: "GET",
        timeout: 10000,     //超时
        dataType: "text",
        data: {},
        IsUSB: false,
        header: false,        //是否需要header标记
        callback: function () { }   //回调
    }, params);


    if (deviceids == "") {
        options.callback();
        console.log('deviceids is null');
        return;
    }
    var data = GetAuth(deviceids);
    if (typeof (data) == "undefined" || data.length == 0) {
        alert("请求Auth失败");
        options.callback();
        return;
    }

    //var connectType =  IsUSB ? "USB" : "WIFI";

    //异步请求多个设备，等待最后结果调用回调函数
    //var awaitParams = { finish: false, result: "" };
    var awaitResult = new Array();
    var await = setInterval(function () {
        var i = 0;
        for (var d in awaitResult)
            i += awaitResult[d].finish ? 1 : 0;

        if (i == data.length) {
            clearInterval(await);
            params.callback(awaitResult);
        }
    }, 100);

    for (var i in data) {
        var d = data[i];

        //如果没有usbip也用wifi请求
        var ip = options.IsUSB && d.usbip != "" ? d.usbip : d.ip;
        var host = $.format("{0}:{1}", ip, d.port);

        //var postData = $.extend(options.data, { host: host, query: options.query });
        var postData = options.data;
        var q = $.format("?host={0}&query={1}", host, options.query);

        $.ajax({
            url: (options.IsUSB ? domain_usb : domain) + "/home/relayapi" + q,
            timeout: options.timeout,
            dataType: options.dataType,
            type: options.method,
            data: postData,
            //headers: { Auth: d.auth },
            beforeSend: function (xhr) {
                if (options.header)
                    xhr.setRequestHeader("Auth", d.auth);
            },
            success: function (data) {
                awaitResult.push({ finish: true, data: d, result: data });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                awaitResult.push({ finish: true, data: d, result: errorThrown });

                if (errorThrown == "timeout") { }
                console.log('error:requestDevice:' + errorThrown);
            }
        });
    }

}


function getStatus() {

    $('.tip-panel').show();

    var IsUSB = $("#connectType").prop('checked') || false;
    var deviceids = getAllDeviceIds();

    requestDevice(deviceids, {
        query: "status",
        method: "GET",
        timeout: 12000,
        IsUSB: IsUSB,
        header: true,
        callback: function (d) {
            for (var i in d) {
                var line = d[i];
                var status = "", result = line.result || "";

                if (result == "f00")
                    status = "空闲";
                else if (result == "f01")
                    status = "运行";
                else if (result == "f02")
                    status = "录制";
                else if (result == "timeout")
                    status = "超时";
                else
                    status = "离线";

                $('.tr' + line.data.ID).find('.action-status').html(status);
            }
            $('.tip-panel').hide();
        }
    });
}

function setLuaPath(params) {
    var options = $.extend({
        deviceids: "",
        IsUSB: false,
        luaName: "",
        callback: function () { }   //回调
    }, params);

    if (options.luaName == 0) {
        alert('请选择要执行的脚本和分组!');
        return;
    }

    requestDevice(options.deviceids, {
        query: "setLuaPath",
        method: "POST",
        timeout: 8000,
        IsUSB: options.IsUSB,
        //data: { path: "/var/mobile/Media/TouchSprite/lua/" + options.luaName },
        data: '{"path": "/var/mobile/Media/TouchSprite/lua/' + options.luaName + '" }',
        header: true,
        callback: function (d) {
            options.callback(d);
        }
    });
}

//运行脚本先设置手机的脚本路径
//然后发送执行命令请求
function runLua() {

    var IsUSB = $("#connectType").prop('checked') || false;
    var deviceids = getCheckDeviceIds();
    if (deviceids == "") {
        alert('请勾选要操作的设备!');
        return;
    }

    var luaName = $(".select-luas").val();
    if (luaName == 0) {
        alert('请选择要执行的脚本和分组!');
        return;
    }

    $('.tip-panel').show();

    //设置脚本路径完成后调用方法
    var callback = function (res) {
        res = res || new Array();
        //var success = res.filter(function (item) {
        //    return item.result == "ok";
        //})
        var success = new Array();
        for (var i in res) {
            if (res[i].result == "ok") {
                success.push(res[i].data.deviceid);
            }
        }
        if (success.length == 0) {
            alert("命令发送失败");
            $('.tip-panel').hide();
            return;
        }

        //发送命令
        requestDevice(success.join(','), {
            query: "runLua",
            method: "GET",
            timeout: 10000,
            IsUSB: IsUSB,
            header: true,
            callback: function (d) {
                var success = res.filter(function (item) { return item.result == "ok"; });
                alert(success.length + "台设备命令发送成功");
                $('.tip-panel').hide();
            }
        });
    }

    setLuaPath({
        deviceids: deviceids,
        IsUSB: IsUSB,
        luaName: luaName,
        callback: callback
    });
}