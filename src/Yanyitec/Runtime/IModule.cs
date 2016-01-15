using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Storaging;

namespace Yanyitec.Runtime
{
    public interface IModule: IInitializable
    {
        object this[string key] { get; set; }
        IModule Container { get; }
        
        string Name { get; }

        bool RunMain(object parameters);
    }
}
