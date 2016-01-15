using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public static class DirectionaryExtension
    {
        public static IReadOnlyDictionary<TKey, TValue> AsReadonly<TKey, TValue>(this IDictionary<TKey, TValue> innerData) {
            return new Yanyitec.Collections.ReadonlyDirectionary<TKey,TValue>(innerData);
        }

        public static IDictionary<TKey, TValue> Join<TKey, TValue>(this IDictionary<TKey, TValue> self, params IDictionary<TKey, TValue>[] others) {
            foreach (var other in others) {
                foreach (var pair in other) {
                    if (self.ContainsKey(pair.Key)) self[pair.Key] = pair.Value;
                    else self.Add(pair.Key,pair.Value);
                }
            }
            return self;
        }
    }
}
