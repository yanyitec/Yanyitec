using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow.Definations
{
    public class ActivityDefination : Defination
    {
        public Guid ContainerId { get; set; }

        public string Typename { get; set; }
        public ExecutionModes StartMode { get; set; }

        public ExecutionModes FinishMode { get; set; }

        public string StartConstraint { get; set; }

        public string FinishConstraint { get; set; }
    }
}
