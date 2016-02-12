using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow.Infos
{
    public class TransactionInfo : Info
    {
        public TransactionInfo(Guid proccessId) : base(proccessId) { }

        public TransactionInfo(Guid proccessId, TransactionDefination defination) : base(proccessId, defination)
        {
            this.ConstraintKind = defination.ConstraintKind;
            this.Constraint = defination.Constraint;

        }

        public TransactionInfo(JObject data) : base(data)
        {

            this.From = new Guid(data["From"].ToString());
            this.To = new Guid(data["To"].ToString());
            this.ConstraintKind = (ConstraintKinds)Enum.Parse(typeof(ConstraintKinds), data["ConstraintKind"].ToString());
            if (this.ConstraintKind != ConstraintKinds.None) this.Constraint = data["Constraint"].ToString();

        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            builder.Member("From", this.From);
            builder.Member("To", this.To);
            builder.Member("ConstraintKind", Enum.GetName(typeof(ConstraintKinds), this.ConstraintKind));
            if (this.ConstraintKind != ConstraintKinds.None && !string.IsNullOrEmpty(this.Constraint)) builder.Member("Constraint", this.Constraint);

        }

        public string Constraint { get; set; }
        public ConstraintKinds ConstraintKind { get; set; }

        public Guid From { get; set; }

        public Guid To { get; set; }
    }
}
