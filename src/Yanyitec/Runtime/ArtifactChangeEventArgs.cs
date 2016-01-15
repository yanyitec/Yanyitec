using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Runtime;

namespace Yanyitec
{
    public class ArtifactChangeEventArgs : EventArgs
    {
        public ArtifactChangeEventArgs(ArtifactChangeTypes changeType) {
            this.ChangeType = changeType;
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
