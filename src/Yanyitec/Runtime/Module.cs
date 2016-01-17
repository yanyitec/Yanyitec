using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    public class Module : IModule
    {
        readonly Dictionary<string, object> _states = new Dictionary<string, object>();
        readonly System.Threading.ReaderWriterLockSlim _asyncLocker = new System.Threading.ReaderWriterLockSlim();
        public System.Threading.ReaderWriterLockSlim AsyncLocker { get { return _asyncLocker; } }

        public Module() {
            
        }

        public IHost Host { get; protected set; }

        public IArtifact Artifact { get; protected set; }

        public string Name { get; protected set; }

        public object this[string key]
        {
            get
            {
                _asyncLocker.EnterReadLock();
                try
                {
                    object result = null;
                    this._states.TryGetValue(key, out result);
                    return result;
                }
                finally
                {
                    _asyncLocker.ExitReadLock();
                }
            }
            set
            {
                _asyncLocker.EnterWriteLock();
                try
                {
                    if (this._states.ContainsKey(key)) this._states[key] = value;
                    this._states.Add(key, value);
                }
                finally
                {
                    _asyncLocker.ExitWriteLock();
                }
            }
        }

        public virtual void Initialize(IHost host, IArtifact artifact)
        {
            this.Host = host;
            this.Artifact = artifact;
        }

        public virtual bool RunMain(object parameters)
        {
            return true;
        }
    }
}
