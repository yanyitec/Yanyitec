using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class TransactionDefination : ExecutionDefination
    {
        public TransactionDefination() : base() { }

        public TransactionDefination(TransactionDefination other) : base(other as ExecutionDefination)
        {

            this.ConstraintKind = other.ConstraintKind;
            this.Constraint = other.Constraint;
            this.From = other.From;

        }

        public TransactionDefination(JObject data) : base(data)
        {
            this.From = data["From"].ToString();
            this.To = data["To"].ToString();
            this.ConstraintKind = (ConstraintKinds)Enum.Parse(typeof(ConstraintKinds), data["ConstraintKind"].ToString());
            if (this.ConstraintKind != ConstraintKinds.None && !string.IsNullOrEmpty(this.Constraint)) this.Constraint = data["Constraint"].ToString();

        }

        //protected override void ToJson(ObjectBuilder builder)
        //{
        //    base.ToJson(builder);
        //    builder.Member("From", this.From);
        //    builder.Member("To", this.To);
        //    builder.Member("ConstraintKind", Enum.GetName(typeof(ConstraintKinds), this.ConstraintKind));
        //    if (this.ConstraintKind != ConstraintKinds.None && !string.IsNullOrEmpty(this.Constraint)) builder.Member("Constraint", this.Constraint);

        //}


        public string Constraint { get; set; }
        public ConstraintKinds ConstraintKind { get; set; }

        public string From { get; set; }

        public string To { get; set; }

    }
}
