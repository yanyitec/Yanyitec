using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Compilation
{
    public class CompileReference: IEquatable<CompileReference>
    {
        public CompileReference(Assembly assembly) {
            this.MetadataReference = MetadataReference.CreateFromFile(assembly.Location);
        }

        public CompileReference(IArtifact artifact)
        {
            this.MetadataReference = MetadataReference.CreateFromFile(artifact.AssemblyLocation.FullName);
        }

        public CompileReference(string assembly)
        {
            this.MetadataReference = MetadataReference.CreateFromFile(assembly);
        }

        public CompileReference(Type keytype)
        {
            this.MetadataReference = MetadataReference.CreateFromFile(keytype.Assembly.Location);
        }
        public MetadataReference MetadataReference { get; private set; }

        public bool Equals(CompileReference other)
        {
            if (other == null) return false;
            return other.MetadataReference.Display == this.MetadataReference.Display;
        }
    }
}
