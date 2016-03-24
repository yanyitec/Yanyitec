using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Yanyitec.ORM
{
    public class Dataset
    {
        public Dataset(string connectionString) { }

        public IRepository<TEntity, TPrimary> GetRepository<TEntity, TPrimary>() { return null; }

        protected virtual void Initialize(DatasetDefine define) { }

        public void BeginTrans() { }

        public void Rollback() { }

        public void Commit() { }

        public IList<TEntity> ExecuteSql<TEntity>(string sql) {
            return null;
        }

        public int ExecuteSql(string sql) {
            return 0;
        }

        public TEntity GetById<TEntity, TPrimary>(TPrimary id) {
            return default(TEntity);
        }

        public IList<TEntity> List<TEntity>(Expression<Func<TEntity,bool>> condition=null)
        {
            return null;
        }

        public bool Save<TEntity>(TEntity entity)
        {
            return false;
        }

        public bool Insert<TEntity>(TEntity entity) {
            return false;
        }

        public bool Delete<TEntity, TPrimary>(TPrimary id) {
            return false;
        }
        public bool Delete<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            return false;
        }
    }
}
