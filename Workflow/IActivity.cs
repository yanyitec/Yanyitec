using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public interface IActivity
    {
        /// <summary>
        /// 活动的唯一编号
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 活动的状态
        /// </summary>
        ActivityStates State { get; }
        object Result { get; }

        string this[string key] { get; }
    }
}
