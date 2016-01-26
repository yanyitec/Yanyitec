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
        readonly object SynchronizingObject = new object();
        public event Action<IArtifact, ArtifactChangeEventArgs> Changed;

        public PrecompiledArtifact(IStorageFile location = null)
        {

            this.Location = location;

        }


        public PrecompiledArtifact(Assembly assembly, IStorageFile location =null) {
            this.Assembly = assembly;
            
            this.Location = location;
            
        }

        public string CompairName {
            get { return this.Assembly.FullName + "-" + this.Assembly.GetName().Version.ToString(); }
        }
        

        public IArtifactLoader AsemblyLoader { get; private set; }

        public IStorageItem Location { get; private set; }

        public string Name {
            get { return this.Assembly.FullName; }
        }


       
        public Assembly Assembly {
            get; private set;
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
