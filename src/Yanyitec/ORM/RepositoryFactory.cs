using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Compilation;

namespace Yanyitec.ORM
{
    public class RepositoryFactory
    {
        public ICompiler Compiler { get; set; }
        IRepository<TEntity, TPrimary> GetOrCreateRepository<TEntity, TPrimary>(EntityInfo info) {
            return null;
        }
    }
}
