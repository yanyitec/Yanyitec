using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    public class ArtifactLoaderOptions
    {
        public ArtifactLoaderOptions() { }

        public ArtifactLoaderOptions(Json.JToken conf,bool defaultVersion=true) {
            if (conf.ValueType == Json.ValueType.String)
            {
                if(defaultVersion) this.Version = conf.ToString();
                else this.Assembly = conf.ToString();
            }
            else
            {
                var config = conf as Json.JObject;
                if (config["assembly"] != null) this.Assembly = config["assembly"].ToString();
                if (config["version"] != null) this.Version = config["version"].ToString();
                if (config["pdb"] != null) this.Version = config["pdb"].ToString();
            }
        }

        public string Name { get; set; }
        public string Version { get; set; }

        public string Assembly { get; set; }

        public string PDB { get; set; }

    }
}
