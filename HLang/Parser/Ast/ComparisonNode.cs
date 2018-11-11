namespace HLang.Parser.Ast
{
    public class ComparisonNode : AstNode
    {
        public ComparisonNode(Token.Token token, AstNode left, AstNode right) : base(NodeType.Comparison, token)
        {
            Left = left;
            Right = right;
        }

        public AstNode Left { get; }
        public AstNode Right { get; }
    }
}
