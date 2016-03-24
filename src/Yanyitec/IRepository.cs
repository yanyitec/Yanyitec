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

        IList<TEntity> List(Expression<Func<TEntity, bool>> condition);

        bool Add(TEntity entity);

        bool DeleteById(TPrimary id);

        int Delete(Expression<Func<TEntity, bool>> condition);

        bool Save(TEntity entity);

        int Update(string members, TEntity value, Expression<Func<TEntity, bool>> condition);
    }
}
