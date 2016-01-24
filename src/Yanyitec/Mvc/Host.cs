using Yanyitec.Storaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Mvc
{
    public class Host
    {
        public IStorage SharePath { get; set; }

        public IStorage ModulePath { get; set; }

        public IStorage DataPath { get; set; }
        

        public IRWLocker SynchronizingObject { get; set; }

        SortedDictionary<string, IModule> Modules { get; set; }

        public void Init() {
            //this.SharePath.OnChange += Storage_OnChange;
        }

        private void Storage_OnChange(IStorageItem sender, StorageChangeEventArgs e)
        {
            try
            {
                SynchronizingObject.EnterReadLock();
                foreach (var pair in Modules)
                {
                    //if (pair.Value.Artifact.IsDependon(e.OldFullName ?? e.Item.FullName)) {
                    //    SynchronizingObject.UpgradeToWriteLock();
                    //    pair.Value.Reflush(false);
                    //}
                }
            }
            finally
            {
                SynchronizingObject.ExitLock();
            }
            
        }

        public async Task<bool> HandleRequestAsync(object rawContext) {
            try {
                SynchronizingObject.EnterReadLock();
                foreach (var pair in Modules) {
                    var result = await pair.Value.HandleRequestAsync(rawContext);
                    if (result) return true;
                }
            } finally {
                SynchronizingObject.ExitReadLock();
            }
            return false;
        }

        public IModule GetModule(string name) {
            try
            {
                SynchronizingObject.EnterReadLock();
                IModule result = null;
                this.Modules.TryGetValue(name, out result);
                return result;

            }
            finally
            {
                SynchronizingObject.ExitReadLock();
            }
        }

        IModule LoadModule(string nameOrPath) {
            throw new NotImplementedException();
        }

        public bool RegisterModule(string nameOrPath) {
            try
            {
                SynchronizingObject.EnterWriteLock();
                
                if (this.Modules.ContainsKey(nameOrPath)) return false;
                var module = this.LoadModule(nameOrPath);
                this.Modules.Add(nameOrPath , module);
                return true;

            }
            finally
            {
                SynchronizingObject.ExitWriteLock();
            }
        }

        public bool UnregisterModule(string nameOrPath) {
            try
            {
                SynchronizingObject.EnterWriteLock();
                return this.Modules.Remove(nameOrPath);
            }
            finally
            {
                SynchronizingObject.ExitWriteLock();
            }
        }
    }
}
