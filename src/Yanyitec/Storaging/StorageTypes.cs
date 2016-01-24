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
        
        File = 1,
        Directory = 1<<1,
        Storage = 1<<30 | Directory,
        Root = 1<<31 | Storage,
        All = File |  Directory | Storage
    }
}
