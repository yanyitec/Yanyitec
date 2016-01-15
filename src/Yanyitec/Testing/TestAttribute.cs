using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    [AttributeUsage( AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    public sealed class TestAttribute : Attribute
    {
        public string Description { get; private set; }

        public TestAttribute(string desciption = "")
        {
            this.Description = desciption;
        }
    }
}
