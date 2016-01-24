﻿using Yanyitec.Storaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    using System.Reflection;
    using System.Text;
    using System.IO;

    public class Artifact: IArtifact
    {
        readonly object SynchronizingObject = new object();
        public event Action<IArtifact, ArtifactChangeEventArgs> Changed;

        private FileSystemWatcher _fsWatcher;
        public Artifact(IStorageItem location ,IAsemblyLoader loader=null) {
            this.Location = location;
            this.AsemblyLoader = loader;
            //location.OnChange += (sender, evtArgs) =>
            //{
            //    lock (this.SynchronizingObject)
            //    { 
            //        this._assembly = null;
            //    }
            //};
        }

        public Artifact(Assembly assembly, IStorageFile location =null) {
            this._assembly = assembly;
            
            this.Location = location;
            
        }

        

        public IAsemblyLoader AsemblyLoader { get; private set; }

        public IStorageItem Location { get; private set; }

        public string Name {
            get { return this.Assembly.FullName; }
        }


        Assembly _assembly;
        Assembly GetOrLoadAssembly() {
            if (_assembly == null)
            {
                if (this.Location.StorageType == StorageTypes.File)
                {
                    _assembly = this.AsemblyLoader.LoadFromStream((this.Location as IStorageFile).GetStream(false), null);
                }
                else
                {
                    _assembly = this.AsemblyLoader.LoadProject(this.Location as IStorageDirectory);
                }
            }
            return _assembly;
        }

        public Assembly Assembly {
            get {
                if (_assembly == null) {
                    lock (this.SynchronizingObject) {
                        GetOrLoadAssembly();
                    }
                }
                return _assembly;
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

        public static implicit operator Artifact(Assembly assembly) {
            return new Artifact(assembly);
        }

        public static implicit operator Assembly(Artifact artifact) {
            return artifact.Assembly;
        }
    }
}
