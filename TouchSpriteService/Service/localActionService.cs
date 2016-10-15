using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TouchSpriteService.Service
{
    public class localActionService
    {
        public bool Runlua(string url, string auth)
        {
            return deviceRequest(url, "runLua", auth) == "ok";
        }
        public bool Stoplua(string url, string auth)
        {
            return deviceRequest(url, "stopLua", auth) == "ok";
        }

        /// <summary>
        /// 普通的手机请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="query"></param>
        /// <param name="auth"></param>
        /// <returns></returns>
        public string deviceRequest(string url, string query, string auth,
            string method = "GET", string postDate = "", int timeout = 3000)
        {
            if (string.IsNullOrWhiteSpace(url) ||
                string.IsNullOrWhiteSpace(query) ||
                string.IsNullOrWhiteSpace(auth))
            {
                return null;
            }

            NameValueCollection headers = new NameValueCollection();
            headers.Add("auth", auth);
            string Url = url + "/" + query;
            string result = Common.NetHelper.GetRequestData(Url, method, postDate, Encoding.UTF8, timeout, headers);

            return result;
        }


        public string getStatus(string url, string auth)
        {
            string result = deviceRequest(url, "status", auth);

            string status = "";
            if (result == "f00")
                status = "空闲";
            else if (result == "f01")
                status = "运行";
            else if (result == "f02")
                status = "录制";
            else if (result == "error:操作超时")
                status = "超时";
            else
                status = "离线";

            return status;
        }


        /// <summary>
        /// 设置程序运行的路径（设置完成并不会执行，需要远程运行代码/runLua）
        /// </summary>
        /// <returns></returns>
        public bool setLuaPath(string url, string auth, string path)
        {
            string luaData = "{\"path\":\"" + path + "\"}";

            string result = deviceRequest(url, "setLuaPath", auth, "POST", luaData);

            return result == "ok";
        }


        /// <summary>
        /// #获取设备目录文件列表
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="Root"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public string getFileList(string url, string auth, string Root, string Path)
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("auth", auth);
            headers.Add("Root", Root);
            headers.Add("Path", Path);
            string Url = url + "/getFileList";

            string result = Common.NetHelper.GetRequestData(Url, "GET", "", Encoding.UTF8, 3000, headers);

            return result;
        }

        /// <summary>
        /// 将文件发送到设备
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="FilePath">文件的相对路径（如：/lua/demo/main.lua）</param>
        /// <returns></returns>
        public bool uploadlua(string url, string auth, string FilePath, HttpContext Current)
        {

            string Url = url + "/upload";

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
                return false;
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
                return System.Text.Encoding.UTF8.GetString(responseBytes)=="ok";
            }
            catch //(System.Net.WebException ex)
            {
                //return ex.Message;
                return false;
            }

        }



        public byte[] snapshot(string url, string auth, string ext, decimal compress, int orient, bool IsUSB = false)
        {
            //传一个空白的图片
            //var screenshot = System.Web.HttpContext.Current.Server.MapPath("~/source/files/screenshot.jpg");
            //byte[] responseBytes = Common.ImageHelper.BytesFromImageFile(screenshot);

            byte[] responseBytes = new byte[0];

            if (auth == "")
                return responseBytes;

            string Url = url + string.Format("/snapshot?ext={0}&compress={1}&orient={2}", ext, compress, orient);

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
