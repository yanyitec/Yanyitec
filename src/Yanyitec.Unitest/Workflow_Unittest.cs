namespace Yanyitec.Unitest
{
    using System.Linq;
    using Yanyitec.Testing;
    using Yanyitec.Workflow.Definations;
    using Yanyitec.Workflow;
    using Yanyitec.Workflow.Repositories;
    [Testing.Test]
    public class Workflow_Unittest
    {
        IDefinationRepository GetDefinationRepository(string type ,string connString) {
            var storage = new Storaging.Storage(connString);
            storage.Clear();
            return new StorageDefinationRepository(storage);
        }

        void TestDefinationAccess(IDefinationRepository rep) {
            #region package
            PackageDefination pack1 = new PackageDefination()
            {
                Alias = "Test1",
                Name = "Test Package1",
                Description = "Just for test package=1"
            };

            rep.SavePackageDefination(pack1);
            PackageDefination pack2 = new PackageDefination() {
                Alias ="Test2",
                Name = "Test Package2",
                Description = "Just for test package=2"
            };
            pack2.Extras["author"] = "yiy";
            rep.SavePackageDefination(pack2);
            var packs = rep.ListPackages();
            Assert.Equal(2,packs.Count);
            Assert.Equal("Test1",packs[0].Alias);
            Assert.Equal("Test2",packs[1].Alias);

            var pack = rep.GetPackageDefination("test2");
            Assert.Equal("Test2",pack.Alias);
            Assert.Equal("Test Package2",pack.Name);
            Assert.Equal("Just for test package=2",pack.Description);
            Assert.Equal(1,pack.Extras.Count);
            Assert.Equal("yiy",pack.Extras["author"]);
            #endregion

            #region proccess
            var proccess = new ProccessDefination();
            proccess.Alias = "test2/proccess1";
            proccess.Name = "Proccess1";
            proccess.StartAlias = "test2/proccess1/PR";
            proccess.FinishAlias = "test2/proccess1/PO";

            rep.SaveProccessDefination(proccess);

            var pro = rep.GetProccessDefination("test2/proccess1");
            Assert.Equal("test2/proccess1",pro.Alias);
            Assert.Equal("Proccess1",pro.Name);
            Assert.Equal("test2/proccess1/PR", pro.StartAlias);
            Assert.Equal("test2/proccess1/PO", pro.FinishAlias);


            #endregion

            #region activity
            var prActivity = new ActivityDefination();
            prActivity.Alias = "test2/proccess1/PR";
            prActivity.Name = "PR";
            prActivity.InstanceType = "Activity";
            prActivity.StartMode = ExecutionModes.Automatic;
            prActivity.StartConstraintKind = ConstraintKinds.Code;
            prActivity.StartConstraint = "return true";
            prActivity.FinishMode = ExecutionModes.Manual;
            prActivity.FinishConstraintKind = ConstraintKinds.Class;
            prActivity.FinishConstraint = "Finish";
            

            rep.SaveActivityDefination(prActivity);

            var activity = rep.GetActivityDefination("test2/proccess1/PR");
            Assert.Equal("test2/proccess1/PR", activity.Alias);
            Assert.Equal("PR", activity.Name);
            Assert.Equal("test2/proccess1", activity.ProccessAlias);
            Assert.Equal("Activity", activity.InstanceType);
            Assert.Equal(ExecutionModes.Automatic, activity.StartMode);
            Assert.Equal(ConstraintKinds.Code, activity.StartConstraintKind);
            Assert.Equal("return true", activity.StartConstraint);
            Assert.Equal(ExecutionModes.Manual, activity.FinishMode);
            Assert.Equal(ConstraintKinds.Class, activity.FinishConstraintKind);
            Assert.Equal("Finish", activity.FinishConstraint);
            #endregion

            #region transaction
            var trans = new TransactionDefination();
            trans.Alias = "test2/proccess1/PR2PO";
            trans.Name = "PR2PO";
            trans.InstanceType = "Transaction";
            trans.Constraint = "return true;";
            trans.ConstraintKind = ConstraintKinds.Code;
            trans.FromAlias = "test2/proccess1/PR";
            trans.ToAlias = "test2/proccess1/PO";


            rep.SaveTransactionDefination(trans);

            var transaction = rep.GetTransactionDefination("test2/proccess1/PR2PO");
            Assert.Equal("test2/proccess1/PR2PO", transaction.Alias);
            Assert.Equal("PR2PO", transaction.Name);
            Assert.Equal("test2/proccess1", transaction.ProccessAlias);
            Assert.Equal("Transaction", transaction.InstanceType);
            Assert.Equal(ConstraintKinds.Code, transaction.ConstraintKind);
            Assert.Equal("return true;", transaction.Constraint);
            
            Assert.Equal("test2/proccess1/PR", transaction.FromAlias);
            Assert.Equal("test2/proccess1/PO", transaction.ToAlias);
            #endregion

            #region list execution
            var list = rep.ListExecutionDefinations("test2/proccess1");
            Assert.NotNull(list);
            Assert.Equal(2,list.Count);
            Assert.NotNull(list.FirstOrDefault(p=>p.Alias == "test2/proccess1/PR"));
            Assert.NotNull(list.FirstOrDefault(p => p.Alias == "test2/proccess1/PR2PO"));
            #endregion
        }

        [Test]
        public void StorageDefinationRepository()
        {
            var location = "d:/yanyi_test/workflow";
            var rep = this.GetDefinationRepository("storage",location);
            TestDefinationAccess(rep);
        }

        
    }
}
