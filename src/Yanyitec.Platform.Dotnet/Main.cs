using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Yanyitec.Runtime;
using Yanyitec.Testing;

namespace Yanyitec.Platform.Dotnet
{
    public class Program
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Main() {
            Assert.ThrowException = true;
            var test = new Testing.TestArtifactInfo(new Artifact(typeof(Program).Assembly));
            
            test.TestMethods("Storage%");
        }

        
    }
}
