using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public abstract class JToken
    {
        protected JToken(ValueType type) {
            this.ValueType = type;
            
        }
        public abstract object ValueOf();

        public ValueType ValueType { get; private set; }

        public abstract JToken this[string key] { get; set; }

        public abstract JToken this[int index] { get; set; }

        public virtual string ToJson() { var sb = new StringBuilder();this.ToJson(sb);return sb.ToString(); }

        public virtual void ToJson(StringBuilder sb) { sb.Append(this.ValueOf().ToString()); }

        #region int
        public static implicit operator int? (JToken jToken) {
            if (jToken.ValueType == ValueType.Int) return (jToken as JInt).Value;
            if (jToken.ValueType == ValueType.Float) return (int)(jToken as JFloat).Value;
            var valueOf = jToken.ValueOf();
            if (valueOf != null) {
                var strValue = valueOf.ToString();
                int result = 0;
                if (int.TryParse(strValue, out result)) return result;
                double dresult = 0.0;
                if (double.TryParse(strValue, out dresult)) return (int)dresult;
            }
            return null;
        }
        public static implicit operator JToken(int value) {
            return new JInt(value);
        }

        public static implicit operator JToken(int? value)
        {
            if (value == null) return JUndefined.Default;
            return new JInt(value.Value);
        }
        #endregion

        #region float
        public static implicit operator double? (JToken jToken)
        {
            if (jToken.ValueType == ValueType.Int) return (jToken as JInt).Value;
            if (jToken.ValueType == ValueType.Float) return (jToken as JFloat).Value;
            var valueOf = jToken.ValueOf();
            if (valueOf != null)
            {
                double dresult = 0.0;
                if (double.TryParse(valueOf.ToString(), out dresult)) return dresult;
            }
            return null;
        }
        public static implicit operator decimal? (JToken jToken)
        {
            if (jToken.ValueType == ValueType.Int) return (jToken as JInt).Value;
            if (jToken.ValueType == ValueType.Float) return (decimal)(jToken as JFloat).Value;
            var valueOf = jToken.ValueOf();
            if (valueOf != null)
            {
                decimal dresult =0;
                if (decimal.TryParse(valueOf.ToString(), out dresult)) return dresult;
            }
            return null;
        }
        public static implicit operator JToken(double value)
        {
            return new JFloat(value);
        }

        public static implicit operator JToken(decimal value)
        {
            return new JFloat((double)value);
        }

        public static implicit operator JToken(double? value)
        {
            if (value == null) return JUndefined.Default;
            return new JFloat(value.Value);
        }

        public static implicit operator JToken(decimal? value)
        {
            if (value == null) return JUndefined.Default;
            return new JFloat((double)value.Value);
        }
        #endregion

        #region bool
        public static implicit operator bool (JToken jToken)
        {
            if (jToken.ValueType == ValueType.Boolean) return (jToken as JBoolean).Value;
            if (jToken.ValueType == ValueType.Int) return (jToken as JInt).Value!=0;
            if (jToken.ValueType == ValueType.Float) return (jToken as JFloat).Value!=0;
            var valueOf = jToken.ValueOf();
            return valueOf != null;
        }

        public static implicit operator JToken(bool value)
        {
            return new JBoolean(value);
        }

        public static implicit operator JToken(bool? value)
        {
            if (value == null) return JUndefined.Default;
            return new JBoolean(value.Value);
        }
        #endregion

        #region string
        public static implicit operator string (JToken jToken)
        {
            if (jToken.ValueType == ValueType.Boolean) return ((JBoolean)jToken).Value ? "true" : "false";
            var valueOf = jToken.ValueOf();
            if (valueOf != null) return valueOf.ToString();
            return null;
        }

        public static implicit operator JToken (string value)
        {
            if (value == null) return JUndefined.Default;
            return new JString(value);
        }

        #endregion
    }
}
