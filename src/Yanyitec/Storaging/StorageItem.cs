using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    using System.IO;
    public class StorageItem : IStorageItem
    {
        protected StorageItem(StorageTypes storageType, FileSystemInfo info,StorageItem parent,Storage root=null) {
            this.StorageType = storageType;
            this.FileSystemInfo = info;
            
            this._parent = parent;
            this.InternalRoot = root;
            
        }
        protected readonly object AsyncLocker = new object();


        public void Delete() {
            if (this.FileSystemInfo != null) {
                this.FileSystemInfo.Delete();
            }
            throw new InvalidOperationException("Cannot delete the system root.");
        }

        public virtual bool IsExisted {
            get {
                if (this.StorageType == StorageTypes.File)
                {
                    return System.IO.File.Exists(this.FullName);
                }
                else if (this.StorageType.IsDirectory()) {
                    return System.IO.Directory.Exists(this.FullName);
                }
                throw new NotSupportedException();
            }
        }

        IStorageItem _parent;
        public IStorageItem Parent {
            get {

                if (_parent == null)
                {
                    if (this == this.InternalRoot) return null;
                    lock (AsyncLocker)
                    {
                        if (_parent == null)
                        {
                            var fullname = this.FileSystemInfo.FullName.Replace("\\","/");
                            var lastSlash = _fullName.LastIndexOf("/");
                            if (lastSlash < 0) return this.Root;
                            _parent = new Directory(fullname.Substring(0, lastSlash));
                            
                        }
                    }
                }
                
                return _parent;
            }
        }

        public StorageTypes StorageType
        {
            get; protected set;
        }

        public System.IO.FileSystemInfo FileSystemInfo { get; private set; }

        string _fullName;
        public string FullName {
            get {
                if (_fullName == null)
                {
                    if (this == this.InternalRoot) return null;
                    lock (AsyncLocker)
                    {
                        if (_fullName == null)
                        {
                            _fullName = this.FileSystemInfo.FullName.Replace("\\", "/");
                        }
                    }
                }
                return _fullName;
            }
        }

        public string Name {
            get { return this.FileSystemInfo.Name; }
        }

        string _relativeName;

        string GetRelativeName() {
            return this.FileSystemInfo.FullName.Substring(this.Root.FullName.Length);
            
        }
        public string RelativeName
        {
            get
            {
                if (_relativeName == null)
                {
                    if (this == this.InternalRoot) return null;
                    lock (AsyncLocker)
                    {
                        if (_relativeName == null)
                        {
                            if (this._parent != null)
                            {
                                _relativeName = this.GetRelativeName();
                            }
                            
                        }
                    }
                }
                return _relativeName;
            }
        }

        protected Storage InternalRoot;
        public IStorage Root {
            get {
                if (InternalRoot != null) {
                    lock (this.AsyncLocker) {
                        if (InternalRoot == null) {
                            if (this._parent != null)
                            {
                                InternalRoot = this._parent.Root as Storage;
                            }
                            else InternalRoot = Storage.System;
                        }
                    }
                }
                return this.InternalRoot;
            }
        }

        public override string ToString()
        {
            return this.FullName;
        }

        

        public bool Equals(IStorageItem other)
        {
            if (other == null) return false;
            var otherStorageItem = other as StorageItem;
            if (otherStorageItem == null) return false;
            return otherStorageItem == this || otherStorageItem.FullName == this.FullName;
        }
        
        
    }
}
