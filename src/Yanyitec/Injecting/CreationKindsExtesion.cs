using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Injecting
{
    public static class CreationKindsExtesion
    {
        public static bool IsContainer(this CreationKinds self) {
            return self == CreationKinds.Constant;
        }

        public static bool IsRuntimeNew(this CreationKinds self) {
            return(((int)self & (int)CreationKinds.RuntimeNew)>0);
        }

        public static bool IsSingleon(this CreationKinds self)
        {
            return (((int)self & (int)CreationKinds.Singleon) > 0);
        }
    }
}
