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
        public class MultFilter : JQuery.Filter
        {
            public MultFilter(IList<JQuery.Filter> filters = null)
            {
                this.Filters = filters ?? new List<JQuery.Filter>();
            }
            public IList<JQuery.Filter> Filters { get; private set; }
            public override IHtmlElement Check(IHtmlElement elem, IHtmlElement root = null)
            {
                foreach (var filter in Filters) {
                    var result = filter.Check(elem,root);
                    if (result == null) return null;
                }
                return elem;
            }

            public override string Expression
            {
                get
                {
                    string result = string.Empty;
                    foreach (var f in this.Filters) {
                        result += f.Expression;
                    }
                    return result;
                }
            }
        }
    }

}
