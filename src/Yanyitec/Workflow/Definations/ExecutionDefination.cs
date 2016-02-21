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

        public ExecutionDefination(ExecutionDefination other) : base(other) {
            this.ProccessAlias = other.ProccessAlias;
            this.InstanceType = other.InstanceType;
        }

        public ExecutionDefination(JObject data) : base(data) {
            this.ProccessAlias = data["ProccessAlias"]?.ToString();
            
            this.InstanceType = data["InstanceType"].ToString();
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            if (!string.IsNullOrEmpty(this.ProccessAlias)) builder.Member("ProccessAlias", this.ProccessAlias);
            builder.Member("InstanceType", this.InstanceType);
        }
        public string InstanceType { get; set; }


        /// <summary>
        /// the process's alias
        /// </summary>
        public string ProccessAlias { get; set; }


    }
}
