using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    /// <summary>
    /// 里面的Activity会并行的运行
    /// </summary>
    public class ParallelActivity : BlockActivity
    {
        public ParallelActivity(GroupActivity container) :base(container) { }

        public override object Deal()
        {
            var dealed = true;
            foreach (var activity in this._Activities)
            {
                var state = activity.Execute();
                if (state != ActivityStates.Finished) dealed = false;
            }
            if (dealed) return null;
            return Yanyitec.Workflow.DealResult.Running;
        }
    }
}
