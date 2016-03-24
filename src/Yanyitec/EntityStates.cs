using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public enum EntityStates
    {
        Undefined = 0,
        Created = 1,
        Modified = 2,
        Deleted = -1,
        Disabled = -2
    }
}
