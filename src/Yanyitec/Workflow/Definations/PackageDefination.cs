using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;

namespace Yanyitec.Workflow.Definations
{
    public class PackageDefination : Defination
    {
        public PackageDefination() { }
        public PackageDefination(PackageDefination other) : base(other)
        {
            
        }

        public PackageDefination(JObject data) : base(data)
        {
        }

    }
}
