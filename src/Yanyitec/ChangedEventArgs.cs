using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public class ChangedEventArgs : System.EventArgs
    {
        public ChangedEventArgs() { }

        public object SynchronizingObject { get;protected set; }

        public ChangeKinds ChangeKind { get; protected set; }

        public ChangedEventArgs SourceEventArgs { get; protected set; }

        public object Sender { get; protected set; }

        public object Extra { get; set; }
    }
}
