using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Injecting
{
    public class Injection
    {
        public string Name { get; set; }

        public Type InjectionType { get; set; }

        public T CreateInstance<T>() { return default(T); }

        public object CreateInstance(string name) { return null; }
    }
}
