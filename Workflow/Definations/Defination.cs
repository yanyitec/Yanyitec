using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class Defination
    {
        public Guid DefinationId { get; set; }
        
        public string Type {
            get; set;
        }

        public Guid RuntimeId { get; set; }

        public Guid ContainerId { get; set; }

        public Guid ProcessId { get; set; }
        
        public string Alias { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public ActivityStates _State { get; set; }

        public ExecutionStates _ExecutionState { get; set; }

        public IDictionary<string, string> Extras { get; set; }
        
        public double _x { get; set; }
        public double _y { get; set; }

        public double _width { get; set; }

        public double _height { get; set; }

        public virtual Defination CloneTo(Defination entity=null) {
            if (entity == null) entity = new Defination();
            entity.Alias = this.Alias;
            entity.Type = this.Type;
            entity.ContainerId = this.ContainerId;
            entity.ProcessId = this.ProcessId;
            entity.DefinationId = this.DefinationId;
            entity.RuntimeId = Guid.NewGuid();
            entity._State = this._State;
            entity._ExecutionState = this._ExecutionState;
            entity._height = this._height;
            entity._width = this._width;
            entity._x = this._x;
            entity._y = this._y;
            if (this.Extras != null) {
                if (entity.Extras == null) entity.Extras = new Dictionary<string, string>();
                foreach (var pair in this.Extras) {
                    entity.Extras[pair.Key] = pair.Value;
                }
            }
            return entity;
        }
        
        protected virtual void ToJson(ObjectBuilder builder) {
            builder.Member("Type",this.Type??this.GetType().FullName);
            builder.Member("DefinationId",this.DefinationId.ToString());
            builder.Member("RuntimeId", this.RuntimeId.ToString());
            builder.Member("ContainerId", this.ContainerId.ToString());
            builder.Member("ProcessId", this.ProcessId.ToString());
            builder.Member("Alias", this.Alias);
            builder.Member("Name", this.Name);
            builder.Member("Description", this.Description);
            builder.Member("_State", Enum.GetName(typeof(ActivityStates), this._State));
            builder.Member("_ExecutionState", Enum.GetName(typeof(ExecutionStates), this._ExecutionState));
            builder.Member("_x", this._x);
            builder.Member("_y", this._y);
            builder.Member("_width", this._width);
            builder.Member("_height", this._height);
            if (this.Extras != null) {
                using (var extra = builder.ObjectMember("Extras")) {
                    foreach (var pair in this.Extras) {
                        extra.Member(pair.Key,pair.Value);
                    }
                }
            }
        }

        public string ToJson() {
            using (System.IO.StringWriter writer = new System.IO.StringWriter()) {
                using (var builder = new ObjectBuilder(writer, "")) {
                    ToJson(builder);
                }
                return writer.ToString();
            }
        }

        public static Defination FromJson(JObject data,Defination entity =null) {
            if (entity == null) entity = new Defination();
            entity.DefinationId = new Guid(data["DefinationId"].ToString());

            entity.RuntimeId = new Guid(data["RuntimeId"].ToString());
            entity.ContainerId = new Guid(data["ContainerId"]);
            entity.ProcessId = new Guid(data["ProcessId"]);
            entity.Alias = data["Alias"].ToString();
            entity.Type = data["Type"].ToString();
            entity.Name = data["Name"].ToString();
            entity.Description = data["Description"]?.ToString();

            entity._State = (ActivityStates)Enum.Parse(typeof(ActivityStates), data["_State"].ToString());
            entity._ExecutionState = (ExecutionStates)Enum.Parse(typeof(ExecutionStates), data["_ExecutionState"].ToString());

            entity._x = double.Parse(data["_x"]);
            entity._y = double.Parse(data["_y"]);
            entity._width = double.Parse(data["_width"]);
            entity._height = double.Parse(data["_height"]);
            var extra = data["Extra"] as JObject;
            if (extra != null) {
                entity.Extras = new Dictionary<string, string>();
                foreach (var pair in extra) {
                    entity.Extras.Add(pair.Key,pair.Value.ToString());
                }
            }
            return entity;
        }

        

    }
}
