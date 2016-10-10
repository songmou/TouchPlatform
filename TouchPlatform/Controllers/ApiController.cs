using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TouchModel;
using TouchSpriteService;
using TouchSpriteService.Business;
using TouchSpriteService.Common;

namespace TouchPlatform.Controllers
{
    public class ApiController : Controller
    {
        #region 设备信息
        public string addDevice()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };
            string jsonStr = Request.Form["string"];
            if (string.IsNullOrWhiteSpace(jsonStr))
            {
                return JsonConvert.SerializeObject(result);
            }

            List<deviceClient> list = JsonConvert.DeserializeObject<List<deviceClient>>(jsonStr);
            foreach (deviceClient d in list)
            {
                TouchSpriteService.DataReflector<devices> service = new TouchSpriteService.DataReflector<devices>();
                devices model = service.Get("deviceid", d.deviceid);
                if (model == null)
                {
                    model = new devices();
                    model.deviceid = d.deviceid;
                    model.devname = d.devname;
                    model.ip = d.ip;
                    model.usbip = "";
                    model.port = d.port;
                    model.osType = d.osType;
                    model.username = d.ip;
                    model.remark = "";
                    model.status = "";
                    model.tsversion = d.tsversion;
                    model.createdate = model.updatedate = DateTime.Now;
                    service.Add(model);
                }
                else
                {
                    model.deviceid = d.deviceid;
                    model.devname = d.devname;
                    model.ip = d.ip;
                    model.usbip = d.usbip;
                    model.port = d.port;
                    model.osType = d.osType;
                    model.tsversion = d.tsversion;
                    model.updatedate = DateTime.Now;
                    service.Update(model);
                }
            }

