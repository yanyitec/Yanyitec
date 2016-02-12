using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class ExecutionDefination :Defination
    {
        public ExecutionDefination() : base() { }

        public ExecutionDefination(ExecutionDefination other) : base(other) { }

        public ExecutionDefination(JObject data) : base(data) { }
    }
}
