using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;
using System;

namespace HLang.Parser.Expr.Derived
{
    class BitwiseOperatorParse : IInfix
    {
        private readonly Precedence _precedence;

        public BitwiseOperatorParse(Precedence precedence)
        {
            _precedence = precedence;
        }

        public AstNode Parse(ExprParser parser, AstNode leftExpr, Token.Token token)
        {
            return new BitwiseOperatorNode(token, leftExpr, parser.Parse(GetPrecedence()));
        }

        public int GetPrecedence()
        {
            return (int)_precedence;
        }
    }
}
