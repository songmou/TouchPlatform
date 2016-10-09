using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TouchSpriteService.Business;

namespace TouchSpriteService
{
    /// <summary>
    /// https://www.zybuluo.com/miniknife/note/129616
    /// </summary>
    public class authActionService
    {
        Common.SimpleCacheProvider cache = Common.SimpleCacheProvider.GetInstance();
        public string GetDeviceUrl(TouchModel.devices model, bool IsUSB = false)
        {
            string ip = model.ip;
            if (IsUSB && !string.IsNullOrWhiteSpace(model.usbip))
            {
                //string No = ip.Split('.').Last();
                //ip = "172.20." + No + ".1";
                ip = model.usbip;
            }

            return string.Format("http://{0}:{1}", ip, model.port);
        }

        /// <summary>
        /// 设置程序运行的路径（设置完成并不会执行，需要远程运行代码/runLua）
        /// </summary>
        /// <returns></returns>
        public bool setLuaPath(string deviceid, string path, bool IsUSB = false)
        {
            getAuthService AuthService = new getAuthService();
            string auth = AuthService.GetAuth(deviceid);

            string luaData = "{\"path\":\"" + path + "\"}";

            var service = new Business.deviceService();
            //var model = service.GetDevice(Common.WebHelper.SqlFilter(deviceid));
            var model = service.GetCacheDevice(deviceid);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("auth", auth);
            string Url = GetDeviceUrl(model, IsUSB) + "/setLuaPath";
            string result = Common.NetHelper.GetRequestData(Url, "POST", luaData, Encoding.UTF8, 3000, headers);

            var Success = result == "ok";

            #region 添加缓存（判断是否可以跳过发送命令这一句）
            //if (Success)
            //{
            //    if (model != null)
            //    {
            //        model.luapath = path;
            //        model.updatedate = DateTime.Now;
            //        service.UpdateDevice(model);
            //        cache.SetCache(deviceid, null);
            //    }
            //}
            #endregion

            return Success;
        }

        public string Runlua(string deviceid, bool IsUSB = false)
        {
            getAuthService AuthService = new getAuthService();
            string auth = AuthService.GetAuth(deviceid);

            var service = new Business.deviceService();
            //var model = service.GetDevice(Common.WebHelper.SqlFilter(deviceid));
            var model = service.GetCacheDevice(deviceid);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("auth", auth);
            string Url = GetDeviceUrl(model, IsUSB) + "/runLua";
            string result = Common.NetHelper.GetRequestData(Url, "GET", "", Encoding.UTF8, 3000, headers);

            return result;
        }


        public string Stoplua(string deviceid, bool IsUSB = false)
        {
            getAuthService AuthService = new getAuthService();
            string auth = AuthService.GetAuth(deviceid);

            var service = new Business.deviceService();
            //var model = service.GetDevice(Common.WebHelper.SqlFilter(deviceid));
            var model = service.GetCacheDevice(deviceid);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("auth", auth);
            string Url = GetDeviceUrl(model, IsUSB) + "/stopLua";
            string result = Common.NetHelper.GetRequestData(Url, "GET", "", Encoding.UTF8, 3000, headers);

            return result;
        }


        public TouchModel.devices getStatus(string deviceid, bool IsUSB = false)
        {
            getAuthService AuthService = new getAuthService();
            string auth = AuthService.GetAuth(deviceid);
            if (auth == "")
            {
                return null;
            }

            var service = new Business.deviceService();
            //var model = service.GetDevice(Common.WebHelper.SqlFilter(deviceid));
            var model = service.GetCacheDevice(deviceid);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("auth", auth);
            string Url = GetDeviceUrl(model, IsUSB) + "/status";
            string result = Common.NetHelper.GetRequestData(Url, "GET", "", Encoding.UTF8, 1200, headers);

            string status = "";
            if (result == "f00")
                status = "空闲";
            else if (result == "f01")
                status = "运行";
            else if (result == "f02")
                status = "录制";
            else
                status = "离线";
            if (model.status != status)
            {
                model.status = status;
                //service.UpdateDevice(model);
            }
            else
                model.status = status;


            return model;
        }


        /// <summary>
        /// 通用请求
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public string DoAction(string deviceid, string actionName, bool IsUSB = false)
        {
            getAuthService AuthService = new getAuthService();
            string auth = AuthService.GetAuth(deviceid);

            var service = new Business.deviceService();
            //var model = service.GetDevice(Common.WebHelper.SqlFilter(deviceid));
            var model = service.GetCacheDevice(deviceid);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("auth", auth);
            string Url = GetDeviceUrl(model, IsUSB) + "/" + actionName;
            string result = Common.NetHelper.GetRequestData(Url, "GET", "", Encoding.UTF8, 3000, headers);

            return result;
        }

