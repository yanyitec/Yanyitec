using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    [Flags]
    public enum ValueType
    {
        Undefined =1,
        Boolean = 1<<1,
        Numeric = 1<<2,
        Int = 1<<3 | Numeric,
        Float = 1<<4 | Numeric,
        String = 1<<16,
        Char = 1<<17 | String,
        Function = 1<<18,
        Object = 1<<19,
        Null = 1<<20 | Object,
        Array = 1<< 21 | Object,
        Unknown = 1<<31
    }
}
