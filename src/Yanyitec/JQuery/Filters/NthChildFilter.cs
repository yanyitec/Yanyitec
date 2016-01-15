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
        public class NthChildFilter  : PseudoChildFilter
        {
            public NthChildFilter(JQuery.Filter attachedFilter , string n):base(attachedFilter)
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
                if (NumberOfNth != 0) {
                    var p = elem.ParentNode;
                    if (p == null) return null;
                    if (p.ChildNodes.Length < NumberOfNth) return null;
                    var chkResult = this.AttachedFilter.Check(elem);
                    return chkResult!=null && p.ChildNodes[NumberOfNth-1].Equals(elem) ? elem : null;
                }
                throw new NotImplementedException();

            }

            public override string Expression
            {
                get
                {
                    return this.AttachedFilter.Expression + ":nth-child(" + this.Nth + ")";
                }
            }

        }
    }

}
