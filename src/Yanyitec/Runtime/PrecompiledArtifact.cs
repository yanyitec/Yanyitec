using Yanyitec.Storaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    using System.Reflection;
    using System.Text;
    using System.IO;

    public class PrecompiledArtifact: IArtifact
    {
        public System.Threading.ReaderWriterLockSlim SynchronizingObject { get; private set; }

        public PrecompiledArtifact(IStorageFile location, System.Threading.ReaderWriterLockSlim synchronizingObject=null)
        {
            this.Location = location;
            this.AssemblyLocation = location;
            this.SynchronizingObject = synchronizingObject ?? new System.Threading.ReaderWriterLockSlim();
        }


        public PrecompiledArtifact(Assembly assembly, IStorageFile location =null, System.Threading.ReaderWriterLockSlim synchronizingObject = null) {
            this._assembly = assembly;
            this.SynchronizingObject = synchronizingObject ?? new System.Threading.ReaderWriterLockSlim();
            this.Location = location;
            this.AssemblyLocation = location;
        }
        public string Name
        {
            get { return this.Assembly.GetName().Name; }
        }

        public string GetCacheName(System.Threading.ReaderWriterLockSlim syncObject = null) {
            var asm = this.GetAssembly(syncObject);
            var n = asm.GetName();
            return n.Name + "." + n.Version.ToString();
        }
        public string CacheName {
            get { return GetCacheName(); }
        }
        

        public IStorageFile AssemblyLocation { get; private set; }

        public IStorageItem Location { get; private set; }

        

        Action<IArtifact, ArtifactChangeEventArgs> _changed;

        public void AttachChangeHandler(Action<IArtifact, ArtifactChangeEventArgs> handler, System.Threading.ReaderWriterLockSlim syncObject = null) {
            if (this.SynchronizingObject == syncObject)
            {
                _changed += handler;
            }
            else {
                this.SynchronizingObject.EnterWriteLock();
                try
                {
                    _changed += handler;
                }
                finally
                {
                    this.SynchronizingObject.ExitWriteLock();
                }
            }
        }
        public void DetechChangeHandler(Action<IArtifact, ArtifactChangeEventArgs> handler, System.Threading.ReaderWriterLockSlim syncObject = null) {
            if (this.SynchronizingObject == syncObject)
            {
                _changed -= handler;
            }
            else
            {
                this.SynchronizingObject.EnterWriteLock();
                try
                {
                    _changed -= handler;
                }
                finally
                {
                    this.SynchronizingObject.ExitWriteLock();
                }
            }
        }


        public event Action<IArtifact, ArtifactChangeEventArgs> Changed
        {
            add
            {
                AttachChangeHandler(value,this.SynchronizingObject);
            }
            remove
            {
                DetechChangeHandler(value,this.SynchronizingObject);
            }
        }

        Assembly _assembly;
        public Assembly GetAssembly(System.Threading.ReaderWriterLockSlim syncObject = null) {
            if (syncObject == this.SynchronizingObject) return InternalGetAssembly();
            this.SynchronizingObject.EnterUpgradeableReadLock();
            try
            {
                return this.InternalGetAssembly();
            }
            finally
            {
                this.SynchronizingObject.ExitUpgradeableReadLock();
            }
        }
        Assembly InternalGetAssembly() {
            if (_assembly != null) return _assembly;
            if (this.AssemblyLocation == null) return null;
            IStorageFile pdbFile = null;
            if (AssemblyLoader.CanLoadPDB)
            {
                var dotAt = this.AssemblyLocation.Name.LastIndexOf(".");
                if (dotAt >= 0)
                {
                    var name = this.AssemblyLocation.Name.Substring(0, dotAt);
                    var pdbFilename = name + "pdb";
                    pdbFile = this.AssemblyLocation.Parent.GetFile(pdbFilename);
                }
            }
            using (var assembly = this.AssemblyLocation.GetStream())
            {
                if (pdbFile != null)
                {
                    using (var pdb = pdbFile.GetStream())
                    {
                        _assembly = AssemblyLoader.Load(assembly, pdb);
                    }
                }
                else
                {
                    _assembly = AssemblyLoader.Load(assembly);
                }
            }
            return _assembly;
        }
        public Assembly Assembly {
            get {
                this.SynchronizingObject.EnterUpgradeableReadLock();
                try {
                    if (_assembly != null) return _assembly;
                    this.SynchronizingObject.EnterWriteLock();
                    try {
                        return this.InternalGetAssembly();
                    } finally {
                        this.SynchronizingObject.ExitWriteLock();
                    }
                    
                } finally {
                    this.SynchronizingObject.ExitUpgradeableReadLock();
                }
            }
        }

        public IEnumerable<TypeInfo> GetTypeInfos() {
            return this.Assembly.DefinedTypes;
        }

        public IEnumerable<Attribute> GetAttributes() {
            return this.Assembly.GetCustomAttributes();
        }

        public T GetAttribute<T>() where T : Attribute {
            return (T)this.Assembly.GetCustomAttributes().FirstOrDefault(p=> p.GetType()==typeof(T));
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public byte[] GetResource(string name) { return null; }
        /// <summary>
        /// 获取资源字符串
        /// </summary>
        /// <returns></returns>
        public string GetResourceText(string name, Encoding encoding = null)
        {
            return null;
        }

        public static implicit operator PrecompiledArtifact(Assembly assembly) {
            return new PrecompiledArtifact(assembly);
        }

        public static implicit operator Assembly(PrecompiledArtifact artifact) {
            return artifact.Assembly;
        }

        
    }
}
