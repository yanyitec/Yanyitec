using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class ObjectBuilder : IDisposable
    {
        public ObjectBuilder(System.IO.TextWriter output,string tab=null) {
            _output = output;
            _tab = tab;
            if (_tab != null) { _output.Write(_tab); _tab += "\t"; }
            _output.Write('{');
            if (_tab != null) _output.Write("\r\n");
        }

        System.IO.TextWriter _output;
        bool _hasMember;
        string _tab;

        public string ToJson() { return _output.ToString(); }

        public void Dispose()
        {
            if (_tab != null) {  _output.Write("\r\n"); _output.Write(_tab.Substring(0, _tab.Length - 1)); }
            _output.Write("}");
            if (_tab != null) _output.Write("/r/n");
        }

        public ObjectBuilder Member(string name, string value) {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
            _output.Write("\"");
            _output.Write(JString.ReplaceSpecialChar(name));
            _output.Write("\"");
            _output.Write(":");
            if (value == null) _output.Write("undefined");
            else
            {
                _output.Write("\"");
                _output.Write(JString.ReplaceSpecialChar(value));
                _output.Write("\"");
            }
            if (_tab != null) _output.Write("\r\n");
            return this;
        }

        public ObjectBuilder Member(string name, Guid id) {
            return Member(name, id.ToString());
        }

        public ObjectBuilder Member(string name, bool value)
        {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
            _output.Write("\"");
            _output.Write(JString.ReplaceSpecialChar(name));
            _output.Write("\"");
            _output.Write(":");
            _output.Write(value?"true":"false");
            if (_tab != null) _output.Write("\r\n");
            return this;
        }

        public ObjectBuilder Member(string name, int value)
        {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
            _output.Write("\"");
            _output.Write(JString.ReplaceSpecialChar(name));
            _output.Write("\"");
            _output.Write(":");
            _output.Write(value);
            if (_tab != null) _output.Write("\r\n");
            return this;
        }

        public ObjectBuilder Member(string name, double value)
        {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
            _output.Write("\"");
            _output.Write(JString.ReplaceSpecialChar(name));
            _output.Write("\"");
            _output.Write(":");
            _output.Write(value);
            if (_tab != null) _output.Write("\r\n");
            return this;
        }

        public ObjectBuilder Member(string name, DateTime value)
        {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
            _output.Write("\"");
            _output.Write(JString.ReplaceSpecialChar(name));
            _output.Write("\"");
            _output.Write(":");
            _output.Write("\"");
            _output.Write(value.ToString("yyyy-MM-ddThh:mm:ss"));
            _output.Write("\"");
            if (_tab != null) _output.Write("\r\n");
            return this;
        }

        public ObjectBuilder ObjectMember(string name) {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
            _output.Write("\"");
            _output.Write(JString.ReplaceSpecialChar(name));
            _output.Write("\"");
            _output.Write(":");
            if (_tab != null) _output.Write("\r\n");
            return new ObjectBuilder(_output, _tab);
        }

        public ArrayBuilder ArrayMember(string name,bool format=false)
        {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
            _output.Write("\"");
            _output.Write(JString.ReplaceSpecialChar(name));
            _output.Write("\"");
            _output.Write(":\r\n");
            return new ArrayBuilder(_output, format?_tab:null);
        }

    }
}
