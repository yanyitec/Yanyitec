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
        

        public IRWLocker AsyncLocker { get; set; }

        SortedDictionary<string, IModule> Modules { get; set; }

        public void Init() {
            //this.SharePath.OnChange += Storage_OnChange;
        }

        private void Storage_OnChange(IStorageItem sender, StorageChangeEventArgs e)
        {
            try
            {
                AsyncLocker.EnterReadLock();
                foreach (var pair in Modules)
                {
                    //if (pair.Value.Artifact.IsDependon(e.OldFullName ?? e.Item.FullName)) {
                    //    AsyncLocker.UpgradeToWriteLock();
                    //    pair.Value.Reflush(false);
                    //}
                }
            }
            finally
            {
                AsyncLocker.ExitLock();
            }
            
        }

        public async Task<bool> HandleRequestAsync(object rawContext) {
            try {
                AsyncLocker.EnterReadLock();
                foreach (var pair in Modules) {
                    var result = await pair.Value.HandleRequestAsync(rawContext);
                    if (result) return true;
                }
            } finally {
                AsyncLocker.ExitReadLock();
            }
            return false;
        }

        public IModule GetModule(string name) {
            try
            {
                AsyncLocker.EnterReadLock();
                IModule result = null;
                this.Modules.TryGetValue(name, out result);
                return result;

            }
            finally
            {
                AsyncLocker.ExitReadLock();
            }
        }

        IModule LoadModule(string nameOrPath) {
            throw new NotImplementedException();
        }

        public bool RegisterModule(string nameOrPath) {
            try
            {
                AsyncLocker.EnterWriteLock();
                
                if (this.Modules.ContainsKey(nameOrPath)) return false;
                var module = this.LoadModule(nameOrPath);
                this.Modules.Add(nameOrPath , module);
                return true;

            }
            finally
            {
                AsyncLocker.ExitWriteLock();
            }
        }

        public bool UnregisterModule(string nameOrPath) {
            try
            {
                AsyncLocker.EnterWriteLock();
                return this.Modules.Remove(nameOrPath);
            }
            finally
            {
                AsyncLocker.ExitWriteLock();
            }
        }
    }
}