            result = new { code = 200, message = "操作成功" };
            return JsonConvert.SerializeObject(result);
        }

        public string Devicelist()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            List<device2GroupDetail> list = null;
            var result = new { code = 100, message = "参数错误", data = list };

            int groupid = WebHelper.GetRequestInt("groupid");


            //DataReflector<devices> service = new DataReflector<devices>();
            //list = service.Get();

            string where = "";
            if (groupid != 0)
            {
                where = " (groups.ID=" + groupid + " or groups.ID is null)";
            }
            var service = new TouchSpriteService.Business.deviceService();
            list = service.GetDevice2GroupDetail(where);

            result = new { code = 200, message = "查询成功", data = list };
            return JsonConvert.SerializeObject(result);
        }

        public string DelDevice()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var ids = WebHelper.GetQueryString("ids");
            foreach (string d in ids.Split(','))
            {
                int id = 0;
                int.TryParse(d, out id);
                TouchSpriteService.DataReflector<devices> service = new TouchSpriteService.DataReflector<devices>();

                service.Delete(id);
            }

            var result = new { code = 200, message = "删除成功" };
            return JsonConvert.SerializeObject(result);
        }
        #endregion

        #region 分组信息
        public string addGroup(groups d)
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            if (string.IsNullOrWhiteSpace(d.groupname))
            {
                return JsonConvert.SerializeObject(result);
            }

            TouchSpriteService.DataReflector<groups> service = new TouchSpriteService.DataReflector<groups>();
            groups model = service.Get(d.ID);
            if (model == null)
            {
                model = new groups();
                model.groupname = d.groupname;
                model.auth = "";
                model.sortcode = d.sortcode;
                model.createdate = model.updatedate = DateTime.Now;
                service.Add(model);
            }
            else
            {

                model.groupname = d.groupname;
                model.sortcode = d.sortcode;
                model.updatedate = DateTime.Now;
                service.Update(model);
            }

            result = new { code = 200, message = "操作成功" };
            return JsonConvert.SerializeObject(result);
        }

        public string Grouplist()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            List<groups> list = null;
            var result = new { code = 100, message = "参数错误", data = list };

            DataReflector<groups> service = new DataReflector<groups>();
            list = service.Get();

            var groupid = WebHelper.GetRequestInt("groupid");
            if (groupid != 0)
                list = list.Where(q => q.ID == groupid).ToList();

            result = new { code = 200, message = "查询成功", data = list };
            return JsonConvert.SerializeObject(result);
        }

        public string GroupDetail()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            int id = WebHelper.GetRequestInt("id");
            groups model = null;
            var result = new { code = 100, message = "参数错误", data = model };
            DataReflector<groups> service = new DataReflector<groups>();
            model = service.Get(id);
            result = new { code = 200, message = "请求成功", data = model };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 设备信息（包括分组）
        /// </summary>
        /// <returns></returns>
        public string GetDeviceGroupsDetail()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            TouchSpriteService.Business.deviceService service = new TouchSpriteService.Business.deviceService();

            string deviceid = WebHelper.GetQueryString("deviceid");
            var model = service.GetDeviceDetail(WebHelper.SqlFilter(deviceid));

            var result = new { code = 200, message = "查询成功", data = model };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 保存设备及设备分组信息
        /// </summary>
        /// <returns></returns>
        public string SaveGroupDevice()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var groupid = WebHelper.GetFormInt("groupname");
            var deviceid = WebHelper.GetFormString("deviceid");
            var username = WebHelper.GetFormString("username");
            var sortcode = WebHelper.GetFormInt("sortcode");
            var ip = WebHelper.GetFormString("ip");
            var usbip = WebHelper.GetFormString("usbip");

            devices model = null;
            var result = new { code = 100, message = "参数错误", data = model };
            if (groupid == 0)
            {
                return JsonConvert.SerializeObject(result);
            }

            TouchSpriteService.DataReflector<devices> service = new TouchSpriteService.DataReflector<devices>();
            model = service.Get("deviceid", deviceid);
            if (model == null)
            {
                return JsonConvert.SerializeObject(result);
            }
            model.username = WebHelper.SqlFilter(username);
            model.ip = WebHelper.SqlFilter(ip);
            model.usbip = WebHelper.SqlFilter(usbip);
            model.sortcode = sortcode;
            model.updatedate = DateTime.Now;
            service.Update(model);

            TouchSpriteService.DataReflector<group_device> group_deviceService = new TouchSpriteService.DataReflector<group_device>();
            var group_deviceModel = group_deviceService.Get("deviceid", deviceid);
            if (group_deviceModel == null)
            {
                group_deviceModel = new group_device();
                group_deviceModel.deviceid = deviceid;
                group_deviceModel.groupid = groupid;
                group_deviceModel.updatedate = DateTime.Now;
                group_deviceService.Add(group_deviceModel);
            }
            else
            {
                group_deviceModel.deviceid = deviceid;
                group_deviceModel.groupid = groupid;
                group_deviceModel.updatedate = group_deviceModel.createdate = DateTime.Now;
                group_deviceService.Update(group_deviceModel);
            }

            TouchSpriteService.Common.SimpleCacheProvider cache = TouchSpriteService.Common.SimpleCacheProvider.GetInstance();
            cache.SetCache(deviceid, null);

            result = new { code = 200, message = "保存成功", data = model };
            return JsonConvert.SerializeObject(result);

        }

        #endregion

        #region 公共方法
        /// <summary>
        /// 获取请求的所有设备编号
        /// </summary>
        /// <returns></returns>
        private string[] GetDevicesParam()
        {
            List<devices> list = null;
            var result = new { code = 100, message = "参数错误", list = list };

            var groupid = WebHelper.GetRequestInt("groupid");
            string deviceStr = WebHelper.GetRequestString("deviceids");
            if (groupid == 0 && deviceStr == "")
            {
                string[] temp = { };
                return temp;
            }

            var listDetail = new List<device2GroupDetail>();
            if (groupid != 0)
            {
                deviceStr = "";
                var service = new TouchSpriteService.Business.deviceService();
                listDetail = service.GetDevice2GroupDetail(" groups.ID=" + groupid);
                foreach (var d in listDetail)
                {
                    deviceStr += d.deviceid + ",";
                }
                deviceStr = deviceStr.TrimEnd(',');
            }
            return deviceStr.Split(',');
        }

        public string GetAuth()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new
            {
                code = 100,
                message = "参数错误",
                data = new { auth = "", ip = "", url = "", lastTime = default(DateTime) }
            };

            var groupid = WebHelper.GetRequestInt("groupid");
            var deviceid = WebHelper.GetRequestString("deviceid");

            getAuthService AuthService = new getAuthService();
            if (deviceid == "")
            {
                return JsonConvert.SerializeObject(result);
            }

            var auth = AuthService.GetAuth(deviceid);

            var service = new deviceService();
            var model = service.GetCacheDeviceDetail(deviceid);

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";
            string ip = model.ip;
            if (IsUSB && !string.IsNullOrWhiteSpace(model.usbip))
            {
                ip = model.usbip;
            }
            var url = string.Format("http://{0}:{1}", model.ip, model.port);

            result = new
            {
                code = 200,
                message = "请求成功",
                data = new { auth = auth, ip = ip, url = url, lastTime = model.lastTime }
            };

            return JsonConvert.SerializeObject(result);
        }

        #endregion

        #region 运行脚本
        /// <summary>
        /// 通过分组运行 脚本
        /// </summary>
        /// <returns></returns>
        public string GroupRunlua()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var result = new { code = 100, message = "参数错误" };

            var groupid = WebHelper.GetFormInt("groupid");

            //DataReflector<groups> Service = new DataReflector<groups>();
            //var group = Service.Get(groupid);

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            var service = new TouchSpriteService.Business.deviceService();
            var list = service.GetDevice2GroupDetail(" groups.ID=" + groupid);

            int success = 0, fail = 0;
            var ActionService = new TouchSpriteService.authActionService();

            var taskDevices = new Task<string>[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                var d = list[i];
                taskDevices[i] = Task.Run(() =>
                {
                    return ActionService.Runlua(d.deviceid, IsUSB);
                });

                //var luaReturn = ActionService.Runlua(d.deviceid);
                //if (luaReturn == "ok") success++;
                //else fail++;
            }

            Task.WaitAll(taskDevices);
            foreach (var task in taskDevices)
            {
                if (task.Result == "ok") success++;
                else fail++;
            }

            string message = string.Format("{0}台设备命令发送成功，{1}台失败", success, fail);
            result = new { code = 200, message = message };

            return JsonConvert.SerializeObject(result);
        }

        public string Runlua()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var result = new { code = 100, message = "参数错误" };

            var deviceids = WebHelper.GetFormString("deviceids").Split(',');
            var ActionService = new TouchSpriteService.authActionService();
            int success = 0, fail = 0;
            //foreach (var deviceid in deviceids)
            //{
            //    var luaReturn = ActionService.Runlua(deviceid);
            //    if (luaReturn == "ok") success++;
            //    else fail++;
            //}

            var taskDevices = new Task<string>[deviceids.Length];
            for (int i = 0; i < deviceids.Length; i++)
            {
                var deviceid = deviceids[i];
                taskDevices[i] = Task.Run(() =>
                {
                    return ActionService.Runlua(deviceid);
                });
            }
            Task.WaitAll(taskDevices);
            foreach (var task in taskDevices)
            {
                if (task.Result == "ok") success++;
                else fail++;
            }

            string message = string.Format("{0}台设备命令发送成功，{1}台失败", success, fail);
            result = new { code = 200, message = message };

            return JsonConvert.SerializeObject(result);
        }


        /// <summary>
        /// 设置路径并运行
        /// </summary>
        /// <returns></returns>
        public string RunDevices()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var result = new { code = 100, message = "参数错误" };

            var ActionService = new TouchSpriteService.authActionService();
            int success = 0, fail = 0;

            var deviceids = GetDevicesParam();

            var path = WebHelper.GetRequestString("path");

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            //string luapath = string.Format("/var/mobile/Media/TouchSprite/lua/{0}/main.lua", path);
            string luapath = string.Format("/var/mobile/Media/TouchSprite/lua/{0}", path);

            #region 设置执行路径，并执行
            var taskSendDevices = new Task<string>[deviceids.Length];

            for (int i = 0; i < deviceids.Length; i++)
            {
                var deviceid = deviceids[i];
                taskSendDevices[i] = Task.Run(() =>
                {
                    bool IsSend = true;
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        IsSend = ActionService.setLuaPath(deviceid, luapath, IsUSB);
                    }

                    if (IsSend)
                        return ActionService.Runlua(deviceid, IsUSB);
                    else
                        return "fail";
                });
            }
            Task.WaitAll(taskSendDevices);

            foreach (var task in taskSendDevices)
            {
                if (task.Result == "ok") success++;
                else fail++;
            }
            #endregion

            string message = string.Format("{0}台设备命令执行成功，{1}台失败", success, fail);
            result = new { code = 200, message = message };

            return JsonConvert.SerializeObject(result);
        }
        #endregion

        #region 停止脚本
        public string GroupStoplua()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var result = new { code = 100, message = "参数错误" };

            var groupid = WebHelper.GetFormInt("groupid");

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            var service = new TouchSpriteService.Business.deviceService();
            var list = service.GetDevice2GroupDetail(" groups.ID=" + groupid);

            int success = 0, fail = 0;
            var ActionService = new TouchSpriteService.authActionService();
            //foreach (var d in list)
            //{
            //    var luaReturn = ActionService.Stoplua(d.deviceid);
            //    if (luaReturn == "ok") success++;
            //    else fail++;
            //}

            var taskDevices = new Task<string>[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                var deviceid = list[i].deviceid;
                taskDevices[i] = Task.Run(() =>
                {
                    return ActionService.Stoplua(deviceid, IsUSB);
                });
            }
            Task.WaitAll(taskDevices);
            foreach (var task in taskDevices)
            {
                if (task.Result == "ok") success++;
                else fail++;
            }

            string message = string.Format("{0}台设备停止执行,{1}台失败", success, fail);
            result = new { code = 200, message = message };

            return JsonConvert.SerializeObject(result);
        }

        public string Stoplua()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var result = new { code = 100, message = "参数错误" };

            var ActionService = new TouchSpriteService.authActionService();
            int success = 0, fail = 0;

            var deviceids = GetDevicesParam();
            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            var taskDevices = new Task<string>[deviceids.Length];

            for (int i = 0; i < deviceids.Length; i++)
            {
                var deviceid = deviceids[i];
                taskDevices[i] = Task.Run(() =>
                {
                    return ActionService.Stoplua(deviceid, IsUSB);
                });
            }
            Task.WaitAll(taskDevices);
            foreach (var task in taskDevices)
            {
                if (task.Result == "ok") success++;
                else fail++;
            }

            string message = string.Format("{0}台设备停止执行,{1}台失败", success, fail);
            result = new { code = 200, message = message };

            return JsonConvert.SerializeObject(result);
        }
        #endregion

        #region 轮询状态
        /// <summary>
        /// 通过groupid更新设备状态
        /// 或者通过设备编号数组更新状态
        /// </summary>
        /// <returns></returns>
        public string getStatus()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            List<devices> list = null;
            var result = new { code = 100, message = "参数错误", list = list };

            //var groupid = WebHelper.GetFormInt("groupid");
            //string deviceStr = WebHelper.GetFormString("deviceids");
            //if (groupid == 0 && deviceStr == "")
            //{
            //    return JsonConvert.SerializeObject(result);
            //}

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            var deviceids = GetDevicesParam();
            if (deviceids.Length == 0)
            {
                return JsonConvert.SerializeObject(result);
            }

            list = new List<devices>();
            var ActionService = new TouchSpriteService.authActionService();

            var taskDevices = new Task<devices>[deviceids.Length];

            for (int i = 0; i < deviceids.Length; i++)
            {
                var deviceid = deviceids[i];
                taskDevices[i] = Task.Run(() =>
                {
                    return ActionService.getStatus(deviceid, IsUSB);
                });
            }
            Task.WaitAll(taskDevices);
            foreach (var task in taskDevices)
            {
                list.Add(task.Result);
            }

            result = new { code = 200, message = "已发送更新命令", list = list };

            return JsonConvert.SerializeObject(result);
        }
        #endregion

        #region 设备重启
        public string reboot()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            var deviceids = GetDevicesParam();

            string type = WebHelper.GetFormString("type");
            if (type == "" || deviceids.Length == 0)
            {
                return JsonConvert.SerializeObject(result);
            }

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            int success = 0, fail = 0;
            var ActionService = new TouchSpriteService.authActionService();

            var taskDevices = new Task<string>[deviceids.Length];

            for (int i = 0; i < deviceids.Length; i++)
            {
                var deviceid = deviceids[i];
                taskDevices[i] = Task.Run(() =>
                {
                    return ActionService.DoAction(deviceid, "reboot?type=1", IsUSB);
                });
                //var ActionService = new TouchSpriteService.authActionService();
                //var luaReturn = ActionService.DoAction(deviceid, "reboot?type=1");
                //if (luaReturn == "ok") success++;
                //else fail++;
            }
            Task.WaitAll(taskDevices);
            foreach (var task in taskDevices)
            {
                if (task.Result == "ok") success++;
                else fail++;
            }

            string message = string.Format("{0}台设备命令发送成功，{1}台失败", success, fail);
            result = new { code = 200, message = message };

            return JsonConvert.SerializeObject(result);
        }

        #endregion

        #region 文件管理
        /// <summary>
        /// 获取单个设备的文件列表
        /// </summary>
        /// <returns></returns>
        public string getFileList()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误", data = default(object) };

            string deviceid = WebHelper.GetRequestString("deviceid");
            if (deviceid == "")
            {
                return JsonConvert.SerializeObject(result);
            }

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            string[] roots = { "lua", "res", "log", "plugin" };
            string root = WebHelper.GetRequestString("root");
            if (!roots.Contains(root))
            {
                return JsonConvert.SerializeObject(result);
            }

            string path = WebHelper.GetRequestString("path");
            if (path == "")
            {
                path = "/";
            }

            var ActionService = new TouchSpriteService.authActionService();
            var luaReturn = ActionService.getFileList(deviceid, root, path, IsUSB);

            result = new { code = 200, message = "请求成功", data = JsonConvert.DeserializeObject(luaReturn) };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 获取服务器source目录下的文件信息
        /// </summary>
        /// <returns></returns>
        public string getSourceList()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            Object data = null;
            var result = new { code = 100, message = "参数错误", data = default(object) };

            string root = "~/source/";
            string path = root + WebHelper.GetRequestString("path", "/");
            path = Server.MapPath(path);
            root = Server.MapPath(root);

            string[] files = Directory.GetFiles(path);
            string[] Fullfiles = Directory.GetFiles(path);
            for (int i = 0; i < files.Count(); i++)
            {
                var tempArray = files[i].Split('\\');
                files[i] = tempArray.LastOrDefault();
                Fullfiles[i] = path.Replace(root, "").Replace('\\', '/');
            }
            string[] dics = Directory.GetDirectories(path);
            string[] Fulldics = Directory.GetDirectories(path);
            for (int i = 0; i < dics.Count(); i++)
            {
                var tempArray = dics[i].Split('\\');
                dics[i] = tempArray.LastOrDefault();
                Fulldics[i] = path.Replace(root, "").Replace('\\', '/');
            }

            data = new { files = files, dics = dics, Fullfiles = Fullfiles, Fulldics = Fulldics };
            result = new { code = 200, message = "请求成功", data = data };
            return JsonConvert.SerializeObject(result);
        }

        public string getluaList()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var result = new { code = 100, message = "参数错误", data = new Dictionary<string, string>() };

            //var array=String.Join(",", TsRemoteConfig.luaPathDic.Keys);

            result = new { code = 200, message = "请求成功", data = TsRemoteConfig.luaPathDic };
            return JsonConvert.SerializeObject(result);

        }


        /// <summary>
        /// 遍历循环将文件发送到设备
        /// </summary>
        /// <returns></returns>
        public string Updatelua()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            var deviceids = GetDevicesParam();

            string paths = WebHelper.GetFormString("paths");
            string[] Arrayfiles = paths.TrimEnd(',').Split(',');
            if (paths == "" || deviceids.Length == 0)
            {
                return JsonConvert.SerializeObject(result);
            }

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            foreach (var d in Arrayfiles)
            {
                FileInfo file = new FileInfo(Server.MapPath("~/source/" + d));
                if (!file.Exists)
                {
                    result = new { code = 101, message = paths + "文件不存在" };
                    return JsonConvert.SerializeObject(result);
                }
            }

            var ActionService = new TouchSpriteService.authActionService();

            int success = 0, fail = 0;
            var Current = System.Web.HttpContext.Current;

            //设备多任务
            var taskDevices = new Task[deviceids.Length];

            //先循环设备
            //foreach (var deviceid in deviceids)
            for (int z = 0; z < deviceids.Length; z++)
            {
                var deviceid = deviceids[z];
                taskDevices[z] = Task.Run(() =>
                 {
                     #region 多任务（同时向多台设备发送文件）
                     if (!string.IsNullOrWhiteSpace(deviceid))
                     {
                         var failNum = 0;

                         //文件发送多任务
                         var taskUploads = new Task<string>[Arrayfiles.Length];

                         //再循环发送文件
                         //foreach (var d in Arrayfiles)
                         for (int i = 0; i < Arrayfiles.Length; i++)
                         {
                             var d = Arrayfiles[i];
                             taskUploads[i] = Task.Run(() =>
                             {
                                 return ActionService.uploadlua(Current, deviceid, d, IsUSB);
                             });

                             //改造成异步方式
                             //string luaReturn = ActionService.uploadlua(deviceid, d);
                             //if (luaReturn == "ok") { }
                             //else { failNum++; } 
                         }

                         Task.WaitAll(taskUploads);

                         foreach (var task in taskUploads)
                         {
                             var luaReturn = task.Result;
                             if (luaReturn == "ok") { }
                             else { failNum++; }
                         }

                         if (failNum == 0)
                             success++;
                         else
                             fail++;
                     }
                     #endregion
                 });
            }

            Task.WaitAll(taskDevices);


            string message = string.Format("{0}台设备文件发送成功，{1}台失败", success, fail);
            result = new { code = 200, message = message };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 删除服务器上的文件或者文件夹
        /// </summary>
        /// <returns></returns>
        public string Deletelua()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            string paths = WebHelper.GetFormString("paths");
            string[] Arrayfiles = paths.TrimEnd(',').Split(',');
            if (paths == "")
            {
                return JsonConvert.SerializeObject(result);
            }

            //可能是文件或者文件夹（Todo）
            foreach (var d in Arrayfiles)
            {
                var sourcePath = Server.MapPath("~/source/" + d);
                var Exists = true;
                if (d.Split('.').Length == 2)
                    Exists = new FileInfo(sourcePath).Exists;//文件
                else
                    Exists = new DirectoryInfo(sourcePath).Exists;//文件夹

                if (!Exists)
                {
                    result = new { code = 101, message = paths + "文件夹不存在" };
                    return JsonConvert.SerializeObject(result);
                }
            }

            foreach (var d in Arrayfiles)
            {
                var sourcePath = Server.MapPath("~/source/" + d);
                if (d.Split('.').Length == 2)
                {
                    FileInfo file = new FileInfo(sourcePath);
                    if (!file.Exists)//文件
                    {
                        continue;
                    }
                    file.Delete();
                }
                else
                {
                    DirectoryInfo dir = new DirectoryInfo(sourcePath);
                    if (!dir.Exists)//文件夹
                    {
                        continue;
                    }
                    dir.Delete(true);
                }
            }
            result = new { code = 200, message = "删除成功" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 将ZIP文件上传到服务器，并解压
        /// </summary>
        /// <returns></returns>
        public string UploadZip()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            string dirname = WebHelper.GetFormString("dirname");
            HttpPostedFileBase zipfile = Request.Files["zipfile"];
            if (dirname == "" || zipfile == null)
                return JsonConvert.SerializeObject(result);
            if (zipfile.ContentLength == 0 || zipfile.ContentLength > 10 * 1024 * 1024)
            {
                result = new { code = 101, message = "文件大小不符合" };
                return JsonConvert.SerializeObject(result);
            }
            if (!zipfile.FileName.EndsWith(".zip"))
            {
                result = new { code = 102, message = "格式错误" };
                return JsonConvert.SerializeObject(result);
            }
            var luaDir = WebHelper.GetFormString("luaDir", "lua");
            var zipPath = Server.MapPath(string.Format("~/source/temp/{0}.zip", dirname));
            var zipExtraPath = Server.MapPath(string.Format("~/source/{0}/", luaDir));
            zipfile.SaveAs(zipPath);

            DirectoryInfo dire = new DirectoryInfo(zipExtraPath);
            if (dire.Exists)
            {
                dire.Delete(true);
            }

            //解压到指定目录
            ZipFile.ExtractToDirectory(zipPath, zipExtraPath);

            result = new { code = 200, message = "上传成功" };
            return JsonConvert.SerializeObject(result);

        }

        public string UploadFilesToDevices()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            var deviceids = GetDevicesParam();
            if (deviceids.Length == 0)
            {
                return JsonConvert.SerializeObject(result);
            }
            string dirname = WebHelper.GetRequestString("dirname");
            if (dirname != "") dirname = dirname + "/";

            var folderName = Server.MapPath("~/source/lua/" + dirname);

            var Current = System.Web.HttpContext.Current;

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            var ActionService = new TouchSpriteService.authActionService();
            var taskDevices = new Task<string>[deviceids.Length];
            for (int i = 0; i < deviceids.Length; i++)
            {
                var deviceid = deviceids[i];
                taskDevices[i] = Task.Run(() =>
                {
                    //循环所有文件
                    return loopsendSource(Current, folderName, deviceid, IsUSB) + ",";
                });
            }

            result = new { code = 200, message = "发送成功" };
            return JsonConvert.SerializeObject(result);
        }

        public string UploadFriendlineImg()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            var deviceids = GetDevicesParam();
            if (deviceids.Length == 0)
            {
                return JsonConvert.SerializeObject(result);
            }
            var folderName = Server.MapPath("~/source/lua/images/");

            var Current = System.Web.HttpContext.Current;

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            var ActionService = new TouchSpriteService.authActionService();
            var taskDevices = new Task<string>[deviceids.Length];
            for (int i = 0; i < deviceids.Length; i++)
            {
                var deviceid = deviceids[i];
                taskDevices[i] = Task.Run(() =>
                {
                    //循环所有文件
                    return loopsendSource(Current, folderName, deviceid, IsUSB) + ",";
                });
            }

            result = new { code = 200, message = "发送成功" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 设置设备的执行路径
        /// 手机设备要执行脚本
        /// 需要先设定执行路径 请求，再发送“执行命令” 请求
        /// </summary>
        /// <returns></returns>
        public string SetLuaPath()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            //文件夹名称
            string path = WebHelper.GetFormString("path");
            var groupid = WebHelper.GetFormInt("groupid");
            var deviceids = GetDevicesParam();


            if (deviceids.Length == 0 || path == "")
            {
                return JsonConvert.SerializeObject(result);
            }

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";


            //var folderFullName = Server.MapPath("~/source/lua/" + path);
            var folderFullName = Server.MapPath("~/source/lua/");
            var fileFullName = Server.MapPath(string.Format("~/source/lua/{0}", path));

            FileInfo TheFile = new FileInfo(fileFullName);
            if (!TheFile.Exists)
            {
                result = new { code = 101, message = "入口文件不存在" };
                return JsonConvert.SerializeObject(result);
            }

            var Current = System.Web.HttpContext.Current;

            //bool isSuccess = true;
            var ActionService = new TouchSpriteService.authActionService();
            //判断是否需要将服务器的文件发送到各个手机设备
            if (WebHelper.GetFormString("send") == "1")
            {
                var taskDevices = new Task<string>[deviceids.Length];

                for (int i = 0; i < deviceids.Length; i++)
                {
                    var deviceid = deviceids[i];
                    taskDevices[i] = Task.Run(() =>
                    {
                        //循环所有文件
                        return loopsendSource(Current, folderFullName, deviceid, IsUSB) + ",";
                    });
                }

                //Task.WaitAll(taskDevices);

                //foreach (var task in taskDevices)
                //{
                //    if (task.Result.IndexOf("fail") > -1)
                //        isSuccess = false;
                //}
            }

            //string luapath = string.Format("/var/mobile/Media/TouchSprite/lua/{0}/main.lua", path);
            string luapath = string.Format("/var/mobile/Media/TouchSprite/lua/{0}", path);
            //文件发送是否成功
            //if (isSuccess)

            //多任务发送命令
            var taskSendDevices = new Task<bool>[deviceids.Length];

            for (int i = 0; i < deviceids.Length; i++)
            {
                var deviceid = deviceids[i];
                taskSendDevices[i] = Task.Run(() =>
                {
                    return ActionService.setLuaPath(deviceid, luapath, IsUSB);
                });
            }
            Task.WaitAll(taskSendDevices);

            result = new { code = 200, message = "保存成功" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 循环某一个文件夹，将一个文件发送到某一个设备
        /// </summary>
        /// <param name="folderFullName">文件夹的实际路径</param>
        /// <param name="deviceid"></param>
        private string loopsendSource(HttpContext Current, string folderFullName, string deviceid, bool IsUSB = false)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);
            if (!TheFolder.Exists)
            {
                return "fail";
            }
            var result = "";
            var rootPath = Server.MapPath("~/source/");
            var ActionService = new TouchSpriteService.authActionService();

            FileInfo[] files = TheFolder.GetFiles();
            //var Current = System.Web.HttpContext.Current;

            //多任务  发送文件
            var taskfiles = new Task<string>[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo NextFile = files[i];
                taskfiles[i] = Task.Run(() =>
                {
                    string filePath = NextFile.FullName.Replace(rootPath, "").Replace('\\', '/');
                    return ActionService.uploadlua(Current, deviceid, filePath, IsUSB) + ",";
                });
            }
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                //递归循环
                result += loopsendSource(Current, NextFolder.FullName, deviceid, IsUSB) + ",";
            }

            Task.WaitAll(taskfiles);
            foreach (var task in taskfiles)
            {
                result += task.Result;
            }

            return result;
        }
        #endregion


        public ActionResult snapshot()
        {
            string deviceid = WebHelper.GetQueryString("deviceid");
            int compressInt = WebHelper.GetQueryInt("compress");
            int orient = WebHelper.GetQueryInt("orient");
            decimal compress = compressInt == 0 ? 0.1M : (decimal)(compressInt / 10);
            string ext = WebHelper.GetQueryString("ext", "jpg");

            var connectType = WebHelper.GetRequestString("connectType");
            bool IsUSB = connectType == "USB";

            var ActionService = new TouchSpriteService.authActionService();
            byte[] bytes = ActionService.snapshot(deviceid, ext, compress, orient, IsUSB);

            return File(bytes, "image/jpeg");
        }
    }
}