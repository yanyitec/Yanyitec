using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Utils
{
    public class TextDecryptor
    {
        int _rxor;
        int? _leftCount;
        char[] _passwords;
        int _pswdAt;
        int _xorn;
        Random _rnd;
        string _result;

        public TextDecryptor(string pswd)
        {
            _passwords = pswd.ToCharArray();
            _result = string.Empty;
            _xorn = _passwords[_pswdAt];
        }



        const int IntSize = 4;

        string _input;
        int _charAt;

        public byte[] Decrypt(string input) {
            using (MemoryStream stream = new MemoryStream()) {
                for (var charAt = 0; charAt < input.Length; charAt++) {
                    if (this._leftCount == null || this._leftCount == 0) {
                        this._leftCount = this._rxor = GetN();
                    }
                    if (_pswdAt == this._passwords.Length)
                    {
                        _xorn = this._passwords[_pswdAt = 0];
                    }
                    int xored = GetN();
                    int oper = xored ^ this._rxor ^ _xorn;
                    var bs = BitConverter.GetBytes(oper);
                    stream.Write(bs,0,bs.Length);
                }
                return stream.ToArray();
            }
        }

        public int GetN() {
            var ch = this._input[this._charAt];
            //'a'=0,b=1
            if (ch >= 'a' && ch <= 'z') { this._charAt++; return (int)(ch - 'a'); }
            //27='0',28='1'
            if (ch >= '0' && ch <= '9') { this._charAt++;return (int)( ch-'0' + 27); }

            if (ch >= 'A' && ch <= 'Z') {
                int nh = (ch - 'A') * 36;
                this._charAt++;
                ch = this._input[this._charAt];
                if (ch >= 'a' && ch <= 'z') { this._charAt++; return nh + (int)(ch - 'a'); }
                //27='0',28='1'
                if (ch >= '0' && ch <= '9') { this._charAt++; return nh + (int)(ch - '0' + 27); }
            }
            throw new ArgumentException("");
        }
    }
}
