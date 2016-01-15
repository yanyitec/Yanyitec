using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Dom
{
    public static class HtmlElementCollectionExtensions 
    {
        

        public static void Walkthough(this IHtmlElement self , Func<IHtmlElement, bool> callback) {
            
            for (var i = 0; i < self.ChildNodes.Length; i++)
            {
                var sub = self.ChildNodes[i];
                if (callback(sub) == false) return;
                if(sub.ChildNodes.Length>0)
                    sub.Walkthough(callback);

            }
        }

        
    }
}
