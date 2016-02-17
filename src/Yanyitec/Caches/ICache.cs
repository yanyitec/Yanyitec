using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Caches
{
    public interface ICache : IEnumerable<CacheItem>
    {
        object this[string name] {
            get; set;
        }

        CacheItem GetItem(string name);

        CacheItem SetItem(string name, object value, DateTime expireTime);
        CacheItem SetItem(string name, object value);
        int ExpireMilliseconds { get; set; }

        int CheckIntervalMilliseconds { get; set; }

        ICache GetOrCreateSubCache(string name);

        int Count { get; }
    }
}
