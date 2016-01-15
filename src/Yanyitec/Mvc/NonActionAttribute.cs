using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Mvc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NonActionAttribute : Attribute
    {
        public NonActionAttribute() { }
    }
}
