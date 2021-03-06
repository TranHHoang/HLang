﻿using HLang.Parser.Ast;

namespace HLang.Parser.Expr.Derived
{
    public class LiteralParse : Base.IPrefix
    {
        public AstNode Parse(ExprParser parser, Token.Token token)
        {
            return new LiteralNode(token);
        }
    }
}
