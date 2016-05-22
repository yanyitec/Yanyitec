using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class TransactionDefination : Defination
    {
        public ConstraintKinds ConstraintKind { get; set; }
        public string Constraint { get; set; }

        public Guid From { get; set; }

        public Guid To { get; set; }

        public override Defination CloneTo(Defination existed = null)
        {
            var entity = existed as TransactionDefination;
            if (entity == null) entity = new TransactionDefination();
            base.CloneTo(entity);
            entity.From = this.From;
            entity.To = this.To;
            entity.Constraint = this.Constraint;
            entity.ConstraintKind = this.ConstraintKind;
            return entity;
        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            builder.Member("ConstraintKind", Enum.GetName(typeof(ConstraintKinds), this.ConstraintKind));
            builder.Member("Constraint",this.Constraint);
            builder.Member("From", this.From.ToString());
            builder.Member("To", this.To.ToString());
        }

        public static TransactionDefination FromJson(JObject data, TransactionDefination entity = null)
        {
            if (entity == null) entity = new TransactionDefination();
            Defination.FromJson(data, entity);
            entity.Constraint = data["Constraint"].ToString();
            entity.ConstraintKind = (ConstraintKinds)Enum.Parse(typeof(ConstraintKinds), data["ConstraintKind"].ToString());
            entity.From = new Guid(data["From"].ToString());
            entity.To = new Guid(data["To"].ToString());
            return entity;
        }

    }
}
