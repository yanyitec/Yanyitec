using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Compilation
{
    interface ICSScript<TContext,TReturn>
    {
        TReturn Execute(TContext context, object args);
    }
}
