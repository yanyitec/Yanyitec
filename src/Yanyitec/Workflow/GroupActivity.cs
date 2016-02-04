using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public class GroupActivity : BlockActivity
    {
        protected Queue<Activity> _ActivedActivities = new Queue<Activity>();

        public GroupActivity(GroupActivity container,int startId,int finishId):base(container) {
            this.StartActivityId = startId;
            this.FinishActivityId = finishId;
        }

        protected GroupActivity() : base() {
            //this.StartActivityId = startId;
            //this.EndActivityId = endId;
        }

        

        public int StartActivityId { get; private set; }

        public int FinishActivityId { get; private set; }

        Activity _startActivity;
        public Activity StartActivity
        {
            get
            {
                if (_startActivity == null)
                {
                    this.SyncObject.EnterWriteLock();
                    try
                    {
                        if (_startActivity == null)
                        {
                            _startActivity = Container.InternalGetActivityById(this.StartActivityId);
                        }
                    }
                    finally
                    {
                        this.SyncObject.ExitWriteLock();
                    }
                }
                return _startActivity;
            }
        }

        Activity _finishActivity;
        public Activity FinishActivity
        {
            get
            {
                if (_finishActivity == null)
                {
                    this.SyncObject.EnterWriteLock();
                    try
                    {
                        if (_finishActivity == null)
                        {
                            _finishActivity = Container.InternalGetActivityById(this.FinishActivityId);
                        }
                    }
                    finally
                    {
                        this.SyncObject.ExitWriteLock();
                    }
                }
                return _finishActivity;
            }
        }


        protected new ActivityStates InternalExecute()
        {
            Activity activity = null;
            var nextActives = new Queue<Activity>();
            while ((activity = _ActivedActivities.Dequeue()) != null)
            {
                var result = activity.InternalExecute();
                if (result == ActivityStates.Finished)
                {
                    if (activity.Id == this.FinishActivityId)
                    {
                        return ActivityStates.Finished;
                    }
                    this.ActiveNext(activity);
                }
                else {
                    nextActives.Enqueue(activity);
                }
            }
            this._ActivedActivities = nextActives;
            return ActivityStates.Dealing;
        }

        

        internal Activity InternalGetActivityById(int id)
        {
            
            foreach (var actvitiy in this._Activities) {
                if (id == actvitiy.Id) return actvitiy;
            }
            return null;
        }

        /// <summary>
        /// 把某个Activity加入到执行队列里去
        /// </summary>
        /// <param name="finishedActivity"></param>
        public void Active(Activity activity)
        {
            this.SyncObject.EnterWriteLock();
            try
            {
                InternalActive(activity);
            }
            finally
            {
                this.SyncObject.ExitWriteLock();
            }
        }

        void InternalActive(Activity activity)
        {
            if (activity._State == ActivityStates.Inactive || activity._State == ActivityStates.Finished)
            {
                activity._State = ActivityStates.Actived;
            }
            this._ActivedActivities.Enqueue(activity);
        }

        void ActiveNext(Activity finished)
        {
            this.SyncObject.EnterWriteLock();
            try
            {
                if (finished._State != ActivityStates.Finished) throw new InvalidOperationException("ActiveNext只能从Finished状态出来，而当前状态是" + Enum.GetName(typeof(ActivityStates), finished._State));
                foreach (var activity in this._Activities)
                {
                    var trans = activity as Transaction;
                    if (trans != null)
                    {
                        if (trans.FromActivityId == finished.Id)
                            this.InternalActive(activity);
                    }
                }
            }
            finally
            {
                this.SyncObject.ExitWriteLock();
            }
        }
    }
}
