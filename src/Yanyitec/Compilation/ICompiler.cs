using System;
using System.Reflection;
using System.Threading;
using Yanyitec.Storaging;

namespace Yanyitec.Compilation
{
    public interface ICompiler
    {
        System.Threading.ReaderWriterLockSlim SynchronizingObject { get; }
        IStorageFile Location { get; set; }

        object AddOrReplaceCode(string key, string code,System.Threading.ReaderWriterLockSlim locker = null);
        bool AddReference(Type keytype, System.Threading.ReaderWriterLockSlim locker = null);

        bool AddReference(IArtifact artifact, System.Threading.ReaderWriterLockSlim locker = null);
        bool AddReference(string assemblyLocation, System.Threading.ReaderWriterLockSlim locker = null);
        bool AddReference(Assembly assembly, System.Threading.ReaderWriterLockSlim locker = null);
        Assembly Compile(string name, System.Threading.ReaderWriterLockSlim locker = null);
        IStorageFile GetLocation(System.Threading.ReaderWriterLockSlim locker = null);
        void SetLocation(string file, System.Threading.ReaderWriterLockSlim locker = null);
        void SetLocation(IStorageFile file, System.Threading.ReaderWriterLockSlim locker = null);
    }
}