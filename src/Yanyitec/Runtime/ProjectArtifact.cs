﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yanyitec.Compilation;
using Yanyitec.Storaging;

namespace Yanyitec.Runtime
{
    public class ProjectArtifact : IArtifact
    {
        /// <summary>
        /// 如果第一个同步参数没有设置，就认为是独立的项目，会自己监听源代码目录
        /// 如果设置了第一个参数，则认为不是独立项目，由外面的来监听目录变化，调用Source_Changed
        /// ArtifactLoader 中使用
        /// </summary>
        /// <param name="synchronizingObject"></param>
        /// <param name="sourceDir"></param>
        /// <param name="outputDir"></param>
        /// <param name="assemblyLoader"></param>
        public ProjectArtifact(System.Threading.ReaderWriterLockSlim synchronizingObject,IStorageDirectory sourceDir, IStorageDirectory outputDir, IArtifactLoader assemblyLoader)
        {
            
            this.SynchronizingObject = synchronizingObject ?? new System.Threading.ReaderWriterLockSlim();
            this.Location = sourceDir;
            this.CacheName = sourceDir.Name;
            this.OutputDirectory = outputDir;
            this.ArtifactLoader = assemblyLoader;
            this.SourceStorage = sourceDir.AsStorage();
            if (synchronizingObject==null)this.SourceStorage.Changed += Source_Changed;
           
        }

        protected internal void Source_Changed(IStorageDirectory sender, ItemChangedEventArgs e)
        {
            if (!e.ChangedItem.FullName.EndsWith(".cs")) return;
            if (e.SynchronizingObject == this.SynchronizingObject)
            {
                InternalClear(e);
            }
            else
            {
                this.SynchronizingObject.EnterWriteLock();
                try
                {
                    this.InternalClear();
                }
                finally
                {
                    this.SynchronizingObject.ExitWriteLock();
                }
            }
        }

        void InternalClear(ChangedEventArgs sourceEvt=null) {
            this._assembly = null;
            //this._name = null;
            this._configuration = null;
            this._assemblyLocation = null;
            if (this._changed != null) this._changed(this, new ArtifactChangeEventArgs(ArtifactChangeTypes.Updated, this, this.SynchronizingObject, sourceEvt));
        }

        public void Clear(object synchronizingObject=null) {
            if (synchronizingObject == this.SynchronizingObject)
            {
                InternalClear();
            }
            else
            {
                this.SynchronizingObject.EnterWriteLock();
                try
                {
                    this.InternalClear();
                }
                finally
                {
                    this.SynchronizingObject.ExitWriteLock();
                }
            }
        }

        string _name;
        public string Name { get; private set; }

        Json.JObject _configuration;
        public Json.JObject Configuration { get; set; }
        
        public string CacheName { get; private set; }

        public string GetCacheName(System.Threading.ReaderWriterLockSlim syncObject = null)
        {
            return CacheName;
        }

        public IArtifactLoader ArtifactLoader { get; private set; }

        

        /// <summary>
        /// 文件变化时重新编译源及其后继依赖
        /// </summary>
        public IStorage SourceStorage { get; private set; }

        public IStorageItem Location { get; private set; }

