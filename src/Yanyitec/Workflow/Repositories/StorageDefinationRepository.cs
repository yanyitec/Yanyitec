

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

        public IStorage Storage { get;private set; }

        static string GetShortAlias(string alias, string parentAlias) {
            if (alias.Length <= parentAlias.Length) return null;
            var sub = alias.Substring(parentAlias.Length);
            if (!sub.StartsWith("/")) return null;
            return sub.TrimStart('/').Replace("/","_");
        }

        static string GetShortAlias(string alias, int deep)
        {
            var als = alias.Split('/');
            var result = string.Empty;
            for (var i = deep; i < als.Length; i++) {
                if (result != string.Empty) result += "_";
                result += als[i];
            }
            return result;
        }

        public void SavePackageDefination(PackageDefination packageDefination)
        {
            var alias = packageDefination.Alias;
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("PackageDefination.Alias is required.");
            
            var dir = this.Storage.GetDirectory(alias,true);
            dir.PutText(alias + ".json",packageDefination.ToJson());
        }

        public PackageDefination GetPackageDefination(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("alias is required.");
            var dir = this.Storage.GetDirectory(alias, true);
            var jsonText = dir.GetText(alias + ".json");
            if(jsonText==null) throw new ArgumentException("Package is not existed or damaged.");
            var data = new Json.Parser().Parse(jsonText) as JObject;
            if(data==null) throw new ArgumentException("Package is damaged.");
            return new PackageDefination(data);
        }

        public IList<PackageDefination> ListPackages()
        {
            var items = this.Storage.ListItems(false, StorageTypes.Directory);
            List<PackageDefination> result = new List<PackageDefination>();
            foreach (var item in items) {
                try
                {
                    var def = this.GetPackageDefination(item.Name);
                    if (def != null) result.Add(def);
                }
                catch (ArgumentException) { }
            }
            return result;
        }

        public void SaveProccessDefination(ProccessDefination proccess)
        {
            var packageAlias = proccess.PackageAlias;
            if (string.IsNullOrWhiteSpace(packageAlias)) throw new ArgumentException("ProccessDefination.PackageAlias is required.");
            var alias = proccess.Alias;
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("ProccessDefination.Alias is required.");
            var shortAlias = GetShortAlias(alias, packageAlias);
            if(string.IsNullOrEmpty(shortAlias))    throw new ArgumentException("ProccessDefination.Alias is not correct. It should be 'packageAlias/proccessId'. ");

            var packDir = this.Storage.GetDirectory(packageAlias);
            if (packDir == null) {
                var packDef = new PackageDefination()
                {
                    Name = packageAlias,
                    Alias = packageAlias,
                    Description = "System generated"
                };
                this.SavePackageDefination(packDef);
                packDir = this.Storage.GetDirectory(packageAlias);
            }

            var dir = this.Storage.GetDirectory(alias, true);
            dir.PutText(shortAlias + ".json", proccess.ToJson());
        }

        public ProccessDefination GetProccessDefination(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("alias is required.");
            var dir = this.Storage.GetDirectory(alias, true);
            var shortAlias = GetShortAlias(alias,1);
            var jsonText = dir.GetText(shortAlias + ".json");
            if (jsonText == null) throw new ArgumentException("Proccess is not existed or damaged.");
            var data = new Json.Parser().Parse(jsonText) as JObject;
            if (data == null) throw new ArgumentException("Proccess is damaged.");
            return new ProccessDefination(data);
        }

        public IList<ProccessDefination> ListProccessDefnations(string packageAlias)
        {
            var packDir = this.Storage.GetDirectory(packageAlias);
            if (packDir == null) return null;
            var items = packDir.ListItems(false, StorageTypes.Directory);
            List<ProccessDefination> result = new List<ProccessDefination>();
            foreach (var item in items)
            {
                var proccessDir = item as IStorageDirectory;
                if (proccessDir == null) continue;
                var jsonText = proccessDir.GetText( item.Name + ".json");
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
            var alias = activityDefination.Alias;
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("ActivityDefination.Alias is required.");
            var proccessAlias = activityDefination.ProccessAlias;
            if (string.IsNullOrWhiteSpace(proccessAlias)) throw new ArgumentException("ActivityDefination.ProccessAlias is required.");
            var prcDir = this.Storage.GetDirectory(proccessAlias);
            if(prcDir==null) throw new ArgumentException("ActivityDefination.ProccessAlias is invalid.Please save the ProccessDefination first.");
            var shortAlias = GetShortAlias(alias, proccessAlias);
            if (string.IsNullOrEmpty(shortAlias)) throw new ArgumentException("ActivityDefination.Alias is not correct. It should be 'packageAlias/proccessId/activityId'. ");

            prcDir.PutText(shortAlias + ".activity.json", activityDefination.ToJson());
        }

        public ActivityDefination GetActivityDefination(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("alias is required.");
            var dir = this.Storage.GetDirectory(alias, true);
            var shortAlias = GetShortAlias(alias, 2);
            var jsonText = dir.GetText(shortAlias + ".activity.json");
            if (jsonText == null) throw new ArgumentException("Activity is not existed or damaged.");
            var data = new Json.Parser().Parse(jsonText) as JObject;
            if (data == null) throw new ArgumentException("Activity is damaged.");
            return new ActivityDefination(data);
        }

        

        public void SaveTransactionDefination(TransactionDefination transactionDefination)
        {
            var alias = transactionDefination.Alias;
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("TransactionDefination.Alias is required.");
            var proccessAlias = transactionDefination.ProccessAlias;
            if (string.IsNullOrWhiteSpace(proccessAlias)) throw new ArgumentException("TransactionDefination.ProccessAlias is required.");
            var prcDir = this.Storage.GetDirectory(proccessAlias);
            if (prcDir == null) throw new ArgumentException("TransactionDefination.ProccessAlias is invalid.Please save the ProccessDefination first.");
            var shortAlias = GetShortAlias(alias, proccessAlias);
            if (string.IsNullOrEmpty(shortAlias)) throw new ArgumentException("TransactionDefination.Alias is not correct. It should be 'packageAlias/proccessId/activityId'. ");

            prcDir.PutText(shortAlias + ".transaction.json", transactionDefination.ToJson());
        }

        public TransactionDefination GetTransactionDefination(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException("alias is required.");
            var dir = this.Storage.GetDirectory(alias, true);
            var shortAlias = GetShortAlias(alias, 2);
            var jsonText = dir.GetText(shortAlias + ".transaction.json");
            if (jsonText == null) throw new ArgumentException("Transaction is not existed or damaged.");
            var data = new Json.Parser().Parse(jsonText) as JObject;
            if (data == null) throw new ArgumentException("Transaction is damaged.");
            return new TransactionDefination(data);
        }

        public void SaveExecutionDefination(ExecutionDefination executionDefination)
        {

            var activityDefination = executionDefination as ActivityDefination;
            if (activityDefination != null) this.SaveActivityDefination(activityDefination);
            var transactionDefination = executionDefination as TransactionDefination;
            if (activityDefination != null) this.SaveTransactionDefination(transactionDefination);
            
        }

        public IList<ExecutionDefination> ListExecutionDefinations(string proccessAlias)
        {
            var pcrDir = this.Storage.GetDirectory(proccessAlias);
            if (pcrDir == null) return null;
            var items = pcrDir.ListItems(false, StorageTypes.Directory);
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
        
    }
}
