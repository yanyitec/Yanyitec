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
            this.InternalStorage = root;
            
        }

        readonly object _synchronizingObject = new object();
        protected object SynchronizingObject {
            get { return _synchronizingObject; }
        }
       

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
                var fullname = _fullName= this.FileSystemInfo.FullName.Replace("\\", "/");
                var lastSlash = fullname.LastIndexOf("/");
                if (lastSlash < 0) return this.InternalStorage;
                _parent = new StorageDirectory(new DirectoryInfo(fullname.Substring(0, lastSlash)), null, this.InternalStorage);

            }
            return _parent;
        }
        public IStorageDirectory Parent {
            get {

                if (_parent == null)
                {
                    if (this == this.InternalStorage) return null;
                    lock (this.Storage.SynchronizingObject)
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
                if (_fullName == null )
                {
                    //if (this == this.InternalRoot) return null;
                    lock (SynchronizingObject)
                    {
                        if (_fullName == null)
                        {

                            _fullName = this.FileSystemInfo ==null?string.Empty : ConvertToXnixStyle(this.FileSystemInfo.FullName);
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
            get { return this.FileSystemInfo==null?string.Empty:this.FileSystemInfo.Name; }
        }

        string _relativeName;

        string GetRelativeName() {
            if (_relativeName == null)
            {
                return this._relativeName =  ConvertToXnixStyle(this.FileSystemInfo.FullName).Substring(this.InternalStorage.FullName.Length);


            }
            return this._relativeName;
        }
        /// <summary>
        /// Root = ""
        /// Storage = "/"
        /// 其他的返回 /dir/filename
        /// </summary>
        public string RelativeName
        {
            get
            {
                if (_relativeName == null)
                {
                    if (this == Storaging.Storage.Root) return string.Empty;
                    if (this == this.InternalStorage) return "/";
                    lock (SynchronizingObject)
                    {
                        this.GetRelativeName();
                        
                    }
                }
                return _relativeName;
            }
        }

        protected internal Storage InternalStorage;
       
        public IStorage Storage {
            get { return InternalStorage; }

        }

        public override string ToString()
        {
            return this.FileSystemInfo==null?"%root%":this.FullName;
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
