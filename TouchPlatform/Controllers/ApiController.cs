using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchModel;
using TouchSpriteService;
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

            int groupid = WebHelper.GetFormInt("groupid");

            //DataReflector<devices> service = new DataReflector<devices>();
            //list = service.Get();

            string where = "";
            if (groupid != 0)
            {
                where = " groups.ID=" + groupid;
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

            result = new { code = 200, message = "查询成功", data = list };
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

            var groupid = WebHelper.GetFormInt("groupid");
            string deviceStr = WebHelper.GetFormString("deviceids");
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

            var service = new TouchSpriteService.Business.deviceService();
            var list = service.GetDevice2GroupDetail(" groups.ID=" + groupid);

            int success = 0, fail = 0;
            var ActionService = new TouchSpriteService.authActionService();
            foreach (var d in list)
            {
                var luaReturn = ActionService.Runlua(d.deviceid);
                if (luaReturn == "ok") success++;
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
            foreach (var deviceid in deviceids)
            {
                if (!string.IsNullOrWhiteSpace(deviceid))
                {
                    var luaReturn = ActionService.Runlua(deviceid);
                    if (luaReturn == "ok") success++;
                    else fail++;
                }
            }

            string message = string.Format("{0}台设备命令发送成功，{1}台失败", success, fail);
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

            var service = new TouchSpriteService.Business.deviceService();
            var list = service.GetDevice2GroupDetail(" groups.ID=" + groupid);

            int success = 0, fail = 0;
            var ActionService = new TouchSpriteService.authActionService();
            foreach (var d in list)
            {
                var luaReturn = ActionService.Stoplua(d.deviceid);
                if (luaReturn == "ok") success++;
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

            var deviceids = WebHelper.GetFormString("deviceids").Split(',');
            var ActionService = new TouchSpriteService.authActionService();
            int success = 0, fail = 0;
            foreach (var deviceid in deviceids)
            {
                if (!string.IsNullOrWhiteSpace(deviceid))
                {
                    var luaReturn = ActionService.Stoplua(deviceid);
                    if (luaReturn == "ok") success++;
                    else fail++;
                }
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

            var groupid = WebHelper.GetFormInt("groupid");
            string deviceStr = WebHelper.GetFormString("deviceids");
            if (groupid == 0 && deviceStr == "")
            {
                return JsonConvert.SerializeObject(result);
            }

            var deviceids = GetDevicesParam();
            if (deviceids.Length == 0)
            {
                return JsonConvert.SerializeObject(result);
            }

            list = new List<devices>();
            var ActionService = new TouchSpriteService.authActionService();
            foreach (var deviceid in deviceids)
            {
                if (!string.IsNullOrWhiteSpace(deviceid))
                {
                    var model = ActionService.getStatus(deviceid);
                    list.Add(model);
                }
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

            int success = 0, fail = 0;
            foreach (var deviceid in deviceids)
            {
                if (!string.IsNullOrWhiteSpace(deviceid))
                {
                    var ActionService = new TouchSpriteService.authActionService();
                    var luaReturn = ActionService.DoAction(deviceid, "reboot?type=1");
                    if (luaReturn == "ok") success++;
                    else fail++;
                }
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
            var luaReturn = ActionService.getFileList(deviceid, root, path);

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


        /// <summary>
        /// 循环将文件发送到设备
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
            //先循环设备
            foreach (var deviceid in deviceids)
            {
                if (!string.IsNullOrWhiteSpace(deviceid))
                {
                    var failNum = 0;
                    //再循环发送文件
                    foreach (var d in Arrayfiles)
                    {
                        string luaReturn = ActionService.uploadlua(deviceid, d);
                        if (luaReturn == "ok") { }
                        else { failNum++; }
                    }

                    if (failNum == 0)
                        success++;
                    else
                        fail++;
                }
            }

            string message = string.Format("{0}台设备文件发送成功，{1}台失败", success, fail);
            result = new { code = 200, message = message };
            return JsonConvert.SerializeObject(result);
        }

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
        /// 将ZIP文件上传到服务器
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
            var zipExtraPath = Server.MapPath(string.Format("~/source/{0}/{1}", luaDir, dirname));
            zipfile.SaveAs(zipPath);

            ZipFile.ExtractToDirectory(zipPath, zipExtraPath);

            result = new { code = 200, message = "上传成功" };
            return JsonConvert.SerializeObject(result);

        }

        public string SetLuaPath()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            //文件夹名称
            string path = WebHelper.GetFormString("path");
            var groupid = WebHelper.GetFormInt("groupid");
            var deviceids = GetDevicesParam();

            DataReflector<groups> service = new DataReflector<groups>();
            var group = service.Get(groupid);

            if (deviceids.Length == 0 || path == "" || group == null)
            {
                return JsonConvert.SerializeObject(result);
            }

            var folderFullName = Server.MapPath("~/source/lua/" + path);
            var fileFullName = Server.MapPath(string.Format("~/source/lua/{0}/main.lua", path));
            FileInfo TheFile = new FileInfo(fileFullName);
            if (!TheFile.Exists)
            {
                result = new { code = 101, message = "入口文件main不存在" };
                return JsonConvert.SerializeObject(result);
            }
            bool isSuccess = true;
            var ActionService = new TouchSpriteService.authActionService();
            foreach (var deviceid in deviceids)
            {
                if (!string.IsNullOrWhiteSpace(deviceid))
                {
                    //循环所有文件
                    string re = loopsendSource(folderFullName, deviceid) + ",";
                    if (re.IndexOf("fail") > -1)
                        isSuccess = false;
                }
            }

            //文件发送是否成功
            if (isSuccess)
            {
                //保存到组信息
                group.issend = true;
                group.luapath = string.Format("/var/mobile/Media/TouchSprite/lua/{0}/main.lua",path);
                service.Update(group);

                foreach (var deviceid in deviceids)
                {
                    if (!string.IsNullOrWhiteSpace(deviceid))
                    {
                        ActionService.setLuaPath(deviceid, group.luapath);
                    }
                }

                result = new { code = 200, message = "保存成功" };
            }
            else
                result = new { code = 100, message = "保存失败" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 循环某一个文件夹，将一个文件发送到某一个设备
        /// </summary>
        /// <param name="folderFullName">文件夹的实际路径</param>
        /// <param name="deviceid"></param>
        private string loopsendSource(string folderFullName, string deviceid)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);
            if (!TheFolder.Exists)
            {
                return "fail";
            }
            var result = "";
            var rootPath = Server.MapPath("~/source/");
            var ActionService = new TouchSpriteService.authActionService();
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                string filePath = NextFile.FullName.Replace(rootPath, "").Replace('\\', '/');
                result += ActionService.uploadlua(deviceid, filePath) + ",";
            }
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                //递归循环
                result += loopsendSource(NextFolder.FullName, deviceid) + ",";
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
            string ext = WebHelper.GetQueryString("ext","jpg");

            var ActionService = new TouchSpriteService.authActionService();
            byte[] bytes = ActionService.snapshot(deviceid, ext, compress, orient);

            return File(bytes, "image/jpeg");
        }
    }
}