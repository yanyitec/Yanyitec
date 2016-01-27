


namespace Yanyitec.Runtime
{
    using System;
    using System.IO;
    using System.Reflection;
#if DNXCORE50
    using System.Runtime.Loader;
#endif
#if DNXCORE50
    class InternalLoader : System.Runtime.Loader.AssemblyLoadContext{
            public Assembly Load(Stream stream,Stream pdn = null){
                return base.LoadFromStream(stream);
            }
            protected override Assembly Load(AssemblyName assemblyName)
            {
                throw new NotImplementedException();
            }
        }
    public static class AssemblyLoader
    {
        readonly static InternalLoader _loader = new InternalLoader(); 
        public static Assembly Load(Stream assembly, Stream pdb = null){
            return _loader.Load(assembly);
        }
        public const bool CanLoadPDB = false;
    }
#else
    public static class AssemblyLoader
    {
        public static Assembly Load(Stream assembly, Stream pdb = null)
        {
            var mainBytes = GetStreamAsByteArray(assembly);
            byte[] pdbBytes = null;
            if (pdb != null) pdbBytes = GetStreamAsByteArray(pdb);
            return Assembly.Load(mainBytes, pdbBytes);
        }
        public const bool CanLoadPDB = true;
        private static byte[] GetStreamAsByteArray(Stream stream)
        {
            // Fast path assuming the stream is a memory stream
            var ms = stream as MemoryStream;
            if (ms != null)
            {
                return ms.ToArray();
            }

            // Otherwise copy the bytes
            using (ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
#endif

}
