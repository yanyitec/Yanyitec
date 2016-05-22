using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public class SelectInfo<TEntity>
    {
        public string Fields { get;  set; }

        public string TableAlias { get;  set; }

        public string Fieldnames { get;  set; }

        public Func<IDbReader, int,TEntity> Fill { get;  set; }

        public int FieldCount { get;  set; }
    }
}
