using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public class JoinTable
    {
        public JoinTable(string alias, ReferenceInfo info,JoinTable prev=null) {
            this.Alias = alias;
            this.ReferenceInfo = info;
            this.JoinFrom = prev;
        }
        public ReferenceInfo ReferenceInfo { get;private set; }
        public string Alias { get; private set; }

        public JoinTable JoinFrom { get; private set; }
    }
}
