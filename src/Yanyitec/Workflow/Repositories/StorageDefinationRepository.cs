

namespace Yanyitec.Workflow.Repositories
{
    using System;
    using System.Collections.Generic;
    using Yanyitec.Json;
    using Yanyitec.Storaging;
    using Yanyitec.Workflow.Definations;

    public class StorageDefinationRepository : IDefinationRepository
    {
        public StorageDefinationRepository(string location) {
            this.Storage = new Storage(location);
        }

        public StorageDefinationRepository(IStorage storage) {
            this.Storage = storage;
        }

        public IStorage Storage { get;protected set; }

        static bool CheckExecutionAlias(string alias, string proccessAlias) {
            if (alias == null || proccessAlias == null) return false;
            if (!alias.StartsWith(proccessAlias)) return false;
            if (alias[proccessAlias.Length] != '/') return false;
            return true;
        }

        public static string CheckExecutionAliases(ExecutionDefination defination) {
            var alias = defination.Alias;
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("Alias should not be empty");
            
            var at = alias.IndexOf("/");
            if (at < 0) throw new ArgumentException("Alias[" + alias + "] is invalid. It should has / at least once.");
            if (++at == alias.Length) throw new ArgumentException("Alias[" + alias + "] is invalid. It should contains proccess part.");
            var at1 = alias.IndexOf("/",at);
            if (at1 < 0) throw new ArgumentException("Alias[" + alias + "] is invalid. It should has / at least twice.");
            if (++at1 == alias.Length) throw new ArgumentException("Alias[" + alias + "] is invalid. It should contains execution part.");
            var proccessAlias1 = alias.Substring(0,at1-1);
            var proccessAlias = defination.ProccessAlias;
            if (proccessAlias == null)
            {
                defination.ProccessAlias = proccessAlias1;
            }
            else {
                if(defination.ProccessAlias!= proccessAlias1) throw new ArgumentException("Alias[" + alias + "] is invalid. It's proccess alias["+proccessAlias1+"] is not match the execution alias.");
            }
            var at0 = alias.LastIndexOf('/');
            if (++at0==alias.Length) throw new ArgumentException("Alias[" + alias + "] is invalid. It should contains execution part.");
            return alias.Substring(at0);
        }

        static string GetShortAlias(string alias) {
            var at0 = alias.LastIndexOf('/');
            if (++at0 == alias.Length) throw new ArgumentException("Alias[" + alias + "] is invalid. It should contains execution part.");
            return alias.Substring(at0);
        }

        public static string CheckProccessAliases(ProccessDefination proccess)
        {
            var alias = proccess.Alias;
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("ProccessDefination.Alias is required.");
            var at = alias.IndexOf("/");
            if (at < 0) throw new ArgumentException("Proccess alias is invalid. It should contains / at least once");
            if (at == alias.Length - 1) throw new ArgumentException("Proccess alias["+alias+"] is invalid.");
            var at1 = alias.IndexOf("/", at+1);
            if (at1 >= 0) throw new ArgumentException("Proccess alias is invalid. It should contains / only once.");
            var shortAlias = alias.Substring(at + 1);
            if (string.IsNullOrEmpty(shortAlias)) throw new ArgumentException("ProccessDefination.Alias is not correct. It should be 'packageAlias/proccessId'. ");

            if (!CheckExecutionAlias(proccess.StartAlias, alias)) throw new ArgumentException("StartAlias is invalid");
            if (!CheckExecutionAlias(proccess.FinishAlias, alias)) throw new ArgumentException("FinishAlias is invalid");

            return shortAlias;
        }

        public static string CheckTransactionAliases(TransactionDefination transaction) {
            var shortAlias = CheckExecutionAliases(transaction);
            if (!CheckExecutionAlias(transaction.FromAlias,transaction.ProccessAlias)) throw new ArgumentException("FromAlias is invalid");
            if (!CheckExecutionAlias(transaction.ToAlias, transaction.ProccessAlias)) throw new ArgumentException("ToAlias is invalid");

            return shortAlias;
        }

        public void SavePackageDefination(PackageDefination packageDefination)
        {
            var alias = packageDefination.Alias;

            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("PackageDefination.Alias is required.");
            if (alias.Contains("/")) throw new ArgumentException("Invalid package alias. package alias cannot contain /");

            var dir = this.Storage.GetDirectory(alias,true);
            this.Storage.PutText(alias + ".package.json",packageDefination.ToJson());
        }

        public PackageDefination GetPackageDefination(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("alias is required.");
            if (alias.Contains("/")) throw new ArgumentException("Invalid package alias. package alias cannot contain /");
           
            var jsonText = this.Storage.GetText(alias + ".package.json");
            if(jsonText==null) throw new ArgumentException("Package["+alias+"] is not existed or damaged.");
            var data = new Json.Parser().Parse(jsonText) as JObject;
            if(data==null) throw new ArgumentException("Package["+alias+"] is damaged.");
            return new PackageDefination(data);
        }

        public IList<PackageDefination> ListPackages()
        {
            var items = this.Storage.ListItems(false, StorageTypes.File);
            List<PackageDefination> result = new List<PackageDefination>();
            foreach (var item in items) {
                if (!item.Name.EndsWith(".package.json")) continue;
                try
                {
                    var jsonText = this.Storage.GetText(item.Name);
                    if (jsonText == null) throw new ArgumentException("Package[" + item.Name + "] is not existed or damaged.");
                    var data = new Json.Parser().Parse(jsonText) as JObject;
                    if (data == null) throw new ArgumentException("Package[" + item.Name + "] is damaged.");
                    var pack= new PackageDefination(data);
                    result.Add(pack);
                }
                catch (ArgumentException) { }
            }
            return result;
        }

        

