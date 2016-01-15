using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    public class AssertException : Exception
    {
        public AssertException(string msg) : base(msg) { }

        public readonly static AssertException None = new AssertException("None");

        public readonly static AssertException Success = new AssertException("Success");
    }
}
