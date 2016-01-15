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
        public abstract class PseudoChildFilter : JQuery.PseudoFilter
        {
            protected PseudoChildFilter(JQuery.Filter attachedFilter) : base(attachedFilter) { }

        }
    }

}
