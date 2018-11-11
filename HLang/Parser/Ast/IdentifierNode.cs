namespace HLang.Parser.Ast
{
    public class IdentifierNode : AstNode
    {
        public IdentifierNode(Token.Token token) : base(NodeType.Identifier, token) { }
    }
}
