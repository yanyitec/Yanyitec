using Yanyitec.Storaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Yanyitec.Compilation
{
    public abstract class DynamicArtifact
    {
        public DynamicArtifact(DynamicMetadataReference reference) {
            this._reference = reference;
            this.Storage = reference.Storage;
        }

        DynamicMetadataReference _reference;

        readonly object asyncLocker = new object();

        public IStorage Storage { get; private set; }

        



        

        List<IMetadataReference> _references;
        public bool AddReference(IMetadataReference reference)
        {
            if (_references.Exists(p => p.Equals(reference))) return false;
            _references.Add(reference);
            reference.OnChange += (t) => {
                lock(asyncLocker) this._assembly = null;
            };
            return true;
        }

        private void Reference_OnChange()
        {
            lock (this)
            {
                _assembly = null;
            }

        }

        Assembly _assembly;
    }
}
