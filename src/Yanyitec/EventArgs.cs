using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public class EventArgs : System.EventArgs
    {
        public EventArgs() { }

        public object LockedObject { get; set; }

        public EventArgs Source { get; set; }
    }
}
