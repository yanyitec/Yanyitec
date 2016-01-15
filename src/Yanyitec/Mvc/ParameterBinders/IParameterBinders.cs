using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Mvc.ParameterBinders
{
    public interface IParameterBinders
    {
        IParameterBinder GetOrCreateBinder(Type type);
    }
}
