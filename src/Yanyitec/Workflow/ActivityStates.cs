using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public enum ActivityStates
    {
        /// <summary>
        /// 还未激活
        /// </summary>
        Inactive =0,
        Actived = 1,
        /// <summary>
        /// 可能是手动设置状态为WaitingStart
        /// 或该活动已经激活但被前置条件阻塞,
        /// 无论何种状态，Process.Run时，都会运行TryStart
        /// 不再检查 StartMode
        /// </summary>
        WaitingStart = 2,
       
       
        Started = 3,
        /// <summary>
        /// 执行中，还没有结果
        /// </summary>
        Dealing = 4,
        /// <summary>
        /// 挂起,等待再次激活
        /// </summary>
        Suspending = 5,
        /// <summary>
        /// 已经处理
        /// </summary>
        Dealed = 6,
        /// <summary>
        /// 等待结束，但后置条件不满足
        /// </summary>
        WaitingFinish = 7,
       
        /// <summary>
        /// 已经执行过了
        /// </summary>
        Finished = 8

    }
}
