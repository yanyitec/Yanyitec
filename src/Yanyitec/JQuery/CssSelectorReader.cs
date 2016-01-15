using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec
{
    public partial class JQuery {
        public class CssSelectorReader
        {
            public CssSelectorReader() { }

            public void Read(string expression) {
                char lastToken = '\0';
                int lastAt = -1;
                for (int at = 0; at < expression.Length; at++) {
                    var ch = expression[at];
                    switch (ch) {
                        case ' ':
                            MeetToken(expression,lastToken,lastAt,at);
                            lastToken = ch;lastAt = at; break;
                        case '.':
                            MeetToken(expression, lastToken, lastAt, at);
                            lastToken = ch; lastAt = at; break;
                        case '>':
                            MeetToken(expression, lastToken, lastAt, at);
                            lastToken = ch; lastAt = at; break;
                        
                    }
                }
            }

            void MeetToken(string expr, char lastToken, int lastAt, int currentAt) {
                var subexpr = expr.Substring(lastAt, currentAt - lastAt);

            }
        }
    }
    
}
