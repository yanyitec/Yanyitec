using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    [Flags]
    public enum StorageTypes
    {
        None = 0,
        Storage = 1,
        File = 4,
        Attribute =24,
        Directory = 25 | Storage,
        All = File |  Directory | Attribute
    }
}
