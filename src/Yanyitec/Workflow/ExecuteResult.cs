using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public class ExecuteResult
    {
        public ExecuteResult(string result) {
            this.Result = result;
        }
        public ExecuteResult(DateTime resumeTime) {
            this.ResumeTime = resumeTime;
        }
        public string Result { get; private set; }

        public DateTime? ResumeTime { get; private set; }

        public readonly static ExecuteResult Running = new ExecuteResult(DateTime.MinValue);
        public readonly static ExecuteResult Suspending = new ExecuteResult(DateTime.MaxValue);
    }
}
