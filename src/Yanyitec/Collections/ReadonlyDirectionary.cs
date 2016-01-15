using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Collections
{
    public class ReadonlyDirectionary<TKey,TValue> : IReadOnlyDictionary<TKey,TValue>
    {
        public ReadonlyDirectionary(IDictionary<TKey, TValue> data) {
            this.Orignal = data;
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.Orignal[key];
            }
        }

        public int Count
        {
            get
            {
                return this.Orignal.Count;
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                return this.Orignal.Keys;
            }
        }

        public IDictionary<TKey, TValue> Orignal { get; private set; }

        public IEnumerable<TValue> Values
        {
            get
            {
                return this.Orignal.Values;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return this.Orignal.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.Orignal.GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.Orignal.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Orignal.GetEnumerator();
        }
    }
}
