using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TouchSpriteService.Common
{
    /// <summary>
    /// SimpleProvider
    /// </summary>
    public class SimpleCacheProvider : ICacheProvider
    {
        private static bool useCache = ConfigurationManager.ConnectionStrings["useCache"].ConnectionString.ToString().ToLower() == "true";
        private static SimpleCacheProvider _instance;

        public static SimpleCacheProvider GetInstance()
        {
            if (_instance == null) _instance = new SimpleCacheProvider();
            return _instance;
        }
        private Dictionary<string, CacheItem> _caches;
        private SimpleCacheProvider()
        {
            this._caches = new Dictionary<string, CacheItem>();
        }
        public object GetCache(string key)
        {
            return this._caches.ContainsKey(key) ? this._caches[key].Expired() ? null : this._caches[key].Value : null;
        }

        public void SetCache(string key, object value, int expire = 300)
        {
            if (this._caches.ContainsKey(key))
                this._caches[key] = new CacheItem(key, value, expire);
            else
                this._caches.Add(key, new CacheItem(key, value, expire));
        }

        class CacheItem
        {

            private long _insertTime;


            private int _expire;

            private object _value;

            public object Value
            {
                get { return _value; }
                set { _value = value; }
            }
            private string _key;

            public string Key
            {
                get { return _key; }
                set { _key = value; }
            }

            public CacheItem(string key, object value, int expire)
            {
                this._key = key;
                this._value = value;
                this._expire = expire;
                this._insertTime = ConvertHelper.Now;
            }

            public bool Expired()
            {
                return ConvertHelper.Now > this._insertTime + _expire;
            }


        }


    }
}
