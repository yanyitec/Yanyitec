using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    public class ModuleInfo
    {
        public ModuleInfo(IHost host, IArtifact artifact) {
            this.Host = host;
            this.Artifact = artifact;
        }
        public string Name { get; protected internal set; }

        public IHost Host { get; private set; }

        public IArtifact Artifact { get; set; }

        public bool IsActived(bool useLock) { return false; }

        public void Disable(bool useLock) { }
        public void Enable(bool useLock) {  }

        //IModule _instance;
        //public IModule Instance {
        //    get {

        //    }
        //}

        public bool RunMain(object parameters) { return false; }
    }
}
