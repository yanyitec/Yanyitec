using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public class Storage : StorageDirectory,IStorage
    {
        public Storage(string path):base(new DirectoryInfo(path),null, Storaging.Storage.Root) {
            
            if (!this.FileSystemInfo.Exists) {
                (this.FileSystemInfo as DirectoryInfo).Create();
            }
            this.StorageType = StorageTypes.Storage;
            //this.InternalStorage = this;
        }

        

        public Storage() : base(null, null, null) {
            this.InternalStorage = this;
            this.StorageType = StorageTypes.Root;
        }

        internal Storage(StorageDirectory dir) : base(dir.FileSystemInfo as DirectoryInfo, null, dir.InternalStorage) {
            this.StorageType = StorageTypes.Storage;
        }

        public static readonly Storage Root = new Storage();

        public override void Delete()
        {
            throw new InvalidOperationException("Can not delete storage.");
        }

        public new object SynchronizingObject {
            get {
                return base.SynchronizingObject;
            }
        }
    }
}
