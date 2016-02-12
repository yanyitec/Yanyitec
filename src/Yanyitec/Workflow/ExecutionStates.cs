using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    [Flags]
    public enum ExecutionStates
    {
        
        /// <summary>
        /// 还未激活,不在执行队列里
        /// </summary>
        Inactive = 1,
        /// <summary>
        /// 已经进入到了执行队列
        /// </summary>
        Actived = 1 << 1,
        /// <summary>
        /// 正在执行中
        /// </summary>
        Running = 1<< 2 | Actived,
        /// <summary>
        /// 挂起，但还在队列中
        /// </summary>
        Suspended = 1 << 3 | Actived,
        /// <summary>
        /// 已经结束，不在执行队列中
        /// </summary>
        Completed = 1 << 4 | Inactive
    }
}
