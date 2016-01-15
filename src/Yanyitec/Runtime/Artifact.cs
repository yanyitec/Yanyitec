using Yanyitec.Storaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    using System.Reflection;
    public class Artifact
    {
        readonly object AsyncLocker = new object();

        public Artifact(IStorageItem location ,IAsemblyLoader loader) {
            this.Location = location;
            this.AsemblyLoader = loader;
            //location.OnChange += (sender, evtArgs) =>
            //{
            //    lock (this.AsyncLocker)
            //    { 
            //        this._assembly = null;
            //    }
            //};
        }

        

        public IAsemblyLoader AsemblyLoader { get; private set; }

        public IStorageItem Location { get; private set; }


        Assembly _assembly;
        Assembly GetOrLoadAssembly() {
            if (_assembly == null)
            {
                if (this.Location.StorageType == StorageTypes.File)
                {
                    _assembly = this.AsemblyLoader.LoadFromStream((this.Location as IFile).GetStream(false), null);
                }
                else
                {
                    _assembly = this.AsemblyLoader.LoadProject(this.Location as IDirectory);
                }
            }
            return _assembly;
        }

        public Assembly Assembly {
            get {
                if (_assembly == null) {
                    lock (this.AsyncLocker) {
                        GetOrLoadAssembly();
                    }
                }
                return _assembly;
            }
        }

        public IEnumerable<TypeInfo> GetTypes() {
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
        public string GetResourceText(string name) {
            return null;
        }
    }
}
