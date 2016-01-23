using System;
using System.Reflection;
using System.Threading;
using Yanyitec.Storaging;

namespace Yanyitec.Compilation
{
    public interface ICompiler
    {
        ReaderWriterLockSlim AsyncLocker { get; }
        IFile Location { get; set; }

        object AddOrReplaceCode(string key, string code);
        bool AddReference(Type keytype, object locker = null);
        bool AddReference(string assemblyLocation, object locker = null);
        bool AddReference(Assembly assembly, object locker = null);
        Assembly Compile(string name, object locker = null);
        IFile GetLocation(object locker = null);
        void SetLocation(string file, object locker = null);
        void SetLocation(IFile file, object locker = null);
    }
}