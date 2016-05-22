using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public class Dataset
    {
        public Dataset(string connectionString) {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }

        public IRepository<TEntity, TPrimary> GetRepository<TEntity, TPrimary>() { return null; }

        protected virtual void Initialize(DatasetDefine define) { }

        public void BeginTrans() { }

        public void Rollback() { }

        public void Commit() { }

        public IList<TEntity> ExecuteSql<TEntity, TPrimary>(string sql) {
            return null;
        }

        public int ExecuteSql(string sql) {
            return 0;
        }

        public TEntity GetById<TEntity, TPrimary>(TPrimary id) {
            return this.GetRepository<TEntity, TPrimary>().GetById(id);
        }

        public CriteriaResult<TEntity> List<TEntity, TPrimary>(Criteria<TEntity> criteria)
        {
            return this.GetRepository<TEntity, TPrimary>().List(criteria);
        }

        public bool Save<TEntity, TPrimary>(TEntity entity)
        {
            return this.GetRepository<TEntity, TPrimary>().Save(entity);
        }

        public bool Add<TEntity, TPrimary>(TEntity entity) {
            return this.GetRepository<TEntity, TPrimary>().Add(entity);
        }

        public bool Remove<TEntity, TPrimary>(TPrimary id) {return this.GetRepository<TEntity, TPrimary>().Remove(id);}

        public int Delete<TEntity, TPrimary>(Criteria<TEntity> crieria)
        {
            return this.GetRepository<TEntity, TPrimary>().Delete(crieria);
        }

        
    }
}
