using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Yanyitec.Compilation
{
    using Yanyitec.Runtime;
    public interface IMetadataReference : IEquatable<IMetadataReference>
    {
        event Action<DynamicChangeTypes> OnChange;
        IArtifact Artifact { get; }
    }
}
