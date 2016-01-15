using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public interface IInitializable
    {
        void Initialize(object intialParameter=null);
    }
}
