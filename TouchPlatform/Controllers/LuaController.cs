using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public class LuaController : Controller
    {
        SimpleCacheProvider cache = SimpleCacheProvider.GetInstance();

        private static object olock = new object();

        public string Index()
        {
            return "";
        }

        /// <summary>
        /// 微信搜索手机添加好友
        /// 保存配置项目
        /// </summary>
        /// <returns></returns>
        public string WxSerach()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            luaconfigService service = new luaconfigService();
            Dictionary<string, string> listValue = new Dictionary<string, string>();
            string luaType = WebHelper.SqlFilter(WebHelper.GetFormString("luaType", "搜索加人"));

            string netWayRadio = WebHelper.SqlFilter(WebHelper.GetFormString("netWay", "WIFI"));
            listValue.Add("netWay", netWayRadio);

            listValue.Add("Name", luaType);

            int Count = WebHelper.GetFormInt("Count", 5);
            listValue.Add("Count", Count.ToString());

            int Interval = WebHelper.GetFormInt("Interval", 20);
            listValue.Add("Interval", Interval.ToString());

            int RestTime = WebHelper.GetFormInt("RestTime", 60);
            listValue.Add("RestTime", RestTime.ToString());

            string WelcomeMsg = WebHelper.SqlFilter(WebHelper.GetFormString("WelcomeMsg"));
            listValue.Add("WelcomeMsg", WelcomeMsg);

            foreach (var d in listValue)
            {
                string luaName = d.Key;
                string value = d.Value;
                service.AddOrUpdateConfig(luaType, luaName, value);
            }

            string PhoneName = "PhoneNums";
            string[] PhoneNums = WebHelper.GetFormString(PhoneName).Split('\r');

            foreach (string d in PhoneNums)
            {
                string Phone = WebHelper.SqlFilter(d.Trim().Replace(" ", ""));
                if (Phone == "") continue;

                luaType = "号码库";
                string where = " and luaType='{0}' and luaName='{1}' and luaValue='{2}' ";
                luaconfig config = service.GetConfig(string.Format(where, luaType, PhoneName, Phone));
                if (config == null)
                {
                    config = new luaconfig();
                    config.deviceId = "";
                    config.luaName = PhoneName;
                    config.luaType = luaType;
                    config.sortcode = 1;
                    config.status = "";
                    config.createdate = config.updatedate = DateTime.Now;
                    config.luaValue = Phone;
                    service.AddConfig(config);
                }
            }


            result = new { code = 200, message = "保存成功" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 微信通讯录添加好友
        /// 保存配置项目
        /// </summary>
        /// <returns></returns>
        public string WxContacts()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            luaconfigService service = new luaconfigService();
            Dictionary<string, string> listValue = new Dictionary<string, string>();
            string luaType = WebHelper.SqlFilter(WebHelper.GetFormString("luaType", "通讯录配置项"));

            string netWayRadio = WebHelper.SqlFilter(WebHelper.GetFormString("netWay", "WIFI"));
            listValue.Add("netWay", netWayRadio);

            string ClearContact = WebHelper.SqlFilter(WebHelper.GetFormString("ClearContact", "是"));
            listValue.Add("ClearContact", ClearContact);

            listValue.Add("Name", luaType);

            int Count = WebHelper.GetFormInt("Count", 5);
            listValue.Add("Count", Count.ToString());

            int Interval = WebHelper.GetFormInt("Interval", 20);
            listValue.Add("Interval", Interval.ToString());

            int RestTime = WebHelper.GetFormInt("RestTime", 60);
            listValue.Add("RestTime", RestTime.ToString());

            string WelcomeMsg = WebHelper.SqlFilter(WebHelper.GetFormString("WelcomeMsg"));
            listValue.Add("WelcomeMsg", WelcomeMsg);

            foreach (var d in listValue)
            {
                string luaName = d.Key;
                string value = d.Value;
                service.AddOrUpdateConfig(luaType, luaName, value);
            }

            string luaNameContact = "Contacts";
            string[] PhoneItems = WebHelper.GetFormString(luaNameContact).Split('\r');

            foreach (string d in PhoneItems)
            {
                string Phone = WebHelper.SqlFilter(d.Trim());
                if (Phone == "") continue;

                luaType = "通讯录";
                string where = " and luaType='{0}' and luaName='{1}' and luaValue='{2}' ";
                luaconfig config = service.GetConfig(string.Format(where, luaType, luaNameContact, Phone));
                if (config == null)
                {
                    config = new luaconfig();
                    config.deviceId = "";
                    config.luaName = luaNameContact;
                    config.luaType = luaType;
                    config.sortcode = 1;
                    config.status = "";
                    config.createdate = config.updatedate = DateTime.Now;
                    config.luaValue = Phone;
                    service.AddConfig(config);
                }
            }


            result = new { code = 200, message = "保存成功" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 获取配置项目的列表
        /// 或者手机设备获取配置项
        /// </summary>
        /// <returns></returns>
        public string GetList()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            List<luaconfigdevice> list = new List<luaconfigdevice>();
            var result = new { code = 100, message = "参数错误", data = list, PageInfo = new { pageIndex = 1, pageSize = 100, Total = 0 } };

            string luaType = WebHelper.SqlFilter(WebHelper.GetRequestString("luaType"));
            string[] luaTypes = WebHelper.GetRequestString("luaTypes").Split(',');
            string where = "";
            foreach (var str in luaTypes)
            {
                where += "'" + WebHelper.SqlFilter(str) + "',";
            }
            where = where.TrimEnd(',');

            //需要分页
            var pageSize = WebHelper.GetRequestInt("pageSize", 100);
            var pageIndex = WebHelper.GetRequestInt("pageIndex", 1);
            int Total = 0;

            luaconfigService service = new luaconfigService();
            string sql = string.Format(" and (luaType='{0}' or luaType in({1}))", luaType, where);
            string Status = WebHelper.SqlFilter(WebHelper.GetRequestString("Status"));
            if (Status != "")
                sql += string.Format(" and status='{0}'", Status);

            string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));
            if (deviceId != "")
                sql += string.Format(" and (deviceId='' or deviceId='{0}')", deviceId);

            list = service.GetDetailPageList(sql, out Total, pageSize, pageIndex);

            result = new { code = 200, message = "获取成功", data = list, PageInfo = new { pageIndex = pageIndex, pageSize = pageSize, Total = Total } };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 获取脚本类型的配置项
        /// </summary>
        /// <returns></returns>
        public string GetConfig()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            Dictionary<string, object> dic = null;
            var result = new { code = 100, message = "参数错误", data = dic };

            string luaType = WebHelper.SqlFilter(WebHelper.GetRequestString("luaType"));
            string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));
            if (luaType == "")
                return JsonConvert.SerializeObject(result);

            string query = string.Format(" and luaType='{0}' and status='' and (deviceId='' or deviceId='{1}')", luaType, deviceId);
            luaconfigService service = new luaconfigService();

            List<luaconfig> list = service.GetList(query);
            dic = new Dictionary<string, object>();
            foreach (var d in list)
            {
                if (!dic.ContainsKey(d.luaName))
                    dic.Add(d.luaName, d.luaValue);
                else
                    dic[d.luaName] += "@" + d.luaValue;


                if (deviceId != "")
                {
                    d.deviceId = deviceId;
                    service.UpdateConfig(d);
                }
            }

            result = new { code = 200, message = "获取成功", data = dic };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 搜索加人脚本，获取添加的微信号
        /// 并添加设备标记
        /// </summary>
        /// <returns></returns>
        public string GetPhoneNums()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            Dictionary<string, object> dic = null;
            var result = new { code = 100, message = "参数错误", data = dic };

            string luaType = WebHelper.SqlFilter(WebHelper.GetRequestString("luaType"));
            string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));
            if (luaType == "")
                return JsonConvert.SerializeObject(result);

            string query = string.Format(" and luaType='{0}' and status='' and (deviceId='' or deviceId='{1}')", luaType, deviceId);
            luaconfigService service = new luaconfigService();

            //添加筛选参数
            int Count = 0;
            Count = WebHelper.GetRequestInt("Count", Count);
            if (Count == 0)
            {
                string size = service.GetValue("搜索加人", "Count");
                int.TryParse(size, out Count);
            }

            lock (olock)
            {
                List<luaconfig> list = service.GetList(query, Count);

                var taskNumbers = new Task<bool>[list.Count];

                dic = new Dictionary<string, object>();
                for (int i = 0; i < list.Count; i++)
                {
                    var d = list[i];
                    if (!dic.ContainsKey(d.luaName))
                        dic.Add(d.luaName, d.luaValue);
                    else
                        dic[d.luaName] += "@" + d.luaValue;

                    taskNumbers[i] = Task.Run(() =>
                    {
                        d.deviceId = deviceId;
                        return service.UpdateConfig(d);
                    });
                }
                Task.WaitAll(taskNumbers);
            }


            result = new { code = 200, message = "获取成功", data = dic };
            return JsonConvert.SerializeObject(result);
        }

        public string GetListByDevice()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            List<luaconfig> list = new List<luaconfig>();
            //需要分页
            var pageSize = WebHelper.GetRequestInt("pageSize", 100);
            var pageIndex = WebHelper.GetRequestInt("pageIndex", 1);
            int Total = 0;

            var result = new { code = 100, message = "参数错误", data = list, PageInfo = new { pageIndex = pageIndex, pageSize = pageSize, Total = 0 } };

            string luaType = WebHelper.SqlFilter(WebHelper.GetRequestString("luaType"));
            string[] luaTypes = WebHelper.GetRequestString("luaTypes").Split(',');
            string where = "";
            foreach (var str in luaTypes)
            {
                where += "'" + WebHelper.SqlFilter(str) + "',";
            }
            where = where.TrimEnd(',');

            string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));
            if (deviceId == "")
            {
                return JsonConvert.SerializeObject(result);
            }

            luaconfigService service = new luaconfigService();
            string sql = string.Format(" and (luaType='{0}' or luaType in({1}))", luaType, where);
            string Status = WebHelper.SqlFilter(WebHelper.GetRequestString("Status"));
            if (Status != "")
                sql += string.Format(" and status='{0}'", Status);

            sql += string.Format(" and (deviceId='' or deviceId='{0}')", deviceId);

            lock (olock)
            {
                list = service.GetPageList(sql, out Total, pageSize, pageIndex);

                var taskNumbers = new Task<bool>[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    var d = list[i];
                    taskNumbers[i] = Task.Run(() =>
                    {
                        d.deviceId = deviceId;
                        return service.UpdateConfig(d);
                    });
                }
                Task.WaitAll(taskNumbers);
            }

            result = new { code = 200, message = "获取成功", data = list, PageInfo = new { pageIndex = pageIndex, pageSize = pageSize, Total = Total } };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 添加设备标记
        /// 记录 添加过的微信号
        /// 手机设备，搜索加人脚本访问使用
        /// </summary>
        /// <returns></returns>
        public string SetStatus()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            string luaType = WebHelper.SqlFilter(WebHelper.GetRequestString("luaType"));
            string luaName = WebHelper.SqlFilter(WebHelper.GetRequestString("luaName"));
            string luaValue = WebHelper.SqlFilter(WebHelper.GetRequestString("luaValue"));

            string status = WebHelper.SqlFilter(WebHelper.GetRequestString("status"));
            string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));

            if (luaType == "" || luaName == "" || luaValue == "")
                return JsonConvert.SerializeObject(result);

            string where = " and luaType='{0}' and luaName='{1}' and luaValue='{2}' ";
            luaconfigService service = new luaconfigService();
            luaconfig config = service.GetConfig(string.Format(where, luaType, luaName, luaValue));
            if (config != null)
            {
                config.deviceId = deviceId;
                config.status = status;
                config.updatedate = DateTime.Now;
                service.UpdateConfig(config);
            }


            result = new { code = 200, message = "设置成功" };
            return JsonConvert.SerializeObject(result);
        }
        public string SetStatusByID()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            int ID = WebHelper.GetRequestInt("ID");

            string status = WebHelper.SqlFilter(WebHelper.GetRequestString("status"));
            string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));

            luaconfigService service = new luaconfigService();
            luaconfig config = service.GetConfig(ID);
            if (config != null)
            {
                config.deviceId = deviceId;
                config.status = status;
                config.updatedate = DateTime.Now;
                service.UpdateConfig(config);
            }

            result = new { code = 200, message = "设置成功" };
            return JsonConvert.SerializeObject(result);
        }
        /// <summary>
        /// 获取配置表的数据项详细
        /// </summary>
        /// <returns></returns>
        public string GetDetail()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            luaconfig config = null;
            var result = new { code = 100, message = "参数错误", data = config };

            string where = " ";
            int ID = WebHelper.GetRequestInt("ID");
            if (ID != 0)
            {
                where += string.Format("and ID={0} ", ID);
            }
            string luaType = WebHelper.SqlFilter(WebHelper.GetRequestString("luaType"));
            if (luaType != "")
            {
                where += string.Format("and luaType='{0}' ", luaType);
            }
            string luaName = WebHelper.SqlFilter(WebHelper.GetRequestString("luaName"));
            if (luaName != "")
            {
                where += string.Format("and luaName='{0}' ", luaName);
            }
            string luaValue = WebHelper.SqlFilter(WebHelper.GetRequestString("luaValue"));
            if (luaValue != "")
            {
                where += string.Format("and luaValue='{0}' ", luaValue);
            }

            //string status = WebHelper.SqlFilter(WebHelper.GetRequestString("status"));
            string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));
            if (deviceId != "")
            {
                where += string.Format("and deviceId='{0}' ", deviceId);
            }
            luaconfigService service = new luaconfigService();
            config = service.GetConfig(where);

            result = new { code = 200, message = "请求成功", data = config };
            return JsonConvert.SerializeObject(result);
        }


        /// <summary>
        /// 提供给手机设备，手机设备储存并执行动态脚本
        /// </summary>
        /// <returns></returns>
        public string DynamicLua()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误", data = "", status = "" };

            string luaType = "动态脚本";
            string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));
            //string luaName = WebHelper.SqlFilter(WebHelper.GetRequestString("luaName"));
            string luaName = "command";

            string where = " and luaType='{0}' and (deviceId='' OR deviceId='{1}') and luaName='{2}' ";
            luaconfigService service = new luaconfigService();
            luaconfig config = service.GetConfig(string.Format(where, luaType, deviceId, luaName));
            if (config != null)
            {
                result = new { code = 200, message = "请求成功", data = config.luaValue, status = config.status };
            }
            return JsonConvert.SerializeObject(result);
        }

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

        /// <summary>
        /// 发送已有的命令到设备（配合脚本使用）
        /// </summary>
        /// <returns></returns>
        public string DynamicSet()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            string luaType = "动态脚本";
            //string devices = WebHelper.GetRequestString("deviceids");
            //if (devices == "")
            //{
            //    result = new { code = 101, message = "参数错误" };
            //    return JsonConvert.SerializeObject(result);
            //}
            //string[] deviceids = devices.Split(',');

            string[] deviceids = GetDevicesParam();
            if (deviceids.Length == 0)
            {
                result = new { code = 101, message = "参数错误" };
                return JsonConvert.SerializeObject(result);
            }

            //string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));
            string deviceId = "";
            string luaName = WebHelper.SqlFilter(WebHelper.GetRequestString("luaName"));

            string where = " and luaType='{0}' and (deviceId='' OR deviceId='{1}') and luaName='{2}' ";
            luaconfigService service = new luaconfigService();
            luaconfig command = service.GetConfig(string.Format(where, luaType, deviceId, "command"));
            if (command != null)
            {
                string StringCmd = service.GetValue("动态脚本", luaName);
                service.SetValue(luaType, "command", StringCmd);
                result = new { code = 200, message = "发送中，请等待..." };

                var connectType = WebHelper.GetRequestString("connectType");
                bool IsUSB = connectType == "USB";

                var ActionService = new TouchSpriteService.authActionService();

                #region 设置执行路径
                ////多任务发送命令
                //var taskSendDevices = new Task<bool>[deviceids.Length];
                //for (int i = 0; i < deviceids.Length; i++)
                //{
                //    var deviceid = deviceids[i];

                //    taskSendDevices[i] = Task.Run(() =>
                //    {
                //        //此次需要执行的脚本路径
                //        string luaPath = "/var/mobile/Media/TouchSprite/lua/Command/main.lua";

                //        //与上次执行路径对比，路径一样就跳过这一步
                //        //var modelPath = cache.GetCache(deviceid) as TouchModel.device2GroupDetail;
                //        //if (modelPath != null && modelPath.luapath == luaPath)
                //        //    return true;
                //        //else
                //        return ActionService.setLuaPath(deviceid, luaPath);

                //    });
                //}
                //Task.WaitAll(taskSendDevices);
                #endregion

                #region 执行脚本
                //多任务发送命令
                var taskDevices = new Task<string>[deviceids.Length];
                for (int i = 0; i < deviceids.Length; i++)
                {
                    var deviceid = deviceids[i];
                    taskDevices[i] = Task.Run(() =>
                    {
                        //此次需要执行的脚本路径
                        //string luaPath = "/var/mobile/Media/TouchSprite/lua/Command/main.lua";
                        string luaPath = "/var/mobile/Media/TouchSprite/lua/MainCommand.lua";
                        bool isSend = ActionService.setLuaPath(deviceid, luaPath, IsUSB);

                        if (isSend)
                            return ActionService.Runlua(deviceid, IsUSB);
                        else
                            return "fail";
                    });
                }

                #endregion

                result = new { code = 200, message = "执行成功" };
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 保存单项的配置文件
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public string ConfigSave()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            int ID = WebHelper.GetRequestInt("ID");

            string luaType = WebHelper.SqlFilter(WebHelper.GetRequestString("luaType"));
            if (luaType == "")
            {
                result = new { code = 101, message = "类别错误" };
                return JsonConvert.SerializeObject(result);
            }
            string luaName = WebHelper.SqlFilter(WebHelper.GetRequestString("luaName"), true);
            if (luaName == "")
            {
                result = new { code = 102, message = "名称错误" };
                return JsonConvert.SerializeObject(result);
            }
            int sortcode = WebHelper.GetRequestInt("sortcode");
            string luaValue = WebHelper.GetRequestString("luaValue");


            luaconfigService service = new luaconfigService();
            luaconfig config = service.GetConfig(ID);
            if (config == null) config = new luaconfig();
            config.luaType = luaType;
            config.luaName = luaName;
            config.luaValue = luaValue;
            config.sortcode = sortcode;

            bool success = service.AddOrUpdateConfig(config);

            result = new { code = 200, message = success ? "保存成功" : "保存失败" };
            return JsonConvert.SerializeObject(result);
        }



        /// <summary>
        /// 微信发送朋友圈 配置
        /// </summary>
        /// <returns></returns>
        public string WxTimeLine()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            luaconfigService service = new luaconfigService();
            Dictionary<string, string> listValue = new Dictionary<string, string>();
            string luaType = WebHelper.SqlFilter(WebHelper.GetFormString("luaType", "发送朋友圈"));

            string netWayRadio = WebHelper.SqlFilter(WebHelper.GetFormString("netWay", "WIFI"));
            listValue.Add("netWay", netWayRadio);

            listValue.Add("Name", luaType);

            int RestTime = WebHelper.GetFormInt("RestTime", 60);
            listValue.Add("RestTime", RestTime.ToString());

            string SendMsg = WebHelper.SqlFilter(WebHelper.GetFormString("SendMsg"));
            listValue.Add("SendMsg", SendMsg);

            foreach (var d in listValue)
            {
                string luaName = d.Key;
                string value = d.Value;
                service.AddOrUpdateConfig(luaType, luaName, value);
            }

            //文件上传
            int index = 1; string imageArray = "";
            HttpFileCollectionBase FileCollect = Request.Files;
            for (int i = 0; i < FileCollect.AllKeys.Length; i++)
            {
                HttpPostedFileBase file = FileCollect[i];
                if (file == null)
                    continue;
                if (file.ContentLength == 0 || file.ContentLength > 2 * 1024 * 1024)
                    continue;
                string[] FileInfos = file.FileName.Split('.');
                if (FileInfos.Length < 2)
                    continue;
                //扩展名
                string[] fileExtends = { ".png", ".jpg" };
                string fileExtension = FileInfos[FileInfos.Length - 1];
                if (fileExtends.Contains(fileExtension))
                {
                    continue;
                }

                string fileName = index++ + "." + fileExtension;
                //var dirPath = Server.MapPath(string.Format("~/source/lua/{0}/image/", luaType));
                var dirPath = Server.MapPath("~/source/lua/images/");
                var filePath = dirPath + fileName;
                System.IO.DirectoryInfo direInfo = new System.IO.DirectoryInfo(dirPath);
                if (!direInfo.Exists)
                {
                    direInfo.Create();
                }
                file.SaveAs(filePath);
                imageArray += fileName + ",";
            }
            service.AddOrUpdateConfig(luaType, "imageArray", imageArray.TrimEnd(','));


            result = new { code = 200, message = "保存成功" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 读取文本内容
        /// </summary>
        /// <returns></returns>
        public ActionResult fileInfo()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误", data = "" };

            string luaPath = "~/source/" + WebHelper.GetRequestString("path");
            string filePath = Server.MapPath(luaPath);

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (fileInfo.Extension == ".lua" || fileInfo.Extension == ".txt")
            {
                System.IO.StreamReader sw = new System.IO.StreamReader(filePath, System.Text.Encoding.GetEncoding("utf-8"));
                result = new { code = 200, message = "请求成功", data = sw.ReadToEnd() };
                sw.Close();
                sw.Dispose();
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 网页保存文件内容
        /// </summary>
        /// <returns></returns>
        public ActionResult SavefileInfo()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误" };

            string luaPath = "~/source/" + WebHelper.GetFormString("luapath");
            string content = WebHelper.GetFormString("content");
            string filePath = Server.MapPath(luaPath);

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            if (content == "" || !fileInfo.Exists)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (fileInfo.Extension == ".lua" || fileInfo.Extension == ".txt")
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.GetEncoding("utf-8"));
                sw.Write(content);
                result = new { code = 200, message = "保存成功" };
                sw.Close();
                sw.Dispose();
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }



        /// <summary>
        /// 提供给脚本使用（lua脚本没有随机种子）
        /// </summary>
        /// <returns></returns>
        public string GetRandom()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            Random rand = new Random();
            string type = WebHelper.GetQueryString("t", "int");
            string max = WebHelper.GetQueryString("max", "1000");
            string randStr = "";
            switch (type)
            {
                case "int":
                    int maxNum = 0;
                    int.TryParse(max, out maxNum);
                    randStr = rand.Next(0, maxNum).ToString();
                    break;
                default:
                    randStr = rand.NextDouble().ToString();
                    break;
            }

            return randStr;
        }

        /// <summary>
        /// 添加或者更新配置表里的值
        /// </summary>
        /// <returns></returns>
        public string SetValue()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            luaconfig config = null;
            var result = new { code = 100, message = "参数错误", data = config };

            string where = " ";
            int ID = WebHelper.GetRequestInt("ID");
            if (ID != 0)
                where += string.Format("and ID={0} ", ID);

            string luaType = WebHelper.SqlFilter(WebHelper.GetRequestString("luaType"));
            where += string.Format("and luaType='{0}' ", luaType);

            string luaName = WebHelper.SqlFilter(WebHelper.GetRequestString("luaName"));
            where += string.Format("and luaName='{0}' ", luaName);

            string luaValue = WebHelper.SqlFilter(WebHelper.GetRequestString("luaValue"));

            string status = WebHelper.SqlFilter(WebHelper.GetRequestString("status"));
            if (status != "")
                where += string.Format("and status='{0}' ", luaName);

            string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));
            if (deviceId != "")
                where += string.Format("and deviceId='{0}' ", deviceId);

            luaconfigService service = new luaconfigService();
            config = service.GetConfig(where);
            if (config == null)
            {
                config = new luaconfig();
                config.deviceId = deviceId;
                config.luaName = luaName;
                config.luaType = luaType;
                config.luaValue = luaValue;
                config.sortcode = 1;
                config.status = status;
                config.createdate = config.updatedate = DateTime.Now;
                service.AddConfig(config);
            }
            else
            {
                config.luaValue = luaValue;
                config.status = status;
                config.updatedate = DateTime.Now;
                service.UpdateConfig(config);
            }

            result = new { code = 200, message = "请求成功", data = config };
            return JsonConvert.SerializeObject(result);
        }

        public string GetIPAddress()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            string deviceid = WebHelper.GetQueryString("deviceid");

            var service = new TouchSpriteService.Business.deviceService();
            var model = service.GetCacheDevice(deviceid);

            var result = new { code = 100, USBIP = "", data = "" };
            if (model == null)
                return JsonConvert.SerializeObject(result);

            string No = model.ip.Split('.').Last();
            result = new { code = 200, USBIP = "192.168." + No + ".1", data = No };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 清除缓存+更新AccessKey
        /// </summary>
        /// <returns></returns>
        public string InitCaches()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            SimpleCacheProvider.GetInstance().ClearAll();

            int groupid = WebHelper.GetRequestInt("groupid");
            if (groupid != 0)
            {
                TouchSpriteService.DataReflector<groups> service = new TouchSpriteService.DataReflector<groups>();
                groups model = service.Get(groupid);
                if (model != null)
                {
                    model.auth = "";
                    model.lastTime = DateTime.Now.AddHours(-1);
                    model.updatedate = DateTime.Now;
                    service.Update(model);
                }
            }

            var result = new { code = 200, message = "更新成功" };
            return JsonConvert.SerializeObject(result);
        }
    }
}