using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow.Repositories
{
    public interface IDefinationRepository
    {
        void SavePackageDefination(PackageDefination packageDefination);
        PackageDefination GetPackageDefination(string alias);

        IList<PackageDefination> ListPackages();

        void SaveProccessDefination(ProccessDefination proccess);
        ProccessDefination GetProccessDefination(string alias);

        IList<ProccessDefination> ListProccessDefnations(string packageAlias);
        void SaveActivityDefination(ActivityDefination activityDefination);
        void SaveTransactionDefination(TransactionDefination transactionDefination);
        void SaveExecutionDefination(ExecutionDefination executionDefination);

        ActivityDefination GetActivityDefination(string alias);
        TransactionDefination GetTransactionDefination(string alias);

        IList<ExecutionDefination> ListExecutionDefinations(string proccessAlias);

    }
}
