using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    public class ModuleInfo
    {
        public string Name { get; protected internal set; }

        public IArtifact Artifact { get; set; }

        public bool IsActived(bool useLock) { return false; }

        public void Disable(bool useLock) { }
        public void Enable(bool useLock) {  }

        public bool RunMain(object parameters) { return false; }
    }
}
