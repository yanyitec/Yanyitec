using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Unitest
{
    using System.Reflection;
    using Yanyitec.Runtime;
    using Testing;

    public class Program
    {
        public static void Main(string[] args)
        {
            Assert.ThrowException = true;
            var test = new Testing.TestArtifactInfo(new Artifact(typeof(Program).GetTypeInfo().Assembly));

            test.TestMethods("Storage%");
        }
    }
}
