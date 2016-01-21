using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    public class AssertEqualException : AssertException
    {
        public AssertEqualException(object expect, object actual) {
            this.ExpectValue = expect;
            this.ActualValue = actual;
        }

        public object ExpectValue { get; private set; }

        public object ActualValue { get; private set; }

        public override string AssertResult
        {
            get
            {
                var msg = "Expect value is ";
                if (ExpectValue == null) msg += "null";
                else msg += "{" + ExpectValue.ToString() + "}";
                msg += ",but actual value is ";
                if (ActualValue == null) msg += "null";
                else msg += "{" + ActualValue.ToString() + "}";
                return msg;
            }
        }
        
    }
}
