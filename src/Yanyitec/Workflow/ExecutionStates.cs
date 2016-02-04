using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public enum ExecutionStates
    {
        /// <summary>
        /// 已经处理完成
        /// </summary>
        Completed,
        /// <summary>
        /// 正在运行，还没处理完
        /// </summary>
        Running,
        /// <summary>
        /// 挂起
        /// </summary>
        Suspended
    }
}
