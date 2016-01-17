using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Storaging;

namespace Yanyitec.Runtime
{
    public interface IModule
    {
        object this[string key] { get; set; }
        IHost Host { get; }
        
        string Name { get; }

        bool RunMain(object parameters);
    }
}
