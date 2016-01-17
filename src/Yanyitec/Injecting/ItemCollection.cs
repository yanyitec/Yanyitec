

namespace Yanyitec.Injecting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Collections;

    public class ItemCollection :IEnumerable<Injection>
    {
        readonly SortedDictionary<string, Injection> _namedItems = new SortedDictionary<string, Injection>();
        readonly SortedDictionary<int, Injection> _typedItems = new SortedDictionary<int, Injection>();
        


        public Injection this[string name] {
            get {
                return GetByName(name);
            }
        }

        public IEnumerator<Injection> NamedItemEnumerator
        {
            get { return _namedItems.Values.GetEnumerator(); }
        }

        public IEnumerator<Injection> TypedItemEnumerator
        {
            get { return _typedItems.Values.GetEnumerator(); }
        }

        public void AddByName(Injection injection) {
            this._namedItems.Add(injection.Name, injection);
        }

        public void AddByType(Injection injection) {
            this._typedItems.Add(injection.TokenType.GetHashCode(),injection);
        }

        public Injection GetByName(string name,bool includeTypeName =true) {
            Injection result = null;
            if (_namedItems.TryGetValue(name, out result)) return result;
            if (includeTypeName) {
                foreach (var sub in _typedItems.Values) {
                    if (sub.TokenType != null && sub.TokenType.FullName == name) return sub;
                }
            }
            return null;
        }

        public Injection GetByType(Type type, bool includeNamedItems=true) {
            Injection result = null;
            if (_typedItems.TryGetValue(type.GetHashCode(), out result)) return result;
            if (includeNamedItems)
            {
                
                foreach (var sub in _namedItems.Values)
                {
                    var subType = sub.TokenType;
                    if (subType != null && type.IsAssignableFrom(subType)) return sub;
                }
            }
            return null;
        }

        public IEnumerator<Injection> GetEnumerator()
        {
            return new ItemCollectionEnumerator(this._namedItems.Values.GetEnumerator(),this._typedItems.Values.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ItemCollectionEnumerator(this._namedItems.Values.GetEnumerator(), this._typedItems.Values.GetEnumerator());
        }

        public class ItemCollectionEnumerator : IEnumerator<Injection> {
            public ItemCollectionEnumerator(IEnumerator<Injection> named, IEnumerator<Injection> typed) {
                this._named = named;
                this._current = this._typed = typed;
            }

            IEnumerator<Injection> _named;
            IEnumerator<Injection> _typed;
            IEnumerator<Injection> _current;
            public Injection Current
            {
                get
                {
                    return _current.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _current.Current;
                }
            }

            public void Dispose()
            {
                _named.Dispose();
                _typed.Dispose();
            }

            public bool MoveNext()
            {
                if (!this._current.MoveNext()) {
                    if (this._current == this._typed) {
                        this._current = this._named;
                        return this._current.MoveNext();
                    }
                }
                return true;
            }

            public void Reset()
            {
                this._named.Reset();
                this._typed.Reset();
                this._current = this._typed;
            }
        }
    }
}
