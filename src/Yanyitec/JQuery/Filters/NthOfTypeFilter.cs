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
        public class NthOfTypeFilter : PseudoChildFilter
        {
            public NthOfTypeFilter(JQuery.Filter attacheFilter, string n):base(attacheFilter)
            {
                this.Nth = n;
                int nnth = 0;
                int.TryParse(n, out nnth);
                this.NumberOfNth = nnth;
            }
            public string Nth { get; private set; }

            public int NumberOfNth { get; private set; }
            public override IHtmlElement Check(IHtmlElement elem, IHtmlElement root = null)
            {
                if (root == elem) return null;
                if (NumberOfNth != 0)
                {
                    var p = elem.ParentNode;
                    if (p == null) return null;
                    var children = p.ChildNodes;
                    if (children.Length < NumberOfNth) return null;
                    var matchCount = 0;
                    foreach (var child in children) {
                        if (this.AttachedFilter.Check(child) != null) {
                            if (++matchCount == NumberOfNth) return elem;
                        }
                    }
                    return null;
                }
                throw new NotImplementedException();

            }

            public override string Expression
            {
                get
                {
                    return this.AttachedFilter.Expression + ":nth-of-type(" + this.Nth + ")";
                }
            }

        }
    }

}
