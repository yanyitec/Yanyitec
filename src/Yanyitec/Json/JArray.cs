﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class JArray : JToken
    {
        public JArray() : base(ValueType.Array) {
            this.Value = new List<JToken>();
        }

        public IList<JToken> Value { get; private set; }

        public override object ValueOf()
        {
            return Value;
        }

        public override JToken this[int index]
        {
            get
            {
                if (index >= this.Value.Count) return JUndefined.Default;
                return this.Value[index];
            }

            set
            {
                if (index < this.Value.Count) {
                    if (index < 0) return;
                    this.Value[index] = value;
                }

                for (int i = this.Value.Count, j = index - this.Value.Count; i < j; i++) {
                    this.Value.Add(JUndefined.Default);
                }
                this.Value.Add(value??JUndefined.Default);
            }
        }

        public override JToken this[string key]
        {
            get
            {
                int index = 0;
                if (int.TryParse(key, out index)) return this[index];
                return JUndefined.Default;
            }

            set
            {
                int index = 0;
                if (int.TryParse(key, out index)) this[index] = value;
            }
        }

        public void Push(JToken token) {
            this.Value.Add(token);
        }

        public override void ToJson(StringBuilder sb)
        {
            sb.Append("[");
            var first = true;
            foreach (var item in this.Value) {
                if (first) { first = false; } else { sb.Append(','); }
            }
            sb.Append("]");
        }

        public override string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            this.ToJson(sb);
            return sb.ToString();
        }
    }
}
