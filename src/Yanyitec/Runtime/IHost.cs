using Yanyitec.Storaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    public interface IHost : IModule
    {
        IModule GetModule(string name);
    }
}
