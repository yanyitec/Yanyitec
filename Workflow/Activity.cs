using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public class Activity
    {
        public Activity(GroupActivity container) {
            
            this.Container = container;
            this.SyncObject = this.Container.SyncObject;
        }

        /// <summary>
        /// 仅用于Process 
        /// </summary>
        protected Activity() {
            this.Container = this as GroupActivity;
            this.SyncObject = new System.Threading.ReaderWriterLockSlim( System.Threading.LockRecursionPolicy.SupportsRecursion);
        }

        public Guid Id { get; private set; }

        public GroupActivity Container { get; private set; }

        internal ExecutionStates _ExecutionState;
        /// <summary>
        /// 一个Activity相当于一个ThreadTask
        /// Engine调度用 set ，其他的不应该使用 set
        /// </summary>
        public ExecutionStates ExecutionState { get; internal set; }

        DateTime _ResumeTime;
        public DateTime ResumeTime { get;  internal set; }


        /// <summary>
        /// 与Process共用锁
        /// </summary>
        public System.Threading.ReaderWriterLockSlim SyncObject {
            get; set;
        }

        /// <summary>
        /// 读写该字段需要注意锁，
        /// 该字段受SyncObject锁保护
        /// </summary>
        internal ActivityStates _State;

        public ActivityStates Status
        {
            get
            {
                SyncObject.EnterReadLock();
                try
                {
                    return _State;
                }
                finally
                {
                    SyncObject.ExitReadLock();
                }
            }
            set
            {
                SyncObject.EnterWriteLock();
                try
                {
                    _State = value;
                }
                finally
                {
                    SyncObject.ExitWriteLock();
                }
            }
        }

        public ActivityStates State {
            get {
                SyncObject.EnterReadLock();
                try
                {
                    return _State;
                }
                finally
                {
                    SyncObject.ExitReadLock();
                }
            }
            set
            {
                SyncObject.EnterWriteLock();
                try
                {
                    _State = value;
                }
                finally
                {
                    SyncObject.ExitWriteLock();
                }
            }
        }
        
        /// <summary>
        /// 开始模式，
        /// 如果是自动开始，引擎会自动检测StartConstraint条件
        /// 如果是手工开始，引擎不会自己开始该activity,需要client代码自己调用start函数
        /// </summary>
        public ExecutionModes StartMode { get;protected set; }
        /// <summary>
        /// 开始条件
        /// 只有满足前置条件才能进去
        /// </summary>
        public Func<Activity,bool> StartConstraint { get; protected set; }

        /// <summary>
        /// 结束条件
        /// 只有满足后置条件该Activity才能结束
        /// </summary>
        public Func<Activity,bool> FinishConstraint { get; protected set; }

        /// <summary>
        /// 结束模式，
        /// 如果是自动结束，引擎会自动检测FinishConstraint条件
        /// 如果是手工结束，引擎不会自己结束该activity,需要client代码自己调用finish函数
        /// </summary>
        public ExecutionModes FinishMode { get; protected set; }
        

        /// <summary>
        /// 注意:该函数会在Activity.SyncObject的UpgradableReadLocker保护中运行。
        /// </summary>
        /// <returns></returns>
        public virtual object Deal() {
            return true;
        }

        object _dealResult;

        public object DealResult {
            get {
                SyncObject.EnterReadLock();
                try
                {
                    return _dealResult;
                }
                finally
                {
                    SyncObject.ExitReadLock();
                }
            }
            set {
                SyncObject.EnterWriteLock();
                try
                {
                    _dealResult = value;
                }
                finally
                {
                    SyncObject.ExitWriteLock();
                }
            }
        }

        #region execute
        public ActivityStates Execute() {
            this.SyncObject.EnterUpgradeableReadLock();
            try
            {
                return this.InternalExecute();
            }
            finally
            {
                this.SyncObject.ExitUpgradeableReadLock();
            }
        }
        protected internal virtual ActivityStates InternalExecute()
        {
            var state = this._State;
            switch (state)
            {
                //还没激活，试图激活它
                //case ActivityStates.Inactive:
                //    if (this.StartMode == ExecutionModes.Manual) break;
                //    TryStart(); break;
                //case ActivityStates.Actived:
                //    if (this.StartMode == ExecutionModes.Manual) break;
                //    TryStart(); break;
                //case ActivityStates.WaitingStart:
                //    #region 上次就没满足前置条件，这次再测试前置条件
                //    TryStart(); break;
                //    #endregion
                //case ActivityStates.Started:
                //    ///手动设置成Started
                //    TryDeal(); break;
                //case ActivityStates.Dealing:
                //    //正在处理中，什么都不要做，等待任务完成
                //    break;
                //case ActivityStates.Suspending:
                //    #region 挂起
                //    var dealResult = this._dealResult as DealResult;
                //    if (dealResult != null) {
                //        if (dealResult.RetryTime < DateTime.Now)
                //        {
                //            TryDeal();
                //        }
                        
                //    }
                //    #endregion
                //    break;
                ////还没激活，试图激活它
                //case ActivityStates.Dealed:
                //    if (this.FinishMode == ExecutionModes.Manual) break;
                //    TryFinish(); break;

                //case ActivityStates.WaitingFinish:
                //    #region 上次就没满足后置条件，这次再测试后置条件
                //    TryFinish(); break;
                //    #endregion
                //case ActivityStates.Finished:
                //    ///已经结束过了的，什么都不做
                //    break;
                    
            }
            return _State;
        }


        void TryStart()
        {
            if (this.StartConstraint != null)
            {
                #region 有前置条件
                if (!this.StartConstraint(this))
                {
                    #region 前置条件不满足,转换activity的状态为WaitingStart,等待下次执行
                    this.SyncObject.EnterWriteLock();
                    try {
                        this._State = ActivityStates.WaitingStart;
                    } finally {
                        this.SyncObject.ExitWriteLock();
                    }
                    
                    return;
                    #endregion
                }
                else
                {
                    #region 前置条件已经满足,直接开始Deal过程
                    try
                    {
                        this._State = ActivityStates.Started;
                    }
                    finally
                    {
                        this.SyncObject.ExitWriteLock();
                    }
                    
                    TryDeal();
                    #endregion
                }
                #endregion
            }
            else
            {
                #region 没有前置条件,直接Deal过程
                try
                {
                    this._State = ActivityStates.Started;
                }
                finally
                {
                    this.SyncObject.ExitWriteLock();
                }
                
                TryDeal();
                #endregion
            }
        }

        void TryDeal()
        {
            this.SyncObject.EnterWriteLock();
            try
            {

                if (this._State != ActivityStates.Started && this._State != ActivityStates.Suspending) throw new InvalidOperationException("Deal只能从Started状态出来，而当前状态是" + Enum.GetName(typeof(ActivityStates), this._State));

                this._State = ActivityStates.Dealing;
            }
            finally
            {
                this.SyncObject.ExitWriteLock();
            }
            var result = this.Deal();
            this.SyncObject.EnterWriteLock();
            try
            {
                if (this._State != ActivityStates.Dealing) throw new InvalidOperationException("Dealed只能从Dealing状态出来，而当前状态是" + Enum.GetName(typeof(ActivityStates), this._State));
                this._dealResult = result;
                var dealResult = result as DealResult;
                if (dealResult != null)
                {
                    if (dealResult.RetryTime != DateTime.MinValue)
                    {
                        this._State = ActivityStates.Suspending;
                    }
                    return;
                }
                this._State = ActivityStates.Dealed;
            }
            finally
            {
                this.SyncObject.ExitWriteLock();
            }

            //挂起与等待都在前面return 了，到这里的都是正常完成的Dealed
            TryFinish();

        }
        void TryFinish() {
            if (this.FinishConstraint != null)
            {
                #region 有后置条件
                if (!this.FinishConstraint(this))
                {
                    #region 后置条件不满足,转换activity的状态为WaitingFinish,等待下次执行
                    this.SyncObject.EnterWriteLock();
                    try
                    {
                        this._State = ActivityStates.WaitingFinish;
                    }
                    finally
                    {
                        this.SyncObject.ExitWriteLock();
                    }

                    return;
                    #endregion
                }
                else
                {
                    #region 后置条件已经满足
                    try
                    {
                        this._State = ActivityStates.Finished;
                    }
                    finally
                    {
                        this.SyncObject.ExitWriteLock();
                    }

                    #endregion
                }
                #endregion
            }
            else
            {
                #region 没有后置条件,直接Deal过程
                try
                {
                    this._State = ActivityStates.Finished;
                }
                finally
                {
                    this.SyncObject.ExitWriteLock();
                }

               
                #endregion
            }
        }
        #endregion

    }
}
