namespace HLang.Parser.Ast
{
    public class BinaryOperatorNode : AstNode
    {
        public BinaryOperatorNode(Token.Token token, AstNode left, AstNode right)
            : base(NodeType.BinaryOperator, token)
        {
            Left = left;
            Right = right;
        }

        public AstNode Left { get; }
        public AstNode Right { get; }
    }
}
