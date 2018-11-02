using HLang.Parser.Ast;

namespace HLang.Parser.Expr.Base
{
    public interface IPrefix
    {
        AstNode Parse(Parser parser, Token.Token token);
    }
}
