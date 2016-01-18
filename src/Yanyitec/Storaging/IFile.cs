using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    using System.IO;
    public interface IFile : IStorageItem
    {
        void Delete();

        Stream GetStream(bool createIfNotExisted = true);

        bool CopyTo(IFile target);
        Task<bool> CopyToAsync(IFile target);
    }
}
