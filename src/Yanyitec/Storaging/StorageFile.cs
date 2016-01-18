using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public class StorageFile : StorageItem
    {
        public StorageFile(string abstractFilename)
            : this(new FileInfo(abstractFilename),null,null) {
            
            
        }

        public StorageFile(FileInfo info)
            : this(info, null, null)
        {


        }

        protected internal StorageFile(FileInfo info,StorageDirectory parent, Storage root)
            :base(StorageTypes.File , info, parent,root)
        {  
        }
        

        public Stream GetStream(bool createIfNotExisted = true) {
            return (this.FileSystemInfo as FileInfo).Open(createIfNotExisted?FileMode.OpenOrCreate: FileMode.Open);
        }

        public bool CopyTo(IFile target) {
            var srcStream = this.GetStream();
            if (srcStream == null) return false;
            using (srcStream) {
                using (var targetStream = target.GetStream(true))
                {
                    const int bufferSize = 1024 * 2;
                    byte[] buffer = new byte[bufferSize];
                    int readCount = 0;
                    while ((readCount = srcStream.Read(buffer, 0, bufferSize)) > 0) {
                        targetStream.Write(buffer, 0, readCount);
                    }
                }
            }
            return true;
        }

        public async Task<bool> CopyToAsync(IFile target)
        {
            var srcStream = this.GetStream();
            if (srcStream == null) return false;
            using (srcStream)
            {
                using (var targetStream = target.GetStream(true))
                {
                    const int bufferSize = 1024 * 2;
                    byte[] buffer = new byte[bufferSize];
                    int readCount = 0;
                    while ((readCount =await srcStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        await targetStream.WriteAsync(buffer, 0, readCount);
                    }
                }
            }
            return true;
        }

    }
}
