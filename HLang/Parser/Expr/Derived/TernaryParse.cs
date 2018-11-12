using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;

namespace HLang.Parser.Expr.Derived
{
    class TernaryParse : IInfix
    {
        public AstNode Parse(ExprParser parser, AstNode leftExpr, Token.Token token)
        {
            var truePart = parser.Parse(0);
            parser.Consume(Token.Token.TokenType.Colon);
            var falsePart = parser.Parse(0);

            return new TernaryNode(token, leftExpr, truePart, falsePart);
        }

        public int GetPrecedence()
        {
            return (int)Precedence.Conditional;
        }
    }
}
