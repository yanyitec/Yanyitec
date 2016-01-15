using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public static class Arch
    {
        public enum ArchTypes {
            X86 = 32,
            AMD64 = 64
        }

        public static ArchTypes GetArch() {
            return (ArchTypes) IntPtr.Size;
        } 
    }
}
