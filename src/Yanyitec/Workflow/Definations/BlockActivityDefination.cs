using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class BlockActivityDefination : ActivityDefination
    {
        public BlockActivityDefination() : base() { }

        public BlockActivityDefination(BlockActivityDefination other):base(other) {
            this.StartActivity = other.StartActivity;
            this.FinishActivity = other.FinishActivity;
        }

        public BlockActivityDefination(JObject data) :base(data){

            this.StartActivity = data["StartActivity"].ToString();
            this.FinishActivity = data["FinishActivity"].ToString();
            
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            builder.Member("StartActivity", this.StartActivity);
            builder.Member("FinishActivity", this.FinishActivity);
            
        }
        
        public string StartActivity { get; set; }

        public string FinishActivity { get; set; }
    }
}
