using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Runtime;

namespace Yanyitec
{
    public class ArtifactChangeEventArgs : ChangedEventArgs
    {
        public ArtifactChangeEventArgs(ArtifactChangeTypes changeType,object sender,object syncLocker, ChangedEventArgs source ) {
            this.ChangeType = changeType;
            this.Sender = sender;
            this.SourceEventArgs = source;
            this.SynchronizingObject = syncLocker;
        }
        public ArtifactChangeEventArgs(string oldName)
        {
            this.ChangeType =  ArtifactChangeTypes.Rename;
            this.OldName = oldName;
        }
        public ArtifactChangeTypes ChangeType { get; private set; }
        public string OldName { get; private set; }
    }
}
