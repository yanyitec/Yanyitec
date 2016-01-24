using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Injecting
{
    public class InjectionChangedEventArgs : ChangedEventArgs
    {
        public InjectionChangedEventArgs(object sender, object locker ,ChangeKinds kind,ChangedEventArgs sourceEvent=null) {
            this.SynchronizingObject = locker;
            this.ChangeKind = kind;
            this.Sender = sender;
            this.SourceEventArgs = sourceEvent;
        }

    }
}
