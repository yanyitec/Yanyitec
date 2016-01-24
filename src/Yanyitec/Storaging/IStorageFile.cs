using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    using System.IO;
    public interface IStorageFile : IStorageItem
    {
        void Delete();

        Stream GetStream(bool createIfNotExisted = true);

        string GetText(System.Text.Encoding encoding = null);

        Task<string> GetTextAsync(System.Text.Encoding encoding = null);

        byte[] GetBytes();

        Task<byte[]> GetBytesAsync();

        bool CopyTo(IStorageFile target);
        Task<bool> CopyToAsync(IStorageFile target);
    }
}
