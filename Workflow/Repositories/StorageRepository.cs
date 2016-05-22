using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;
using Yanyitec.Storaging;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow.Repositories
{
    public class StorageRepository : IRepository
    {
        public StorageRepository(string defineDir, string rtDir) {
            this.DefinationStorage = new Storage(defineDir);
            this.RuntimeStorage = new Storage(rtDir);
        }
        public IStorage DefinationStorage { get; private set; }
        
        public IStorage RuntimeStorage { get; private set; }

        public void SaveDefination(Defination defination) {
            var data = defination.ToJson();
            DefinationStorage.PutText(defination.DefinationId.ToString() + ".json",data);
        }
        
        public Defination GetDefination(Guid id)
        {
            var json = DefinationStorage.GetText(id.ToString() + ".json");
            if (json == null) return null;
            var data = new Json.Parser().Parse(json) as JObject;
            if (data == null) return null;
            return GetDefination(data);
        }
        

        public Guid CreateProcess( Guid activityDefinationId, string description)
        {
            var id = Guid.NewGuid();
            var dir = this.RuntimeStorage.CreateItem(id.ToString()) as IStorageDirectory;
            dir.CreateItem("activites");
            var define = new ProcessDefination();
            define.Id = id;
            define.ActivityDefinationId = activityDefinationId;
            define.Description = description;
            var json = define.ToJson();
            dir.PutText("process.json",json);
            return id;
        }

        public void SaveProcess(ProcessDefination data) {
            var dir = this.RuntimeStorage.GetDirectory(data.Id.ToString(),true) as IStorageDirectory;
            var json = data.ToJson();
            dir.PutText("process.json", json);
        }

        public Defination GetActivity(Guid runtimeId, Guid processId)
        {
            var dir = this.RuntimeStorage.GetDirectory(processId.ToString() + "/activities", false) as IStorageDirectory;
            if (dir == null) return null;
            var json = dir.GetText(runtimeId.ToString() + ".json");
            if (json == null) return null;
            var data = new Json.Parser().Parse(json) as JObject;
            return this.GetDefination(data) ;
        }

        public Defination GetActivity(Guid runtimeId)
        {
            var dirs = this.RuntimeStorage.ListItems(false,StorageTypes.Directory);
            foreach (var dir in dirs) {
                Guid processId = Guid.Empty;
                if (Guid.TryParse(dir.Name, out processId)) {
                    var result = this.GetActivity(runtimeId,processId);
                    if (result != null) return result;
                }
                
            }
            return null;
            
        }

        public bool SaveActivity(Defination data)
        {
            var proccessId = data.ProcessId;
            if (proccessId == Guid.Empty) return false;
            var runtimeId = data.RuntimeId;
            if (runtimeId == Guid.Empty) return false;
            var json = data.ToJson();
            this.RuntimeStorage.PutText(proccessId.ToString() + "/activities/" + runtimeId.ToString() + ".json",json);
            return true;
        }


        Defination GetDefination(JObject data)
        {
            var type = data["Type"];
            if (type == "Yanyitec.Workflow.Definations.TransactionDefination")
            {
                return TransactionDefination.FromJson(data);
            }
            else if (type == "Yanyitec.Workflow.Definations.ActivityDefination")
            {
                return ActivityDefination.FromJson(data);
            }
            else if (type == "Yanyitec.Workflow.Definations.BlockDefination")
            {
                return BlockDefination.FromJson(data);
            }
            throw new NotSupportedException();
        }
    }
}
