using HLang.Visitor.AstVisitor;

namespace HLang.Parser.Ast
{
    public class LiteralNode : AstNode
    {
        public LiteralNode(Token.Token token) : base(NodeType.Literal, token) { }

        public override void Accept(AstVisitor astVisitor)
        {
        }
    }
}
