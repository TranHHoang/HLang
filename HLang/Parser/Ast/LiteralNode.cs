using HLang.Visitor;

namespace HLang.Parser.Ast
{
    public class LiteralNode : AstNode
    {
        public LiteralNode(Token.Token token) : base(NodeType.Literal, token) { }

        public override object Accept(AstVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
