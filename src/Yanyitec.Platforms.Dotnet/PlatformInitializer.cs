using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yanyitec.Compilation;

namespace Yanyitec.Platforms
{
    public static class Dotnet
    {
        public static void Init() {
            Yanyitec.Platform.SetDotnetVersion("net451");
            Yanyitec.Runtime.ProjectArtifact.CreateComplierFunc = (syncObject) => new CSharpCompiler();
        }
    }
}
