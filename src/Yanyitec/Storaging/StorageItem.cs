using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    using System.IO;
    public class StorageItem : IStorageItem
    {
        protected StorageItem(StorageTypes storageType, FileSystemInfo info,StorageDirectory parent,Storage root=null) {
            this.StorageType = storageType;
            this.FileSystemInfo = info;
            
            this._parent = parent;
            this.InternalRoot = root;
            
        }
        protected readonly object AsyncLocker = new object();


       

        public bool IsExisted {
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

        StorageDirectory _parent;

        StorageDirectory GetParent() {
            if (_parent == null)
            {
                var fullname = this.FileSystemInfo.FullName.Replace("\\", "/");
                var lastSlash = _fullName.LastIndexOf("/");
                if (lastSlash < 0) return this.InternalRoot;
                _parent = new StorageDirectory(new DirectoryInfo(fullname.Substring(0, lastSlash)), null, this.InternalRoot);

            }
            return _parent;
        }
        public IStorageDirectory Parent {
            get {

                if (_parent == null)
                {
                    if (this == this.InternalRoot) return null;
                    lock (AsyncLocker)
                    {
                        this.GetParent();
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
                    //if (this == this.InternalRoot) return null;
                    lock (AsyncLocker)
                    {
                        if (_fullName == null)
                        {
                            _fullName = ConvertToXnixStyle(this.FileSystemInfo.FullName);
                        }
                    }
                }
                return _fullName;
            }
        }

        static string ConvertToXnixStyle(string path) {
            return path.Replace(":\\\\", ":/").Replace("://", ":/").Replace("\\", "/");
        }

        public string Name {
            get { return this.FileSystemInfo.Name; }
        }

        string _relativeName;

        string GetRelativeName() {
            if (_relativeName == null)
            {
                return this._relativeName = this.InternalRoot == this ? string.Empty : ConvertToXnixStyle(this.FileSystemInfo.FullName).Substring(this.Root.FullName.Length + 1);


            }
            return this._relativeName;
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
                        this.GetRelativeName();
                        
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
