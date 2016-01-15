using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Mvc.ParameterBinders
{
    public interface IParameterBinder
    {
        object GetValue(string name,IInputData inputData);
    }
}
