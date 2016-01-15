using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Hosting
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string Name) {
            this.Name = Name;
        }

        public string Name { get; private set; }
    }
}
