using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public interface ITypeProvider
    {
        Type GetType(string name);

        Type GetTypes(IEnumerable<string> memberNames);
    }
}
