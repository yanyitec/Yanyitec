using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.ORM
{
    public class DatasetDefine
    {
        public DatasetDefine(DatasetInfo ds) {
            this.DatasetInfo = ds;
        }
        public DatasetInfo DatasetInfo { get; private set; }
        public EntityDefine<TEntity> Entity<TEntity>() {
            return new EntityDefine<TEntity>(this.DatasetInfo);
        }
    }
}
