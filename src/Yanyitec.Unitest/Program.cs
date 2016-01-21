

namespace Yanyitec.Unitest
{
    using System;
    using System.Reflection;
    using System.Linq;
    using System.Linq.Expressions;
    using Yanyitec.Runtime;
    using System.Reflection.Emit;
    using Testing;

    public class Program
    {
        public static void Main(string[] args)
        {
            //Expression<Func<string, int&, bool>> expr;
            
            //var paras = method.GetParameters();

            Assert.ThrowException = true;
            var test = new Testing.TestArtifactInfo(new Artifact(typeof(Program).GetTypeInfo().Assembly));

            test.TestMethods("Inject%");
        }

        public class JObj
        {
            public int Id { get; set; }
        }

        public void Parse(string name, string value)
        {
            var result = new JObj();
            if (name == "Id")
            {
                int Id = 0;
                if (int.TryParse(value, out Id))
                {
                    result.Id = Id;
                }
            }
        }
        //static MethodInfo TryParseMethodInfo = typeof(int).GetMethods().First(p=>p.Name=="TryParse");
        //public Action<JObj, string,string> Gen() {

        //    var IdExpr = Expression.Parameter(typeof(int),"Id");
        //    var inputExpr = Expression.Parameter(typeof(string),"value");
        //    var callExpr = Expression.Call( TryParseMethodInfo,inputExpr,);
        //    return null;
        //}
    }
}
