using Yanyitec.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yanyitec
{
    public partial class JQuery {
        public class Selector
        {
            protected Selector(string expression, Filter filter) {
                this.OrignalExpression = expression;
                this.Filter = filter;
            }
            public string OrignalExpression { get; private set; }

            public string Expression {
                get { return this.Filter.Expression; }
            }

            public override string ToString()
            {
                return "Yanyitec.JQuery.Selector, Expression=" + this.Expression;
            }

            public Filter Filter { get; protected internal set; }
            public IList<IHtmlElement> Apply(JQuery context) {
                var result = new List<IHtmlElement>();
                
                foreach (var root in context) {
                    if (Filter.Check(root, root)!=null) result.Add(root);
                    root.Walkthough((elem) => {
                        if (Filter.Check(elem, root)!=null) result.Add(root);return true;
                    });
                }
                return result;
            }

            public IList<IHtmlElement> Apply(IEnumerable<IHtmlElement> context)
            {
                var result = new List<IHtmlElement>();

                foreach (var root in context)
                {
                    if (Filter.Check(root, root) != null) result.Add(root);
                    root.Walkthough((elem) => {
                        if (Filter.Check(elem, root) != null) result.Add(root); return true;
                    });
                }
                return result;
            }

            public IList<IHtmlElement> Apply(IHtmlElement root)
            {
                var result = new List<IHtmlElement>();

                if (Filter.Check(root, root) != null) result.Add(root);
                    root.Walkthough((elem) => {
                        if (Filter.Check(elem, root) != null)
                            result.Add(elem); return true;
                    });
                return result;
            }

            static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Selector> ParsedSelectors = new System.Collections.Concurrent.ConcurrentDictionary<string, Selector>();

            public static Selector GetOrCreate(string expression) {
                return ParsedSelectors.GetOrAdd(expression,(exprstr)=>GenSelector(exprstr));
            }

            #region regex
            public static readonly string booleans = "checked|selected|async|autofocus|autoplay|controls|defer|disabled|hidden|ismap|loop|multiple|open|readonly|required|scoped";

            // Regular expressions

            // Whitespace characters http://www.w3.org/TR/css3-selectors/#whitespace
            public static readonly string whitespace = "[\\x20\\t\\r\\n\\f]";
            // http://www.w3.org/TR/css3-syntax/#characters
            public static readonly string characterEncoding = "(?:\\\\.|[\\w-]|[^\\x00-\\xa0])+";

            // Loosely modeled on CSS identifier characters
            // An unquoted value should be a CSS identifier http://www.w3.org/TR/css3-selectors/#attribute-selectors
            // Proper syntax: http://www.w3.org/TR/CSS21/syndata.html#value-def-identifier
            public static readonly string identifier = characterEncoding.Replace("w", "w#");

            // Attribute selectors: http://www.w3.org/TR/selectors/#attribute-selectors
            public static readonly string attributes = "\\[" + whitespace + "*(" + characterEncoding + ")(?:" + whitespace +
                // Operator (capture 2)
                "*([*^$|!~]?=)" + whitespace +
                // "Attribute values must be CSS identifiers [capture 5] or strings [capture 3 or capture 4]"
                "*(?:'((?:\\\\.|[^\\\\'])*)'|\"((?:\\\\.|[^\\\\\"])*)\"|(" + identifier + "))|)" + whitespace +
                "*\\]";

            public static readonly string pseudos = ":(" + characterEncoding + ")(?:\\((" +
                // To reduce the number of selectors needing tokenize in the preFilter, prefer arguments:
                // 1. quoted (capture 3; capture 4 or capture 5)
                "('((?:\\\\.|[^\\\\'])*)'|\"((?:\\\\.|[^\\\\\"])*)\")|" +
                // 2. simple (capture 6)
                "((?:\\\\.|[^\\\\()[\\]]|" + attributes + ")*)|" +
                // 3. anything else (capture 2)
                ".*" +
                ")\\)|)";

            // Leading and non-escaped trailing whitespace, capturing some non-whitespace characters preceding the latter
            public static readonly Regex rtrim = new Regex("^" + whitespace + "+|((?:^|[^\\\\])(?:\\\\.)*)" + whitespace + "+$", RegexOptions.Multiline);

            public static readonly Regex rcomma = new Regex("^" + whitespace + "*," + whitespace + "*");
            public static readonly Regex rcombinators = new Regex("^" + whitespace + "*([>+~]|" + whitespace + ")" + whitespace + "*");

            public static readonly Regex rattributeQuotes = new Regex("=" + whitespace + "*([^\\]'\"]*?)" + whitespace + "*\\]", RegexOptions.Multiline);

            public static readonly Regex rpseudo = new Regex(pseudos);
            public static readonly Regex ridentifier = new Regex("^" + identifier + "$");

            public static readonly Dictionary<string, Regex> matchExpr = new Dictionary<string, Regex>(){
                { "ID" , new Regex("^#(" + characterEncoding + ")") },
                { "CLASS" , new Regex("^\\.(" + characterEncoding + ")") } ,
                { "TAG" , new Regex("^(" + characterEncoding.Replace("w", "w*") + ")") } ,
                { "ATTR" , new Regex("^" + attributes) },
                { "PSEUDO", new Regex("^" + pseudos) },
                { "CHILD", new Regex("^:(only|first|last|nth|nth-last)-(child|of-type)(?:\\(" + whitespace +
                                        "*(even|odd|(([+-]|)(\\d*)n|)" + whitespace + "*(?:([+-]|)" + whitespace +
                                        "*(\\d+)|))" + whitespace + "*\\)|)", RegexOptions.IgnoreCase)},
                { "bool" , new Regex("^(?:" + booleans + ")$", RegexOptions.IgnoreCase) },
                // For use in libraries implementing .is()
                // We use this for POS matching in `select`
                {  "needsContext" , new Regex("^" + whitespace + "*[>+~]|:(even|odd|eq|gt|lt|nth|first|last)(?:\\(" +
                                                whitespace + "*((?:-\\d)?\\d*)" + whitespace + "*\\)|)(?=[^-]|$)", RegexOptions.IgnoreCase) }
            };
        
            static readonly Regex WhitespaceRegex = new Regex("\\s+", RegexOptions.Compiled);
            static readonly Regex ChildrenRegex = new Regex("\\s*>\\s*", RegexOptions.Compiled);
            #endregion

            #region generate selector
            public static Selector GenSelector(string expression) {
                //devive the group selector
                var exprs = expression.Split(',');
                if (exprs.Length == 1) {
                    return new Selector(expression,GenContainFilter(expression));
                }
                JQuery.GroupFilter groupFilter = new GroupFilter();
                foreach (var exprstr in exprs) {
                    var exprstr1 = exprstr.Trim();
                    if (exprstr1 == string.Empty) continue;
                    var filter = GenContainFilter(exprstr);
                    groupFilter.Filters.Add(filter);
                }
                if (groupFilter.Filters.Count == 0) throw new JQuery.SelectorExpressionException(expression);
                return new Selector(expression,groupFilter);
            }
            public static JQuery.Filter GenContainFilter(string expression) {
                var clearedExpr = ClearExpression(expression);
                //devive the contain selector
                var containExprs = clearedExpr.Split(' ');
                if (containExprs.Length == 1) return GenChildFilter(expression);
                //.ab div input
                var containFilter = new JQuery.DescendantFilter();
                foreach (var containExpr in containExprs)
                {
                    var childFilter = GenChildFilter(containExpr);
                    containFilter.AscendantFilters.Add(childFilter);
                }
                return containFilter;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <exception cref="NotSupportedException"></exception>
            /// <returns></returns>
            public static JQuery.Filter GenChildFilter(string expression) {
                var childExprs = expression.Split('>');
                if (childExprs.Length == 1) return GenMultFilter(expression);
                //.ab div input
                var childFilter = new JQuery.ChildFilter();
                foreach (var childExpr in childExprs)
                {
                    var finalFilter = GenMultFilter(childExpr);
                    childFilter.ParentFilters.Add(finalFilter);
                }
                return childFilter;
            }

            public static JQuery.Filter GenMultFilter(string expression)
            {
                //var startAt = 0;
                List<Filter> result = new List<Filter>();
                Match psuedoMatch = null;
                while (true) {
                    var hasMatches = false;
                    foreach (var pair in matchExpr) {

                        //var match = pair.Value.Match(expression, startAt);
                        var match = pair.Value.Match(expression);
                        if (match == null || !match.Success) continue;
                        //startAt += match.Length;
                        expression = expression.Substring(match.Length);
                        hasMatches = true;
                        switch (pair.Key) {
                            case "ID":
                                result.Add(new JQuery.IdFilter(match.Groups[1].Value));
                                break;
                            case "CLASS":
                                result.Add(new JQuery.ClassFilter(match.Groups[1].Value));
                                break;
                            case "TAG":
                                result.Add(new JQuery.TagFilter(match.Captures[0].Value));
                                break;
                            case "ATTR":
                                var attrName = match.Groups[1].Value;
                                string attrValue = null;
                                if (match.Groups.Count >=4) attrValue = match.Groups[3].Value;
                                result.Add(new JQuery.AttributeFilter(attrName,attrValue));
                                break;
                            case "PSEUDO":
                                psuedoMatch = match;
                                break;
                            default:
                                throw new NotSupportedException("");
                        }
                        break;
                    }
                    //所有都匹配完了，没有找到匹配，结束循环
                    if (!hasMatches) break;
                }
                Filter attrFilter = null;
                if (result.Count == 1) attrFilter = result[0];
                else attrFilter = new MultFilter(result);
                if (psuedoMatch != null) {
                    return GenPseudoFilter(psuedoMatch,attrFilter);
                }
                return attrFilter;
            }

            static JQuery.Filter GenPseudoFilter(Match match,JQuery.Filter attachedFilter) {
                var pseudoType = match.Groups[1].Value;
                switch (pseudoType) {
                    case "nth-child":return new NthChildFilter(attachedFilter, match.Groups[2].Value);
                    case "nth-of-type":return new NthOfTypeFilter(attachedFilter,match.Groups[2].Value);
                    default:
                        throw new NotImplementedException();
                }
            }
            #endregion

            static string ClearExpression(string expr)
            {
                expr = WhitespaceRegex.Replace(expr, " ");
                expr = ChildrenRegex.Replace(expr, ">");
                expr = expr.Trim();
                return expr;
            }
        }
    }
    
}
