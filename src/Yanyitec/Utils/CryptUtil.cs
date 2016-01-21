using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Utils
{
    using System.IO;
    public static class CryptUtil
    {
        public static string QuickCryptToText(byte[] input, string password) {
            var cryptor = new TextCryptor(password);
            return cryptor.Crypt(input);
        }
        public static byte[] QuickDecryptFromText(string input, string password) {
            var decryptor = new TextDecryptor(password);
            return decryptor.Decrypt(input);
        }
    }
}
