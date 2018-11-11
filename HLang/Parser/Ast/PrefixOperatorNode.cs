namespace HLang.Parser.Ast
{
    public class PrefixOperatorNode : AstNode
    {
        public PrefixOperatorNode(Token.Token token, AstNode expr) : base(NodeType.UnaryOperator, token)
        {
            Expr = expr;
        }

        public AstNode Expr { get; }
    }
}
