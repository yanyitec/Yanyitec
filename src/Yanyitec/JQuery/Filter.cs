using Yanyitec.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec
{
    public partial class JQuery {
        public abstract class Filter
        {
            public IEnumerable<IHtmlElement> Apply(IEnumerable<IHtmlElement> elems, IHtmlElement root = null) {
                var result = new List<IHtmlElement>();
                foreach (var elem in elems) {
                    if (this.Check(elem,root)!=null) result.Add(elem);
                }
                return result;
            }
            /// <summary>
            /// 检查是否符合要求
            /// </summary>
            /// <param name="elem">要检查的元素</param>
            /// <param name="root">根元素context，检查到这里为止。如果是null就表示检查整个document</param>
            /// <returns>下一次检查的元素，null表示检查失败，不用再检查</returns>
            public abstract IHtmlElement Check(IHtmlElement elem,IHtmlElement root = null);

            public override string ToString()
            {
                return "Yanyitec.JQuery.Filter,Expression=" + this.Expression;
            }

            public abstract string Expression { get; }
        }
    }
    
}
