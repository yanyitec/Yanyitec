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
        public class AttributeFilter : JQuery.Filter
        {
            public AttributeFilter(string attrName,string value)
            {
                this.AttributeName = attrName.ToLower();
                this.AttributeValue = AttributeValue = value;
            }
            public string AttributeName { get; private set; }
            public string AttributeValue { get; private set; }
            public override IHtmlElement Check(IHtmlElement elem,IHtmlElement root = null)
            {
                var attr = elem.GetAttribute(this.AttributeName);
                if (attr == null) return null;
                if (this.AttributeValue != null) {
                    return this.AttributeValue == attr?elem:null;
                }
                return elem;
            }

            public override string Expression
            {
                get
                {
                    if (this.AttributeValue != null)
                    {
                        return "[" + this.AttributeName + "=\"" + this.AttributeValue + "\"]";
                    }
                    else
                    {
                        return "[" + this.AttributeName + "]";
                    }
                }
            }
            
        }
    }

}
