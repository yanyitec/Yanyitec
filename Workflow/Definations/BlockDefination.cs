using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class BlockDefination : ActivityDefination
    {
        public Guid StartActivityId { get; set; }

        public Guid FinishActivityId { get; set; }

        //public IList<Guid> TransactionIds { get; set; }

        //public IList<Guid> ActivityIds { get; set; }
        public override Defination CloneTo(Defination existed = null)
        {
            var entity = existed as BlockDefination;
            if (entity == null) entity = new BlockDefination();
            base.CloneTo(entity);
            entity.StartActivityId = this.StartActivityId;
            entity.FinishActivityId = this.FinishActivityId;
            return entity;
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            builder.Member("StartActivityId",this.StartActivityId);
            builder.Member("FinishActivityId", this.FinishActivityId);
        }

        public static BlockDefination FromJson(JObject data, BlockDefination entity = null) {
            if (entity == null) entity = new BlockDefination();
            entity.StartActivityId = new Guid(data["StartActivityId"]);
            entity.FinishActivityId = new Guid(data["FinishActivityId"]);
            return entity;
        }
    }
}
