using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class JBoolean : JToken
    {
        public JBoolean(bool value) : base(ValueType.Boolean) { this.Value = value; }

        public bool Value { get; private set; }

        public override object ValueOf()
        {
            return this.Value;
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

        public override void ToJson(StringBuilder sb)
        {
            sb.Append( (bool)this.Value ? "true" : "false");
        }
        public override string ToJson()
        {
            return (bool)this.Value ? "true" : "false";
        }

        public readonly static JBoolean True = new JBoolean(true);
        public readonly static JBoolean False = new JBoolean(false);
    }
}