        Action<IArtifact, ArtifactChangeEventArgs> _changed;
        public void AttachChangeHandler(Action<IArtifact, ArtifactChangeEventArgs> handler, System.Threading.ReaderWriterLockSlim syncObject = null)
        {
            if (this.SynchronizingObject == syncObject)
            {
                _changed += handler;
            }
            else
            {
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
        public void DetechChangeHandler(Action<IArtifact, ArtifactChangeEventArgs> handler, System.Threading.ReaderWriterLockSlim syncObject = null)
        {
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
        public event Action<IArtifact, ArtifactChangeEventArgs> Changed {
            add {
                this.SynchronizingObject.EnterWriteLock();
                try {
                    _changed += value;
                } finally {
                    this.SynchronizingObject.ExitWriteLock();
                }
            }
            remove {
                this.SynchronizingObject.EnterWriteLock();
                try {
                    _changed -= value;
                }finally{
                    this.SynchronizingObject.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 编译文件输出目录
        /// </summary>
        public IStorageDirectory OutputDirectory { get; private set; }

        public System.Threading.ReaderWriterLockSlim SynchronizingObject { get; private set; }


        IStorageFile _assemblyLocation;

        public IStorageFile AssemblyLocation {
            get
            {
                this.SynchronizingObject.EnterUpgradeableReadLock();
                try
                {
                    if (_assemblyLocation != null) return _assemblyLocation;
                    this.SynchronizingObject.EnterWriteLock();
                    try {
                        this.InternalCompile(this.SynchronizingObject);
                        return _assemblyLocation;
                    } finally {
                        this.SynchronizingObject.ExitWriteLock();
                    }
                }
                finally
                {
                    this.SynchronizingObject.ExitUpgradeableReadLock();
                }
            }
        }

        public Assembly GetAssembly(System.Threading.ReaderWriterLockSlim syncObject = null) {
            if (this.SynchronizingObject == syncObject) return this.InternalGetAssembly(syncObject);
            this.SynchronizingObject.EnterUpgradeableReadLock();
            try
            {
                if (_assembly != null) return _assembly;
                this.SynchronizingObject.EnterWriteLock();
                try
                {
                    return this.InternalGetAssembly(syncObject);
                }
                finally
                {
                    this.SynchronizingObject.ExitWriteLock();
                }
            }
            finally
            {
                this.SynchronizingObject.ExitUpgradeableReadLock();
            }
        }

        Assembly InternalGetAssembly(System.Threading.ReaderWriterLockSlim syncObject = null) {
            if (_assembly != null) return _assembly;
            this.InternalCompile(syncObject);
            return _assembly;
        }

        Assembly _assembly;
        public Assembly Assembly
        {
            get
            {
                this.SynchronizingObject.EnterUpgradeableReadLock();
                try
                {
                    if (_assembly != null) return _assembly;
                    this.SynchronizingObject.EnterWriteLock();
                    try
                    {
                        this.InternalCompile(this.SynchronizingObject);
                        return _assembly;
                    }
                    finally
                    {
                        this.SynchronizingObject.ExitWriteLock();
                    }
                }
                finally
                {
                    this.SynchronizingObject.ExitUpgradeableReadLock();
                }                
            }
        }
        IReadOnlyList<IArtifact> _references;
        public IReadOnlyList<IArtifact> References {
            get {
                this.SynchronizingObject.EnterReadLock();
                try {
                    return _references;
                } finally {
                    this.SynchronizingObject.ExitReadLock();
                }
            }
        }

        //public IArtifact 

        public void Compile(System.Threading.ReaderWriterLockSlim locker = null)
        {
            if (locker == this.SynchronizingObject) this.InternalCompile(locker);
            this.SynchronizingObject.EnterWriteLock();
            try
            {
                this.InternalCompile(this.SynchronizingObject);
            }
            finally
            {
                this.SynchronizingObject.ExitWriteLock();
            }
        }


        Json.JObject ReadProjectJson() {
            var item = this.SourceStorage.GetItem("project.json") as IStorageFile;
            if (item == null) return new Json.JObject();
            return new Json.Parser().Parse(item.GetText()) as Json.JObject ?? new Json.JObject();
        }
        List<IArtifact> GetReferences(Json.JObject projectJson,System.Threading.ReaderWriterLockSlim syncObject) {
            var result = new List<IArtifact>();
            var dep = projectJson["dependencies"] as Json.JObject;
            GetReferences(dep,result,syncObject);
            dep = (projectJson["frameworks"]?[Platform.DotnetVersion]?["dependencies"]) as Json.JObject;
            GetReferences(dep,result, syncObject);
            return result;
            
        }
        void GetReferences(Json.JObject depSection, List<IArtifact> result, System.Threading.ReaderWriterLockSlim syncObject) {
            if (depSection == null) return;
            foreach (var pair in depSection) {
                ArtifactLoaderOptions opt = null;
                if (pair.Key == "bin") {
                    opt = new ArtifactLoaderOptions(pair.Value) {  };
                } else {
                    opt = new ArtifactLoaderOptions(pair.Value) { Name = pair.Key };
                }
                var artifact = this.ArtifactLoader.Load(opt,syncObject);
                if (artifact != null) result.Add(artifact);
            }
        }
        void InternalCompile(System.Threading.ReaderWriterLockSlim syncLocker = null)
        {
            var compiler = this.CreateCompiler(syncLocker);
            this._configuration = ReadProjectJson();

            var name = this.Name + Guid.NewGuid().ToString().Replace("-", "") + ".dll";
            //var file = ".Compilation/" + name;
            var location = this._assemblyLocation = this.OutputDirectory.GetItem(name, StorageTypes.File, true) as IStorageFile;
            compiler.SetLocation(location, syncLocker);

            var items = this.SourceStorage.ListItems(true, StorageTypes.File);
            foreach (IStorageFile item in items)
            {
                if (item.Name.EndsWith(".cs"))
                {
                    compiler.AddOrReplaceCode(item.RelativeName, item.GetText());
                }
            }

            compiler.AddReference(typeof(object));
            compiler.AddReference(typeof(Guid));
            compiler.AddReference(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException));
            compiler.AddReference(typeof(System.Dynamic.DynamicMetaObjectBinder));
            var refcs = this._references= this.GetReferences(this._configuration, syncLocker).AsReadOnly();
            if (refcs != null) {
                foreach (var refc in refcs)
                {
                    if (compiler.AddReference(refc)) {
                        refc.AttachChangeHandler(Reference_Changed,syncLocker);
                    }
                }
            }
            

            var assembly = this._assembly = compiler.Compile(name, syncLocker);
            
        }

        private void Reference_Changed(IArtifact sender, ArtifactChangeEventArgs e)
        {
            if (e.SynchronizingObject == this.SynchronizingObject)
            {
                if (e.ChangeKind == ChangeKinds.Deleted)
                {

                    sender.Changed -= Reference_Changed;
                }
                this.InternalClear(e);
                
            }
            this.SynchronizingObject.EnterWriteLock();
            try {
                this.InternalClear(e);
            } finally {
                this.SynchronizingObject.ExitWriteLock();
            }
            
            
        }

        public ICompiler CreateCompiler(object synchronizingObject) {
            return CreateComplierFunc(synchronizingObject);
        }

        public IEnumerable<TypeInfo> GetTypeInfos()
        {
            return this.Assembly.DefinedTypes;
        }

        public IEnumerable<Attribute> GetAttributes()
        {
            return this.Assembly.GetCustomAttributes();
        }

        public T GetAttribute<T>() where T : Attribute
        {
            return (T)this.Assembly.GetCustomAttributes().FirstOrDefault(p=>p.GetType() == typeof(T));
        }

        public byte[] GetResource(string name)
        {
            return this.SourceStorage.GetBytes(name);
        }

        public string GetResourceText(string name, Encoding encoding = null)
        {
            return this.SourceStorage.GetText(name,encoding);
        }

        public static Func<object,ICompiler> CreateComplierFunc;
    }
}
