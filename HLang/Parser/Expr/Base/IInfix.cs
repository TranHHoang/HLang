using HLang.Parser.Ast;

namespace HLang.Parser.Expr.Base
{
    public interface IInfix
    {
        AstNode Parse(Parser parser, AstNode leftExpr, Token.Token token);
        int GetPrecedence();
    }
}
