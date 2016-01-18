using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Compilation
{
    public interface ICSScript
    {
        Type ReturnType { get; }

        Type ContextType { get; }
    }
}
