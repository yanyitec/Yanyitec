using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    public class AssertUnhandledException : AssertException
    {
        public AssertUnhandledException(Exception exception) {
            this.UnhandledException = exception;
        }

        public Exception UnhandledException { get; private set; }

        public override string AssertResult
        {
            get
            {
                return "Unexpect exception["+this.UnhandledException.GetType().FullName+"] is thrown : " + this.UnhandledException.Message;
            }
        }
    }
}
