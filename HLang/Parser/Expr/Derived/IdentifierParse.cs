using HLang.Parser.Ast;

namespace HLang.Parser.Expr.Derived
{
    class IdentifierParse : Base.IPrefix
    {
        public AstNode Parse(ExprParser parser, Token.Token token)
        {
            return new IdentifierNode(token);
        }
    }
}
