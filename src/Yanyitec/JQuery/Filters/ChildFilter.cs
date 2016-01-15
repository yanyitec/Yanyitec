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
        public class ChildFilter : JQuery.Filter
        {
            public ChildFilter(IList<JQuery.Filter> parentFilters=null)
            {
                this.ParentFilters = parentFilters ?? new List<JQuery.Filter>();
            }
            public IList<JQuery.Filter> ParentFilters { get; private set; }
            public override IHtmlElement Check(IHtmlElement elem, IHtmlElement root = null)
            {
                if (elem.ParentNode == null) return null;
                if (elem == root) return null;
                var p = elem;
                
                for (var at = ParentFilters.Count - 1; at >= 0; at--) {
                    if (ParentFilters[at].Check(p)==null) return null;
                    p = p.ParentNode;
                    if (elem.ParentNode == null) return null;
                }
                
                return p;
            }

            public override string Expression
            {
                get
                {
                    var result = string.Empty;
                    foreach (var pa in this.ParentFilters)
                    {
                        if (result != string.Empty) result += ">";
                        result +=  pa.Expression;
                    }
                    return result;
                }
            }

        }
    }

}
