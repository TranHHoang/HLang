using HLang.Visitor;

namespace HLang.Parser.Ast
{
    public class IdentifierNode : AstNode
    {
        public IdentifierNode(Token.Token token) : base(NodeType.Identifier, token) { }

        public override object Accept(AstVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
