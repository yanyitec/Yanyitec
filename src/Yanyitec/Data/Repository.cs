using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public class Repository<TEntity>
    {
        public Repository(DbCommandFactory<TEntity> cmdFactory)
        {
            this.DbCommandFactory = cmdFactory;
        }
        public DbCommandFactory<TEntity> DbCommandFactory { get;private set; }

        public IDbConnection DbConnection { get; set; }

        public bool Add(TEntity entity,string fields=null) {
            IDbCommand cmd = DbConnection.CreateCommand();
            this.DbCommandFactory.BuildInsert(cmd, entity, fields);
            return cmd.ExecuteNonQuery()==1;
            //this.CommandFactory.b 
        }

        
    }
}
