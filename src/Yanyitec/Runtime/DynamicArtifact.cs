using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yanyitec.Compilation;
using Yanyitec.Storaging;

namespace Yanyitec.Runtime
{
    public class DynamicArtifact : IArtifact
    {
        public DynamicArtifact(object locker,string name,IStorageDirectory sourceDir, IStorageDirectory tempDir, IAsemblyLoader assemblyLoader)
        {
            this.Name = name;
            this.SynchronizingObject = locker?? new object();
            this.Location = sourceDir;
            this.SourceStorage = sourceDir.AsStorage();
            this.SourceStorage.Changed += SourceStorage_Changed;
            this.TempDirectory = tempDir;
            
        }

        private void SourceStorage_Changed(IStorageDirectory sender, ItemChangedEventArgs e)
        {
            Task.Run(() => {
                if (e.SynchronizingObject == this.SynchronizingObject)
                {
                    this._assembly = null;
                    this.InternalCompile(this.SynchronizingObject);
                    if (this._changed != null) this._changed(this,new ArtifactChangeEventArgs(ArtifactChangeTypes.Updated,this,this.SynchronizingObject, e));
                }
                else {
                    lock(this.SynchronizingObject)
                    {
                        this._assembly = null;
                        this.InternalCompile(this.SynchronizingObject);
                        if (this._changed != null) this._changed(this, new ArtifactChangeEventArgs(ArtifactChangeTypes.Updated, this, this.SynchronizingObject, e));
                    } 
                }
                
            });
        }

        public string Name { get; private set; }


               

        /// <summary>
        /// 文件变化时重新编译源及其后继依赖
        /// </summary>
        public IStorage SourceStorage { get; private set; }

        public IStorageItem Location { get; private set; }

        Action<IArtifact, ArtifactChangeEventArgs> _changed;
        public event Action<IArtifact, ArtifactChangeEventArgs> Changed {
            add {
                lock (this.SynchronizingObject) {
                    _changed += value;
                }

            }
            remove {
                lock (this.SynchronizingObject)
                {
                    _changed -= value;
                }
            }
        }

        /// <summary>
        /// 临时文件目录
        /// </summary>
        public IStorageDirectory TempDirectory { get; private set; }

        public object SynchronizingObject { get; private set; }


        IStorageFile _assemblyLocation;

        public IStorageFile AssemblyLocation {
            get
            {
                lock( this.SynchronizingObject)
                {
                    if (_assemblyLocation != null) return _assemblyLocation;
                    this.Compile(this.SynchronizingObject);
                    return _assemblyLocation;
                }
                
            }
        }

        internal List<IArtifact> References { get; set; }

        Assembly _assembly;
        public Assembly Assembly
        {
            get
            {
                lock (this.SynchronizingObject) {
                    if (_assembly != null) return _assembly;
                    this.InternalCompile(this.SynchronizingObject);
                    return _assembly;
                }
                
            }
        }

        //public IArtifact 

        public void Compile(object locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) this.InternalCompile(locker);
            lock(this.SynchronizingObject)
            {
                this.InternalCompile(locker);
            }
            
        }

        void InternalCompile(object locker = null)
        {

            var compiler = this.CreateCompiler(this.SynchronizingObject);
            var name = this.Name + Guid.NewGuid().ToString().Replace("-", "") + ".dll";
            var file = ".Compilation/" + name;
            var location = this._assemblyLocation = this.TempDirectory.GetItem(file, StorageTypes.File, true) as IStorageFile;
            compiler.SetLocation(location, locker);

            var items = this.SourceStorage.ListItems(true, StorageTypes.File);
            foreach (IStorageFile item in items)
            {
                if (item.Name.EndsWith(".cs"))
                {
                    compiler.AddOrReplaceCode(item.RelativeName, item.GetText());
                }
            }

            compiler.AddReference(typeof(object));
            compiler.AddReference(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException));
            compiler.AddReference(typeof(System.Dynamic.DynamicMetaObjectBinder));
            if (this.References != null) {
                foreach (var refc in this.References)
                {
                    compiler.AddReference(refc.Assembly);
                }
            }
            

            var assembly = this._assembly = compiler.Compile(name, locker);
            
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
