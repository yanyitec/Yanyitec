using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class ArrayBuilder : IDisposable
    {
        public ArrayBuilder(System.IO.TextWriter output, string tab = null)
        {
            _output = output;
            //if (_tab != null) {  _tab += "\t"; }
            _output.Write('[');
        }

        System.IO.TextWriter _output;
        bool _hasMember;
        string _tab;

        public string ToJson() { return _output.ToString(); }

        public void Dispose()
        {
            if (_tab != null) { _output.Write(_tab.Substring(0, _tab.Length - 1)); }
            _output.Write(']');
        }

        public ArrayBuilder Add(string value)
        {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;

            if (value == null) _output.Write("undefined");
            else
            {
                _output.Write("\"");
                _output.Write(JString.ReplaceSpecialChar(value));
                _output.Write("\"");
            }
            //if (_tab != null) _output.Write("\r\n");
            return this;
        }

        public ArrayBuilder Add( Guid id)
        {
            return Add(id.ToString());
        }

        public ArrayBuilder Add(bool value)
        {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
            _output.Write(value ? "true" : "false");
            if (_tab != null) _output.Write("\r\n");
            return this;
        }

        public ArrayBuilder Add(string name, int value)
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

        public ArrayBuilder Member(string name, double value)
        {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
           
            _output.Write(value);
            if (_tab != null) _output.Write("\r\n");
            return this;
        }

        public ArrayBuilder Add(DateTime value)
        {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
            
            _output.Write("\"");
            _output.Write(value.ToString("yyyy-MM-ddThh:mm:ss"));
            _output.Write("\"");
            if (_tab != null) _output.Write("\r\n");
            return this;
        }

        public ObjectBuilder AddObject()
        {
            if (_tab != null) _output.Write(_tab);
            if (_hasMember) _output.Write(",");
            else _hasMember = true;
            
            return new ObjectBuilder(_output, _tab);
        }



    }
}
