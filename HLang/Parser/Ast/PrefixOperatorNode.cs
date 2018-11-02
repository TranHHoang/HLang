using HLang.Visitor;

namespace HLang.Parser.Ast
{
    public class PrefixOperatorNode : AstNode
    {
        public PrefixOperatorNode(Token.Token token, AstNode expr) : base(NodeType.UnaryOperator, token)
        {
            Expr = expr;
        }

        public override object Accept(AstVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Expr { get; }
    }
}
