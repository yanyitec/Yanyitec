using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow.Definations
{
    public class GroupDefination : ActivityDefination
    {
        public string StartActivityId { get; set; }

        public string FinishActivityId { get; set; }

        public IList<int> ActivityRuntimeIds { get; set; }

        public IList<Guid> TransactionDefinations { get; set; }

        public IList<Guid> ActivityDefinations { get; set; }
    }
}
