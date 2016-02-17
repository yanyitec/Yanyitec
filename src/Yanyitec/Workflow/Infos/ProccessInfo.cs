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
            var actived = data["Activied"] as JArray;
            if (actived != null) {
                foreach (var idstr in actived) {
                    this.Actived.Add(new Guid(idstr));
                }
            }
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            builder.Member("BlockId", this.BlockId);
            if (this.Actived.Count > 0) {
                using (var actived = builder.ArrayMember("Actived")) {
                    foreach (var id in this.Actived) {
                        actived.Add(id);
                    }
                }
            }

        }
        public Guid BlockId { get; set; }

        public bool Finished { get; set; }


        public IList<Guid> Actived { get;  set; }
    }
}
