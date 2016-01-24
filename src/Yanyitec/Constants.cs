using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public static class Constants
    {
        public const string UTF8Codepage = "utf-8";

        public const string DefaultCodepage = UTF8Codepage;

        public readonly static int IntSize = BitConverter.GetBytes(0).Length;

        public readonly static int DoubleSize = BitConverter.GetBytes((double)0.1).Length;

        public readonly static int GuidSize = Guid.Empty.ToByteArray().Length;

        public readonly static Task EmptyTask = new Task(()=> { });

    }
}
