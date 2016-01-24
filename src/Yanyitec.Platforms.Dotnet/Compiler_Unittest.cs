using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yanyitec.Compilation;
using Yanyitec.Testing;

namespace Yanyitec
{
    [Test]
    public class Compiler_Unittest
    {
        [Test("The basic compile a code")]
        public void Simple() {
            var code = @"
    class TestCompiler{
        public int Foo(int k) {return k += 10;}
    }
";
            var compiler = new CSharpCompiler();
            compiler.AddOrReplaceCode("first", code);
            compiler.AddReference(typeof(object));
            var asm = compiler.Compile("foo.dll");
            var type = asm.GetType("TestCompiler");
            var method = type.GetMethod("Foo");
            var result = method.Invoke(Activator.CreateInstance(type), new object[] { 11 });
            Assert.Equal(21,(int)result);
        }
    }
}
