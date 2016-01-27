using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yanyitec.Storaging;

namespace Yanyitec
{
    public interface IArtifact
    {
        

        event Action<IArtifact, ArtifactChangeEventArgs> Changed;

        void AttachChangeHandler(Action<IArtifact, ArtifactChangeEventArgs> handler, System.Threading.ReaderWriterLockSlim syncObject = null);
        void DetechChangeHandler(Action<IArtifact, ArtifactChangeEventArgs> handler, System.Threading.ReaderWriterLockSlim syncObject = null);


        //bool IsValid { get; }
        string Name { get; }
        Assembly GetAssembly(System.Threading.ReaderWriterLockSlim syncObject = null);
        Assembly Assembly { get; }

        IStorageFile AssemblyLocation { get; }

        string CacheName { get; }

        string GetCacheName(System.Threading.ReaderWriterLockSlim syncObject = null);
        //Type GetType(string name);
        IEnumerable<TypeInfo> GetTypeInfos();

        IEnumerable<Attribute> GetAttributes();

        T GetAttribute<T>() where T : Attribute;
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        byte[] GetResource(string name);
        /// <summary>
        /// 获取资源字符串
        /// </summary>
        /// <returns></returns>
        string GetResourceText(string name,Encoding encoding = null);

        

    }
}
