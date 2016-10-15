using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchSpriteService.Common
{
    public class CacheHelper
    {
        private static SimpleCacheProvider cache = SimpleCacheProvider.GetInstance();

        public static T GetCache<T>(string type, string Key)
        {
            T t = (T)cache.GetCache(type + Key);

            return t;
        }
        public static T GetCache<T>(string Key)
        {
            T t = GetCache<T>("", Key);

            return t;
        }


        public static void SetCache(string type, string Key, object o)
        {
            cache.SetCache(type + Key, o);
        }
        public static void SetCache(string Key, object o)
        {
            SetCache("", Key, o);
        }
    }
}
