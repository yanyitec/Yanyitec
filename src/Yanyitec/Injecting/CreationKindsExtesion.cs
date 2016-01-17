using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Injecting
{
    public static class CreationKindsExtesion
    {
        public static bool IsContainer(this InjectionKinds self) {
            return self == InjectionKinds.Constant;
        }

        public static bool IsRuntimeNew(this InjectionKinds self) {
            return(((int)self & (int)InjectionKinds.RuntimeNew)>0);
        }

        public static bool IsSingleon(this InjectionKinds self)
        {
            return (((int)self & (int)InjectionKinds.Singleon) > 0);
        }
    }
}
