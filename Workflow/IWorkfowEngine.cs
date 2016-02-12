using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow
{
    public interface IWorkfowEngine
    {
        ActivityDefination GetActivityDefination(Guid id);

        TransactionDefination GetTransactionDefination(Guid id);

        Func<Activity, bool> GetCondition(Guid entityId, ConditionKinds kinds);

        Func<Activity, object> GetDeal(Guid activityId);

        Activity CreateActivity(Guid entityId);



    }
}
