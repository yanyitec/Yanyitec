using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public class StorageChangeEventArgs
    {
        public StorageChangeEventArgs(StorageChangeTypes type, IStorageItem item, string oldName) {
            this.ChangeType = type;
            this.Item = item;
            this.OldName = oldName;
        }
        public StorageChangeTypes ChangeType { get; private set; }

        public IStorageItem Item { get; private set; }

        public string OldName { get; private set; }
    }
}
