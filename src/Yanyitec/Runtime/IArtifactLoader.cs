
namespace Yanyitec.Runtime
{
    using Storaging;
    using System.IO;
    using System.Reflection;
    public interface IArtifactLoader
    {
        IArtifact Load(IStorageItem location);

        IArtifact Load(string name, ArtifactLoaderOptions opts);

        

        IArtifact LoadProject(IStorageDirectory projectDirectory);
        IArtifact LoadAssembly(Assembly assembly ,string name=null);

        IArtifact LoadPackage(IStorageDirectory packageDirectory);

        IArtifact LoadStream(Stream assembly, Stream pcd = null);
    }
}
