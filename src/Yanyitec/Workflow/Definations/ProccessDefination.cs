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
            this.Block = other.Block;
        }

        public ProccessDefination(JObject data) : base(data)
        {
            this.Block = data["Block"].ToString();
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            builder.Member("Block", this.Block);
        }

        public string Block { get; set; }
    }
}
