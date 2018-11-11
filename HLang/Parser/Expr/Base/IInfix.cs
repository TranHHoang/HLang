using HLang.Parser.Ast;

namespace HLang.Parser.Expr.Base
{
    public interface IInfix
    {
        AstNode Parse(ExprParser parser, AstNode leftExpr, Token.Token token);
        int GetPrecedence();
    }
}
