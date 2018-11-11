using HLang.Parser.Ast;

namespace HLang.Parser.Expr.Base
{
    public interface IPrefix
    {
        AstNode Parse(ExprParser parser, Token.Token token);
    }
}
