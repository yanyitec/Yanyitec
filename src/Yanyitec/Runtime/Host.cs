using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Storaging;

namespace Yanyitec.Runtime
{
    public class Host : Module
    {
        readonly System.Threading.ReaderWriterLockSlim _asyncLocker = new System.Threading.ReaderWriterLockSlim();

        readonly SortedDictionary<string, ModuleInfo> _moduleInfos = new SortedDictionary<string, ModuleInfo>();

        


        IDirectory ModDir { get; set; }

        void InitWatching() {
            //ModDir.WatchingSubdirectories = false;
            //ModDir.OnChange += ModDir_OnChange;
        }

        private void ModDir_OnChange(IStorageItem sender, StorageChangeEventArgs e)
        {
            if (e.ChangeType == StorageChangeTypes.Created)
            {
                InitModuleInfo(e.Item);
            }
            else if (e.ChangeType == StorageChangeTypes.Renamed) {
                ModuleInfo info = null;
                //if (_moduleInfos.TryGetValue(e.OldItem.Name, out info)){
                //    if (info != null)
                //    {
                //        _moduleInfos.Add(e.Item.Name, info);
                //        info.Name = e.Item.Name;
                //    }
                //}
                
            }
        }
        private void InitModuleInfo(IStorageItem item) {
            ModuleInfo info = null;
            if (item.StorageType == StorageTypes.File)
            {
                info = LoadFromFile(item as IFile);
            }
            else if (item.StorageType == StorageTypes.Directory)
            {
                info = LoadFromDir(item as IDirectory);
            }
            if (info != null)
            {
                _moduleInfos.Add(info.Name, info);
                info.Artifact.OnChange += Artifact_OnChange;
            }
        }
        public override void Initialize(IHost host, IArtifact artifact) {
            
            foreach (var item in ModDir.ListItems(false)) {
                InitModuleInfo(item);
            }
        }

        private void Artifact_OnChange(IArtifact sender, ArtifactChangeEventArgs e)
        {
            this.AsyncLocker.EnterWriteLock();
            try
            {
                ModuleInfo targetInfo = null;
                foreach (var info in this._moduleInfos.Values)
                {
                    if (info.Artifact.Equals(sender))
                    {
                        this._moduleInfos.Remove(info.Name);
                        targetInfo = info;
                        break;
                    }
                }
                switch (e.ChangeType) {
                    case ArtifactChangeTypes.Deleted: this._moduleInfos.Remove(targetInfo.Name); break;
                    case ArtifactChangeTypes.Rename:
                        this._moduleInfos.Remove(e.OldName);
                        this._moduleInfos.Add(sender.Name, targetInfo);
                        targetInfo.Name = sender.Name;
                        break;
                    default:
                        targetInfo.Disable(false);
                        targetInfo.Enable(false);
                        break;
                }
                
            }
            finally
            {
                this.AsyncLocker.ExitWriteLock();
            }

            
        }

        ModuleInfo LoadFromFile(IFile file) {
            return null;
        }

        ModuleInfo LoadFromDir(IDirectory dir) {
            return null;
        }

        
        string[] _runableModuleNames;
        public string[] RunableModuleNames {
            get { return _runableModuleNames; }
            set
            {
                if (_runableModuleNames != value)
                {
                    _asyncLocker.EnterWriteLock();
                    try
                    {
                        _runableModuleNames = value;
                        EnableModules();

                    }
                    finally
                    {
                        _asyncLocker.ExitWriteLock();
                    }
                }
            }
        }

        ModuleInfo InternalGetModuleInfo(string name) { return null; }

        ModuleInfo InternalRemoveModuleInfo(string name) { return null; }

        public bool IsDeveloping { get; set; }

        public override bool RunMain(object parameters)
        {
            _asyncLocker.EnterReadLock();
            try
            {
                foreach (var moduleInfo in _moduleInfos.Values)
                {
                    if (moduleInfo.IsActived(false)) {
                        if (moduleInfo.RunMain(parameters) == true)
                        {
                            return true;
                        }
                    }
                    
                }
                return false;
            }
            finally
            {
                _asyncLocker.ExitReadLock();
            }


        }

        void EnableModules() {
            if (this._runableModuleNames != null)
            {
                foreach (var info in this._moduleInfos.Values)
                {
                    if (_runableModuleNames.Contains(info.Name))
                    {
                        if (!info.IsActived(false)) info.Enable(false);
                    }
                    else
                    {
                        if (info.IsActived(false)) info.Disable(false);
                    }
                }
            }
            else {
                foreach (var info in this._moduleInfos.Values)
                {
                    if (!info.IsActived(false)) info.Enable(false);
                }
            }
        }
    }
}
