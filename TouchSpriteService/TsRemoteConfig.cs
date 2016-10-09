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

        public static Dictionary<string, string> luaPathDic = new Dictionary<string, string>{
            { "发送朋友圈","MainTimeline.lua"},
            { "搜索加人","MainAddFriends.lua"},
            { "通讯录加粉","MainAddContacts.lua"},
            { "朋友圈互相点赞","MainWxlike.lua"},
            { "统计粉丝数","MainGetFriendsCount.lua"},
            { "动态脚本","MainCommand.lua"}
        };
    }
}