        public void SaveProccessDefination(ProccessDefination proccess)
        {
            var alias = proccess.Alias;
            var shortAlias = CheckProccessAliases(proccess);
            
            var dir = this.Storage.GetDirectory(alias, true);
            this.Storage.PutText(alias + ".proccess.json", proccess.ToJson());
        }

        public ProccessDefination GetProccessDefination(string alias)
        {
            var jsonText = this.Storage.GetText(alias + ".proccess.json");
            if (jsonText == null) throw new ArgumentException("Proccess["+alias+"] is not existed or damaged.");
            var data = new Json.Parser().Parse(jsonText) as JObject;
            if (data == null) throw new ArgumentException("Proccess["+alias+"] is damaged.");
            return new ProccessDefination(data);
        }

        public IList<ProccessDefination> ListProccessDefnations(string packageAlias)
        {
            var packDir = this.Storage.GetDirectory(packageAlias);
            if (packDir == null) return null;
            var items = packDir.ListItems(false, StorageTypes.File);
            List<ProccessDefination> result = new List<ProccessDefination>();
            foreach (var item in items)
            {
                if (!item.Name.EndsWith(".proccess.json")) continue;
                var jsonText = packDir.GetText( item.Name);
                if (jsonText == null) continue;
                var data = new Json.Parser().Parse(jsonText) as JObject;
                if (data == null) continue;
                var def = new ProccessDefination(data);
                result.Add(def);
            }
            return result;
        }

        public void SaveActivityDefination(ActivityDefination activityDefination)
        {
            var shortAlias =  CheckExecutionAliases(activityDefination);
            if (string.IsNullOrEmpty(shortAlias)) throw new ArgumentException("ActivityDefination.Alias is not correct. It should be 'packageAlias/proccessId/activityId'. ");
            
            this.Storage.PutText(activityDefination.Alias + ".activity.json", activityDefination.ToJson());
        }

        public ActivityDefination GetActivityDefination(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("alias is required.");
            
            var shortAlias = GetShortAlias(alias);
            var jsonText = this.Storage.GetText(alias + ".activity.json");
            if (jsonText == null) throw new ArgumentException("Activity["+alias+"] is not existed or damaged.");
            var data = new Json.Parser().Parse(jsonText) as JObject;
            if (data == null) throw new ArgumentException("Activity["+alias+"] is damaged.");
            return new ActivityDefination(data);
        }

        

        public void SaveTransactionDefination(TransactionDefination transactionDefination)
        {
            var alias = transactionDefination.Alias;
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("TransactionDefination.Alias is required.");
            CheckTransactionAliases(transactionDefination);
            Storage.PutText(alias + ".transaction.json", transactionDefination.ToJson());
        }

        public TransactionDefination GetTransactionDefination(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("alias is required.");
            
           
            var jsonText = this.Storage.GetText(alias + ".transaction.json");
            if (jsonText == null) throw new ArgumentException("Transaction["+alias+"] is not existed or damaged.");
            var data = new Json.Parser().Parse(jsonText) as JObject;
            if (data == null) throw new ArgumentException("Transaction["+alias+"] is damaged.");
            return new TransactionDefination(data);
        }

        public void SaveExecutionDefination(ExecutionDefination executionDefination)
        {

            var activityDefination = executionDefination as ActivityDefination;
            if (activityDefination != null) this.SaveActivityDefination(activityDefination);
            var transactionDefination = executionDefination as TransactionDefination;
            if (activityDefination != null) this.SaveTransactionDefination(transactionDefination);
            
        }

        public IList<ExecutionDefination> ListExecutionDefinations(string containerAlias)
        {
            var dir = this.Storage.GetDirectory(containerAlias);
            if (dir == null) return null;
            var items = dir.ListItems(true, StorageTypes.File);
            List<ExecutionDefination> result = new List<ExecutionDefination>();
            foreach (var item in items)
            {
                var file = item as IStorageFile;
                if (file == null) continue;
                string jsonText = null;
                JObject data = null;
                if (file.Name.EndsWith(".activity.json"))
                {
                    jsonText = file.GetText();
                    if (jsonText == null) continue;
                    data = new Json.Parser().Parse(jsonText) as JObject;
                    if (data == null) continue;
                    var def = new ActivityDefination(data);
                    result.Add(def);
                }
                else if (file.Name.EndsWith(".transaction.json")) {
                    jsonText = file.GetText();
                    if (jsonText == null) continue;
                    data = new Json.Parser().Parse(jsonText) as JObject;
                    if (data == null) continue;
                    var def = new TransactionDefination(data);
                    result.Add(def);
                }
                
            }
            return result;
        }

        public bool ModifyAlias(string oldAlias, string shortAlias) {
            var dir = this.Storage.GetDirectory(oldAlias);
            var pdir = dir.Parent;
            if (dir == null) return false;
            #region rename the defination file name
            var filename = oldAlias + ".proccess.json";
            if (pdir.GetItem(filename) != null) pdir.Rename(shortAlias + ".proccess.json");
            else {
                filename = oldAlias + ".activity.json";
                if (pdir.GetItem(filename) != null) pdir.Rename(shortAlias + ".activity.json");
                else {
                    filename = oldAlias + ".transaction.json";
                    if (pdir.GetItem(filename) != null) pdir.Rename(shortAlias + ".transaction.json");
                    else
                    {

                        filename = oldAlias + ".package.json";
                        if (pdir.GetItem(filename) != null) pdir.Rename(shortAlias + ".package.json");
                        else
                        {

                            filename = null;
                        }
                    }
                }
            }
            #endregion
            if (filename == null) return false;

            

            return true;
        }
    }
}
