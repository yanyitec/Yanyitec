




namespace Yanyitec.Json
{
    using System.Collections.Generic;
    public class Parser
    {
        public Parser()
        {
        }



        public JToken Parse(string jsonString, int at = 0)
        {
            var reader = new Reader(jsonString, at);
            JArray targetArr = null;
            JToken result = null;
            JObject targetObj = null;
            var stack = new Stack<JToken>();
            reader.OnObjectStart = (name,colAt,lineAt) =>
            {

                if (result == null)
                {
                    result = targetObj = new JObject();

                }
                else
                {
                    var newTarget = new JObject();
                    if (targetObj != null)
                    {
                        targetObj[name] = newTarget;
                        stack.Push(targetObj);
                    }
                    else if (targetArr != null && name == null)
                    {
                        targetArr.Push(newTarget);
                        stack.Push(targetArr);

                    }
                    targetObj = newTarget;
                    targetArr = null;

                }

            };
            reader.OnArrayStart = (name) =>
            {

                if (result == null)
                {
                    result = targetArr = new JArray();
                }
                else
                {
                    var newTarget = new JArray();
                    if (targetObj != null)
                    {
                        targetObj[name] = newTarget;
                        stack.Push(targetObj);
                    }
                    else if (targetArr != null && name == null)
                    {
                        targetArr.Push(newTarget);
                        stack.Push(targetArr);
                    }
                    targetArr = newTarget;
                    targetObj = null;

                }

            };
            reader.OnObjectEnd = () =>
            {

                if (targetObj != result && targetArr != result)
                {
                    var target = stack.Pop();
                    if ((targetArr = target as JArray) == null)
                    {
                        if ((targetObj = target as JObject) == null)
                        {
                            throw new ParseException("不期望的}", reader.JsonString, reader.At, reader.LineAt,
                                reader.ColumnAt);
                        }
                    }
                    else targetObj = null;
                }

                return true;
            };
            reader.OnArrayEnd = () =>
            {

                if (targetObj != result && targetArr != result)
                {
                    var target = stack.Pop();
                    if ((targetArr = target as JArray) == null)
                    {
                        if ((targetObj = target as JObject) == null)
                        {
                            throw new ParseException("不期望的]", reader.JsonString, reader.At, reader.LineAt,
                                reader.ColumnAt);
                        }
                    }
                    else targetObj = null;
                }
                return true;
            };
            reader.OnFunctionFound = (name, value) =>
            {
                if (targetObj != null) targetObj[name] = new JFunction(value);
                else if (targetArr != null) targetArr.Push(new JFunction(value));
                else if (name == null) result = new JFunction(value);
                else
                {
                    result = targetObj = new JObject();
                    targetObj[name] = new JFunction(value);
                }
                return true;
            };
            reader.OnKeyValueFound = (name, valueString) => {
                JToken value = null;
                if (targetObj != null) targetObj[name] = value;
                else if (targetArr != null) targetArr.Push(value);
                else if (name == null) result = value;
                else
                {
                    result = targetObj = new JObject();
                    targetObj[name] = value;
                }

                return true;
            };
            reader.Read();
            return result;
        }

        static JToken ParseValueString(string valueString) {
            if (valueString == "null") return new JNull();
            if (valueString == "undefined") return JUndefined.Default;
            if (valueString == "true") return JBoolean.True;
            if (valueString == "false") return JBoolean.False;
            var numberInt = 0;
            if (int.TryParse(valueString, out numberInt))
            {
                return new JInt(numberInt);

            }

            var numberDouble = 0.0;

            if (double.TryParse(valueString, out numberDouble))
            {
                return new JFloat(numberDouble);
            }

            string strValue = null;
            if (valueString.StartsWith("\"") && valueString.EndsWith("\""))
            {
                strValue = valueString.Trim('"');

            }
            if (valueString.StartsWith("'") && valueString.EndsWith("'"))
            {
                strValue = valueString.Trim('\'');
            }
            if (strValue != null) return new JString(strValue);
            return new JUnknown(valueString);

        }
    
    }
}
