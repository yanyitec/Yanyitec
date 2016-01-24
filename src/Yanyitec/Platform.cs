using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Compilation;

namespace Yanyitec
{
    public static class Platform
    {
        public enum ArchTypes {
            X86 = 32,
            AMD64 = 64
        }

        public static ArchTypes GetArch() {
            return (ArchTypes) IntPtr.Size;
        }
        public static string DotnetVersion {
            get; private set;
        }

        public static void SetDotnetVersion(string version) {
            DotnetVersion = version;
        }
    }
}
