using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class ParallelActivityDefination : ActivityDefination
    {
        public ParallelActivityDefination() : base() { }

        public ParallelActivityDefination(BlockActivityDefination other):base(other) {
            
        }

        public ParallelActivityDefination(JObject data) :base(data){

            
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
           

        }

        
    }
}
