using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class JString : JToken
    {
        protected JString(ValueType type, string value) :base(type){ this.Value = value; }
        public JString() : base(ValueType.String) { this.Value = string.Empty; }

        public JString(string value) : base(ValueType.String) { this.Value = value == null ? string.Empty : value; }

        public JString(char value) : base(ValueType.Char) { this.Value = new string(value,1); }

        public string Value { get; private set; }

        public override object ValueOf()
        {
            return this.Value;
        }

        public override JToken this[string key]
        {
            get
            {
                
                var str = this.Value as string;
                if (str == null || str == string.Empty) return JUndefined.Default;
                int index = 0;
                if (int.TryParse(key, out index)) {
                    var ch = str[index]; 
                    return new JString(ch);
                }
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
                var str = this.Value as string;
                if (str == null || str == string.Empty) return JUndefined.Default;
                var ch = str[index];
                return new JString(ch);
            }

            set
            {
                
            }
        }
        public static string ReplaceSpecialChar(string input) {
            return input.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r\n", "\\r\\n").Replace("\r","\\r").Replace("\n","\\n");
        }
        public override void ToJson(StringBuilder sb)
        {
            sb.Append('"').Append(ReplaceSpecialChar(this.Value.ToString())).Append('"');
        }

        public override string ToJson()
        {
            return "\"" + ReplaceSpecialChar(this.Value) + "\"";
        }
    }
}
