

namespace Yanyitec.Json
{
    using System.Text;
    public class JFunction : JToken
    {
        public JFunction(string code) : base(ValueType.Function) { this.Value = code; }

        public string Value { get; private set; }

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
