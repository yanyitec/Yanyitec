using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Infos
{
    public class Info
    {
        public Info(Guid proccessId) {
            this.ProccessId = proccessId;
            this.Id = Guid.NewGuid();
        }
        public Info(Guid processId,Definations.Defination defination)
        {
            this.Id = Guid.NewGuid();
            this.ProccessId = processId;
            this.Alias = defination.Alias;
            this.InstanceType = defination.InstanceType;
            
            if (defination.Extras.Count > 0) {
                foreach (var pair in defination.Extras) {
                    this._extras.Add(pair.Key,pair.Value);
                }
            }
        }

        public Info(JObject data) {
            this.Id = new Guid(data["Id"].ToString());
            this.ProccessId = new Guid(data["ProccessId"].ToString());
            this.Alias = data["Alias"]?.ToString();
            this.InstanceType = data["InstanceType"].ToString();
            
            var extra = data["Extra"] as JObject;
            if (extra != null)
            {
                foreach (var pair in extra)
                {
                    this._extras.Add(pair.Key, pair.Value.ToString());
                }
            }
        }

        protected virtual void ToJson(ObjectBuilder builder)
        {
            
            builder.Member("Id", this.Id.ToString());
            if(!string.IsNullOrEmpty(this.Alias))builder.Member("Alias", this.Alias);
            builder.Member("InstanceType", this.InstanceType);
            builder.Member("ProccessId", this.ProccessId);
            if (this._extras.Count > 0)
            {
                using (var extra = builder.ObjectMember("Extras"))
                {
                    foreach (var pair in this._extras)
                    {
                        extra.Member(pair.Key, pair.Value);
                    }
                }
            }
        }
        public string ToJson()
        {
            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                using (var builder = new ObjectBuilder(writer, ""))
                {
                    ToJson(builder);
                }
                return writer.ToString();
            }
        }
        public Guid Id { get;  set; }

        public Guid ProccessId { get; set; }

        public string Alias { get; set; }

        public string InstanceType { get; set; }
        

        readonly Dictionary<string, string> _extras = new Dictionary<string, string>();

        public IDictionary<string, string> Extras { get { return _extras; } }
    }
}
