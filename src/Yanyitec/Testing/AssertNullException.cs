using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    public class AssertNullException : AssertException
    {
        public AssertNullException(bool expectNull) {
            this.ExpectNull = expectNull;

        }
        public bool ExpectNull { get; private set; }

        public override string  AssertResult
        {
            get
            {
                return "Expect " + (this.ExpectNull ? "null" : "no-null") + ",but actual is " + (this.ExpectNull ? "no-null" : "null");
            }
            
        }
    }
}
