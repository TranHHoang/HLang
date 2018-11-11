using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;

namespace HLang.Parser.Expr.Derived
{
    class ComparisonParse : IInfix
    {
        public AstNode Parse(ExprParser parser, AstNode leftExpr, Token.Token token)
        {
            return new ComparisonNode(token, leftExpr, parser.Parse(GetPrecedence()));
        }

        public int GetPrecedence()
        {
            return (int) Precedence.Comparison;
        }
    }
}
