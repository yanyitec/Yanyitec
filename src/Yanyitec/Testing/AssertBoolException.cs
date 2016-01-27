using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    public class AssertBoolException : AssertException
    {
        public AssertBoolException(bool expectTrue)
        {
            this.ExpectTrue = expectTrue;

        }
        public bool ExpectTrue { get; private set; }

        public override string AssertResult
        {
            get
            {
                return "Expect " + (this.ExpectTrue ? "true" : "false") + ",but actual is " + (this.ExpectTrue ? "false" : "true");
            }

        }
    }
}
