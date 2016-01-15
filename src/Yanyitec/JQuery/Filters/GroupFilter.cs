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
        public class GroupFilter : JQuery.Filter
        {
            public GroupFilter(params JQuery.Filter[] filters)
            {
                this.Filters = new List<JQuery.Filter>(filters);
            }
            public IList<JQuery.Filter> Filters { get; private set; }
            public override IHtmlElement Check(IHtmlElement elem, IHtmlElement root = null)
            {
                foreach (var filter in this.Filters) {
                    var next = filter.Check(elem, root);
                    return next != null ? elem : null;
                }
                return null;

            }

            public override string Expression
            {
                get
                {
                    var result = string.Empty;
                    foreach (var pa in this.Filters)
                    {
                        if (result != string.Empty) result += ",";
                        result += pa.Expression;
                    }
                    return result;
                }
            }
        }
    }

}
