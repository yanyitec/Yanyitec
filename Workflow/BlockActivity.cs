using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public abstract class BlockActivity : Activity
    {
        public BlockActivity(GroupActivity container) :base(container) { }

        protected BlockActivity() : base() { }

        public virtual void AddActivity(Activity activity) {
            this.SyncObject.EnterWriteLock();
            try {
                _Activities.Add(activity);
            } finally {
                this.SyncObject.ExitWriteLock();
            }
        }

        public virtual bool RemoveActivity(Activity activity)
        {
            this.SyncObject.EnterWriteLock();
            try
            {
                return _Activities.Remove(activity);
            }
            finally
            {
                this.SyncObject.ExitWriteLock();
            }
        }

        public List<Activity> _Activities;

        public IReadOnlyList<Activity> Activites { get { return _Activities; } }

    }
}