        /// <summary>
        /// #获取设备目录文件列表
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="Root"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public string getFileList(string deviceid, string Root, string Path,bool IsUSB=false)
        {
            getAuthService AuthService = new getAuthService();
            string auth = AuthService.GetAuth(deviceid);

            var service = new Business.deviceService();
            //var model = service.GetDevice(Common.WebHelper.SqlFilter(deviceid));
            var model = service.GetCacheDevice(deviceid);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("auth", auth);
            headers.Add("Root", Root);
            headers.Add("Path", Path);
            string Url = GetDeviceUrl(model,IsUSB) + "/getFileList";
            string result = Common.NetHelper.GetRequestData(Url, "GET", "", Encoding.UTF8, 3000, headers);

            return result;
        }

        /// <summary>
        /// 将文件发送到设备
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="FilePath">文件的相对路径（如：/lua/demo/main.lua）</param>
        /// <returns></returns>
        public string uploadlua(HttpContext Current, string deviceid, string FilePath,bool IsUSB=false)
        {
            getAuthService AuthService = new getAuthService();
            string auth = AuthService.GetAuth(deviceid);

            var service = new Business.deviceService();
            //var model = service.GetDevice(Common.WebHelper.SqlFilter(deviceid));
            var model = service.GetCacheDevice(deviceid);

            string Url = GetDeviceUrl(model,IsUSB) + "/upload";

            //System.Web.HttpContext.Current
            string PhysicalPath = Current.Server.MapPath("~/source/" + FilePath);
            FileStream fs = new FileStream(PhysicalPath, FileMode.Open, FileAccess.Read);
            byte[] fileBytes = new byte[fs.Length];
            fs.Read(fileBytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            fs.Dispose();

            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Headers.Add("Content-Type", "touchsprite/uploadfile");
            webClient.Headers.Add("auth", auth);

            string[] ArrayfilePaths = FilePath.TrimStart('/').Split('/');
            string filename = ArrayfilePaths.LastOrDefault();
            string root = ArrayfilePaths.FirstOrDefault();
            string[] rootDic = "lua|res|log|plugin".Split('|');
            if (!rootDic.Contains(root))
            {
                return "根类型不正确";
            }
            string path = "/";
            for (int i = 0; i < ArrayfilePaths.Length; i++)
            {
                if (i != 0 && i != ArrayfilePaths.Length - 1)
                {
                    path += ArrayfilePaths[i] + "/";
                }
            }
            webClient.Headers.Add("filename", filename);
            webClient.Headers.Add("root", root);
            webClient.Headers.Add("path", HttpUtility.UrlEncode(path));

            byte[] responseBytes;
            try
            {
                responseBytes = webClient.UploadData(Url, fileBytes);
                return System.Text.Encoding.UTF8.GetString(responseBytes);
            }
            catch (System.Net.WebException ex)
            {
                return ex.Message;
            }

        }



        public byte[] snapshot(string deviceid, string ext, decimal compress, int orient, bool IsUSB = false)
        {
            //传一个空白的图片
            //var screenshot = System.Web.HttpContext.Current.Server.MapPath("~/source/files/screenshot.jpg");
            //byte[] responseBytes = Common.ImageHelper.BytesFromImageFile(screenshot);

            byte[] responseBytes = new byte[0];

            getAuthService AuthService = new getAuthService();
            string auth = AuthService.GetAuth(deviceid);
            if (auth == "")
                return responseBytes;

            var service = new Business.deviceService();
            //var model = service.GetDevice(Common.WebHelper.SqlFilter(deviceid));
            var model = service.GetCacheDevice(deviceid);

            if (model == null)
                return responseBytes;

            string Url = GetDeviceUrl(model, IsUSB) + string.Format("/snapshot?ext={0}&compress={1}&orient={2}", ext, compress, orient);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("auth", auth);

            try
            {
                byte[] reslut = Common.NetHelper.GetRequestByte(Url, "GET", "", 500, headers);
                if (reslut.Length != 0)
                    responseBytes = reslut;
                return responseBytes;
            }
            catch (System.Net.WebException)
            {
                return responseBytes;
            }

        }
    }
}
