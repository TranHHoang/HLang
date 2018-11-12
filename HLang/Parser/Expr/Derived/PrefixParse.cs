using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;

namespace HLang.Parser.Expr.Derived
{
    class PrefixParse : IPrefix
    {
        private readonly Precedence _precedence;

        public PrefixParse(Precedence prec)
        {
            _precedence = prec;
        }

        public AstNode Parse(ExprParser parser, Token.Token token)
        {
            return new PrefixOperatorNode(token, parser.Parse((int)_precedence));
        }
    }
}
