using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Yanyitec.Runtime;
using Yanyitec.Testing;

namespace Yanyitec.Platforms
{
    public class Program
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Main() {
            Yanyitec.Platforms.Dotnet.Init();
            var test = new Testing.TestArtifactInfo(new Artifact(typeof(Program).Assembly));
            
            test.TestMethods("Artifact%");
        }

        
    }
}
