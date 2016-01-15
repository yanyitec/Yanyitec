using Yanyitec.Storaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    public interface IProjectCompiler
    {
        

        Assembly Compile(IDirectory Location);
    }
}
