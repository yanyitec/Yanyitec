using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow
{
    public class Process : GroupActivity
    {
        
        public Process(Guid proccessId,IWorkfowEngine engine) : base() {

        }

        IWorkfowEngine Engine { get; set; }

        readonly SortedDictionary<string, string> _data;
        
        public string this[string name] {
            get {
                this.SyncObject.EnterReadLock();
                try {
                    string value = null;
                    _data.TryGetValue(name, out value);
                    return value;
                } finally {
                    this.SyncObject.ExitReadLock();
                }
            }
            set {
                this.SyncObject.EnterWriteLock();
                try
                {
                    _data[name] = value;
                }
                finally
                {
                    this.SyncObject.ExitWriteLock();
                }
            }
        }
    }
}
