using System;
using System.Collections.Generic;
using Adfnet.Core.Constants;
using Microsoft.Extensions.Caching.Memory;

namespace Adfnet.Core.Caching
{

    /// <inheritdoc />
    /// <summary>
    /// RAM önbellekme işlemlerini yapan sınıf
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly int _cacheTime;

        private readonly IMemoryCache _cache;
        public MemoryCacheService(IMemoryCache cache, int cacheTime)
        {
            _cache = cache;
            _cacheTime = cacheTime;
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out var o);

        }

        public void Add(string key, object value)
        {
            // Değer boş ise ekleme yapılmaz
            if (value == null)
            {
                return;
            }

            // Anahtar daha öce kullanıldı ise ekleme yapılmaz
            if (Exists(key))
            {
                return;
            }

            // Kayıt eklenir
            _cache.Set(key, value, DateTimeOffset.Now.AddHours(_cacheTime));
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public List<string> GetAllKeyList()
        {
            var list = new List<string>();
            if (Exists(CacheConstants.CacheMainKey))
            {
                list = Get<List<string>>(CacheConstants.CacheMainKey);
            }
            return list;
        }

        public void AddToKeyList(string key)
        {
            var keyList = GetAllKeyList();

            if (!keyList.Contains(key))
            {
                keyList.Add(key);
            }
            Remove(CacheConstants.CacheMainKey);
            Add(CacheConstants.CacheMainKey, keyList);
        }

        public void RemoveFromKeyList(string key)
        {
            var keyList = GetAllKeyList();

            if (keyList.Contains(key))
            {
                keyList.Remove(key);
            }
            Remove(CacheConstants.CacheMainKey);
            Add(CacheConstants.CacheMainKey, keyList);
        }

        public void CleanKeyList()
        {
            foreach (var key in GetAllKeyList())
            {
                Remove(key);
            }
            Remove(CacheConstants.CacheMainKey);
        }
    }
}
