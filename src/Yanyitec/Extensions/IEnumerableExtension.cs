using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public static class IEnumerableExtension
    {
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<TValue> self, Func<TValue,TKey> predicate) {
            var result = new Dictionary<TKey, TValue>();
            foreach (var item in self) result.Add(predicate(item),item);
            return result;
        }
    }
}
