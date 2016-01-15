using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public static class StorageTypeExtension
    {
        public static bool IsFile(this StorageTypes self) {
            return self == StorageTypes.File || (((int)self & (int)StorageTypes.File )>0);
        }

        public static bool IsDirectory(this StorageTypes self)
        {
            return self == StorageTypes.Directory || (((int)self & (int)StorageTypes.Directory) > 0);
        }
    }
}
