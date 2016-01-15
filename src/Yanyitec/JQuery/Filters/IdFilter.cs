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
        public class IdFilter : JQuery.Filter
        {
            public IdFilter(string id)
            {
                this.ElementId = id;
            }
            public string ElementId { get; private set; }
            public override IHtmlElement Check(IHtmlElement elem, IHtmlElement root = null)
            {
                
                return elem.Id == this.ElementId ? elem : null;

            }

            public override string Expression
            {
                get
                {
                    return "#" + this.ElementId;
                }
            }

        }
    }

}
