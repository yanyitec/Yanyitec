using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public interface IDbConnection : IDisposable
    {
        void Open();
        void Close();
        bool IsInTransaction { get; }

        IDbCommand CreateCommand();
        IDbTransaction BeginTrans();
    }
}
