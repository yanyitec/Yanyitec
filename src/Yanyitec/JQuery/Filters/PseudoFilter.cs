using Yanyitec.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec
{
    public partial class JQuery
    {
        /// <summary>
        /// div
        /// </summary>
        public abstract class PseudoFilter : JQuery.Filter
        {
            protected PseudoFilter(JQuery.Filter attachedFilter)
            {
                this.AttachedFilter = attachedFilter;
            }
            public JQuery.Filter AttachedFilter { get; private set; }


        }
    }

}
