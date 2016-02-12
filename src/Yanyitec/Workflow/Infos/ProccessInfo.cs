using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow.Infos
{
    public class ProccessInfo : Info
    {

        public ProccessInfo() : base(Guid.NewGuid())
        {

        }
        public ProccessInfo(ProccessDefination defination) : base(Guid.NewGuid(), defination)
        {

        }
        public ProccessInfo(JObject data) : base(data)
        {
            this.BlockId = new Guid(data["BlockId"]);
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            builder.Member("BlockId", this.BlockId);

        }
        public Guid BlockId { get; set; }
    }
}
