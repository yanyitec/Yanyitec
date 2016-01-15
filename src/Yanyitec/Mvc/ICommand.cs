using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Mvc
{
    public interface ICommand
    {
        object Execute(object controller, IInputData inputData);

        HttpMethods AcceptMethods { get; }
    }
}
