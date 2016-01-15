using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class JUndefined : JToken
    {
        public JUndefined() : base(ValueType.Undefined) { }

        public override object ValueOf()
        {
            return null;
        }

        public override JToken this[string key]
        {
            get
            {
                throw new InvalidOperationException();
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        public override JToken this[int index]
        {
            get
            {
                throw new InvalidOperationException();
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        public readonly static JUndefined Default = new JUndefined();

        public override void ToJson(StringBuilder sb)
        {
            sb.Append( "undefined");
        }
    }
}
