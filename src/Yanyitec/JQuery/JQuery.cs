using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Yanyitec.Dom;

namespace Yanyitec
{
    public partial class JQuery : IEnumerable<IHtmlElement>
    {
        internal protected IList<IHtmlElement> InnerList { get; set; }
        public static readonly Regex QuickRegExpr = new Regex(@"^(?:\s*(<[\w\W]+>)[^>]*|#([\w-]*))$");

        public Selector _Selector { get; private set; }

        #region constructors
        public JQuery()
        {
            this.InnerList = new List<IHtmlElement>();
        }

        public JQuery(IList<IHtmlElement> data, Selector selector) {
            this.InnerList = data;
            this._Selector = selector;
        }

        public JQuery(JQuery other)
        {
            this.InnerList = new List<IHtmlElement>(other.InnerList);

        }

        public JQuery(params IHtmlElement[] elems)
        {
            this.InnerList = new List<IHtmlElement>(elems);
        }

        public static JQuery Query(string selectorExpression, JQuery context) {
            var selector = Selector.GetOrCreate(selectorExpression);
            return new JQuery(selector.Apply(context), selector);
        }

        public static JQuery Query(string selectorExpression, IHtmlElement context)
        {
            var selector = Selector.GetOrCreate(selectorExpression);
            return new JQuery(selector.Apply(context), selector);
        }

        public static JQuery Query(string selectorExpression, IHtmlDocument context)
        {
            var selector = Selector.GetOrCreate(selectorExpression);
            return new JQuery(selector.Apply(context.DocumentElement), selector);
        }

        //public JQuery(string selector, IHtmlElement context) : this(selector,new JQuery(context)){ }




        #endregion

        public override string ToString()
        {
            return this._Selector == null ? "Yanyitec.JQuery,Length=" + this.InnerList.Count.ToString() : "Yanyitec.JQuery, Selector=" + this._Selector.Expression + ", Length=" + this.Length.ToString();
        }

        #region enumeratable

        public int Length {
            get { return this.InnerList.Count; } }
        public IHtmlElement this[int index]
        {
            get
            {
                if (index < this.InnerList.Count) return InnerList[index];
                return null;
            }
            set
            {
                var c = index - this.InnerList.Count;
                for (var i = 0; i < c; i++)
                {
                    this.InnerList.Add(null);
                }
                this.InnerList[index] = value;
            }
        }

        public IEnumerator<IHtmlElement> GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }
        #endregion

        #region operations

        #region attr
        public string Attr(string name) {
            if (this.InnerList.Count == 0) return null;
            foreach (var elem in this.InnerList) {
                if (elem != null) {
                    return elem.GetAttribute(name);
                }
            }
            return null;
        }

        public JQuery Attr(string name, object value) {
            if (this.InnerList.Count == 0) return this;
            var valstr = value==null?string.Empty: value.ToString();
            foreach (var elem in this.InnerList)
            {
                if (elem != null)
                {
                    elem.SetAttribute(name,valstr);
                }
            }
            return this;
        }

        public JQuery RemoveAttr(string name)
        {
            if (this.InnerList.Count == 0) return this;
            
            foreach (var elem in this.InnerList)
            {
                if (elem != null)
                {
                    elem.RemoveAttribute(name);
                }
            }
            return this;
        }
        #endregion


        #endregion
    }
}
