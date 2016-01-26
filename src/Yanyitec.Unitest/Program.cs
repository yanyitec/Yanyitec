

namespace Yanyitec.Unitest
{
    using System;
    using System.Reflection;
    using System.Linq;
    using System.Linq.Expressions;
    using Yanyitec.Runtime;
    using System.Reflection.Emit;
    using Testing;
    using System.IO;

    public class Program
    {
        //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Main(string[] args)
        {
            //Expression<Func<string, int&, bool>> expr;
            
            //var paras = method.GetParameters();

      
            var test = new Testing.TestArtifactInfo(new PrecompiledArtifact(typeof(Program).GetTypeInfo().Assembly));

            test.TestMethods("Storage%");
            Console.Write("Press q to exit..");
            while (Console.Read() != 'q');
        }

        
    }
}
