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

        bool CopyTo(IStorageFile target);
        Task<bool> CopyToAsync(IStorageFile target);
    }
}
