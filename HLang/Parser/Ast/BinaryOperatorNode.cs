using HLang.Visitor.AstVisitor;

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

        public override void Accept(AstVisitor astVisitor)
        {
        }

        public AstNode Left { get; }
        public AstNode Right { get; }
    }
}
