namespace HLang.Parser.Ast
{
    public class AssignmentNode : AstNode
    {
        public AssignmentNode(Token.Token token, AstNode lhs, AstNode rhs) : base(NodeType.Assignment, token)
        {
            Left = lhs;
            Right = rhs;
        }

        public AstNode Left { get; }
        public AstNode Right { get; }
    }
}
