using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Injecting
{
    using System.Reflection;
    public class ItemInfo
    {
        public string Name { get; set; }

        public Type InjectionType { get; set; }

        public InjectionKinds Kind { get; set; }

        public bool isDefination { get; set; }

        public Type TokenType { get; set; }

        public string TypeName { get; set; }

        public object ConstValue { get; set; }

        public Func<object> InstanceFactory { get; set; }
    }
}
