using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Workflow.Infos;

namespace Yanyitec.Workflow
{
    public class Execution
    {
        public Execution(Proccess proccess,ExecutionInfo info) {
            this.Info = info;
            this.Proccess = proccess;
        }

        internal protected ExecutionInfo Info { get; private set; }
        public Proccess Proccess { get; private set; }

        public void Store() { }

        
        
        /// <summary>
        /// 与Proccess 共用同步锁
        /// 该锁为可重入的读写锁
        /// </summary>
        public System.Threading.ReaderWriterLockSlim SyncObject { get { return this.Proccess.SyncObject; }  }

        public virtual ExecuteResult Execute(DateTime executeTime) {
            return new ExecuteResult(null);
        }

        protected internal virtual IList<Guid> GetNexts() {
            return this.Info.Nexts;
        }
    }
}
