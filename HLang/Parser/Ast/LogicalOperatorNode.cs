namespace HLang.Parser.Ast
{
    public class LogicalOperatorNode : AstNode
    {
        /// <summary>
        /// Default constructor, use for AND and OR operator
        /// </summary>
        /// <param name="token"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public LogicalOperatorNode(Token.Token token, AstNode lhs, AstNode rhs) : base(NodeType.LogicalOperator, token)
        {
            Left = lhs;
            Right = rhs;
        }

        public AstNode Left { get; }
        public AstNode Right { get; }
    }
}
