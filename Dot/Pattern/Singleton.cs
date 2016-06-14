using System;
using System.Collections.Concurrent;
using Dot.Extension;

namespace Dot.Pattern
{
    public class Singleton
    {
        private static readonly ConcurrentDictionary<Type, object> CACHE = new ConcurrentDictionary<Type, object>();

        public static ConcurrentDictionary<Type, object> Cache
        {
            get { return CACHE; }
        }
    }

    public class Singleton<T> : Singleton where T : class
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                if (_instance == null && Cache.TryAdd(typeof(T), value))
                    _instance = value;
            }
        }

        public static bool Replace(T other)
        {
            bool isSuccess = false;
            if (Cache.AddOrReplace(typeof(T), _instance, other))
            {
                _instance = other;
                isSuccess = true;
            }

            return isSuccess;
        }
    }
}