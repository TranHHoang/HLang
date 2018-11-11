using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;

namespace HLang.Parser.Expr.Derived
{
    class AssignmentParse : IInfix
    {
        public AstNode Parse(ExprParser parser, AstNode leftExpr, Token.Token token)
        {
            return new AssignmentNode(token, leftExpr, parser.Parse(GetPrecedence() - 1)); // right associative
        }

        public int GetPrecedence()
        {
            return (int)Precedence.Assignment;
        }
    }
}
