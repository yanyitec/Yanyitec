using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class ProccessDefination : Defination
    {
        public ProccessDefination() : base() { }

        public ProccessDefination(ProccessDefination other) : base(other)
        {
            this.StartAlias = other.StartAlias;
            this.FinishAlias = other.FinishAlias;
            this.RuntimeId = other.RuntimeId;
        }

        public ProccessDefination(JObject data) : base(data)
        {
            this.StartAlias = data["StartAlias"].ToString();
            this.FinishAlias = data["FinishAlias"].ToString();
            var rtIdData = data["RuntimeId"];
            if (rtIdData != null) this.RuntimeId = new Guid(rtIdData.ToString());
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            if (this.RuntimeId != null) builder.Member("RuntimeId",this.RuntimeId.Value);
            builder.Member("StartAlias", this.StartAlias??"");
            builder.Member("FinishAlias", this.FinishAlias ?? "");
        }

        public Guid? RuntimeId { get; set; }

        public string StartAlias { get; set; }
        public string FinishAlias { get; set; }

    }
}
