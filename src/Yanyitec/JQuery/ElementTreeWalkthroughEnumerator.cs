//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Yanyitec
//{
//    public partial class JQuery {
//        public class ElementTreeWalkthroughEnumerator : IEnumerator<JQuery.IHtmlElement>
//        {
//            class Location {
//                public JQuery.IHtmlElementCollection ChildList { get; set; }
//                public int At { get; set; }
//            }
//            System.Collections.Generic.Stack<Location> Locations;

//            public ElementTreeWalkthroughEnumerator(JQuery.IHtmlElement elem) {
//                this.Root = elem;
//                this.Locations = new Stack<Location>();
//            }

//            public JQuery.IHtmlElement Root { get; private set; }
//            public IHtmlElement Current
//            {
//                get; private set;
//            }

//            object IEnumerator.Current
//            {
//                get
//                {
//                    return this.Current;
//                }
//            }

//            public void Dispose()
//            {
                
//            }

//            public bool MoveNext()
//            {
//                if (this.Current == null) {
//                    this.Current = this.Root;
//                    return true;
//                }
//                if (Current == this.Root) {
//                    if (!this.Root.HasChildren) return false;
//                    var loc = new Location() {

//                    };
//                }
//            }

//            public void Reset()
//            {
//                throw new NotImplementedException();
//            }
//        }
//    }
    
//}
