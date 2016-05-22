using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow
{
    public class Process
    {
        public Process(ProcessDefination data,Engine engine,IRepository rep=null) {
            this._defination = data;
        }
        public Engine Engine { get; private set; }

        ProcessDefination _defination;

        public IRepository Repository { get; private set; }
        /// <summary>

        /// </summary>
        public ProcessDefination Defination { get; set; }
        public Guid CreateActivity(ActivityInfo info) { return Guid.Empty; }
        /// <summary>
        /// 根据定义创建一个活动
        /// </summary>
        /// <param name="definationId"></param>
        /// <returns></returns>
        public Guid CreateActivity(Guid definationId) { return Guid.Empty; }
        /// <summary>
        /// 手动开始一个活动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void StartActivity(Guid id) {  }

        /// <summary>
        /// 手动结束一个活动
        /// </summary>
        /// <param name="id"></param>
        public void FinishActivity(Guid id) { }

        /// <summary>
        /// 直接执行某个活动
        /// </summary>
        /// <param name="id"></param>
        public ActivityStates ExecuteActivity(Guid id)
        {
            this._syncLocker.EnterWriteLock();
            try
            {
                var activity = this.GetOrLoadActivity(id);
                if (activity == null) return ActivityStates.Initial;
                if (activity._ExecutionState.IsInactive()) activity._ExecutionState = ExecutionStates.Actived;
                return this.InteralExecuteActivity(activity, this._activedActivities, DateTime.Now);
            }
            finally
            {
                this._syncLocker.EnterWriteLock();
            }
        }
        /// <summary>
        /// 执行队列,Waited,Suspended.
        /// </summary>
        Queue<Activity> _activedActivities = new Queue<Activity>();
        /// <summary>
        /// 完成或才创建的活动
        /// 基本上就是个缓存，用于减少IO操作
        /// </summary>
        readonly List<Activity> _inactiveActivities = new List<Activity>();
        /// <summary>
        /// 同步锁,支持递归
        /// </summary>
        readonly System.Threading.ReaderWriterLockSlim _syncLocker = new System.Threading.ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion);
        /// <summary>
        /// 活动调度线程
        /// </summary>
        System.Threading.Tasks.Task _schedulingTask;

        bool? _scheduling;

        void InternalStore() {
            this._defination.ActivedActivityIds = new List<Guid>();
            foreach (var actived in _activedActivities) {
                this._defination.ActivedActivityIds.Add(actived.Id);
            }
            this.Repository.SaveProcess(this._defination);
        }

        void Active(Activity activity) {
            this._syncLocker.EnterWriteLock();
            try
            {
                if (activity._ExecutionState.IsInactive()) activity._ExecutionState = ExecutionStates.Actived;
                _activedActivities.Enqueue(activity);
                if (_scheduling != null && _schedulingTask==null) {
                    _schedulingTask = new Task(async ()=> await Scheduling());
                    _scheduling = true;
                    _schedulingTask.Start();
                }
            }
            finally
            {
                this._syncLocker.ExitWriteLock();
            }
        }
        int _schedulingInterval;
        public int SchedulingInterval { get; private set; }

        async Task Scheduling() {
            while (true) {
                DateTime now = DateTime.Now;
                Execute(now);
                //没有后续处理了,停止后台调度任务，如果有的话
                
                this._syncLocker.EnterWriteLock();
                var interval = 0;
                try
                {
                    if (this._activedActivities.Count == 0)
                    {
                        _scheduling = false;
                        return;
                    }
                    
                    if (_scheduling == null || _scheduling.Value == false) break;
                    interval = (this._schedulingInterval<= 0)? 200: this._schedulingInterval;
                }
                finally
                {
                    this._syncLocker.ExitWriteLock();
                }
                await Task.Delay(interval);
            }
        }
        public void Execute(DateTime? executeTime=null)
        {
            var suspendedQueue = new Queue<Activity>();
            var execTime = executeTime == null ? DateTime.Now : executeTime.Value;
            this._syncLocker.EnterWriteLock();
            try
            {
                Activity activity = null;
                while ((activity = _activedActivities.Dequeue()) != null)
                {
                    InteralExecuteActivity(activity, suspendedQueue, execTime);
                }
                this._activedActivities = suspendedQueue;
                this.InternalStore();
            }
            finally
            {
                this._syncLocker.ExitWriteLock();
            }
            
        }

        ActivityStates InteralExecuteActivity(Activity activity,Queue<Activity> suspendedQueue, DateTime execTime) {
            //这个属性虽然放在activity上面，起读写都受Engine锁的保护
            var state = activity._ExecutionState;
            //谁在线程sleep期间修改了activity的状态
            //比如手动调用了ExecuteActivity
            //就不用执行了。
            if (state.IsInactive()) return activity._State;
            //状态是挂起，但时间还没到，继续挂起，处理下一个活动
            if (state == ExecutionStates.Suspended && activity.ResumeTime > execTime)
            {
                suspendedQueue.Enqueue(activity);
                return activity._State;
            }
            activity._ExecutionState = ExecutionStates.Running;
            var result1 = activity.Execute();
            var result = new ExecuteResult();
            ///挂起状态，并且有明确的恢复时间，重新进入执行队列
            if (result.ResumeTime != null)
            {
                activity._ExecutionState = ExecutionStates.Suspended;
                suspendedQueue.Enqueue(activity);
            }
            else if (result.State == ActivityStates.Finished)
            {
                activity._ExecutionState = ExecutionStates.Completed;
            }
            else {
                activity._ExecutionState = ExecutionStates.Inactive;
            }
            return activity._State;
        }

        
        /// <summary>
        /// 获取或载入一个活动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Activity GetOrLoadActivity(Guid id) { return null; }

        void ActiveNexts(Activity finishedActivity) {

        }
    }
}
