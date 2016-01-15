using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class JInt : JToken
    {
       

        public JInt(int value) : base(ValueType.Int) { this.Value = value; }

        public int Value { get; private set; }

        public override object ValueOf()
        {
            return this.Value;
        }
        public override JToken this[string key]
        {
            get
            {
                return JUndefined.Default;
            }

            set
            {
                
            }
        }

        public override JToken this[int index]
        {
            get
            {
                return JUndefined.Default;
            }

            set
            {
                
            }
        }

        public override string ToJson()
        {
            return this.Value.ToString();
        }

        public override void ToJson(StringBuilder sb)
        {
            sb.Append(this.Value);
        }

    }
}
