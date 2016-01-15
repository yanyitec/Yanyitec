using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Dom
{
    public interface IHtmlElementCollection : IEnumerable<IHtmlElement>
    {
        int Length { get; }
        IHtmlElement this[int index] { get; }
    }
}
