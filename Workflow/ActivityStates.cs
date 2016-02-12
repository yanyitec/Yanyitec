using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    
    public enum ActivityStates
    {
        Initial,

        /// <summary>
        /// 可能是手动设置状态为WaitingStart
        /// 或该活动已经激活但被前置条件阻塞,
        /// 无论何种状态，Execute时，都会运行TryStart
        /// 不再检查 StartMode
        /// </summary>
        WaitingStart ,
       
        /// <summary>
        /// 已经开始
        /// </summary>
        Started ,
        /// <summary>
        /// 执行中，还没有结果
        /// </summary>
        Dealing,
        /// <summary>
        /// 挂起了，下次还会执行
        /// </summary>
        WaitingDeal,
        /// <summary>
        /// 已经处理，Activiy上有执行结果
        /// </summary>
        Dealed ,
        /// <summary>
        /// 等待结束，但后置条件不满足
        /// </summary>
        WaitingFinish ,
        /// <summary>
        /// 已经结束
        /// </summary>
        Finished
       

    }
}
