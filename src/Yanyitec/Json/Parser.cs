




namespace Yanyitec.Json
{
    using System;
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
                JToken value = ParseValueString(valueString);
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
        static System.Text.RegularExpressions.Regex JsonDateRegex = new System.Text.RegularExpressions.Regex("^(\\d{4})\\-(10|11|12|0?\\d)\\-([012]\\d|30|31)T([01]\\d|2[0-3]):([0-5]?\\d):([0-5]?\\d)$", System.Text.RegularExpressions.RegexOptions.Compiled);
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
            if (strValue == null)
                return new JUnknown(valueString);
            var dateMatch = JsonDateRegex.Match(strValue);
            if (dateMatch != null && dateMatch.Success)
            {
                var dateTime = new DateTime(
                    int.Parse(dateMatch.Groups[1].Value)
                    , int.Parse(dateMatch.Groups[2].Value)
                    , int.Parse(dateMatch.Groups[3].Value)
                    , int.Parse(dateMatch.Groups[4].Value)
                    , int.Parse(dateMatch.Groups[5].Value)
                    , int.Parse(dateMatch.Groups[6].Value)
                );
                return new JDate(dateTime);
            }


           return new JString(strValue);


        }
    
    }
}
