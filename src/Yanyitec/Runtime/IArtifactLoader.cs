using System.Threading;
using Yanyitec.Storaging;

namespace Yanyitec.Runtime
{
    public interface IArtifactLoader
    {
        IStorageDirectory OutputDirectory { get; set; }
        IStorageDirectory PackageDirectory { get; set; }
        ArtifactLoader Parent { get; set; }
        ReaderWriterLockSlim SynchronizingObject { get; }
        IArtifact Load(ArtifactLoaderOptions opt);
        IArtifact Load(string name, string version = null);
        IArtifact LoadPackage(string name, string version = null, ReaderWriterLockSlim synchonizingObject = null);
        IArtifact LoadProject(IStorageDirectory projectDir, ReaderWriterLockSlim synchonizingObject = null);
    }
}