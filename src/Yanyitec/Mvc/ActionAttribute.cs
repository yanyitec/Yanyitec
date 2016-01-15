using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Mvc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : Attribute
    {
        public ActionAttribute(string Name=null) { this.ActionName = Name; }

        public string ActionName { get; private set; }
    }
}
