﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class JObject : JToken,IEnumerable<KeyValuePair<string,JToken>>
    {
        public JObject() : base(ValueType.Object) {
            this.Value = new Dictionary<string, JToken>();
        }

        public IDictionary<string, JToken> Value { get; private set; }

        public override object ValueOf()
        {
            return Value;
        }

        public override JToken this[int index]
        {
            get
            {
                var data = this.Value as Dictionary<string,JToken>;
                JToken result = null;
                if (data.TryGetValue(index.ToString(), out result)) { return result; }
                return JUndefined.Default;
            }

            set
            {
                var data = this.Value as Dictionary<string, JToken>;
                data[index.ToString()] = value;
            }
        }

        public override JToken this[string key]
        {
            get
            {
                var data = this.Value as Dictionary<string, JToken>;
                JToken result = null;
                if (data.TryGetValue(key, out result)) { return result; }
                return null;
            }

            set
            {
                var data = this.Value as Dictionary<string, JToken>;
                data[key] = value;
            }
        }

        public void SetNull(string name) {
            this.Value[name] = new JNull();
        }

        public void SetUndefined(string name) {
            this.Value[name] = JUndefined.Default;
        }

        public override void ToJson(StringBuilder sb)
        {
            sb.Append("{");
            var first = true;
            foreach (var pair in this.Value as Dictionary<string, JToken>) {
                if (first) { first = false; } else { sb.Append(','); }
                sb.Append('"').Append(JString.ReplaceSpecialChar(pair.Key)).Append('"').Append(':');
                pair.Value.ToJson(sb);
                
            }
            sb.Append("}");
        }

        public IEnumerator<KeyValuePair<string, JToken>> GetEnumerator()
        {
            return this.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Value.GetEnumerator();
        }
    }
}
