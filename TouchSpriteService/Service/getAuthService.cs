using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TouchSpriteService
{
    public class getAuthService
    {
        Common.SimpleCacheProvider cache = Common.SimpleCacheProvider.GetInstance();

        /// <summary>
        /// 将一部分设备设置分组，自己的业务逻辑判断分组情况。
        /// </summary>
        /// <returns></returns>
        //public string GetAuth(string groupname)
        //{
        //    //string param = "action=getAuth&key=DcyiHZdt2F2Ig8rxF2zX6nAEhYmtuKNq0lWUjN2LQ33lPqYZYJmg5zKc2OUeE34c";
        //    //string param = "{ \"action\": \"getAuth\", \"devices\": [ \"7afdd664927bf5df99ed791810cf4e0b\"], \"key\": \"mrIG71LVkF2MMRR89JztY5rshs5gQbOBZEHBpyiJnSKBBhn4xlcls5Eav1Q4pXCO\",\"time\": 1465814096,\"valid\": 3000}";

        //    //string groupname = "测试组";
        //    string[] devices = { "bbbc4b959487779966775508c9ac20c8" };

        //    string auth = "";

        //    //读取缓存
        //    if (cache.GetCache(groupname) == null)
        //    {
        //        auth = freshenAuth(groupname, devices);
        //    }
        //    else
        //        auth = (string)cache.GetCache(groupname);

        //    return auth;

        //}


        //public string GetAuth(int groupid)
        public string GetAuth(TouchModel.device2GroupDetail authDevice)
        {
            //string[] devices = { "bbbc4b959487779966775508c9ac20c8" };

            string auth = "";

            var timeSpan = DateTime.Now.Subtract(authDevice.lastTime);
            var valid = timeSpan.TotalSeconds;
            if (valid > 3600)
            {
                var deviceService = new Business.deviceService();
                var list = deviceService.GetDevice2GroupDetail(" groups.ID=" + authDevice.groupid);
                string deviceids = "";
                foreach (var d in list)
                {
                    deviceids += d.deviceid + ",";
                }
                deviceids = deviceids.TrimEnd(',');

                auth = freshenAuth(deviceids.Split(','));

                //更新auth
                if (!string.IsNullOrWhiteSpace(auth))
                {
                    DataReflector<TouchModel.groups> Service = new DataReflector<TouchModel.groups>();
                    var group = Service.Get(authDevice.groupid);
                    group.auth = auth;
                    group.lastTime = group.updatedate = DateTime.Now;
                    Service.Update(group);

                    foreach (var d in list)
                    {
                        d.auth = group.auth;
                        d.lastTime = group.lastTime;
                        //更新缓存
                        cache.SetCache(d.deviceid, d, 3600);
                    }
                }
                else
                    auth = authDevice.auth;
            }
            else
            {
                auth = authDevice.auth;
            }

            //读取缓存
            //if (cache.GetCache(groupname) == null)
            //{
            //    auth = freshenAuth(devices);
            //}
            //else
            //    auth = (string)cache.GetCache(groupname);

            return auth;

        }


        /// <summary>
        /// 通过设备号获取auth
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        public string GetAuth(string deviceid)
        {
            var authDevice = cache.GetCache(deviceid) as TouchModel.device2GroupDetail;
            if (authDevice == null)
            {
                var service = new Business.deviceService();
                authDevice = service.GetDeviceDetail(deviceid);

                //添加缓存
                cache.SetCache(deviceid, authDevice, 3600);
            }

            if (authDevice != null && authDevice.groupid != 0)
            {
                //string auth = GetAuth(authDevice.groupid);
                string auth = GetAuth(authDevice);
                return auth;
            }
            else
                return "";
        }

        /// <summary>
        /// 刷新 auth
        /// </summary>
        /// <param name="groupname"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        public string freshenAuth(string[] devices)
        {
            string auth = "";
            Object paramJson = new
            {
                action = "getAuth",
                devices = devices,
                key = TsRemoteConfig.AccessKey,
                time = Common.ConvertHelper.DateTimeToStamp(DateTime.Now),
                valid = 3600
            };
            var param = Newtonsoft.Json.JsonConvert.SerializeObject(paramJson);
            //Common.Log.Info("postJson", param);

            string resultJson = Common.NetHelper.GetRequestData("https://storeauth.touchsprite.net/api/openapi", "POST", param);
            if (resultJson.IndexOf("error") > -1)
            {
                return "";
            }
            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.getAuth>(resultJson);
            if (json.auth != null)
            {
                auth = json.auth;
                //cache.SetCache(groupname, auth,json.valid);
            }

            string decodeJson = new Common.EncodeHelper().UnicodeDecoding(resultJson);
            //Common.Log.Info("freshenAuth", decodeJson);

            return auth;
        }




        //private string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        //{
        //    string ret = string.Empty;
        //    //try
        //    {
        //        byte[] byteArray = dataEncode.GetBytes(paramData); //转化
        //        HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
        //        webReq.Method = "POST";
        //        webReq.ContentType = "application/json;charset=UTF-8";

        //        webReq.ContentLength = byteArray.Length;
        //        System.IO.Stream newStream = webReq.GetRequestStream();
        //        newStream.Write(byteArray, 0, byteArray.Length);//写入参数
        //        newStream.Close();
        //        HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
        //        System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream(), Encoding.Default);
        //        ret = sr.ReadToEnd();
        //        sr.Close();
        //        response.Close();
        //        newStream.Close();
        //    }
        //    //catch (Exception ex)
        //    //{
        //    //    Console.WriteLine(ex.InnerException);
        //    //}
        //    return ret;
        //}

    }
}
