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
        /// div .error
        /// descendant selector
        /// </summary>
        public class DescendantFilter : JQuery.Filter
        {
            public DescendantFilter(IList<JQuery.Filter> ascendantFilters =null)
            {
                this.AscendantFilters = ascendantFilters?? new List<JQuery.Filter>();
            }
            public IList<JQuery.Filter> AscendantFilters { get; private set; }

            public override IHtmlElement Check(IHtmlElement elem, IHtmlElement root = null)
            {
                if (elem == root) return null;
                if (AscendantFilters.Count == 0) throw new ArgumentException("Ascendant Filter is required.");
                var at = AscendantFilters.Count-1;
               
                
                var filter = AscendantFilters[at];
                if (filter.Check(elem, root) == null || --at<0) return null;
                filter = AscendantFilters[at];
                var p = elem.ParentNode;
                while (p != null)
                {
                    
                    var checkResult = filter.Check(p,root);
                    if (checkResult != null) {
                        
                        if (--at < 0) return elem;
                        p = checkResult.ParentNode;
                        continue;
                    }
                    if (p == root) break;
                    p = p.ParentNode;  
                      
                }
                return null;
            }

            public override string Expression
            {
                get
                {
                    var result = string.Empty;
                    foreach (var pa in this.AscendantFilters)
                    {
                        if (result != string.Empty) result += " ";
                        result += pa.Expression;
                    }
                    return result;
                }
            }
        }
    }

}
