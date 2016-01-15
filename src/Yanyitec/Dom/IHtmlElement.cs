using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Dom
{
    public interface IHtmlElement :IEquatable<IHtmlElement>
    {
        string ToString();
        string Id { get; set; }
        string TagName { get;  }

        string ClassName { get; set; }

        string GetAttribute(string name);

        void SetAttribute(string name, string value);

        void RemoveAttribute(string name);
        IHtmlElement ParentNode { get; }

        IHtmlElement NextSibling { get; }

        IHtmlElement PreviousSibling { get; }

        IHtmlElementCollection ChildNodes { get; }

        bool HasChildren { get; }

        string InnerHTML { get; }

        void AppendChild(IHtmlElement child);
    }

}
