using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;

namespace HLang.Parser.Expr.Derived
{
    class BinaryOperatorParse : IInfix
    {
        private readonly Precedence _precedence;
        private readonly bool _rightAssociative;

        /// <summary>
        /// Construct new BinaryOperatorNode
        /// </summary>
        /// <param name="prec">Precedence of operator</param>
        /// <param name="isRa">Is right associative</param>
        public BinaryOperatorParse(Precedence prec, bool isRa)
        {
            _precedence = prec;
            _rightAssociative = isRa;
        }

        public AstNode Parse(Parser parser, AstNode leftExpr, Token.Token token)
        {
            return new BinaryOperatorNode(token, leftExpr, parser.Parse((int)_precedence - (_rightAssociative ? 1 : 0)));
        }

        public int GetPrecedence()
        {
            return (int) _precedence;
        }
    }
}
