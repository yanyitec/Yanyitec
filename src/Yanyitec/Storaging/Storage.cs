using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public class Storage : Directory,IStorage
    {
        public Storage(string path):base(new DirectoryInfo(path),null, null) {
            this.InternalRoot = this;
        }

        public Storage() : base(null, null, null) {
            this.InternalRoot = this;
        }

        public static readonly Storage System = new Storage();
    }
}
