namespace HLang.Parser.Ast
{
    public class BitwiseOperatorNode : AstNode
    {
        public BitwiseOperatorNode(Token.Token token, AstNode left, AstNode right)
            : base(NodeType.BitwiseOperator, token)
        {
            Left = left;
            Right = right;
        }

        public AstNode Left { get; }
        public AstNode Right { get; }
    }
}
