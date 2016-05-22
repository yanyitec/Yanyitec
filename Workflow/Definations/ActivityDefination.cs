using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class ActivityDefination : Defination
    {

        public string ActivityType { get; set; }

        public string Deal { get; set; }

        public ExecutionModes StartMode { get; set; }

        public ExecutionModes FinishMode { get; set; }

        public string StartConstraint { get; set; }

        public string FinishConstraint { get; set; }

        public ConstraintKinds StartConstraintKind { get; set; }

        public ConstraintKinds FinishConstraintKind { get; set; }

        public override Defination CloneTo(Defination existed = null)
        {
            var entity = existed as ActivityDefination;
            if (entity == null) entity = new ActivityDefination();
            base.CloneTo(entity);
            entity.ActivityType = this.ActivityType;
            entity.Deal = this.Deal;
            entity.StartMode = this.StartMode;
            entity.StartConstraint = this.StartConstraint;
            entity.StartConstraintKind = this.StartConstraintKind;
            entity.FinishConstraint = this.FinishConstraint;
            entity.FinishConstraintKind = this.FinishConstraintKind;
            entity.FinishMode = this.FinishMode;
            return entity;
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            builder.Member("ActivityType", this.ActivityType);
            builder.Member("Deal", this.Deal);
            builder.Member("StartMode", Enum.GetName(typeof(ExecutionModes),this.StartMode));
            builder.Member("FinishMode", Enum.GetName(typeof(ExecutionModes), this.FinishMode));
            builder.Member("StartConstraint", this.StartConstraint);
            builder.Member("FinishConstraint", this.FinishConstraint);
            builder.Member("StartConstraintKind", Enum.GetName(typeof(ConstraintKinds), this.StartConstraintKind));
            builder.Member("FinishConstraintKind", Enum.GetName(typeof(ConstraintKinds), this.FinishConstraintKind));
        }

        public static ActivityDefination FromJson(JObject data, ActivityDefination entity=null) {
            if (entity == null) entity = new ActivityDefination();
            Defination.FromJson(data, entity);
            
            entity.ActivityType = data["ActivityType"].ToString();
            entity.Deal = data["Deal"].ToString();
            entity.StartMode =(ExecutionModes)Enum.Parse(typeof(ExecutionModes), data["StartMode"].ToString());
            entity.FinishMode = (ExecutionModes)Enum.Parse(typeof(ExecutionModes), data["FinishMode"].ToString());
            entity.StartConstraint = data["StartConstraint"].ToString();
            entity.FinishConstraint = data["FinishConstraint"].ToString();
            entity.StartConstraintKind = (ConstraintKinds)Enum.Parse(typeof(ConstraintKinds), data["StartConstraintKind"].ToString());
            entity.FinishConstraintKind = (ConstraintKinds)Enum.Parse(typeof(ConstraintKinds), data["FinishConstraintKind"].ToString());
            return entity;
        }
    }
}
