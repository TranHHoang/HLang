using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;

namespace HLang.Parser.Expr.Derived
{
    class PrefixParse : IPrefix
    {
        public AstNode Parse(ExprParser parser, Token.Token token)
        {
            return new PrefixOperatorNode(token, parser.Parse((int)Precedence.Prefix));
        }
    }
}
