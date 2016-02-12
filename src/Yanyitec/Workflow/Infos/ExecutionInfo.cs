using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow.Infos
{
    public class ExecutionInfo : Info
    {
        public ExecutionInfo(Guid proccessId) : base(proccessId)
        {
            this.State = ActivityStates.Initial;
            this.ExecutionState = ExecutionStates.Inactive;
        }
        public ExecutionInfo(Guid proccessId, ExecutionDefination defination) : base(proccessId, defination)
        {
            this.State = ActivityStates.Initial;
            this.ExecutionState = ExecutionStates.Inactive;
        }
        public ExecutionInfo(JObject data) : base(data)
        {
            this.State = (ActivityStates)Enum.Parse(typeof(ActivityStates), data["State"].ToString());
            this.ExecutionState = (ExecutionStates)Enum.Parse(typeof(ExecutionStates), data["ExecutionState"].ToString());
            if (this.ExecutionState == ExecutionStates.Suspended)
            {
                var time = data["ResumeTime"];
                if (time != null && !(time is JUndefined))
                {
                    DateTime t = DateTime.MinValue;
                    if (DateTime.TryParse(time, out t)) this.ResumeTime = t;
                }
            }
            var result = data["Result"];
            if (result != null && !(result is JUndefined)) this.Result = result.ToString();
            var nexts = data["Nexts"] as JArray;
            if (nexts != null)
            {
                foreach (var idstr in nexts)
                {
                    this.Nexts.Add(new Guid(idstr));
                }
            }
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            builder.Member("State", Enum.GetName(typeof(ActivityStates), this.State));
            builder.Member("ExecutionState", Enum.GetName(typeof(ExecutionStates), this.ExecutionState));
            if (this.ExecutionState == ExecutionStates.Suspended && this.ResumeTime != null) builder.Member("ResumeTime", this.ResumeTime.Value.ToString());
            if (this.Result != null) builder.Member("Result", this.Result);
            if (this._nexts.Count > 0)
            {
                using (var nexts = builder.ArrayMember("Nexts"))
                {
                    foreach (var nextId in this._nexts)
                    {
                        nexts.Add(nextId.ToString());
                    }
                }
            }
        }
        readonly List<Guid> _nexts = new List<Guid>();
        public IList<Guid> Nexts { get { return _nexts; } }

        public ActivityStates State { get; set; }

        public ExecutionStates ExecutionState { get; set; }

        DateTime? ResumeTime { get; set; }

        public string Result { get; set; }
    }
}
