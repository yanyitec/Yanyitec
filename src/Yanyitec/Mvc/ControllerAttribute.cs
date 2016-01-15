using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Mvc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ControllerAttribute : Attribute
    {
        public ControllerAttribute(string Name=null) { this.ControllerName = Name; }

        public string ControllerName { get; private set; }
    }
}
