using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TouchSpriteService.Common;
using TouchSpriteService.Service;

namespace TouchLocal.Controllers
{
    public class DeviceController : Controller
    {
        public string Runlua()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var result = new { code = 100, message = "参数错误" };

            var ip = WebHelper.GetRequestString("ip");
            var port = WebHelper.GetRequestInt("port", 50005);
            var auth = WebHelper.GetRequestString("auth");
            if (string.IsNullOrWhiteSpace(ip) ||
                string.IsNullOrWhiteSpace(auth))
            {
                return JsonConvert.SerializeObject(result);
            }
            var url = string.Format("http://{0}:{1}", ip, port);

            var ActionService = new localActionService();

            bool success = ActionService.Runlua(url, auth);

            if (success)
                result = new { code = 200, message = "发送成功" };
            else
                result = new { code = 200, message = "发送失败" };

            return JsonConvert.SerializeObject(result);
        }


        public string Stoplua()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var result = new { code = 100, message = "参数错误" };

            var ip = WebHelper.GetRequestString("ip");
            var port = WebHelper.GetRequestInt("port", 50005);
            var auth = WebHelper.GetRequestString("auth");
            if (string.IsNullOrWhiteSpace(ip) ||
                string.IsNullOrWhiteSpace(auth))
            {
                return JsonConvert.SerializeObject(result);
            }
            var url = string.Format("http://{0}:{1}", ip, port);

            var ActionService = new localActionService();

            bool success = ActionService.Stoplua(url, auth);

            if (success) result = new { code = 200, message = "停止成功" };
            else
                result = new { code = 200, message = "停止失败" };

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 设置路径并运行
        /// </summary>
        /// <returns></returns>
        //public string RunDevices()
        //{
        //    HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

        //    var result = new { code = 100, message = "参数错误" };

        //    var ActionService = new TouchSpriteService.authActionService();
        //    int success = 0, fail = 0;

        //    var deviceids = GetDevicesParam();

        //    var path = WebHelper.GetRequestString("path");

        //    var connectType = WebHelper.GetRequestString("connectType");
        //    bool IsUSB = connectType == "USB";

        //    //string luapath = string.Format("/var/mobile/Media/TouchSprite/lua/{0}/main.lua", path);
        //    string luapath = string.Format("/var/mobile/Media/TouchSprite/lua/{0}", path);

        //    #region 设置执行路径，并执行
        //    var taskSendDevices = new Task<string>[deviceids.Length];

        //    for (int i = 0; i < deviceids.Length; i++)
        //    {
        //        var deviceid = deviceids[i];
        //        taskSendDevices[i] = Task.Run(() =>
        //        {
        //            bool IsSend = true;
        //            if (!string.IsNullOrWhiteSpace(path))
        //            {
        //                IsSend = ActionService.setLuaPath(deviceid, luapath, IsUSB);
        //            }

        //            if (IsSend)
        //                return ActionService.Runlua(deviceid, IsUSB);
        //            else
        //                return "fail";
        //        });
        //    }
        //    Task.WaitAll(taskSendDevices);

        //    foreach (var task in taskSendDevices)
        //    {
        //        if (task.Result == "ok") success++;
        //        else fail++;
        //    }
        //    #endregion

        //    string message = string.Format("{0}台设备命令执行成功，{1}台失败", success, fail);
        //    result = new { code = 200, message = message };

        //    return JsonConvert.SerializeObject(result);
        //}

        //public string GroupStoplua()
        //{
        //    HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

        //    var result = new { code = 100, message = "参数错误" };

        //    var groupid = WebHelper.GetFormInt("groupid");

        //    var connectType = WebHelper.GetRequestString("connectType");
        //    bool IsUSB = connectType == "USB";

        //    var service = new TouchSpriteService.Business.deviceService();
        //    var list = service.GetDevice2GroupDetail(" groups.ID=" + groupid);

        //    int success = 0, fail = 0;
        //    var ActionService = new TouchSpriteService.authActionService();
        //    //foreach (var d in list)
        //    //{
        //    //    var luaReturn = ActionService.Stoplua(d.deviceid);
        //    //    if (luaReturn == "ok") success++;
        //    //    else fail++;
        //    //}

        //    var taskDevices = new Task<string>[list.Count];
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        var deviceid = list[i].deviceid;
        //        taskDevices[i] = Task.Run(() =>
        //        {
        //            return ActionService.Stoplua(deviceid, IsUSB);
        //        });
        //    }
        //    Task.WaitAll(taskDevices);
        //    foreach (var task in taskDevices)
        //    {
        //        if (task.Result == "ok") success++;
        //        else fail++;
        //    }

        //    string message = string.Format("{0}台设备停止执行,{1}台失败", success, fail);
        //    result = new { code = 200, message = message };

        //    return JsonConvert.SerializeObject(result);
        //}



    }
}