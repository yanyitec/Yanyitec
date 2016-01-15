

namespace Yanyitec.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;
    /// <summary>
    /// 编译器
    /// </summary>
    public interface ICompiler
    {
        /// <summary>
        /// 从assembly文件中添加引用
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        void AddReferenceFromFile(string filename);

        void AddReference(IArtifact artifact);
        /// <summary>
        /// 根据某个类型添加引用
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        void AddReferenceFromType(Type type);
        /// <summary>
        /// 加载某个assembly作为引用
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        void AddReferenceFromAssembly(Assembly assembly);

        
        /// <summary>
        /// 添加代码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        ICompiler AddCode(string code);
        /// <summary>
        /// 解析出语法树
        /// </summary>
        /// <returns></returns>
        ICompiler Parse();

        Assembly Compile();
    }
}
