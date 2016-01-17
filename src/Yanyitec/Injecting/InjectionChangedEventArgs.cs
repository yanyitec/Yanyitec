using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Injecting
{
    public class InjectionChangedEventArgs : EventArgs
    {
        public InjectionChangedEventArgs(bool locked) { this.Locked = locked; }

        public bool Locked { get; set; }
    }
}
