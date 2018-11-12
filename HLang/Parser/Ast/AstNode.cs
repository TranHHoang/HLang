namespace HLang.Parser.Ast
{
    public abstract class AstNode
    {
        protected AstNode(NodeType type, Token.Token token)
        {
            Type = type;
            Token = token;
        }
        /// <summary>
        /// Each token contains Line and Column will be helpful for error reporting
        /// </summary>
        public Token.Token Token { get; }
        /// <summary>
        /// Type of each node
        /// </summary>
        public NodeType Type { get; }

        public enum NodeType
        {
            Literal, Identifier,
            TernaryOperator, BinaryOperator, UnaryOperator, BitwiseOperator,
            Comparison,
            LogicalOperator,
            Assignment,
        }
    }
}
