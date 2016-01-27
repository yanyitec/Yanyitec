using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Utils
{
    class TextCryptor 
    {

        int _rxor;
        int _leftCount;
        char[] _passwords;
        int _pswdAt;
        int _xorn;
        Random _rnd;
        string _result;

        public TextCryptor(string pswd)
        {
            _rnd = new Random();
            _passwords = pswd.ToCharArray();
            _result = string.Empty;
            _xorn = _passwords[_pswdAt];
        }

        

        const int IntSize = 4;
        static byte[] ZeroBytes = new byte[] { 0, 0, 0, 0 };

        public string Crypt(byte[] buffer)
        {

            for (var bufferAt = 0; bufferAt < buffer.Length; bufferAt += 4)
            {
                if (_leftCount == 0)
                {
                    this._leftCount = _rxor = _rnd.Next(2, 8);
                    this.Output(this._leftCount);
                }
                if (_pswdAt == this._passwords.Length)
                {
                    _xorn = this._passwords[_pswdAt = 0];
                }
                int oper = BitConverter.ToInt32(buffer, bufferAt);
                int xored = oper ^ _xorn ^ _rxor;
                this._leftCount--; this._pswdAt++;
                this.Output(xored);
            }

            return _result;
        }
        //static char[] SeedChars = new Char[] { '|', '+', '-', '=', '!' };
        void Output(int n)
        {
            var n1 = n % byte.MaxValue;
            n = n / byte.MaxValue;
            var n2 = n % byte.MaxValue;
            n = n / byte.MaxValue;
            var n3 = n % byte.MaxValue;
            var n4 = n / byte.MaxValue;
            PutChar(n1); PutChar(n2); PutChar(n3); PutChar(n4);
            //26 + 10 =36

        }
        void PutChar(int n) {
            if (n <= 26) _result += (char)('a' + n);
            else if (n <= 36) _result += (char)('0' + n -27);
            var first = n / 36;
            var last = n % 36;
            _result += 'A' + first;
            if (last <= 26) _result += (char)('a' + n);
            else if (last <= 36) _result += (char)('0' + n-27);
        }
    }
}
