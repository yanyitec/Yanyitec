//using Yanyitec.Storaging;
//using System;
//using System.Collections.Generic;
//using Yanyitec.Collections;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;

//namespace Yanyitec.Compilation
//{
//    using Yanyitec.Runtime;
//    public abstract class DynamicMetadataReference : IDynamicMetadataReference
//    {
//        public DynamicMetadataReference(IStorage storage) {
//            this.Storage = storage;
//        }

//        public DynamicMetadataReference(string location) {
//            this.Storage = new Storage(location);
//        }

//        protected readonly object _asyncLocker = new object();
//        public IStorage Storage
//        {
//            get; private set;
//        }


//        public event Action<DynamicChangeTypes> OnChange;

//        IArtifact _artifact;

//        public abstract IArtifact Artifact {
//            get;
//        }

//        public bool Equals(IMetadataReference other)
//        {
//            if (other == null) return false;
//            var otherDynamic = other as IDynamicMetadataReference;
//            if (otherDynamic == null) return false;
//            return otherDynamic.Storage.Equals(this.Storage);
//        }

        

//        protected abstract void AddOrUpdateSyntaxTree(string name);

//        protected abstract void RemoveSyntaxTree(string name);

//        SortedDictionary<string, IFile> _files;
//        protected SortedDictionary<string, IFile> InnerFiles {
//            get
//            {
//                if (_files == null)
//                {
//                    lock (this._asyncLocker)
//                    {
//                        if (_files == null)
//                        {
//                            var files = this.Storage.ListItems(true, StorageTypes.File);
//                            var sorted = new SortedDictionary<string, IFile>();
//                            foreach (var item in files)
//                            {
//                                sorted.Add(item.RelativeName, item as IFile);
//                            }
//                            this._files = sorted;
//                            //Storage.OnChange += Storage_OnChange;
//                        }
//                    }
//                }

//                return _files;
//            }
//        }
//        public IReadOnlyDictionary<string, IFile> Files {
//            get { return _files.AsReadonly(); }
//        }

//        private void Storage_OnChange(IStorageItem sender, StorageChangeEventArgs e)
//        {
//            bool isCsChanged = true;
//            lock (this._asyncLocker)
//            {
//                #region check is code changed
                
//                if (e.Item.StorageType.IsFile() && e.Item.FullName.EndsWith(".cs"))
//                {
//                    _artifact = null;
//                }
//                else
//                {
//                    isCsChanged = false;
//                }
//                #endregion

//                #region update ref data

//                switch (e.ChangeType)
//                {

//                    case StorageChangeTypes.Created:
//                        this.InnerFiles.Add(e.Item.RelativeName, e.Item as IFile);
//                        if (isCsChanged) AddOrUpdateSyntaxTree(e.Item.RelativeName);
//                        break;
//                    case StorageChangeTypes.Deleted:
//                        InnerFiles.Remove(e.Item.RelativeName);
//                        if (isCsChanged) RemoveSyntaxTree(e.Item.RelativeName);
//                        break;
//                    case StorageChangeTypes.Updated:
//                        if (isCsChanged) AddOrUpdateSyntaxTree(e.Item.RelativeName);
//                        break;
//                    case StorageChangeTypes.Renamed:
//                        InnerFiles.Remove(e.OldItem.RelativeName);
//                        InnerFiles.Add(e.Item.RelativeName, e.Item as IFile);
//                        if (isCsChanged)
//                        {
//                            RemoveSyntaxTree(e.Item.RelativeName);
//                            AddOrUpdateSyntaxTree(e.OldItem.RelativeName);
//                        }

//                        break;
//                }
//                #endregion

//                this.OnChange(isCsChanged ? DynamicChangeTypes.Code : DynamicChangeTypes.File);
//            }
            

//        }
//    }
//}
