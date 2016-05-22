using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Compilation;

namespace Yanyitec.Data
{
    public class RepositoryFactory
    {
        public DatasetInfo DatasetInfo { get; set; }
        public ICompiler Compiler { get; set; }

        SortedDictionary<int, object> _repositories;

        System.Threading.ReaderWriterLockSlim _locker = new System.Threading.ReaderWriterLockSlim();

        IRepository<TEntity, TPrimary> GetOrCreateRepository<TEntity, TPrimary>(Type entityType) {
            object result = null;
            var typeid = entityType.GetHashCode();
            _locker.EnterUpgradeableReadLock();
            try {
                if (_repositories.TryGetValue(typeid, out result)) return result as IRepository<TEntity, TPrimary>;
                var entityInfo = this.DatasetInfo.GetEntityInfo(typeid);
                if (entityInfo == null) throw new ArgumentException(entityType.FullName + " is not registered in dataset.");
                _locker.EnterWriteLock();
                try {
                    if (_repositories.TryGetValue(typeid, out result)) return result as IRepository<TEntity, TPrimary>;
                    result = GenRepository(entityInfo);
                    _repositories.Add(typeid,result);
                }
                finally {
                    _locker.ExitWriteLock();
                }
            } finally {
                _locker.ExitUpgradeableReadLock();
            }
            return result as IRepository<TEntity, TPrimary>;
        }
        object GenRepository(EntityInfo info) {
            return null;
        }
    }
}
