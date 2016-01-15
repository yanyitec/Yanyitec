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
        /// .success
        /// </summary>
        public class ClassFilter : JQuery.Filter
        {
            public ClassFilter(string className)
            {
                this.ClassName = className;
            }
            public string ClassName { get; private set; }

            public override IHtmlElement Check(IHtmlElement elem,IHtmlElement root = null)
            {
                if (string.IsNullOrWhiteSpace(elem.ClassName)) return null;
                var clsName = elem.ClassName.Trim();
                if (clsName == this.ClassName)
                {
                    return elem;
                }
                var clsNames = clsName.Split(' ');

                foreach (var clsn in clsNames)
                {
                    if (clsn == this.ClassName)
                    {
                        return elem;
                    }
                }
                return null;
            }

            public override string Expression
            {
                get
                {
                    return "." + this.ClassName;
                }
            }
        }
    }

}
