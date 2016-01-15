

namespace Yanyitec.Injecting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;
    public class ItemCollection
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
            this._typedItems.Add(injection.InjectionType.GetHashCode(),injection);
        }

        public Injection GetByName(string name,bool includeTypeName =true) {
            Injection result = null;
            if (_namedItems.TryGetValue(name, out result)) return result;
            if (includeTypeName) {
                foreach (var sub in _typedItems.Values) {
                    if (sub.InjectionType != null && sub.InjectionType.FullName == name) return sub;
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
                    var subType = sub.InjectionType;
                    if (subType != null && type.IsAssignableFrom(subType)) return sub;
                }
            }
            return null;
        }
    }
}
