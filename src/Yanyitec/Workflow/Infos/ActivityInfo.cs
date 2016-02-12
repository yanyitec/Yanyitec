using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow.Infos
{
    public class ActivityInfo : Info
    {
        public ActivityInfo(Guid proccessId) : base(proccessId) { }

        public ActivityInfo(Guid proccessId, ActivityDefination defination):base(proccessId,defination) {
            this.Deal = defination.Deal;
            this.StartMode = defination.StartMode;
            this.FinishMode = defination.FinishMode;
            this.StartConstraintKind = defination.StartConstraintKind;
            this.FinishConstraintKind = defination.FinishConstraintKind;
            this.StartConstraint = defination.StartConstraint;
            this.FinishConstraint = defination.FinishConstraint;
        }

        public ActivityInfo(JObject data) :base(data){

            this.Deal = data["Deal"]?.ToString();
            this.StartMode = (ExecutionModes)Enum.Parse(typeof(ExecutionModes), data["StartMode"].ToString());
            this.FinishMode = (ExecutionModes)Enum.Parse(typeof(ExecutionModes), data["FinishMode"].ToString());
            this.StartConstraintKind = (ConstraintKinds)Enum.Parse(typeof(ConstraintKinds), data["StartConstraintKind"].ToString());
            this.FinishConstraintKind = (ConstraintKinds)Enum.Parse(typeof(ConstraintKinds), data["FinishConstraintKind"].ToString());
            if (this.StartConstraintKind != ConstraintKinds.None ) this.StartConstraint = data["StartConstraint"].ToString();
            if (this.FinishConstraintKind != ConstraintKinds.None ) this.FinishConstraint = data["FinishConstraint"].ToString();
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            if(!string.IsNullOrEmpty(this.Deal))builder.Member("Deal", this.Deal);
            builder.Member("StartMode", Enum.GetName(typeof(ExecutionModes), this.StartMode));
            builder.Member("FinishMode", Enum.GetName(typeof(ExecutionModes), this.FinishMode));
            builder.Member("StartConstraintKind", Enum.GetName(typeof(ConstraintKinds), this.StartConstraintKind));
            builder.Member("FinishConstraintKind", Enum.GetName(typeof(ConstraintKinds), this.FinishConstraintKind));
            if (this.StartConstraintKind != ConstraintKinds.None && !string.IsNullOrEmpty(this.StartConstraint)) builder.Member("StartConstraint", this.StartConstraint);
            if (this.FinishConstraintKind != ConstraintKinds.None && !string.IsNullOrEmpty(this.FinishConstraint)) builder.Member("FinishConstraint", this.FinishConstraint);

        }

        public string Deal { get; set; }

        public ExecutionModes StartMode { get; set; }

        public ExecutionModes FinishMode { get; set; }

        public string StartConstraint { get; set; }

        public string FinishConstraint { get; set; }

        public ConstraintKinds StartConstraintKind { get; set; }

        public ConstraintKinds FinishConstraintKind { get; set; }
    }
}
