using HLang.Visitor;

namespace HLang.Parser.Ast
{
    public class ComparisonNode : AstNode
    {
        public ComparisonNode(Token.Token token, AstNode left, AstNode right) : base(NodeType.Comparison, token)
        {
            Left = left;
            Right = right;
        }

        public override object Accept(AstVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Left { get; }
        public AstNode Right { get; }
    }
}
