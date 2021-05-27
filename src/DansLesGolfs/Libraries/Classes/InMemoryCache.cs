using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DansLesGolfs
{
    public class InMemoryCache : ICache<string, object>
    {
        private MemoryCache _cache;

        public InMemoryCache(string name = "SampleCache")
        {
            _cache = new MemoryCache(name);
        }

        public object Get(string key)
        {
            return _cache[key];
        }

        public void Put(string key, object value)
        {
            _cache[key] = value;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            var oldCache = _cache;
            _cache = new MemoryCache("SampleCache");
            oldCache.Dispose();
        }
    }
}
