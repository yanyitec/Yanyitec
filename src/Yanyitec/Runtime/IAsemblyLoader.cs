
namespace Yanyitec.Runtime
{
    using Storaging;
    using System.IO;
    using System.Reflection;
    public interface IAsemblyLoader
    {
        Assembly LoadFromName(string assemblyName);
        Assembly LoadFromFile(string file);
        Assembly LoadProject(IDirectory location);
        Assembly LoadFromStream(Stream assembly, Stream assemblySymbols);
    }
}
