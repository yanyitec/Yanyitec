using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow.Infos
{
    public class ParallelActivityInfo : ActivityInfo
    {
        public ParallelActivityInfo(Guid proccessId) : base(proccessId) { }

        public ParallelActivityInfo(Guid proccessId, ParallelActivityDefination defination):base(proccessId,defination) {

        }

        public ParallelActivityInfo(JObject data) :base(data){
            var parallels = data["ParallelActivities"] as JArray;
            if (parallels != null) {
                foreach (var idstr in parallels) {
                    this._parallelActivities.Add(new Guid(idstr));
                }
            }

        }

        protected override void ToJson(ObjectBuilder builder)
        {
            base.ToJson(builder);
            if (this._parallelActivities.Count > 0) {
                using (var ids = builder.ArrayMember("ParallelActivities")) {
                    foreach (var id in this._parallelActivities) {
                        ids.Add(id);
                    }
                }
            }
        }

        readonly List<Guid> _parallelActivities = new List<Guid>();
        public IList<Guid> ParallelActivities { get { return _parallelActivities; } }
    }
}
