using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public class Transaction : Activity
    {
        public Transaction(GroupActivity container) :base(container) {
            
        }

        public Func<Activity,bool> Condition { get; private set; }

        public int FromActivityId { get; private set; }

        public int ToActivityId { get; private set; }

        Activity _fromActivity;
        public Activity FromActivity {
            get
            {
                if (_fromActivity == null)
                {
                    this.SyncObject.EnterWriteLock();
                    try
                    {
                        if (_fromActivity == null)
                        {
                            _fromActivity = Container.InternalGetActivityById(this.FromActivityId);
                        }
                    }
                    finally
                    {
                        this.SyncObject.ExitWriteLock();
                    }
                }
                return _fromActivity;
            }
        }

        Activity _toActivity;
        public Activity ToActivity {
            get {
                if (_toActivity == null) {
                    this.SyncObject.EnterWriteLock();
                    try {
                        if (_toActivity == null)
                        {
                            _toActivity = Container.InternalGetActivityById(this.ToActivityId);
                        }
                    } finally {
                        this.SyncObject.ExitWriteLock();
                    }
                }
                return _toActivity;
            }
        }

        protected internal override ActivityStates InternalExecute()
        {
            var isOk = true;
            if (this.Condition != null)
            {
                isOk = this.Condition(this);
            }
            if (isOk) this.Container.Active(this.ToActivity);
            return ActivityStates.Finished;
        }
    }
}
