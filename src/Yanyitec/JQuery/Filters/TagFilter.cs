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
        public class TagFilter : JQuery.Filter
        {
            public TagFilter(string tagName) {
                this.TagName = tagName.ToUpper();
            }
            public string TagName { get; private set; }
            public override IHtmlElement Check(IHtmlElement elem, IHtmlElement root=null)
            {
                return elem.TagName == this.TagName? elem :null;

            }

            public override string Expression
            {
                get
                {
                    return this.TagName;
                }
            }

        }
    }

}
