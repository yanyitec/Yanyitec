using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class ProcessDefination 
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid ActivityDefinationId { get; set; }

        public Guid ActivityId { get; set; }

        public IList<Guid> ActivedActivityIds { get; set; }

        public IDictionary<string, string> Extras { get; set; }

        protected void ToJson(ObjectBuilder builder) {
            builder.Member("Id",this.Id);
            builder.Member("ActivityDefinationId",this.ActivityDefinationId);
            builder.Member("ActivityId", this.ActivityId);
            builder.Member("Description",this.Description);
            if (this.ActivedActivityIds != null) {
                using (var arr = builder.ArrayMember("ActivedActivityIds")) {
                    foreach (var item in this.ActivedActivityIds) arr.Add(item);
                }
            }
            if (this.Extras != null)
            {
                using (var extra = builder.ObjectMember("Extras"))
                {
                    foreach (var pair in this.Extras)
                    {
                        extra.Member(pair.Key, pair.Value);
                    }
                }
            }
        }

        public string ToJson() {
            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                using (var builder = new ObjectBuilder(writer, ""))
                {
                    ToJson(builder);
                }
                return writer.ToString();
            }
        }

        public static ProcessDefination FromJson(JObject data) {
            var entity = new ProcessDefination();
            entity.Id = new Guid(data["Id"].ToString());
            entity.ActivityDefinationId = new Guid(data["ActivityDefinationId"].ToString());
            entity.ActivityId = new Guid(data["ActivityId"].ToString());
            entity.Description = data["Description"].ToString();
            var arr = data["ActivedActivityIds"] as JArray;
            if (arr != null) {
                entity.ActivedActivityIds = new List<Guid>();
                foreach (JToken item in arr) {
                    entity.ActivedActivityIds.Add(new Guid(item.ToString()));
                }
            }
            var extra = data["Extra"] as JObject;
            if (extra != null)
            {
                entity.Extras = new Dictionary<string, string>();
                foreach (var pair in extra)
                {
                    entity.Extras.Add(pair.Key, pair.Value.ToString());
                }
            }

            return entity;
        }
    }
}
