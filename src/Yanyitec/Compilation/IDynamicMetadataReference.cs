using Yanyitec.Storaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Compilation
{
    public interface IDynamicMetadataReference : IMetadataReference
    {
        IStorage Storage { get; }
        
    }
}
