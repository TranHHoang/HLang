﻿using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;

namespace HLang.Parser.Expr.Derived
{
    class ComparisonParse : IInfix
    {
        private readonly Precedence _precedence;

        public ComparisonParse(Precedence precedence)
        {
            _precedence = precedence;
        }

        public AstNode Parse(ExprParser parser, AstNode leftExpr, Token.Token token)
        {
            return new ComparisonNode(token, leftExpr, parser.Parse(GetPrecedence()));
        }

        public int GetPrecedence()
        {
            return (int) _precedence;
        }
    }
}
