using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec
{
    public interface IArtifact
    {
        event Action<IArtifact, ArtifactChangeEventArgs> OnChange;

        event Action<IArtifact, ArtifactChangeEventArgs> Changed;

        bool IsValid { get; }
        string Name { get; }

        Assembly Assembly { get; }



        Type GetType(string name);
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
