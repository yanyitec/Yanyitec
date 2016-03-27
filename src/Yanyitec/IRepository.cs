using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Yanyitec
{
    public interface IRepository<TEntity,TPrimary>
    {
        TEntity GetById(TPrimary id);

        CriteriaResult<TEntity> List(Criteria<TEntity> criteria);

        bool Add(TEntity entity);

        bool Remove(TPrimary id);

        int Delete(Criteria<TEntity> criteria);

        bool Save(TEntity entity);

        int Update(string members, TEntity value, Criteria<TEntity> criteria);
    }
}
