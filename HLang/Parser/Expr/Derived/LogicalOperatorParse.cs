using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;

namespace HLang.Parser.Expr.Derived
{
    public class LogicalOperatorParse : IInfix
    {
        private readonly Precedence _precedence;

        /// <summary>
        /// Construct new BinaryOperatorNode
        /// </summary>
        /// <param name="prec">Precedence of operator</param>
        public LogicalOperatorParse(Precedence prec)
        {
            _precedence = prec;
        }

        public AstNode Parse(ExprParser parser, AstNode leftExpr, Token.Token token)
        {
            return new LogicalOperatorNode(token, leftExpr, parser.Parse(GetPrecedence()));
        }

        public int GetPrecedence()
        {
            return (int) _precedence;
        }
    }
}
