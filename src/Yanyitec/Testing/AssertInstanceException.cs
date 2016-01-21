using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    public class AssertInstanceException : AssertException
    {
        public AssertInstanceException(object obj, Type type) {
            this.Instance = obj;
            this.ExpectType = type;
            
        }

        public Object Instance { get; private set; }

        public Type ExpectType { get; private set; }

        public override string ToString()
        {
            var msg = "Expect type is [" + this.ExpectType.FullName + "],but actual instance";
            if (Instance == null) msg += " is null";
            else msg += "'s type is [" + this.Instance.GetType().FullName + "]";
            return msg;

        }
    }
}
