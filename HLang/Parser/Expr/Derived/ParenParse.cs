using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;

namespace HLang.Parser.Expr.Derived
{
    class ParenParse : IPrefix
    {
        public AstNode Parse(ExprParser parser, Token.Token token)
        {
            var expr = parser.Parse(0);
            parser.Consume(Token.Token.TokenType.RightParen);
            return expr;
        }
    }
}
