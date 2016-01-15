using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Dom
{
    public interface IHtmlDocument
    {
        IHtmlElement GetElementById(string id);

        IHtmlElement DocumentElement { get; }
    }

}
