using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class JDate :JToken
    {
        public JDate(DateTime value) : base(ValueType.DateTime) { this.Value = value; }

        public DateTime Value { get; private set; }

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
            sb.Append("\"").Append(this.Value.ToString("yyyy-MM-dd") + "T" + this.Value.ToString("HH:mm:ss")).Append("\"");
        }
        public override string ToJson()
        {
            return "\"" + this.Value.ToString("yyyy-MM-dd") + "T" + this.Value.ToString("HH:mm:ss") + "\"";
        }

        
    }
}
