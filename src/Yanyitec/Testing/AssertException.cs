using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    public class AssertException : Exception
    {
        public AssertException(string msg=null) : base(msg) { }

        public virtual string AssertResult
        {
            get{ return base.Message; }     
        }

        public override string ToString()
        {
            return "Assert:" + this.AssertResult;
        }
        public readonly static AssertException None = new AssertException("None");

        public readonly static AssertException Success = new AssertException("Success");
    }
}
