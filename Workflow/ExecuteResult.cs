using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public class ExecuteResult
    {
        public object Result { get; set; }
        public ActivityStates State { get; set; }

        public bool IsSuspending { get; set; }
        public DateTime? ResumeTime { get; set; }
    }
}
