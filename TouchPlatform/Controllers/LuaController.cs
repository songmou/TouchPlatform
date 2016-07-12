using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public string Index()
        {
            return "";
        }

        /// <summary>
        /// 微信搜索手机添加好友
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
                string Phone = WebHelper.SqlFilter(d.Trim());
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
        public string GetList()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            List<luaconfig> list = new List<luaconfig>();
            var result = new { code = 100, message = "参数错误", data = list };

            string luaType = WebHelper.SqlFilter(WebHelper.GetRequestString("luaType"));
            if (luaType == "")
                return JsonConvert.SerializeObject(result);

            luaconfigService service = new luaconfigService();
            list = service.GetList(string.Format(" and luaType='{0}'", luaType));

            result = new { code = 200, message = "获取成功", data = list };
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

            List<luaconfig> list = service.GetList(query, Count);
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
        /// 添加设备标记
        /// 记录 添加过的微信号
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

        public string DynamicSet()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var result = new { code = 100, message = "参数错误"};

            string luaType = "动态脚本";
            string deviceId = WebHelper.SqlFilter(WebHelper.GetRequestString("deviceId"));
            string luaName = WebHelper.SqlFilter(WebHelper.GetRequestString("luaName"));

            string where = " and luaType='{0}' and (deviceId='' OR deviceId='{1}') and luaName='{2}' ";
            luaconfigService service = new luaconfigService();
            luaconfig config = service.GetConfig(string.Format(where, luaType, deviceId, luaName));
            luaconfig command = service.GetConfig(string.Format(where, luaType, deviceId, "command"));
            if (config != null && command != null)
            {
                command.luaValue = config.luaValue;
                command.updatedate = DateTime.Now;
                service.UpdateConfig(command);

                result = new { code = 200, message = "命令执行中，请等待..." };

                //SendCommand  TODO
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}