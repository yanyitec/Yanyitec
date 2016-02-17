using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yanyitec.Workflow.Definations;
using Yanyitec.Workflow.Infos;

namespace Yanyitec.Workflow
{
    public class Proccess
    {
        readonly ReaderWriterLockSlim _syncObject = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        /// <summary>
        /// 已激活的Execution
        /// </summary>
        System.Collections.Generic.Queue<Execution> _actived = new Queue<Execution>();

        readonly List<Exception> _inactived = new List<Exception>();

        public ProccessInfo Info { get; private set; }

        Guid _finishActivityId;
        /// <summary>
        /// 可重入的读写锁
        /// </summary>
        public System.Threading.ReaderWriterLockSlim SyncObject { get { return this._syncObject; } }

        public DateTime AccessTime { get; internal set; }

        #region Execute
        public bool Execute(DateTime? now) {
            if (now == null) now = DateTime.Now;
            this.SyncObject.EnterUpgradeableReadLock();
            try {
                return this.InternalExecute(now.Value);
            } finally {
                this.SyncObject.ExitUpgradeableReadLock();
            }
        }
        bool InternalExecute(DateTime now) {
            Execution execution = null;
            bool finished = false;
            List<Execution> operated = new List<Execution>();
            Queue<Execution> suspended = new Queue<Execution>();
            List<Guid> actived = new List<Guid>();
            while ((execution = _actived.Dequeue()) != null) {
                #region 试图运行一个 execution
                #region 运行前检查状态
                //1在Proccess挂起期间，修改了执行状态，就不再执行了
                if (execution.Info.ExecutionState.IsInactive()) continue;
                if (execution.Info.ExecutionState == ExecutionStates.Suspended) {
                    #region 挂起状态，就看时间到了没有,没到执行时间就进去挂起队列
                    this.SyncObject.EnterWriteLock();
                    try
                    {
                        if (execution.Info.ResumeTime == null)
                        {
                        
                            execution.Info.ExecutionState = ExecutionStates.Inactive;
                            operated.Add(execution);
                            continue;
                        
                        
                        }
                        else if(execution.Info.ResumeTime.Value>now){
                            suspended.Enqueue(execution);
                            actived.Add(execution.Info.Id);
                            continue;
                        }
                    }
                    finally
                    {
                        this.SyncObject.ExitWriteLock();
                    }
                    #endregion 
                }
                #endregion 

                var result = execution.Execute(now);

                #region 运行后处理运行结果
                this.SyncObject.EnterWriteLock();
                try {
                    if (result.ResumeTime != null)
                    {
                        #region 结果表明该活动正在运行，或者挂起
                        if (result.ResumeTime.Value == DateTime.MinValue)
                        {
                            #region 正在运行,进入挂起队列
                            execution.Info.ExecutionState = ExecutionStates.Running;
                            execution.Info.ResumeTime = null;
                            suspended.Enqueue(execution);
                            actived.Add(execution.Info.Id);
                            operated.Add(execution);
                            continue;
                            #endregion
                        }
                        else
                        {
                            #region 挂起
                            execution.Info.ExecutionState = ExecutionStates.Suspended;
                            //指定了重执行时间，继续进去队列
                            if (result.ResumeTime.Value != DateTime.MaxValue)
                            {
                                execution.Info.ResumeTime = result.ResumeTime;
                                suspended.Enqueue(execution);
                                actived.Add(execution.Info.Id);
                            }
                            else
                            {
                                //没指定重执行时间，无期限挂起，不再进入队列
                                execution.Info.ExecutionState = ExecutionStates.Inactive;
                            }
                            operated.Add(execution);
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region 运行已经有了结果，不再执行，并推入nexts到执行队列中
                        execution.Info.Result = result.Result;
                        execution.Info.ExecutionState = ExecutionStates.Completed;
                        //如果是finishactivity完成了，该过程全部结束
                        if (execution.Info.Id != _finishActivityId)
                        {
                            EnqueueNexts(execution, this._actived, operated);
                        }
                        else
                        {
                            finished = true;
                        }
                        #endregion
                    }
                } finally {
                    this.SyncObject.ExitWriteLock();
                }
                #endregion
                #endregion
            }
            this.SyncObject.EnterWriteLock();
            try {
                _actived = suspended;
                this.Info.Actived = actived;
                this.Info.Finished = finished;
                this.InternalStore(operated);
            } finally {
                this.SyncObject.ExitWriteLock();
            }

            
            return finished;
        }

        void EnqueueNexts(Execution execution,Queue<Execution> actived, List<Execution> operated) {
            var nexts = execution.GetNexts();
            foreach (var id in nexts) {
                var next = this.InternalGetExecutionById(id);
                if (next != null) {
                    next.Info.ExecutionState = ExecutionStates.Actived;
                    actived.Enqueue(next);
                    operated.Add(next);
                }
            }

        }

        Execution InternalGetExecutionById(Guid id) { return null; }
        #endregion

        #region store & restore

        void InternalStore(IList<Execution> operated) { }

        

        void InternalCreate(Guid proccessId, IList<ExecutionDefination> executions) {
            IList<ExecutionInfo> infos = new List<ExecutionInfo>();
            Dictionary<string, Guid> maps = new Dictionary<string, Guid>();
            List<TransactionInfo> trans = new List<TransactionInfo>();
            List<ActivityInfo> activities = new List<ActivityInfo>();
            foreach (var defination in executions) {
                var tranDef = defination as TransactionDefination;
                if (tranDef != null) {
                    var tran = new TransactionInfo(proccessId, tranDef);
                    tran.From = GetOrNewId(tranDef.From,maps);
                    tran.To = GetOrNewId(tranDef.To,maps);
                    trans.Add(tran);
                    //_inactived.Add(new);
                    continue;
                }
                var actDef = defination as ActivityDefination;
                if (actDef != null) {
                    var act = new ActivityInfo(proccessId, actDef);
                }
            }
        }

        static Guid GetOrNewId(string alias, Dictionary<string, Guid> data ) {
            Guid id = Guid.Empty;
            if (data.TryGetValue(alias, out id)) return id;
            id = Guid.NewGuid();data.Add(alias,id);
            return id;
        }

        #endregion
    }
}
