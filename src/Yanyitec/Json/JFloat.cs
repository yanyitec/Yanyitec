using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class JFloat : JToken
    {
        

        public JFloat(double value) : base(ValueType.Float) { this.Value = value; }

        public double Value { get; private set; }

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

        public override JToken this[int key]
        {
            get
            {
                return JUndefined.Default;
            }

            set
            {

            }
        }

        public override void ToJson(StringBuilder sb)
        {
            sb.Append(this.Value.ToString());
        }

        public override string ToJson()
        {
            return this.Value.ToString();
        }
    }
}
