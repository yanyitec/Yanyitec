using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow.Infos
{
    public class BlockActivityInfo : ActivityInfo
    {
        public BlockActivityInfo(Guid proccessId) : base(proccessId) { }

        public BlockActivityInfo(Guid proccessId, BlockActivityDefination defination):base(proccessId,defination) {
            
        }

        public BlockActivityInfo(JObject data) :base(data){
            this.StartActivityId = new Guid(data["StartActivityId"].ToString());
            this.FinishActivityId = new Guid(data["FinishActivityId"].ToString());


        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            builder.Member("StartActivityId",this.StartActivityId);
            builder.Member("FinishActivityId", this.FinishActivityId);
        }

        public Guid StartActivityId { get; set; }

        public Guid FinishActivityId { get; set; }
    }
}
