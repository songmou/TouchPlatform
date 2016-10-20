using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchSpriteService
{
    public class TsRemoteConfig
    {
        //public static string AccessKey = "8gE3J3i4L1mleWfvkIfZjt1tXgkiYy0h02O8uASn3miACwwC5HnzuH6tjI36HQAN";
        public static string AccessKey = ConfigurationManager.ConnectionStrings["AccessKey"].ConnectionString.ToString();

        //public static Dictionary<string, string> luaPathDic = new Dictionary<string, string>{
        //    { "发送朋友圈","MainTimeline.lua"},
        //    { "搜索加人","MainAddFriends.lua"},
        //    { "通讯录加粉","MainAddContacts.lua"},
        //    { "朋友圈互相点赞","MainWxlike.lua"},
        //    { "统计粉丝数","MainGetFriendsCount.lua"},
        //    { "清空相册并重启","MainClearDCIM.lua"},
        //    //{ "微信活跃度","MainLiveness.lua"},
        //    { "关闭屏幕","MainLockScreen.lua"},
        //    { "解锁屏幕","MainUnLockScreen.lua"},
        //    { "动态脚本","MainCommand.lua"},
        //    { "入口主脚本","MainLooper.lua"}
        //};

        public static List<luaEntity> luaPathDic
        {
            get
            {
                var content = Common.IOHelper.Read("luaDictionary.json");
                List<luaEntity> list = System.Web.HttpRuntime.Cache["lualist"] as List<luaEntity>;
                if (list == null)
                {
                    list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<luaEntity>>(content);
                    System.Web.HttpRuntime.Cache["lualist"] = list;
                }
                return list;
            }
        }
    }

    public class luaEntity
    {
        public string luaName { get; set; }
        public string luaFileName { get; set; }
        public string Description { get; set; }
    }
}
