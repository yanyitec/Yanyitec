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
        public Defination() {
            
        }
        public Defination(Defination other) {
            this.Alias = other.Alias;
            
            foreach (var pair in other._extras)
            {
                this._extras[pair.Key] = pair.Value;
            }

            
        }

        public Defination(JObject data) {
            
            this.Alias = data["Alias"]?.ToString();

            this.Name = data["Name"]?.ToString();
            this.Description = data["Description"]?.ToString();
            

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
            if(!string.IsNullOrEmpty(this.Alias))builder.Member("Alias", this.Alias);
            
            
            if (!string.IsNullOrEmpty(this.Name))builder.Member("Name", this.Name);
            if(!string.IsNullOrEmpty(this.Description))builder.Member("Description", this.Description);

            if (this._extras.Count > 0) {
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
        public string Alias { get; set; }

        
        public string Name { get; set; }

        public string Description { get; set; }

        readonly Dictionary<string, string> _extras = new Dictionary<string, string>();

        public IDictionary<string, string> Extras { get { return _extras; } }

        

        

        public double _x {
            get {
                string text = null;
                if (_extras.TryGetValue("_x", out text)) {
                    double value = double.NaN;
                    if (double.TryParse(text, out value)) return value;
                }
                return double.NaN;
            }
            set
            {
                if (_extras.ContainsKey("_x")) Extras["_x"] = value.ToString();
                else Extras["_x"] = value.ToString();
            }
        }
        public double _y
        {
            get
            {
                string text = null;
                if (_extras.TryGetValue("_y", out text))
                {
                    double value = double.NaN;
                    if (double.TryParse(text, out value)) return value;
                }
                return double.NaN;
            }
            set
            {
                if (_extras.ContainsKey("_y")) _extras["_y"] = value.ToString();
                else _extras["_y"] = value.ToString();
            }
        }

        public double _width
        {
            get
            {
                string text = null;
                if (_extras.TryGetValue("_width", out text))
                {
                    double value = double.NaN;
                    if (double.TryParse(text, out value)) return value;
                }
                return double.NaN;
            }
            set
            {
                if (_extras.ContainsKey("_width")) _extras["_width"] = value.ToString();
                else _extras["_width"] = value.ToString();
            }
        }

        public double _height
        {
            get
            {
                string text = null;
                if (_extras.TryGetValue("_height", out text))
                {
                    double value = double.NaN;
                    if (double.TryParse(text, out value)) return value;
                }
                return double.NaN;
            }
            set
            {
                if (_extras.ContainsKey("_height")) _extras["_height"] = value.ToString();
                else _extras["_height"] = value.ToString();
            }
        }

        

        

        



    }
}
