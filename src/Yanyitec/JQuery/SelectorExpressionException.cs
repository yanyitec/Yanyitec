using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec
{
    public partial class JQuery {
        public class SelectorExpressionException : Exception
        {
            public SelectorExpressionException(string selector) { this.Expression = selector; }

            public string Expression { get; private set; }
        }
    }
    
}
