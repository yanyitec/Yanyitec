using System;


namespace Yanyitec.Json
{
    public class Reader
    {
        public Action<string,int ,int> OnObjectStart { get; set; }

        public Func<bool> OnObjectEnd { get; set; }

        public Action<string> OnArrayStart { get; set; }

        public Func<bool> OnArrayEnd { get; set; }

        public Func<string ,string, bool> OnFunctionFound { get; set; }
       

        public Func<string,string,bool> OnKeyValueFound { get; set; }

        public Reader(string jsonString, int at = 0, bool endWhileObjectEnd = false)
        {
            this.JsonString = jsonString;
            this.At = at;
            this.EndWhileMeetObjectEnd = endWhileObjectEnd;
        }

        public bool EndWhileMeetObjectEnd { get; private set; }

        public int At { get; private set; }

        public int LineAt { get; private set; }

        public int ColumnAt { get; private set; }

        public string JsonString { get; private set; }



        public int Read()
        {
            var text = JsonString;
            var len = text.Length;

            var lastAt = 0;
            var lastCh = '\0';
            var inString = '\0';
            var inFunction = 0;

            LineAt = 1;
            ColumnAt = 1;
            string name = null;
            //string value = null;
            while (At < len)
            {
                var ch = text[At];
                switch (ch)
                {
                    case '\n': // case \n
                        if (inString != '\0') throw new ParseException("字符串里不能出现换行符", this);
                        LineAt++; ColumnAt = 1;
                        break;
                    case ':':// case ':':
                        if (inString != '\0') break;
                        if (inFunction > 0) break;
                        name = text.Substring(lastAt, At - lastAt).Trim('"', '\'', ' ', '\t', '\r', '\n');
                        lastAt = At + 1;
                        lastCh = ch;
                        break;

                    case ',':// case ','
                        if (inString != '\0') break;
                        if (inFunction > 0) break;
                        var value = text.Substring(lastAt, At - lastAt).Trim();

                        if (lastCh == '}' || lastCh == ']')
                        {//lastCh ===}
                            lastAt = At + 1; lastCh = ch;
                            break;
                        }
                        //function(abc,  这种情况，忽略,号
                        if (value.StartsWith("function("))
                        {
                            break;
                        }
                        //if(name==null) throw  new ParseException("在[,]之前应该有name,但未能分析出name",text,At,LineAt,ColumnAt);

                        if (!this.OnKeyValueFound(name, value))
                        {
                            throw new ParseException("未能正确解析出json值", text, At, LineAt, ColumnAt);
                        }
                        name = null;
                        lastAt = At + 1; lastCh = ch;
                        break;
                    case '{':// case {

                        if (inString != '\0') break;
                        if (inFunction > 0) { inFunction++; break; }
                        var bewteen = text.Substring(lastAt, At - lastAt).Trim();
                        if (bewteen.StartsWith("function(") && bewteen.EndsWith(")"))
                        {
                            inFunction++; break;
                        }
                        #region new object found
                        //if(name==null) throw  new ParseException("在[{]之前应该有name,但未能分析出name",text,at,lineAt,columnAt);
                        if (this.OnObjectStart != null) this.OnObjectStart(name,ColumnAt,LineAt);
                        name = null;
                        lastAt = At + 1; lastCh = ch;

                        break;
                    #endregion
                    case '}'://case }

                        if (inString != '\0') break;
                        if (inFunction > 0)
                        {
                            inFunction--;
                            if (inFunction == 0)
                            {
                                var func = text.Substring(lastAt, At - lastAt + 1);
                                this.OnFunctionFound(name, func.Trim());
                                name = null;
                                lastAt = At + 1; lastCh = ch;
                            }
                            break;
                        }
                        if (!(lastCh == '}' || lastCh == ']'))
                        {
                            if (name == null) throw new ParseException("在[}]之前应该有name,但未能分析出name", text, At, LineAt, ColumnAt);
                            value = text.Substring(lastAt, At - lastAt).Trim();
                            if (!this.OnKeyValueFound(name, value))
                            {
                                throw new ParseException("未能正确解析出json值", text, At, LineAt, ColumnAt);
                            }
                            name = null;
                        }
                        if (this.OnObjectEnd != null) this.OnObjectEnd();

                        lastAt = At + 1; lastCh = ch;
                        if (EndWhileMeetObjectEnd) return lastAt;
                        break;
                    case '[':// case {
                        if (inString != '\0') break;
                        if (inFunction > 0) break;
                        #region new object found
                        if (this.OnArrayStart != null) this.OnArrayStart(name);
                        name = null;
                        lastAt = At + 1; lastCh = ch;

                        break;
                    #endregion
                    case ']'://case }
                        if (inString != '\0') break;
                        if (inFunction > 0) break;
                        if (lastCh != '}' && lastCh != ']')
                        {
                            value = text.Substring(lastAt, At - lastAt).Trim();
                            this.OnKeyValueFound(null,value);
                        }
                        if (this.OnArrayEnd != null) this.OnArrayEnd();
                        lastAt = At + 1; lastCh = ch;
                        if (EndWhileMeetObjectEnd) return lastAt;
                        break;
                    case '"':
                        if (inString == '"')
                        {
                            if (text[At - 1] != '\\')
                            {
                                inString = '\0';
                            }
                        }
                        else if (inString == '\0')
                        {
                            inString = '"';
                        }
                        break;
                    case '\'':
                        if (inString == '\'')
                        {
                            if (text[At - 1] != '\\')
                            {
                                inString = '\0';
                            }

                        }
                        else if (inString == '\0')
                        {
                            inString = '\'';
                        }
                        break;
                }
                At++;
                ColumnAt++;
            }
            if (lastCh == '\0') return len;
            if (name != null)
            {
                var value = text.Substring(lastAt).Trim();
                this.OnKeyValueFound(name, value);
            }
            return len;

        }

        

    }
}
