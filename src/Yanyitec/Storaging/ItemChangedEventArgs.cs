using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public class ItemChangedEventArgs : ChangedEventArgs
    {
        public ItemChangedEventArgs(object sender, IStorageItem changedItem ,ChangeKinds kind,string oldName, object locker=null, ItemChangedEventArgs source=null) {
            this.ChangeKind = kind;
            this.SynchronizingObject = locker;
            this.SourceEventArgs = source;
            this.Sender = sender;
            this.ChangedItem = changedItem;
            this.OldName = oldName;
        }

        public IStorageItem ChangedItem { get; private set; }

        public string OldName { get; set; }
    }
}
