using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class JUnknown :JString
    {
        public JUnknown(string value = null) : base(ValueType.Unknown, value) { }
    }
}
