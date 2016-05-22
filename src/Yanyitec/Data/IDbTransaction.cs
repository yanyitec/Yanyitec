using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public interface IDbTransaction
    {
        IDbConnection Connection { get; }

        void Commit();

        void Rollback();

        
    }
}
